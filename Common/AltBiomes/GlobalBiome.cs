using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltBiomes {
	/// TODO: use <see cref="Terraria.ModLoader.Core.GlobalHookList{GlobalBiome}"/>
	public abstract class GlobalBiome : ModType {
		protected sealed override void Register() {
			ModTypeLookup<GlobalBiome>.Register(this);
			AltLibrary.GlobalBiomes.Add(this);
		}
		/// <summary>
		/// Called before a tile is converted to a different biome
		/// Return false to prevent conversion
		/// </summary>
		/// <returns></returns>
		public virtual bool PreConvertTile(AltBiome currentBiome, AltBiome newBiome, int i, int j) => true;

		/// <summary>
		/// Called after a tile is converted
		/// </summary>
		public virtual void PostConvertTile(AltBiome oldBiome, AltBiome newBiome, int i, int j) { }

		/// <summary>
		/// Called before a tile is converted to a different biome
		/// Return false to prevent conversion
		/// </summary>
		/// <returns></returns>
		public virtual bool PreConvertWall(AltBiome currentBiome, AltBiome newBiome, int i, int j) => true;

		/// <summary>
		/// Called after a tile is converted
		/// </summary>
		public virtual void PostConvertWall(AltBiome oldBiome, AltBiome newBiome, int i, int j) { }
	}
	public static class GlobalBiomeHooks {
		/// <summary>
		/// Called before a tile is converted to a different biome
		/// Return false to prevent conversion
		/// </summary>
		/// <returns></returns>
		public static bool PreConvertTile(AltBiome currentBiome, AltBiome newBiome, int i, int j) =>
			AltLibrary.GlobalBiomes.TrueForAll(g => g.PreConvertTile(currentBiome, newBiome, i, j)) && (currentBiome?.ConvertTileAway(i, j) ?? true);

		/// <summary>
		/// Called after a tile is converted
		/// </summary>
		public static void PostConvertTile(AltBiome oldBiome, AltBiome newBiome, int i, int j) {
			foreach (GlobalBiome global in AltLibrary.GlobalBiomes) {
				global.PostConvertTile(oldBiome, newBiome, i, j);
			}
		}
		class GlobalBiomeTileHookInvoker : GlobalTile {
			public override void Load() {
				On_WorldGen.ConvertTile_int_int_int += (orig, i, j, newType) => {
					if (PreConvertTile(TileSets.GetOwnerBiome(Main.tile[i, j].TileType), TileSets.GetOwnerBiome(newType), i, j)) orig(i, j, newType);
				};
			}

			public override void OnTileConverted(int i, int j, int fromType, int toType, int conversionType) {
				PostConvertTile(TileSets.GetOwnerBiome(fromType), TileSets.GetOwnerBiome(toType), i, j);
			}
		}

		/// <summary>
		/// Called before a tile is converted to a different biome
		/// Return false to prevent conversion
		/// </summary>
		/// <returns></returns>
		public static bool PreConvertWall(AltBiome currentBiome, AltBiome newBiome, int i, int j) =>
			AltLibrary.GlobalBiomes.TrueForAll(g => g.PreConvertWall(currentBiome, newBiome, i, j)) && (currentBiome?.ConvertWallAway(i, j) ?? true);

		/// <summary>
		/// Called after a tile is converted
		/// </summary>
		public static void PostConvertWall(AltBiome oldBiome, AltBiome newBiome, int i, int j) {
			foreach (GlobalBiome global in AltLibrary.GlobalBiomes) {
				global.PostConvertWall(oldBiome, newBiome, i, j);
			}
		}
		class GlobalBiomeWallHookInvoker : GlobalWall {
			public override void Load() {
				On_WorldGen.ConvertWall += (orig, i, j, newType) => {
					if (PreConvertTile(WallSets.GetOwnerBiome(Main.tile[i, j].WallType), WallSets.GetOwnerBiome(newType), i, j)) orig(i, j, newType);
				};
			}

			public override void OnWallConverted(int i, int j, int fromType, int toType, int conversionType) {
				PostConvertTile(WallSets.GetOwnerBiome(fromType), WallSets.GetOwnerBiome(toType), i, j);
			}
		}
	}
}
