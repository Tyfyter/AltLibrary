using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks
{
	public class ShimmerDecraft {
		public static Dictionary<int, (int createCount, List<(Func<int> type, int count)> ingredients, bool alchemy)> recipes;
		internal static void Load() {
			recipes = new();
			On_Item.CanShimmer += On_Item_CanShimmer;
			On_Item.GetShimmered += On_Item_GetShimmered;
		}

		private static bool On_Item_CanShimmer(On_Item.orig_CanShimmer orig, Item self) {
			if (recipes.ContainsKey(self.type)) return true;
			return orig(self);
		}
		private static void On_Item_GetShimmered(On_Item.orig_GetShimmered orig, Item self) {
			if (recipes.TryGetValue(self.type, out var recipe)) {
				int decraftAmount = self.stack / recipe.createCount;
				bool spread = recipe.ingredients.Count > 1;
				int i = 0;
				foreach ((Func<int> ingredientType, int stack) in recipe.ingredients) {
					int itemType = ingredientType();
					if (itemType <= 0) {
						break;
					}
					i++;
					int totalDecrafted = decraftAmount * stack;
					if (recipe.alchemy) {
						for (int num9 = totalDecrafted; num9 > 0; num9--) {
							if (Main.rand.NextBool(3)) {
								totalDecrafted--;
							}
						}
					}
					while (totalDecrafted > 0) {
						int stackDecrafted = totalDecrafted;
						if (stackDecrafted > 9999) {
							stackDecrafted = 9999;
						}
						totalDecrafted -= stackDecrafted;
						Item newItem = Main.item[Item.NewItem(self.GetSource_Misc("Shimmer"), self.position, self.width, self.height, itemType)];
						newItem.stack = stackDecrafted;
						newItem.shimmerTime = 1f;
						newItem.shimmered = true;
						newItem.shimmerWet = true;
						newItem.wet = true;
						newItem.velocity *= 0.1f;
						newItem.playerIndexTheItemIsReservedFor = Main.myPlayer;
						if (spread) {
							newItem.velocity.X = 1f * i;
							newItem.velocity.X *= 1f + i * 0.05f;
							if (i % 2 == 0) {
								newItem.velocity.X *= -1f;
							}
						}
						NetMessage.SendData(145, -1, -1, null, newItem.whoAmI, 1f);
					}
				}
				self.stack -= decraftAmount * recipe.createCount;
				if (self.stack <= 0) {
					self.TurnToAir();
				}
			} else {
				orig(self);
			}
		}

		internal static void Unload() {
			recipes = null;
		}
	}
}
