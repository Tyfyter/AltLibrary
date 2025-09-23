using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Hooks;
using AltLibrary.Core;
using Microsoft.Xna.Framework.Input;
using PegasusLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common.Systems {
	internal class RecipeChanges : ModSystem {

		readonly static Dictionary<int, RecipeGroup> findGroup = [];
		readonly static HashSet<int> handledGroups = [];
		static void AddGroup(RecipeGroup group) {
			handledGroups.Add(group.RegisteredId);
			foreach (int item in group.ValidItems) {
				findGroup[item] = group;
			}
		}
		public override void PostAddRecipes() {
			FieldInfo[] fields = typeof(RecipeGroups).GetFields();
			List<RecipeGroup> recipeGroups = fields.Select(f => f.GetValue(null) as RecipeGroup)
				.Where(v => v is not null && RecipeGroups.AppropriateMaterials.ContainsKey(v.RegisteredId)).ToList();

			ModItem critMonocle = null;

			if (ModLoader.TryGetMod("CritRework", out Mod critRework)) {
				critRework.TryFind("CritMonocle", out critMonocle);
			}
			findGroup.Clear();
			handledGroups.Clear();
			AddGroup(RecipeGroups.IronOres);
			AddGroup(RecipeGroups.GoldOres);
			AddGroup(RecipeGroups.GoldBars);
			AddGroup(RecipeGroups.EvilOres);
			AddGroup(RecipeGroups.EvilBars);
			AddGroup(RecipeGroups.AdamantiteBars);
			AddGroup(RecipeGroups.EvilSwords);
			AddGroup(RecipeGroups.RottenChunks);
			AddGroup(RecipeGroups.ShadowScales);
			AddGroup(RecipeGroups.GoldCandles);
			AddGroup(RecipeGroups.CopperWatches);
			AddGroup(RecipeGroups.SilverWatches);
			AddGroup(RecipeGroups.GoldWatches);
			MergingListDictionary<RecipePattern, Recipe> patterns = [];

			for (int i = 0; i < Recipe.numRecipes; i++) {
				Recipe recipe = Main.recipe[i];

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

				for (int j = 0; j < recipe.requiredItem.Count; j++) {
					if (findGroup.ContainsKey(recipe.requiredItem[j].type)) {
						patterns.Add(RecipePattern.FromRecipe(recipe), recipe);
						if (recipe.acceptedGroups.Any(g => RecipeGroup.recipeGroups[g].ValidItems.Count(findGroup.ContainsKey) >= 2)) {
							patterns.Add(RecipePattern.FromRecipe(recipe), recipe);
						}
						break;
					}
				}
			}
			foreach ((RecipePattern pattern, List<Recipe> recipes) in patterns) {
				if (recipes.Count < 2) continue;
				for (int i = 0; i < recipes.Count; i++) {
					Recipe recipe = recipes[i];
					if (i == 0) {
						recipe.acceptedGroups.AddRange(pattern.GetGroups());
					} else if (recipe != recipes[0]) {
						recipe.DisableRecipe();
					}
				}
			}

			for (int i = 0; i < Recipe.numRecipes; i++) {
				Recipe recipe = Main.recipe[i];
				if (!recipe.Disabled && !ShimmerDecraft.Recipes.ContainsKey(recipe.createItem.type)) {
					List<(int type, int count)> ingredients = recipe.requiredItem.Select(it => (it.type, it.stack)).ToList();
					List<(int type, int count)> ingredientGroups = new();
					for (int j = ingredients.Count - 1; j >= 0; j--) {
						(int type, int count) = ingredients[j];
						foreach (RecipeGroup group in recipeGroups) {
							if (recipe.AcceptsGroup(group.RegisteredId) && RecipeGroups.AppropriateMaterials.ContainsKey(group.RegisteredId) && group.ContainsItem(type)) {
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
								recipe
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
		public record RecipePattern(IngredientSet Ingredients, int Result) {
			public static RecipePattern FromRecipe(Recipe recipe) {
				return new RecipePattern(
					[..recipe.requiredItem.Select(item => (findGroup.TryGetValue(item.type, out RecipeGroup group) ? -group.IconicItemId : item.type, item.stack))],
					recipe.createItem.type
				);
			}
			public IEnumerable<int> GetGroups() {
				foreach ((int type, _) in Ingredients) {
					if (findGroup.TryGetValue(-type, out RecipeGroup group)) yield return group.RegisteredId;
				}
			}
		}
		public class IngredientSet() : HashSet<(int type, int stack)>() {
			public override bool Equals(object obj) => obj is IngredientSet other && SetEquals(other);
			public override int GetHashCode() {
				int hash = 0;
				foreach ((int type, int stack) item in this) hash ^= item.GetHashCode();
				return hash;
			}
		}
	}
	public class MergingListDictionary<TKey, TValue>(IDictionary<TKey, List<TValue>> innerDictionary) : IDictionary<TKey, List<TValue>> {
		public MergingListDictionary(IEqualityComparer<TKey> comparer) : this(new Dictionary<TKey, List<TValue>>(comparer)) { }
		public MergingListDictionary() : this(new Dictionary<TKey, List<TValue>>()) { }

		public void Add(TKey key, TValue value) {
			if (ContainsKey(key)) {
				innerDictionary[key].Add(value);
			} else {
				innerDictionary.Add(key, [value]);
			}
		}
		public void Add(TKey key, IEnumerable<TValue> value) {
			if (ContainsKey(key)) {
				innerDictionary[key].AddRange(value);
			} else {
				innerDictionary.Add(key, value.ToList());
			}
		}
		#region implementations
		public List<TValue> this[TKey key] { get => innerDictionary[key]; set => innerDictionary[key] = value; }
		public ICollection<TKey> Keys => innerDictionary.Keys;
		public ICollection<List<TValue>> Values => innerDictionary.Values;
		public int Count => innerDictionary.Count;
		public bool IsReadOnly => innerDictionary.IsReadOnly;
		public void Add(TKey key, List<TValue> value) {
			innerDictionary.Add(key, value);
		}
		public void Add(KeyValuePair<TKey, List<TValue>> item) {
			innerDictionary.Add(item);
		}
		public void Clear() {
			innerDictionary.Clear();
		}
		public bool Contains(KeyValuePair<TKey, List<TValue>> item) {
			return innerDictionary.Contains(item);
		}
		public bool ContainsKey(TKey key) {
			return innerDictionary.ContainsKey(key);
		}
		public void CopyTo(KeyValuePair<TKey, List<TValue>>[] array, int arrayIndex) {
			innerDictionary.CopyTo(array, arrayIndex);
		}
		public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator() {
			return innerDictionary.GetEnumerator();
		}
		public bool Remove(TKey key) {
			return innerDictionary.Remove(key);
		}
		public bool Remove(KeyValuePair<TKey, List<TValue>> item) {
			return innerDictionary.Remove(item);
		}
		public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out List<TValue> value) {
			return innerDictionary.TryGetValue(key, out value);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return innerDictionary.GetEnumerator();
		}
		#endregion implementations
	}
}
