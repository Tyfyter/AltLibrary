using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltBiomes {
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
		/*public delegate bool _HookPreConvertTile(AltBiome currentBiome, AltBiome newBiome, int i, int j);
		public static readonly GlobalHookList<GlobalBiome, _HookPreConvertTile> HookPreConvertTile = ItemLoader.AddModHook(new HookList<GlobalItem, Delegate>(
		//Method reference
			typeof(IShowItemCrosshair).GetMethod(nameof(IShowItemCrosshair.ShowItemCrosshair)),
			//Invocation
			e => (Item item, Player player) => {
				foreach (IShowItemCrosshair g in e.Enumerate(item)) {
					if (g.ShowItemCrosshair(item, player)) {
						return true;
					}
				}

				return false;
			}
		));*/
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
			foreach (var global in AltLibrary.GlobalBiomes) {
				global.PostConvertTile(oldBiome, newBiome, i, j);
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
			foreach (var global in AltLibrary.GlobalBiomes) {
				global.PostConvertWall(oldBiome, newBiome, i, j);
			}
		}
	}
}
