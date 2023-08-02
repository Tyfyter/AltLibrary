using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.AltOres;
using AltLibrary.Common.Systems;
using AltLibrary.Core;
using AltLibrary.Core.Baking;
using AltLibrary.Core.UIs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Gamepad;
using static Humanizer.In;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common
{
	[Autoload(Side = ModSide.Client)]
	internal class UIWorldCreationEdits {
		internal static List<AltOre> AddInFinishedCreation;
		internal static List<AltBiome> AddInFinishedCreation2;
		internal static bool panelActive = false;
		internal static int AltEvilBiomeChosenType;
		internal static int AltHallowBiomeChosenType;
		internal static int AltJungleBiomeChosenType;
		internal static int AltHellBiomeChosenType;
		internal static List<ALUIBiomeListItem> _biomeElements;
		internal static FilterableUIList _altList;
		internal static List<ALUIOreListItem> _oreElements;
		internal static List<ALDrawingStruct<AltBiome>> QuenedDrawing2;
		internal static List<ALDrawingStruct<AltOre>> QuenedDrawing;
		internal static int Copper;
		internal static int Iron;
		internal static int Silver;
		internal static int Gold;
		internal static int Cobalt;
		internal static int Mythril;
		internal static int Adamantite;
		internal static bool isCrimson;
		internal static string seed;
		internal static bool initializedLists;
		internal enum CurrentAltOption
		{
			Biome,
			Ore
		}

		public static void Init()
		{
			if (Main.dedServ)
				return;

			_biomeElements = new();
			_oreElements = new();
			AddInFinishedCreation = new();
			AddInFinishedCreation2 = new();

			QuenedDrawing = new();
			QuenedDrawing2 = new();
			initializedLists = false;
			IL_UIWorldCreation.MakeInfoMenu += ILMakeInfoMenu;
			On_UIWorldCreation.AddWorldEvilOptions += OnAddWorldEvilOptions;
			On_UIWorldCreation.BuildPage += UIWorldCreation_BuildPage;
			IL_UIWorldCreation.Draw += UIWorldCreation_Draw;
			IL_UIWorldCreation.FinishCreatingWorld += UIWorldCreation_FinishCreatingWorld;
			IL_UIWorldCreationPreview.DrawSelf += UIWorldCreationPreview_DrawSelf1;
			On_UIWorldListItem.PlayGame += MakesWorldsUnplayable;
			IL_UIWorldCreation.SetupGamepadPoints += IL_UIWorldCreation_SetupGamepadPoints;
			On_UIWorldCreation.GetSnapGroup += On_UIWorldCreation_GetSnapGroup;
		}

		private static List<SnapPoint> On_UIWorldCreation_GetSnapGroup(On_UIWorldCreation.orig_GetSnapGroup orig, UIWorldCreation self, List<SnapPoint> ptsOnPage, string groupName) {
			if (groupName == "evil") groupName = "difficulty";
			return orig(self, ptsOnPage, groupName);
		}

		private static void IL_UIWorldCreation_SetupGamepadPoints(ILContext il) {
			ILCursor c = new(il);
			ILLabel skip = c.DefineLabel();
			int emptyGroupLocal = -1;
			int emptyArrayLocal = -1;
			c.GotoNext(MoveType.After,
				i => i.MatchLdstr("evil"),
				i => i.MatchCall<UIWorldCreation>("GetSnapGroup"),
				i => i.MatchStloc(out emptyGroupLocal)
			);

			c.GotoNext(MoveType.After,
				i => i.MatchLdloc(emptyGroupLocal),
				i => i.MatchCallvirt(out _),
				i => i.MatchNewarr<UILinkPoint>(),
				i => i.MatchStloc(out emptyArrayLocal)
			);
			//IL_0328: ldarg.0
			//IL_0329: ldloc.s 20
			//IL_032b: call instance void Terraria.GameContent.UI.States.UIWorldCreation::LoopHorizontalLineLinks(class Terraria.UI.Gamepad.UILinkPoint[])

			c.GotoNext(MoveType.Before,
				i => i.MatchLdarg(0),
				i => i.MatchLdloc(emptyArrayLocal),
				i => i.MatchCall(out _)
			);
			c.Emit(OpCodes.Br, skip);
			c.GotoNext(MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdloc(out _),
				i => i.MatchLdloc(emptyArrayLocal),
				i => i.MatchCall(out _)
			);
			c.MarkLabel(skip);
		}

		public static void Unload()
		{
			if (Main.netMode == NetmodeID.Server)
				return;

			panelActive = false;
			AltEvilBiomeChosenType = 0;
			AltHallowBiomeChosenType = 0;
			AltJungleBiomeChosenType = 0;
			AltHellBiomeChosenType = 0;
			_biomeElements = null;
			_altList = null;
			_oreElements = null;
			Copper = 0;
			Iron = 0;
			Silver = 0;
			Gold = 0;
			Cobalt = 0;
			Mythril = 0;
			Adamantite = 0;
			seed = null;
			initializedLists = false;
			QuenedDrawing = null;
			QuenedDrawing2 = null;
			AddInFinishedCreation = null;
			AddInFinishedCreation2 = null;
		}

		#region Useful stuff
		internal static void RandomizeValues()
		{
			List<int> evilBiomeTypes = new() { -333, -666 };
			AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Evil && x.Selectable).ToList().ForEach(x => evilBiomeTypes.Add(x.Type - 1));
			AltEvilBiomeChosenType = Main.rand.Next(evilBiomeTypes);
			isCrimson = AltEvilBiomeChosenType == -666;
			List<int> hallowBiomeTypes = new() { -3 };
			AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Hallow && x.Selectable).ToList().ForEach(x => hallowBiomeTypes.Add(x.Type - 1));
			AltHallowBiomeChosenType = Main.rand.Next(hallowBiomeTypes);
			List<int> hellBiomeTypes = new() { -5 };
			AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Hell && x.Selectable).ToList().ForEach(x => hellBiomeTypes.Add(x.Type - 1));
			AltHellBiomeChosenType = Main.rand.Next(hellBiomeTypes);
			List<int> jungleBiomeTypes = new() { -4 };
			AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Jungle && x.Selectable).ToList().ForEach(x => jungleBiomeTypes.Add(x.Type - 1));
			AltJungleBiomeChosenType = Main.rand.Next(jungleBiomeTypes);
			List<int> ores = new() { -1, -2 };
			AltLibrary.Ores.Where(x => x.OreType == OreType.Copper && x.Selectable).ToList().ForEach(x => ores.Add(x.Type));
			Copper = Main.rand.Next(ores);
			ores = new() { -3, -4 };
			AltLibrary.Ores.Where(x => x.OreType == OreType.Iron && x.Selectable).ToList().ForEach(x => ores.Add(x.Type));
			Iron = Main.rand.Next(ores);
			ores = new() { -5, -6 };
			AltLibrary.Ores.Where(x => x.OreType == OreType.Silver && x.Selectable).ToList().ForEach(x => ores.Add(x.Type));
			Silver = Main.rand.Next(ores);
			ores = new() { -7, -8 };
			AltLibrary.Ores.Where(x => x.OreType == OreType.Gold && x.Selectable).ToList().ForEach(x => ores.Add(x.Type));
			Gold = Main.rand.Next(ores);
			ores = new() { -9, -10 };
			AltLibrary.Ores.Where(x => x.OreType == OreType.Cobalt && x.Selectable).ToList().ForEach(x => ores.Add(x.Type));
			Cobalt = Main.rand.Next(ores);
			ores = new() { -11, -12 };
			AltLibrary.Ores.Where(x => x.OreType == OreType.Mythril && x.Selectable).ToList().ForEach(x => ores.Add(x.Type));
			Mythril = Main.rand.Next(ores);
			ores = new() { -13, -14 };
			AltLibrary.Ores.Where(x => x.OreType == OreType.Adamantite && x.Selectable).ToList().ForEach(x => ores.Add(x.Type));
			Adamantite = Main.rand.Next(ores);

			foreach (AltBiome ore in AltLibrary.Biomes)
			{
				ore.OnInitialize();
			}
			foreach (AltOre ore in AltLibrary.Ores)
			{
				ore.OnInitialize();
			}
		}

		internal static List<AltOre> MakeOreList()
		{
			List<AltOre> prehmList = new();
			prehmList.Clear();
			prehmList.Add(new RandomOptionOre("RandomCopper", OreType.Copper));
			prehmList.Add(new RandomOptionOre("RandomIron", OreType.Iron));
			prehmList.Add(new RandomOptionOre("RandomSilver", OreType.Silver));
			prehmList.Add(new RandomOptionOre("RandomGold", OreType.Gold));
			prehmList.Add(new RandomOptionOre("RandomCobalt", OreType.Cobalt));
			prehmList.Add(new RandomOptionOre("RandomMythril", OreType.Mythril));
			prehmList.Add(new RandomOptionOre("RandomAdamantite", OreType.Adamantite));
			prehmList.AddRange(ALWorldCreationLists.prehmOreData.Types);
			return prehmList;
		}

		internal static List<ALDrawingStruct<AltOre>> MakeQuenedDrawingList()
		{
			QuenedDrawing.Clear();
			QuenedDrawing.AddRange(ALWorldCreationLists.prehmOreData.Quenes);
			return QuenedDrawing;
		}

		internal static List<ALDrawingStruct<AltBiome>> MakeQuenedDrawingList2()
		{
			QuenedDrawing2.Clear();
			QuenedDrawing2.AddRange(ALWorldCreationLists.biomeData.Quenes);
			return QuenedDrawing2;
		}

		internal static List<List<AltBiome>> MakeBiomeList()
		{

			List<AltBiome> evils = new();
			evils.Add(new RandomOptionBiome("RandomEvilBiome", BiomeType.Evil));
			evils.Add(GetInstance<CorruptionAltBiome>());
			evils.Add(GetInstance<CrimsonAltBiome>());
			evils.AddRange(AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Evil && x.Selectable));

			List<AltBiome> hallows = new();
			if (AltLibrary.Biomes.Any(x => x.BiomeType == BiomeType.Hallow && x.Selectable)) {
				hallows.Add(new RandomOptionBiome("RandomHallowBiome", BiomeType.Hallow));
			}
			hallows.Add(GetInstance<HallowAltBiome>());
			hallows.AddRange(AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Hallow && x.Selectable));

			List<AltBiome> jungles = new();
			if (AltLibrary.Biomes.Any(x => x.BiomeType == BiomeType.Jungle && x.Selectable)) {
				jungles.Add(new RandomOptionBiome("RandomJungleBiome", BiomeType.Jungle));
			}
			jungles.Add(GetInstance<JungleAltBiome>());
			jungles.AddRange(AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Jungle && x.Selectable));

			List<AltBiome> hells = new();
			if (AltLibrary.Biomes.Any(x => x.BiomeType == BiomeType.Hell && x.Selectable)) {
				hells.Add(new RandomOptionBiome("RandomUnderworldBiome", BiomeType.Hell));
			}
			hells.Add(GetInstance<UnderworldAltBiome>());
			hells.AddRange(AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Hell && x.Selectable));

			List<List<AltBiome>> list = new();
			list.Add(evils);
			list.Add(hallows);
			list.Add(jungles);
			list.Add(hells);

			return list;
		}

		public static void UIWorldCreation_BuildPage(On_UIWorldCreation.orig_BuildPage orig, UIWorldCreation self)
		{
			if (!initializedLists)
			{
				ALWorldCreationLists.FillData();
				initializedLists = true;
			}
			panelActive = false;
			MakeQuenedDrawingList();
			MakeQuenedDrawingList2();

			RandomizeValues();

			orig(self);

			_oreElements.Clear();
			_biomeElements.Clear();
			_altList = null;

			#region UI List
			{
				UIElement uIElement3 = new()
				{
					Left = StyleDimension.FromPixels(Main.screenWidth - (Main.screenWidth - 100f))
				};
				uIElement3.Width.Set(0f, 0.8f);
				uIElement3.MaxWidth.Set(450, 0f);
				uIElement3.MinWidth.Set(350, 0f);
				uIElement3.Top.Set(150f, 0f);
				uIElement3.Height.Set(-150f, 1f);
				uIElement3.HAlign = 1f;
				uIElement3.OnUpdate += RUIElement3_OnUpdate;
				self.Append(uIElement3);

				UIPanel uIPanel = new();
				uIPanel.Width.Set(0f, 1f);
				uIPanel.Height.Set(-110f, 1f);
				uIPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
				uIPanel.PaddingTop = 0f;
				uIPanel.OnUpdate += JUIPanel_OnUpdate;
				uIElement3.Append(uIPanel);

				_altList = new FilterableUIList();
				_altList.Width.Set(25f, 1f);
				_altList.Height.Set(-50f, 1f);
				_altList.Top.Set(25f, 0f);
				_altList.ListPadding = 5f;
				_altList.HAlign = 1f;
				_altList.OnUpdate += M2oreList_OnUpdate;
				uIPanel.Append(_altList);

				UIScrollbar uIScrollbar = new();
				uIScrollbar.SetView(100f, 100f);
				uIScrollbar.Left = StyleDimension.FromPixels(Main.screenWidth - (Main.screenWidth - 75f));
				uIScrollbar.Height.Set(-250f, 1f);
				uIScrollbar.Top.Set(150f, 0f);
				uIScrollbar.HAlign = 1f;
				uIScrollbar.OnUpdate += GUIScrollbar_OnUpdate;
				self.Append(uIScrollbar);
				_altList.SetScrollbar(uIScrollbar);

				UIImageButton closeIcon = new(ALTextureAssets.ButtonClose);
				closeIcon.Width.Set(22, 0f);
				closeIcon.Height.Set(22, 0f);
				closeIcon.Top.Set(5, 0);
				closeIcon.Left.Set(5, 0);
				closeIcon.SetVisibility(1f, 1f);
				closeIcon.OnLeftClick += CloseIcon_OnClick;
				uIElement3.Append(closeIcon);

				List<AltOre> prehmList = MakeOreList();
				List<ALUIOreListItem> items = new();
				prehmList.ForEach(x => items.Add(new(x, false)));
				_altList.list.AddRange(items);

				List<AltBiome> biomeList = MakeBiomeList().Where(l => !AltLibraryConfig.Config.VanillaShowUpIfOnlyAltVarExist || l.Count > 1).SelectMany(l => l).ToList();
				foreach (AltBiome biome in AltLibrary.Biomes) {
					if (biome.BiomeType == BiomeType.None) {
						biome.CustomSelection(biomeList);
					}
				}
				List<ALUIBiomeListItem> biomeItems = new();
				biomeList.ForEach(x => biomeItems.Add(new(x, false)));
				_altList.list.AddRange(biomeItems);
				/*
				_oreList._items.AddRange(items);
				foreach (UIElement item in items)
				{
					((UIElement)ALReflection.UIList__innerList.GetValue(_oreList)).Append(item);
				}
				((UIElement)ALReflection.UIList__innerList.GetValue(_oreList)).Recalculate();
				*/
				_oreElements.AddRange(items);
				_biomeElements.AddRange(biomeItems);
			}
			#endregion
		}

		public static void UIWorldCreation_FinishCreatingWorld(ILContext il)
		{
			ILCursor c = new(il);
			if (!c.TryGotoNext(i => i.MatchRet()))
			{
				AltLibrary.Instance.Logger.Info("0 $ 1");
				return;
			}
			if (!c.TryGotoPrev(i => i.MatchLdnull()))
			{
				AltLibrary.Instance.Logger.Info("0 $ 2");
				return;
			}
			c.EmitDelegate<Action>(() =>
			{
				if (AltHallowBiomeChosenType <= -1)
				{
					WorldBiomeManager.WorldHallow = "";
				}
				else
				{
					WorldBiomeManager.WorldHallow = AltLibrary.Biomes[AltHallowBiomeChosenType].FullName;
				}
				if (AltEvilBiomeChosenType <= -1)
				{
					WorldBiomeManager.WorldEvil = "";
					WorldGen.WorldGenParam_Evil = isCrimson ? 1 : 0;
					WorldGen.crimson = isCrimson;
				}
				else
				{
					WorldBiomeManager.WorldEvil = AltLibrary.Biomes[AltEvilBiomeChosenType].FullName;
					WorldGen.WorldGenParam_Evil = 0;
					WorldGen.crimson = false;
				}
				if (AltJungleBiomeChosenType <= -1)
				{
					WorldBiomeManager.WorldJungle = "";
				}
				else
				{
					WorldBiomeManager.WorldJungle = AltLibrary.Biomes[AltJungleBiomeChosenType].FullName;
				}
				if (AltHellBiomeChosenType <= -1)
				{
					WorldBiomeManager.WorldHell = "";
				}
				else
				{
					WorldBiomeManager.WorldHell = AltLibrary.Biomes[AltHellBiomeChosenType].FullName;
				}
				WorldBiomeManager.Copper = Copper;
				WorldBiomeManager.Iron = Iron;
				WorldBiomeManager.Silver = Silver;
				WorldBiomeManager.Gold = Gold;
				WorldBiomeManager.Cobalt = Cobalt;
				WorldBiomeManager.Mythril = Mythril;
				WorldBiomeManager.Adamantite = Adamantite;
				foreach (AltBiome o in AddInFinishedCreation2)
					o.OnCreating();
				foreach (AltOre o in AddInFinishedCreation)
					o.OnCreating();

				AltLibrary.Instance.Logger.Info($"On creating world - Hallow: {AltHallowBiomeChosenType} Corrupt: {AltEvilBiomeChosenType} Jungle: {AltJungleBiomeChosenType} Underworld: {AltHellBiomeChosenType}");
			});
		}

		internal static void WorldCreationUIIcons(UIWorldCreationPreview self, SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = self.GetDimensions();
			Vector2 position = new(dimensions.X + 4f, dimensions.Y + 4f);
			Color color = Color.White;
			Rectangle mouseRectangle = Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);
			int y = 0;

			static bool DrawBiomeIcon(SpriteBatch spriteBatch, Vector2 position, Rectangle mouseRectangle, ref int y, Color color, BiomeType biomeType, Func<Asset<Texture2D>, Asset<Texture2D>> func, Func<Rectangle?> rect, Func<string> onHoverName, Func<string, string> onHoverMod, bool ignoreMouse) {
				bool hovered = false;
				Asset<Texture2D> asset = func(ALTextureAssets.BestiaryIcons);
				Rectangle? rectangle = null;
				if (rect() != null) rectangle = rect();
				ValueTuple<Asset<Texture2D>, Rectangle?> valueTuple = new(asset, rectangle);
				Texture2D buttonTexture = ALTextureAssets.Button.Value;
				Vector2 topLeft = new(position.X + 96f, position.Y + 26f * y);
				if (!ignoreMouse && mouseRectangle.Intersects(new Rectangle((int)topLeft.X, (int)topLeft.Y, buttonTexture.Width, buttonTexture.Height))) {
					string line1 = onHoverName();
					string line2 = $"{Language.GetTextValue("Mods.AltLibrary.AddedBy")} {onHoverMod("Terraria")}";
					string line = $"{line1}\n{line2}\n{Language.GetOrRegister("Mods.AltLibrary.ChooseBiome")}";
					Main.instance.MouseText(line);
					hovered = true;
					if (Main.mouseLeft && Main.mouseLeftRelease) {
						_altList.SetFilter(el => el is ALUIBiomeListItem biome && biome.biomeType == biomeType);
						panelActive = true;
					}
				}
				spriteBatch.Draw(ALTextureAssets.Button.Value, topLeft, (color * (hovered ? 1 : 0.75f)) with { A = 255 });
				spriteBatch.Draw(valueTuple.Item1.Value, topLeft + new Vector2(3f, 3f), valueTuple.Item2, color, 0f, new Vector2(0f, 0f), 0.5f, SpriteEffects.None, 0f);
					
				y++;
				return hovered;
			}

			static bool DrawOreIcon(SpriteBatch spriteBatch, Vector2 position, ref int y, Rectangle mouseRectangle, Color color, OreType oreType, Func<Asset<Texture2D>, Asset<Texture2D>> func, Func<Rectangle?> rect, Func<string> onHoverName, Func<string, string> onHoverMod, bool ignoreMouse) {
				bool hovered = false;
				Asset<Texture2D> asset = func(ALTextureAssets.OreIcons);
				Rectangle? rectangle = null;
				if (rect() != null) rectangle = rect();
				ValueTuple<Asset<Texture2D>, Rectangle?> valueTuple = new(asset, rectangle);
				Texture2D buttonTexture = ALTextureAssets.Button.Value;
				Vector2 topLeft = new(position.X + 96f, position.Y + 26f * y);
					
				Rectangle hitbox = new Rectangle((int)topLeft.X, (int)topLeft.Y, buttonTexture.Width, buttonTexture.Height);
				if (!ignoreMouse && mouseRectangle.Intersects(hitbox)) {
					string line1 = onHoverName();
					string line2 = $"{Language.GetTextValue("Mods.AltLibrary.AddedBy")} {onHoverMod("Terraria")}";
					string line = $"{line1}\n{line2}\n{Language.GetOrRegister("Mods.AltLibrary.ChooseOre")}";
					Main.instance.MouseText(line);
					hovered = true;
					if (Main.mouseLeft && Main.mouseLeftRelease) {
						_altList.SetFilter(el => el is ALUIOreListItem ore && ore.oreType == oreType);
						panelActive = true;
					}
				}

				spriteBatch.Draw(buttonTexture, topLeft, (color * (hovered ? 1 : 0.75f)) with { A = 255 });
				spriteBatch.Draw(valueTuple.Item1.Value, topLeft + new Vector2(3f, 3f), valueTuple.Item2, color, 0f, new Vector2(1f, 1f), 0.5f, SpriteEffects.None, 0f);
				y++;
				return hovered;
			}

			bool ignoreMouse = false;

			foreach (ALDrawingStruct<AltBiome> biome in QuenedDrawing2) {
				if(DrawBiomeIcon(spriteBatch, position, mouseRectangle, ref y, color, (BiomeType)biome.type, biome.func, biome.rect, biome.onHoverName, biome.onHoverMod, ignoreMouse)) {
					ignoreMouse = true;
				}
			}

			foreach (ALDrawingStruct<AltOre> ore in QuenedDrawing) {
				if(DrawOreIcon(spriteBatch, position, ref y, mouseRectangle, color, (OreType)ore.type, ore.func, ore.rect, ore.onHoverName, ore.onHoverMod, ignoreMouse)) {
					ignoreMouse = true;
				}
			}
		}
		#endregion

		#region Other stuff
		public static void ILMakeInfoMenu(ILContext il)
		{
			var c = new ILCursor(il);

			c.GotoNext(i => i.MatchLdstr("evil"))
				.GotoNext(i => i.MatchLdloc(1), i => i.MatchLdcR4(48f));

			ILLabel label = c.DefineLabel();
			c.Emit(OpCodes.Br, label)
				.GotoNext(i => i.MatchLdarg(0), i => i.MatchLdloc(0), i => i.MatchLdloc(1), i => i.MatchLdstr("desc"));

			c.MarkLabel(label);
		}

		private static void UIWorldCreationPreview_DrawSelf1(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel label = il.DefineLabel();

			if (!c.TryGotoNext(i => i.MatchLdarg(0),
				i => i.MatchLdfld<UIWorldCreationPreview>("_size"),
				i => i.MatchStloc(3),
				i => i.MatchLdloc(3),
				i => i.MatchSwitch(out _),
				i => i.MatchBr(out _)))
			{
				AltLibrary.Instance.Logger.Info("z $ 1");
				return;
			}

			c.Index++;
			c.Emit(OpCodes.Pop);
			c.Emit(OpCodes.Br, label);

			if (!c.TryGotoNext(i => i.MatchLdarg(0),
				i => i.MatchLdfld<UIWorldCreationPreview>("_difficulty"),
				i => i.MatchStloc(3),
				i => i.MatchLdloc(3),
				i => i.MatchSwitch(out _),
				i => i.MatchBr(out _)))
			{
				AltLibrary.Instance.Logger.Info("z $ 2");
				return;
			}

			c.MarkLabel(label);
			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldarg, 1);
			c.Emit(OpCodes.Ldloc, 1);
			c.Emit(OpCodes.Ldloc, 2);
			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldfld, typeof(UIWorldCreationPreview).GetField("_size", BindingFlags.NonPublic | BindingFlags.Instance));
			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldfld, typeof(UIWorldCreationPreview).GetField("_EvilCorruptionTexture", BindingFlags.NonPublic | BindingFlags.Instance));
			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldfld, typeof(UIWorldCreationPreview).GetField("_EvilCrimsonTexture", BindingFlags.NonPublic | BindingFlags.Instance));
			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldfld, typeof(UIWorldCreationPreview).GetField("_EvilRandomTexture", BindingFlags.NonPublic | BindingFlags.Instance));
			c.EmitDelegate<Action<UIWorldCreationPreview, SpriteBatch, Vector2, Color, byte, Asset<Texture2D>, Asset<Texture2D>, Asset<Texture2D>>>((self, spriteBatch, position, color, size, _EvilCorruptionTexture, _EvilCrimsonTexture, _EvilRandomTexture) =>
			{
				string folder = (seed != null ? seed.ToLower() : "") switch
				{
					"05162020" or "5162020" => "Drunk",
					"not the bees" or "not the bees!" => "NotTheBees",
					"for the worthy" => "ForTheWorthy",
					"celebrationmk10" or "05162011" or "5162011" or "05162021" or "5162021" => "Anniversary",
					"constant" or "theconstant" or "the constant" or "eye4aneye" or "eye4aneye" => "Constant",
					"don't dig up" or "dont dig up" or "dontdigup" => "Remix",
					"no traps" or "notraps" => "NoTraps",
					_ => "",
				};
				bool broken = false;
				if (AltLibrary.PreviewWorldIcons.Count > 0)
				{
					foreach (AltLibrary.CustomPreviews preview in AltLibrary.PreviewWorldIcons)
					{
						if ((seed != null ? seed.ToLower() : "").ToLower() == preview.seed.ToLower())
						{
							switch (size)
							{
								case 0:
								default:
									spriteBatch.Draw(ModContent.Request<Texture2D>(preview.pathSmall, AssetRequestMode.ImmediateLoad).Value, position, color);
									break;
								case 1:
									spriteBatch.Draw(ModContent.Request<Texture2D>(preview.pathMedium, AssetRequestMode.ImmediateLoad).Value, position, color);
									break;
								case 2:
									spriteBatch.Draw(ModContent.Request<Texture2D>(preview.pathLarge, AssetRequestMode.ImmediateLoad).Value, position, color);
									break;
							}
							broken = true;
						}
					}
				}
				if (!broken)
				{
					int style = AltLibraryConfig.Config.SpecialSeedWorldPreview ? (folder switch
					{
						"Drunk" => 1,
						"NotTheBees" => 2,
						"ForTheWorthy" => 3,
						"Anniversary" => 4,
						"Constant" => 5,
						"Remix" => 6,
						"NoTraps" => 7,
						_ => 0,
					}) : 0;
					spriteBatch.Draw(ALTextureAssets.PreviewSpecialSizes[style, size].Value, position, color);
				}
				Asset<Texture2D> asset = AltEvilBiomeChosenType switch
				{
					-333 => _EvilCorruptionTexture,
					-666 => _EvilCrimsonTexture,
					_ => _EvilRandomTexture,
				};
				if (AltEvilBiomeChosenType > -1)
				{
					asset = ALTextureAssets.BiomeIconLarge[AltEvilBiomeChosenType] ?? ALTextureAssets.NullPreview;
				}
				spriteBatch.Draw(asset.Value, position, color);
				WorldCreationUIIcons(self, spriteBatch);
			});
		}

		private static void UIWorldCreation_Draw(ILContext il)
		{
			ILCursor c = new(il);
			if (!c.TryGotoNext(i => i.MatchRet() && i.Offset != 0))
			{
				AltLibrary.Instance.Logger.Info("3 $ 1");
				return;
			}

			c.Emit(OpCodes.Ldarg, 0);
			c.EmitDelegate<Action<UIWorldCreation>>((self) =>
			{
				seed = (string)typeof(UIWorldCreation).GetField("_optionSeed", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self);
			});
		}

		private static void MakesWorldsUnplayable(Terraria.GameContent.UI.Elements.On_UIWorldListItem.orig_PlayGame orig, UIWorldListItem self, UIMouseEvent evt, UIElement listeningElement)
		{
			if ((WorldFileData)typeof(UIWorldListItem).GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self) == null)
				return;
			if (ALUtils.IsWorldValid(self))
			{
				orig(self, evt, listeningElement);
			}
		}

		private static void CloseIcon_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			panelActive = false;
		}

		public static void OnAddWorldEvilOptions(
			On_UIWorldCreation.orig_AddWorldEvilOptions orig,
			UIWorldCreation self, UIElement container,
			float accumualtedHeight,
			UIElement.MouseEvent clickEvent,
			string tagGroup, float usableWidthPercent)
		{
			FieldInfo _evilButtons = ALReflection.UIWorldCreation__evilButtons;
			var ctors = _evilButtons.FieldType.GetConstructors();
			var inst = ctors[0].Invoke(new object[] { 0 });
			_evilButtons.SetValue(self, inst);
		}
		#endregion

		//TODO: refactor
		#region useless shit that need to be refactored somehow
		private static void M2oreList_OnUpdate(UIElement affectedElement)
		{
			UIList element = affectedElement as UIList;
			if (panelActive)
			{
				element.Width.Set(25f, 1f);
				element.Height.Set(-50f, 1f);
				element.Top.Set(25f, 0f);
			}
			else
			{
				element.Width.Set(250000f, 1f);
				element.Height.Set(-500000f, 1f);
				element.Top.Set(25000000f, 0f);
			}
		}

		private static void GUIScrollbar_OnUpdate(UIElement affectedElement)
		{
			UIScrollbar scrollbar = affectedElement as UIScrollbar;
			if (panelActive)
			{
				scrollbar.Left = StyleDimension.FromPixels(-25f);
				scrollbar.Height.Set(-250f, 1f);
				scrollbar.Top.Set(150f, 0f);
			}
			else
			{
				scrollbar.Left = StyleDimension.FromPixels(75000000f);
				scrollbar.Height.Set(-250f, 1f);
				scrollbar.Top.Set(150000f, 0f);
			}
		}

		private static void JUIPanel_OnUpdate(UIElement affectedElement)
		{
			UIPanel element = affectedElement as UIPanel;
			if (panelActive)
			{
				element.Width.Set(0f, 1f);
				element.Height.Set(-110f, 1f);
			}
			else
			{
				element.Width.Set(0f, 1f);
				element.Height.Set(-110000000f, 1f);
			}
		}

		private static void RUIElement3_OnUpdate(UIElement affectedElement)
		{
			UIElement element = affectedElement;
			if (panelActive)
			{
				element.Left = StyleDimension.FromPixels(-50f);
				element.Width.Set(0f, 0.8f);
				element.MaxWidth.Set(450, 0f);
				element.MinWidth.Set(350, 0f);
				element.Top.Set(150f, 0f);
				element.Height.Set(-150f, 1f);
			}
			else
			{
				element.Left = StyleDimension.FromPixels(1000000f);
				element.Width.Set(0f, 0.8f);
				element.MaxWidth.Set(4500000, 0f);
				element.MinWidth.Set(3500000, 0f);
				element.Top.Set(150000000f, 0f);
				element.Height.Set(-150000000f, 1f);
			}
		}
		#endregion
	}

	public readonly struct ALDrawingStruct<T> where T : ModType
	{
		public readonly string UniqueID;
		internal readonly int type;
		internal readonly Func<Asset<Texture2D>, Asset<Texture2D>> func;
		internal readonly Func<Rectangle?> rect;
		internal readonly Func<string> onHoverName;
		internal readonly Func<string, string> onHoverMod;

		public ALDrawingStruct(string ID, int type, Func<Asset<Texture2D>, Asset<Texture2D>> func, Func<Rectangle?> rect, Func<string> onHoverName, Func<string, string> onHoverMod)
		{
			UniqueID = ID;
			this.type = type;
			this.func = func;
			this.rect = rect;
			this.onHoverName = onHoverName;
			this.onHoverMod = onHoverMod;
		}

		public ALDrawingStruct(ModType type, int enumType, Func<Asset<Texture2D>, Asset<Texture2D>> func, Func<Rectangle?> rect, Func<string> onHoverName, Func<string, string> onHoverMod)
		{
			UniqueID = type.FullName;
			this.type = enumType;
			this.func = func;
			this.rect = rect;
			this.onHoverName = onHoverName;
			this.onHoverMod = onHoverMod;
		}
	}
}
