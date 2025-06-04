using AltLibrary.Common.AltBiomes;
using AltLibrary.Core.Baking;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Core {
	internal class Spreading : GlobalTile {
		public override void Load() {
			IL_WorldGen.hardUpdateWorld += IL_WorldGen_hardUpdateWorld;
		}

		private void IL_WorldGen_hardUpdateWorld(ILContext il) {
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.AllowedToSpreadInfections)));
			c.EmitLdarg0();
			c.EmitLdarg1();
			c.EmitDelegate((bool allowedToSpreadInfections, int i, int j) => {
				if (!allowedToSpreadInfections) return false;
				ushort type = Main.tile[i, j].TileType;
				ALConvertInheritanceData.tileParentageData.TryGetParent(type, out (int baseTile, AltBiome fromBiome) parent);
				AltBiome biome = parent.fromBiome;
				if (biome is null or VanillaBiome) {
					return true;
				}
				if (biome.SpreadingTiles.Contains(type)) {
					SpreadInfection(i, j, biome);
				}
				return false;
			});
		}

		public override void RandomUpdate(int i, int j, int type) {
			if (Main.tile[i, j].IsActuated) {
				return;
			}
			bool isOreGrowingTile = false;
			bool isJungleSpreadingOre = false;
			bool isGrass = false;
			AltBiome biomeToSpread = null;
			ALConvertInheritanceData.tileParentageData.TryGetParent(type, out (int baseTile, AltBiome fromBiome) parent);
			AltBiome biome = parent.fromBiome;
			if (biome is null or VanillaBiome) {
				return;
			}
			if (biome.BiomeType == BiomeType.Evil || biome.BiomeType == BiomeType.Hallow) {
				if (type == biome.BiomeGrass) {
					isGrass = true;
					biomeToSpread = biome;
				}
			} else if (biome.BiomeType == BiomeType.Jungle) {
				if (type == biome.BiomeGrass || !biome.BiomeGrass.HasValue && type == biome.BiomeJungleGrass) {
					isGrass = true;
					isOreGrowingTile = true;
					biomeToSpread = biome;
				} else if (type == biome.BiomeOre) {
					isJungleSpreadingOre = true;
					biomeToSpread = biome;
				}
			}

			if (isGrass) {
				if (biomeToSpread.BiomeType == BiomeType.Jungle) {
					SpreadGrass(i, j, TileID.Mud, type, false);
				} else {
					bool blockedBySunflowers = false;
					if (biomeToSpread.BiomeType == BiomeType.Evil) blockedBySunflowers = true;
					if (j < (Main.worldSurface + Main.rockLayer) / 2) {
						SpreadGrass(i, j, TileID.Dirt, type, blockedBySunflowers);
					}
					if (WorldGen.AllowedToSpreadInfections) SpreadGrass(i, j, TileID.Grass, type, blockedBySunflowers);
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
		private static bool NearbyEvilSlowingOres(int i, int j) {
			float count = 0f;
			int worldEdgeDistance = 5;
			if (i <= worldEdgeDistance + 5 || i >= Main.maxTilesX - worldEdgeDistance - 5) {
				return false;
			}
			if (j <= worldEdgeDistance + 5 || j >= Main.maxTilesY - worldEdgeDistance - 5) {
				return false;
			}
			for (int k = i - worldEdgeDistance; k <= i + worldEdgeDistance; k++) {
				for (int l = j - worldEdgeDistance; l <= j + worldEdgeDistance; l++) {
					if (!Main.tile[k, l].IsActuated && AltLibrary.evilStoppingOres.Contains(Main.tile[k, l].TileType)) {
						count += 1f;
						if (count >= 4f) {
							return true;
						}
					}
				}
			}
			if (count > 0f && WorldGen.genRand.Next(5) < count) {
				return true;
			}
			return false;
		}
		/// <summary>
		/// Runs the biome spreading function at the given coordinates for the given AltBiome
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="biome"></param>
		public static void SpreadInfection(int i, int j, AltBiome biome) {
			if (Main.hardMode && WorldGen.AllowedToSpreadInfections && !(NPC.downedPlantBoss && !WorldGen.genRand.NextBool(2))) {
				bool flag = true;
				while (flag) {
					flag = false;
					int xdif = WorldGen.genRand.Next(-3, 4);
					int ydif = WorldGen.genRand.Next(-3, 4);
					int targetX = i + xdif;
					int targetY = j + ydif;

					if (WorldGen.InWorld(targetX, targetY, 10)) {
						Tile target = Main.tile[targetX, targetY];
						bool canSpread = target.HasUnactuatedTile && Main.tileSolid[target.TileType];
						ushort oldTileType = target.TileType;
						int newTileType = -1;

						if (biome.BiomeType == BiomeType.Evil) {
							if (WorldGen.nearbyChlorophyte(targetX, targetY)) {
								WorldGen.ChlorophyteDefense(targetX, targetY);
								continue;
							}
							if (WorldGen.CountNearBlocksTypes(targetX, targetY, 2, 1, TileID.Sunflower) > 0) continue;
							if (NearbyEvilSlowingOres(targetX, targetY)) continue;
						}
						if (canSpread) {
							newTileType = biome.GetAltBlock(oldTileType, targetX, targetY);


							if (newTileType != -1 && newTileType != oldTileType) {
								if (WorldGen.genRand.NextBool(2)) flag = true;
								target.TileType = (ushort)newTileType;
								WorldGen.SquareTileFrame(targetX, targetY);
								if (Main.netMode == NetmodeID.Server) NetMessage.SendTileSquare(-1, targetX, targetY);
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// Runs the grass growing function for the given coordinates with the given TileIDs
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="dirt"></param>
		/// <param name="grass"></param>
		/// <param name="blockedBySunflowers"></param>
		public static void SpreadGrass(int i, int j, int dirt, int grass, bool blockedBySunflowers) {
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
					SpreadGrassPhase2(k, l, dirt, grass, blockedBySunflowers);
				}
			}
		}
		/// <summary>
		/// should only ever be called as part of the SpreadGrass function
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="dirt"></param>
		/// <param name="grass"></param>
		/// <param name="blockedBySunflowers"></param>
		private static void SpreadGrassPhase2(int i, int j, int dirt, int grass, bool blockedBySunflowers) {
			int left = i - 1; // defining the bounds of the 3x3 space which will be checked for lava and air
			int right = i + 1;
			int top = j - 1;
			int bottom = j + 1;

			if (left < 0) // making sure the coords to detect obstacles are within the bounds of the world
			{
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

			bool haltSpread = true; // a boolean that determines if the grass can spread
			for (int k = left; k <= right; k++) {
				for (int l = top; l <= bottom; l++) {
					if (!Main.tile[k, l].HasUnactuatedTile || !Main.tileSolid[Main.tile[k, l].TileType]) // checking that at least one adjacent tile is air
					{
						haltSpread = false;
					}
					if (Main.tile[k, l].LiquidType == LiquidID.Lava && Main.tile[k, l].LiquidAmount > 0) // checking that none of the adjacent tiles contain lava
					{
						haltSpread = true;
						break; // stops checking adjacent blocks if even one is lava
					}
				} // effectively, what was just done is this; grass is halted by default, but if even one adjacent tile has air (or furniture, etc) then
			}     // grass is no longer halted. the lava check then comes after the air check so that if there *is* any lava touching the block, the grass will not 
			if (Main.tile[i, j - 1].TileType == TileID.Sunflower && blockedBySunflowers) {
				haltSpread = true;
			}
			// checking if the grass is allowed to spread and if the block in question is dirt
			if (!haltSpread && Main.tile[i, j].TileType == dirt) {
				Main.tile[i, j].TileType = (ushort)grass;
				WorldGen.SquareTileFrame(i, j);
				if (Main.netMode == NetmodeID.Server) NetMessage.SendTileSquare(-1, i, j);
			}
		}
	}
}
