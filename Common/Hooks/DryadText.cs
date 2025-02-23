using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using AltLibrary.Core.Baking;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace AltLibrary.Common.Hooks {
	internal class DryadText {
		public static void Init() {
			On_Lang.GetDryadWorldStatusDialog += Lang_GetDryadWorldStatusDialog;
			IL_WorldGen.AddUpAlignmentCounts += WorldGen_AddUpAlignmentCounts;
			On_WorldGen.CountTiles += On_WorldGen_CountTiles;
		}

		private static void On_WorldGen_CountTiles(On_WorldGen.orig_CountTiles orig, int X) {
			if (X == 0) {
				Utils.Swap(ref WorldBiomeManager.biomeCountsWorking, ref WorldBiomeManager.biomeCountsFinished);
				WorldBiomeManager.biomeCountsWorking.Clear();
				WorldBiomeManager.biomePercents.Clear();
				foreach (KeyValuePair<int, int> kvp in WorldBiomeManager.biomeCountsFinished) {
					if (kvp.Value > 0) {
						WorldBiomeManager.biomePercents.Add(kvp.Key, (byte)Math.Max(Math.Round((kvp.Value * 100.0) / WorldGen.totalSolid2), 1));
					}
				}
			}
			orig(X);
		}

		private static string Lang_GetDryadWorldStatusDialog(On_Lang.orig_GetDryadWorldStatusDialog orig, out bool worldIsEntirelyPure) {
			int tGood = 0;
			int tEvil = 0;
			worldIsEntirelyPure = true;
			List<(AltBiome biome, byte percent)> hallows = [];
			List<(AltBiome biome, byte percent)> evils = [];
			foreach (KeyValuePair<int, byte> _biome in WorldBiomeManager.biomePercents) {
				AltBiome biome = AltLibrary.GetAltBiome(_biome.Key);
				if (biome is not null) {
					switch (biome.BiomeType) {
						case BiomeType.Evil:
						evils.Add((biome, _biome.Value));
						tEvil += _biome.Value;
						worldIsEntirelyPure = false;
						break;
						case BiomeType.Hallow:
						hallows.Add((biome, _biome.Value));
						tGood += _biome.Value;
						worldIsEntirelyPure = false;
						break;
					}
				}
			}
			if (worldIsEntirelyPure) return Language.GetTextValue("DryadSpecialText.WorldStatusPure", Main.worldName);

			List<(AltBiome biome, byte percent)> biomes = [
				..hallows,
				..evils
			];
			StringBuilder builder = new();
			string statusPart = Language.GetTextValue("Mods.AltLibrary.DryadSpecialText.WorldStatusPart");
			string statusConnection = Language.GetTextValue("Mods.AltLibrary.DryadSpecialText.WorldStatusConnection");
			string statusConnectionFinal = Language.GetTextValue("Mods.AltLibrary.DryadSpecialText.WorldStatusLastConnection");
			for (int i = 0; i < biomes.Count; i++) {
				if (i > 0) {
					builder.Append(i == biomes.Count - 1 ? statusConnectionFinal : statusConnection);
				}
				builder.Append(string.Format(statusPart, biomes[i].percent, biomes[i].biome.DryadTextDescriptor.Value));
			}
			string statuses = builder.ToString();
			builder.Clear();
			builder.Append(string.Format(Language.GetTextValue("Mods.AltLibrary.DryadSpecialText.WorldStatus"), Main.worldName, statuses));
			builder.Append(' ');
			if (tGood * 1.2 >= tEvil && tGood * 0.8 <= tEvil) {
				builder.Append(Language.GetTextValue("DryadSpecialText.WorldDescriptionBalanced"));
			} else if (tGood > tEvil) {
				builder.Append(Language.GetTextValue("DryadSpecialText.WorldDescriptionFairyTale"));
			} else if (tEvil > tGood + 20) {
				builder.Append(Language.GetTextValue("DryadSpecialText.WorldDescriptionGrim"));
			} else if (tEvil <= 5) {
				builder.Append(Language.GetTextValue("DryadSpecialText.WorldDescriptionClose"));
			} else {
				builder.Append(Language.GetTextValue("DryadSpecialText.WorldDescriptionWork"));
			}
			return builder.ToString();
		}

		private static void WorldGen_AddUpAlignmentCounts(ILContext il) {
			ILCursor c = new(il);
			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.tileCounts)),
				i => i.MatchLdcI4(0),
				i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.tileCounts)),
				i => i.MatchLdlen(),
				i => i.MatchConvI4(),
				i => i.MatchCall<Array>(nameof(Array.Clear))
				)) {
				AltLibrary.Instance.Logger.Error($"Could not find {nameof(WorldGen.tileCounts)}.{nameof(Array.Clear)} in {nameof(WorldGen.AddUpAlignmentCounts)}");
				return;
			}

			c.EmitDelegate(() => {
				for (int i = 0; i < WorldGen.tileCounts.Length; i++) {
					if (WorldGen.tileCounts[i] > 0 && ALConvertInheritanceData.tileParentageData.TryGetParent(i, out (int baseTile, AltBiome fromBiome) parent) && parent.fromBiome is not null) {
						if (parent.fromBiome is DeconvertAltBiome) continue;
						WorldBiomeManager.biomeCountsWorking.TryGetValue(parent.fromBiome.Type, out int alreadyCounted);
						WorldBiomeManager.biomeCountsWorking[parent.fromBiome.Type] = alreadyCounted + WorldGen.tileCounts[i];
						WorldGen.totalSolid2 += WorldGen.tileCounts[i];
					}
				}
			});
		}
		public static void Unload() { }
	}
}
