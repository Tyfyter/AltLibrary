using AltLibrary.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace AltLibrary.Core.Generation {
	internal class CorruptionEvilBiomeGenerationPass : EvilBiomeGenerationPass {
		public override string ProgressMessage => Lang.gen[20].Value;
		public override bool CanGenerateNearDungeonOcean => false;

		public override void GenerateEvil(int evilBiomePosition, int evilBiomePositionWestBound, int evilBiomePositionEastBound) {
			bool worldCrimson = WorldGen.crimson;
			WorldGen.crimson = false;

			int distToNextChasm = 0;
			for (int i = evilBiomePositionWestBound; i < evilBiomePositionEastBound; i++) {
				if (distToNextChasm > 0) {
					distToNextChasm--;
				}
				if (i == evilBiomePosition || distToNextChasm == 0) {
					int surface = (int)GenVars.worldSurfaceLow;
					while (surface < Main.worldSurface - 1.0) {
						if (Main.tile[i, surface].HasTile || Main.tile[i, surface].WallType > WallID.None) {
							if (i == evilBiomePosition) {
								distToNextChasm = 20;
								WorldGen.ChasmRunner(i, surface, WorldGen.genRand.Next(150) + 150, true);
								break;
							}
							if (WorldGen.genRand.NextBool(35) && distToNextChasm == 0) {
								distToNextChasm = 30;
								WorldGen.ChasmRunner(i, surface, WorldGen.genRand.Next(50) + 50, true);
								break;
							}
							break;
						}
						surface++;
					}
				}
			}
			double maxSurfaceDepth = Main.worldSurface + 40.0;
			for (int i = evilBiomePositionWestBound; i < evilBiomePositionEastBound; i++) {
				maxSurfaceDepth += WorldGen.genRand.Next(-2, 3);
				Max(ref maxSurfaceDepth, Main.worldSurface + 30.0);
				Min(ref maxSurfaceDepth, Main.worldSurface + 50.0);
				bool highestTile = true;
				int y = (int)GenVars.worldSurfaceLow;
				while (y < maxSurfaceDepth) {
					Tile tile = Main.tile[i, y];
					if (tile.HasTile) {
						if (y < Main.worldSurface - 1.0 && highestTile) {
							if (tile.TileType == TileID.Dirt) {
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(i, y, TileID.Dirt, TileID.CorruptGrass, true);
							} else if (tile.TileType == TileID.Mud) {
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(i, y, TileID.Mud, TileID.CorruptJungleGrass);
							}
						}
						highestTile = false;
						WorldGen.Convert(i, y, BiomeConversionID.Corruption,
							tiles: tile.TileType is not TileID.Stone and not TileID.Sand || (i >= evilBiomePositionWestBound + WorldGen.genRand.Next(5) && i <= evilBiomePositionEastBound - WorldGen.genRand.Next(5)),
							walls: false
						);
						if (tile.WallType == WallID.HardenedSand) {
							tile.WallType = WallID.CorruptHardenedSand;
						} else if (tile.WallType == WallID.Sandstone) {
							tile.WallType = WallID.CorruptSandstone;
						}
					}
					y++;
				}
			}
			for (int i = evilBiomePositionWestBound; i < evilBiomePositionEastBound; i++) {
				for (int j = 0; j < Main.maxTilesY - 50; j++) {
					if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.ShadowOrbs) {
						int minX = i - 13;
						int maxX = i + 13;
						int minY = j - 13;
						int maxY = j + 13;
						for (int x = minX; x < maxX; x++) {
							if (x > 10 && x < Main.maxTilesX - 10) {
								for (int y = minY; y < maxY; y++) {
									Tile tile = Main.tile[x, y];
									if (Math.Abs(x - i) + Math.Abs(y - j) < 9 + WorldGen.genRand.Next(11) && !WorldGen.genRand.NextBool(3) && tile.TileType != TileID.ShadowOrbs) {
										if (Math.Abs(x - i) <= 1 && Math.Abs(y - j) <= 1) {
											tile.HasTile = false;
										} else {
											tile.HasTile = true;
											tile.TileType = TileID.Ebonstone;
										}
									}
									if (tile.TileType != TileID.ShadowOrbs && Math.Abs(x - i) <= 2 + WorldGen.genRand.Next(3) && Math.Abs(y - j) <= 2 + WorldGen.genRand.Next(3)) {
										tile.HasTile = false;
									}
								}
							}
						}
					}
				}
			}
			WorldGen.crimson = worldCrimson;
			int worldSurfaceLow = (int)GenVars.worldSurfaceLow;
			WorldBiomeGeneration.EvilBiomeGenRanges.Add(new Rectangle(
				evilBiomePositionWestBound,
				worldSurfaceLow,
				evilBiomePositionEastBound - evilBiomePositionWestBound,
				(int)GenVars.worldSurfaceHigh - worldSurfaceLow + 500
			));
		}

		public override void PostGenerateEvil() {
		}
	}
}
