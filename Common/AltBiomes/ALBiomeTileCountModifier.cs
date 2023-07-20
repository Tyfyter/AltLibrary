using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltBiomes
{
	public abstract class ALBiomeTileCountModifier : ModSystem
	{
		public enum TileCountType
		{
			HALLOW,
			EVIL,
			SUNFLOWER,
		}

		public virtual TileCountType TileType => TileCountType.HALLOW;

		internal int TileCount;

		public abstract int GetTileCount(ReadOnlySpan<int> TileCounts);

		public abstract void OnReceiveModifiedTileCount(int TileCount);

		public class ALBiomeTileCountModifier_Loader : ILoadable
		{
			public void Load(Mod mod)
			{
				HolyTileCountOriginal = 0;
				EvilTileCountOriginal = 0;
				ModSunflowerCount = 0;
				IL_SceneMetrics.ExportTileCountsToMain += SceneMetrics_GetModdedHallowEvil;
			}

			public void Unload()
			{
				HolyTileCountOriginal = 0;
				EvilTileCountOriginal = 0;
				ModSunflowerCount = 0;
			}
		}

		internal static void SceneMetrics_GetModdedHallowEvil(ILContext il)
		{
			ILCursor c = new(il);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldfld, typeof(SceneMetrics).GetField("_tileCounts", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
			c.EmitDelegate<Action<SceneMetrics, int[]>>(GetOriginalTileCounts);

			if (!c.TryGotoNext(i => i.MatchStloc(0)))
				throw new Exception("[SceneMetrics_GetModdedHallowEvil] Failed to find i => i.MatchStloc(0)");
			c.Index++;
			if (!c.TryGotoNext( // find first if (HolyTileCount < 0)
				i => i.MatchLdarg(0),
				i => i.MatchCall<SceneMetrics>("get_HolyTileCount"),
				i => i.MatchLdcI4(0),
				i => i.MatchBge(out ILLabel _)
			)) {
				AltLibrary.Instance.Logger.Info("10 $ 1");
			}
			c.Index--;
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldfld, typeof(SceneMetrics).GetField("_tileCounts", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
			c.EmitDelegate<Action<SceneMetrics, int[]>>(ExportTileCounts);
		}

		protected sealed override void Register()
		{
			AltLibrary.ALBiomeTileCountModifiers.Add(this);
		}

		internal static int HolyTileCountOriginal = 0;
		internal static int EvilTileCountOriginal = 0;
		internal static int SunflowerCountOriginal = 0;
		internal static int ModSunflowerCount = 0;

		internal static void GetOriginalTileCounts(SceneMetrics metrics, int[] tileCounts)
		{
			SunflowerCountOriginal = tileCounts[27];
			ModSunflowerCount = 0;
			AltLibrary.ALBiomeTileCountModifiers.ForEach(i =>
			{
				switch (i.TileType)
				{
					case TileCountType.SUNFLOWER:
						int TileCount = i.GetTileCount(tileCounts);
						i.TileCount = TileCount;
						ModSunflowerCount += TileCount;
						tileCounts[27] += TileCount;
						break;
				}
			});
			HolyTileCountOriginal = metrics.HolyTileCount;
			EvilTileCountOriginal = metrics.EvilTileCount + metrics.BloodTileCount;
		}
		//TODO: double check that this code makes sense to begin with
		internal static void ExportTileCounts(SceneMetrics metrics, int[] tileCounts)
		{
			int TotalHolyTiles = 0;
			int TotalEvilTiles = 0;
			AltLibrary.ALBiomeTileCountModifiers.ForEach(i =>
			{
				if (i.TileType == TileCountType.SUNFLOWER)
					return;
				int TileCount = i.GetTileCount(tileCounts);
				i.TileCount = TileCount;
				switch (i.TileType)
				{
					case TileCountType.HALLOW:
						TotalHolyTiles += TileCount;
						break;
					case TileCountType.EVIL:
						TotalEvilTiles += TileCount;
						break;
				}
			});

			metrics.HolyTileCount -= TotalEvilTiles;
			metrics.EvilTileCount -= TotalHolyTiles;
			metrics.BloodTileCount -= TotalHolyTiles;

			TotalHolyTiles += HolyTileCountOriginal;
			TotalEvilTiles += EvilTileCountOriginal;

			AltLibrary.ALBiomeTileCountModifiers.ForEach(i =>
			{
				switch (i.TileType)
				{
					case TileCountType.HALLOW:
						i.OnReceiveModifiedTileCount(Math.Max(i.TileCount - TotalEvilTiles, 0));
						break;
					case TileCountType.EVIL:
						i.OnReceiveModifiedTileCount(Math.Max(i.TileCount - TotalHolyTiles, 0));
						break;
				}
			});
		}
	}
}
