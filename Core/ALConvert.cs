using AltLibrary.Common.AltBiomes;
using AltLibrary.Core.Baking;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Bestiary.On_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;

namespace AltLibrary.Core
{
	public static class ALConvert
	{
		internal static void Load()
		{
			On_WorldGen.orig_Convert vvv = WorldGen.Convert;
			Terraria.On_WorldGen.Convert += WorldGen_Convert;
		}

		internal static void Unload()
		{
		}

		private static void WorldGen_Convert(Terraria.On_WorldGen.orig_Convert orig, int i, int j, int conversionType, int size)
		{
			for (int k = i - size; k <= i + size; k++)
			{
				for (int l = j - size; l <= j + size; l++)
				{
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < 6)
					{
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
						ConvertTile(k, l, biome, biome.TileConversions, biome.ConversionType);

						ConvertWall(k, l, biome, biome.WallConversions, biome.ConversionType);
						continue;
					}
				}
			}
			return;
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
						ConvertTile(k, l, biome, biome.TileConversions, biome.ConversionType);

						ConvertWall(k, l, biome, biome.WallConversions, biome.ConversionType);
					}
				}
			}
			return;
		}
		public delegate void ConversionOverrideHack(int baseTile, ref int newTile);
		public static (int newTile, AltBiome fromBiome) GetTileConversionState(int i, int j, AltBiome targetBiome, Dictionary<int, int> conversions, int conversionType) {
			Tile tile = Main.tile[i, j];
			int newTile = -1;
			if (targetBiome is not null) {
				newTile = targetBiome.GetAltBlock(tile.TileType, i, j);
			} else if (conversions.TryGetValue(tile.TileType, out int convertedTile)) {
				newTile = convertedTile;
			}
			int baseTile = tile.TileType;
			AltBiome fromBiome = null;
			if (ALConvertInheritanceData.tileParentageData.Parent.TryGetValue(tile.TileType, out (int baseTile, AltBiome fromBiome) parent)) {
				(baseTile, fromBiome) = parent;
			}
			if (newTile == -1) {
				if (targetBiome is not null) {
					newTile = targetBiome.GetAltBlock(baseTile, i, j);
				} else if (conversions.TryGetValue(baseTile, out int convertedTile)) {
					newTile = convertedTile;
				}
			}

			if (newTile == -1 && ALConvertInheritanceData.tileParentageData.BreakIfConversionFail.TryGetValue(baseTile, out BitsByte bits)) {
				if (bits[conversionType]) newTile = -2;
			}
			return (newTile, fromBiome);
		}
		public static void ConvertTile(int i, int j, AltBiome targetBiome, Dictionary<int, int> conversions, int conversionType, bool silent = false) {
			Tile tile = Main.tile[i, j];
			(int newTile, AltBiome fromBiome) = GetTileConversionState(i, j, targetBiome, conversions, conversionType);

			if (newTile != -1 && newTile != tile.TileType && GlobalBiomeHooks.PreConvertTile(fromBiome, targetBiome, i, j)) {
				WorldGen.TryKillingTreesAboveIfTheyWouldBecomeInvalid(i, j, newTile);
				if (newTile == -2) {
					WorldGen.KillTile(i, j, false, false, false);
					if (Main.netMode == NetmodeID.MultiplayerClient && !silent) {
						NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j, 0f, 0, 0, 0);
					}
				} else if (newTile != tile.WallType) {
					tile.TileType = (ushort)newTile;

					WorldGen.SquareTileFrame(i, j, true);
					if (!silent) NetMessage.SendTileSquare(-1, i, j, TileChangeType.None);
				}
				GlobalBiomeHooks.PostConvertTile(fromBiome, targetBiome, i, j);
			}
		}
		public static (int newWall, AltBiome fromBiome) GetWallConversionState(int i, int j, AltBiome targetBiome, Dictionary<int, int> conversions, int conversionType) {
			Tile tile = Main.tile[i, j];
			int newWall = -1;
			if (targetBiome is not null) {
				newWall = targetBiome.GetAltBlock(tile.WallType, i, j);
			} else if (conversions.TryGetValue(tile.WallType, out int convertedWall)) {
				newWall = convertedWall;
			}
			int baseWall = tile.WallType;
			AltBiome fromBiome = null;
			if (ALConvertInheritanceData.wallParentageData.Parent.TryGetValue(tile.WallType, out (int baseTile, AltBiome fromBiome) parent)) {
				(baseWall, fromBiome) = parent;
			}
			if (newWall == -1) {
				if (targetBiome is not null) {
					newWall = targetBiome.GetAltBlock(baseWall, i, j);
				} else if (conversions.TryGetValue(baseWall, out int convertedWall)) {
					newWall = convertedWall;
				}
			}

			if (newWall == -1 && ALConvertInheritanceData.wallParentageData.BreakIfConversionFail.TryGetValue(baseWall, out BitsByte bits)) {
				if (bits[conversionType]) newWall = -2;
			}
			return (newWall, fromBiome);
		}
		public static void ConvertWall(int i, int j, AltBiome targetBiome, Dictionary<int, int> conversions, int conversionType, ConversionOverrideHack conversionOverrideHack = null, bool silent = false) {
			Tile tile = Main.tile[i, j];
			int baseWall = tile.WallType;
			AltBiome fromBiome = null;
			if (ALConvertInheritanceData.wallParentageData.Parent.TryGetValue(tile.WallType, out (int baseTile, AltBiome fromBiome) parent)) {
				(baseWall, fromBiome) = parent;
			}
			int newWall = -1;
			if (conversions.TryGetValue(tile.WallType, out int convertedWall)) {
				newWall = convertedWall;
			} else if (conversions.TryGetValue((ushort)baseWall, out convertedWall)) {
				newWall = convertedWall;
			}
			if (conversionOverrideHack is not null) conversionOverrideHack(baseWall, ref newWall);


			if (newWall == -1 && ALConvertInheritanceData.wallParentageData.BreakIfConversionFail.TryGetValue(baseWall, out BitsByte bits)) {
				if (bits[conversionType]) newWall = -2; //change this to make use of spraytype
			}

			if (newWall != -1 && newWall != tile.WallType && GlobalBiomeHooks.PreConvertWall(fromBiome, targetBiome, i, j)) {
				if (newWall == -2) {
					WorldGen.KillWall(i, j, false);
					if (Main.netMode == NetmodeID.MultiplayerClient && !silent) {
						NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j, 0f, 0, 0, 0);
					}
				} else if (newWall != tile.WallType) {
					tile.WallType = (ushort)newWall;

					WorldGen.SquareTileFrame(i, j, true);
					if (!silent) NetMessage.SendTileSquare(-1, i, j, TileChangeType.None);
				}
				GlobalBiomeHooks.PostConvertWall(fromBiome, targetBiome, i, j);
			}
		}
	}
}
