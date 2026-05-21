using AltLibrary.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace AltLibrary.Core.Generation {
	internal class CrimsonEvilBiomeGenerationPass : EvilBiomeGenerationPass {
		public override string ProgressMessage => Lang.gen[72].Value;
		public override int DrunkRNGMapCenterGive => 100;
		public override void GenerateEvil(int evilBiomePosition, int evilBiomePositionWestBound, int evilBiomePositionEastBound) {
			WorldGen.CrimStart(evilBiomePosition, (int)GenVars.worldSurfaceLow - 10);
			double maxSurfaceDepth = Main.worldSurface + 40.0;
			int j = (int)GenVars.worldSurfaceLow;
			for (int i = evilBiomePositionWestBound; i < evilBiomePositionEastBound; i++) {
				maxSurfaceDepth += WorldGen.genRand.Next(-2, 3);
				Max(ref maxSurfaceDepth, Main.worldSurface + 30.0);
				Min(ref maxSurfaceDepth, Main.worldSurface + 50.0);
				bool highestTile = true;
				while (j < maxSurfaceDepth) {
					Tile tile = Main.tile[i, j];
					if (tile.HasTile) {
						if (j < Main.worldSurface - 1.0 && highestTile) {
							if (tile.TileType == TileID.Dirt) {
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(i, j, TileID.Dirt, TileID.CrimsonGrass, true);
							} else if (tile.TileType == TileID.Mud) {
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(i, j, TileID.Mud, TileID.CrimsonJungleGrass);
							}
						}
						highestTile = false;
						WorldGen.Convert(i, j, BiomeConversionID.Crimson,
							tiles: tile.TileType is not TileID.Stone and not TileID.Sand || (i >= evilBiomePositionWestBound + WorldGen.genRand.Next(5) && i <= evilBiomePositionEastBound - WorldGen.genRand.Next(5)),
							walls: false
						);
						if (tile.WallType == WallID.HardenedSand) {
							tile.WallType = WallID.CrimsonHardenedSand;
						} else if (tile.WallType == WallID.Sandstone) {
							tile.WallType = WallID.CrimsonSandstone;
						}
					}
					j++;
				}
			}
			int altarCount = WorldGen.genRand.Next(10, 15);
			for (int k = 0; k < altarCount; k++) {
				int tries = 0;
				bool placedAltar = false;
				int tryLevel = 0;
				while (!placedAltar) {
					tries++;
					int x;
					int y;
					do {
						x = WorldGen.genRand.Next(evilBiomePositionWestBound - tryLevel, evilBiomePositionEastBound + tryLevel);
						y = WorldGen.genRand.Next((int)(Main.worldSurface - tryLevel / 2), (int)(Main.worldSurface + 100.0 + tryLevel));
					} while (WorldGen.oceanDepths(x, y));
					if (tries > 100) {
						tryLevel++;
						tries = 0;
					}
					if (!Main.tile[x, y].HasTile) {
						while (!Main.tile[x, y].HasTile) y++;
						y--;
					} else {
						while (Main.tile[x, y].HasTile && y > Main.worldSurface) y--;
					}
					if ((tryLevel > 10 || Main.tile[x, y + 1].HasTile && Main.tile[x, y + 1].TileType == TileID.Crimstone) && !WorldGen.IsTileNearby(x, y, TileID.DemonAltar, 3)) {
						WorldGen.Place3x2(x, y, TileID.DemonAltar, 1);
						if (Main.tile[x, y].TileType == TileID.DemonAltar) placedAltar = true;
					}
					if (tryLevel > 100) placedAltar = true;
				}
			}

			WorldBiomeGeneration.EvilBiomeGenRanges.Add(new Rectangle(
				evilBiomePositionWestBound,
				j,
				evilBiomePositionEastBound - evilBiomePositionWestBound,
				(int)GenVars.worldSurfaceHigh - j + 500
			));
		}

		public override void PostGenerateEvil() {
			bool worldCrimson = WorldGen.crimson;
			WorldGen.crimson = false;
			WorldGen.CrimPlaceHearts();
			WorldGen.crimson = worldCrimson;
		}
	}
}
