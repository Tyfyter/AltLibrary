using ReLogic.Reflection;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common {
	public class AltBiomeID {
		[ReinitializeDuringResizeArrays]
		public static class Sets {
			public static SetFactory Factory { get; } = new(AltLibrary.Biomes.Count, nameof(AltBiomeID), Search);
		}
		/// <inheritdoc cref="IdDictionary"/>
		public static readonly IdDictionary Search = IdDictionary.Create<AltBiomeID, int>();
	}
}
