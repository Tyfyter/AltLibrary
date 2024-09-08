using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using AltLibrary.Core.Baking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.GameContent.Bestiary.On_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;

namespace AltLibrary.Core
{
	public static class ALConvert
	{
		internal static void Load()
		{
			Terraria.On_WorldGen.Convert += WorldGen_Convert;
			//IL_Projectile.VanillaAI += IL_Projectile_VanillaAI;
			IL_WorldGen.smCallBack += IL_WorldGen_smCallBack;
		}

		internal static void Unload()
		{
		}

		private static void WorldGen_Convert(On_WorldGen.orig_Convert orig, int i, int j, int conversionType, int size) {
			AltBiome biome;
			switch (conversionType) {
				case 1:
				biome = ModContent.GetInstance<CorruptionAltBiome>();
				break;
				case 4:
				biome = ModContent.GetInstance<CrimsonAltBiome>();
				break;

				case 2:
				biome = ModContent.GetInstance<HallowAltBiome>();
				break;

				case 3:
				biome = ModContent.GetInstance<MushroomAltBiome>();
				break;

				case 5:
				biome = ModContent.GetInstance<DesertAltBiome>();
				break;

				case 6:
				biome = ModContent.GetInstance<SnowAltBiome>();
				break;

				case 7:
				biome = ModContent.GetInstance<ForestAltBiome>();
				break;

				default:
				biome = ModContent.GetInstance<DeconvertAltBiome>();
				break;
			}
			for (int k = i - size; k <= i + size; k++)
			{
				for (int l = j - size; l <= j + size; l++)
				{
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < 6)
					{
						ConvertTile(k, l, biome);

						ConvertWall(k, l, biome);
						continue;
					}
				}
			}
			return;
		}
		private static void IL_WorldGen_smCallBack(ILContext il) {
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
				WorldGen.Convert(i, j, conversionType, size);
				return;
			}
			AltBiome hallow = WorldBiomeManager.GetWorldHallow(true);
			Convert(hallow, i, j, 1);
			Tile tile = Framing.GetTileSafely(i, j);
			if (hallow.GERunnerConversion.TryGetValue(tile.TileType, out int value)) tile.TileType = (ushort)value;
		}
		static bool CheckTileEvil(bool evil, Tile tile) {
			if (!evil
				&& ALConvertInheritanceData.tileParentageData.Parent.TryGetValue(tile.TileType, out (int baseTile, AltBiome fromBiome) parent)
				&& parent.fromBiome?.BiomeType == BiomeType.Evil) return true;
			return evil;
		}
		/// <summary>
		/// Makes throwing water converting effect.
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="fullName"></param>
		public static void SimulateThrownWater(Projectile projectile, string fullName)
		{
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
		public static void SimulateThrownWater(Projectile projectile, Mod mod, string name)
		{
			int i = (int)(projectile.position.X + projectile.width / 2) / 16;
			int j = (int)(projectile.position.Y + projectile.height / 2) / 16;
			Convert(mod, name, i, j, 4);
		}

		/// <summary>
		/// Makes throwing water converting effect.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="projectile"></param>
		public static void SimulateThrownWater<T>(Projectile projectile) where T : AltBiome
		{
			int i = (int)(projectile.position.X + projectile.width / 2) / 16;
			int j = (int)(projectile.position.Y + projectile.height / 2) / 16;
			Convert<T>(i, j, 4);
		}

		/// <summary>
		/// Makes throwing water converting effect.
		/// </summary>
		/// <param name="projectile"></param>
		/// <param name="biome"></param>
		public static void SimulateThrownWater(Projectile projectile, AltBiome biome)
		{
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
		public static void SimulateSolution(Projectile projectile, AltBiome biome)
		{
			Convert(biome, (int)(projectile.position.X + projectile.width / 2) / 16, (int)(projectile.position.Y + projectile.height / 2) / 16, 2);
		}

		public static void Convert<T>(int i, int j, int size = 4) where T : AltBiome => Convert(ContentInstance<T>.Instance, i, j, size);

		public static void Convert(string fullName, int i, int j, int size = 4) => Convert(AltLibrary.Biomes.Find(x => x.FullName == fullName), i, j, size);

		public static void Convert(Mod mod, string name, int i, int j, int size = 4) => Convert(AltLibrary.Biomes.Find(x => x.Mod == mod && x.Name == name), i, j, size);

		public static void Convert(AltBiome biome, int i, int j, int size = 4)
		{
			if (biome is null)
				throw new ArgumentNullException(nameof(biome), "Can't be null!");
			for (int k = i - size; k <= i + size; k++)
			{
				for (int l = j - size; l <= j + size; l++)
				{
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < 6)
					{
						ConvertTile(k, l, biome);

						ConvertWall(k, l, biome);
					}
				}
			}
			return;
		}
		public delegate void ConversionOverrideHack(int baseTile, ref int newTile);
		public static (int newTile, AltBiome fromBiome) GetTileConversionState(int i, int j, AltBiome targetBiome) {
			Tile tile = Main.tile[i, j];
			int newTile = -1;
			if (targetBiome is not null) {
				newTile = targetBiome.GetAltBlock(tile.TileType, i, j);
			}
			int baseTile = tile.TileType;
			AltBiome fromBiome = null;
			if (ALConvertInheritanceData.tileParentageData.Parent.TryGetValue(tile.TileType, out (int baseTile, AltBiome fromBiome) parent)) {
				(baseTile, fromBiome) = parent;
			}
			if (newTile == -1) {
				if (targetBiome is not null) {
					newTile = targetBiome.GetAltBlock(baseTile, i, j);
				}
			}

			if (newTile == -1 && ALConvertInheritanceData.tileParentageData.BreakIfConversionFail.TryGetValue(baseTile, out BitsByte bits)) {
				if (bits[targetBiome.ConversionType]) newTile = -2;
			}
			return (newTile, fromBiome);
		}
		public static void ConvertTile(int i, int j, AltBiome targetBiome, bool silent = false) {
			(int newTile, AltBiome fromBiome) = GetTileConversionState(i, j, targetBiome);
			if (fromBiome == targetBiome || (fromBiome is null && targetBiome is DeconvertAltBiome)) return;
			Tile tile = Main.tile[i, j];
			if (Main.tileFrameImportant[tile.TileType] && TileObjectData.GetTileData(tile) is TileObjectData tileObjectData) {
				ConvertMultiTile(i, j, newTile, tileObjectData, fromBiome, targetBiome, silent);
				return;
			}

			if (newTile != -1 && newTile != tile.TileType && (WorldGen.generatingWorld || GlobalBiomeHooks.PreConvertTile(fromBiome, targetBiome, i, j))) {
				WorldGen.TryKillingTreesAboveIfTheyWouldBecomeInvalid(i, j, newTile);
				if (newTile == -2) {
					WorldGen.KillTile(i, j, false, false, false);
					if (Main.netMode != NetmodeID.SinglePlayer && !silent) {
						NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j, 0f, 0, 0, 0);
					}
				} else if (newTile != tile.TileType) {
					tile.TileType = (ushort)newTile;
					if (!WorldGen.generatingWorld) {
						WorldGen.SquareTileFrame(i, j, true);
						if (!silent) NetMessage.SendTileSquare(-1, i, j, TileChangeType.None);
					}
				}
				if (!WorldGen.generatingWorld) GlobalBiomeHooks.PostConvertTile(fromBiome, targetBiome, i, j);
				//AltLibrary.RateLimitedLog($"converted tile at {i}, {j}, from {fromBiome} to {targetBiome}", $"({i},{j})");
			}
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
		public static (int newWall, AltBiome fromBiome) GetWallConversionState(int i, int j, AltBiome targetBiome) {
			Tile tile = Main.tile[i, j];
			int newWall = -1;
			if (targetBiome is not null) {
				newWall = targetBiome.GetAltWall(tile.WallType, i, j);
			}
			int baseWall = tile.WallType;
			AltBiome fromBiome = null;
			if (ALConvertInheritanceData.wallParentageData.Parent.TryGetValue(tile.WallType, out (int baseTile, AltBiome fromBiome) parent)) {
				(baseWall, fromBiome) = parent;
			}
			if (newWall == -1) {
				if (targetBiome is not null) {
					newWall = targetBiome.GetAltWall(baseWall, i, j);
				}
			}
			if (newWall == -1 && ALConvertInheritanceData.wallParentageData.BreakIfConversionFail.TryGetValue(baseWall, out BitsByte bits)) {
				if (bits[targetBiome.ConversionType]) newWall = -2;
			}
			return (newWall, fromBiome);
		}
		public static void ConvertWall(int i, int j, AltBiome targetBiome, bool silent = false) {
			Tile tile = Main.tile[i, j];
			(int newWall, AltBiome fromBiome) = GetWallConversionState(i, j, targetBiome);
			if (fromBiome == targetBiome) return;

			if (newWall != -1 && newWall != tile.WallType && (WorldGen.generatingWorld || GlobalBiomeHooks.PreConvertWall(fromBiome, targetBiome, i, j))) {
				if (newWall == -2) {
					WorldGen.KillWall(i, j, false);
					if (Main.netMode != NetmodeID.SinglePlayer && !silent) {
						NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j, 0f, 0, 0, 0);
					}
				} else if (newWall != tile.WallType) {
					tile.WallType = (ushort)newWall;
					if (!WorldGen.generatingWorld) {
						WorldGen.SquareTileFrame(i, j, true);
						if (!silent) NetMessage.SendTileSquare(-1, i, j, TileChangeType.None);
					}
				}
				if (!WorldGen.generatingWorld) GlobalBiomeHooks.PostConvertWall(fromBiome, targetBiome, i, j);
			}
		}
	}
}
