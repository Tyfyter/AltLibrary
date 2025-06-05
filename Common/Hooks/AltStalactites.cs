using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks {
	[ReinitializeDuringResizeArrays]
	public class AltStalactites : ILoadable {
		public static bool[] IsStalactite { get; } = TileID.Sets.Factory.CreateBoolSet();
		public static int[] StalactiteTypeForAnchor { get; } = TileID.Sets.Factory.CreateIntSet(-1);
		public static void AddStalactite(int stalactite, params int[] anchors) {
			IsStalactite[stalactite] = true;
			for (int i = 0; i < anchors.Length; i++) StalactiteTypeForAnchor[anchors[i]] = stalactite;
		}
		public void Unload() { }
		public void Load(Mod mod) {
			On_WorldGen.GetDesiredStalagtiteStyle += On_WorldGen_GetDesiredStalagtiteStyle;
			IL_WorldGen.PaintTheSand += Stalacheck;
			IL_WorldGen.PlaceTile += Stalacheck;
			IL_WorldGen.PlaceTight += Stalacheck;
			IL_WorldGen.BlockBelowMakesSandFall += Stalacheck;
			IL_WorldGen.TileFrame += Stalacheck;
			IL_WorldGen.ReplaceTile_EliminateNaturalExtras += Stalacheck;
		}

		private void Stalacheck(ILContext il) {
			ILCursor c = new(il);
			while (c.TryGotoNext(MoveType.AfterLabel,
				i => i.MatchLdcI4(TileID.Stalactite),
				i => i.MatchBeq(out _) || i.MatchBneUn(out _)
			)) {
				c.EmitDelegate((int type) => IsStalactite[type] ? TileID.Stalactite : type);
				c.Index++;
			}
		}

		private void On_WorldGen_GetDesiredStalagtiteStyle(On_WorldGen.orig_GetDesiredStalagtiteStyle orig, int i, int j, out bool fail, out int desiredStyle, out int height, out int y) {
			orig(i, j, out fail, out desiredStyle, out height, out y);
			if (fail) {
				if (StalactiteTypeForAnchor[desiredStyle] != -1) {
					ushort stalactype = (ushort)StalactiteTypeForAnchor[desiredStyle];
					fail = false;
					desiredStyle = 7;
					for (int k = y; k < y + height; k++) {
						Main.tile[i, k].TileType = stalactype;
					}
				}
			} else if (IsStalactite[Main.tile[i, j].TileType]) {
				for (int k = y; k < y + height; k++) {
					Main.tile[i, k].TileType = TileID.Stalactite;
				}
			}
		}
	}
}
