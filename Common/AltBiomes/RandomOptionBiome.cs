using AltLibrary.Core;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltBiomes
{
	internal sealed class RandomOptionBiome : AltBiome
	{
		public override string Name => name;
		public override Color NameColor => Color.Yellow;

		private readonly string name;
		public RandomOptionBiome(string name, BiomeType biomeType) : base()
		{
			ALReflection.ModType_Mod.SetValue(this, AltLibrary.Instance);
			this.name = name;
			BiomeType = biomeType;
		}

		public override string IconSmall => "Terraria/Images/UI/Bestiary/Icon_Tags_Shadow";

		public override bool IsLoadingEnabled(Mod mod) => false;
	}
}
