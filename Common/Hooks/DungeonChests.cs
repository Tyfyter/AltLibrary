using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AltLibrary.Common.Hooks {
	internal class DungeonChests {

		public static void Init() {
			IL_WorldGen.MakeDungeon += WorldGen_MakeDungeon;
		}

		public static void Unload() {}
		static void MakeDungeonChests() {
			List<(int chestTileType, int contain, int style2)> biomeChests = new();
			static bool GetBiomeChest(AltBiome biome, out (int chestTileType, int contain, int style2) chestData) {
				if (biome is not null && biome.BiomeChestTile.HasValue && biome.BiomeChestItem.HasValue) {
					chestData = (biome.BiomeChestTile.Value, biome.BiomeChestItem.Value, biome.BiomeChestTileStyle.GetValueOrDefault());
					return true;
				}
				chestData = (0, 0, 0);
				return false;
			}
			if (ModContent.TryFind(WorldBiomeManager.WorldJungle, out AltBiome altJungle) && GetBiomeChest(altJungle, out var chestData)) {
				biomeChests.Add(chestData);
			} else {
				biomeChests.Add((TileID.Containers, ItemID.PiranhaGun, 23));
			}

			AltBiome altEvil = WorldBiomeManager.GetWorldEvil(false);
			if (altEvil is not null && GetBiomeChest(altEvil, out chestData)) {
				biomeChests.Add(chestData);
			} else {
				biomeChests.Add(WorldGen.crimson ? (TileID.Containers, ItemID.VampireKnives, 25) : (TileID.Containers, ItemID.ScourgeoftheCorruptor, 24));
			}

			if (WorldBiomeManager.GetWorldHallow(false) is AltBiome altHallow && GetBiomeChest(altHallow, out chestData)) {
				biomeChests.Add(chestData);
			} else {
				biomeChests.Add((TileID.Containers, ItemID.RainbowGun, 26));
			}

			biomeChests.Add((TileID.Containers, ItemID.StaffoftheFrostHydra, 27));
			biomeChests.Add((TileID.Containers2, ItemID.StormTigerStaff, 13));

			if (GetBiomeChest(WorldBiomeManager.GetWorldHell(true), out chestData)) {
				biomeChests.Add(chestData);
			}

			if (WorldGen.drunkWorldGen || ModSupport.FargoSeeds.BothEvils()) {
				if (altEvil is null) {
					biomeChests.Add(!WorldGen.crimson ? (TileID.Containers, ItemID.VampireKnives, 25) : (TileID.Containers, ItemID.ScourgeoftheCorruptor, 24));
				} else {
					biomeChests.Add((TileID.Containers, ItemID.ScourgeoftheCorruptor, 24));
					biomeChests.Add((TileID.Containers, ItemID.VampireKnives, 25));
				}
				for (int i = 0; i < AltLibrary.Biomes.Count; i++) {
					AltBiome currentDrunkBiome = AltLibrary.Biomes[i];
					if (currentDrunkBiome.BiomeType == BiomeType.Evil && currentDrunkBiome.Type != (altEvil?.Type ?? -1) && GetBiomeChest(currentDrunkBiome, out chestData)) {
						biomeChests.Add(chestData);
					}
				}
			}
			for (int i = 0; i < biomeChests.Count; i++) {
				bool placed = false;
				while (!placed) {
					int x = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
					int y = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.dMaxY);
					if (!Main.wallDungeon[Main.tile[x, y].WallType] || Main.tile[x, y].HasTile) {
						continue;
					}
					(int chestTileType, int contain, int style2) = biomeChests[i];
					placed = WorldGen.AddBuriedChest(x, y, contain, notNearOtherChests: false, style2, trySlope: false, (ushort)chestTileType);
				}
			}
		}
		private static void WorldGen_MakeDungeon(ILContext il) {
			//IL_213b: ldc.i4.5
			//IL_213c: stloc.s 15
			// if (drunkWorldGen)
			//IL_213e: ldsfld bool Terraria.WorldGen::drunkWorldGen
			//IL_2143: brfalse.s IL_2148

			// num79 = 6;
			//IL_2145: ldc.i4.6
			//IL_2146: stloc.s 15

			ILCursor c = new(il);

			int biomeChestCount = -1;
			if (c.TryGotoNext(MoveType.Before,
				i => i.MatchLdcI4(5),
				i => i.MatchStloc(out biomeChestCount),
				i => i.MatchLdsfld<WorldGen>("drunkWorldGen"),
				i => i.MatchBrfalse(out _),
				i => i.MatchLdcI4(6),
				i => i.MatchStloc(biomeChestCount)
			)) {
				c.RemoveRange(6);
				c.EmitDelegate(MakeDungeonChests);
				c.Emit(OpCodes.Ldc_I4_0);
				c.Emit(OpCodes.Stloc, biomeChestCount);
			}
		}
	}
}
