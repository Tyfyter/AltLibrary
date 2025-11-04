using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using static AltLibrary.Core.Grasses;

namespace AltLibrary.Core {
	public static class Grasses {
		static readonly List<GrassType> grasses = [];
		public static IReadOnlyList<GrassType> GrassTypes => grasses;
		public static GrassType Register(GrassGrowthSettings settings, params (int dirtType, int grassType)[] types) {
			GrassType existing = null;
			for (int j = 0; j < grasses.Count && (existing is null); j++) {
				GrassType possible = grasses[j];
				for (int i = 0; i < types.Length && (existing is null); i++) {
					if (possible.TypesByDirt.Contains(types[i].grassType)) {
						existing = possible;
						break;
					}
				}
			}
			GrassType newGrass = new(settings, types);
			if (existing is not null) {
				if ((newGrass.GrassGrowthSettings != existing.GrassGrowthSettings) || !newGrass.TypesByDirt.SequenceEqual(existing.TypesByDirt)) {
					throw new ArgumentException($"A grass type already exists which registers one or more of the same grass tiles", nameof(types));
				}
				return existing;
			}
			newGrass.GrassID = grasses.Count;
			grasses.Add(newGrass);
			for (int i = 0; i < types.Length; i++) {
				GrassSpreading.GrassTypeIDs[types[i].grassType] = newGrass.GrassID;
			}
			return newGrass;
		}
		public record struct GrassGrowthSettings(bool Infection = false, bool AlwaysGrowUnderground = false);
	}
	public class GrassType {
		public int GrassID { get; internal set; }
		public GrassGrowthSettings GrassGrowthSettings { get; init; }
		public ReadOnlySpan<int> TypesByDirt => typesByDirt;
		readonly int[] typesByDirt = TileID.Sets.Factory.CreateIntSet();
		internal GrassType(GrassGrowthSettings settings, params (int dirtType, int grassType)[] types) {
			GrassGrowthSettings = settings;
			for (int i = 0; i < types.Length; i++) {
				typesByDirt[types[i].dirtType] = types[i].grassType;
			}
		}
	}
}
