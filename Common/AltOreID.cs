using ReLogic.Reflection;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common {
	public class AltOreID {
		[ReinitializeDuringResizeArrays]
		public static class Sets {
			public static SetFactory Factory { get; } = new(AltLibrary.Ores.Count, nameof(AltOreID), Search);
		}
		/// <inheritdoc cref="IdDictionary"/>
		public static readonly IdDictionary Search = IdDictionary.Create<AltOreID, int>();
	}
}
