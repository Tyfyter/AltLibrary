using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using AltLibrary.Core;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AltLibrary.Common.Hooks
{
	internal static class EvenMoreWorldGen
	{
		public static void Init()
		{
			Terraria.IL_WorldGen.GenerateWorld += GenPasses.ILGenerateWorld;
			GenPasses.HookGenPassReset += GenPasses_HookGenPassReset;
			GenPasses.HookGenPassShinies += GenPasses_HookGenPassShinies;
			GenPasses.HookGenPassUnderworld += GenPasses_HookGenPassUnderworld;
			GenPasses.HookGenPassAltars += ILGenPassAltars;
			GenPasses.HookGenPassMicroBiomes += GenPasses_HookGenPassMicroBiomes;
			Terraria.GameContent.Biomes.IL_MiningExplosivesBiome.Place += MiningExplosivesBiome_Place;
			IL_WorldGen.FinishRemixWorld += IL_WorldGen_FinishRemixWorld;
			MonoModHooks.Modify(GenPasses.SpreadingGrassInfo, IL_GenPasses_SpreadingGrass);
			IL_WorldGen.IslandHouse += IL_WorldGen_IslandHouse;
			IL_WorldGen.AddBuriedChest_int_int_int_bool_int_bool_ushort += IL_WorldGen_AddBuriedChest;
			On_WorldGenRange.GetRandom += On_WorldGenRange_GetRandom;
		}

		private static int On_WorldGenRange_GetRandom(On_WorldGenRange.orig_GetRandom orig, WorldGenRange self, Terraria.Utilities.UnifiedRandom random) {
			return random.Next(self.ScaledMinimum, self.ScaledMaximum + 1);
		}

		public static void Unload()
		{
		}

		//TODO: double check that this code makes sense to begin with
		private static void MiningExplosivesBiome_Place(ILContext il)
		{
			ILCursor c = new(il);

			if (!c.TryGotoNext(i => i.MatchStloc(0)))
			{
				AltLibrary.Instance.Logger.Info("12 $ 1");
				return;
			}

			c.Index++;
			c.Emit(OpCodes.Ldloc, 0);
			c.EmitDelegate<Func<ushort, ushort>>((type) =>
			{
				type = Utils.SelectRandom(WorldGen.genRand, new ushort[]
				{
					(ushort)WorldGen.SavedOreTiers.Gold,
					(ushort)WorldGen.SavedOreTiers.Silver,
					(ushort)WorldGen.SavedOreTiers.Iron,
					(ushort)WorldGen.SavedOreTiers.Copper,
				});
				return type;
			});
			c.Emit(OpCodes.Stloc, 0);
		}

		//TODO: double check that this code makes sense to begin with
		private static void GenPasses_HookGenPassUnderworld(ILContext il)
		{
			ILCursor c = new(il);

			if (!c.TryGotoNext(i => i.MatchCallvirt<GenerationProgress>("set_Message")))
			{
				AltLibrary.Instance.Logger.Info("d $ 1");
				return;
			}

			c.Index++;
			c.Emit(OpCodes.Ldarg, 1);
			c.EmitDelegate<Action<GenerationProgress>>((progress) =>
			{
				if (WorldBiomeManager.GetWorldHell(false) is AltBiome hell && hell.GenPassName != null)
				{
					progress.Message = hell.GenPassName.Value;
				}
			});

			ALUtils.ReplaceIDs(il, TileID.Ash,
				(orig) => (ushort)WorldBiomeManager.GetWorldHell(false).TileConversions[TileID.Stone],
				(orig) => WorldBiomeManager.GetWorldHell(false) is AltBiome hell && hell.TileConversions.ContainsKey(TileID.Stone));
			ALUtils.ReplaceIDs(il, TileID.Hellstone,
				(orig) => (ushort?)WorldBiomeManager.GetWorldHell(false).BiomeOre ?? orig,
				(orig) => WorldBiomeManager.GetWorldHell(false) is AltBiome hell && hell.BiomeOre.HasValue);

			if (!c.TryGotoNext(i => i.MatchCall<WorldGen>(nameof(WorldGen.AddHellHouses))))
			{
				AltLibrary.Instance.Logger.Info("d $ 2");
				return;
			}

			ILLabel label = c.DefineLabel();
			c.EmitDelegate<Func<bool>>(() => WorldBiomeManager.WorldHell == "");
			c.Emit(OpCodes.Brfalse_S, label);
			c.Index++;
			c.MarkLabel(label);
		}

		//TODO: double check that this code makes sense to begin with
		private static void GenPasses_HookGenPassMicroBiomes(ILContext il)
		{
			ILCursor c = new(il);
			if (!c.TryGotoNext(i => i.MatchLdstr("LivingTreeCount")))
			{
				AltLibrary.Instance.Logger.Info("c $ 1");
				return;
			}
			if (!c.TryGotoPrev(i => i.MatchCallvirt<GenerationProgress>("Set")))
			{
				AltLibrary.Instance.Logger.Info("c $ 2");
				return;
			}

			var label = il.DefineLabel();

			c.Index++;
			c.EmitDelegate<Func<bool>>(() => WorldBiomeManager.WorldJungle == "");
			c.Emit(OpCodes.Brfalse_S, label);

			if (!c.TryGotoNext(i => i.MatchLdstr("..Long Minecart Tracks")))
			{
				AltLibrary.Instance.Logger.Info("c $ 3");
				return;
			}
			if (!c.TryGotoPrev(i => i.MatchLdarg(1)))
			{
				AltLibrary.Instance.Logger.Info("c $ 4");
				return;
			}

			c.MarkLabel(label);
		}

		//TODO: double check that this code makes sense to begin with
		public static void ILGenPassAltars(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel endNormalAltar = c.DefineLabel();
			ILLabel startNormalAltar = c.DefineLabel();
			if (!c.TryGotoNext(i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.crimson))))
			{
				AltLibrary.Instance.Logger.Info("e $ 1");
				return;
			}
			c.EmitDelegate<Func<bool>>(() => WorldGen.crimson || WorldBiomeManager.WorldEvil != "");
			c.Emit(OpCodes.Brfalse, startNormalAltar);
			c.Emit(OpCodes.Ldloc, 3);
			c.Emit(OpCodes.Ldloc, 4);
			c.EmitDelegate<Action<int, int>>((int x, int y) =>
			{
				if (WorldBiomeManager.WorldEvil != "" && ModContent.Find<AltBiome>(WorldBiomeManager.WorldEvil).AltarTile.HasValue)
				{
					if (!WorldGen.IsTileNearby(x, y, ModContent.Find<AltBiome>(WorldBiomeManager.WorldEvil).AltarTile.Value, 3))
					{
						WorldGen.Place3x2(x, y, (ushort)ModContent.Find<AltBiome>(WorldBiomeManager.WorldEvil).AltarTile.Value);
					}
				}
			});
			c.Emit(OpCodes.Br, endNormalAltar);
			c.MarkLabel(startNormalAltar);
			if (!c.TryGotoNext(i => i.MatchLdloc(5)))
			{
				AltLibrary.Instance.Logger.Info("e $ 2");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchLdsflda(out _)))
			{
				AltLibrary.Instance.Logger.Info("e $ 3");
				return;
			}
			c.MarkLabel(endNormalAltar);
		}

		//TODO: double check that this code makes sense to begin with
		private static void GenPasses_HookGenPassReset(ILContext il)
		{
			ILCursor c = new(il);
			FieldReference copper = null;
			FieldReference iron = null;
			FieldReference silver = null;
			FieldReference gold = null;

			if (!c.TryGotoNext(i => i.MatchStsfld(typeof(WorldGen).GetField(nameof(WorldGen.crimson), BindingFlags.Public | BindingFlags.Static))))
			{
				AltLibrary.Instance.Logger.Info("f $ 1");
				return;
			}
			if (!c.TryGotoPrev(i => i.MatchLdcI4(166)))
			{
				AltLibrary.Instance.Logger.Info("f $ 2");
				return;
			}
			if (!c.TryGotoPrev(i => i.MatchLdcI4(166)))
			{
				AltLibrary.Instance.Logger.Info("f $ 3");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchStsfld(out copper)))
			{
				AltLibrary.Instance.Logger.Info("f $ 4");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchLdcI4(167)))
			{
				AltLibrary.Instance.Logger.Info("f $ 5");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchStsfld(out iron)))
			{
				AltLibrary.Instance.Logger.Info("f $ 6");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchLdcI4(168)))
			{
				AltLibrary.Instance.Logger.Info("f $ 7");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchStsfld(out silver)))
			{
				AltLibrary.Instance.Logger.Info("f $ 8");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchLdcI4(169)))
			{
				AltLibrary.Instance.Logger.Info("f $ 9");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchStsfld(out gold)))
			{
				AltLibrary.Instance.Logger.Info("f $ 10");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchStsfld(typeof(WorldGen).GetField(nameof(WorldGen.crimson), BindingFlags.Public | BindingFlags.Static))))
			{
				AltLibrary.Instance.Logger.Info("f $ 11");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchRet()))
			{
				AltLibrary.Instance.Logger.Info("f $ 12");
				return;
			}
			if (!c.TryGotoPrev(i => i.MatchBneUn(out _)))
			{
				AltLibrary.Instance.Logger.Info("f $ 13");
				return;
			}
			if (!c.TryGotoPrev(i => i.MatchLdcI4(-1)))
			{
				AltLibrary.Instance.Logger.Info("f $ 14");
				return;
			}

			c.EmitDelegate<Func<int, int>>(dungeonSide =>
			{
				WorldBiomeGeneration.DungeonSide = dungeonSide;
				return dungeonSide;
			});

			replaceValues(() => WorldBiomeManager.Copper, (-1, TileID.Copper), (-2, TileID.Tin), copper);
			replaceValues(() => WorldBiomeManager.Iron, (-3, TileID.Iron), (-4, TileID.Lead), iron);
			replaceValues(() => WorldBiomeManager.Silver, (-5, TileID.Silver), (-6, TileID.Tungsten), silver);
			replaceValues(() => WorldBiomeManager.Gold, (-7, TileID.Gold), (-8, TileID.Platinum), gold);

			for (int i = 0; i < 2; i++)
			{
				if (!c.TryGotoNext(i => i.MatchRet()))
				{
					AltLibrary.Instance.Logger.Info("f $ 15 " + i);
					return;
				}
				c.Index--;
				c.EmitDelegate<Func<int, int>>(dungeonLocation =>
				{
					WorldBiomeGeneration.DungeonLocation = dungeonLocation;
					return dungeonLocation;
				});
				c.Index += 2;
			}

			void replaceValues(Func<int> type, ValueTuple<int, int> value1, ValueTuple<int, int> value2, FieldReference field)
			{
				var label = c.DefineLabel();
				var label2 = c.DefineLabel();
				var label3 = c.DefineLabel();
				c.EmitDelegate(type);
				c.Emit(OpCodes.Ldc_I4, value1.Item1);
				c.Emit(OpCodes.Bne_Un_S, label);
				c.Emit(OpCodes.Ldarg, 0);
				c.Emit(OpCodes.Ldc_I4, value1.Item2);
				c.Emit(OpCodes.Stfld, field);
				c.Emit(OpCodes.Br_S, label3);
				c.MarkLabel(label);
				c.EmitDelegate(type);
				c.Emit(OpCodes.Ldc_I4, value2.Item1);
				c.Emit(OpCodes.Bne_Un_S, label2);
				c.Emit(OpCodes.Ldarg, 0);
				c.Emit(OpCodes.Ldc_I4, value2.Item2);
				c.Emit(OpCodes.Stfld, field);
				c.Emit(OpCodes.Br_S, label3);
				c.MarkLabel(label2);
				c.Emit(OpCodes.Ldarg, 0);
				c.EmitDelegate<Func<int>>(() => AltLibrary.Ores[type() - 1].ore);
				c.Emit(OpCodes.Stfld, field);
				c.MarkLabel(label3);
			}
		}

		//TODO: double check that this code makes sense to begin with
		private static void GenPasses_HookGenPassShinies(ILContext il)
		{
			ILCursor c = new(il);
			for (int j = 0; j < 3; j++)
			{
				if (!c.TryGotoNext(i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.drunkWorldGen))))
				{
					AltLibrary.Instance.Logger.Info("g $ 1 " + j);
					return;
				}
				if (!c.TryGotoNext(i => i.MatchLdcI4(2)))
				{
					AltLibrary.Instance.Logger.Info("g $ 2 " + j);
					return;
				}
				c.Index++;
				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldc_I4, 1);
				if (!c.TryGotoNext(i => i.MatchLdcI4(7)))
				{
					AltLibrary.Instance.Logger.Info("g $ 3 " + j);
					return;
				}
				c.Index++;
				c.EmitDelegate<Func<int, int>>((value) =>
				{
					List<int> list = new()
					{
						7,
						166
					};
					AltLibrary.Ores.Where(x => x.OreType == OreType.Copper)
								   .ToList()
								   .ForEach(x => list.Add(x.ore));
					return WorldGen.genRand.Next(list);
				});
			}

			for (int j = 0; j < 3; j++)
			{
				if (!c.TryGotoNext(i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.drunkWorldGen))))
				{
					AltLibrary.Instance.Logger.Info("g $ 4 " + j);
					return;
				}
				if (!c.TryGotoNext(i => i.MatchLdcI4(2)))
				{
					AltLibrary.Instance.Logger.Info("g $ 5 " + j);
					return;
				}
				c.Index++;
				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldc_I4, 1);
				if (!c.TryGotoNext(i => i.MatchLdcI4(6)))
				{
					AltLibrary.Instance.Logger.Info("g $ 6 " + j);
					return;
				}
				c.Index++;
				c.EmitDelegate<Func<int, int>>((value) =>
				{
					List<int> list = new()
					{
						6,
						167
					};
					AltLibrary.Ores.Where(x => x.OreType == OreType.Iron)
								   .ToList()
								   .ForEach(x => list.Add(x.ore));
					return WorldGen.genRand.Next(list);
				});
			}

			for (int j = 0; j < 3; j++)
			{
				if (!c.TryGotoNext(i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.drunkWorldGen))))
				{
					AltLibrary.Instance.Logger.Info("g $ 7 " + j);
					return;
				}
				if (!c.TryGotoNext(i => i.MatchLdcI4(2)))
				{
					AltLibrary.Instance.Logger.Info("g $ 8 " + j);
					return;
				}
				c.Index++;
				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldc_I4, 1);
				if (!c.TryGotoNext(i => i.MatchLdcI4(9)))
				{
					AltLibrary.Instance.Logger.Info("g $ 9 " + j);
					return;
				}
				c.Index++;
				c.EmitDelegate<Func<int, int>>((value) =>
				{
					List<int> list = new()
					{
						9,
						168
					};
					AltLibrary.Ores.Where(x => x.OreType == OreType.Silver)
								   .ToList()
								   .ForEach(x => list.Add(x.ore));
					return WorldGen.genRand.Next(list);
				});
			}

			for (int j = 0; j < 2; j++)
			{
				if (!c.TryGotoNext(i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.drunkWorldGen))))
				{
					AltLibrary.Instance.Logger.Info("g $ 10 " + j);
					return;
				}
				if (!c.TryGotoNext(i => i.MatchLdcI4(2)))
				{
					AltLibrary.Instance.Logger.Info("g $ 11 " + j);
					return;
				}
				c.Index++;
				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldc_I4, 1);
				if (!c.TryGotoNext(i => i.MatchLdcI4(8)))
				{
					AltLibrary.Instance.Logger.Info("g $ 12 " + j);
					return;
				}
				c.Index++;
				c.EmitDelegate<Func<int, int>>((value) =>
				{
					List<int> list = new()
					{
						8,
						169
					};
					AltLibrary.Ores.Where(x => x.OreType == OreType.Gold)
								   .ToList()
								   .ForEach(x => list.Add(x.ore));
					return WorldGen.genRand.Next(list);
				});
			}

			if (!c.TryGotoNext(i => i.MatchRet()))
			{
				AltLibrary.Instance.Logger.Info("g $ 13");
				return;
			}
			if (!c.TryGotoPrev(i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.drunkWorldGen))))
			{
				AltLibrary.Instance.Logger.Info("g $ 14");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchBrfalse(out _)))
			{
				AltLibrary.Instance.Logger.Info("g $ 15");
				return;
			}
			c.Index++;
			c.EmitDelegate<Action>(() =>
			{
				if (WorldBiomeManager.WorldEvil != "" && ModContent.Find<AltBiome>(WorldBiomeManager.WorldEvil).BiomeOre.HasValue)
				{
					for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * 2.25E-05 / 2.0); i++)
					{
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX),
							WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY), WorldGen.genRand.Next(3, 6),
							WorldGen.genRand.Next(4, 8), ModContent.Find<AltBiome>(WorldBiomeManager.WorldEvil).BiomeOre.Value);
					}
				}
			});
			if (!c.TryGotoNext(i => i.MatchRet()))
			{
				AltLibrary.Instance.Logger.Info("g $ 16");
				return;
			}
			if (!c.TryGotoNext(i => i.MatchBr(out _)))
			{
				AltLibrary.Instance.Logger.Info("g $ 17");
				return;
			}
			ILLabel startCorruptionGen = c.DefineLabel();
			c.EmitDelegate<Func<bool>>(() => !WorldGen.crimson && WorldBiomeManager.WorldEvil != "");
			c.Emit(OpCodes.Brfalse, startCorruptionGen);
			c.EmitDelegate<Action>(() =>
			{
				if (WorldBiomeManager.WorldEvil != "" && ModContent.Find<AltBiome>(WorldBiomeManager.WorldEvil).BiomeOre.HasValue)
				{
					for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * 2.25E-05); i++)
					{
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX),
							WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY), WorldGen.genRand.Next(3, 6),
							WorldGen.genRand.Next(4, 8), ModContent.Find<AltBiome>(WorldBiomeManager.WorldEvil).BiomeOre.Value);
					}
				}
			});
			c.Emit(OpCodes.Ret);
			c.MarkLabel(startCorruptionGen);
		}

		static AltBiome GetRemixBiome(int x) {
			if (WorldGen.drunkWorldGen) {
				if (GenVars.crimsonLeft) {
					if (x < Main.maxTilesX / 2 + WorldGen.genRand.Next(-2, 3)) {
						return WorldBiomeManager.GetDrunkEvil(true);
					} else {
						return WorldBiomeManager.GetWorldEvil(true);
					}
				} else if (x < Main.maxTilesX / 2 + WorldGen.genRand.Next(-2, 3)) {
					return WorldBiomeManager.GetWorldEvil(true);
				} else {
					return WorldBiomeManager.GetDrunkEvil(true);
				}
			} else {
				return WorldBiomeManager.GetWorldEvil(true);
			}
		}
		private static void IL_WorldGen_FinishRemixWorld(ILContext il) {
			ILCursor c = new(il);
			ILLabel skipLabel = default;
			c.GotoNext(MoveType.Before,
				i => i.MatchLdsfld<WorldGen>("drunkWorldGen"),
				i => i.MatchBrfalse(out skipLabel)
			);
			int index = c.Index;
			int x = -1;
			int y = -1;
			c.GotoNext(MoveType.Before,
				i => i.MatchLdloc(out x),
				i => i.MatchLdloc(out y),
				i => i.MatchLdcI4(out _),
				i => i.MatchLdcI4(out _),
				i => i.MatchCall<WorldGen>("Convert")
			);
			c.GotoLabel(skipLabel, MoveType.Before);
			c.Index--;
			c.Next.MatchBr(out skipLabel);
			c.Index = index;
			c.Emit(OpCodes.Ldloc, x);
			c.Emit(OpCodes.Ldloc, y);
			c.EmitDelegate<Action<int, int>>((x, y) => {
				ALConvert.ConvertTile(x, y, GetRemixBiome(x));
			});
			c.Emit(OpCodes.Br, skipLabel);
			c.GotoLabel(skipLabel, MoveType.After);


			skipLabel = default;
			c.GotoNext(MoveType.AfterLabel,
				i => i.MatchLdsfld<WorldGen>("drunkWorldGen"),
				i => i.MatchBrfalse(out skipLabel)
			);
			index = c.Index;
			x = -1;
			y = -1;
			c.GotoNext(MoveType.Before,
				i => i.MatchLdloc(out x),
				i => i.MatchLdloc(out y),
				i => i.MatchCall<Tilemap>("get_Item")
			);
			c.GotoLabel(skipLabel, MoveType.Before);
			skipLabel = (ILLabel)c.Prev.Operand;
			c.Index = index - 1;
			c.GotoNext(MoveType.AfterLabel,
				i => i.MatchLdsfld<WorldGen>("drunkWorldGen"),
				i => i.MatchBrfalse(out _)
			);
			c.Emit(OpCodes.Ldloc, x);
			c.Emit(OpCodes.Ldloc, y);
			c.EmitDelegate<Action<int, int>>((x, y) => {
				AltBiome biome = GetRemixBiome(x);
				Main.tile[x, y].TileType = (ushort)(biome.BiomeFlesh ?? Main.tile[x, y].TileType);
			});
			c.Emit(OpCodes.Br, skipLabel);
			c.GotoLabel(skipLabel, MoveType.After);


			skipLabel = default;
			c.GotoNext(MoveType.Before,
				i => i.MatchLdsfld<WorldGen>("drunkWorldGen"),
				i => i.MatchBrfalse(out skipLabel)
			);
			index = c.Index;
			x = -1;
			y = -1;
			c.GotoNext(MoveType.Before,
				i => i.MatchLdloc(out x),
				i => i.MatchLdloc(out y),
				i => i.MatchCall<Tilemap>("get_Item")
			);
			c.GotoLabel(skipLabel, MoveType.Before);
			skipLabel = (ILLabel)c.Prev.Operand;
			c.Index = index - 1;
			c.GotoNext(MoveType.AfterLabel,
				i => i.MatchLdsfld<WorldGen>("drunkWorldGen"),
				i => i.MatchBrfalse(out _)
			);
			c.Emit(OpCodes.Ldloc, x);
			c.Emit(OpCodes.Ldloc, y);
			c.EmitDelegate<Action<int, int>>((x, y) => {
				Main.tile[x, y].WallType = (ushort)(GetRemixBiome(x).BiomeFleshWall ?? Main.tile[x, y].WallType);
			});
			c.Emit(OpCodes.Br, skipLabel);
			c.GotoLabel(skipLabel, MoveType.After);
		}
		private static void IL_GenPasses_SpreadingGrass(ILContext il) {
			ILCursor c = new(il);
			while(c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(out _),
				i => i.MatchLdloc(out _),
				i => i.MatchLdcI4(out _),
				i => i.MatchLdcI4(out _),
				i => i.MatchCall<WorldGen>("Convert")
			)) {
				c.Index--;
				c.Remove();
				c.EmitDelegate<Action<int, int, int, int>>(DrunkRemixGrassConvert);
			}
			c.GotoNext(MoveType.After,
				i => i.MatchLdloc(out _),
				i => i.MatchLdloc(out _),
				i => i.MatchLdloc(out _),
				i => i.MatchLdcI4(out _),
				i => i.MatchCall<WorldGen>("Convert")
			);
			c.Index--;
			c.Remove();
			c.EmitDelegate<Action<int, int, int, int>>(SoberRemixGrassConvert);
		}
		public static void DrunkRemixGrassConvert(int i, int j, int conversionType, int size = 4) {
			if (size != 1 || (conversionType != 1 && conversionType != 4)) {
				WorldGen.Convert(i, j, conversionType, size);
				return;
			}
			AltBiome evil = conversionType == 4 ? WorldBiomeManager.GetDrunkEvil(true) : WorldBiomeManager.GetWorldEvil(true);
			ALConvert.Convert(evil, i, j, 1);
		}
		public static void SoberRemixGrassConvert(int i, int j, int conversionType, int size = 4) {
			if (size != 1 || (conversionType != 1 && conversionType != 4)) {
				WorldGen.Convert(i, j, conversionType, size);
				return;
			}
			AltBiome evil = WorldBiomeManager.GetWorldEvil(true);
			ALConvert.Convert(evil, i, j, 1);
		}

		private static void IL_WorldGen_IslandHouse(ILContext il) {
			void HookParams(ILCursor c, int breakIndex, params (int value, Func<int, int> hook)[] hooks) {
				ILLabel returnIndex = c.MarkLabel();
				foreach ((int value, Func<int, int> hook) in hooks) {
					c.GotoPrev(MoveType.After,
						i => i.MatchLdcI4(value)
					);
					if (c.Index < breakIndex) {
						AltLibrary.Instance.Logger.Error($"failed HookParams looking for constant {value} between {breakIndex} and {returnIndex}");
						throw new ILPatchFailureException(AltLibrary.Instance, il, null);
					}
					c.EmitDelegate(hook);
					c.Index -= 2;
				}
				c.GotoLabel(returnIndex, MoveType.After);
			}
			ILCursor c = new(il);
			#region Doors
			c.GotoNext(MoveType.After,
				i => i.MatchLdsfld<WorldGen>("remixWorldGen"),
				i => i.MatchBrfalse(out _)
			);
			int breakIndex = c.Index;

			//crimson
			c.GotoNext(MoveType.After,
				i => i.MatchCall<WorldGen>("PlaceTile")
			);
			HookParams(c, breakIndex,
				(5, (v) => WorldGen.drunkWorldGen ? WorldBiomeManager.GetDrunkEvil(true).FleshDoorTileStyle ?? v : v),
				(TileID.ClosedDoor, (v) => WorldGen.drunkWorldGen ? WorldBiomeManager.GetDrunkEvil(true).FleshDoorTile ?? v : v)
			);
			breakIndex = c.Index;
			//other
			c.GotoNext(MoveType.After,
				i => i.MatchCall<WorldGen>("PlaceTile")
			);
			HookParams(c, breakIndex,
				(38, (v) => WorldBiomeManager.GetWorldEvil(true).FleshDoorTileStyle ?? v),
				(TileID.ClosedDoor, (v) => WorldBiomeManager.GetWorldEvil(true).FleshDoorTile ?? v)
			);
			#endregion Doors

			#region Tables/Chairs
			//crimson
			c.GotoNext(MoveType.After,
				i => i.MatchLdsfld<WorldGen>("remixWorldGen"),
				i => i.MatchBrfalse(out _)
			);
			breakIndex = c.Index;

			c.GotoNext(MoveType.After,
				i => i.MatchCall<WorldGen>("PlaceTile")
			);
			HookParams(c, breakIndex,
				(5, (v) => WorldGen.drunkWorldGen ? WorldBiomeManager.GetDrunkEvil(true).FleshTableTileStyle ?? v : v),
				(TileID.Tables, (v) => WorldGen.drunkWorldGen ? WorldBiomeManager.GetDrunkEvil(true).FleshTableTile ?? v : v)
			);
			breakIndex = c.Index;

			for (int i = 0; i < 2; i++) {
				c.GotoNext(MoveType.After,
					i => i.MatchCall<WorldGen>("PlaceTile")
				);
				HookParams(c, breakIndex,
					(8, (v) => WorldGen.drunkWorldGen ? WorldBiomeManager.GetDrunkEvil(true).FleshChairTileStyle ?? v : v),
					(TileID.Chairs, (v) => WorldGen.drunkWorldGen ? WorldBiomeManager.GetDrunkEvil(true).FleshChairTile ?? v : v)
				);
				breakIndex = c.Index;
			}

			//other
			c.GotoNext(MoveType.After,
				i => i.MatchCall<WorldGen>("PlaceTile")
			);
			HookParams(c, breakIndex,
				(2, (v) => WorldBiomeManager.GetWorldEvil(true).FleshTableTileStyle ?? v),
				(TileID.Tables2, (v) => WorldBiomeManager.GetWorldEvil(true).FleshTableTile ?? v)
			);
			breakIndex = c.Index;

			for (int i = 0; i < 2; i++) {
				c.GotoNext(MoveType.After,
					i => i.MatchCall<WorldGen>("PlaceTile")
				);
				HookParams(c, breakIndex,
					(38, (v) => WorldBiomeManager.GetWorldEvil(true).FleshChairTileStyle ?? v),
					(TileID.Chairs, (v) => WorldBiomeManager.GetWorldEvil(true).FleshChairTile ?? v)
				);
				breakIndex = c.Index;
			}
			#endregion Tables/Chairs
		}
		delegate void ManipChestType(ref int type, ref int frame);
		private static void IL_WorldGen_AddBuriedChest(ILContext il) {
			ILCursor c = new(il);
			ILLabel skipLabel = default;
			c.GotoNext(MoveType.After,
				i => i.MatchLdsfld<WorldGen>("remixWorldGen"),
				i => i.MatchBrfalse(out skipLabel),
				i => i.MatchLdsfld<WorldGen>("getGoodWorldGen"),
				i => i.MatchBrtrue(out ILLabel l) && l.Target == skipLabel.Target
			);
			int typeArg = -1;
			int frameLoc = -1;
			int index = c.Index;
			c.GotoNext(
				i => i.MatchLdcI4(467),
				i => i.MatchStarg(out typeArg),
				i => i.MatchLdcI4(3),
				i => i.MatchStloc(out frameLoc)
			);
			c.Index = index;
			c.Emit(OpCodes.Ldarga, typeArg);
			c.Emit(OpCodes.Ldloca, frameLoc);
			c.EmitDelegate<ManipChestType>((ref int type, ref int frame) => {
				type = WorldBiomeManager.GetWorldEvil(true).FleshChestTile ?? type;
				frame = WorldBiomeManager.GetWorldEvil(true).FleshChestTileStyle ?? frame;
			});
			c.Emit(OpCodes.Br, skipLabel);
		}

	}
}
