using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace AltLibrary.Common.AltBiomes; 
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

	/// <summary>
	/// Called before a tile attempts to spread to nearby blocks
	/// Return false to prevent spreading
	/// </summary>
	/// <returns></returns>
	public virtual bool PreSpread(AltBiome biome, int i, int j) => true;

	/// <summary>
	/// Called just before a tile spreads to nearby blocks
	/// </summary>
	/// <returns></returns>
	public virtual void OnSpread(AltBiome biome, int i, int j) { }
}
public static class GlobalBiomeHooks {
	private static readonly List<HookList<GlobalBiome>> hooks = [];
	delegate bool DelegatePreConvertTile(AltBiome currentBiome, AltBiome newBiome, int i, int j);
	private static readonly HookList<GlobalBiome> HookPreConvertTile = AddHook<DelegatePreConvertTile>(g => g.PreConvertTile);
	/// <summary>
	/// Called before a tile is converted to a different biome
	/// Return false to prevent conversion
	/// </summary>
	/// <returns></returns>
	public static bool PreConvertTile(AltBiome currentBiome, AltBiome newBiome, int i, int j) =>
		HookPreConvertTile.Enumerate().TrueForAll(g => g.PreConvertTile(currentBiome, newBiome, i, j)) && (currentBiome?.ConvertTileAway(i, j) ?? true);

	delegate void DelegatePostConvertTile(AltBiome oldBiome, AltBiome newBiome, int i, int j);
	private static readonly HookList<GlobalBiome> HookPostConvertTile = AddHook<DelegatePostConvertTile>(g => g.PostConvertTile);
	/// <summary>
	/// Called after a tile is converted
	/// </summary>
	public static void PostConvertTile(AltBiome oldBiome, AltBiome newBiome, int i, int j) {
		foreach (GlobalBiome global in HookPostConvertTile.Enumerate()) {
			global.PostConvertTile(oldBiome, newBiome, i, j);
		}
	}

	delegate bool DelegatePreSpread(AltBiome biome, int i, int j);
	private static readonly HookList<GlobalBiome> HookPreSpread = AddHook<DelegatePreSpread>(g => g.PreSpread);
	/// <summary>
	/// Called before a tile attempts to spread to nearby blocks
	/// Return false to prevent spreading
	/// </summary>
	public static bool PreSpread(AltBiome biome, int i, int j) =>
		HookPreSpread.Enumerate().TrueForAll(g => g.PreSpread(biome, i, j));

	delegate void DelegateOnSpread(AltBiome biome, int i, int j);
	private static readonly HookList<GlobalBiome> HookOnSpread = AddHook<DelegateOnSpread>(g => g.OnSpread);
	/// <summary>
	/// Called just before a tile spreads to nearby blocks
	/// </summary>
	public static void OnSpread(AltBiome biome, int i, int j) {
		foreach (GlobalBiome global in HookOnSpread.Enumerate()) {
			global.OnSpread(biome, i, j);
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

	delegate bool DelegatePreConvertWall(AltBiome currentBiome, AltBiome newBiome, int i, int j);
	private static readonly HookList<GlobalBiome> HookPreConvertWall = AddHook<DelegatePreConvertWall>(g => g.PreConvertWall);
	/// <summary>
	/// Called before a tile is converted to a different biome
	/// Return false to prevent conversion
	/// </summary>
	/// <returns></returns>
	public static bool PreConvertWall(AltBiome currentBiome, AltBiome newBiome, int i, int j) =>
		HookPreConvertWall.Enumerate().TrueForAll(g => g.PreConvertWall(currentBiome, newBiome, i, j)) && (currentBiome?.ConvertWallAway(i, j) ?? true);

	delegate void DelegatePostConvertWall(AltBiome oldBiome, AltBiome newBiome, int i, int j);
	private static readonly HookList<GlobalBiome> HookPostConvertWall = AddHook<DelegatePostConvertWall>(g => g.PostConvertWall);
	/// <summary>
	/// Called after a tile is converted
	/// </summary>
	public static void PostConvertWall(AltBiome oldBiome, AltBiome newBiome, int i, int j) {
		foreach (GlobalBiome global in HookPostConvertWall.Enumerate()) {
			global.PostConvertWall(oldBiome, newBiome, i, j);
		}
	}
	class GlobalBiomeWallHookInvoker : GlobalWall {
		public override void Load() {
			On_WorldGen.ConvertWall += (orig, i, j, newType) => {
				if (PreConvertWall(WallSets.GetOwnerBiome(Main.tile[i, j].WallType), WallSets.GetOwnerBiome(newType), i, j)) orig(i, j, newType);
			};
		}

		public override void OnWallConverted(int i, int j, int fromType, int toType, int conversionType) {
			PostConvertWall(WallSets.GetOwnerBiome(fromType), WallSets.GetOwnerBiome(toType), i, j);
		}
	}
	private static HookList<GlobalBiome> AddHook<F>(Expression<Func<GlobalBiome, F>> func) where F : Delegate {
		HookList<GlobalBiome> hook = HookList<GlobalBiome>.Create(func);
		hooks.Add(hook);
		return hook;
	}

	private static void RebuildHooks() {
		foreach (HookList<GlobalBiome> hook in hooks) hook.Update(AltLibrary.GlobalBiomes);
	}
	class Rebuilder : ModSystem {
		public override void ResizeArrays() => RebuildHooks();
	}
	static bool TrueForAll<T>(this ReadOnlySpan<T> values, Predicate<T> predicate) {
		for (int i = 0; i < values.Length; i++) {
			if (!predicate(values[i])) return false;
		}
		return true;
	}
#if DEBUG
	class EmptyGlobalBiome : GlobalBiome {

	}
	class FullGlobalBiome : GlobalBiome {
		public override void OnSpread(AltBiome biome, int i, int j) {
			base.OnSpread(biome, i, j);
		}
		public override void PostConvertTile(AltBiome oldBiome, AltBiome newBiome, int i, int j) {
			base.PostConvertTile(oldBiome, newBiome, i, j);
		}
		public override void PostConvertWall(AltBiome oldBiome, AltBiome newBiome, int i, int j) {
			base.PostConvertWall(oldBiome, newBiome, i, j);
		}
		public override bool PreConvertTile(AltBiome currentBiome, AltBiome newBiome, int i, int j) {
			return base.PreConvertTile(currentBiome, newBiome, i, j);
		}
		public override bool PreConvertWall(AltBiome currentBiome, AltBiome newBiome, int i, int j) {
			return base.PreConvertWall(currentBiome, newBiome, i, j);
		}
		public override bool PreSpread(AltBiome biome, int i, int j) {
			return base.PreSpread(biome, i, j);
		}
	}
#endif
}
