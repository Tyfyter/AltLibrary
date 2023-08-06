using AltLibrary.Common;
using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.AltOres;
using AltLibrary.Common.Hooks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static AltLibrary.Common.UIWorldCreationEdits;

namespace AltLibrary.Core.Baking {
	internal class ALWorldCreationLists {
		internal static OreCreationList prehmOreData;
		internal static BiomeCreationList biomeData;

		internal class ALWorldCreationLists_Loader : ILoadable {
			public void Load(Mod mod) {
				prehmOreData = new();
				biomeData = new();

				List<int> ores = new()
				{
					ItemID.CopperOre,
					ItemID.TinOre,

					ItemID.IronOre,
					ItemID.LeadOre,

					ItemID.SilverOre,
					ItemID.TungstenOre,

					ItemID.GoldOre,
					ItemID.PlatinumOre
				};

				foreach (var ore in AltLibrary.Ores.Where(x => x.OreType < OreType.Cobalt)) {
					ores.Add(TileLoader.GetItemDropFromTypeAndStyle(ore.ore));
				}

				AltOreInsideBodies.ores = ores;
			}

			public void Unload() {
				prehmOreData = null;
				biomeData = null;

				AltOreInsideBodies.ores = null;
			}
		}

		public static void FillData() {
			prehmOreData.Initialize();
			biomeData.Initialize();
		}

		internal class BiomeCreationList : WorldCreationList<AltBiome> {
			internal List<ALDrawingStruct<AltBiome>> Quenes;

			public override void Initialize() {
				List<ALDrawingStruct<AltBiome>> quene = new();
				#region Evil
				quene.Add(new("Terraria/Evil", (int)BiomeType.Evil, (value) => {
					if (AltEvilBiomeChosenType >= 0 && AltLibraryConfig.Config.BiomeIconsVisible) {
						return ModContent.Request<Texture2D>(AltLibrary.Biomes[AltEvilBiomeChosenType].IconSmall
							?? "AltLibrary/Assets/Menu/ButtonCorrupt", AssetRequestMode.ImmediateLoad);
					}
					return value;
				}, () => {
					if (!AltLibraryConfig.Config.BiomeIconsVisible) return new(210, 0, 30, 30);
					return AltEvilBiomeChosenType switch {
						-333 => new(210, 0, 30, 30),
						-666 => new(360, 0, 30, 30),
						_ => null
					};
				}, () => {
					if (!AltLibraryConfig.Config.BiomeIconsVisible) return Language.GetTextValue("Mods.AltLibrary.AltBiomeName.RandomEvilBiome");
					if (AltEvilBiomeChosenType == -333) return Language.GetTextValue("Mods.AltLibrary.AltBiomeName.CorruptBiome");
					if (AltEvilBiomeChosenType == -666) return Language.GetTextValue("Mods.AltLibrary.AltBiomeName.CrimsonBiome");
					return AltLibrary.Biomes[AltEvilBiomeChosenType].Name;
				}, (mod) => {
					if (!AltLibraryConfig.Config.BiomeIconsVisible) return Language.GetTextValue("Mods.AltLibrary.BiomeOrOreModSecret");
					return AltEvilBiomeChosenType switch {
						>= 0 => AltLibrary.Biomes[AltEvilBiomeChosenType].Mod.Name,
						_ => mod
					};
				}));
				#endregion Evil
				#region Hallow
				if (AltLibrary.Biomes.Any(x => x.BiomeType == BiomeType.Hallow && x.Selectable))
					quene.Add(new("Terraria/Hallow", (int)BiomeType.Hallow, (value) => {
						if (AltHallowBiomeChosenType >= 0 && AltLibraryConfig.Config.BiomeIconsVisible) {
							return ModContent.Request<Texture2D>(AltLibrary.Biomes[AltHallowBiomeChosenType].IconSmall
								?? "AltLibrary/Assets/Menu/ButtonHallow", AssetRequestMode.ImmediateLoad);
						}
						return value;
					},
					() => {
						if (!AltLibraryConfig.Config.BiomeIconsVisible) return new(30, 30, 30, 30);
						return AltHallowBiomeChosenType switch {
							< 0 => new(30, 30, 30, 30),
							_ => null
						};
					},
					() => {
						if (!AltLibraryConfig.Config.BiomeIconsVisible) return Language.GetTextValue("Mods.AltLibrary.AltBiomeName.RandomHallowBiome");
						return AltHallowBiomeChosenType < 0
							? Language.GetTextValue("Mods.AltLibrary.AltBiomeName.HallowBiome")
							: AltLibrary.Biomes[AltHallowBiomeChosenType].Name;
					},
					(mod) => {
						if (!AltLibraryConfig.Config.BiomeIconsVisible) return Language.GetTextValue("Mods.AltLibrary.BiomeOrOreModSecret");
						return AltHallowBiomeChosenType switch {
							>= 0 => AltLibrary.Biomes[AltHallowBiomeChosenType].Mod.Name,
							_ => mod
						};
					}));
				#endregion
				#region Jungle
				if (AltLibrary.Biomes.Any(x => x.BiomeType == BiomeType.Jungle && x.Selectable))
					quene.Add(new("Terraria/Jungle", (int)BiomeType.Jungle,
					(value) => {
						if (AltJungleBiomeChosenType >= 0 && AltLibraryConfig.Config.BiomeIconsVisible) {
							return ModContent.Request<Texture2D>(AltLibrary.Biomes[AltJungleBiomeChosenType].IconSmall
								?? "AltLibrary/Assets/Menu/ButtonJungle", AssetRequestMode.ImmediateLoad);
						}
						return value;
					}, () => {
						if (!AltLibraryConfig.Config.BiomeIconsVisible) return new(180, 30, 30, 30);
						return AltJungleBiomeChosenType switch {
							< 0 => new(180, 30, 30, 30),
							_ => null
						};
					}, () => {
						if (!AltLibraryConfig.Config.BiomeIconsVisible) return Language.GetTextValue("Mods.AltLibrary.AltBiomeName.RandomJungleBiome");
						return AltJungleBiomeChosenType < 0
								? Language.GetTextValue("Mods.AltLibrary.AltBiomeName.JungleBiome")
								: AltLibrary.Biomes[AltJungleBiomeChosenType].Name;
					}, (mod) => {
						if (!AltLibraryConfig.Config.BiomeIconsVisible) return Language.GetTextValue("Mods.AltLibrary.BiomeOrOreModSecret");
						return AltJungleBiomeChosenType switch {
							>= 0 => AltLibrary.Biomes[AltJungleBiomeChosenType].Mod.Name,
							_ => mod
						};
					}));
				#endregion
				#region Underworld
				if (AltLibrary.Biomes.Any(x => x.BiomeType == BiomeType.Hell && x.Selectable))
					quene.Add(new("Terraria/Underworld", (int)BiomeType.Hell,
					(value) => {
						if (AltHellBiomeChosenType >= 0 && AltLibraryConfig.Config.BiomeIconsVisible) {
							return ModContent.Request<Texture2D>(AltLibrary.Biomes[AltHellBiomeChosenType].IconSmall
								?? "AltLibrary/Assets/Menu/ButtonHell", AssetRequestMode.ImmediateLoad);
						}
						return value;
					}, () => {
						if (!AltLibraryConfig.Config.BiomeIconsVisible) return new(30, 60, 30, 30);
						return AltHellBiomeChosenType switch {
							< 0 => new(30, 60, 30, 30),
							_ => null
						};
					}, () => {
						if (!AltLibraryConfig.Config.BiomeIconsVisible) return Language.GetTextValue("Mods.AltLibrary.AltBiomeName.RandomUnderworldBiome");
						return AltHellBiomeChosenType < 0
								? Language.GetTextValue("Mods.AltLibrary.AltBiomeName.UnderworldBiome")
								: AltLibrary.Biomes[AltHellBiomeChosenType].Name;
					}, (mod) => {
						if (!AltLibraryConfig.Config.BiomeIconsVisible) return Language.GetTextValue("Mods.AltLibrary.BiomeOrOreModSecret");
						return AltHellBiomeChosenType switch {
							>= 0 => AltLibrary.Biomes[AltHellBiomeChosenType].Mod.Name,
							_ => mod
						};
					}));
				#endregion
				foreach (AltBiome ore in AltLibrary.Biomes) {
					ore.AddBiomeOnScreenIcon(quene);
				}
				Quenes = quene;
			}
		}

		internal class OreCreationList : WorldCreationList<AltOre> {
			internal List<ALDrawingStruct<AltOre>> Quenes;

			public override void Initialize() {
				List<AltOre> preOrder = new()
			{
					new VanillaOre("Copper", "Copper", -1, TileID.Copper, ItemID.CopperBar, OreType.Copper),
					new VanillaOre("Tin", "Tin", -2, TileID.Tin, ItemID.TinBar, OreType.Copper),
					new VanillaOre("Iron", "Iron", -3, TileID.Iron, ItemID.IronBar, OreType.Iron),
					new VanillaOre("Lead", "Lead", -4, TileID.Lead, ItemID.LeadBar, OreType.Iron),
					new VanillaOre("Silver", "Silver", -5, TileID.Silver, ItemID.SilverBar, OreType.Silver),
					new VanillaOre("Tungsten", "Tungsten", -6, TileID.Tungsten, ItemID.TungstenBar, OreType.Silver),
					new VanillaOre("Gold", "Gold", -7, TileID.Gold, ItemID.GoldBar, OreType.Gold),
					new VanillaOre("Platinum", "Platinum", -8, TileID.Platinum, ItemID.PlatinumBar, OreType.Gold),
					new VanillaOre("Cobalt", "Cobalt", -9, TileID.Cobalt, ItemID.CobaltBar, OreType.Cobalt),
					new VanillaOre("Palladium", "Palladium", -10, TileID.Palladium, ItemID.PalladiumBar, OreType.Cobalt),
					new VanillaOre("Mythril", "Mythril", -11, TileID.Mythril, ItemID.MythrilBar, OreType.Mythril),
					new VanillaOre("Orichalcum", "Orichalcum", -12, TileID.Orichalcum, ItemID.OrichalcumBar, OreType.Mythril),
					new VanillaOre("Adamantite", "Adamantite", -13, TileID.Adamantite, ItemID.AdamantiteBar, OreType.Adamantite),
					new VanillaOre("Titanium", "Titanium", -14, TileID.Titanium, ItemID.TitaniumBar, OreType.Adamantite)
				};
				foreach (AltOre ore in AltLibrary.Ores) {
					ore.CustomSelection(preOrder);
				}
				Types = preOrder;

				List<ALDrawingStruct<AltOre>> quene = new()  {
				#region Pre-HM Ores
				#region Copper
					new("Terraria/Copper", (int)OreType.Copper,
					(value) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return value;
						return Copper switch {
							>= 0 => ModContent.Request<Texture2D>(AltLibrary.Ores[Copper - 1].Texture),
							_ => value
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return new(0, 0, 30, 30);
						return Copper switch {
							-1 => new(0, 0, 30, 30),
							-2 => new(30, 0, 30, 30),
							_ => null,
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.AltOreName.RandomCopper");
						return Copper switch {
							-1 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Copper"),
							-2 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Tin"),
							_ => AltLibrary.Ores[Copper - 1].Name
						};
					}, (mod) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.BiomeOrOreModSecret");
						return Copper switch {
							>= 0 => AltLibrary.Ores[Copper - 1].Mod.Name,
							_ => mod
						};
					}),
				#endregion

				#region Iron
					new("Terraria/Iron", (int)OreType.Iron,
					(value) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return value;
						return Iron switch {
							>= 0 => ModContent.Request<Texture2D>(AltLibrary.Ores[Iron - 1].Texture),
							_ => value
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return new(60, 0, 30, 30);
						return Iron switch {
							-3 => new(60, 0, 30, 30),
							-4 => new(90, 0, 30, 30),
							_ => null,
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.AltOreName.RandomIron");
						return Iron switch {
							-3 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Iron"),
							-4 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Lead"),
							_ => AltLibrary.Ores[Iron - 1].Name
						};
					}, (mod) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.BiomeOrOreModSecret");
						return Iron switch {
							>= 0 => AltLibrary.Ores[Iron - 1].Mod.Name,
							_ => mod
						};
					}),
				#endregion

				#region Silver
					new("Terraria/Silver", (int)OreType.Silver,
					(value) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return value;
						return Silver switch {
							>= 0 => ModContent.Request<Texture2D>(AltLibrary.Ores[Silver - 1].Texture),
							_ => value
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return new(120, 0, 30, 30);
						return Silver switch {
							-5 => new(120, 0, 30, 30),
							-6 => new(150, 0, 30, 30),
							_ => null,
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.AltOreName.RandomSilver");
						return Silver switch {
							-5 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Silver"),
							-6 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Tungsten"),
							_ => AltLibrary.Ores[Silver - 1].Name
						};
					}, (mod) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.BiomeOrOreModSecret");
						return Silver switch {
							>= 0 => AltLibrary.Ores[Silver - 1].Mod.Name,
							_ => mod
						};
					}),
				#endregion

				#region Gold
					new("Terraria/Gold", (int)OreType.Gold,
					(value) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return value;
						return Gold switch {
							>= 0 => ModContent.Request<Texture2D>(AltLibrary.Ores[Gold - 1].Texture),
							_ => value
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return new(180, 0, 30, 30);
						return Gold switch {
							-7 => new(180, 0, 30, 30),
							-8 => new(210, 0, 30, 30),
							_ => null,
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.AltOreName.RandomGold");
						return Gold switch {
							-7 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Gold"),
							-8 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Platinum"),
							_ => AltLibrary.Ores[Gold - 1].Name
						};
					}, (mod) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.BiomeOrOreModSecret");
						return Gold switch {
							>= 0 => AltLibrary.Ores[Gold - 1].Mod.Name,
							_ => mod
						};
					}),
				#endregion
				#endregion

				#region HM Ores
				#region Cobalt
					new("Terraria/Cobalt", (int)OreType.Cobalt,
					(value) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return value;
						return Cobalt switch {
							>= 0 => ModContent.Request<Texture2D>(AltLibrary.Ores[Cobalt - 1].Texture),
							_ => value
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return new(0, 30, 30, 30);
						return Cobalt switch {
							-9 => new(0, 30, 30, 30),
							-10 => new(30, 30, 30, 30),
							_ => null,
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.AltOreName.RandomCobalt");
						return Cobalt switch {
							-9 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Cobalt"),
							-10 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Palladium"),
							_ => AltLibrary.Ores[Cobalt - 1].Name
						};
					}, (mod) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.BiomeOrOreModSecret");
						return Cobalt switch {
							>= 0 => AltLibrary.Ores[Cobalt - 1].Mod.Name,
							_ => mod
						};
					}),
				#endregion

				#region Mythril
					new("Terraria/Mythril", (int)OreType.Mythril,
					(value) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return value;
						return Mythril switch {
							>= 0 => ModContent.Request<Texture2D>(AltLibrary.Ores[Mythril - 1].Texture),
							_ => value
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return new(60, 30, 30, 30);
						return Mythril switch {
							-11 => new(60, 30, 30, 30),
							-12 => new(90, 30, 30, 30),
							_ => null,
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.AltOreName.RandomMythril");
						return Mythril switch {
							-11 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Mythril"),
							-12 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Orichalcum"),
							_ => AltLibrary.Ores[Mythril - 1].Name
						};
					}, (mod) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.BiomeOrOreModSecret");
						return Mythril switch {
							>= 0 => AltLibrary.Ores[Mythril - 1].Mod.Name,
							_ => mod
						};
					}),
				#endregion

				#region Adamantite
					new("Terraria/Adamantite", (int)OreType.Adamantite,
					(value) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return value;
						return Adamantite switch {
							>= 0 => ModContent.Request<Texture2D>(AltLibrary.Ores[Adamantite - 1].Texture),
							_ => value
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return new(120, 30, 30, 30);
						return Adamantite switch {
							-13 => new(120, 30, 30, 30),
							-14 => new(150, 30, 30, 30),
							_ => null,
						};
					}, () => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.AltOreName.RandomAdamantite");
						return Adamantite switch {
							-13 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Adamantite"),
							-14 => Language.GetTextValue("Mods.AltLibrary.AltOreName.Titanium"),
							_ => AltLibrary.Ores[Adamantite - 1].Name
						};
					}, (mod) => {
						if (!AltLibraryConfig.Config.OreIconsVisible) return Language.GetTextValue("Mods.AltLibrary.BiomeOrOreModSecret");
						return Adamantite switch {
							>= 0 => AltLibrary.Ores[Adamantite - 1].Mod.Name,
							_ => mod
						};
					})
				#endregion
				#endregion
				};
				foreach (AltOre ore in AltLibrary.Ores) {
					ore.AddOreOnScreenIcon(quene);
				}
				Quenes = quene;
			}
		}

		internal abstract class WorldCreationList<T> {
			public List<T> Types = new();

			public abstract void Initialize();
		}
	}
}
