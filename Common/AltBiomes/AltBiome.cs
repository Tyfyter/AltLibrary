﻿using AltLibrary.Common.Hooks;
using AltLibrary.Core.Baking;
using AltLibrary.Core.Generation;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Generation;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AltLibrary.Common.AltBiomes {
	public abstract class AltBiome : ModType, ILocalizedModType {
		internal int SpecialValueForWorldUIDoNotTouchElseYouCanBreakStuff { get; set; }
		internal bool? IsForCrimsonOrCorruptWorldUIFix { get; set; }
		/// <summary>
		/// Tells the Library what biome this is an alternative to
		/// </summary>
		public BiomeType BiomeType { get; set; }
		public virtual int ConversionType => BiomeType switch {
			BiomeType.Evil => 1,
			BiomeType.Hallow => 2,
			_ => 0,
		};
		internal int BiomeConversionType { get; set; } = -1;
		public virtual bool NPCsHate => BiomeType == BiomeType.Evil;
		public int Type { get; private protected set; }
		public virtual string LocalizationCategory => "AltBiomes";

		/// <summary>
		/// The name of this biome that will display on the biome selection screen.
		/// </summary>
		public virtual LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);
		/// <summary>
		/// The description for this biome that will appear on the biome selection screen.
		/// </summary>
		public virtual LocalizedText Description => this.GetLocalization("Description", PrettyPrintName);
		/// <summary>
		/// The message that will appear during world generation. Used by Evil alts.
		/// </summary>
		public virtual LocalizedText GenPassName => this.GetLocalization("GenPassName", PrettyPrintName);
		/// <summary>
		/// The descriptor the dryad will use when saying how much of the world is this biome
		/// </summary>
		public virtual LocalizedText DryadTextDescriptor => this.GetLocalization("DryadTextDescriptor", PrettyPrintName);
		/// <summary>
		/// Used in NPC dialog
		/// </summary>
		public virtual LocalizedText WorldEvilStone {
			get {
				if (!TileConversions.TryGetValue(TileID.Stone, out int evilStone)) return Language.GetText("misingno");
				int drop = TileLoader.GetItemDropFromTypeAndStyle(evilStone);
				if (drop == 0) return Language.GetText("misingno");
				return Lang.GetItemName(drop);
			}
		}

		/// <summary>
		/// Set this to something if for some reason you need RNG generation types or something
		/// </summary>
		public virtual EvilBiomeGenerationPass GetEvilBiomeGenerationPass() => EvilBiomeGenerationPass;

		/// <summary>
		/// Set this to something so your evil biome can generate
		/// </summary>
		public EvilBiomeGenerationPass EvilBiomeGenerationPass = null;

		/// <summary>
		/// For all biome types, only used used by AltLibrary if NPCsHate returns true, but should be overridden if a biome is associated with this biome
		/// </summary>
		public virtual IShoppingBiome Biome => null;

		#region Dungeon Loot
		/// <summary>
		/// For Underworld alts. If your biome uses a different kind of locked chest than a Shadow Chest, set this field to your equivalent to a Shadow Key so that it may appear in the Dungeon
		/// </summary>
		public int? ShadowKeyAlt = null;

		/// <summary>
		/// For Any alts with a biome key, not used by AltLibrary, just here so other mods can use it (for example, making a boss have a chance of dropping the world evil's key)
		/// </summary>
		public int? BiomeKeyItem = null;
		/// <summary>
		/// For Jungle, Evil, and Hallow alts. The ItemID for the rare item that will be found inside this biome's dungeon chest.
		/// </summary>
		public int? BiomeChestItem = null;
		/// <summary>
		/// For Jungle, Evil, and Hallow alts. The TileID of the special biome chest which will generate in the dungeon.
		/// </summary>
		public int? BiomeChestTile = null;
		/// <summary>
		/// For Jungle, Evil, and Hallow alts. The style number of the biome chest tile to be placed. Defaults to 0.
		/// </summary>
		public int? BiomeChestTileStyle = 0;
		#endregion

		#region Blocks and Convertables
		/// <summary>
		/// For Jungle and Evil alts. The TileID of the biome's associated ore.
		/// For Jungle alts, this block will begin to appear during hardmode and slow down the spread of nearby evil biome blocks. 
		/// </summary>
		public int? BiomeOre = null;
		/// <summary>
		/// For Jungle and Evil alts. The TileID of the biome's associated ore brick.
		/// For Jungle alts, this block will slow down the spread of nearby evil biome blocks. 
		/// For Evil alts, this is the block that will surround the loot of the Underworld boss. (WoF or Alt)
		/// </summary>
		public int? BiomeOreBrick = null;
		/// <summary>
		/// For Jungle, Evil, and Hallow alts. The TileID of the biome's grass block, which will also spread to dirt in a 1 block radius.
		/// For Evil and Hallow alts, this is the block that grass blocks will be converted into.
		/// For Jungle alts, this will block will spread to mud instead of dirt.
		/// </summary>
		public int? BiomeGrass = null;
		/// <summary>
		/// For Hallow alts. The TileID of the biome's mowed grass block. 
		/// You can also use this field for Evil or Jungle alts if you desire to make the grass able to be mowed.
		/// </summary>
		public int? BiomeMowedGrass = null;

		public Dictionary<int, int> TileConversions = new();
		public Dictionary<int, int> WallConversions = new();
		public Dictionary<int, int> GERunnerWallConversions = new();
		/// <summary>
		/// For Jungle alts. The tile which will replace vanilla Mud.
		/// </summary>
		public int? BiomeMud = null;
		/// <summary>
		/// For Jungle and evil alts, since vanilla introduced evil jungle grass. 
		/// </summary>
		public int? BiomeJungleGrass = null;
		/// <summary>
		/// For Jungle alts.
		/// </summary>
		public int? BiomeThornBush = null;
		/// <summary>
		/// For Jungle alts. The wall that replaces mud walls.
		/// </summary>
		public int? BiomeMudWall = null;
		/// <summary>
		/// For Jungle alts. The shortgrass that grows on the grass tile.
		/// </summary>
		public int? BiomeJunglePlants = null;
		/// <summary>
		/// For Jungle alts - the large 3x2 plants that grow in the jungle.
		/// </summary>
		public int? BiomeJungleBushes = null;
		/// <summary>
		/// For Jungle alts. Used in the shrine-like structure in the underground.
		/// </summary>
		public int? BiomeShrineChestType = null;

		/// <summary>
		/// For Evil and Hallow alts. The list of tiles that can spread this biome. 
		/// This should generally include the biomeGrass, biomeStone, etc blocks, but they can be omitted if you for some reason do not wish for those blocks to spread the biome.
		/// </summary>
		public virtual List<int> SpreadingTiles { get; } = [];

		/// <summary>
		/// For Evil alts, this is the TileID of this biome's equivalent to Demon Altars.
		/// For Underworld alts, this is the TileID of this biome's equivalent to Hellforges.
		/// </summary>
		public int? AltarTile = null;

		/// <summary>
		/// For Jungle alts, if you use a mechanic similar to Plantera's bulb, list the TileID of that tile here.
		/// </summary>
		public int? BossBulb = null;

		public virtual List<int> HardmodeWalls => new();
		#endregion

		#region Blood Moon
		/// <summary>
		/// For Evil alts, the NPCID of the creature Bunnies will turn into during a blood moon.
		/// </summary>
		public int? BloodBunny = null;
		/// <summary>
		/// For Evil alts, the NPCID of the creature Goldfish will turn into during a blood moon.
		/// </summary>
		public int? BloodGoldfish = null;
		/// <summary>
		/// For Evil alts, the NPCID of the creature Penguins will turn into during a blood moon.
		/// </summary>
		public int? BloodPenguin = null;
		#endregion

		#region Mimic
		/// <summary>
		/// For Evil and Hallow alts. The NPC ID to be spawned when placing a Key of Night or Light respectively in a chest.
		/// </summary>
		public int? MimicType = null;
		/// <summary>
		/// For Evil and Hallow alts. The NPC ID to be spawned when placing a Key of Night or Light respectively in a chest.
		/// </summary>
		public int? MimicKeyType = null;
		#endregion

		#region Boss Loot
		/// <summary>
		/// For Hallow alts. The ItemID of the metal that the mechanical bosses will drop in place of Hallowed Bars.
		/// </summary>
		public int? MechDropItemType = null;
		/// <summary>
		/// For Hallow alts. The ItemID of your biome's counterpart to the Pwnhammer, if it has one. 
		/// </summary>
		public int HammerType = ItemID.Pwnhammer;

		/// <summary>
		/// For Evil and Hallow alts. You may list additional tiles that this biome can convert into its own blocks during GERunner.
		/// The first value is the pure tile, the second value is its infected counterpart.
		/// </summary>
		public virtual Dictionary<int, int> GERunnerConversion => _geRunnerConversion;
		Dictionary<int, int> _geRunnerConversion = [];

		/// <summary>
		/// For Evil Alts. The ItemID of the seeds that Eye of Cthulhu will drop in worlds with this biome.
		/// </summary>
		public int? SeedType = null;
		/// <summary>
		/// For Evil Alts. The ItemID of the ore that Eye of Cthulhu will drop in worlds with this biome.
		/// </summary>
		public int? BiomeOreItem = null;
		/// <summary>
		/// For Evil Alts. In Corruption worlds, EoC will drop Unholy Arrows, which the Crimson has no equivalent for. 
		/// If you have a comparable item you wish for EoC to drop, define it here.
		/// </summary>
		public int? ArrowType = null;
		#endregion

		#region Menu Graphics
		[Obsolete("The associated feature has been replaced, and its replacement uses IconSmall", true)]
		public virtual string IconLarge => null;
		/// <summary>
		/// The path to the 30x30 texture of the small icon that will appear on the biome selection screen to represent this biome. This should typically be the same as the biome's main bestiary icon.
		/// </summary>
		public virtual string IconSmall => null;

		/// <summary>
		/// The path to the texture that will serve as one of the layers of the tree on the world selection screen.
		/// </summary>
		public virtual string WorldIcon => "AltLibrary/Assets/WorldIcons/NullBiome/NullBiome";
		/// <summary>
		/// For Evil biomes. The texture that appears around the loading bar on world creation.
		/// </summary>
		public virtual string OuterTexture => "AltLibrary/Assets/Loading/Outer Empty";
		public Asset<Texture2D> OuterTextureAsset { get; protected internal set; }
		/// <summary>
		/// For Underworld biomes. The texture that appears around the lower loading bar on world creation.
		/// </summary>
		public virtual string LowerTexture => "AltLibrary/Assets/Loading/Outer Lower Empty";// TODO: create a default/template sprite to make the bar look less ugly when no bar is specified
		public Asset<Texture2D> LowerTextureAsset { get; protected internal set; }
		/// <summary>
		/// For Evil biomes. The texture that appears inside the loading bar on world creation.
		/// </summary>
		public virtual Color OuterColor => new(127, 127, 127);
		/// <summary>
		/// For Underworld biomes. The texture that appears inside the lower loading bar on world creation.
		/// </summary>
		public virtual Color LowerColor => new(127, 127, 127);
		/// <summary>
		/// The color of this biome's name that will appear on the biome selection menu.
		/// </summary>
		public virtual Color NameColor => new(255, 255, 255);
		/// <summary>
		/// Whether or not this biome will appear on the selection menu.
		/// </summary>
		public virtual bool Selectable { get; } = true;
		#endregion

		#region Remix Worlds
		/// <summary>
		/// In Remix worlds, sky islands generate as the world's Evil biome.
		/// Set this field to the TileID of the block that should replace flesh/lesion blocks if you are making an Evil alt.
		/// </summary>
		public int? BiomeFlesh = null;
		/// <summary>
		/// In Remix worlds, sky islands generate as the world's Evil biome.
		/// Set this field to the TileID of the block that should replace flesh/lesion walls if you are making an Evil alt.
		/// </summary>
		public int? BiomeFleshWall = null;
		/// <summary>
		/// In Remix worlds, sky islands generate as the world's Evil biome.
		/// Set this field to the TileID of the block that should replace the flesh/lesion chest if you are making an Evil alt.
		/// </summary>
		public int? FleshChestTile = null;
		/// <summary>
		/// In Remix worlds, sky islands generate as the world's Evil biome.
		/// Set this field to the style of the block that should replace the flesh/lesion chest if you are making an Evil alt.
		/// </summary>
		public int? FleshChestTileStyle = null;
		/// <summary>
		/// In Remix worlds, sky islands generate as the world's Evil biome.
		/// Set this field to the TileID of the block that should replace the flesh/lesion table if you are making an Evil alt.
		/// </summary>
		public int? FleshTableTile = null;
		/// <summary>
		/// In Remix worlds, sky islands generate as the world's Evil biome.
		/// Set this field to the style of the block that should replace the flesh/lesion table if you are making an Evil alt.
		/// </summary>
		public int? FleshTableTileStyle = null;
		/// <summary>
		/// In Remix worlds, sky islands generate as the world's Evil biome.
		/// Set this field to the TileID of the block that should replace the flesh/lesion chair if you are making an Evil alt.
		/// </summary>
		public int? FleshChairTile = null;
		/// <summary>
		/// In Remix worlds, sky islands generate as the world's Evil biome.
		/// Set this field to the style of the block that should replace the flesh/lesion chair if you are making an Evil alt.
		/// </summary>
		public int? FleshChairTileStyle = null;
		/// <summary>
		/// In Remix worlds, sky islands generate as the world's Evil biome.
		/// Set this field to the TileID of the block that should replace the flesh/lesion door if you are making an Evil alt.
		/// </summary>
		public int? FleshDoorTile = null;
		/// <summary>
		/// In Remix worlds, sky islands generate as the world's Evil biome.
		/// Set this field to the style of the block that should replace the flesh/lesion door if you are making an Evil alt.
		/// </summary>
		public int? FleshDoorTileStyle = null;
		#endregion
		/// <summary>
		/// In CelebrationMk10 worlds, sky islands may generate as the world's Hallow biome, with a corresponding water fountain.
		/// Set this field to the TileID of your biome's water fountain if you are making a Hallow alt.
		/// </summary>
		public int? FountainTile = null;
		/// <summary>
		/// If your Hallow alt's water fountain is part of a larger tilesheet, specify the style of the appropriate water fountain here.
		/// </summary>
		public int? FountainTileStyle = null;
		/// <summary>
		/// If your Hallow alt's water fountain is part of a larger tilesheet, specify the x frame of the appropriate active water fountain here.
		/// </summary>
		public int? FountainActiveFrameX = null;
		/// <summary>
		/// If your Hallow alt's water fountain is part of a larger tilesheet, specify the y frame of the appropriate active water fountain here.
		/// </summary>
		public int? FountainActiveFrameY = null;

		/// <summary>
		/// If this is set to true, none of this biome's conversions will be reversed by purification powder or green solution
		/// </summary>
		public bool NoDeconversion = false;
		public virtual Color AltUnderworldColor => Color.Black;
		public virtual Asset<Texture2D>[] AltUnderworldBackgrounds => new Asset<Texture2D>[14];
		public virtual AltMaterialContext MaterialContext => null;
		public sealed override void SetupContent() {
			if (NPCsHate && Biome is not null) ShopHelper_EvilBiomes.DangerousBiomes.Add(Biome);
			SetStaticDefaults();
		}
		/// <summary>
		/// Called before a tile is converted to a different biome
		/// Return false to prevent conversion
		/// </summary>
		/// <returns></returns>
		public virtual bool ConvertTileAway(int i, int j) => true;
		/// <summary>
		/// Called before a wall is converted to a different biome
		/// Return false to prevent conversion
		/// </summary>
		/// <returns></returns>
		public virtual bool ConvertWallAway(int i, int j) => true;

		/// <summary>
		/// You have no reason to overwrite this unless you need to make some conversions conditional or random
		/// For Clentaminator purposes. Gets the alt block/wall of the base block/wall. Override this function and call base if you want to add new functionality.
		/// Returns -1 if it's an invalid conversion
		/// </summary>
		public virtual int GetAltBlock(int BaseBlock, int posX, int posY, bool GERunner = false) {
			return TileConversions.TryGetValue(BaseBlock, out int val) ? val : (GERunner && GERunnerConversion.TryGetValue(BaseBlock, out val) ? val : -1);
		}
		/// <inheritdoc cref="GetAltBlock(int, int, int, bool)"/>
		public virtual int GetAltWall(int BaseWall, int posX, int posY, bool GERunner = false) {
			return WallConversions.TryGetValue(BaseWall, out int val) ? val : (GERunner && GERunnerWallConversions.TryGetValue(BaseWall, out val) ? val : -1);
		}
		public bool HasAllTileConversions(params int[] tiles) {
			for (int i = 0; i < tiles.Length; i++) if (!TileConversions.ContainsKey(tiles[i])) return false;
			return true;
		}
		protected sealed override void Register() {
			ModTypeLookup<AltBiome>.Register(this);
			if (ModContent.RequestIfExists(OuterTexture, out Asset<Texture2D> asset)) OuterTextureAsset = asset;
			if (ModContent.RequestIfExists(LowerTexture, out asset)) LowerTextureAsset = asset;
			_ = DisplayName.Value;
			_ = Description.Value;
			_ = GenPassName.Value;
			if (BiomeType is BiomeType.Evil or BiomeType.Hallow) _ = DryadTextDescriptor.Value;
			if (this is VanillaBiome) {
				AltLibrary.VanillaBiomes.Add(this);
				return;
			}
			AltLibrary.Biomes.Add(this);
			if (BossBulb != null) AltLibrary.planteraBulbs.Add((int)BossBulb);
			if (BiomeType == BiomeType.Jungle) {
				if (BiomeGrass != null) {
					AltLibrary.jungleGrass.Add((int)BiomeGrass);
				} else {
					if (BiomeJungleGrass != null) AltLibrary.jungleGrass.Add((int)BiomeJungleGrass);
				}
				if (BiomeMowedGrass != null) AltLibrary.jungleGrass.Add((int)BiomeGrass);
				if (BiomeOre != null) AltLibrary.evilStoppingOres.Add((int)BiomeOre);
				if (BiomeOreBrick != null) AltLibrary.evilStoppingOres.Add((int)BiomeOreBrick);
			}
			if (BiomeType == BiomeType.Hell && AltUnderworldBackgrounds != null && AltUnderworldBackgrounds.Length != TextureAssets.Underworld.Length) {
				throw new IndexOutOfRangeException(nameof(AltUnderworldBackgrounds) + " length isn't same as Underworld's! (" + TextureAssets.Underworld.Length + ")");
			}
			Type = AltLibrary.Biomes.Count;
		}

		/// <summary>
		/// Override if you want custom selection
		/// </summary>
		/// <param name="list"></param>
		[Obsolete("Custom selection is no longer supported, support may be re-added if someone has a use for it")]
		public virtual void CustomSelection(List<AltBiome> list) {
			int index = list.FindLastIndex(x => x.BiomeType == BiomeType);
			if (index != -1) {
				list.Insert(index + 1, this);
			}
		}

		/// <summary>
		/// Override if you want to have random value whenever creating new world. Should be used just for custom tiers.
		/// </summary>
		public virtual void OnInitialize() {
		}

		/// <summary>
		/// If you want custom action on click, then use this. Useful for "RandomX" options and custom tiers.
		/// <br/>By default: false.
		/// <br/>Set to true if you want to override default behavior.
		/// </summary>
		public virtual bool OnClick() => false;

		public virtual void OnCreating() {
		}

		public virtual void AddBiomeOnScreenIcon(List<ALDrawingStruct<AltBiome>> list) {
		}
		public static TileLoader.ConvertTile CreateConversion(int toType) => (i, j, _, _) => {
			WorldGen.ConvertTile(i, j, toType);
			return false;
		};
		public void AddTileConversion(int block, int parentBlock, bool spread = true, bool oneWay = false, bool extraFunctions = true) {
			if (!oneWay) AddChildTile(block, parentBlock);
			if (BiomeConversionType != -1) TileLoader.RegisterConversion(parentBlock, BiomeConversionType, CreateConversion(block));
			if (Main.tileMoss[parentBlock] && TileConversions.TryGetValue(TileID.Stone, out int stone) && TileConversions[parentBlock] == stone) {
				TileConversions[parentBlock] = block;
			} else {
				TileConversions.Add(parentBlock, block);
			}
			if (extraFunctions) {
				switch (parentBlock) {
					case TileID.Grass:
					BiomeGrass = block;
					if (BiomeType == BiomeType.Evil) TileConversions.TryAdd(TileID.GolfGrass, block);
					break;

					case TileID.JungleGrass:
					if (BiomeType == BiomeType.Evil) TileConversions.TryAdd(TileID.MushroomGrass, block);
					break;

					case TileID.GolfGrass:
					BiomeMowedGrass = block;
					break;

					case TileID.Stone:
					for (int i = 0; i < Main.tileMoss.Length; i++) {
						if (Main.tileMoss[i]) TileConversions.TryAdd(i, block);
					}
					break;
				}
			}
			if (spread) {
				SpreadingTiles.Add(block);
			}
			if (NoDeconversion) ALConvertInheritanceData.tileParentageData.NoDeconversion.Add(block);
		}
		public void AddWallConversions<T>(params int[] orig) where T : ModWall {
			ushort type = ContentInstance<T>.Instance.Type;
			AddWallConversions(type, orig);
		}
		public void AddWallConversions(int with, params int[] orig) {
			foreach (int original in orig) {
				WallConversions.TryAdd(original, with);
				ALConvertInheritanceData.wallParentageData.AddParent(with, (original, this));
				if (NoDeconversion) ALConvertInheritanceData.wallParentageData.NoDeconversion.Add(with);
			}
		}
		public void AddWallConversions<T>(params bool[] set) where T : ModWall {
			ushort type = ContentInstance<T>.Instance.Type;
			AddWallConversions(type, set);
		}
		public void AddWallConversions(int with, params bool[] set) {
			for (int i = 0; i < set.Length; i++) {
				if (set[i]) {
					WallConversions.TryAdd(i, with);
					ALConvertInheritanceData.wallParentageData.AddParent(with, (i, this));
					if (NoDeconversion) ALConvertInheritanceData.wallParentageData.NoDeconversion.Add(with);
				}
			}
		}
		[Obsolete("This method's parameters are ordered differently than similar methods, this method has only been kept for the sake of simplifying porting")]
		public void AddWallReplacement(int orig, int with) {
			WallConversions.TryAdd(orig, with);
			ALConvertInheritanceData.wallParentageData.AddParent(with, (orig, this));
		}
		[Obsolete("This method's parameters are ordered differently than similar methods, this method has only been kept for the sake of simplifying porting")]
		public void AddWallReplacement<T>(params int[] orig) where T : ModWall {
			ushort type = ContentInstance<T>.Instance.Type;
			foreach (ushort original in orig) {
				AddWallReplacement(original, type);
			}
		}

		public void AddChildTile(int Block, int ParentBlock, BitsByte? BreakIfConversionFail = null) {
			ALConvertInheritanceData.tileParentageData.AddParent(Block, (ParentBlock, this));
			if (BreakIfConversionFail != null) {
				ALConvertInheritanceData.tileParentageData.BreakIfConversionFail.Add(Block, BreakIfConversionFail.Value);
			}
		}

		public void AddChildWall(int Wall, int ParentWall, BitsByte? BreakIfConversionFail = null) {
			ALConvertInheritanceData.tileParentageData.AddParent(Wall, (ParentWall, this));
			if (BreakIfConversionFail != null) {
				ALConvertInheritanceData.wallParentageData.BreakIfConversionFail.Add(Wall, BreakIfConversionFail.Value);
			}
		}
		public virtual bool PreConvertMultitileAway(int i, int j, int width, int height, ref int newTile, AltBiome targetBiome) {
			return true;
		}
		public virtual void ConvertMultitileTo(int i, int j, int width, int height, int newTile, AltBiome fromBiome) { }
		public virtual void ModifyGenPass(List<GenPass> passes, GenPass originalPass) { }
		public static bool operator ==(AltBiome a, AltBiome b) {
			if (a is null) return b is null;
			if (b is null) return false;
			return a.GetType() == b.GetType();
		}
		public static bool operator !=(AltBiome a, AltBiome b) {
			if (a is null) return b is not null;
			if (b is null) return true;
			return a.GetType() != b.GetType();
		}
		public override bool Equals(object obj) {
			return (obj is AltBiome other) && other == this;
		}
		public override int GetHashCode() {
			unchecked {
				return GetType().GetHashCode();
			}
		}
		internal Condition ActiveShopCondition = null;
		internal Condition InactiveShopCondition = null;
	}
}
