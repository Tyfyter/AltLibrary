using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using MonoMod.Cil;
using System.Linq;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common.Hooks {
	internal class SimpleReplacements {
		internal static void Load() {
			IL_NPC.AttemptToConvertNPCToEvil += NPC_AttemptToConvertNPCToEvil;
			IL_NPC.CreateBrickBoxForWallOfFlesh += NPC_CreateBrickBoxForWallOfFlesh;
			On_WorldGen.nearbyChlorophyte += GoodDetourChloro;
			IL_WorldGen.GrowUndergroundTree += WorldGen_GrowUndergroundTree;
			Terraria.GameContent.Biomes.Desert.IL_DesertDescription.RowHasInvalidTiles += DesertDescription_RowHasInvalidTiles;
			IL_WorldGen.AddBuriedChest_int_int_int_bool_int_bool_ushort += WorldGen_AddBuriedChest_int_int_int_bool_int_bool_ushort;
		}

		internal static void Unload() {
		}

		//TODO: make alt jungle so I can test this
		private static bool GoodDetourChloro(Terraria.On_WorldGen.orig_nearbyChlorophyte orig, int i, int j) {
			bool ch = orig(i, j);
			foreach (AltBiome b in AltLibrary.Biomes.Where(g => g.BiomeType == BiomeType.Jungle)) {
				float num = 0f;
				int num2 = 5;
				if (i <= num2 + 5 || i >= Main.maxTilesX - num2 - 5) {
					continue;
				}
				if (j <= num2 + 5 || j >= Main.maxTilesY - num2 - 5) {
					continue;
				}
				for (int k = i - num2; k <= i + num2; k++) {
					for (int l = j - num2; l <= j + num2; l++) {
						if (Main.tile[k, l].HasTile && (Main.tile[k, l].TileType == (b.BiomeOre ?? 211) || Main.tile[k, l].TileType == (b.BiomeOreBrick ?? 346))) {
							num += 1f;
							if (num >= 4f) {
								ch |= true;
								continue;
							}
						}
					}
				}
				ch |= num > 0f && WorldGen.genRand.Next(5) < num;
			}
			return ch;
		}

		private static void NPC_AttemptToConvertNPCToEvil(ILContext il) {
			ALUtils.ReplaceIDs(il,
				NPCID.CorruptBunny,
				(orig) => (short)(WorldBiomeManager.WorldEvilBiome.BloodBunny ?? orig),
				(orig) => WorldBiomeManager.WorldEvilName != "" && WorldBiomeManager.WorldEvilBiome.BloodBunny.HasValue);
			ALUtils.ReplaceIDs(il,
				NPCID.CorruptGoldfish,
				(orig) => (short)(WorldBiomeManager.WorldEvilBiome.BloodGoldfish ?? orig),
				(orig) => WorldBiomeManager.WorldEvilName != "" && WorldBiomeManager.WorldEvilBiome.BloodGoldfish.HasValue);
			ALUtils.ReplaceIDs(il,
				NPCID.CorruptPenguin,
				(orig) => (short)(WorldBiomeManager.WorldEvilBiome.BloodPenguin ?? orig),
				(orig) => WorldBiomeManager.WorldEvilName != "" && WorldBiomeManager.WorldEvilBiome.BloodPenguin.HasValue);
		}

		private static void NPC_CreateBrickBoxForWallOfFlesh(ILContext il) {
			ALUtils.ReplaceIDs(il,
				TileID.DemoniteBrick,
				(orig) => (ushort)(WorldBiomeManager.WorldEvilBiome.BiomeOreBrick ?? orig),
				(orig) => WorldBiomeManager.WorldEvilName != "" && WorldBiomeManager.WorldEvilBiome.BiomeOreBrick.HasValue);
		}

		//TODO: fix when declarative chest loot
		private static void WorldGen_AddBuriedChest_int_int_int_bool_int_bool_ushort(ILContext il) {
			ALUtils.ReplaceIDs<int>(il,
				ItemID.ShadowKey,
				(orig) => WorldBiomeManager.WorldHell?.ShadowKeyAlt ?? orig,
				(orig) => (WorldBiomeManager.WorldHell?.ShadowKeyAlt).HasValue);
		}
		//TODO: make alt jungle so I can test this
		private static void WorldGen_GrowUndergroundTree(ILContext il) {
			ALUtils.ReplaceIDs<int>(il, TileID.JungleGrass,
				(orig) => WorldBiomeManager.WorldJungle?.BiomeGrass ?? orig,
				(orig) => (WorldBiomeManager.WorldJungle?.BiomeGrass).HasValue);
		}
		//TODO: make alt jungle so I can test this
		private static void DesertDescription_RowHasInvalidTiles(ILContext il) {
			ALUtils.ReplaceIDs<int>(il, TileID.JungleGrass,
				(orig) => WorldBiomeManager.WorldJungle?.BiomeGrass ?? orig,
				(orig) => (WorldBiomeManager.WorldJungle?.BiomeGrass).HasValue);
			ALUtils.ReplaceIDs<int>(il, TileID.Mud,
				(orig) => WorldBiomeManager.WorldJungle?.BiomeMud ?? orig,
				(orig) => (WorldBiomeManager.WorldJungle?.BiomeMud).HasValue);
		}
	}
}
