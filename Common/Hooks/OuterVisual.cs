#pragma warning disable CS0649
using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PegasusLib;
using PegasusLib.Reflection;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace AltLibrary.Common.Hooks {
	internal class ProgressBarMethods : ReflectionLoader {
		public static FastFieldInfo<UIGenProgressBar, float> _targetOverallProgress;
		public static FastFieldInfo<UIGenProgressBar, float> _targetCurrentProgress;
		public static FastFieldInfo<UIGenProgressBar, int> _longBarWidth;
		public static FastFieldInfo<UIGenProgressBar, int> _smallBarWidth;
		[ReflectionParentType(typeof(UIGenProgressBar))]
		static Action<SpriteBatch, Texture2D, Texture2D, Vector2, int, int, Color, Color> DrawFilling;
		[ReflectionParentType(typeof(UIGenProgressBar))]
		static Action<SpriteBatch, Vector2, int, int, int, Color, Color, Color> DrawFilling2;
		public static void DoDrawFilling(UIGenProgressBar self, SpriteBatch spritebatch, Texture2D tex, Texture2D texShadow, Vector2 topLeft, int completedWidth, int totalWidth, Color separator, Color empty) {
			PegasusLib.Reflection.DelegateMethods._target.SetValue(DrawFilling, self);
			DrawFilling(spritebatch, tex, texShadow, topLeft, completedWidth, totalWidth, separator, empty);
		}
		public static void DoDrawFilling2(UIGenProgressBar self, SpriteBatch spritebatch, Vector2 topLeft, int height, int completedWidth, int totalWidth, Color filled, Color separator, Color empty) {
			PegasusLib.Reflection.DelegateMethods._target.SetValue(DrawFilling2, self);
			DrawFilling2(spritebatch, topLeft, height, completedWidth, totalWidth, filled, separator, empty);
		}
	}
	internal class OuterVisual : ILoadable {
		List<AltBiome> biomes;
		public void Load(Mod mod) {
			On_UIGenProgressBar.DrawSelf += On_UIGenProgressBar_DrawSelf;
		}
		private void On_UIGenProgressBar_DrawSelf(On_UIGenProgressBar.orig_DrawSelf orig, UIGenProgressBar self, SpriteBatch spriteBatch) {
			AltBiome hell = WorldBiomeManager.GetWorldHell();
			Asset<Texture2D> lowerTexture = hell.LowerTextureAsset;
			if (lowerTexture.IsLoaded) {
				AltBiome biome;
				if (WorldGen.drunkWorldGen) {
					biomes ??= [..((IEnumerable<AltBiome>)AltLibrary.AllBiomes).Where(biome => biome.BiomeType == BiomeType.Evil)];
					biome = Main.rand.Next(biomes);
				} else {
					biome = WorldBiomeManager.GetWorldEvil();
				}
				CalculatedStyle dimensions = self.GetDimensions();
				Vector2 pos = dimensions.Position();
				Color color = biome.OuterColor;
				ProgressBarMethods.DoDrawFilling2(self, spriteBatch,
					pos + new Vector2(20f, 40f),
					16,
					(int)(ProgressBarMethods._targetOverallProgress.GetValue(self) * ProgressBarMethods._longBarWidth.GetValue(self)),
					ProgressBarMethods._longBarWidth.GetValue(self),
					color,
					Color.Lerp(color, Color.Black, 0.5f),
					new Color(48, 48, 48)
				);
				color = hell.LowerColor;
				ProgressBarMethods.DoDrawFilling2(self, spriteBatch,
					pos + new Vector2(50f, 60f),
					8,
					(int)(ProgressBarMethods._targetCurrentProgress.GetValue(self) * ProgressBarMethods._smallBarWidth.GetValue(self)),
					ProgressBarMethods._smallBarWidth.GetValue(self),
					color,
					Color.Lerp(color, Color.Black, 0.5f),
					new Color(33, 33, 33)
				);
				Rectangle r = dimensions.ToRectangle();
				r.X -= 8;
				spriteBatch.Draw(biome.OuterTextureAsset.Value, r.TopLeft(), Color.White);
				spriteBatch.Draw(lowerTexture.Value, r.TopLeft() + new Vector2(44f, 60f), Color.White);
			}
		}
		public void Unload() { }
	}
}
