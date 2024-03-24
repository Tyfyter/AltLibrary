using AltLibrary.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace AltLibrary.Core.Generation
{
	internal class CorruptionEvilBiomeGenerationPass : EvilBiomeGenerationPass
	{
		public override string ProgressMessage => Lang.gen[20].Value;
		public override bool CanGenerateNearDungeonOcean => false;

		public override void GenerateEvil(int evilBiomePosition, int evilBiomePositionWestBound, int evilBiomePositionEastBound)
		{
			bool worldCrimson = WorldGen.crimson;
			WorldGen.crimson = false;

			int num38 = 0;
			for (int n = evilBiomePositionWestBound; n < evilBiomePositionEastBound; n++)
			{
				if (num38 > 0)
				{
					num38--;
				}
				if (n == evilBiomePosition || num38 == 0)
				{
					int num39 = (int)GenVars.worldSurfaceLow;
					while (num39 < Main.worldSurface - 1.0)
					{
						if (Main.tile[n, num39].HasTile || Main.tile[n, num39].WallType > 0)
						{
							if (n == evilBiomePosition)
							{
								num38 = 20;
								WorldGen.ChasmRunner(n, num39, WorldGen.genRand.Next(150) + 150, true);
								break;
							}
							if (WorldGen.genRand.NextBool(35) && num38 == 0)
							{
								num38 = 30;
								bool makeOrb = true;
								WorldGen.ChasmRunner(n, num39, WorldGen.genRand.Next(50) + 50, makeOrb);
								break;
							}
							break;
						}
						else
						{
							num39++;
						}
					}
				}
			}
			double num43 = Main.worldSurface + 40.0;
			for (int num44 = evilBiomePositionWestBound; num44 < evilBiomePositionEastBound; num44++)
			{
				num43 += WorldGen.genRand.Next(-2, 3);
				if (num43 < Main.worldSurface + 30.0)
				{
					num43 = Main.worldSurface + 30.0;
				}
				if (num43 > Main.worldSurface + 50.0)
				{
					num43 = Main.worldSurface + 50.0;
				}
				int i2 = num44;
				bool flag7 = false;
				int num45 = (int)GenVars.worldSurfaceLow;
				while (num45 < num43)
				{
					if (Main.tile[i2, num45].HasTile)
					{
						if (Main.tile[i2, num45].TileType == 53 && i2 >= evilBiomePositionWestBound + WorldGen.genRand.Next(5) && i2 <= evilBiomePositionEastBound - WorldGen.genRand.Next(5))
						{
							Main.tile[i2, num45].TileType = 112;
						}
						if (num45 < Main.worldSurface - 1.0 && !flag7) {
							if (Main.tile[i2, num45].TileType == TileID.Dirt) {
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(i2, num45, TileID.Dirt, TileID.CorruptGrass, true);
							} else if (Main.tile[i2, num45].TileType == TileID.Mud) {
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(i2, num45, TileID.Mud, TileID.CorruptJungleGrass);
							}
						}
						flag7 = true;
						if (Main.tile[i2, num45].TileType == 1 && i2 >= evilBiomePositionWestBound + WorldGen.genRand.Next(5) && i2 <= evilBiomePositionEastBound - WorldGen.genRand.Next(5))
						{
							Main.tile[i2, num45].TileType = 25;
						}
						if (Main.tile[i2, num45].WallType == 216)
						{
							Main.tile[i2, num45].WallType = 217;
						}
						else if (Main.tile[i2, num45].WallType == 187)
						{
							Main.tile[i2, num45].WallType = 220;
						}
						if (Main.tile[i2, num45].TileType == 2)
						{
							Main.tile[i2, num45].TileType = 23;
						}
						if (Main.tile[i2, num45].TileType == 161)
						{
							Main.tile[i2, num45].TileType = 163;
						}
						else if (Main.tile[i2, num45].TileType == 396)
						{
							Main.tile[i2, num45].TileType = 400;
						}
						else if (Main.tile[i2, num45].TileType == 397)
						{
							Main.tile[i2, num45].TileType = 398;
						}
					}
					num45++;
				}
			}
			for (int num46 = evilBiomePositionWestBound; num46 < evilBiomePositionEastBound; num46++)
			{
				for (int num47 = 0; num47 < Main.maxTilesY - 50; num47++)
				{
					if (Main.tile[num46, num47].HasTile && Main.tile[num46, num47].TileType == 31)
					{
						int num48 = num46 - 13;
						int num49 = num46 + 13;
						int num50 = num47 - 13;
						int num51 = num47 + 13;
						for (int num52 = num48; num52 < num49; num52++)
						{
							if (num52 > 10 && num52 < Main.maxTilesX - 10)
							{
								for (int num53 = num50; num53 < num51; num53++)
								{
									if (Math.Abs(num52 - num46) + Math.Abs(num53 - num47) < 9 + WorldGen.genRand.Next(11) && !WorldGen.genRand.NextBool(3) && Main.tile[num52, num53].TileType != 31)
									{
										Main.tile[num52, num53].TileType = 0;
										Main.tile[num52, num53].TileType = 25;
										if (Math.Abs(num52 - num46) <= 1 && Math.Abs(num53 - num47) <= 1)
										{
											Main.tile[num52, num53].TileType = 0;
										}
									}
									if (Main.tile[num52, num53].TileType != 31 && Math.Abs(num52 - num46) <= 2 + WorldGen.genRand.Next(3) && Math.Abs(num53 - num47) <= 2 + WorldGen.genRand.Next(3))
									{
										Main.tile[num52, num53].TileType = 0;
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
