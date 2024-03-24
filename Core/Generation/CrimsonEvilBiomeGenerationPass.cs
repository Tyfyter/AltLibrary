using AltLibrary.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace AltLibrary.Core.Generation
{
	internal class CrimsonEvilBiomeGenerationPass : EvilBiomeGenerationPass
	{
		public override string ProgressMessage => Lang.gen[72].Value;
		public override int DrunkRNGMapCenterGive => 100;
		public override void GenerateEvil(int evilBiomePosition, int evilBiomePositionWestBound, int evilBiomePositionEastBound)
		{
			WorldGen.CrimStart(evilBiomePosition, (int)GenVars.worldSurfaceLow - 10);
			double num22 = Main.worldSurface + 40.0;
			int worldSurfaceLow = (int)GenVars.worldSurfaceLow;
			for (int l = evilBiomePositionWestBound; l < evilBiomePositionEastBound; l++)
			{
				num22 += WorldGen.genRand.Next(-2, 3);
				if (num22 < Main.worldSurface + 30.0)
				{
					num22 = Main.worldSurface + 30.0;
				}
				if (num22 > Main.worldSurface + 50.0)
				{
					num22 = Main.worldSurface + 50.0;
				}
				int i2 = l;
				bool flag4 = false;
				while (worldSurfaceLow < num22)
				{
					if (Main.tile[i2, worldSurfaceLow].HasTile)
					{
						if (Main.tile[i2, worldSurfaceLow].TileType == 53 && i2 >= evilBiomePositionWestBound + WorldGen.genRand.Next(5) && i2 <= evilBiomePositionEastBound - WorldGen.genRand.Next(5))
						{
							Main.tile[i2, worldSurfaceLow].TileType = 234;
						}
						if (worldSurfaceLow < Main.worldSurface - 1.0 && !flag4) {
							if (Main.tile[i2, worldSurfaceLow].TileType == TileID.Dirt) {
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(i2, worldSurfaceLow, TileID.Dirt, TileID.CrimsonGrass, true);
							} else if (Main.tile[i2, worldSurfaceLow].TileType == TileID.Mud) {
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(i2, worldSurfaceLow, TileID.Mud, TileID.CrimsonJungleGrass);
							}
						}
						flag4 = true;
						if (Main.tile[i2, worldSurfaceLow].WallType == 216)
						{
							Main.tile[i2, worldSurfaceLow].WallType = 218;
						}
						else if (Main.tile[i2, worldSurfaceLow].WallType == 187)
						{
							Main.tile[i2, worldSurfaceLow].WallType = 221;
						}
						if (Main.tile[i2, worldSurfaceLow].TileType == 1)
						{
							if (i2 >= evilBiomePositionWestBound + WorldGen.genRand.Next(5) && i2 <= evilBiomePositionEastBound - WorldGen.genRand.Next(5))
							{
								Main.tile[i2, worldSurfaceLow].TileType = 203;
							}
						}
						else if (Main.tile[i2, worldSurfaceLow].TileType == 2)
						{
							Main.tile[i2, worldSurfaceLow].TileType = 199;
						}
						else if (Main.tile[i2, worldSurfaceLow].TileType == 161)
						{
							Main.tile[i2, worldSurfaceLow].TileType = 200;
						}
						else if (Main.tile[i2, worldSurfaceLow].TileType == 396)
						{
							Main.tile[i2, worldSurfaceLow].TileType = 401;
						}
						else if (Main.tile[i2, worldSurfaceLow].TileType == 397)
						{
							Main.tile[i2, worldSurfaceLow].TileType = 399;
						}
					}
					worldSurfaceLow++;
				}
			}
			int num24 = WorldGen.genRand.Next(10, 15);
			for (int m = 0; m < num24; m++)
			{
				int num25 = 0;
				bool flag5 = false;
				int num26 = 0;
				while (!flag5)
				{
					num25++;
					int x = WorldGen.genRand.Next(evilBiomePositionWestBound - num26, evilBiomePositionEastBound + num26);
					int num27 = WorldGen.genRand.Next((int)(Main.worldSurface - num26 / 2), (int)(Main.worldSurface + 100.0 + num26));
					while (WorldGen.oceanDepths(x, num27))
					{
						x = WorldGen.genRand.Next(evilBiomePositionWestBound - num26, evilBiomePositionEastBound + num26);
						num27 = WorldGen.genRand.Next((int)(Main.worldSurface - num26 / 2), (int)(Main.worldSurface + 100.0 + num26));
					}
					if (num25 > 100)
					{
						num26++;
						num25 = 0;
					}
					if (!Main.tile[x, num27].HasTile)
					{
						while (!Main.tile[x, num27].HasTile)
						{
							num27++;
						}
						num27--;
					}
					else
					{
						while (Main.tile[x, num27].HasTile && num27 > Main.worldSurface)
						{
							num27--;
						}
					}
					if ((num26 > 10 || Main.tile[x, num27 + 1].HasTile && Main.tile[x, num27 + 1].TileType == 203) && !WorldGen.IsTileNearby(x, num27, 26, 3))
					{
						WorldGen.Place3x2(x, num27, 26, 1);
						if (Main.tile[x, num27].TileType == 26)
						{
							flag5 = true;
						}
					}
					if (num26 > 100)
					{
						flag5 = true;
					}
				}
			}

			WorldBiomeGeneration.EvilBiomeGenRanges.Add(new Rectangle(
				evilBiomePositionWestBound,
				worldSurfaceLow,
				evilBiomePositionEastBound - evilBiomePositionWestBound,
				(int)GenVars.worldSurfaceHigh - worldSurfaceLow + 500
			));
		}

		public override void PostGenerateEvil()
		{
			bool worldCrimson = WorldGen.crimson;
			WorldGen.crimson = false;
			WorldGen.CrimPlaceHearts();
			WorldGen.crimson = worldCrimson;
		}
	}
}
