using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Core.Baking
{
	internal abstract class BlockParentageData
	{
		//tiles
		public Dictionary<int, (int, AltBiome)> Parent = new();

		public Dictionary<int, int> Deconversion => new(Parent.Where(i => !NoDeconversion.Contains(i.Key)).Select(i => new KeyValuePair<int, int>(i.Key, i.Value.Item1)));
		public HashSet<int> NoDeconversion = new();

		public Dictionary<int, BitsByte> BreakIfConversionFail = new();

		public abstract void Bake();

		public int GetConverted(int baseTile, Func<int, int> GetAltBlock, int ConversionType)
		{
			int ForcedConvertedTile = -1;
			while (true)
			{
				int test = GetAltBlock(baseTile);
				if (test != -1)
					return test;
				if (BreakIfConversionFail.TryGetValue(baseTile, out BitsByte bits))
				{
					if (bits[ConversionType])
						ForcedConvertedTile = -2; //change this to make use of spraytype
				}
				if (!Parent.TryGetValue(baseTile, out (int test, AltBiome biome) value))
					return ForcedConvertedTile;
				baseTile = value.test;
			}
		}
	}
	internal class TileParentageData : BlockParentageData
	{
		public override void Bake()
		{
			// Mass Parenting

			for (int x = 0; x < TileLoader.TileCount; x++)
			{
				if (TileID.Sets.Conversion.GolfGrass[x])
					Parent.TryAdd(x, (TileID.GolfGrass, null));
				else if (TileID.Sets.Conversion.Grass[x])
					Parent.TryAdd(x, (TileID.Grass, null));
				else if (TileID.Sets.Conversion.JungleGrass[x])
					Parent.TryAdd(x, (TileID.JungleGrass, null));
				else if (TileID.Sets.Conversion.MushroomGrass[x])
					Parent.TryAdd(x, (TileID.MushroomGrass, null));
				else if (Main.tileMoss[x] && x != TileID.Stone)
				{
					NoDeconversion.Add(x); //prevents deconversion of moss to stone
					Parent.TryAdd(x, (TileID.Stone, null));
				}
				else if (TileID.Sets.Conversion.Stone[x])
					Parent.TryAdd(x, (TileID.Stone, null));
				else if (TileID.Sets.Conversion.Ice[x])
					Parent.TryAdd(x, (TileID.IceBlock, null));
				else if (TileID.Sets.Conversion.Sandstone[x])
					Parent.TryAdd(x, (TileID.Sandstone, null));
				else if (TileID.Sets.Conversion.HardenedSand[x])
					Parent.TryAdd(x, (TileID.HardenedSand, null));
				else if (TileID.Sets.Conversion.Sand[x])
					Parent.TryAdd(x, (TileID.Sand, null));
			}

			BreakIfConversionFail.TryAdd(TileID.JungleThorns, new(true, true, true, true));

			BreakIfConversionFail.TryAdd(TileID.CorruptThorns, new(true, true, true, true));

			BreakIfConversionFail.TryAdd(TileID.CrimsonThorns, new(true, true, true, true));
		}
	}

	internal class WallParentageData : BlockParentageData
	{
		const int GRASS_UNSAFE_DIFFERENT = -3;
		public override void Bake()
		{
			for (int x = 0; x < WallLoader.WallCount; x++)
			{
				if (WallID.Sets.Conversion.Grass[x] && x != WallID.Grass)
				{
					switch (x)
					{
						case WallID.CorruptGrassUnsafe:
						case WallID.CrimsonGrassUnsafe:
						case WallID.HallowedGrassUnsafe:
							Parent.TryAdd(x, (GRASS_UNSAFE_DIFFERENT, null));
							break;
						default:
							Parent.TryAdd(x, (WallID.Grass, null));
							break;
					}
				}
				else if (WallID.Sets.Conversion.Stone[x] && x != WallID.Stone)
					Parent.TryAdd(x, (WallID.Stone, null));
				else if (WallID.Sets.Conversion.HardenedSand[x] && x != WallID.HardenedSand)
					Parent.TryAdd(x, (WallID.HardenedSand, null));
				else if (WallID.Sets.Conversion.Sandstone[x] && x != WallID.Sandstone)
					Parent.TryAdd(x, (WallID.Sandstone, null));
				else if (WallID.Sets.Conversion.NewWall1[x] && x != WallID.RocksUnsafe1)
					Parent.TryAdd(x, (WallID.RocksUnsafe1, null));
				else if (WallID.Sets.Conversion.NewWall2[x] && x != WallID.RocksUnsafe2)
					Parent.TryAdd(x, (WallID.RocksUnsafe2, null));
				else if (WallID.Sets.Conversion.NewWall3[x] && x != WallID.RocksUnsafe3)
					Parent.TryAdd(x, (WallID.RocksUnsafe3, null));
				else if (WallID.Sets.Conversion.NewWall4[x] && x != WallID.RocksUnsafe4)
					Parent.TryAdd(x, (WallID.RocksUnsafe4, null));
			}

			//Manual Grass conversionating to ensure safe grass walls cannot become unsafe through green solution conversion

			Parent.TryAdd(GRASS_UNSAFE_DIFFERENT, (WallID.Grass, null));

			Parent[WallID.Cave7Echo] = (WallID.Cave7Unsafe, null);
			Parent[WallID.Cave8Echo] = (WallID.Cave8Unsafe, null);
			Parent[WallID.MushroomUnsafe] = (WallID.JungleUnsafe, null);

			Parent.TryAdd(WallID.JungleUnsafe1, (WallID.JungleUnsafe, null));
			Parent.TryAdd(WallID.JungleUnsafe2, (WallID.JungleUnsafe, null));
			Parent.TryAdd(WallID.JungleUnsafe3, (WallID.JungleUnsafe, null));
			Parent.TryAdd(WallID.JungleUnsafe4, (WallID.JungleUnsafe, null));

			Parent.TryAdd(WallID.Jungle1Echo, (WallID.Jungle, null));
			Parent.TryAdd(WallID.Jungle2Echo, (WallID.Jungle, null));
			Parent.TryAdd(WallID.Jungle3Echo, (WallID.Jungle, null));
			Parent.TryAdd(WallID.Jungle4Echo, (WallID.Jungle, null));
		}
	}

	internal static class ALConvertInheritanceData
	{
		internal static TileParentageData tileParentageData;
		internal static WallParentageData wallParentageData;

		internal class ALConvertInheritanceData_Loader : ILoadable
		{
			public void Load(Mod mod)
			{
				tileParentageData = new();
				wallParentageData = new();
			}

			public void Unload()
			{
				tileParentageData = null;
				wallParentageData = null;
			}
		}

		public static void FillData()
		{
			// Mass Parenting
			tileParentageData.Bake();
			wallParentageData.Bake();
		}

		public static int GetUltimateParent(int baseTile)
		{
			while (true)
			{
				if (!tileParentageData.Parent.TryGetValue(baseTile, out (int tileType, AltBiome) test))
					return baseTile;
				baseTile = test.tileType;
			}
		}
	}
}
