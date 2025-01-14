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

		private static void SendOriginalToToBackOfList(AltOre[] list, int original) {
			if (list.Length <= 1)
				return;
			for (int x = 0; x < list.Length - 1; x++) {
				if (list[x].Type == original) {
					(list[^1], list[x]) = (list[x], list[^1]);
					return;
				}
			}
		}

		internal static void BakeDrunken() {
			UnifiedRandom rngSeed = new(WorldGen._genRandSeed); //bake seed later
			List<AltOre> hardmodeListing = ALWorldCreationLists.prehmOreData.Types.FindAll(x => x.includeInHardmodeDrunken || x.OreType >= OreType.Cobalt & x.OreType != OreType.None);

			WorldBiomeManager.drunkCobaltCycle = hardmodeListing.Where(x => x.OreType == OreType.Cobalt && x.Selectable).ToArray();
			WorldBiomeManager.drunkMythrilCycle = hardmodeListing.Where(x => x.OreType == OreType.Mythril && x.Selectable).ToArray();
			WorldBiomeManager.drunkAdamantiteCycle = hardmodeListing.Where(x => x.OreType == OreType.Adamantite && x.Selectable).ToArray();

			SendOriginalToToBackOfList(WorldBiomeManager.drunkCobaltCycle, WorldBiomeManager.Cobalt);
			SendOriginalToToBackOfList(WorldBiomeManager.drunkMythrilCycle, WorldBiomeManager.Mythril);
			SendOriginalToToBackOfList(WorldBiomeManager.drunkAdamantiteCycle, WorldBiomeManager.Adamantite);

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
