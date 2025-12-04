using AltLibrary.Core;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks
{
	public class ShimmerDecraft {
		public static Dictionary<int, (int createCount, List<(Func<int> type, int count)> ingredients, Recipe recipe)> Recipes { get; private set; } = [];
		internal static void Load() {
			On_Item.CanShimmer += On_Item_CanShimmer;
			On_Item.GetShimmered += On_Item_GetShimmered;
		}

		private static bool On_Item_CanShimmer(On_Item.orig_CanShimmer orig, Item self) {
			if (Recipes.ContainsKey(self.type)) return true;
			return orig(self);
		}
		private static void On_Item_GetShimmered(On_Item.orig_GetShimmered orig, Item self) {
			if (Recipes.TryGetValue(self.type, out (int createCount, List<(Func<int> type, int count)> ingredients, Recipe recipe) recipe)) {
				int decraftAmount = self.stack / recipe.createCount;
				bool spread = recipe.ingredients.Count > 1;
				int num = 0;
				for (int i = 0; i < recipe.ingredients.Count; i++) {
					(Func<int> ingredientType, int stack) = recipe.ingredients[i];
					int itemType = ingredientType();
					if (itemType <= 0) {
						break;
					}
					num++;
					int totalDecrafted = decraftAmount * stack;
					if (recipe.recipe.ConsumeIngredientHooks() is Recipe.IngredientQuantityCallback ConsumeIngredientHooks) {
						ConsumeIngredientHooks(recipe.recipe, itemType, ref totalDecrafted, true);
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
							newItem.velocity.X = 1f * num;
							newItem.velocity.X *= 1f + num * 0.05f;
							if (num % 2 == 0) {
								newItem.velocity.X *= -1f;
							}
						}
						NetMessage.SendData(MessageID.SyncItemsWithShimmer, -1, -1, null, newItem.whoAmI, 1f);
					}
				}
				self.stack -= decraftAmount * recipe.createCount;
				if (self.stack <= 0) {
					self.TurnToAir();
					self.shimmerTime = 0f;
				} else {
					self.shimmerTime = 1f;
				}
				self.shimmerWet = true;
				self.wet = true;
				self.velocity *= 0.1f;
				if (Main.netMode == NetmodeID.SinglePlayer) {
					Item.ShimmerEffect(self.Center);
				} else {
					NetMessage.SendData(MessageID.ShimmerActions, -1, -1, null, 0, (int)self.Center.X, (int)self.Center.Y);
					NetMessage.SendData(MessageID.SyncItemsWithShimmer, -1, -1, null, self.whoAmI, 1f);
				}
				AchievementsHelper.NotifyProgressionEvent(27);
			} else {
				orig(self);
			}
		}

		internal static void Unload() {
			Recipes = null;
		}
	}
}
