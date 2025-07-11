using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltOres {
	public abstract class AltOre : ModTexturedType, ILocalizedModType {
		/// <summary>
		/// The TileID of the ore that will generate in the world.
		/// </summary>
		public int ore;
		/// <summary>
		/// The ItemID of the ore.
		/// </summary>
		public int oreItem;
		/// <summary>
		/// The ItemID of the bar that will generate as chest loot.
		/// </summary>
		public int bar;
		/// <summary>
		/// Whether or not this ore will appear on the selection menu.
		/// </summary>
		public bool Selectable = true;
		/// <summary>
		/// The color of this ore's name that will appear on the biome selection menu.
		/// </summary>
		public virtual Color NameColor => new(255, 255, 255);
		/// <summary>
		/// Tells the Library what ore this is an alternative to
		/// </summary>
		public abstract OreSlot OreSlot { get; }

		public int? Candle = null;
		public int? Watch = null;
		public bool IncludeInExtractinator = true;
		
		public int Type { get; internal set; }
		public string LocalizationCategory => "AltOres";
		public new virtual string FullName => base.FullName;
		/// <summary>
		/// The name of this ore that will display on the biome selection screen.
		/// </summary>
		public virtual LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);
		/// <summary>
		/// The description for this ore that will appear on the biome selection screen.
		/// </summary>
		public virtual LocalizedText Description => this.GetLocalization("Description", () => "");
		/// <summary>
		/// Used for adamantite ore alts.
		/// </summary>
		public virtual LocalizedText GuideHelpText => this.GetLocalization("GuideHelpText", PrettyPrintName);

		public bool includeInHardmodeDrunken = false;

		/// <summary>
		/// Override if you want to have random value whenever creating new world. Should be used just for custom tiers.
		/// </summary>
		public virtual void OnInitialize() { }
		protected sealed override void Register() {
			ModTypeLookup<AltOre>.Register(this);

			Type = AltLibrary.Ores.Count;
			AltLibrary.Ores.Add(this);
		}

		public sealed override void SetupContent() {
			SetStaticDefaults();
			if (oreItem == 0) oreItem = TileLoader.GetItemDropFromTypeAndStyle(ore);
			try {
				_ = DisplayName;
				_ = Description;
			} catch { }
		}
	}
}
