using AltLibrary.Common.AltOres;
using AltLibrary.Common.Systems;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;

namespace AltLibrary.Core.Baking {
	internal class DrunkenBaking {
		//move to utility function later
		private static void ShuffleArrayUsingSeed<T>(T[] list, UnifiedRandom seed) {
			int randIters = list.Length - 1; //-1 cause we don't wanna shuffle the back
			if (randIters == 1)
				return;
			while (randIters > 1) {
				int thisRand = seed.Next(randIters);
				randIters--;
				if (thisRand != randIters) {
					(list[thisRand], list[randIters]) = (list[randIters], list[thisRand]);
				}
			}
		}

		private static void SendOriginalToToBackOfList(AltOre[] list, AltOre original) {
			if (list.Length <= 1)
				return;
			for (int x = 0; x < list.Length - 1; x++) {
				if (list[x] == original) {
					(list[^1], list[x]) = (list[x], list[^1]);
					return;
				}
			}
		}

		internal static void BakeDrunken() {
			UnifiedRandom rngSeed = new(WorldGen._genRandSeed); //bake seed later
			List<AltOre> hardmodeListing = AltLibrary.Ores.FindAll(x => x.includeInHardmodeDrunken || x.OreSlot.Hardmode);

			WorldBiomeManager.drunkCobaltCycle = OreSlotLoader.GetOres<CobaltOreSlot>().Where(o => o.Selectable).ToArray();
			WorldBiomeManager.drunkMythrilCycle = OreSlotLoader.GetOres<MythrilOreSlot>().Where(o => o.Selectable).ToArray();
			WorldBiomeManager.drunkAdamantiteCycle = OreSlotLoader.GetOres<AdamantiteOreSlot>().Where(o => o.Selectable).ToArray();

			SendOriginalToToBackOfList(WorldBiomeManager.drunkCobaltCycle, WorldBiomeManager.GetAltOre<CobaltOreSlot>());
			SendOriginalToToBackOfList(WorldBiomeManager.drunkMythrilCycle, WorldBiomeManager.GetAltOre<MythrilOreSlot>());
			SendOriginalToToBackOfList(WorldBiomeManager.drunkAdamantiteCycle, WorldBiomeManager.GetAltOre<AdamantiteOreSlot>());

			ShuffleArrayUsingSeed(WorldBiomeManager.drunkCobaltCycle, rngSeed);
			ShuffleArrayUsingSeed(WorldBiomeManager.drunkMythrilCycle, rngSeed);
			ShuffleArrayUsingSeed(WorldBiomeManager.drunkAdamantiteCycle, rngSeed);
		}

		internal static void GetDrunkenOres() {
			if (WorldBiomeManager.drunkCobaltCycle == null)
				BakeDrunken();

			int cobaltCycle = WorldBiomeManager.hmOreIndex % WorldBiomeManager.drunkCobaltCycle.Length;
			int mythrilCycle = WorldBiomeManager.hmOreIndex % WorldBiomeManager.drunkMythrilCycle.Length;
			int adamantiteCycle = WorldBiomeManager.hmOreIndex % WorldBiomeManager.drunkAdamantiteCycle.Length;

			WorldGen.SavedOreTiers.Cobalt = WorldBiomeManager.drunkCobaltCycle[cobaltCycle].ore;
			WorldGen.SavedOreTiers.Mythril = WorldBiomeManager.drunkMythrilCycle[mythrilCycle].ore;
			WorldGen.SavedOreTiers.Adamantite = WorldBiomeManager.drunkAdamantiteCycle[adamantiteCycle].ore;

			if (cobaltCycle == 0 && mythrilCycle == 0 && adamantiteCycle == 0)
				WorldBiomeManager.hmOreIndex = 0;
			WorldBiomeManager.hmOreIndex++;
		}
	}
}
