using AltLibrary.Common.AltOres;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks {
	public class AltOreInsideBodies {
		internal static void Setup() {
			foreach (AltOre ore in AltLibrary.Ores.Where(x => x.OreType < OreType.Cobalt)) {
				ItemID.Sets.OreDropsFromSlime.TryAdd(TileLoader.GetItemDropFromTypeAndStyle(ore.ore), (3, 13));
			}
		}
	}
}
