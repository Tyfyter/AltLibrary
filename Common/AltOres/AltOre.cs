using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
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
			try {
				_ = DisplayName;
				_ = Description;
			} catch { }
		}
		internal void VerifyRequiredItems() {
			if (oreItem == 0) oreItem = TileLoader.GetItemDropFromTypeAndStyle(ore);
			if (oreItem == 0) {
				for (int i = 0; i < ItemLoader.ItemCount; i++) {
					try {
						if (!ItemID.Sets.DisableAutomaticPlaceableDrop[i] && new Item(i).createTile == ore) {
							oreItem = i;
							break;
						}
					} catch (Exception) { }
				}
			}
			if (AnyUnset(out string[] unset, (nameof(ore), ore), (nameof(oreItem), oreItem), (nameof(bar), bar))) {
				string message = $"{Name}: fields [{string.Join(", ", unset)}] were unset, these values must be set in SetStaticDefaults";
				if (Directory.Exists(Path.Combine(Program.SavePathShared, "ModSources", Mod.Name))) {
					throw new NotImplementedException(message);
				} else {
					Mod.Logger.Error(message);
				}
			}
		}
		static bool AnyUnset(out string[] unset, params Span<(string name, int value)> values) {
			int count = 0;
			for (int i = 0; i < values.Length; i++) {
				if (values[i].value == 0) count++;
			}
			unset = new string[count];
			int j = 0;
			for (int i = 0; j < count; i++) {
				if (values[i].value == 0) unset[j++] = values[i].name;
			}
			return count > 0;
		}
	}
}
