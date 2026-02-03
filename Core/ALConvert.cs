using AltLibrary.Common;
using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using AltLibrary.Core.Baking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AltLibrary.Core {
	public static class ALConvert {
		internal static void Load() {
			if (typeof(WorldGen).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance ).FirstOrDefault(m => m.Name.Contains("HardmodeGoodRemixTask")) is MethodInfo method) {
				MonoModHooks.Modify(method, IL_WorldGen_smCallBack_HardmodeGoodRemixTask);
			} else {
				AltLibrary.Instance.Logger.Error("Could not find HardmodeGoodRemixTask in WorldGen");
			}
		}

		internal static void Unload() { }
		private static void IL_WorldGen_smCallBack_HardmodeGoodRemixTask(ILContext il) {
			ILCursor c = new(il);
			int count = 0;
			while (c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(out _),
				i => i.MatchLdloc(out _),
				i => i.MatchLdcI4(2),
				i => i.MatchLdcI4(1),
				i => i.MatchCall<WorldGen>("Convert")
			)) {
				count++;
				c.Index--;
				c.Remove();
				c.EmitDelegate<Action<int, int, int, int>>(RemixHardmodeConvert);
				int tile = -1;
				if (c.TryGotoPrev(MoveType.After,
					i => i.MatchLdloca(out tile),
					i => i.MatchCall<Tile>("get_type"),
					i => i.MatchLdindU2(),
					i => i.MatchLdelemU1(),
					i => i.MatchBrfalse(out _)
				)) {
					c.Index--;
					c.Emit(OpCodes.Ldloc, tile);
					c.EmitDelegate<Func<bool, Tile, bool>>(CheckTileEvil);
				}
			}
			if (count != 4) AltLibrary.Instance.Logger.Warn($"{count} Convert calls found in WorldGen_smCallBack, should be 4");
		}
		public static void RemixHardmodeConvert(int i, int j, int conversionType, int size = 4) {
			if (conversionType != 2 || size != 1) {
				WorldGen.Convert(i, j, conversionType, size, true);
				return;
			}
			AltBiome hallow = WorldBiomeManager.GetWorldHallow(true);
			Convert(hallow, i, j, 1);
			Tile tile = Framing.GetTileSafely(i, j);
			if (hallow.GERunnerConversion.TryGetValue(tile.TileType, out int value)) tile.TileType = (ushort)value;
		}
		static bool CheckTileEvil(bool evil, Tile tile) {
			if (evil) return true;
			return TileSets.OwnedByBiomeID[tile.TileType] >= 0 && (AltLibrary.GetAltBiome(TileSets.OwnedByBiomeID[tile.TileType])?.BiomeType.Equals(BiomeType.Evil) ?? false);
		}
		/// <summary>
		/// Makes throwing water converting effect.
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="fullName"></param>
		public static void SimulateThrownWater(Projectile projectile, string fullName) {
			int i = (int)(projectile.position.X + projectile.width / 2) / 16;
			int j = (int)(projectile.position.Y + projectile.height / 2) / 16;
			Convert(fullName, i, j, 4);
		}

		/// <summary>
		/// Makes throwing water converting effect.
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="mod"></param>
		/// <param name="name"></param>
		public static void SimulateThrownWater(Projectile projectile, Mod mod, string name) {
			int i = (int)(projectile.position.X + projectile.width / 2) / 16;
			int j = (int)(projectile.position.Y + projectile.height / 2) / 16;
			Convert(mod, name, i, j, 4);
		}

		/// <summary>
		/// Makes throwing water converting effect.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="projectile"></param>
		public static void SimulateThrownWater<T>(Projectile projectile) where T : AltBiome {
			int i = (int)(projectile.position.X + projectile.width / 2) / 16;
			int j = (int)(projectile.position.Y + projectile.height / 2) / 16;
			Convert<T>(i, j, 4);
		}

		/// <summary>
		/// Makes throwing water converting effect.
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="biome"></param>
		public static void SimulateThrownWater(Projectile projectile, AltBiome biome) {
			int i = (int)(projectile.position.X + projectile.width / 2) / 16;
			int j = (int)(projectile.position.Y + projectile.height / 2) / 16;
			Convert(biome, i, j, 4);
		}

		/// <summary>
		/// Makes solution converting effect.
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="mod"></param>
		/// <param name="name"></param>
		public static void SimulateSolution(Projectile projectile, Mod mod, string name) => SimulateSolution(projectile, AltLibrary.Biomes.Find(x => x.Mod == mod && x.Name == name));

		/// <summary>
		/// Makes solution converting effect.
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="fullname"></param>
		public static void SimulateSolution(Projectile projectile, string fullname) => SimulateSolution(projectile, AltLibrary.Biomes.Find(x => x.FullName == fullname));

		/// <summary>
		/// Makes solution converting effect.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="projectile"></param>
		public static void SimulateSolution<T>(Projectile projectile) where T : AltBiome => SimulateSolution(projectile, ContentInstance<T>.Instance);

		/// <summary>
		/// Makes solution converting effect.
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="biome"></param>
		public static void SimulateSolution(Projectile projectile, AltBiome biome) {
			Convert(biome, (int)(projectile.position.X + projectile.width / 2) / 16, (int)(projectile.position.Y + projectile.height / 2) / 16, 2);
		}

		public static void Convert<T>(int i, int j, int size = 4) where T : AltBiome => Convert(ContentInstance<T>.Instance, i, j, size);
		public static void Convert(string fullName, int i, int j, int size = 4) => Convert(AltLibrary.Biomes.Find(x => x.FullName == fullName), i, j, size);
		public static void Convert(Mod mod, string name, int i, int j, int size = 4) => Convert(AltLibrary.Biomes.Find(x => x.Mod == mod && x.Name == name), i, j, size);
		public static void Convert(AltBiome biome, int i, int j, int size = 4) {
			if (biome is null)
				throw new ArgumentNullException(nameof(biome), "Can't be null!");

			WorldGen.Convert(i, j, biome.ConversionType, size, true);
			return;
		}
		public delegate void ConversionOverrideHack(int baseTile, ref int newTile);
		[Obsolete("AltLibrary no longer replaces the vanilla conversion system", true)]
		public static (int newTile, AltBiome fromBiome) GetTileConversionState(int i, int j, AltBiome targetBiome) => default;
		[Obsolete("AltLibrary no longer replaces the vanilla conversion system", true)]
		public static void ConvertTile(int i, int j, AltBiome targetBiome, bool silent = false) {
			Tile tile = Main.tile[i, j];
			if (!tile.HasTile) return;
			(int newTile, AltBiome fromBiome) = GetTileConversionState(i, j, targetBiome);
			if (fromBiome == targetBiome || (fromBiome is null && targetBiome is DeconvertAltBiome)) return;
			if (Main.tileFrameImportant[tile.TileType] && TileObjectData.GetTileData(tile) is TileObjectData tileObjectData) {
				ConvertMultiTile(i, j, newTile, tileObjectData, fromBiome, targetBiome, silent);
				return;
			}
			WorldGen.Convert(i, j, targetBiome.ConversionType, 0, true, false);
		}
		public static void ConvertMultiTile(int i, int j, int newTile, TileObjectData objectData, AltBiome fromBiome, AltBiome targetBiome, bool silent = false) {
			Tile tile = Main.tile[i, j];
			int innerFrameY = tile.TileFrameY % objectData.CoordinateFullHeight;
			int frameI = (tile.TileFrameX % objectData.CoordinateFullWidth) / (objectData.CoordinateWidth + objectData.CoordinatePadding);
			int frameJ = 0;
			while (innerFrameY >= objectData.CoordinateHeights[frameJ] + objectData.CoordinatePadding) {
				innerFrameY -= objectData.CoordinateHeights[frameJ] + objectData.CoordinatePadding;
				frameJ++;
			}
			int left = i - frameI;
			int top = j - frameJ;
			if (fromBiome is not null && !fromBiome.PreConvertMultitileAway(left, top, objectData.Width, objectData.Height, ref newTile, targetBiome)) return;
			targetBiome.ConvertMultitileTo(left, top, objectData.Width, objectData.Height, newTile, fromBiome);
		}
		[Obsolete("AltLibrary no longer replaces the vanilla conversion system", true)]
		public static (int newWall, AltBiome fromBiome) GetWallConversionState(int i, int j, AltBiome targetBiome) => default;
		[Obsolete("AltLibrary no longer replaces the vanilla conversion system", true)]
		public static void ConvertWall(int i, int j, AltBiome targetBiome, bool silent = false) {
			WorldGen.Convert(i, j, targetBiome.BiomeConversionType, 0, false, true);
		}
	}
}
