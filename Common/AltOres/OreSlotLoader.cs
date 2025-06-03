using AltLibrary.Common.AltBiomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltOres {
	public class OreSlotLoader : ILoadable {
		internal static List<OreSlot> oreSlots = [];
		public static int OreSlotCount => oreSlots.Count;
		internal static int[][] oreder;
		internal static void SortOreSlots() {
			if ((oreSlots?.Count ?? 0) <= 0) return;
			TopoSort<OreSlot> sort = new(oreSlots,
				slot => slot.SortAfter(),
				slot => slot.SortBefore()
			);
			oreSlots = sort.Sort();

			oreder = new int[oreSlots.Count][];
			for (int i = 0; i < oreSlots.Count; i++) {
				oreSlots[i].Type = i;
				oreder[i] = new int[oreSlots.Count];
			}
			for (int i = 0; i < oreSlots.Count; i++) {
				foreach (OreSlot item in sort.AllDependencies(oreSlots[i])) {
					oreder[i][item.Type] = 1;
					oreder[item.Type][i] = -1;
				}
				foreach (OreSlot item in sort.AllDependendents(oreSlots[i])) {
					oreder[i][item.Type] = -1;
					oreder[item.Type][i] = 1;
				}
			}
		}
		public static OreSlot GetOreSlot(int type) => oreSlots[type];
		public static IEnumerable<AltOre> GetOres(int type) => GetOres(oreSlots[type]);
		public static IEnumerable<AltOre> GetOres<TOreSlot>() where TOreSlot : OreSlot => GetOres(ModContent.GetInstance<TOreSlot>());
		public static IEnumerable<AltOre> GetOres(OreSlot slot) {
			for (int i = 0; i < AltLibrary.Ores.Count; i++) {
				if (AltLibrary.Ores[i].OreSlot == slot) yield return AltLibrary.Ores[i];
			}
		}
		public void Load(Mod mod) { }
		public void Unload() => oreSlots = null;
	}
	[ReinitializeDuringResizeArrays]
	static class Orederer {
		static Orederer() {
			OreSlotLoader.SortOreSlots();
		}
	}
	public abstract class OreSlot : ModType, ILocalizedModType, IComparable<OreSlot> {
		public virtual IEnumerable<OreSlot> SortAfter() => [];
		public virtual IEnumerable<OreSlot> SortBefore() => [];
		public int Type { get; internal set; }
		public virtual bool Selectable => true;
		public virtual bool Hardmode => false;
		/// <summary>
		/// The name of this ore that will display on the biome selection screen.
		/// </summary>
		public virtual LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);
		public string LocalizationCategory => "AltOreSlot";
		protected sealed override void Register() {
			ModTypeLookup<OreSlot>.Register(this);
			OreSlotLoader.oreSlots.Add(this);
			_ = DisplayName;
		}
		/// <summary>
		/// Used for unloaded ores
		/// </summary>
		public abstract AltOre FallbackOre { get; }
		public int CompareTo(OreSlot other) => OreSlotLoader.oreder[Type][other.Type];
		public static bool operator <(OreSlot left, OreSlot right) => left.CompareTo(right) < 0;
		public static bool operator <=(OreSlot left, OreSlot right) => left.CompareTo(right) <= 0;
		public static bool operator >(OreSlot left, OreSlot right) => left.CompareTo(right) > 0;
		public static bool operator >=(OreSlot left, OreSlot right) => left.CompareTo(right) >= 0;
	}
}
