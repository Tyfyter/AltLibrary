using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Microsoft.Xna.Framework.Input;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Core.Baking {
	internal abstract class BlockParentageData {
		//tiles
		internal Dictionary<int, (int parentTile, AltBiome parentBiome)> UnbakedParents = new();
		(int parentTile, AltBiome parentBiome)[] parents;
		public ReadOnlySpan<(int parentTile, AltBiome parentBiome)> ParentData => parents;
		internal void SetupDeconversion() {
			parents = TileID.Sets.Factory.CreateCustomSet<(int, AltBiome)>((-1, null));
			Deconversion = TileID.Sets.Factory.CreateIntSet(-1);
			foreach (KeyValuePair<int, (int parentTile, AltBiome parentBiome)> item in UnbakedParents) {
				if (parents.IndexInRange(item.Key)) {
					parents[item.Key] = item.Value;
					if (!NoDeconversion.Contains(item.Key)) Deconversion[item.Key] = item.Value.parentTile;
				}
			}
		}
		public int[] Deconversion { get; private set; }
		public HashSet<int> NoDeconversion = [];

		public Dictionary<int, BitsByte> BreakIfConversionFail = new();

		public abstract void Bake();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AddParent(int type, (int parentTile, AltBiome parentBiome) parent) => UnbakedParents.TryAdd(type, parent);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetParent(int type, out (int parentTile, AltBiome parentBiome) parent) {
			parent = parents[type];
			return parent.parentTile != -1 || parent.parentBiome is not null;
		}
	}
	internal class TileParentageData : BlockParentageData {
		public override void Bake() {
			// Mass Parenting

			for (int x = 0; x < TileLoader.TileCount; x++) {
				if (TileID.Sets.Conversion.GolfGrass[x])
					AddParent(x, (TileID.GolfGrass, null));
				else if (TileID.Sets.Conversion.Grass[x])
					AddParent(x, (TileID.Grass, null));
				else if (TileID.Sets.Conversion.JungleGrass[x])
					AddParent(x, (TileID.JungleGrass, null));
				else if (TileID.Sets.Conversion.MushroomGrass[x])
					AddParent(x, (TileID.MushroomGrass, null));
				else if (Main.tileMoss[x] && x != TileID.Stone) {
					NoDeconversion.Add(x); //prevents deconversion of moss to stone
					AddParent(x, (TileID.Stone, null));
				} else if (TileID.Sets.Conversion.Stone[x])
					AddParent(x, (TileID.Stone, null));
				else if (TileID.Sets.Conversion.Ice[x])
					AddParent(x, (TileID.IceBlock, null));
				else if (TileID.Sets.Conversion.Sandstone[x])
					AddParent(x, (TileID.Sandstone, null));
				else if (TileID.Sets.Conversion.HardenedSand[x])
					AddParent(x, (TileID.HardenedSand, null));
				else if (TileID.Sets.Conversion.Sand[x])
					AddParent(x, (TileID.Sand, null));
			}

			BreakIfConversionFail.TryAdd(TileID.JungleThorns, new(true, true, true, true));

			BreakIfConversionFail.TryAdd(TileID.CorruptThorns, new(true, true, true, true));

			BreakIfConversionFail.TryAdd(TileID.CrimsonThorns, new(true, true, true, true));
		}
	}

	internal class WallParentageData : BlockParentageData {
		const int GRASS_UNSAFE_DIFFERENT = -3;
		public override void Bake() {
			for (int x = 0; x < WallLoader.WallCount; x++) {
				if (WallID.Sets.Conversion.Grass[x] && x != WallID.Grass) {
					switch (x) {
						case WallID.CorruptGrassUnsafe:
						case WallID.CrimsonGrassUnsafe:
						case WallID.HallowedGrassUnsafe:
						AddParent(x, (GRASS_UNSAFE_DIFFERENT, null));
						break;
						default:
						AddParent(x, (WallID.Grass, null));
						break;
					}
				} else if (WallID.Sets.Conversion.Stone[x] && x != WallID.Stone)
					AddParent(x, (WallID.Stone, null));
				else if (WallID.Sets.Conversion.HardenedSand[x] && x != WallID.HardenedSand)
					AddParent(x, (WallID.HardenedSand, null));
				else if (WallID.Sets.Conversion.Sandstone[x] && x != WallID.Sandstone)
					AddParent(x, (WallID.Sandstone, null));
				else if (WallID.Sets.Conversion.NewWall1[x] && x != WallID.RocksUnsafe1)
					AddParent(x, (WallID.RocksUnsafe1, null));
				else if (WallID.Sets.Conversion.NewWall2[x] && x != WallID.RocksUnsafe2)
					AddParent(x, (WallID.RocksUnsafe2, null));
				else if (WallID.Sets.Conversion.NewWall3[x] && x != WallID.RocksUnsafe3)
					AddParent(x, (WallID.RocksUnsafe3, null));
				else if (WallID.Sets.Conversion.NewWall4[x] && x != WallID.RocksUnsafe4)
					AddParent(x, (WallID.RocksUnsafe4, null));
			}

			//Manual Grass conversionating to ensure safe grass walls cannot become unsafe through green solution conversion

			AddParent(GRASS_UNSAFE_DIFFERENT, (WallID.Grass, null));

			UnbakedParents[WallID.Cave7Echo] = (WallID.Cave7Unsafe, null);
			UnbakedParents[WallID.Cave8Echo] = (WallID.Cave8Unsafe, null);
			UnbakedParents[WallID.MushroomUnsafe] = (WallID.JungleUnsafe, null);

			AddParent(WallID.JungleUnsafe1, (WallID.JungleUnsafe, null));
			AddParent(WallID.JungleUnsafe2, (WallID.JungleUnsafe, null));
			AddParent(WallID.JungleUnsafe3, (WallID.JungleUnsafe, null));
			AddParent(WallID.JungleUnsafe4, (WallID.JungleUnsafe, null));

			AddParent(WallID.Jungle1Echo, (WallID.Jungle, null));
			AddParent(WallID.Jungle2Echo, (WallID.Jungle, null));
			AddParent(WallID.Jungle3Echo, (WallID.Jungle, null));
			AddParent(WallID.Jungle4Echo, (WallID.Jungle, null));
		}
	}

	internal static class ALConvertInheritanceData {
		internal static TileParentageData tileParentageData;
		internal static WallParentageData wallParentageData;

		internal class ALConvertInheritanceData_Loader : ILoadable {
			public void Load(Mod mod) {
				tileParentageData = new();
				wallParentageData = new();
			}

			public void Unload() {
				tileParentageData = null;
				wallParentageData = null;
			}
		}

		public static void FillData() {
			// Mass Parenting
			tileParentageData.Bake();
			wallParentageData.Bake();
			tileParentageData.SetupDeconversion();
			wallParentageData.SetupDeconversion();
		}

		public static int GetUltimateParent(int baseTile) {
			while (true) {
				if (!tileParentageData.TryGetParent(baseTile, out (int tileType, AltBiome) test))
					return baseTile;
				baseTile = test.tileType;
			}
		}
	}
}
