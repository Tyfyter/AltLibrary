using PegasusLib;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltBiomes {
	/// <summary>
	/// Handles <see cref="IsActive"/> for the provided alt biome, otherwise the same as <see cref="FishingLootPool"/>
	/// </summary>
	public class FishingLootPool<TAltBiome> : FishingLootPool where TAltBiome : AltBiome {
		public override bool IsActive(Player player, FishingAttempt attempt) {
			if (player.ZoneDungeon) return false;
			TAltBiome biome = ModContent.GetInstance<TAltBiome>();
			if (biome.BiomeType is not BiomeType.Evil or BiomeType.None) {
				if (player.ZoneDesert && Main.rand.NextBool()) return false;
				foreach (AltBiome other in AltLibrary.AllBiomes) {
					if (other == biome) continue;
					if (other.BiomeType > biome.BiomeType) return false;
				}
			}
			return biome.Biome.IsInBiome(player);
		}
	}
}
