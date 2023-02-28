﻿using AltLibrary.Common.AltTypes;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;

namespace AltLibrary.Common.OrderGroups;

public abstract class OreGroup : AOrderGroup<OreGroup, IAltOre>, IStaticOrderGroup {
	public sealed override string Texture => GetTexture();

	public static Rectangle? GetSourceRectangle() => new(60, 0, 30, 30);
	public static string GetTexture() => DefaultTexture;
	public static Color GetColor() => new(143, 183, 183);

	public override string LocalizationCategory => "OreGroup";

	private protected override Type GetMainSubclass() => typeof(AltOre<>);
}
