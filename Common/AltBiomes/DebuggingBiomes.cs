#if DEBUG && false
namespace AltLibrary.Common.AltBiomes {
	public class DebuggingHallowBiome : AltBiome {
		public override void SetStaticDefaults() {
			BiomeType = BiomeType.Hallow;
		}
	}
	public class DebuggingHellBiome : AltBiome {
		public override void SetStaticDefaults() {
			BiomeType = BiomeType.Hell;
		}
	}
	public class DebuggingJungleBiome : AltBiome {
		public override void SetStaticDefaults() {
			BiomeType = BiomeType.Jungle;
		}
	}
	public class DebuggingNoneBiome : AltBiome {
		public override void SetStaticDefaults() {
			BiomeType = BiomeType.None;
		}
	}
	public class DebuggingNoneBiome2 : AltBiome {
		public override void SetStaticDefaults() {
			BiomeType = BiomeType.None;
		}
	}
}
#endif
