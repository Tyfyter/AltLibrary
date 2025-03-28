﻿using AltLibrary.Core;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace AltLibrary.Common.AltOres
{
	internal sealed class RandomOptionOre : AltOre
	{
		public override Color NameColor => Color.Yellow;
		public override string Name => name;

		private readonly string name;
		private readonly string display;
		public RandomOptionOre(string name, OreType oreType, string overrideDisplay = "") : base() {
			ALReflection.ModType_Mod.SetValue(this, AltLibrary.Instance);
			this.name = name;
			display = overrideDisplay;
			OreType = oreType;
		}

		public override bool IsLoadingEnabled(Mod mod) => false;
	}
}
