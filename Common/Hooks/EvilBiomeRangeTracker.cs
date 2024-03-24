using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;

namespace AltLibrary.Common.Hooks {
	internal class EvilBiomeRangeTracker {
		public static void Init() {
			IL_WorldGen.CrimEnt += ChangeOn(TileID.Crimstone);
			IL_WorldGen.CrimVein += ChangeOn(TileID.Crimstone);
			IL_WorldGen.ChasmRunner += ChangeOn(TileID.Ebonstone);
			IL_WorldGen.ChasmRunnerSideways += ChangeOn(TileID.Ebonstone);
			On_WorldGen.CrimEnt += (orig, param1, param2) => {
				WorldBiomeGeneration.ChangeRange.ResetRange();
				orig(param1, param2);
				WorldBiomeGeneration.EvilBiomeGenRanges.Add(WorldBiomeGeneration.ChangeRange.GetRange());
			};
			On_WorldGen.CrimVein += (orig, param1, param2) => {
				WorldBiomeGeneration.ChangeRange.ResetRange();
				orig(param1, param2);
				WorldBiomeGeneration.EvilBiomeGenRanges.Add(WorldBiomeGeneration.ChangeRange.GetRange());
			};
			On_WorldGen.ChasmRunner += (orig, param1, param2, param3, param4) => {
				WorldBiomeGeneration.ChangeRange.ResetRange();
				orig(param1, param2, param3, param4);
				WorldBiomeGeneration.EvilBiomeGenRanges.Add(WorldBiomeGeneration.ChangeRange.GetRange());
			};
			On_WorldGen.ChasmRunnerSideways += (orig, param1, param2, param3, param4) => {
				WorldBiomeGeneration.ChangeRange.ResetRange();
				orig(param1, param2, param3, param4);
				WorldBiomeGeneration.EvilBiomeGenRanges.Add(WorldBiomeGeneration.ChangeRange.GetRange());
			};
		}

		private static ILContext.Manipulator ChangeOn(int type) {
			return (il) => {
				ILCursor c = new(il);
				while (c.TryGotoNext(MoveType.After,
						i => i.MatchCall<Tile>("get_type"),
						i => i.MatchLdcI4(type),
						i => i.MatchStindI2()
					)) {
					int index = c.Index;
					c.Index -= 6;
					if (!c.Next.MatchCall<Tilemap>("get_Item")) continue;
					Stack<(OpCode code, object operand)> instructions = new(2);
					while (!c.Prev.MatchLdsflda<Main>(nameof(Main.tile))) {
						instructions.Push((c.Prev.OpCode, c.Prev.Operand));
						c.Index--;
					}
					c.Index = index;
					foreach (var (code, operand) in instructions) {
						c.Emit(code, operand);
					}
					c.EmitDelegate<Action<int, int>>(WorldBiomeGeneration.ChangeRange.AddChangeToRange);
				}
			};
		}

		public static void Unload() { }
	}
}
