using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks {
	internal class TenthAnniversaryFix {
		public static void Init() {
			IL_WorldGen.ConvertSkyIslands += WorldGen_ConvertSkyIslands;
			IL_WorldGen.IslandHouse += WorldGen_IslandHouse;
		}

		public static void Unload() { }
		static int style = 0;
		//TODO: I can see how this is supposed to work, and as far as I can tell it should do so consistently, but there has to be a better way to do it
		private static void WorldGen_IslandHouse(ILContext il) {
			ILCursor c = new(il);
			if (!c.TryGotoNext(MoveType.After, i => i.MatchLdcI4(207))) {
				AltLibrary.Instance.Logger.Error("Could not find fountain placement in WorldGen_IslandHouse");
				return;
			}
			c.Emit(OpCodes.Ldarg, 2);
			c.EmitDelegate<Func<ushort, int, ushort>>((orig, style) => {
				if (WorldGen.remixWorldGen) {
					if (style == 5 && WorldGen.drunkWorldGen && WorldBiomeManager.GetDrunkEvil(true).FountainTile is int crimsonTile) {
						TenthAnniversaryFix.style = 3;
						return (ushort)crimsonTile;
					}
					if (WorldBiomeManager.GetWorldEvil(true).FountainTile is int corruptTile) {
						TenthAnniversaryFix.style = 2;
						return (ushort)corruptTile;
					}
				}
				if (WorldGen.tenthAnniversaryWorldGen && WorldBiomeManager.GetWorldHallow(true).FountainTile is int value) {
					TenthAnniversaryFix.style = 1;
					return (ushort)value;
				}
				return orig;
			});
			if (!c.TryGotoNext(MoveType.After, i => i.MatchLdarg(2))) {
				AltLibrary.Instance.Logger.Error("Missing islandStyle in WorldGen_IslandHouse fountain placement");
				return;
			}
			c.EmitDelegate<Func<int, int>>((orig) => {
				switch (TenthAnniversaryFix.style) {
					case 1:
					return WorldBiomeManager.GetWorldHallow(true).FountainTileStyle ?? orig;
					case 2:
					return WorldBiomeManager.GetWorldEvil(true).FountainTileStyle ?? orig;
					case 3:
					return WorldBiomeManager.GetDrunkEvil(true).FountainTileStyle ?? orig;
				}
				return orig;
			});
			if (!c.TryGotoNext(i => i.MatchCall(out _))) {
				AltLibrary.Instance.Logger.Error("o $ 3");
				return;
			}
			c.Remove();
			c.EmitDelegate<Action<int, int, ushort, int>>((x, y, type, style) => {
				short frameX = 0;
				short frameY = 0;
				AltBiome biome = null;
				switch (TenthAnniversaryFix.style) {
					case 1:
					biome = WorldBiomeManager.GetWorldHallow(true);
					break;

					case 2:
					biome = WorldBiomeManager.GetWorldEvil(true);
					break;

					case 3:
					biome = WorldBiomeManager.GetDrunkEvil(true);
					break;
				}
				if (biome is not null) {
					frameX = (short)biome.FountainActiveFrameX.GetValueOrDefault();
					frameY = (short)biome.FountainActiveFrameY.GetValueOrDefault();
				}

				UselessCallThatDoesTechnicallyNothing(x, y, type, style, frameX, frameY);
			});
			if (!c.TryGotoNext(i => i.MatchCall<WorldGen>(nameof(WorldGen.SwitchFountain))))
			{
				AltLibrary.Instance.Logger.Error("o $ 5");
				return;
			}
			c.Index -= 2;
			c.RemoveRange(3);
		}

		internal static void UselessCallThatDoesTechnicallyNothing(int x, int y, ushort type, int style = 0, short frameX = 0, short frameY = 0) {
			if (type != 0) {
				WorldGen.Place2xX(x, y, type, style);
				if (type != 207 && Main.tile[x, y].HasTile) {
					Main.tile[x, y].TileFrameX = frameX;
					Main.tile[x, y].TileFrameY = frameY;
				} else {
					WorldGen.SwitchFountain(x, y);
				}
			}
		}

		private static void WorldGen_ConvertSkyIslands(ILContext il) {
			ILCursor c = new(il);

			if (!c.TryGotoNext(i => i.MatchCall<WorldGen>(nameof(WorldGen.Convert)))) {
				AltLibrary.Instance.Logger.Error($"Could not find {nameof(WorldGen.Convert)} call in WorldGen.ConvertSkyIslands");
				return;
			}
			c.Index++;
			c.Emit(OpCodes.Ldloc, 4);
			c.Emit(OpCodes.Ldloc, 5);
			c.EmitDelegate<Action<int, int>>((i, j) => {
				if (WorldBiomeManager.WorldHallowName != "" && WorldBiomeManager.WorldHallowBiome.BiomeGrass is int biomeGrass) {
					int size = 1;
					for (int l = i - size; l <= i + size; l++) {
						for (int k = j - size; k <= j + size; k++) {
							Tile tile = Main.tile[l, k];
							int type = tile.TileType;
							if (TileID.Sets.Conversion.Grass[type] && type == TileID.HallowedGrass) {
								tile = Main.tile[l, k];
								tile.TileType = (ushort)biomeGrass;
								WorldGen.SquareTileFrame(l, k, true);
								NetMessage.SendTileSquare(-1, l, k, TileChangeType.None);
							}
						}
					}
				}
			});
		}
	}
}
