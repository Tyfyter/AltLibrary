﻿using AltLibrary.Common.AltBiomes;
using AltLibrary.Core;
using AltLibrary.Core.Generation;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace AltLibrary.Common.Systems {
	public class WorldBiomeGeneration : ModSystem {
		public static class ChangeRange {
			static int minX, maxX, minY, maxY;
			public static void ResetRange() {
				minX = int.MaxValue;
				maxX = int.MinValue;

				minY = int.MaxValue;
				maxY = int.MinValue;
			}
			public static void AddChangeToRange(int i, int j) {
				if (i < minX) minX = i;
				if (i > maxX) maxX = i;

				if (j < minY) minY = j;
				if (j > maxY) maxY = j;
			}
			public static Rectangle GetRange() {
				return new(
					minX,
					minY,
					maxX - minX,
					maxY - minY
				);
			}
		}
		public static int WofKilledTimes { get; internal set; } = 0;
		static List<Rectangle> _evilBiomeGenRanges = [];
		public static ref List<Rectangle> EvilBiomeGenRanges => ref _evilBiomeGenRanges;

		public override void Unload() {
			EvilBiomeGenRanges = [];
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
			int resetIndex = tasks.FindIndex(genpass => genpass.Name == "Reset");
			if (resetIndex != -1) {
				tasks.Insert(resetIndex + 1, new PassLegacy("Alt Library Setup", new WorldGenLegacyMethod(WorldSetupTask)));
			}
			int corruptionIndex = tasks.FindIndex(i => i.Name.Equals("Corruption"));
			if ((WorldBiomeManager.IsAnyModdedEvil || WorldGen.drunkWorldGen || ModSupport.FargoSeeds.BothEvils()) && corruptionIndex != -1) {
				tasks[corruptionIndex] = new PassLegacy("Corruption", new WorldGenLegacyMethod(EvilTaskGen));
			}
			if (WorldBiomeManager.WorldHell != "") {
				AltBiome biome = AltLibrary.Biomes.Find(x => x.FullName == WorldBiomeManager.WorldHell);
				int underworldIndex = tasks.FindIndex(i => i.Name.Equals("Underworld"));
				if (underworldIndex != -1 && biome.WorldGenPassLegacy != null) {
					tasks.Insert(underworldIndex + 1, biome.WorldGenPassLegacy);
				}

				int hellforgeIndex = tasks.FindIndex(i => i.Name.Equals("Hellforge"));
				if (hellforgeIndex != -1) {
					var gen = biome.GetHellforgeGenerationPass();
					if (gen != null)
						tasks[hellforgeIndex] = new PassLegacy("Hellforge", gen);
				}
			}
			if (!WorldGen.notTheBees && WorldBiomeManager.WorldJungle != "") {
				int jungleIndex = tasks.FindIndex(i => i.Name.Equals("Wet Jungle"));
				if (jungleIndex != -1) {
					tasks[jungleIndex] = new PassLegacy("Wet Jungle", new WorldGenLegacyMethod(JunglesWetTask)); // TODO: translatable genpass names. pass in display name of biome? 
				}
				jungleIndex = tasks.FindIndex(i => i.Name.Equals("Mud Caves To Grass"));
				if (jungleIndex != -1) {
					tasks[jungleIndex] = new PassLegacy("Mud Caves To Grass", new WorldGenLegacyMethod(JunglesGrassTask));
					if (ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeMud.HasValue) {
						tasks.Insert(jungleIndex, new PassLegacy("AltLibrary: Jungle Mud", new WorldGenLegacyMethod(delegate (GenerationProgress progress, GameConfiguration configuration) {
							int tile = ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeMud.Value;
							for (int i = 0; i < Main.maxTilesX; i++) {
								for (int j = 0; j < Main.maxTilesY; j++) {
									if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.Mud) {
										Main.tile[i, j].TileType = (ushort)tile;
									}
								}
							}
						})));
					}
				}
				jungleIndex = tasks.FindIndex(i => i.Name.Equals("Jungle Temple"));
				if (jungleIndex != -1) {
					if (ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).TempleGenPass != null) {
						tasks[jungleIndex] = new PassLegacy("AltLibrary: Temple", new WorldGenLegacyMethod(delegate (GenerationProgress progress, GameConfiguration configuration) {
							ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).TempleGenPass(progress, configuration);
						}));
					}
				}
				jungleIndex = tasks.FindIndex(i => i.Name.Equals("Hives"));
				if (jungleIndex != -1) {
					if (ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).HiveGenerationPass != null) {
						tasks[jungleIndex] = new PassLegacy("AltLibrary: Hives", new WorldGenLegacyMethod(delegate (GenerationProgress progress, GameConfiguration configuration) {
							ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).HiveGenerationPass(progress, configuration);
						}));
					}
				}
				jungleIndex = tasks.FindIndex(i => i.Name.Equals("Jungle Chests"));
				if (jungleIndex != -1) {
					if (!ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeShrineChestType.HasValue)
						tasks.RemoveAt(jungleIndex);
				}
				jungleIndex = tasks.FindIndex(i => i.Name.Equals("Jungle Chests Placement"));
				if (jungleIndex != -1) {
					if (ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeShrineChestType.HasValue)
						tasks[jungleIndex] = new PassLegacy("AltLibrary: Jungle Chests Placement", new WorldGenLegacyMethod(JungleChestPlacementTask));
					else
						tasks.RemoveAt(jungleIndex);
				}
				jungleIndex = tasks.FindIndex(i => i.Name.Equals("Temple"));
				if (jungleIndex != -1) {
					tasks[jungleIndex] = new PassLegacy("AltLibrary: Re-solidify Lihzahrd Brick", new WorldGenLegacyMethod(LihzahrdBrickReSolidTask));
				}
				jungleIndex = tasks.FindIndex(i => i.Name.Equals("Glowing Mushrooms and Jungle Plants"));
				if (jungleIndex != -1) {
					tasks[jungleIndex] = new PassLegacy("AltLibrary: Glowing Mushrooms and Jungle Plants", new WorldGenLegacyMethod(GlowingMushroomsandJunglePlantsTask));
				}
				jungleIndex = tasks.FindIndex(i => i.Name.Equals("Jungle Plants"));
				if (jungleIndex != -1) {
					tasks[jungleIndex] = new PassLegacy("AltLibrary: Jungle Plants", new WorldGenLegacyMethod(JungleBushesTask));
				}
				jungleIndex = tasks.FindIndex(i => i.Name.Equals("Muds Walls In Jungle"));
				if (jungleIndex != -1) {
					if (ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeMudWall.HasValue) {
						tasks.Insert(jungleIndex, new PassLegacy("AltLibrary: Jungle Mud Walls", new WorldGenLegacyMethod(delegate (GenerationProgress progress, GameConfiguration configuration) {
							int wall = ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeMudWall.Value;
							for (int i = 0; i < Main.maxTilesX; i++) {
								for (int j = 0; j < Main.maxTilesY; j++) {
									if (Main.tile[i, j].WallType == WallID.MudUnsafe) {
										Main.tile[i, j].WallType = (ushort)wall;
									}
								}
							}
						})));
					}
				}
				jungleIndex = tasks.FindIndex(i => i.Name.Equals("Lihzahrd Altars"));
				if (jungleIndex != -1) {
					tasks.RemoveAt(jungleIndex);
				}
			}
		}

		private void EvilTaskGen(GenerationProgress progress, GameConfiguration configuration) {
			EvilBiomeGenerationPassHandler.GenerateAllCorruption(GenVars.dungeonSide, GenVars.dungeonLocation, progress);
		}
		#region Jungle
		//TODO: make alt jungle so I can test these
		private void LihzahrdBrickReSolidTask(GenerationProgress progress, GameConfiguration configuration) {
			Main.tileSolid[TileID.LihzahrdBrick] = true;
		}
		private void JungleChestPlacementTask(GenerationProgress progress, GameConfiguration configuration) {
			progress.Message = Lang.gen[32].Value;
			for (int num442 = 0; num442 < GenVars.numJChests; num442++) {
				float value7 = num442 / GenVars.numJChests;
				int style = ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeShrineChestType.HasValue ? 0 : 10;
				int chestType = ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeShrineChestType ?? 0;
				progress.Set(value7);
				int nextJungleChestItem = WorldGen.GetNextJungleChestItem();
				if (!WorldGen.AddBuriedChest(GenVars.JChestX[num442] + WorldGen.genRand.Next(2), GenVars.JChestY[num442], nextJungleChestItem, false, style, false, (ushort)chestType)) {
					for (int num443 = GenVars.JChestX[num442] - 1; num443 <= GenVars.JChestX[num442] + 1; num443++) {
						for (int num444 = GenVars.JChestY[num442]; num444 <= GenVars.JChestY[num442] + 2; num444++) {
							WorldGen.KillTile(num443, num444);
						}
					}
					for (int num445 = GenVars.JChestX[num442] - 1; num445 <= GenVars.JChestX[num442] + 1; num445++) {
						for (int num446 = GenVars.JChestY[num442]; num446 <= GenVars.JChestY[num442] + 3; num446++) {
							if (num446 < Main.maxTilesY) {
								Tile t = Main.tile[num445, num446];
								t.Slope = SlopeType.Solid;
								t.IsHalfBlock = false;
							}
						}
					}
					WorldGen.AddBuriedChest(GenVars.JChestX[num442], GenVars.JChestY[num442], nextJungleChestItem, false, style, false, (ushort)chestType);
				}
			}
		}

		private void JunglesWetTask(GenerationProgress progress, GameConfiguration configuration) {
			progress.Set(1f);
			for (int i = 0; i < Main.maxTilesX; i++) {
				int i2 = i;
				for (int j = (int)GenVars.worldSurfaceLow; j < Main.worldSurface - 1.0; j++) {
					Tile tile49 = Main.tile[i2, j];
					if (tile49.HasTile) {
						tile49 = Main.tile[i2, j];
						bool bl = tile49.TileType == 60;
						foreach (AltBiome biome in AltLibrary.Biomes) {
							if (biome.BiomeType == BiomeType.Jungle && biome.BiomeGrass.HasValue) {
								bl |= tile49.TileType == biome.BiomeGrass.Value;
							}
						}
						if (bl) {
							tile49 = Main.tile[i2, j - 1];
							tile49.LiquidAmount = 255;
							tile49 = Main.tile[i2, j - 2];
							tile49.LiquidAmount = 255;
						}
						break;
					}
				}
			}
		}
		private void JungleBushesTask(GenerationProgress progress, GameConfiguration passConfig) {
			progress.Set(1f);
			int bush = ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeJungleBushes ?? TileID.PlantDetritus;
			for (int num204 = 0; num204 < Main.maxTilesX * 100; num204++) {
				int num205 = WorldGen.genRand.Next(40, Main.maxTilesX / 2 - 40);
				if (GenVars.dungeonSide < 0) {
					num205 += Main.maxTilesX / 2;
				}
				int num206;
				for (num206 = WorldGen.genRand.Next(Main.maxTilesY - 300); !Main.tile[num205, num206].HasTile && num206 < Main.maxTilesY - 300; num206++) {
				}
				if (Main.tile[num205, num206].HasTile && Main.tile[num205, num206].TileType == (ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeJungleGrass ?? TileID.JungleGrass)) {
					num206--;
					WorldGen.PlaceJunglePlant(num205, num206, (ushort)bush, WorldGen.genRand.Next(8), 0);
					if (Main.tile[num205, num206].TileType != bush) {
						WorldGen.PlaceJunglePlant(num205, num206, (ushort)bush, WorldGen.genRand.Next(12), 1);
					}
				}
			}
		}
		private void GlowingMushroomsandJunglePlantsTask(GenerationProgress progress, GameConfiguration passConfig) {
			progress.Set(1f);
			int grass = ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeJunglePlants ?? TileID.JunglePlants;
			for (int num207 = 0; num207 < Main.maxTilesX; num207++) {
				for (int num208 = 0; num208 < Main.maxTilesY; num208++) {
					if (Main.tile[num207, num208].HasTile) {
						if (num208 >= (int)Main.worldSurface && Main.tile[num207, num208].TileType == TileID.MushroomGrass && !Main.tile[num207, num208 - 1].HasTile) {
							WorldGen.GrowTree(num207, num208);
							if (!Main.tile[num207, num208 - 1].HasTile) {
								WorldGen.GrowTree(num207, num208);
								if (!Main.tile[num207, num208 - 1].HasTile) {
									WorldGen.GrowShroom(num207, num208);
									if (!Main.tile[num207, num208 - 1].HasTile) {
										WorldGen.PlaceTile(num207, num208 - 1, TileID.MushroomPlants, mute: true);
									}
								}
							}
						}
						if (Main.tile[num207, num208].TileType == (ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeGrass ?? TileID.JungleGrass) && !Main.tile[num207, num208 - 1].HasTile) {
							WorldGen.PlaceTile(num207, num208 - 1, grass, mute: true, style: grass == TileID.JunglePlants ? 0 : WorldGen.genRand.Next(8));
						}
					}
				}
			}
		}
		private void JunglesGrassTask(GenerationProgress progress, GameConfiguration passConfig) {
			progress.Message = Lang.gen[77].Value;
			for (int i = 0; i < Main.maxTilesX; i++) {
				for (int j = 0; j < Main.maxTilesY; j++) {
					if (Main.tile[i, j].HasUnactuatedTile) {
						WorldGen.grassSpread = 0;
						WorldGen.SpreadGrass(
							i, j,
							ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeMud ?? TileID.Mud,
							ModContent.Find<AltBiome>(WorldBiomeManager.WorldJungle).BiomeGrass.GetValueOrDefault(),
							repeat: true
						);
					}
					progress.Set(0.2f * ((i * Main.maxTilesY + j) / (float)(Main.maxTilesX * Main.maxTilesY)));
				}
			}
			WorldGen.SmallConsecutivesFound = 0;
			WorldGen.SmallConsecutivesEliminated = 0;
			float rightBorder = Main.maxTilesX - 20;
			for (int i = 10; i < Main.maxTilesX - 10; i++) {
				ALReflection.WorldGen_ScanTileColumnAndRemoveClumps(i);
				float num835 = (i - 10) / rightBorder;
				progress.Set(0.2f + num835 * 0.8f);
			}
		}
		#endregion

		private void WorldSetupTask(GenerationProgress progress, GameConfiguration configuration) {
			if (WorldBiomeManager.Copper == -1) {
				GenVars.copper = WorldGen.SavedOreTiers.Copper = TileID.Copper;
				GenVars.copperBar = ItemID.CopperBar;
			} else if (WorldBiomeManager.Copper == -2) {
				GenVars.copper = WorldGen.SavedOreTiers.Copper = TileID.Tin;
				GenVars.copperBar = ItemID.TinBar;
			} else {
				GenVars.copper = WorldGen.SavedOreTiers.Copper = AltLibrary.Ores[WorldBiomeManager.Copper - 1].ore;
				GenVars.copperBar = AltLibrary.Ores[WorldBiomeManager.Copper - 1].bar;
			}
			if (WorldBiomeManager.Iron == -3) {
				GenVars.iron = WorldGen.SavedOreTiers.Iron = TileID.Iron;
				GenVars.ironBar = ItemID.IronBar;
			} else if (WorldBiomeManager.Iron == -4) {
				GenVars.iron = WorldGen.SavedOreTiers.Iron = TileID.Lead;
				GenVars.ironBar = ItemID.LeadBar;
			} else {
				GenVars.iron = WorldGen.SavedOreTiers.Iron = AltLibrary.Ores[WorldBiomeManager.Iron - 1].ore;
				GenVars.ironBar = AltLibrary.Ores[WorldBiomeManager.Iron - 1].bar;
			}
			if (WorldBiomeManager.Silver == -5) {
				GenVars.silver = WorldGen.SavedOreTiers.Silver = TileID.Silver;
				GenVars.silverBar = ItemID.SilverBar;
			} else if (WorldBiomeManager.Silver == -6) {
				GenVars.silver = WorldGen.SavedOreTiers.Silver = TileID.Tungsten;
				GenVars.silverBar = ItemID.TungstenBar;
			} else {
				GenVars.silver = WorldGen.SavedOreTiers.Silver = AltLibrary.Ores[WorldBiomeManager.Silver - 1].ore;
				GenVars.silverBar = AltLibrary.Ores[WorldBiomeManager.Silver - 1].bar;
			}
			if (WorldBiomeManager.Gold == -7) {
				GenVars.gold = WorldGen.SavedOreTiers.Gold = TileID.Gold;
				GenVars.goldBar = ItemID.GoldBar;
			} else if (WorldBiomeManager.Gold == -8) {
				GenVars.gold = WorldGen.SavedOreTiers.Gold = TileID.Platinum;
				GenVars.goldBar = ItemID.PlatinumBar;
			} else {
				GenVars.gold = WorldGen.SavedOreTiers.Gold = AltLibrary.Ores[WorldBiomeManager.Gold - 1].ore;
				GenVars.goldBar = AltLibrary.Ores[WorldBiomeManager.Gold - 1].bar;
			}
			if (WorldBiomeManager.Cobalt == -9) {
				WorldGen.SavedOreTiers.Cobalt = TileID.Cobalt;
			} else if (WorldBiomeManager.Cobalt == -10) {
				WorldGen.SavedOreTiers.Cobalt = TileID.Palladium;
			} else {
				WorldGen.SavedOreTiers.Cobalt = AltLibrary.Ores[WorldBiomeManager.Cobalt - 1].ore;
			}
			if (WorldBiomeManager.Mythril == -11) {
				WorldGen.SavedOreTiers.Mythril = TileID.Mythril;
			} else if (WorldBiomeManager.Mythril == -12) {
				WorldGen.SavedOreTiers.Mythril = TileID.Orichalcum;
			} else {
				WorldGen.SavedOreTiers.Mythril = AltLibrary.Ores[WorldBiomeManager.Mythril - 1].ore;
			}
			if (WorldBiomeManager.Adamantite == -13) {
				WorldGen.SavedOreTiers.Adamantite = TileID.Adamantite;
			} else if (WorldBiomeManager.Adamantite == -14) {
				WorldGen.SavedOreTiers.Adamantite = TileID.Titanium;
			} else {
				WorldGen.SavedOreTiers.Adamantite = AltLibrary.Ores[WorldBiomeManager.Adamantite - 1].ore;
			}

			if (WorldGen.drunkWorldGen) {
				List<AltBiome> biomes = AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Evil && x.Selectable).Append(ModContent.GetInstance<CorruptionAltBiome>()).Append(ModContent.GetInstance<CrimsonAltBiome>()).ToList();
				int index = WorldGen.genRand.Next(biomes.Count);
				AltBiome mainEvil = biomes[index];
				biomes.RemoveAt(index);
				WorldBiomeManager.DrunkEvil = biomes[WorldGen.genRand.Next(biomes.Count)];
			}
		}
		public override void ClearWorld() {
			EvilBiomeGenRanges = [];
		}
		public override void SaveWorldData(TagCompound tag) {
			tag.Add("AltLibrary:WofKilledTimes", WofKilledTimes);
			tag.Add("AltLibrary:EvilBiomeGenRanges", EvilBiomeGenRanges);
		}

		public override void LoadWorldData(TagCompound tag) {
			WofKilledTimes = tag.GetInt("AltLibrary:WofKilledTimes");
			if (!tag.TryGet("AltLibrary:EvilBiomeGenRanges", out EvilBiomeGenRanges)) EvilBiomeGenRanges = [];
		}
	}
}
