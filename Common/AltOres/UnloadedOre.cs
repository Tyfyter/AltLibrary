using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltOres {
	[Autoload(false)]
	public class UnloadedOre : AltOre {
		public override OreSlot OreSlot => oreSlot;
		public override string FullName => name;
		private readonly OreSlot oreSlot;
		private readonly string name;
		public UnloadedOre(OreSlot oreSlot, string name) {
			this.oreSlot = oreSlot;
			this.name = name;
			AltOre fallbackOre = oreSlot.FallbackOre;
			ore = fallbackOre.ore;
			oreItem = fallbackOre.oreItem;
			bar = fallbackOre.bar;
		}
	}
}
