using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using AltLibrary.Core;
using AltLibrary.Core.Baking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common.Hooks
{
	internal static class HardmodeWorldGen
	{
		public static void Init()
		{
			Terraria.IL_WorldGen.GERunner += WorldGen_GERunner;
			Terraria.On_WorldGen.GERunner += WorldGen_GERunner1;
			IL_WorldGen.HardmodeWallsTask += GenPasses_HookGenPassHardmodeWalls;
		}

		public static void Unload()
		{
		}
		//TODO: double check that this code makes sense to begin with

		private static void WorldGen_GERunner1(Terraria.On_WorldGen.orig_GERunner orig, int i, int j, double speedX, double speedY, bool good)
		{
			if (Main.drunkWorld && WorldBiomeGeneration.WofKilledTimes > 1) {
				if (good) {
					List<int> possibles = new() { 0 };
					AltLibrary.Biomes.FindAll(x => x.BiomeType == BiomeType.Hallow).ForEach(x => possibles.Add(x.Type));
					if (AltLibraryServerConfig.Config.HardmodeGenRandom)
						AltLibrary.Biomes.FindAll(x => x.BiomeType == BiomeType.Jungle).ForEach(x => possibles.Add(x.Type));
					WorldBiomeManager.drunkGoodGen = Main.rand.Next(possibles);
					possibles = new()
					{
						0,
						-1
					};
					AltLibrary.Biomes.FindAll(x => x.BiomeType == BiomeType.Evil).ForEach(x => possibles.Add(x.Type));
					if (AltLibraryServerConfig.Config.HardmodeGenRandom)
						AltLibrary.Biomes.FindAll(x => x.BiomeType == BiomeType.Hell).ForEach(x => possibles.Add(x.Type));
					WorldBiomeManager.drunkEvilGen = Main.rand.Next(possibles);
				}

				int addX = WorldGen.genRand.Next(300, 400) * WorldBiomeGeneration.WofKilledTimes;
				if (!good) addX *= -1;
				i += addX;
				if (i < 0)
				{
					i *= -1;
				}
				if (i > Main.maxTilesX)
				{
					i %= Main.maxTilesX;
				}
			}
			orig(i, j, speedX, speedY, good);
		}

		//TODO: double check that this code makes sense to begin with
		private static void GenPasses_HookGenPassHardmodeWalls(ILContext il)
		{
			ILCursor c = new(il);
			if (!c.TryGotoNext(MoveType.AfterLabel, 
				i => i.MatchLdloca(5),
				i => i.MatchCall<Tile>("active")
				)) {
				AltLibrary.Instance.Logger.Info("GenPassHardmodeWalls $ active");
				return;
			}
			c.Index++;
			c.Emit(OpCodes.Ldloc, 7);
			c.Emit(OpCodes.Ldloc, 5);
			c.EmitDelegate<Func<int, Tile, int>>((orig, tile) =>
			{
				if (WorldBiomeGeneration.WofKilledTimes <= 1)
				{
					if (WorldBiomeManager.GetWorldHallow(false) is AltBiome worldHallow && worldHallow.HardmodeWalls.Count > 0 && worldHallow.TileConversions.ContainsValue(tile.TileType)) {
						orig = WorldGen.genRand.Next(worldHallow.HardmodeWalls);
					}
					if (TryFind(WorldBiomeManager.WorldEvilName, out AltBiome worldEvil) && worldEvil.HardmodeWalls.Count > 0 && worldEvil.TileConversions.ContainsValue(tile.TileType)) {
						orig = WorldGen.genRand.Next(worldEvil.HardmodeWalls);
					}
				} else {
					if (Main.drunkWorld) {
						if (Evil?.HardmodeWalls.Count > 0 && (Evil?.TileConversions?.ContainsValue(tile.TileType) ?? false)) {
							orig = WorldGen.genRand.Next(Evil?.HardmodeWalls);
						}
						if (Good?.HardmodeWalls.Count > 0 && (Good?.TileConversions?.ContainsValue(tile.TileType) ?? false)) {
							orig = WorldGen.genRand.Next(Good?.HardmodeWalls);
						}
					}
				}
				return orig;
			});
			c.Emit(OpCodes.Stloc, 7);
		}
		//TODO: double check that this code makes sense to begin with

		private static int GetTileOnStateHallow(int tileID, int x, int y, bool GERunner = false)
		{
			AltBiome biome;
			if (WorldBiomeManager.drunkGoodGen > 0) {
				biome = Good;
			} else if (WorldBiomeManager.WorldEvilName != "" && WorldBiomeGeneration.WofKilledTimes <= 1) {
				biome = WorldBiomeManager.GetWorldHallow();
			} else {
				biome = GetInstance<HallowAltBiome>();
			}
			int rv = biome.GetAltBlock(tileID, x, y, GERunner);
			if (rv == -1)
				return tileID;
			else if (rv == -2)
				return 0;
			return rv;
		}

		//TODO: double check that this code makes sense to begin with
		private static int GetTileOnStateEvil(int tileID, int x, int y, bool GERunner = false) {
			AltBiome biome;
			if (WorldBiomeManager.drunkEvilGen > 0) {
				biome = Evil;
			} else if (WorldBiomeManager.WorldEvilName != "" && WorldBiomeGeneration.WofKilledTimes <= 1) {
				biome = WorldBiomeManager.GetWorldEvil(true, true);
			} else {
				biome = WorldBiomeGeneration.WofKilledTimes <= 1 ?
					(!WorldGen.crimson ? GetInstance<CorruptionAltBiome>() : GetInstance<CrimsonAltBiome>()) :
					(WorldBiomeManager.drunkEvilGen == 0 ? GetInstance<CorruptionAltBiome>() : GetInstance<CrimsonAltBiome>());
			}
			int rv = biome.GetAltBlock(tileID, x, y, GERunner);
			if (rv == -1)
				return tileID;
			else if (rv == -2)
				return 0;
			return rv;
		}

		//TODO: double check that this code makes sense to begin with
		private static int GetWallOnStateHallow(int wallID, int x, int y) {
			AltBiome biome;
			if (WorldBiomeManager.drunkGoodGen > 0) biome = Good;
			else biome = WorldBiomeManager.GetWorldHallow(true);

			if (biome.GERunnerWallConversions.TryGetValue(wallID, out int newWall)) return newWall;
			return biome.WallConversions.TryGetValue(wallID, out newWall) ? newWall : wallID;
		}

		//TODO: double check that this code makes sense to begin with
		private static int GetWallOnStateEvil(int wallID, int x, int y) {
			AltBiome biome;
			if (WorldBiomeManager.drunkGoodGen > 0) biome = Evil;
			else biome = WorldBiomeManager.GetWorldEvil(true, true);

			if (biome.GERunnerWallConversions.TryGetValue(wallID, out int newWall)) return newWall;
			return biome.WallConversions.TryGetValue(wallID, out newWall) ? newWall : wallID;
		}

		private static AltBiome Good => AltLibrary.Biomes.Find(x => x.Type == WorldBiomeManager.drunkGoodGen);

		private static AltBiome Evil => AltLibrary.Biomes.Find(x => x.Type == WorldBiomeManager.drunkEvilGen);

		//TODO: double check that this code makes sense to begin with
		private static void WorldGen_GERunner(ILContext il)
		{
			ILCursor c = new(il);
			int good = 0;
			if (!c.TryGotoNext(i => i.MatchBrfalse(out _)))
			{
				AltLibrary.Instance.Logger.Info("i $ 1");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchBrfalse(out _)))
			{
				AltLibrary.Instance.Logger.Info("i $ 2");
				return;
			}
			if (!c.TryGotoPrev(i => i.MatchLdarg(out good)))
			{
				AltLibrary.Instance.Logger.Info("i $ 3");
				return;
			}
			if (!c.TryGotoPrev(i => i.MatchBgeUn(out _)))
			{
				AltLibrary.Instance.Logger.Info("i $ 4");
				return;
			}

			c.Index++;
			c.Emit(OpCodes.Ldarg, good);
			c.Emit(OpCodes.Ldloc, 15);
			c.Emit(OpCodes.Ldloc, 16);
			c.EmitDelegate<Action<bool, int, int>>((good, m, l) =>
			{
				if (!good)
				{
					Tile tile = Main.tile[m, l];
					if (WorldBiomeGeneration.WofKilledTimes <= 1 && WorldBiomeManager.WorldEvilName != "") {
						AltBiome evilBiome = WorldBiomeManager.WorldEvilBiome;
						if (evilBiome.TileConversions.TryGetValue(tile.TileType, out int tileReplacement)) {
							tile.TileType = (ushort)tileReplacement;
							WorldGen.SquareTileFrame(m, l, true);
						}
						if (evilBiome.GERunnerConversion.TryGetValue(tile.TileType, out tileReplacement)) {
							tile.TileType = (ushort)tileReplacement;
							WorldGen.SquareTileFrame(m, l, true);
						}
						if (evilBiome.WallConversions.TryGetValue(tile.WallType, out int wallReplacement)) {
							tile.WallType = (ushort)wallReplacement;
						}
					}
					if (Main.drunkWorld && WorldBiomeGeneration.WofKilledTimes > 1)
					{
						int type = tile.TileType;
						int wall = tile.WallType;
						if (WorldGen.InWorld(m, l) && type != -1) {
							int evilState = GetTileOnStateEvil(type, m, l, true);
							if (evilState != -1 && type != evilState) {
								type = (ushort)evilState;
								WorldGen.SquareTileFrame(m, l, true);
							}
						}
						if (WorldGen.InWorld(m, l) && wall != -1) {
							int evilWall = GetWallOnStateEvil(wall, m, l);
							if (evilWall != -1 && wall != evilWall) {
								tile.WallType = (ushort)evilWall;
							}
						}
					}
				}
			});

			if (!c.TryGotoNext(i => i.MatchBrfalse(out _)))
			{
				AltLibrary.Instance.Logger.Info("i $ 5");
				return;
			}

			c.Index++;
			c.Emit(OpCodes.Ldloc, 15);
			c.Emit(OpCodes.Ldloc, 16);
			c.EmitDelegate<Action<int, int>>((m, l) =>
			{
				Tile tile = Main.tile[m, l];
				if (WorldBiomeGeneration.WofKilledTimes <= 1 && WorldBiomeManager.WorldHallowName != "") {
					AltBiome worldHallow = WorldBiomeManager.WorldHallowBiome;
					if (worldHallow.TileConversions.TryGetValue(tile.TileType, out int tileReplacement)) {
						tile.TileType = (ushort)tileReplacement;
						WorldGen.SquareTileFrame(m, l, true);
					}
					if (worldHallow.GERunnerConversion.TryGetValue(tile.TileType, out tileReplacement)) {
						tile.TileType = (ushort)tileReplacement;
						WorldGen.SquareTileFrame(m, l, true);
					}
					if (worldHallow.WallConversions.TryGetValue(tile.WallType, out int wallReplacement)) {
						tile.WallType = (ushort)wallReplacement;
					}
				}
				if (Main.drunkWorld && WorldBiomeGeneration.WofKilledTimes > 1)
				{
					int type = tile.TileType;
					int wall = tile.WallType;
					if (WorldGen.InWorld(m, l) && type != -1) {
						int hallowState = GetTileOnStateHallow(type, m, l, true);
						if (hallowState != -1 && type != hallowState) {
							type = (ushort)hallowState;
							WorldGen.SquareTileFrame(m, l, true);
						}
					}
					if (WorldGen.InWorld(m, l) && wall != -1) {
						int hallowWall = GetWallOnStateHallow(wall, m, l);
						if (hallowWall != -1 && wall != hallowWall) {
							wall = (ushort)hallowWall;
						}
					}
				}
			});

			void goodWall(int id)
			{
				ILCursor c = new(il);
				while (c.TryGotoNext(i => i.MatchLdcI4(id) && i.Offset != 0))
				{
					c.Index++;
					c.Emit(OpCodes.Ldloc, 15);
					c.Emit(OpCodes.Ldloc, 16);
					c.EmitDelegate<Func<int, int, int, int>>(GetWallOnStateHallow);
				}
			}
			void goodTile(int id)
			{
				ILCursor c = new(il);
				while (c.TryGotoNext(i => i.MatchLdcI4(id) && i.Offset != 0))
				{
					c.Index++;
					c.Emit(OpCodes.Ldloc, 15);
					c.Emit(OpCodes.Ldloc, 16);
					c.Emit(OpCodes.Ldc_I4_1);
					c.EmitDelegate<Func<int, int, int, bool, int>>(GetTileOnStateHallow);
				}
			}
			void evilWall(int id)
			{
				ILCursor c = new(il);
				while (c.TryGotoNext(i => i.MatchLdcI4(id) && i.Offset != 0))
				{
					c.Index++;
					c.Emit(OpCodes.Ldloc, 15);
					c.Emit(OpCodes.Ldloc, 16);
					c.EmitDelegate<Func<int, int, int, int>>(GetWallOnStateEvil);
				}
			}
			void evilTile(int id)
			{
				ILCursor c = new(il);
				if (id != TileID.FleshIce)
				{
					while (c.TryGotoNext(i => i.MatchLdcI4(id) && i.Offset != 0))
					{
						c.Index++;
						c.Emit(OpCodes.Ldloc, 15);
						c.Emit(OpCodes.Ldloc, 16);
						c.Emit(OpCodes.Ldc_I4_1);
						c.EmitDelegate<Func<int, int, int, bool, int>>(GetTileOnStateEvil);
					}
				}
				else
				{
					while (c.TryGotoNext(i => !i.MatchCall<WorldGen>("get_genRand"),
						i => i.MatchLdcI4(id) && i.Offset != 0))
					{
						c.Index += 2;
						c.Emit(OpCodes.Ldloc, 15);
						c.Emit(OpCodes.Ldloc, 16);
						c.Emit(OpCodes.Ldc_I4_1);
						c.EmitDelegate<Func<int, int, int, bool, int>>(GetTileOnStateEvil);
					}
				}
			}

			goodWall(WallID.HallowedGrassUnsafe);
			goodWall(WallID.HallowHardenedSand);
			goodWall(WallID.HallowSandstone);
			goodWall(WallID.PearlstoneBrickUnsafe);

			evilWall(WallID.CorruptGrassUnsafe);
			evilWall(WallID.CorruptHardenedSand);
			evilWall(WallID.CorruptSandstone);

			evilWall(WallID.CrimsonGrassUnsafe);
			evilWall(WallID.CrimsonHardenedSand);
			evilWall(WallID.CrimsonSandstone);

			goodTile(TileID.Pearlstone);
			goodTile(TileID.HallowHardenedSand);
			goodTile(TileID.HallowedGrass);
			goodTile(TileID.Pearlsand);
			goodTile(TileID.HallowedIce);
			goodTile(TileID.HallowSandstone);

			evilTile(TileID.Ebonstone);
			evilTile(TileID.CorruptHardenedSand);
			evilTile(TileID.CorruptGrass);
			evilTile(TileID.Ebonsand);
			evilTile(TileID.CorruptIce);
			evilTile(TileID.CorruptSandstone);

			evilTile(TileID.Crimstone);
			evilTile(TileID.CrimsonHardenedSand);
			evilTile(TileID.CrimsonGrass);
			evilTile(TileID.Crimsand);
			evilTile(TileID.FleshIce);
			evilTile(TileID.CrimsonSandstone);
		}
	}
}
