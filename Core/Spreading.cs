using AltLibrary.Common;
using AltLibrary.Common.AltBiomes;
using AltLibrary.Core.Baking;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using PegasusLib.Networking;
using System;
using System.Numerics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Core {
	internal class Spreading : GlobalTile {
		public override void RandomUpdate(int i, int j, int type) {
			if (Main.tile[i, j].IsActuated) {
				return;
			}
			bool isOreGrowingTile = false;
			bool isJungleSpreadingOre = false;
			AltBiome biomeToSpread = null;
			AltBiome biome = AltLibrary.GetAltBiome(TileSets.OwnedByBiomeID[type]);
			if (biome is null or VanillaBiome) {
				return;
			}
			if (biome.BiomeType == BiomeType.Evil) {
				if (type == biome.BiomeGrass) {
					biomeToSpread = biome;
				} else if (type == biome.BiomeJungleGrass) {
					biomeToSpread = biome;
				}
			} else if (biome.BiomeType == BiomeType.Hallow) {
				if (type == biome.BiomeGrass) {
					biomeToSpread = biome;
				}

			} else if (biome.BiomeType == BiomeType.Jungle) {
				if (type == biome.BiomeGrass || !biome.BiomeGrass.HasValue && type == biome.BiomeJungleGrass) {
					isOreGrowingTile = true;
					biomeToSpread = biome;
				} else if (type == biome.BiomeOre) {
					isJungleSpreadingOre = true;
					biomeToSpread = biome;
				}
			}

			if (isOreGrowingTile || isJungleSpreadingOre) {
				if (j > (Main.worldSurface + Main.rockLayer) / 2.0) {
					if (isOreGrowingTile && WorldGen.genRand.NextBool(300)) {
						int xdif = WorldGen.genRand.Next(-10, 11);
						int ydif = WorldGen.genRand.Next(-10, 11);
						int targetX = i + xdif;
						int targetY = j + ydif;
						Tile target = Main.tile[targetX, targetY];

						if (WorldGen.InWorld(targetX, targetY) && Main.tile[targetX, targetY].TileType == TileID.Mud) {
							if (Main.tile[targetX, targetY].IsActuated ||
								Main.tile[targetX, targetY - 1].TileType != TileID.Trees && Main.tile[targetX, targetY - 1].TileType != TileID.LifeFruit
								&& !AltLibrary.planteraBulbs.Contains(Main.tile[targetX, targetY - 1].TileType)) {
								target.TileType = (ushort)biomeToSpread.BiomeOre;
								WorldGen.SquareTileFrame(targetX, targetY);
								if (Main.netMode == NetmodeID.Server) NetMessage.SendTileSquare(-1, targetX, targetY);
							}
						}
					}
					if (isJungleSpreadingOre && !WorldGen.genRand.NextBool(3)) {
						int targX = i;
						int targY = j;
						int random = WorldGen.genRand.Next(4);
						if (random == 0) {
							targX++;
						}
						if (random == 1) {
							targX--;
						}
						if (random == 2) {
							targY++;
						}
						if (random == 3) {
							targY--;
						}
						Tile target = Main.tile[targX, targY];
						if (WorldGen.InWorld(targX, targY, 2) && !target.IsActuated && (target.TileType == TileID.Mud || target.TileType == biomeToSpread.BiomeGrass)) {
							target.TileType = (ushort)type;
							WorldGen.SquareTileFrame(targX, targY);
							if (Main.netMode == NetmodeID.Server) {
								NetMessage.SendTileSquare(-1, targX, targY);
							}
						}
						bool flag = true;
						while (flag) {
							flag = false;
							targX = i + Main.rand.Next(-6, 7);
							targY = j + Main.rand.Next(-6, 7);
							target = Main.tile[targX, targY];
							if (!WorldGen.InWorld(targX, targY, 2) || target.IsActuated) {
								continue;
							}
							if (TileID.Sets.Conversion.Grass[target.TileType] && !AltLibrary.jungleGrass.Contains(target.TileType)) {
								target.TileType = (ushort)biomeToSpread.BiomeGrass;
								WorldGen.SquareTileFrame(targX, targY);
								if (Main.netMode == NetmodeID.Server) {
									NetMessage.SendTileSquare(-1, targX, targY);
								}
								flag = true;
							} else if (target.TileType == TileID.Dirt) {
								target.TileType = TileID.Mud;
								WorldGen.SquareTileFrame(targX, targY);
								if (Main.netMode == NetmodeID.Server) {
									NetMessage.SendTileSquare(-1, targX, targY);
								}
								flag = true;
							}
						}
					}
				}
			}
		}
		/// <inheritdoc cref="WorldGen.SpreadInfectionToNearbyTile"/>
		public static void SpreadInfection(int i, int j, AltBiome biome) => WorldGen.SpreadInfectionToNearbyTile(i, j, biome.BiomeConversionType);
		public static void SpreadGrass(int i, int j, GrassType grassType, int remainingSteps = 0, TileColorCache color = default) {
			int left = i - 1; // defining the bounds of the 3x3 space which will be checked for dirt
			int right = i + 1;
			int top = j - 1;
			int bottom = j + 1;
			// making sure the coords to detect dirt are within the bounds of the world
			if (left < 0) {
				left = 0;
			}
			if (right > Main.maxTilesX) {
				right = Main.maxTilesX;
			}
			if (top < 0) {
				top = 0;
			}
			if (bottom > Main.maxTilesY) {
				bottom = Main.maxTilesY;
			}
			for (int k = left; k <= right; k++) {
				for (int l = top; l <= bottom; l++) {
					SpreadGrassPhase2(k, l, grassType, remainingSteps, color);
				}
			}
		}
		private static void SpreadGrassPhase2(int i, int j, GrassType grassType, int remainingSteps = 0, TileColorCache color = default) {
			if (!WorldGen.InWorld(i, j, 10)) return;
			Tile tile = Main.tile[i, j];
			if (!tile.HasTile || !TileID.Sets.CanBeClearedDuringGeneration[tile.TileType])
				return;
			int tileType = tile.TileType;
			if (WorldGen.AllowedToSpreadInfections && grassType.GrassGrowthSettings.Infection) {
				switch (tileType) {
					case TileID.Grass:
					tileType = TileID.Dirt;
					break;
				}
			}
			if (grassType.TypesByDirt[tileType] == -1) return;
			if (tile.TileType == TileID.Dirt) {
				if (WorldGen.gen && grassType.GrassGrowthSettings.Infection) {
					if ((!WorldGen.tenthAnniversaryWorldGen && i > Main.maxTilesX * 0.45 && i <= Main.maxTilesX * 0.55) || i < WorldGen.beachDistance || i >= Main.maxTilesX - WorldGen.beachDistance)
						return;
				} else if ((WorldGen.gen || grassType.GrassGrowthSettings.Infection) && j >= Main.worldSurface && !Main.remixWorld) {
					return;
				}
			}
			int left = i - 1; // defining the bounds of the 3x3 space which will be checked for lava and air
			int right = i + 1;
			int top = j - 1;
			int bottom = j + 1;
			// making sure the coords to detect obstacles are within the bounds of the world
			Max(ref left, 0);
			Min(ref right, Main.maxTilesX);
			Max(ref top, 0);
			Min(ref bottom, Main.maxTilesY);

			bool haltSpread = true; // a boolean that determines if the grass can spread
			for (int k = left; k <= right; k++) {
				for (int l = top; l <= bottom; l++) {
					// checking that at least one adjacent tile is air
					if (!Main.tile[k, l].HasUnactuatedTile || !Main.tileSolid[Main.tile[k, l].TileType]) {
						haltSpread = false;
					}
					// checking that none of the adjacent tiles contain lava
					if (Main.tile[k, l].LiquidType == LiquidID.Lava && Main.tile[k, l].LiquidAmount > 0) {
						haltSpread = true;
						break; // stops checking adjacent blocks if even one is lava
					}
				}
			}
			if (grassType.GrassGrowthSettings.Infection && Main.tile[i, j - 1].TileType == TileID.Sunflower) {
				haltSpread = true;
			}
			if (haltSpread) return;
			tile.TileType = (ushort)grassType.TypesByDirt[tileType];
			tile.UseBlockColors(color);
			WorldGen.SquareTileFrame(i, j);
			if (NetmodeActive.Server) NetMessage.SendTileSquare(-1, i, j);
			if (remainingSteps > 0) SpreadGrass(i, j, grassType, --remainingSteps, color);
		}

		public override bool? IsTileBiomeSightable(int i, int j, int type, ref Color sightColor) {
			if (TileSets.BiomeSightColors[type] is Color color) {
				sightColor = color;
				return true;
			}
			return null;
		}
		static void Min<T>(ref T current, T @new) where T : IComparisonOperators<T, T, bool> {
			if (current > @new) current = @new;
		}
		static void Max<T>(ref T current, T @new) where T : IComparisonOperators<T, T, bool> {
			if (current < @new) current = @new;
		}
	}
	[ReinitializeDuringResizeArrays]
	public class GrassSpreading : GlobalTile {
		internal static int[] GrassTypeIDs = TileID.Sets.Factory.CreateIntSet(-1);
		public override void RandomUpdate(int i, int j, int type) {
			SpreadGrassByTile(i, j);
		}
		public static void SpreadGrassByTile(int i, int j) {
			Tile tile = Main.tile[i, j];
			if (tile.IsActuated || GrassTypeIDs[tile.TileType] == -1) return;
			Spreading.SpreadGrass(i, j, Grasses.GrassTypes[GrassTypeIDs[tile.TileType]]);
		}
	}
}
