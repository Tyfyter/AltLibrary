using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using AltLibrary.Core;
using AltLibrary.Core.Baking;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common.Hooks {
	public static class HardmodeWorldGen {
		public static void Init() {
			On_WorldGen.GERunner += WorldGen_GERunner1;
			IL_WorldGen.HardmodeWallsTask += GenPasses_HookGenPassHardmodeWalls;
		}

		public static void Unload() { }
		public static bool GERunnerRunning { get; private set; }
		/// <summary>
		/// In Not the bees! worlds, hive and crispy honey blocks are replaced with 
		/// </summary>
		public static bool ShouldConvertBeeTiles { get; private set; }
		private static void WorldGen_GERunner1(On_WorldGen.orig_GERunner orig, int i, int j, double speedX, double speedY, bool good) {
			if (Main.drunkWorld && WorldBiomeGeneration.WofKilledTimes > 1) {
				int addX = WorldGen.genRand.Next(300, 400) * WorldBiomeGeneration.WofKilledTimes;
				if (!good) addX *= -1;
				i += addX;
				//TODO: double check that this code makes sense to begin with
				if (i < 0) {
					i *= -1;
				}
				if (i > Main.maxTilesX) {
					i %= Main.maxTilesX;
				}
			}
			try {
				ShouldConvertBeeTiles = false;
				int num = 0;
				for (int k = 20; k < Main.maxTilesX - 20; k++) {
					for (int l = 20; l < Main.maxTilesY - 20; l++) {
						if (Main.tile[k, l].HasTile && Main.tile[k, l].TileType == TileID.Hive) {
							if (++num > 200000) {
								ShouldConvertBeeTiles = true;
								break;
							}
						}
					}
				}
				GERunnerRunning = true;
				AltBiome biome = good ? WorldBiomeManager.GetWorldHallow(false) : WorldBiomeManager.GetWorldEvil(false, true);
				if (biome is null) {
					orig(i, j, speedX, speedY, good);
				} else {
					GERunner(biome, i, j, speedX, speedY);
				}
			} finally {
				GERunnerRunning = false;
				ShouldConvertBeeTiles = false;
			}
		}
		private static void GenPasses_HookGenPassHardmodeWalls(ILContext il) {
			ILCursor c = new(il);
			if (!c.TryGotoNext(MoveType.AfterLabel, 
				i => i.MatchLdloca(5),
				i => i.MatchCall<Tile>("active")
			)) {
				AltLibrary.Instance.Logger.Info("GenPassHardmodeWalls $ active");
				return;
			}
			c.Index++;
			c.Emit(OpCodes.Ldloc, 7);
			c.Emit(OpCodes.Ldloc, 5);
			c.EmitDelegate<Func<int, Tile, int>>((wallType, tile) => {
				for (int i = 0; i < AltLibrary.Biomes.Count; i++) {
					AltBiome biome = AltLibrary.Biomes[i];
					if (biome.BiomeType <= BiomeType.Hallow && biome.HardmodeWalls.Count > 0 && biome.TileConversions.ContainsValue(tile.TileType)) {
						wallType = WorldGen.genRand.Next(biome.HardmodeWalls);
					}
				}
				return wallType;
			});
			c.Emit(OpCodes.Stloc, 7);
		}
		public static void GERunner(AltBiome biome, int i, int j, double speedX = 0.0, double speedY = 0.0) {
			int size = WorldGen.genRand.Next(200, 250);
			double worldSizeMult = Main.maxTilesX / 4200.0;
			size = (int)(size * worldSizeMult);
			double halfSize = size * 0.5;
			Vector2D position = new(i, j);
			Vector2D speed;
			if (speedX != 0.0 || speedY != 0.0) {
				speed = new(speedX, speedY);
			} else {
				speed = new(WorldGen.genRand.Next(-10, 11) * 0.1, WorldGen.genRand.Next(-10, 11) * 0.1);
			}
			bool inBounds = true;
			do {
				int minX = (int)Math.Clamp(position.X - halfSize, 0, Main.maxTilesX);
				int maxX = (int)Math.Clamp(position.X + halfSize, 0, Main.maxTilesX);
				int minY = (int)Math.Clamp(position.Y - halfSize, 0, Main.maxTilesY - 5);
				int maxY = (int)Math.Clamp(position.Y + halfSize, 0, Main.maxTilesY - 5);
				for (int x = minX; x < maxX; x++) {
					for (int y = minY; y < maxY; y++) {
						if (Math.Abs(x - position.X) + Math.Abs(y - position.Y) >= size * 0.5 * (1.0 + WorldGen.genRand.Next(-10, 11) * 0.015)) {
							continue;
						}
						Tile tile = Main.tile[x, y];
						int tileConversion = biome.GetAltBlock(tile.TileType, x, y, true);
						if (tileConversion == -1 && ShouldConvertBeeTiles) {
							switch (tile.TileType) {
								case TileID.Hive:
								tileConversion = biome.GetAltBlock(TileID.Stone, x, y, true);
								break;

								case TileID.CrispyHoneyBlock:
								tileConversion = biome.GetAltBlock(TileID.HardenedSand, x, y, true);
								break;
							}
						}
						if (tileConversion != -1) {
							if (tileConversion == -2) {
								tile.HasTile = false;
								WorldGen.SquareTileFrame(x, y);
							} else {
								tile.TileType = (ushort)tileConversion;
								WorldGen.SquareTileFrame(x, y);
							}
						}
						int wallConversion = biome.GetAltWall(tile.WallType, x, y, true);
						if (wallConversion != -1) {
							tile.WallType = (ushort)wallConversion;
						}
					}
				}
				position += speed;
				speed.X += WorldGen.genRand.Next(-10, 11) * 0.05;
				if (speed.X > speedX + 1.0) {
					speed.X = speedX + 1.0;
				}
				if (speed.X < speedX - 1.0) {
					speed.X = speedX - 1.0;
				}
				if (position.X < -size || position.Y < -size || position.X > (Main.maxTilesX + size) || position.Y > (Main.maxTilesY + size)) {
					inBounds = false;
				}
			} while (inBounds);
		}
	}
}
