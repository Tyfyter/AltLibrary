using AltLibrary.Common.AltBiomes;
using AltLibrary.Core;
using AltLibrary.Core.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PegasusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AltLibrary.Common.Hooks {
	//TODO: completely redesign and rewrite this (other than the icons in the top right), maybe even go out of your way to chastise whoever wrote it
	//but first find a better way
	internal static class WorldIcons {
		internal static int WarnUpdate = 0;

		public static void Init() {
			IL_UIWorldListItem.ctor += UIWorldListItem_ctor;
			IL_UIWorldListItem.DrawSelf += UIWorldListItem_DrawSelf1;
			On_UIWorldListItem.DrawSelf += UIWorldListItem_DrawSelf;
			On_AWorldListItem.GetIcon += UIWorldListItem_GetIcon;
			WarnUpdate = 0;
		}

		public static void Unload() {
			WarnUpdate = 0;
		}
		private static Asset<Texture2D> UIWorldListItem_GetIcon(On_AWorldListItem.orig_GetIcon orig, AWorldListItem self) {
			Asset<Texture2D> asset = orig(self);
			if (asset.Height() == 58) {
				if (asset.Width() == 59) {
					return ALTextureAssets.UIWorldSeedIcon[1];
				} else if (asset.Width() == 60) {
					return ALTextureAssets.UIWorldSeedIcon[0];
				}
			}
			return asset;
		}

		private static void UIWorldListItem_DrawSelf1(ILContext il) {
			ILCursor c = new(il);
			if (!c.TryGotoNext(i => i.MatchCall<Color>("get_White"))) {
				AltLibrary.Instance.Logger.Error("s $ 1");
				return;
			}

			c.Index++;
			c.Emit(OpCodes.Ldarg, 0);
			c.EmitDelegate<Func<Color, UIWorldListItem, Color>>((value, self) => {
				return ALUtils.IsWorldValid(self) ? value : Color.MediumPurple;
			});
		}

		private static void UIWorldListItem_ctor(ILContext il) {
			ILCursor c = new(il);
			FieldReference canBePlayed = null;
			if (!c.TryGotoNext(i => i.MatchLdarg(3),
				i => i.MatchStfld(out canBePlayed))) {
				AltLibrary.Instance.Logger.Error("t $ 1");
				return;
			}

			c.Index += 2;
			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldarg, 3);
			c.EmitDelegate<Func<UIWorldListItem, bool, bool>>((self, canPlay) => {
				return ALUtils.IsWorldValid(self) && canPlay;
			});
			c.Emit(OpCodes.Stfld, canBePlayed);

			if (!c.TryGotoNext(i => i.MatchLdftn(out _))) {
				AltLibrary.Instance.Logger.Error("t $ 2");
				return;
			}
			FieldReference fieldReference = null;
			if (!c.TryGotoNext(i => i.MatchLdfld(out fieldReference))) {
				AltLibrary.Instance.Logger.Error("t $ 3");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchCall(out _))) {
				AltLibrary.Instance.Logger.Error("t $ 4");
				return;
			}
			c.Index++;
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldfld, fieldReference);
			c.EmitDelegate<Action<UIWorldListItem, UIImage>>((uiWorldListItem, _worldIcon) => {
				WorldFileData data = uiWorldListItem.Data;
				ALUtils.GetWorldData(uiWorldListItem, out Dictionary<string, AltLibraryConfig.WorldDataValues> tempDict, out string path2);

				//TODO: zenith icon
				if (!AltLibraryConfig.Config.ZenithIconJank && data.DrunkWorld && data.RemixWorld) return;
				ReplaceIcons(data, ref _worldIcon);
				LayeredIcons("Normal", data, ref _worldIcon, tempDict, path2);
				LayeredIcons(data, ref _worldIcon, tempDict, path2);
				LayeredIcons("ForTheWorthy", data, ref _worldIcon, tempDict, path2);
				LayeredIcons("NotTheBees", data, ref _worldIcon, tempDict, path2);
				LayeredIcons("Anniversary", data, ref _worldIcon, tempDict, path2);
				LayeredIcons("DontStarve", data, ref _worldIcon, tempDict, path2);
				LayeredIcons("RemixWorld", data, ref _worldIcon, tempDict, path2);
			});
		}

		/// <summary>
		/// For drunk icons
		/// </summary>
		/// <param name="data"></param>
		/// <param name="image"></param>
		/// <param name="tempDict"></param>
		/// <param name="path2"></param>
		private static void LayeredIcons(WorldFileData data, ref UIImage image, Dictionary<string, AltLibraryConfig.WorldDataValues> tempDict, string path2) {
			Dictionary<string, Func<WorldFileData, bool>> assets = new();
			bool bl2 = data.DrunkWorld && tempDict.ContainsKey(path2);
			if (data.HasCrimson && bl2 && tempDict[path2].worldEvil == "" && ModContent.RequestIfExists("AltLibrary/Assets/WorldIcons/DrunkBase/Crimson", out Asset<Texture2D> newAsset2)) {
				UIImage worldIcon = new(newAsset2);
				worldIcon.Height.Set(0, 1);
				worldIcon.Width.Set(0, 1);
				image.Append(worldIcon);
			}
			if (data.HasCorruption && bl2 && tempDict[path2].worldEvil == "" && ModContent.RequestIfExists("AltLibrary/Assets/WorldIcons/DrunkBase/Corruption", out Asset<Texture2D> newAsset3)) {
				UIImage worldIcon = new(newAsset3);
				worldIcon.Height.Set(0, 1);
				worldIcon.Width.Set(0, 1);
				image.Append(worldIcon);
			}
			if (data.HasCorruption && bl2 && tempDict[path2].worldEvil != "" && ModContent.TryFind(tempDict[path2].worldEvil, out AltBiome altBiome) && ModContent.RequestIfExists(altBiome?.WorldIcon + "DrunkBase", out Asset<Texture2D> newAsset1)) {
				UIImage worldIcon = new(newAsset1);
				worldIcon.Height.Set(0, 1);
				worldIcon.Width.Set(0, 1);
				image.Append(worldIcon);
			}

			assets.Add("Corrupt", (ourData) => {
				return ourData.DrunkWorld && tempDict.ContainsKey(path2) && tempDict[path2].drunkEvil == "Terraria/Corruption";
			});
			assets.Add("Crimson", (ourData) => {
				return ourData.DrunkWorld && tempDict.ContainsKey(path2) && tempDict[path2].drunkEvil == "Terraria/Crimson";
			});
			foreach (AltBiome biome in AltLibrary.Biomes) {
				if (biome.BiomeType == BiomeType.Evil) {
					assets.Add(biome.FullName, (ourData) => {
						return ourData.DrunkWorld && tempDict.ContainsKey(path2) && tempDict[path2].drunkEvil == biome.FullName;
					});
				}
			}
			assets.Add("Hallow", (ourData) => {
				return ourData.DrunkWorld && ourData.IsHardMode && tempDict.ContainsKey(path2) && tempDict[path2].worldHallow == "";
			});
			foreach (AltBiome biomes in AltLibrary.Biomes) {
				if (!biomes.FullName.StartsWith("Terraria/") && biomes.BiomeType != BiomeType.Evil) {
					AltBiome biome = ModContent.Find<AltBiome>(biomes.FullName);
					if (biome is not null && tempDict.ContainsKey(path2)) {
						assets.Add(biome.FullName, new Func<WorldFileData, bool>((ourData) => {
							string equals = biome.BiomeType switch {
								BiomeType.Hallow => tempDict[path2].worldHallow,
								BiomeType.Jungle => tempDict[path2].worldJungle,
								BiomeType.Hell => tempDict[path2].worldHell,
								_ => "",
							};
							bool bl = ourData.DrunkWorld && equals == biome.FullName;
							if (biome.BiomeType == BiomeType.Hallow) bl &= ourData.IsHardMode;
							if (biome.BiomeType == BiomeType.Hell) bl &= false;
							return bl;
						}));
					}
				}
			}

			bool bl = data.DrunkWorld;
			foreach (KeyValuePair<string, Func<WorldFileData, bool>> entry in assets) {
				string path = $"AltLibrary/Assets/WorldIcons/Drunk/{entry.Key.Replace("Terraria/", "")}";
				if (entry.Key != "Corrupt" && entry.Key != "Crimson" && entry.Key != "Hallow") {
					path = ModContent.Find<AltBiome>(entry.Key).WorldIcon + "Drunk";
				}
				if (bl && entry.Value.Invoke(data) && ModContent.RequestIfExists(path, out Asset<Texture2D> newAsset)) {
					UIImage worldIcon = new(newAsset);
					worldIcon.Height.Set(0, 1);
					worldIcon.Width.Set(0, 1);
					image.Append(worldIcon);
				}
			}
		}
		static FastFieldInfo<UIImageButton, Asset<Texture2D>> _texture = "_texture";
		private static void UIWorldListItem_DrawSelf(On_UIWorldListItem.orig_DrawSelf orig, UIWorldListItem self, SpriteBatch spriteBatch) {
			orig(self, spriteBatch);
			if (++WarnUpdate >= 120) {
				WarnUpdate = 0;
			}
			if ((WorldFileData)typeof(UIWorldListItem).GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self) == null)
				return;
			ALUtils.GetWorldData(self, out Dictionary<string, AltLibraryConfig.WorldDataValues> tempDict, out string path2);
			UIElement _worldIcon = ALReflection.UIWorldListItem__worldIcon.GetValue(self);

			WorldFileData _data = self.Data;
			CalculatedStyle innerDimensions = self.GetInnerDimensions();
			CalculatedStyle dimensions = _worldIcon.GetDimensions();
			float num7 = innerDimensions.X + innerDimensions.Width;
			Rectangle mouseRectangle = Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);
			float warningOffset = 0;
			if (tempDict.TryGetValue(path2, out AltLibraryConfig.WorldDataValues worldData)) {
				foreach (UIElement item in self.Children) {
					if (item is UIImageButton button && _texture.GetValue(button) == UICommon.ButtonErrorTexture) {
						float pixels = item.Left.Pixels - item.Width.Pixels;
						if (warningOffset > pixels) warningOffset = pixels;
					}
				}
				for (int i = 0; i < 4; i++) {
					Asset<Texture2D> asset = ALTextureAssets.BestiaryIcons;
					Rectangle? frame = null;
					AltBiome biome;
					string text = "";
					switch (i) {
						case 0:
						if (worldData.worldHallow == "") worldData.worldHallow = "AltLibrary/HallowBiome";
						if (ModContent.TryFind(worldData.worldHallow, out biome)) {
							if (biome is VanillaBiome) {
								frame = new(30, 30, 30, 30);
							} else {
								asset = ModContent.Request<Texture2D>(biome.IconSmall ?? "AltLibrary/Assets/Menu/ButtonHallow");
							}
							text = biome.DisplayName.Value;
						} else {
							asset = ALTextureAssets.ButtonHallow;
							text = worldData.worldHallow;
						}
						break;
						case 1:
						if (worldData.worldEvil == "") worldData.worldEvil = _data.HasCorruption ? "AltLibrary/CorruptBiome" : "AltLibrary/CrimsonBiome";
						if (ModContent.TryFind(worldData.worldEvil, out biome)) {
							if (biome is VanillaBiome) {
								frame = new(_data.HasCorruption ? 210 : 360, 0, 30, 30);
							} else {
								asset = ModContent.Request<Texture2D>(biome.IconSmall ?? "AltLibrary/Assets/Menu/ButtonCorrupt");
							}
							text = biome.DisplayName.Value;
						} else {
							asset = ALTextureAssets.ButtonCorrupt;
							text = worldData.worldEvil;
						}
						break;
						case 2:
						if (worldData.worldHell == "") worldData.worldHell = "AltLibrary/UnderworldBiome";
						if (ModContent.TryFind(worldData.worldHell, out biome)) {
							if (biome is VanillaBiome) {
								frame = new(30, 60, 30, 30);
							} else {
								asset = ModContent.Request<Texture2D>(biome.IconSmall ?? "AltLibrary/Assets/Menu/ButtonHell");
							}
							text = biome.DisplayName.Value;
						} else {
							asset = ALTextureAssets.ButtonHell;
							text = worldData.worldHell;
						}
						break;
						case 3:
						if (worldData.worldJungle == "") worldData.worldJungle = "AltLibrary/JungleBiome";
						if (ModContent.TryFind(worldData.worldJungle, out biome)) {
							if (biome is VanillaBiome) {
								frame = new(180, 30, 30, 30);
							} else {
								asset = ModContent.Request<Texture2D>(biome.IconSmall ?? "AltLibrary/Assets/Menu/ButtonJungle");
							}
							text = biome.DisplayName.Value;
						} else {
							asset = ALTextureAssets.ButtonJungle;
							text = worldData.worldJungle;
						}
						break;
					}
					ValueTuple<Asset<Texture2D>, Rectangle?> valueTuple = new(asset, frame);
					spriteBatch.Draw(ALTextureAssets.Button.Value, new Vector2(num7 - 26f * (i + 1) + warningOffset, dimensions.Y - 2f), Color.White);
					spriteBatch.Draw(valueTuple.Item1.Value, new Vector2(num7 - 26f * (i + 1) + 3f + warningOffset, dimensions.Y + 1f), valueTuple.Item2, Color.White, 0f, new Vector2(0f, 0f), 0.5f, SpriteEffects.None, 0f);
					if (mouseRectangle.Intersects(new Rectangle((int)(num7 - 26f * (i + 1) + warningOffset), (int)(dimensions.Y - 2f), 22, 22))) {
						Main.instance.MouseText(text);
					}
				}
			}

			if (!ALUtils.IsWorldValid(self)) {
				Asset<Texture2D> asset = ALTextureAssets.ButtonWarn;
				int num = WarnUpdate % 120;
				int num2 = num < 60 ? 0 : 1;
				Rectangle rectangle = new(num2 * 22, 0, 22, 22);
				spriteBatch.Draw(asset.Value, new Vector2(num7 - 26f * 5 + warningOffset, dimensions.Y - 2f), rectangle, Color.White);
				Vector2 vector2 = new(num7 - 26f * 5 + warningOffset, dimensions.Y - 2f);
				if (mouseRectangle.Intersects(Utils.CenteredRectangle(vector2 + new Vector2(11f, 11f), Utils.Size(new Rectangle(0, 0, 22, 22))))) {
					string line = Language.GetTextValue("Mods.AltLibrary.WorldBreak");
					Main.instance.MouseText(line);
				}
			}
		}

		private static void LayeredIcons(string forWhich, WorldFileData data, ref UIImage image, Dictionary<string, AltLibraryConfig.WorldDataValues> tempDict, string path2) {
			Dictionary<string, Func<WorldFileData, bool>> assets = new()
			{
				{ "Corruption", new Func<WorldFileData, bool>((ourData) => ourData.HasCorruption && tempDict.ContainsKey(path2) && tempDict[path2].worldEvil is "" or "") },
				{ "Crimson", new Func<WorldFileData, bool>((ourData) => ourData.HasCrimson && tempDict.ContainsKey(path2) && tempDict[path2].worldEvil is "" or "") },
				{ "Hallow", new Func<WorldFileData, bool>((ourData) => ourData.IsHardMode && tempDict.ContainsKey(path2) && tempDict[path2].worldHallow is "" or "") }
			};
			foreach (AltBiome biome in AltLibrary.Biomes) {
				if (!biome.FullName.StartsWith("Terraria/")) {
					if (biome is not null && tempDict.TryGetValue(path2, out AltLibraryConfig.WorldDataValues value)) {
						assets.Add(biome.FullName, new Func<WorldFileData, bool>((ourData) => {
							string equals = biome.BiomeType switch {
								BiomeType.Hallow => value.worldHallow,
								BiomeType.Hell => value.worldHell,
								BiomeType.Jungle => value.worldJungle,
								BiomeType.Evil => value.worldEvil,
								_ => "",
							};
							bool bl = equals == biome.FullName;
							if (biome.BiomeType == BiomeType.Hallow) bl &= ourData.IsHardMode;
							if (biome.BiomeType == BiomeType.Hell) bl &= false;
							return bl;
						}));
					}
				}
			}
			bool bl;
			if (forWhich != "Normal") {
				bl = (bool)typeof(WorldFileData).GetField($"{(forWhich == "Drunk" ? "DrunkWorld" : forWhich)}").GetValue(data);
			} else {
				bl = !data.Anniversary && !data.DontStarve && !data.NotTheBees && !data.ForTheWorthy && !data.DrunkWorld && !data.RemixWorld && !data.ZenithWorld;
			}
			foreach (KeyValuePair<string, Func<WorldFileData, bool>> entry in assets) {
				if (!entry.Value.Invoke(data)) continue;
				string path;
				switch (entry.Key) {
					default:
					string biomePath = ModContent.Find<AltBiome>(entry.Key).WorldIcon;
					if (string.IsNullOrWhiteSpace(biomePath)) continue;
					path = biomePath + forWhich;
					break;
					case "Corruption":
					case "Crimson":
					case "Hallow":
					path = $"AltLibrary/Assets/WorldIcons/{entry.Key}/{forWhich}";
					break;
				}
				if (bl && ModContent.RequestIfExists(path, out Asset<Texture2D> newAsset)) {
					UIImage worldIcon = new(newAsset);
					worldIcon.Height.Set(0, 1);
					worldIcon.Width.Set(0, 1);
					image.Append(worldIcon);
				}
			}
		}

		private static void ReplaceIcons(WorldFileData data, ref UIImage image) {
			Asset<Texture2D> asset = ALTextureAssets.WorldIconNormal;
			if (data.DrunkWorld) {
				asset = ALTextureAssets.WorldIconDrunk;
				if (data.HasCrimson) {
					asset = ALTextureAssets.WorldIconDrunkCrimson;
				}
				if (data.HasCorruption) {
					asset = ALTextureAssets.WorldIconDrunkCorrupt;
				}
			}
			if (data.ForTheWorthy) {
				asset = ALTextureAssets.WorldIconForTheWorthy;
			}
			if (data.NotTheBees) {
				asset = ALTextureAssets.WorldIconNotTheBees;
			}
			if (data.Anniversary) {
				asset = ALTextureAssets.WorldIconAnniversary;
			}
			if (data.DontStarve) {
				asset = ALTextureAssets.WorldIconDontStarve;
			}
			if (data.RemixWorld) {
				asset = ALTextureAssets.WorldIconRemix;
			}
			UIImage worldIcon = new(asset);
			worldIcon.Height.Set(0, 1);
			worldIcon.Width.Set(0, 1);
			image.Append(worldIcon);
		}
	}
}
