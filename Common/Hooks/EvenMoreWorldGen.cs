using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.AltOres;
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
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using static Terraria.WorldGen;

namespace AltLibrary.Common.Hooks {
	internal static class EvenMoreWorldGen {
		public static void Init() {
			IL_WorldGen.GenerateWorld += GenPasses.ILGenerateWorld;
			GenPasses.HookGenPassShinies += GenPasses_HookGenPassShinies;
			GenPasses.HookGenPassUnderworld += GenPasses_HookGenPassUnderworld;
			GenPasses.HookGenPassAltars += ILGenPassAltars;
			GenPasses.HookGenPassMicroBiomes += GenPasses_HookGenPassMicroBiomes;
			Terraria.GameContent.Biomes.IL_MiningExplosivesBiome.Place += MiningExplosivesBiome_Place;
			IL_WorldGen.FinishRemixWorld += IL_WorldGen_FinishRemixWorld;
			MonoModHooks.Modify(GenPasses.SpreadingGrassInfo, IL_GenPasses_SpreadingGrass);
			IL_WorldGen.IslandHouse += IL_WorldGen_IslandHouse;
			IL_WorldGen.AddBuriedChest_int_int_int_bool_int_bool_ushort += IL_WorldGen_AddBuriedChest;
			//On_WorldGenRange.GetRandom += On_WorldGenRange_GetRandom;
		}

		public static void Unload() { }

		private static void MiningExplosivesBiome_Place(ILContext il) {
			ILCursor c = new(il);

			if (!c.TryGotoNext(MoveType.Before, i => i.MatchStloc(0))) {
				AltLibrary.Instance.Logger.Error("Missing stloc.0 in MiningExplosivesBiome_Place");
				return;
			}

			c.EmitDelegate<Func<ushort, ushort>>((type) => {
				type = Utils.SelectRandom(WorldGen.genRand, new ushort[]
				{
					(ushort)WorldGen.SavedOreTiers.Gold,
					(ushort)WorldGen.SavedOreTiers.Silver,
					(ushort)WorldGen.SavedOreTiers.Iron,
					(ushort)WorldGen.SavedOreTiers.Copper,
				});
				return type;
			});
		}

		private static void GenPasses_HookGenPassUnderworld(ILContext il) {
			ILCursor c = new(il);

			if (c.TryGotoNext(MoveType.Before, i => i.MatchCallvirt<GenerationProgress>("set_Message"))) {
				c.EmitDelegate<Func<string, string>>(message => WorldBiomeManager.GetWorldHell(false) is AltBiome hell && hell.GenPassName != null ? hell.GenPassName.Value : message);
			} else {
				AltLibrary.Instance.Logger.Error("Missing Message set in Underworld GenPass");
			}

			ALUtils.ReplaceIDs(il, TileID.Hellstone,
				(orig) => (ushort?)WorldBiomeManager.GetWorldHell(false).BiomeOre ?? orig,
				(orig) => WorldBiomeManager.GetWorldHell(false) is AltBiome hell && hell.BiomeOre.HasValue);

			if (!c.TryGotoNext(MoveType.AfterLabel, i => i.MatchCall<WorldGen>(nameof(WorldGen.AddHellHouses)))) {
				AltLibrary.Instance.Logger.Error("Missing WorldGen.AddHellHouses call in Underworld GenPass");
				return;
			}

			ILLabel label = c.DefineLabel();
			c.EmitDelegate(() => WorldBiomeManager.WorldHell is VanillaBiome or null);
			c.Emit(OpCodes.Brfalse_S, label);
			c.Index++;
			c.MarkLabel(label);
		}

		private static void GenPasses_HookGenPassMicroBiomes(ILContext il) {
			ILCursor c = new(il);
			if (!c.TryGotoNext(i => i.MatchLdstr("..Long Minecart Tracks")) || !c.TryGotoPrev(MoveType.AfterLabel, i => i.MatchLdarg(1))) {
				AltLibrary.Instance.Logger.Error("Missing Long Minecart Tracks micro-biome");
				return;
			}
			ILLabel label = c.MarkLabel();
			c.Index = 0;
			if (!c.TryGotoNext(i => i.MatchLdstr("..Living Trees")) || !c.TryGotoPrev(MoveType.AfterLabel, i => i.MatchLdarg(1))) {
				AltLibrary.Instance.Logger.Error("Missing Living Mahogany Trees micro-biome");
				return;
			}

			c.EmitDelegate(() => WorldBiomeManager.WorldJungle is VanillaBiome or null);
			c.Emit(OpCodes.Brfalse_S, label);
		}

		public static void ILGenPassAltars(ILContext il) {
			ILCursor c = new(il);
			while (c.TryGotoNext(MoveType.After, i => i.MatchLdcI4(TileID.DemonAltar))) {
				if (c.Previous.Previous.MatchLdsfld<Lang>(nameof(Lang.gen))) continue;
				c.EmitDelegate<Func<int, int>>((value) => {
					if (WorldBiomeManager.WorldEvil?.AltarTile is int type) return type;
					return value;
				});
				if (c.Next.MatchLdloc(out _) && c.Next.Next.MatchCall<WorldGen>(nameof(WorldGen.Place3x2))) {
					c.Index++;
					c.EmitDelegate<Func<int, int>>((value) => {
						if (WorldBiomeManager.WorldEvil?.AltarTile is not null) return 0;
						return value;
					});
				}
			}
			return;
			/*ILLabel startNormalAltar = c.DefineLabel();
			if (!c.TryGotoNext(i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.crimson)))) {
				AltLibrary.Instance.Logger.Error("ILGenPassAltars could not find use of WorldGen.crimson");
				return;
			}
			ILLabel skipNormalAltar = default;
			int x = -1;
			int y = -1;
			int distance = -1;
			c.GotoNext(MoveType.AfterLabel,
				i => i.MatchLdloc(out x),
				i => i.MatchLdloc(out y),
				i => i.MatchLdcI4(TileID.DemonAltar),
				i => i.MatchLdcI4(out distance),
				i => i.MatchCall<WorldGen>(nameof(WorldGen.IsTileNearby)),
				i => i.MatchBrtrue(out skipNormalAltar)
			);
			c.EmitDelegate(() => WorldGen.crimson || WorldBiomeManager.WorldEvilName != "");
			c.Emit(OpCodes.Brfalse, startNormalAltar);
			c.Emit(OpCodes.Ldloc, x);
			c.Emit(OpCodes.Ldloc, y);
			c.EmitDelegate((int x, int y) => {
				if (WorldBiomeManager.WorldEvilName != "" && WorldBiomeManager.WorldEvilBiome.AltarTile is int type) {
					if (!WorldGen.IsTileNearby(x, y, type, distance)) {
						WorldGen.Place3x2(x, y, (ushort)type);
						return Main.tile[x, y].TileType == type;
					}
				}
				return false;
			});
			ILLabel placedAltar = c.DefineLabel();
			placedAltar.Target = skipNormalAltar.Target;
			c.Emit(OpCodes.Brtrue, placedAltar);
			c.Emit(OpCodes.Br, skipNormalAltar);
			c.MarkLabel(startNormalAltar);
			ILLabel origPlacedAltar = default;
			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchCallOrCallvirt<Tile>("get_type"),
				i => i.MatchLdindI2(),
				i => i.MatchLdcI4(TileID.DemonAltar),
				i => i.MatchBeq(out origPlacedAltar)
			)) {
				AltLibrary.Instance.Logger.Error("e $ 3");
				return;
			}
			placedAltar.Target = origPlacedAltar.Target;*/
		}

		//TODO: make alt ores so I can test this
		private static void GenPasses_HookGenPassShinies(ILContext il) {
			ILCursor c = new(il);
			FieldReference oreField = default;
			while (c.TryGotoNext(MoveType.AfterLabel,
				i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.drunkWorldGen)),
				i => i.MatchBrfalse(out _) || i.MatchBrtrue(out _),
				i => i.MatchCall<WorldGen>("get_genRand"),
				i => i.MatchLdcI4(2),
				i => i.MatchCallvirt<UnifiedRandom>("Next"),
				i => i.MatchBrfalse(out _) || i.MatchBrtrue(out _),
				i => i.MatchLdcI4(out _),
				i => i.MatchStsfld(out oreField),
				i => i.MatchBr(out _),
				i => i.MatchLdcI4(out _),
				i => i.MatchStsfld(oreField)
			)) {
				c.SkipIf(false, true);
				List<int> list = null;
				OreSlot oreSlot = null;
				switch (oreField.Name) {
					case nameof(GenVars.copper):
					oreSlot = ModContent.GetInstance<CopperOreSlot>();
					list = [
						TileID.Copper,
						TileID.Tin
					];
					break;

					case nameof(GenVars.iron):
					oreSlot = ModContent.GetInstance<IronOreSlot>();
					list = [
						TileID.Iron,
						TileID.Lead
					];
					break;

					case nameof(GenVars.silver):
					oreSlot = ModContent.GetInstance<SilverOreSlot>();
					list = [
						TileID.Silver,
						TileID.Tungsten
					];
					break;

					case nameof(GenVars.gold):
					oreSlot = ModContent.GetInstance<GoldOreSlot>();
					list = [
						TileID.Gold,
						TileID.Platinum
					];
					break;
				}
				if (oreSlot is not null) list.AddRange(OreSlotLoader.GetOres(oreSlot).Select(o => o.ore));
				c.EmitDelegate<Func<int>>(() => WorldGen.genRand.Next(list));
				c.Emit(OpCodes.Stsfld, oreField);
			}
			c = new(il);
			while (c.TryGotoNext(MoveType.AfterLabel,
				i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.drunkWorldGen)),
				i => i.MatchBrfalse(out _) || i.MatchBrtrue(out _),
				i => i.MatchLdcI4(0),
				i => i.MatchStloc(out _),
				i => i.MatchBr(out _)
			)) {
				c.EmitDelegate(GenEvilOres);
				c.Emit(OpCodes.Ret);
				c.Index += 5;
			}
		}
		static void GenEvilOres() {
			int minY = (int)Main.rockLayer;
			int maxY = Main.maxTilesY;
			bool isDrunk = WorldGen.drunkWorldGen || ModSupport.FargoSeeds.BothEvils();
			double oreMult = 2.25E-05;
			if (WorldGen.remixWorldGen) {
				oreMult = 4.25E-05;
				if (isDrunk) {
					oreMult += 2.25E-05;
				}
			} else if (isDrunk) {
				oreMult += 2.25E-05;
			}
			int oreType = WorldBiomeManager.GetWorldEvil(true, false).BiomeOre ?? TileID.Demonite;
			List<int> list = new() {
				TileID.Demonite,
				TileID.Crimtane
			};
			foreach (AltBiome biome in AltLibrary.Biomes) {
				if (biome.BiomeType == BiomeType.Evil && biome.BiomeOre.HasValue) list.Add(biome.BiomeOre.Value);
			}
			for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * oreMult); i++) {
				if (isDrunk) {
					oreType = WorldGen.genRand.Next(list);
					if (WorldGen.remixWorldGen && i > Main.maxTilesX * Main.maxTilesY * oreMult * 0.5f) {
						minY = (int)Main.worldSurface;
						maxY = (int)Main.rockLayer;
					}
				}
				WorldGen.TileRunner(
					WorldGen.genRand.Next(0, Main.maxTilesX),
					WorldGen.genRand.Next(minY, maxY),
					WorldGen.genRand.Next(3, 6),
					WorldGen.genRand.Next(4, 8),
					oreType
				);
			}
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
				ALConvert.Convert(GetRemixBiome(x), x, y, 0);
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
			while (c.TryGotoNext(MoveType.After,
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
				WorldGen.Convert(i, j, conversionType, size, true);
				return;
			}
			AltBiome evil = conversionType == 4 ? WorldBiomeManager.GetDrunkEvil(true) : WorldBiomeManager.GetWorldEvil(true);
			ALConvert.Convert(evil, i, j, 1);
		}
		public static void SoberRemixGrassConvert(int i, int j, int conversionType, int size = 4) {
			if (size != 1 || (conversionType != 1 && conversionType != 4)) {
				WorldGen.Convert(i, j, conversionType, size, true);
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
		//TODO: this is going to need to be entirely rewritten because of declarative chest loot
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
