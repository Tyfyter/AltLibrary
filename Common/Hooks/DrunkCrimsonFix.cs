using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks {
	internal class DrunkCrimsonFix : ILoadable {
		public void Load(Mod mod) {
			Terraria.IL_Main.UpdateTime_StartDay += Main_UpdateTime_StartDay;
		}
		public void Unload() { }
		private static void Main_UpdateTime_StartDay(ILContext il) {
			ILCursor c = new(il);
			Func<Instruction, bool>[] predicates = [
				i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.crimson)),
				i => i.MatchLdcI4(0),
				i => i.MatchCeq(),
				i => i.MatchStsfld<WorldGen>(nameof(WorldGen.crimson))
			];
			if (!c.TryGotoNext(MoveType.Before, predicates)) {
				AltLibrary.Instance.Logger.Error("Could not find WorldGen.crimson = !WorldGen.crimson in Main.UpdateTime_StartDay");
				return;
			}
			c.EmitDelegate(() => {
				AltBiome currentEvil = WorldBiomeManager.GetDrunkEvil();
				bool foundCurrent = false;
				bool shouldContinue = true;
				for (int i = 0; shouldContinue && i < AltLibrary.AllBiomes.Count; i++) {
					AltBiome otherBiome = AltLibrary.AllBiomes[i];
					if (otherBiome.BiomeType == BiomeType.Evil) {
						if (foundCurrent) {
							WorldBiomeManager.DrunkEvil = otherBiome;
							shouldContinue = false;
						} else if (otherBiome == currentEvil) {
							foundCurrent = true;
						}
					}
				}
				for (int i = 0; shouldContinue && i < AltLibrary.AllBiomes.Count; i++) {
					AltBiome otherBiome = AltLibrary.AllBiomes[i];
					if (otherBiome.BiomeType == BiomeType.Evil) {
						WorldBiomeManager.DrunkEvil = otherBiome;
						shouldContinue = false;
					}
				}
				WorldGen.crimson = WorldBiomeManager.DrunkEvil is CrimsonAltBiome;
			});
			ILLabel skipVanilla = c.DefineLabel();
			c.Emit(OpCodes.Br, skipVanilla);
			c.Index += predicates.Length;
			c.MarkLabel(skipVanilla);
		}
	}
}
