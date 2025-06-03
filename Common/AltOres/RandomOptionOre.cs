using AltLibrary.Core;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltOres {
	internal sealed class RandomOptionOre : AltOre {
		public override Color NameColor => Color.Yellow;
		readonly OreSlot oreSlot;
		public override OreSlot OreSlot => oreSlot;
		public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(oreSlot.DisplayName);
		public override LocalizedText Description => base.Description.WithFormatArgs(oreSlot.DisplayName);
		public RandomOptionOre(OreSlot oreSlot) : base() {
			ALReflection.ModType_Mod.SetValue(this, AltLibrary.Instance);
			this.oreSlot = oreSlot;
		}

		public override bool IsLoadingEnabled(Mod mod) => false;
	}
}
