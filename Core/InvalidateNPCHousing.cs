using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria;
using Terraria.ModLoader;
using AltLibrary.Common.AltBiomes;

namespace AltLibrary.Core {
	public class InvalidateNPCHousing : ILoadable {
		public static HashSet<int> NPCTypeIgnoresAllEvil { get; private set; } = [];
		public static Dictionary<int, HashSet<AltBiome>> NPCTypeIgnoresSpecificBiome { get; private set; } = [];
		int npcTypeScoringRoom;
		public void Load(Mod mod) {
			On_WorldGen.ScoreRoom += (On_WorldGen.orig_ScoreRoom orig, int ignoreNPC, int npcTypeAskingToScoreRoom) => {
				npcTypeScoringRoom = npcTypeAskingToScoreRoom;
				orig(ignoreNPC, npcTypeAskingToScoreRoom);
			};
			On_WorldGen.GetTileTypeCountByCategory += (On_WorldGen.orig_GetTileTypeCountByCategory orig, int[] tileTypeCounts, TileScanGroup group) => {
				if (group == TileScanGroup.TotalGoodEvil) {
					int moddedEvil = 0;
					int moddedGood = 0;
					if (!NPCTypeIgnoresSpecificBiome.TryGetValue(npcTypeScoringRoom, out HashSet<AltBiome> ignoreBiomes)) ignoreBiomes = [];
					for (int i = 0; i < AltLibrary.Biomes.Count; i++) {
						AltBiome biome = AltLibrary.Biomes[i];
						if (ignoreBiomes.Contains(biome)) continue;
						IReadOnlyList<int> tileTypes = biome.SpreadingTiles;
						switch (biome.BiomeType) {
							case BiomeType.Evil:
							if (NPCTypeIgnoresAllEvil.Contains(npcTypeScoringRoom)) break;
							for (int j = 0; j < tileTypes.Count; j++) {
								moddedEvil += tileTypeCounts[tileTypes[j]];
							}
							break;
							case BiomeType.Hallow:
							for (int j = 0; j < tileTypes.Count; j++) {
								moddedGood += tileTypeCounts[tileTypes[j]];
							}
							break;
						}
					}

					return orig(tileTypeCounts, group) + moddedGood - moddedEvil;
				}
				return orig(tileTypeCounts, group);
			};
		}

		public void Unload() {
			NPCTypeIgnoresAllEvil = null;
			NPCTypeIgnoresSpecificBiome = null;
		}
	}
}
