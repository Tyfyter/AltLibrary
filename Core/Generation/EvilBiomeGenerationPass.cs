using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AltLibrary.Core.Generation {
	internal static class EvilBiomeGenerationPassHandler {
		internal static List<(int min, int max, float edgeGivePercent)> evilRanges;
		internal static bool GenerateAllCorruption(
			int dungeonSide,
			int dungeonLocation,
			GenerationProgress progress) {
			int JungleBoundMinX = Main.maxTilesX;
			int JungleBoundMaxX = 0;
			int SnowBoundMinX = Main.maxTilesX;
			int SnowBoundMaxX = 0;
			for (int i = 0; i < Main.maxTilesX; i++) {
				int snowJungleIter = (int)GenVars.worldSurfaceLow - 10;
				while (snowJungleIter < GenVars.worldSurfaceHigh - 50) {
					if (Main.tile[i, snowJungleIter].HasTile) {
						if (Main.tile[i, snowJungleIter].TileType == (WorldBiomeManager.WorldJungle?.BiomeGrass ?? TileID.JungleGrass)) {
							if (i < JungleBoundMinX) {
								JungleBoundMinX = i;
							}
							if (i > JungleBoundMaxX) {
								JungleBoundMaxX = i;
							}
						} else if (Main.tile[i, snowJungleIter].TileType == TileID.SnowBlock || Main.tile[i, snowJungleIter].TileType == TileID.IceBlock) {
							if (i < SnowBoundMinX) {
								SnowBoundMinX = i;
							}
							if (i > SnowBoundMaxX) {
								SnowBoundMaxX = i;
							}
						}
					}
					snowJungleIter++;
				}
			}

			int jungleSnowGive = 10;
			JungleBoundMinX -= jungleSnowGive;
			JungleBoundMaxX += jungleSnowGive;
			SnowBoundMinX -= jungleSnowGive;
			SnowBoundMaxX += jungleSnowGive;

			List<EvilBiomeGenerationPass> EvilBiomes = new();

			if (WorldGen.drunkWorldGen) {
				EvilBiomes.Add(VanillaBiome.crimsonPass);
				EvilBiomes.Add(VanillaBiome.corruptPass);
				AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Evil).ToList().ForEach(i => {
					if (i.GetEvilBiomeGenerationPass() is EvilBiomeGenerationPass genPass) EvilBiomes.Add(genPass);
				});
				//shuffle list

				int n = EvilBiomes.Count;
				while (n > 1) {
					n--;
					int k = WorldGen.genRand.Next(n + 1);
					(EvilBiomes[n], EvilBiomes[k]) = (EvilBiomes[k], EvilBiomes[n]);
				}
			} else {
				EvilBiomes.Add(WorldBiomeManager.GetWorldEvil(true).GetEvilBiomeGenerationPass());
				/*if (WorldBiomeManager.WorldEvil == "" && !WorldGen.crimson)
					EvilBiomes.Add(VanillaBiome.corruptPass);
				else if (WorldBiomeManager.WorldEvil == "" && WorldGen.crimson)
					EvilBiomes.Add(VanillaBiome.crimsonPass);
				else
					EvilBiomes.Add(AltLibrary.Biomes.Find(x => x.FullName == WorldBiomeManager.WorldEvil).GetEvilBiomeGenerationPass());*/
			}

			double numberPasses = Main.maxTilesX * 0.00045;
			numberPasses /= EvilBiomes.Count;

			int drunkIter = 0;
			if (AltLibraryConfig.Config.DrunkMaxBiomes != 0 && EvilBiomes.Count > AltLibraryConfig.Config.DrunkMaxBiomes) {
				EvilBiomes.RemoveRange(AltLibraryConfig.Config.DrunkMaxBiomes, EvilBiomes.Count - AltLibraryConfig.Config.DrunkMaxBiomes);
			}
			int drunkMax = EvilBiomes.Count;
			evilRanges = [];
			EvilBiomes.ForEach(i => {
				if (i != null) {
					progress.Message = i.ProgressMessage ?? "No ProgressMessage! Report that to Mod Developer!";

					int passesDone = 0;
					while (passesDone < numberPasses) {
						WorldBiomeGeneration.ChangeRange.ResetRange();
						i.GetEvilSpawnLocation(dungeonSide, dungeonLocation, SnowBoundMinX, SnowBoundMaxX, JungleBoundMinX, JungleBoundMaxX, drunkIter, drunkMax, out int evilMid, out int evilLeft, out int evilRight);
						evilRanges.Add((evilLeft, evilRight, i.EdgeGivePercent));
						i.GenerateEvil(evilMid, evilLeft, evilRight);
						passesDone++;
					}
					i.PostGenerateEvil();
				}
				drunkIter++;
			});
			evilRanges = [];

			return false;
		}
	}

	public abstract class EvilBiomeGenerationPass {
		private const int beachBordersWidth = 275;
		private const int beachSandRandomCenter = beachBordersWidth + 5 + 40;
		private const int evilBiomeBeachAvoidance = beachSandRandomCenter + 60;
		public virtual int EvilBiomeAvoidanceMidFixer => 50;
		public virtual int NonDrunkBorderDist => 500;
		public virtual int DungeonGive => 150;

		public virtual int DrunkRNGMapCenterGive => 200; //100 if crimson

		public virtual bool CanGenerateNearDungeonOcean => true;
		/// <summary>
		/// The amount of this biome's area in which other evil biomes generating will be disincentivized rather than outright prohibited
		/// </summary>
		public virtual float EdgeGivePercent => 0.25f;
		public virtual string ProgressMessage => "";

		/* This is the code which allows you to spawn the evil */
		public abstract void GenerateEvil(int evilBiomePosition, int evilBiomePositionWestBound, int evilBiomePositionEastBound);

		/// <summary>
		/// Use this if you need to do stuff after spawning all chasms.
		/// </summary>
		public abstract void PostGenerateEvil();

		/// <summary>
		/// Call this method if you somehow need to know a valid evil spawn location.
		/// <br/>This is automatically called when generating the world.
		/// <br/>This is a very long function. Please do not overwrite this unless you absolutely know what you are doing.
		/// </summary>
		/// <param name="dungeonSide"></param>
		/// <param name="dungeonLocation"></param>
		/// <param name="SnowBoundMinX"></param>
		/// <param name="SnowBoundMaxX"></param>
		/// <param name="JungleBoundMinX"></param>
		/// <param name="JungleBoundMaxX"></param>
		/// <param name="currentDrunkIter"></param>
		/// <param name="maxDrunkBorders"></param>
		/// <param name="evilBiomePosition"></param>
		/// <param name="evilBiomePositionWestBound"></param>
		/// <param name="evilBiomePositionEastBound"></param>
		public virtual void GetEvilSpawnLocation(
			int dungeonSide,
			int dungeonLocation,

			int SnowBoundMinX,
			int SnowBoundMaxX,
			int JungleBoundMinX,
			int JungleBoundMaxX,

			int currentDrunkIter,
			int maxDrunkBorders,

			out int evilBiomePosition, out int evilBiomePositionWestBound, out int evilBiomePositionEastBound) {
			DefaultGetEvilSpawnLocation(SnowBoundMinX, SnowBoundMaxX, JungleBoundMinX, JungleBoundMaxX, currentDrunkIter, maxDrunkBorders, out evilBiomePosition, out evilBiomePositionWestBound, out evilBiomePositionEastBound);
			//START GENERATING!
		}
		public void DefaultGetEvilSpawnLocation(int SnowBoundMinX, int SnowBoundMaxX, int JungleBoundMinX, int JungleBoundMaxX, int currentDrunkIter, int maxDrunkBorders, out int evilBiomePosition, out int evilBiomePositionWestBound, out int evilBiomePositionEastBound, int baseExtent = 100, int rngExtent = 200) {

			bool FoundEvilLocation = false;
			evilBiomePosition = 0;
			evilBiomePositionWestBound = 0;
			evilBiomePositionEastBound = 0;
			int MapCenter = Main.maxTilesX / 2;
			int MapCenterGive = WorldGen.drunkWorldGen ? DrunkRNGMapCenterGive : 200;

			int tries;
			string newSystemFailReason = $"RNG could not reach non-zero total weight";
			int minPriority = 0;
			while (minPriority <= 5) {
				PegasusLib.RangeRandom rand = new(WorldGen.genRand, 0, Main.maxTilesX);
				void ProtectRange(int min, int max, int priority, int padding = 200) {
					if (priority < minPriority) return;
					rand.Multiply(min, max, 0);
					rand.Multiply(min - padding, max + padding, 0.5);
				}
				int beachPadding = minPriority < 2 ? 100 : 0;
				ProtectRange(0, evilBiomeBeachAvoidance, 3, beachPadding);
				ProtectRange(Main.maxTilesX - evilBiomeBeachAvoidance, Main.maxTilesX, 3, beachPadding);
				if (!Main.remixWorld) ProtectRange(MapCenter - MapCenterGive, MapCenter + MapCenterGive, 5, 100);
				ProtectRange(GenVars.UndergroundDesertLocation.X, GenVars.UndergroundDesertLocation.X + GenVars.UndergroundDesertLocation.Width, 4);
				ProtectRange(GenVars.dungeonLocation - DungeonGive, GenVars.dungeonLocation + DungeonGive, 5, 100 - minPriority * 20);
				ProtectRange(SnowBoundMinX, SnowBoundMaxX, 3);
				foreach ((int min, int max, float edgeGivePercent) in EvilBiomeGenerationPassHandler.evilRanges) {
					int padding = (int)((max - min) * edgeGivePercent * 0.5f);
					ProtectRange(min + padding, max - padding, 4, padding);
				}
				ProtectRange(JungleBoundMinX, JungleBoundMaxX, 5);

				if (rand.AnyWeight) {
					/*for (int i = 0; i < Main.maxTilesX; i++) {
						double weight = rand.GetWeight(i);
						byte paintType = weight == 0 ? PaintID.ShadowPaint : PaintID.DeepRedPaint;
						if (weight == 1) paintType = PaintID.None;
						for (int j = 0; j < Main.maxTilesY; j++) {
							Tile tile = Main.tile[i, j];
							tile.TileColor = paintType;
						}
					}*/
					tries = 0;
					int bestExtent = 0;
					while (tries < 100) {
						evilBiomePosition = rand.Get();
						int westExtent = WorldGen.genRand.Next(rngExtent) + baseExtent;
						int eastExtent = WorldGen.genRand.Next(rngExtent) + baseExtent;
						evilBiomePositionWestBound = evilBiomePosition;
						evilBiomePositionEastBound = evilBiomePosition;
						for (int i = 0; i < westExtent; i++) {
							if (evilBiomePositionWestBound - 1 < evilBiomeBeachAvoidance || rand.GetWeight(evilBiomePositionWestBound - 1) == 0) {
								westExtent = i;
								break;
							}
							evilBiomePositionWestBound--;
						}
						for (int i = 0; i < eastExtent; i++) {
							if (evilBiomePositionEastBound + 1 > Main.maxTilesX - evilBiomeBeachAvoidance || rand.GetWeight(evilBiomePositionEastBound + 1) == 0) {
								eastExtent = i;
								break;
							}
							evilBiomePositionEastBound++;
						}
						if (westExtent + eastExtent >= 150) {
							/*if (evilBiomePositionWestBound < evilBiomeBeachAvoidance) {
								evilBiomePositionWestBound = evilBiomeBeachAvoidance;
							}
							if (evilBiomePositionEastBound > Main.maxTilesX - evilBiomeBeachAvoidance) {
								evilBiomePositionEastBound = Main.maxTilesX - evilBiomeBeachAvoidance;
							}*/
							if (evilBiomePosition < evilBiomePositionWestBound + EvilBiomeAvoidanceMidFixer) {
								evilBiomePosition = evilBiomePositionWestBound + EvilBiomeAvoidanceMidFixer;
							}
							if (evilBiomePosition > evilBiomePositionEastBound - EvilBiomeAvoidanceMidFixer) {
								evilBiomePosition = evilBiomePositionEastBound - EvilBiomeAvoidanceMidFixer;
							}
							return;
						}
						if (bestExtent < westExtent + eastExtent) {
							newSystemFailReason = $"Could not find large enough area, largest was {westExtent}+{eastExtent}";
						}
						tries++;
					}
				}
				minPriority++;
			}

			AltLibrary.Instance.Logger.Warn($"Could not find location with new system (reason:\"{newSystemFailReason}\"), falling back to old system");
			tries = 0;
			while (!FoundEvilLocation) {
				FoundEvilLocation = true;

				if (WorldGen.drunkWorldGen) {

					int diff = Main.maxTilesX - NonDrunkBorderDist - NonDrunkBorderDist;

					int left = NonDrunkBorderDist + diff * currentDrunkIter / maxDrunkBorders;
					int right = NonDrunkBorderDist + diff * (currentDrunkIter + 1) / maxDrunkBorders;

					evilBiomePosition = WorldGen.genRand.Next(left, right);

					/*
					if (drunkRNGTilt)
						evilBiomePosition = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.5), Main.maxTilesX - nonDrunkBorderDist);
					else
						evilBiomePosition = WorldGen.genRand.Next(nonDrunkBorderDist, (int)((double)Main.maxTilesX * 0.5));*/
				} else {
					evilBiomePosition = WorldGen.genRand.Next(NonDrunkBorderDist, Main.maxTilesX - NonDrunkBorderDist);
				}
				evilBiomePositionWestBound = evilBiomePosition - WorldGen.genRand.Next(200) - 100;
				evilBiomePositionEastBound = evilBiomePosition + WorldGen.genRand.Next(200) + 100;

				if (evilBiomePositionWestBound < evilBiomeBeachAvoidance) {
					evilBiomePositionWestBound = evilBiomeBeachAvoidance;
				}
				if (evilBiomePositionEastBound > Main.maxTilesX - evilBiomeBeachAvoidance) {
					evilBiomePositionEastBound = Main.maxTilesX - evilBiomeBeachAvoidance;
				}
				if (evilBiomePosition < evilBiomePositionWestBound + EvilBiomeAvoidanceMidFixer) {
					evilBiomePosition = evilBiomePositionWestBound + EvilBiomeAvoidanceMidFixer;
				}
				if (evilBiomePosition > evilBiomePositionEastBound - EvilBiomeAvoidanceMidFixer) {
					evilBiomePosition = evilBiomePositionEastBound - EvilBiomeAvoidanceMidFixer;
				}
				//DIFFERENCE 2 - CRIMSON ONLY
				if (!CanGenerateNearDungeonOcean) {
					if (GenVars.dungeonSide < 0 && evilBiomePositionWestBound < 400) {
						evilBiomePositionWestBound = 400;
					} else if (GenVars.dungeonSide > 0 && evilBiomePositionWestBound > Main.maxTilesX - 400) {
						evilBiomePositionWestBound = Main.maxTilesX - 400;
					}
				}
				//DIFFERENCE 2 END
				if (!Main.remixWorld && evilBiomePosition > MapCenter - MapCenterGive && evilBiomePosition < MapCenter + MapCenterGive) {
					FoundEvilLocation = false;
				}
				if (!Main.remixWorld && evilBiomePositionWestBound > MapCenter - MapCenterGive && evilBiomePositionWestBound < MapCenter + MapCenterGive) {
					FoundEvilLocation = false;
				}
				if (!Main.remixWorld && evilBiomePositionEastBound > MapCenter - MapCenterGive && evilBiomePositionEastBound < MapCenter + MapCenterGive) {
					FoundEvilLocation = false;
				}
				if (tries < 200 && evilBiomePosition > GenVars.UndergroundDesertLocation.X && evilBiomePosition < GenVars.UndergroundDesertLocation.X + GenVars.UndergroundDesertLocation.Width) {
					FoundEvilLocation = false;
				}
				if (tries < 200 && evilBiomePositionWestBound > GenVars.UndergroundDesertLocation.X && evilBiomePositionWestBound < GenVars.UndergroundDesertLocation.X + GenVars.UndergroundDesertLocation.Width) {
					FoundEvilLocation = false;
				}
				if (tries < 200 && evilBiomePositionEastBound > GenVars.UndergroundDesertLocation.X && evilBiomePositionEastBound < GenVars.UndergroundDesertLocation.X + GenVars.UndergroundDesertLocation.Width) {
					FoundEvilLocation = false;
				}
				if (tries < 1000 && evilBiomePositionWestBound < GenVars.dungeonLocation + DungeonGive && evilBiomePositionEastBound > GenVars.dungeonLocation - DungeonGive) {
					FoundEvilLocation = false;
				}
				if (tries < 100 && evilBiomePositionWestBound < SnowBoundMinX && evilBiomePositionEastBound > SnowBoundMaxX) {
					SnowBoundMinX++;
					SnowBoundMaxX--;
					FoundEvilLocation = false;
				}
				if (tries < 500 && evilBiomePositionWestBound < JungleBoundMinX && evilBiomePositionEastBound > JungleBoundMaxX) {
					JungleBoundMinX++;
					JungleBoundMaxX--;
					FoundEvilLocation = false;
				}
				/*#if DEBUG
								for (int i = 0; i <= tries; i++) {
									Tile tile = Framing.GetTileSafely(evilBiomePosition, ((int)GenVars.worldSurfaceLow - 50) + i);
									tile.ResetToType(TileID.LihzahrdBrick);
								}
				#endif*/
			}
		}
	}
}
