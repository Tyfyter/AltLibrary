﻿using Microsoft.Xna.Framework;
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
		public OreType OreType = OreType.Copper;

		public int? Candle = null;
		public int? Watch = null;
		public bool IncludeInExtractinator = false;

		public int Type { get; internal set; }
		public string LocalizationCategory => "AltOres";

		/// <summary>
		/// The name of this ore that will display on the biome selection screen.
		/// </summary>
		public LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);
		/// <summary>
		/// The description for this ore that will appear on the biome selection screen.
		/// </summary>
		public LocalizedText Description => this.GetLocalization("Description", PrettyPrintName);
		/// <summary>
		/// The 'World was blessed by ...' message. Used for hardmode ore alts.
		/// </summary>
		public LocalizedText BlessingMessage => this.GetLocalization("BlessingMessage", PrettyPrintName);
		/// <summary>
		/// Used for adamantite ore alts.
		/// </summary>
		public LocalizedText GuideHelpText => this.GetLocalization("GuideHelpText", PrettyPrintName);

		public bool includeInHardmodeDrunken = false;

		/// <summary>
		/// Override if you want custom selection
		/// </summary>
		/// <param name="list"></param>
		public virtual void CustomSelection(List<AltOre> list) {
			int index = list.FindLastIndex(x => x.OreType == OreType);
			if (index != -1) {
				list.Insert(index + 1, this);
			}
		}

		/// <summary>
		/// Override if you want to have random value whenever creating new world. Should be used just for custom tiers.
		/// </summary>
		public virtual void OnInitialize() { }

		/// <summary>
		/// If you want custom action on click, then use this. Useful for "RandomX" options and custom tiers.
		/// <br/>By default: false.
		/// <br/>Set to true if you want to override default behavior.
		/// </summary>
		public virtual bool OnClick() => false;

		public virtual void OnCreating() { }

		public virtual void AddOreOnScreenIcon(List<ALDrawingStruct<AltOre>> list) {
		}

		protected sealed override void Register() {
			ModTypeLookup<AltOre>.Register(this);

			AltLibrary.Ores.Add(this);
			Type = AltLibrary.Ores.Count;
		}

		public sealed override void SetupContent() {
			SetStaticDefaults();
		}
	}
}
