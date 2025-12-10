using AltLibrary.Common.Hooks;
using AltLibrary.Core;
using AltLibrary.Core.Generation;
using AltLibrary.Core.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PegasusLib;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.WorldBuilding;
using static AltLibrary.Core.Grasses;
using static Terraria.Utilities.NPCUtils;

namespace AltLibrary.Common.AltBiomes {
	public abstract class AltBiome : ModType, ILocalizedModType {
		internal string FullNameOverride = null;
		public new string FullName => FullNameOverride ?? base.FullName;
		/// <summary>
		/// Tells the Library what biome this is an alternative to
		/// </summary>
		public BiomeType BiomeType { get; set; }
		[Obsolete("AltLibrary no longer replaces the vanilla conversion system", true)]
		public virtual int ConversionType => BiomeConversionType;
		public int BiomeConversionType { get; internal set; } = -1;
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
		public virtual LocalizedText WorldEvilStone => _worldEvilStone;
		LocalizedText _worldEvilStone = Language.GetText("misingno");

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
		public virtual List<int> SpreadingTiles => spreadingTiles;
		List<int> spreadingTiles = [];

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
		public virtual string WorldIcon => BiomeType > BiomeType.Hallow ? null : "AltLibrary/Assets/WorldIcons/NullBiome/NullBiome";
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

		/// <summary>
		/// The color biome sight potions will highlight this biome's tiles in
		/// </summary>
		public virtual Color? BiomeSightColor => null;
		public virtual Color AltUnderworldColor => Color.Black;
		public virtual Asset<Texture2D>[] AltUnderworldBackgrounds => new Asset<Texture2D>[14];
		public virtual AltMaterialContext MaterialContext => null;
		public sealed override void SetupContent() {
			if (NPCsHate && Biome is not null) ShopHelper_EvilBiomes.DangerousBiomes.Add(Biome);
			SetStaticDefaults();
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
			if (this is VanillaBiome) return;
			if (!createdGrassType) {
				GrassGrowthSettings settings = new(BiomeType is BiomeType.Evil or BiomeType.Hallow, BiomeType is BiomeType.Evil or BiomeType.Hallow);
				switch ((BiomeGrass.HasValue, BiomeJungleGrass.HasValue)) {
					case (true, true):
					CreateGrassType(settings, (TileID.Dirt, BiomeGrass.Value), (TileID.Mud, BiomeJungleGrass.Value));
					break;
					case (true, false):
					CreateGrassType(settings, (TileID.Dirt, BiomeGrass.Value));
					break;
					case (false, true):
					CreateGrassType(settings, (TileID.Mud, BiomeJungleGrass.Value));
					break;
				}
			}
		}
		bool createdGrassType = false;
		public void CreateGrassType(GrassGrowthSettings settings, params (int dirtType, int grassType)[] types) {
			if (!createdGrassType.TrySet(true)) return;
			Grasses.Register(settings, types);
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

		[Obsolete("AltLibrary no longer replaces the vanilla conversion system", true)]
		public virtual int GetAltBlock(int BaseBlock, int posX, int posY, bool GERunner = false) => default;
		[Obsolete("AltLibrary no longer replaces the vanilla conversion system", true)]
		public virtual int GetAltWall(int BaseWall, int posX, int posY, bool GERunner = false) => default;
		public bool HasAllTileConversions(params int[] tiles) {
			for (int i = 0; i < tiles.Length; i++) if (!TileLoaderReflection.HasConversion(tiles[i], BiomeConversionType)) return false;
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
			}
			Type = AltLibrary.Biomes.Count;
			AltLibrary.Biomes.Add(this);
			if (BiomeType == BiomeType.Hell && AltUnderworldBackgrounds != null && AltUnderworldBackgrounds.Length != TextureAssets.Underworld.Length) {
				throw new IndexOutOfRangeException(nameof(AltUnderworldBackgrounds) + " length isn't same as Underworld's! (" + TextureAssets.Underworld.Length + ")");
			}
			if (this is not VanillaBiome) BiomeConversionType = BiomeConversionLoader.Register(new AltBiomeConversion(this));
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
		public static TileLoader.ConvertTile CreateConversion(int toType) {
			switch (toType) {
				case -2:
				return (i, j, _, _) => {
					Tile tile = Main.tile[i, j];
					tile.HasTile = false;
					WorldGen.SquareTileFrame(i, j);
					return false;
				};

				case -1:
				return (_, _, _, _) => true;

				default:
				return (i, j, _, _) => {
					WorldGen.ConvertTile(i, j, toType);
					return false;
				};
			}
		}
		public record MultitileData(int Type, params int[] Styles) {
			public MultitileData(int type, Range firstStyles, params Range[] styles) : this(type, styles.Concat([firstStyles]).SelectMany(r => {
				int[] values = new int[(r.End.Value - r.Start.Value) + 1];
				for (int i = 0; i < values.Length; i++) values[i] = i + r.Start.Value;
				return values;
			}).ToArray()) { }
		}
		static TileObjectData potsData;
		static TileObjectData PotsData => potsData ??= new(TileObjectData.Style2x2) {
			Width = 2,
			Height = 2,
			StyleWrapLimit = 3,
			StyleHorizontal = true
		};
		static int GetTileStyle(TileObjectData tileObjectData, Tile getTile) {
			if (!getTile.HasTile || tileObjectData is null)
				return -1;

			// Adapted from GetTileData
			int num = getTile.TileFrameX / tileObjectData.CoordinateFullWidth;
			int num2 = getTile.TileFrameY / tileObjectData.CoordinateFullHeight;
			int num3 = tileObjectData.StyleWrapLimit;
			if (num3 == 0)
				num3 = 1;

			int styleLineSkip = tileObjectData.StyleLineSkip;
			int num4 = (!tileObjectData.StyleHorizontal) ? (num / styleLineSkip * num3 + num2) : (num2 / styleLineSkip * num3 + num);
			int num5 = num4 / tileObjectData.StyleMultiplier;

			return num5;
		}
		static TileObjectData GetTileData(Tile tile) => tile.TileType == TileID.Pots ? PotsData : TileObjectData.GetTileData(tile);
		static TileObjectData GetTileData(int type, int style) => type == TileID.Pots ? PotsData : TileObjectData.GetTileData(type, style);
		public static TileLoader.ConvertTile CreateMultitileConversion(MultitileData from, MultitileData to) {
			return (i, j, _, _) => {
				Tile startTile = Main.tile[i, j];
				TileObjectData fromData = GetTileData(startTile);
				int style = GetTileStyle(fromData, startTile);
				if (startTile.TileType != from.Type || (from.Styles.Length > 0 && !from.Styles.Contains(style)))
					return true;
				TileUtils.GetMultiTileTopLeft(i, j, fromData, out int left, out int top);

				TileObjectData toData = GetTileData(to.Type, 0);
				int targetStyle = to.Styles.Length <= 0 ? WorldGen.genRand.Next(toData.RandomStyleRange) : WorldGen.genRand.Next(to.Styles);
				int wrapLimit = toData.StyleWrapLimit;
				if (wrapLimit == 0)
					wrapLimit = 1;

				int frameX = targetStyle % wrapLimit;
				int frameY = targetStyle / wrapLimit;

				if (!toData.StyleHorizontal) (frameY, frameX) = (frameX, frameY);

				frameX *= toData.CoordinateFullWidth;
				frameY *= toData.CoordinateFullHeight;
				for (int x = 0; x < fromData.Width; x++) {
					for (int y = 0; y < fromData.Height; y++) {
						Tile tile = Main.tile[left + x, top + y];
						tile.TileType = (ushort)to.Type;
						tile.TileFrameX = (short)(frameX + x * 18);
						tile.TileFrameY = (short)(frameY + y * 18);
					}
				}
				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendTileSquare(-1, left, top, fromData.Width, fromData.Height);
				return false;
			};
		}
		public void AddMultiTileConversion(MultitileData block, MultitileData parentBlock, bool oneWay = false) => AddMultiTileConversion(block, parentBlock, out _, oneWay);
		public void AddMultiTileConversion(MultitileData block, MultitileData parentBlock, out TileLoader.ConvertTile purify, bool oneWay = false) {
			purify = null;
			if (block.Type == -2) {
				TileLoader.RegisterConversion(parentBlock.Type, BiomeConversionType, (i, j, _, _) => {
					Tile tile = Main.tile[i, j];
					tile.HasTile = false;
					WorldGen.SquareTileFrame(i, j);
					return false;
				});
				return;
			}
			oneWay |= NoDeconversion;

			TileLoader.RegisterConversion(parentBlock.Type, BiomeConversionType, CreateMultitileConversion(parentBlock, block));
			if (!oneWay) {
				purify = CreateMultitileConversion(block, parentBlock);
				TileLoader.RegisterConversion(block.Type, BiomeConversionID.Purity, purify);
				TileLoader.RegisterConversion(block.Type, BiomeConversionID.PurificationPowder, purify);
				TileLoader.RegisterConversion(block.Type, BiomeConversionID.Chlorophyte, purify);
			}
			if (block.Type == parentBlock.Type) return;
			if (TileSets.OwnedByBiomeID[block.Type] == -1) TileSets.OwnedByBiomeID[block.Type] = Type;
		}
		public void AddTileConversion(int block, int parentBlock, bool spread = true, bool oneWay = false, bool extraFunctions = true) {
			oneWay |= NoDeconversion;
			CreateConversion(parentBlock);
			if (parentBlock == TileID.Stone) {
				int drop = TileLoader.GetItemDropFromTypeAndStyle(block);
				if (drop == 0) _worldEvilStone = Lang.GetItemName(drop);
			}
			if (extraFunctions) {
				switch (parentBlock) {
					case TileID.Grass:
					BiomeGrass = block;
					if (BiomeType == BiomeType.Evil) CreateConversion(TileID.GolfGrass);
					break;

					case TileID.JungleGrass:
					BiomeJungleGrass = block;
					if (BiomeType == BiomeType.Evil) CreateConversion(TileID.MushroomGrass);
					break;

					case TileID.GolfGrass:
					BiomeMowedGrass = block;
					break;

					case TileID.Stone:
					for (int i = 0; i < Main.tileMoss.Length; i++) {
						if (Main.tileMoss[i]) CreateConversion(i);
					}
					if (block < 0) break;
					TileLoader.RegisterConversion(TileID.Hive, BiomeConversionType, (i, j, _, _) => {
						if (!HardmodeWorldGen.GERunnerRunning || !HardmodeWorldGen.ShouldConvertBeeTiles) return true;
						WorldGen.ConvertTile(i, j, block);
						return false;
					});
					break;

					case TileID.HardenedSand:
					for (int i = 0; i < Main.tileMoss.Length; i++) {
						if (Main.tileMoss[i]) CreateConversion(i);
					}
					if (block < 0) break;
					TileLoader.RegisterConversion(TileID.CrispyHoneyBlock, BiomeConversionType, (i, j, _, _) => {
						if (!HardmodeWorldGen.GERunnerRunning || !HardmodeWorldGen.ShouldConvertBeeTiles) return true;
						WorldGen.ConvertTile(i, j, block);
						return false;
					});
					break;
				}
			}
			if (block < 0) return;
			if (spread) {
				spreadingTiles.Add(block);
				TileSets.BiomeSightColors[block] = BiomeSightColor;
			}
			if (TileSets.OwnedByBiomeID[block] == -1) TileSets.OwnedByBiomeID[block] = Type;
			void CreateConversion(int fromType) {
				switch (block) {
					case -2:
					TileLoader.RegisterConversion(fromType, BiomeConversionType, (i, j, _, _) => {
						Tile tile = Main.tile[i, j];
						tile.HasTile = false;
						WorldGen.SquareTileFrame(i, j);
						return false;
					});
					break;

					default:
					TileLoader.RegisterSimpleConversion(fromType, BiomeConversionType, block, !oneWay);
					break;
				}
			}
		}
		public void AddWallConversions<T>(params int[] orig) where T : ModWall {
			AddWallConversions(ModContent.WallType<T>(), orig);
		}
		public void AddWallConversions(int with, params int[] orig) {
			foreach (int original in orig) WallLoader.RegisterSimpleConversion(original, BiomeConversionType, with, !NoDeconversion);
			if (WallSets.OwnedByBiomeID[with] == -1) WallSets.OwnedByBiomeID[with] = Type;
		}
		public void AddWallConversions<T>(params bool[] set) where T : ModWall {
			AddWallConversions(ModContent.WallType<T>(), set);
		}
		public void AddWallConversions(int with, params bool[] set) {
			for (int i = 0; i < set.Length; i++) {
				if (set[i]) WallLoader.RegisterSimpleConversion(i, BiomeConversionType, with, !NoDeconversion);
			}
			if (WallSets.OwnedByBiomeID[with] == -1) WallSets.OwnedByBiomeID[with] = Type;
		}
		[Obsolete("This method's parameters are ordered differently than similar methods, this method has only been kept for the sake of simplifying porting, this method will be removed in an upcoming version", true)]
		public void AddWallReplacement(int orig, int with) {
			WallLoader.RegisterSimpleConversion(orig, BiomeConversionType, with, !NoDeconversion);
			if (WallSets.OwnedByBiomeID[with] == -1) WallSets.OwnedByBiomeID[with] = Type;
		}
		[Obsolete("This method's parameters are ordered differently than similar methods, this method has only been kept for the sake of simplifying porting, this method will be removed in an upcoming version", true)]
		public void AddWallReplacement<T>(params int[] orig) where T : ModWall {
			int with = ModContent.WallType<T>();
			foreach (int original in orig) {
				WallLoader.RegisterSimpleConversion(original, BiomeConversionType, with, !NoDeconversion);
			}
			if (WallSets.OwnedByBiomeID[with] == -1) WallSets.OwnedByBiomeID[with] = Type;
		}

		[Obsolete("AltLibrary no longer replaces the vanilla conversion system")]
		public void AddChildTile(int Block, int ParentBlock, BitsByte? BreakIfConversionFail = null) {
			TileLoader.RegisterConversionFallback(Block, ParentBlock, BiomeConversionType);
			if (TileSets.OwnedByBiomeID[Block] == -1) TileSets.OwnedByBiomeID[Block] = Type;
		}
		[Obsolete("AltLibrary no longer replaces the vanilla conversion system")]
		public void AddChildWall(int Wall, int ParentWall, BitsByte? BreakIfConversionFail = null) {
			WallLoader.RegisterConversionFallback(Wall, ParentWall, BiomeConversionType);
			if (WallSets.OwnedByBiomeID[Wall] == -1) WallSets.OwnedByBiomeID[Wall] = Type;
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
	public class AltBiomeConversion(AltBiome biome) : ModBiomeConversion {
		public override string Name => biome.Name + "_Conversion";
	}
}
