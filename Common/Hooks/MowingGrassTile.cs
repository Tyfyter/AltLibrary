using AltLibrary.Common.AltBiomes;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;

namespace AltLibrary.Common.Hooks {
	internal class MowingGrassTile {
		public static void Init() {
			IL_Player.MowGrassTile += Player_MowGrassTile;
		}

		public static void Unload() { }

		private static void Player_MowGrassTile(ILContext il) {
			ILCursor c = new(il);
			if (!c.TryGotoNext(MoveType.AfterLabel,
				i => i.MatchLdloc(2),
				i => i.MatchBrfalse(out _)
			) && !c.TryGotoNext(MoveType.AfterLabel,
				i => i.MatchLdloc(2),
				i => i.MatchLdcI4(0),
				i => i.MatchCgtUn() || i.MatchBgtUn(out _)
			)) {
				AltLibrary.Instance.Logger.Error("Could not find num != 0 in Player_MowGrassTile");
				return;
			}

			c.Index++;
			c.Emit(OpCodes.Ldloc, 1);
			c.EmitDelegate<Func<int, Tile, int>>((mowedTileType, tile) => {
				foreach (AltBiome biome in AltLibrary.Biomes) {
					if (biome.BiomeGrass.HasValue && tile.TileType == biome.BiomeGrass.Value && biome.BiomeMowedGrass.HasValue) {
						return biome.BiomeMowedGrass.Value;
					}
				}
				return mowedTileType;
			});
			c.Emit(OpCodes.Stloc, 2);
			c.Emit(OpCodes.Ldloc, 2);
		}
	}
}
