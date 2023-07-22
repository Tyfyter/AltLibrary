using AltLibrary.Core;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltOres
{
	internal sealed class VanillaOre : AltOre
	{
		public override string Texture => oreTexture;
		public override string Name => oreTexture;

		internal readonly string oreTexture;
		private readonly string name;

		internal VanillaOre(string texture, string name, int type, int ore, int bar, OreType oreType) {
			ALReflection.ModType_Mod.SetValue(this, AltLibrary.Instance);
			oreTexture = texture;
			Type = type;
			this.name = name;
			this.ore = ore;
			this.bar = bar;
			OreType = oreType;
		}

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault(name);
			// Description.SetDefault(desc);
		}

		public override bool IsLoadingEnabled(Mod mod)
		{
			return false;
		}
	}
}
