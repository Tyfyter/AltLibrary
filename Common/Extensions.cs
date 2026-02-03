using System.Collections.Generic;
using Terraria;

namespace AltLibrary.Common {
	static class Extensions {
		public static IEnumerable<(int index, T value)> Iterate<T>(this IList<T> values) {
			for (int i = 0; i < values.Count; i++) {
				yield return (i, values[i]);
			}
		}
		public static T GetIfInRange<T>(this IList<T> array, int index, T fallback = default) {
			if (index < 0 || index >= array.Count) return fallback;
			return array[index];
		}
	}
}
