using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary {
	internal class AltLibraryGlobalItem : GlobalItem {
		public static Dictionary<int, bool> HallowedOreList;
		public override bool IsAnglerQuestAvailable(int type) {
			switch (type) {
				case ItemID.Cursedfish or ItemID.EaterofPlankton or ItemID.InfectedScabbardfish:
				case ItemID.Ichorfish or ItemID.BloodyManowar:
				return !WorldBiomeManager.IsAnyModdedEvil;
				default:
				return true;
			}
		}

		class AltLibraryOre_Loader : ILoadable {
			public void Load(Mod mod) {
				On_WorldGen.OreRunner += OreRunner_ReplaceHallowedOre;
				HallowedOreList = [];
			}

			public void Unload() {
				HallowedOreList = null;
			}

			private static void OreRunner_ReplaceHallowedOre(On_WorldGen.orig_OreRunner orig, int i, int j, double strength, int steps, ushort type) {
				if (HallowedOreList.Count == 0 || WorldBiomeManager.WorldHallowName == "") {
					orig(i, j, strength, steps, type);
					return;
				}
				if (HallowedOreList.ContainsKey(type)) {
					AltBiome biome = WorldBiomeManager.WorldHallowBiome;
					if (biome.BiomeOre != null)
						type = (ushort)biome.BiomeOre.Value;
				}
				orig(i, j, strength, steps, type);
			}
		}
	}
}
