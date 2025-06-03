using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System.Reflection;
using AltLibrary.Common.AltBiomes;
using MonoMod.Cil;
using PegasusLib;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using AltLibrary.Common.Systems;
using AltLibrary.Common.AltOres;
using Terraria.ID;
using AltLibrary.Core.UIs;
using Terraria.WorldBuilding;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Humanizer;
using Terraria.ModLoader.UI;

namespace AltLibrary.Common {
	public class NewWorldCreationMenu : ILoadable {
		public static FieldInfo _evilButtons;
		public static FastFieldInfo<GroupOptionButton<AltBiome>, Asset<Texture2D>> _iconTexture;
		public static FastFieldInfo<GroupOptionButton<AltBiome>, UIText> _title;
		public static FastFieldInfo<UIWorldCreation, UIText> _descriptionText;
		public static FastFieldInfo<UIText, Vector2> _textSize;
		public static FastFieldInfo<UIText, float> _textScale;
		public static AltBiome[] randomBiomes;
		public static AltBiome[] selectedBiomes;
		public static List<GroupOptionButton<AltBiome>>[] biomeButtons;
		public static List<OreDropdown> oreButtons;

		public static HashSet<AltBiome> extraBiomes = [];
		public static(BiomeType type, AltBiome[] biomes)[] selectableBiomes;
		public static(OreSlot slot, AltOre[] ores)[] selectableOres;
		internal static void Init() {
			On_UIWorldCreation.SetupGamepadPoints += On_UIWorldCreation_SetupGamepadPoints;
			IL_UIWorldCreation.MakeInfoMenu += IL_UIWorldCreation_MakeInfoMenu;
			IL_UIWorldCreation.FinishCreatingWorld += IL_UIWorldCreation_FinishCreatingWorld;
			_evilButtons = typeof(UIWorldCreation).GetField("_evilButtons", BindingFlags.NonPublic | BindingFlags.Instance);
			_iconTexture = new(nameof(_iconTexture), BindingFlags.NonPublic);
			_title = new(nameof(_title), BindingFlags.NonPublic);
			_descriptionText = new(nameof(_descriptionText), BindingFlags.NonPublic);
			_textSize = new(nameof(_textSize), BindingFlags.NonPublic);
			_textScale = new(nameof(_textScale), BindingFlags.NonPublic);
			randomBiomes = [..Enum.GetValues<BiomeType>().Where(v => v != BiomeType.None).Select(type => new RandomOptionBiome($"Random{type}", type))];
		}
		public static void SetupWorldCreationData() {
			for (int i = 0; i < selectedBiomes.Length; i++) {
				if (selectedBiomes[i] is RandomOptionBiome) selectedBiomes[i] = Main.rand.Next(selectableBiomes[i].biomes);
			}
			WorldBiomeManager.WorldEvilBiome = selectedBiomes[(int)BiomeType.Evil];
			bool isCrimson = WorldBiomeManager.WorldEvilBiome is CrimsonAltBiome;
			WorldGen.WorldGenParam_Evil = isCrimson ? 1 : 0;
			WorldGen.crimson = isCrimson;

			WorldBiomeManager.WorldHallowBiome = selectedBiomes[(int)BiomeType.Hallow];
			//TODO: fix name-based system
			WorldBiomeManager.WorldHell = selectedBiomes[(int)BiomeType.Hell];
			WorldBiomeManager.WorldJungle = selectedBiomes[(int)BiomeType.Jungle];
			//WorldBiomeManager.WorldHellBiome = selectedBiomes[(int)BiomeType.Hell];
			//WorldBiomeManager.WorldJungleBiome = selectedBiomes[(int)BiomeType.Jungle];
			foreach (AltBiome o in extraBiomes) o.OnCreating();
			WorldBiomeManager.ores = new AltOre[OreSlotLoader.OreSlotCount];
			foreach (OreDropdown ore in oreButtons) ore.SetOre();
			for (int i = 0; i < WorldBiomeManager.ores.Length; i++) {
				if (WorldBiomeManager.ores[i] is null) {
					WorldBiomeManager.ores[i] = OreSlotLoader.GetOres(i).First();
				}
			}
			/*foreach (AltOre o in AddInFinishedCreation)
				o.OnCreating();*/
		}
		private static void IL_UIWorldCreation_FinishCreatingWorld(ILContext il) {
			ILCursor c = new(il);
			c.GotoNext(MoveType.Before,
				i => i.MatchLdnull(),
				i => i.MatchCall<WorldGen>(nameof(WorldGen.CreateNewWorld))
			);
			c.EmitDelegate(SetupWorldCreationData);
		}
		private static void IL_UIWorldCreation_MakeInfoMenu(ILContext il) {
			ILCursor c = new(il);
			c.GotoNext(MoveType.Before, i => i.MatchCall<UIWorldCreation>("AddWorldEvilOptions"));
			c.Next.Operand = il.Import(typeof(NewWorldCreationMenu).GetMethod(nameof(On_UIWorldCreation_AddWorldEvilOptions)));
			int loc = -1;
			c.GotoPrev(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdloc0(), i => i.MatchLdloc(out loc));
			c.EmitPop();
			c.EmitLdloca(loc);
		}
		public static void On_UIWorldCreation_AddWorldEvilOptions(UIWorldCreation self, UIElement container, ref float accumualtedHeight, UIElement.MouseEvent clickEvent, string tagGroup, float usableWidthPercent) {
			selectedBiomes = [..randomBiomes];
			biomeButtons = new List<GroupOptionButton<AltBiome>>[selectedBiomes.Length + 1];

			for (int i = 0; i < biomeButtons.Length; i++) biomeButtons[i] = [];
			selectableBiomes = [
				..((IEnumerable<AltBiome>)AltLibrary.AllBiomes)
				.Where(biome => biome.Selectable)
				.GroupBy(biome => biome.BiomeType)
				.OrderBy(group => group.Key)
				.Select<IGrouping<BiomeType, AltBiome>, (BiomeType, AltBiome[])>(group => (group.Key, [..group]))
			];
			selectableOres = new (OreSlot slot, AltOre[] ores)[OreSlotLoader.OreSlotCount];
			for (int i = 0; i < OreSlotLoader.OreSlotCount; i++) {
				OreSlot oreSlot = OreSlotLoader.GetOreSlot(i);
				selectableOres[i] = (oreSlot, OreSlotLoader.GetOres(oreSlot).ToArray());
			}
			int maxButtonCountPerRow = 3;
			void LengthenPanel(float height, ref float accumualtedHeight) {
				accumualtedHeight += height;
				container.Parent.Parent.Height.Pixels += height;
				container.Parent.Parent.Parent.Height.Pixels += height;
			}
			bool addedButtons = false;
			for (int j = 0; j < selectableBiomes.Length; j++) {
				int buttonCount = selectableBiomes[j].biomes.Length;
				if (buttonCount <= (selectableBiomes[j].type == BiomeType.None ? 0 : 1)) continue;
				if (addedButtons) {
					LengthenPanel(48, ref accumualtedHeight);
					UIHorizontalSeparator element = new() {
						Width = StyleDimension.FromPercent(1f),
						Top = StyleDimension.FromPixels(accumualtedHeight - 8f),
						Color = Color.Lerp(Color.White, new Color(63, 65, 151, 255), 0.85f) * 0.9f
					};
					container.Append(element);
				}
				addedButtons = true;
				int remainingButtonCount = buttonCount + 1;
				int buttonCountPerRow = Math.Min(remainingButtonCount, maxButtonCountPerRow);
				int hIndex = 0;
				void AddButton(AltBiome biome, ref float accumualtedHeight, int index) {
					GroupOptionButton<AltBiome> groupOptionButton = new(biome, biome.DisplayName, biome.Description, biome.NameColor, null, 1f, 1f, 16f);
					UIText title = _title.GetValue(groupOptionButton);
					title.DynamicallyScaleDownToWidth = false;
					title.OnInternalTextChange += () => {
						title.MinWidth.Set(0, 0);
						float num = 1;
						float width = title.Width.GetValue(title.Parent.GetInnerDimensions().Width) - (title.PaddingLeft + title.PaddingRight);
						float size = _textSize.GetValue(title).X * 1.2f;
						if (size > width) {
							num *= width / size;
							title.TextOriginX = num * 0.5f - .5f;
						}
						_textScale.SetValue(title, num);
					};
					/*title.Append(new UIPanel() {
						BackgroundColor = new Color(100, 0, 0, 100),
						Width = StyleDimension.FromPercent(1),
						Height = StyleDimension.FromPercent(1),
					});*/
					if (biome?.IconSmall is not null) _iconTexture.SetValue(groupOptionButton, ModContent.Request<Texture2D>(biome.IconSmall));
					groupOptionButton.Width = StyleDimension.FromPixelsAndPercent(-4 * buttonCountPerRow, 1f / buttonCountPerRow * usableWidthPercent);
					groupOptionButton.Left = StyleDimension.FromPercent(1f - usableWidthPercent);
					if (buttonCountPerRow == 1) {
						groupOptionButton.HAlign = 0.5f;
					} else {
						groupOptionButton.HAlign = hIndex / (float)(buttonCountPerRow - 1);
					}
					groupOptionButton.Top.Set(accumualtedHeight, 0f);
					//groupOptionButton.OnLeftMouseDown += clickEvent;
					if (biome.BiomeType == BiomeType.None) {
						groupOptionButton.OnLeftMouseDown += (_, _) => {
							if (!extraBiomes.Add(biome)) extraBiomes.Remove(biome);
							RefreshSelectionVisuals();
						};
					} else {
						groupOptionButton.OnLeftMouseDown += (_, _) => {
							selectedBiomes[(int)biome.BiomeType] = biome;
							RefreshSelectionVisuals();
						};
					}
					groupOptionButton.OnMouseOver += (_, _) => _descriptionText.GetValue(self).SetText(biome.Description);
					groupOptionButton.OnMouseOut += (_, _) => _descriptionText.GetValue(self).SetText(Language.GetText("UI.WorldDescriptionDefault"));
					groupOptionButton.SetSnapPoint(selectableBiomes[j].type.ToString(), index);
					container.Append(groupOptionButton);
					hIndex++;
					if (hIndex == buttonCountPerRow) {
						if (buttonCountPerRow == maxButtonCountPerRow && index < buttonCount) {
							remainingButtonCount -= buttonCountPerRow;
							if (buttonCountPerRow > remainingButtonCount) buttonCountPerRow = remainingButtonCount;
							LengthenPanel(40, ref accumualtedHeight);
						}
						hIndex = 0;
					}
					biomeButtons[j].Add(groupOptionButton);
				}
				if (selectableBiomes[j].type == BiomeType.None) {
					remainingButtonCount = buttonCount;
					buttonCountPerRow = Math.Min(remainingButtonCount, maxButtonCountPerRow);
				} else {
					AddButton(randomBiomes[j], ref accumualtedHeight, 0);
				}
				for (int i = 0; i < buttonCount; i++) {
					AddButton(selectableBiomes[j].biomes[i], ref accumualtedHeight, i + 1);
				}
			}
			_evilButtons.SetValue(self, Array.CreateInstance(_evilButtons.FieldType.GetElementType(), 0));
			RefreshSelectionVisuals();
			oreButtons = [];
			int oreHeight = 0;
			for (int i = 0; i < selectableOres.Length; i++) {
				OreDropdown dropdown = new(selectableOres[i].ores, selectableOres[i].slot) {
					Left = StyleDimension.FromPixelsAndPercent(22, 1),
					Top = StyleDimension.FromPixels(oreHeight)
				};
				oreButtons.Add(dropdown);
				container.Append(dropdown);
				oreHeight += 28;
			}
		}
		internal static void RefreshSelectionVisuals() {
			for (int i = 0; i < selectedBiomes.Length; i++) {
				foreach (GroupOptionButton<AltBiome> button in biomeButtons[i]) {
					button.SetCurrentOption(selectedBiomes[i]);
				}
			}
			foreach (GroupOptionButton<AltBiome> button in biomeButtons[(int)BiomeType.None]) {
				button.SetCurrentOption(extraBiomes.Contains(button.OptionValue) ? button.OptionValue : null);
			}
		}
		private static void On_UIWorldCreation_SetupGamepadPoints(On_UIWorldCreation.orig_SetupGamepadPoints orig, UIWorldCreation self, SpriteBatch spriteBatch) {
			
		}
		public void Load(Mod mod) {}
		public void Unload() {
			foreach (FieldInfo field in GetType().GetFields(BindingFlags.DeclaredOnly)) {
				if (field.IsStatic && field.FieldType.IsClass) field.SetValue(null, null);
			}
		}
	}
	public class OreDropdown : UIElement {
		readonly AltOre[] ores;
		readonly OreSlot oreSlot;
		public AltOre selectedOre;
		bool open = false;
		Asset<Texture2D> randomTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/IconEvilRandom");
		public void SetOre() {
			if (selectedOre is RandomOptionOre) {
				selectedOre = ores[Main.rand.Next(1, ores.Length)];
			}
			WorldBiomeManager.GetAltOre(oreSlot) = selectedOre;
		}
		public OreDropdown(IEnumerable<AltOre> ores, OreSlot oreSlot) {
			selectedOre = new RandomOptionOre(oreSlot);
			this.ores = [selectedOre, ..ores];
			this.oreSlot = oreSlot;
			Width.Set(22, 0);
			Height.Set(22, 0);
		}
		public override void Draw(SpriteBatch spriteBatch) {
			Vector2 pos = this.GetDimensions().ToRectangle().BottomLeft();
			bool wasOpen = open;
			void DrawIcon(AltOre ore) {
				bool hovered = Main.MouseScreen.Between(pos, pos + Vector2.One * 22);
				if (Main.mouseLeft && Main.mouseLeftRelease) {
					if (hovered) {
						selectedOre = ore;
						open = !wasOpen;
					} else {
						open = false;
					}
				}
				spriteBatch.Draw(ALTextureAssets.Button.Value, pos, (Color.White * (hovered ? 1 : 0.75f)) with { A = 255 });
				Main.instance.LoadTiles(ore.ore);
				if (ore is RandomOptionOre) {
					spriteBatch.Draw(randomTexture.Value, pos + new Vector2(11f), null, Color.White, 0f, randomTexture.Size() * 0.5f, 0.75f, SpriteEffects.None, 0f);
				} else {
					spriteBatch.Draw(TextureAssets.Tile[ore.ore].Value, pos + new Vector2(11f), new Rectangle(162, 54, 16, 16), Color.White, 0f, new Vector2(8f), 1, SpriteEffects.None, 0f);
				}
				if (hovered) {
					UICommon.TooltipMouseText(ore.DisplayName.Value);
				}
				pos.X += 22;
			}
			DrawIcon(selectedOre);
			if (wasOpen) {
				for (int i = 0; i < ores.Length; i++) {
					DrawIcon(ores[i]);
				}
			}
		}
	}
}
