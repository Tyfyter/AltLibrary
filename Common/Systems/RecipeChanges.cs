using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Hooks;
using AltLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Systems {
	internal class RecipeChanges : ModSystem {
		public override void PostAddRecipes() {
			FieldInfo[] fields = typeof(RecipeGroups).GetFields();
			List<RecipeGroup> recipeGroups = fields.Select(f => f.GetValue(null) as RecipeGroup)
				.Where(v => v is not null && RecipeGroups.AppropriateMaterials.ContainsKey(v.RegisteredId)).ToList();

			for (int i = 0; i < Recipe.numRecipes; i++) {
				Recipe recipe = Main.recipe[i];

				ReplaceRecipe(ref recipe,
								  [ItemID.IronskinPotion],
								  [ItemID.IronOre],
								  "IronOres",
								  ItemID.LeadOre);
				ReplaceRecipe(ref recipe,
								  [ItemID.SpelunkerPotion],
								  [ItemID.GoldOre],
								  "GoldOres",
								  ItemID.PlatinumOre);

				ReplaceRecipe(ref recipe,
								  [ItemID.DeerThing],
								  [ItemID.DemoniteOre],
								  "EvilOres",
								  ItemID.CrimtaneOre);
				ReplaceRecipe(ref recipe,
								  [ItemID.Magiluminescence, ItemID.ShadowCandle],
								  [ItemID.DemoniteBar],
								  "EvilBars",
								  ItemID.CrimtaneBar);
				ReplaceRecipe(ref recipe,
								  [ItemID.OpticStaff, ItemID.PumpkinMoonMedallion],
								  [ItemID.HallowedBar],
								  "HallowBars");
				ReplaceRecipe(ref recipe,
								  [ItemID.DrillContainmentUnit],
								  [ItemID.HellstoneBar],
								  "HellBars");
				ReplaceRecipe(ref recipe,
								  [ItemID.DrillContainmentUnit, ItemID.VenomStaff, ItemID.TrueExcalibur],
								  [ItemID.ChlorophyteBar],
								  "JungleBars");
				ReplaceRecipe(ref recipe,
								  [ItemID.DrillContainmentUnit, ItemID.MiniNukeI, ItemID.MiniNukeII],
								  [ItemID.ShroomiteBar],
								  "MushroomBars");
				ReplaceRecipe(ref recipe,
								  [ItemID.PeaceCandle, ItemID.Throne, ItemID.FlinxFurCoat, ItemID.FlinxStaff],
								  [ItemID.GoldBar],
								  "GoldBars",
								  ItemID.PlatinumBar);
				ReplaceRecipe(ref recipe,
								  [ItemID.FrostBreastplate, ItemID.FrostLeggings, ItemID.FrostHelmet, ItemID.AncientBattleArmorHat, ItemID.AncientBattleArmorPants, ItemID.AncientBattleArmorShirt],
								  [ItemID.AdamantiteBar],
								  "AdamantiteBars",
								  ItemID.TitaniumBar);

				ReplaceRecipe(ref recipe,
								  [ItemID.NightsEdge],
								  [ItemID.LightsBane],
								  "EvilSwords",
								  ItemID.BloodButcherer);
				ReplaceRecipe(ref recipe,
								  [ItemID.NightsEdge],
								  [ItemID.BladeofGrass],
								  "JungleSwords");
				ReplaceRecipe(ref recipe,
								  [ItemID.NightsEdge],
								  [ItemID.FieryGreatsword],
								  "HellSwords");
				ReplaceRecipe(ref recipe,
								  [ItemID.TerraBlade],
								  [ItemID.TrueNightsEdge],
								  "TrueComboSwords");
				ReplaceRecipe(ref recipe,
								  [ItemID.TerraBlade],
								  [ItemID.TrueExcalibur],
								  "TrueHallowSwords");

				ReplaceRecipe(ref recipe,
								  [ItemID.MonsterLasagna, ItemID.CoffinMinecart, ItemID.MechanicalWorm, ItemID.BattlePotion],
								  [ItemID.RottenChunk],
								  "RottenChunks",
								  ItemID.Vertebrae);
				ReplaceRecipe(ref recipe,
								  [ItemID.MeteorStaff, ItemID.GreaterHealingPotion],
								  [ItemID.PixieDust],
								  "PixieDusts");
				ReplaceRecipe(ref recipe,
								  [ItemID.SuperManaPotion],
								  [ItemID.UnicornHorn],
								  "UnicornHorns");
				ReplaceRecipe(ref recipe,
								  [ItemID.SuperManaPotion, ItemID.GreaterHealingPotion, ItemID.BluePhasesaber, ItemID.GreenPhasesaber, ItemID.PurplePhasesaber, ItemID.RedPhasesaber, ItemID.WhitePhasesaber, ItemID.YellowPhasesaber, ItemID.OrangePhasesaber],
								  [ItemID.CrystalShard],
								  "CrystalShards");
				ReplaceRecipe(ref recipe,
								  [ItemID.VoidLens, ItemID.VoidVault, ItemID.ObsidianHelm, ItemID.ObsidianShirt, ItemID.ObsidianPants],
								  [ItemID.ShadowScale],
								  "ShadowScales",
								  ItemID.TissueSample);

				ReplaceRecipe(ref recipe,
								  [ItemID.VoidLens, ItemID.VoidVault],
								  [ItemID.JungleSpores],
								  "JungleSpores");
				ReplaceRecipe(ref recipe,
								  [ItemID.GarlandHat, ItemID.GenderChangePotion, ItemID.BattlePotion, ItemID.GravitationPotion, ItemID.MagicPowerPotion, ItemID.StinkPotion, ItemID.TitanPotion],
								  [ItemID.Deathweed],
								  "Deathweed");
				ReplaceRecipe(ref recipe,
								  [ItemID.GarlandHat, ItemID.GenderChangePotion, ItemID.GravitationPotion, ItemID.TeleportationPotion],
								  [ItemID.Fireblossom],
								  "Fireblossom");
				ReplaceRecipe(ref recipe,
								  [ItemID.GarlandHat, ItemID.GenderChangePotion, ItemID.BuilderPotion, ItemID.CratePotion, ItemID.LifeforcePotion, ItemID.SpelunkerPotion],
								  [ItemID.JungleSpores],
								  "JungleSpores");

				ReplaceRecipe(ref recipe,
								  [ItemID.AdamantiteForge, ItemID.TitaniumForge],
								  [ItemID.Hellforge],
								  "Hellforges");

				ReplaceRecipe(ref recipe,
								  [ItemID.WaterCandle],
								  [ItemID.Candle],
								  "GoldCandles",
								  ItemID.PlatinumCandle);
				ReplaceRecipe(ref recipe,
								  [ItemID.Timer5Second],
								  [ItemID.CopperWatch],
								  "CopperWatches",
								  ItemID.TinWatch);
				ReplaceRecipe(ref recipe,
								  [ItemID.Timer3Second],
								  [ItemID.SilverWatch],
								  "SilverWatches",
								  ItemID.TungstenWatch);
				ReplaceRecipe(ref recipe,
								  [ItemID.Timer1Second],
								  [ItemID.GoldWatch],
								  "GoldWatches",
								  ItemID.PlatinumWatch);
				if (!recipe.Disabled && !ShimmerDecraft.Recipes.ContainsKey(recipe.createItem.type)) {
					List<(int type, int count)> ingredients = recipe.requiredItem.Select(it => (it.type, it.stack)).ToList();
					List<(int type, int count)> ingredientGroups = new();
					for (int j = ingredients.Count - 1; j >= 0; j--) {
						(int type, int count) = ingredients[j];
						foreach (var group in recipeGroups) {
							if (recipe.AcceptsGroup(group.RegisteredId) && group.ContainsItem(type)) {
								ingredientGroups.Add((group.RegisteredId, count));
								ingredients.RemoveAt(j);
								break;
							}
						}
					}
					if (ingredientGroups.Count > 0) {
						ShimmerDecraft.Recipes.Add(
							recipe.createItem.type,
							(
							recipe.createItem.stack,
								ingredients.Select<(int type, int count), (Func<int>, int)>(ing => (() => ing.type, ing.count)).Union(
								ingredientGroups.Select<(int type, int count), (Func<int>, int)>(g => (RecipeGroups.AppropriateMaterials[g.type], g.count)))

								.ToList(),
								ALReflection.Recipe_alchemy.GetValue(recipe)
							)
						);
					}
				}
			}

		}
		private static void ReplaceRecipe(ref Recipe r, int[] results, int[] ingredients, string group) {
			foreach (int result in results) {
				if (r.HasResult(result)) {
					foreach (int ingredient in ingredients) {
						if (r.HasIngredient(ingredient)) {
							r.TryGetIngredient(ingredient, out Item ing);
							if (ing == null)
								continue;
							r.RemoveIngredient(ing);
							r.AddRecipeGroup(group, ing.stack);
						}
					}
				}
			}
		}

		private static void ReplaceRecipe(ref Recipe r, int[] results, int[] ingredients, string group, int altIng) {
			foreach (int result in results) {
				if (r.HasResult(result)) {
					foreach (int ingredient in ingredients) {
						if (r.HasIngredient(altIng)) {
							r.DisableRecipe();
						} else if (r.HasIngredient(ingredient)) {
							r.TryGetIngredient(ingredient, out Item ing);
							if (ing == null)
								continue;
							r.RemoveIngredient(ing);
							r.AddRecipeGroup(group, ing.stack);
						}
					}
				}
			}
		}
	}
}
