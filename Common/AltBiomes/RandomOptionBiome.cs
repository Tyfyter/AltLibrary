using AltLibrary.Core;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltBiomes {
	internal sealed class RandomOptionBiome : AltBiome {
		public override string Name => name;
		public override Color NameColor => Color.White;
		private readonly string name;
		public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.AltLibrary.AltBiomes.RandomOptionBiome.DisplayName").WithFormatArgs(Language.GetOrRegister($"Mods.AltLibrary.AltBiomeName.{BiomeType}"));
		public override LocalizedText Description => Language.GetOrRegister($"Mods.AltLibrary.AltBiomes.RandomOptionBiome.Description").WithFormatArgs(Language.GetOrRegister($"Mods.AltLibrary.AltBiomeName.{BiomeType}"));
		public RandomOptionBiome(string name, BiomeType biomeType) : base() {
			ALReflection.ModType_Mod.SetValue(this, AltLibrary.Instance);
			this.name = name;
			BiomeType = biomeType;
		}

		public override string IconSmall => "Terraria/Images/UI/WorldCreation/IconEvilRandom";

		public override bool IsLoadingEnabled(Mod mod) => false;
	}
}
