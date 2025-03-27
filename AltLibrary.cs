using AltLibrary.Common;
using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.AltOres;
using AltLibrary.Common.Hooks;
using AltLibrary.Common.Systems;
using AltLibrary.Core;
using AltLibrary.Core.Baking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace AltLibrary {
	public partial class AltLibrary : Mod {
		public static AltLibrary Instance { get; internal set; }

		internal static List<AltBiome> Biomes { get; private set; } = [];
		internal static List<AltBiome> VanillaBiomes { get; private set; } = [];
		public static MultiList<AltBiome> AllBiomes { get; private set; }

		internal static List<AltOre> Ores = [];

		internal static List<GlobalBiome> GlobalBiomes = [];

		internal static List<ALBiomeTileCountModifier> ALBiomeTileCountModifiers = [];

		internal static List<CustomPreviews> PreviewWorldIcons = [];

		// Spreading related lists.
		internal static List<int> planteraBulbs = [TileID.PlanteraBulb];
		internal static List<int> jungleGrass = [TileID.JungleGrass];
		internal static List<int> evilStoppingOres = [TileID.Chlorophyte, TileID.ChlorophyteBrick];

		internal static Dictionary<int, int> baseTiles = [];

		internal static int TimeHoveringOnIcon;
		internal static bool HallowBunnyUnlocked;
		internal static int ModIconVariation;
		static AltLibrary() {
			AllBiomes = new MultiList<AltBiome>() { VanillaBiomes, Biomes };
		}
		public AltLibrary() {
			ALReflection.Init();
			Instance = this;
		}

		public static void AddInFinishList(AltOre ore) => UIWorldCreationEdits.AddInFinishedCreation.Add(ore);
		public static void AddInFinishList(AltBiome ore) => UIWorldCreationEdits.AddInFinishedCreation2.Add(ore);

		public override void Load() {
			ALTextureAssets.Load();
			ILHooks.OnInitialize();
			ALConvert.Load();
			GuideHelpText.Load();
			ExtractinatorOres.Load();
			ModIconVariation = Main.rand.Next(ALTextureAssets.AnimatedModIcon.Length);
			TimeHoveringOnIcon = 0;
			HallowBunnyUnlocked = false;
		}

		public override void PostSetupContent() {
			ALHooks.OnInitialize();
			ALTextureAssets.PostContentLoad();
			MimicSummon.SetupContent();
			ALConvertInheritanceData.FillData();
			ModSupport.ModSupport.HookAll();
			AltOreInsideBodies.Setup();
			ExtractinatorOres.Setup();

			if (ModLoader.TryGetMod("Fargowiltas", out Mod fargowiltas)) {
				foreach (AltBiome biome in Biomes) {
					if (biome.AltarTile.HasValue) fargowiltas.Call("AddEvilAltar", (int)biome.AltarTile.Value);
				}
			}
			//BackgroundsAlternating.Init();//TODO: redo
		}

		public override object Call(params object[] args)
		{
			if (args is null)
				throw new ArgumentNullException(nameof(args), "Arguments cannot be null!");
			if (args.Length == 0)
				throw new ArgumentException("Arguments cannot be empty!");
			if (args[0] is string content)
			{
				switch (content.ToLower())
				{
					case "addcustomseedpreviews":
						{
							if (args.Length != 5)
								throw new ArgumentException("Arguments cannot be less or more than 5 in length for AddCustomSeedPreviews");
							if (args[1] is not string seed)
								throw new ArgumentException("Second argument (seed) is not string!");
							if (args[2] is not string small)
								throw new ArgumentException("Third argument (small) is not string!");
							if (args[3] is not string medium)
								throw new ArgumentException("Fourth argument (medium) is not string!");
							if (args[4] is not string large)
								throw new ArgumentException("Fifth argument (large) is not string!");
							PreviewWorldIcons.Add(new CustomPreviews(seed, small, medium, large));
							Logger.Debug($"Registered custom preview! Seed: {seed} Path: {small} {medium} {large}");
							return "Success";
						}
					case "conversiongetultimateparent":
						if (args.Length != 2)
							throw new ArgumentException("Arguments cannot be less or more than 0 in length for GetUltimateParent");
						if (args[1] is not int tile)
							throw new ArgumentException("Second argument (tile) is not int!");
						return ALConvertInheritanceData.GetUltimateParent(tile);
					case "convert":
						if (args.Length == 6)
						{
							if (args[1] is not Mod mod)
								throw new ArgumentException("Second argument (mod) is not Mod!");
							if (args[2] is not string name)
								throw new ArgumentException("Third argument (name) is not string!");
							if (args[3] is not int i)
								throw new ArgumentException("Fourth argument (i) is not int!");
							if (args[4] is not int j)
								throw new ArgumentException("Fifth argument (j) is not int!");
							if (args[5] is not int size)
								throw new ArgumentException("Sixth argument (size) is not int!");
							ALConvert.Convert(mod, name, i, j, size);
						}
						else if (args.Length == 5)
						{
							if (args[1] is not string fullname)
								throw new ArgumentException("Second argument (fullname) is not string!");
							if (args[2] is not int i)
								throw new ArgumentException("Third argument (i) is not int!");
							if (args[3] is not int j)
								throw new ArgumentException("Fourth argument (j) is not int!");
							if (args[4] is not int size)
								throw new ArgumentException("Fifth argument (size) is not int!");
							ALConvert.Convert(fullname, i, j, size);
						}
						else
						{
							throw new ArgumentException("Arguments cannot be less or more than 5 or 6 in length for Convert");
						}
						return "Success";
					case "addinmimiclist": {
						if (args.Length == 3) {
							if (args[1] is ValueTuple<int, int> mimicType) {
								if (args[2] is Func<bool> condition) {
									if (!MimicSummon.Mimics.TryGetValue(mimicType.Item1, out List<(Func<bool> condition, int npcID)> keyMimics)) MimicSummon.Mimics.TryAdd(mimicType.Item1, keyMimics ??= []);
									keyMimics.Add((condition, mimicType.Item2));
									return "Success";
								}
								throw new ArgumentException("Third argument (condition) is not Func<bool>!");
							}
							throw new ArgumentException("Second argument (mimicType) is not ValueTuple<int, int>!");
						}
						throw new ArgumentException("Incorrect argument count");
					}
					case "adddungeonchest": {
						if (args.Length == 4 || args.Length == 5) {
							if (args[1] is not int chestTileType) {
								if (args[1] is ushort uShortChestTileType) chestTileType = uShortChestTileType;
								else throw new ArgumentException("Second argument (chestTileType) is not int");
							}
							if (args[2] is not int contain) throw new ArgumentException("Third argument (contain) is not int");
							if (args[3] is not int style) throw new ArgumentException("Fourth argument (style) is not int");
							Func<bool> condition = null;
							if (args.Length == 5) {
								condition = args[4] as Func<bool>;
								if (condition is null) throw new ArgumentException("Fifth argument (condition) is not present and not Func<bool>");
							}
							DungeonChests.extraDungeonChests.Add((chestTileType, contain, style, condition));
							return "Success";
						}
						throw new ArgumentException("Incorrect argument count");
					}
					default:
						throw new ArgumentException("Invalid option!");
				}
			}
			return null;
		}

		public override void Unload()
		{
			//AnimatedModIcon.Unload();
			ALTextureAssets.Unload();
			ALConvert.Unload();
			GuideHelpText.Unload();
			ExtractinatorOres.Unload();
			ALReflection.Unload();
			AltLibraryConfig.Config = null;
			TimeHoveringOnIcon = 0;
			HallowBunnyUnlocked = false;
			PreviewWorldIcons = null;
			if (!Main.dedServ)
			{
				Instance = null;
			}
			Biomes = null;
			VanillaBiomes = null;
			AllBiomes = null;
			Ores = null;
			GlobalBiomes = null;
			planteraBulbs = null;
			jungleGrass = null;
			evilStoppingOres = null;
			baseTiles = null;
			ILHooks.Unload();
			ALHooks.Unload();
			AltLibraryServerConfig.Config = null;
			ALBiomeTileCountModifiers = null;
			ReflectionDictionary.Unload();
		}


		public static IReadOnlyList<AltBiome> GetAltBiomes() => Biomes;
		public static AltBiome GetAltBiome(int type) => Biomes.FirstOrDefault(x => x.Type == type) ?? VanillaBiomes.FirstOrDefault(x => x.Type == type);
		public static int AltBiomeType<T>() where T : AltBiome => ModContent.GetInstance<T>()?.Type ?? 0;
		public static IReadOnlyList<AltOre> GetAltOres() => Ores;
		public static AltOre GetAltOre(int type) => Ores.FirstOrDefault(x => x.Type == type);
		public static int AltAltOre<T>() where T : AltOre => ModContent.GetInstance<T>()?.Type ?? 0;

		internal readonly struct CustomPreviews
		{
			internal readonly string seed;
			internal readonly string pathSmall;
			internal readonly string pathMedium;
			internal readonly string pathLarge;

			internal CustomPreviews(string seed, string pathSmall, string pathMedium, string pathLarge)
			{
				if (seed is null)
				{
					throw new ArgumentNullException(nameof(seed), "Cannot be null!");
				}
				if (pathSmall is null)
				{
					throw new ArgumentNullException(nameof(pathSmall), "Cannot be null!");
				}
				if (pathMedium is null)
				{
					throw new ArgumentNullException(nameof(pathMedium), "Cannot be null!");
				}
				if (pathLarge is null)
				{
					throw new ArgumentNullException(nameof(pathLarge), "Cannot be null!");
				}

				this.seed = seed;
				this.pathSmall = pathSmall;
				this.pathMedium = pathMedium;
				this.pathLarge = pathLarge;
			}
		}
		internal static void DumpIL(MonoMod.Cil.ILContext il) => MonoModHooks.DumpIL(Instance, il);
		internal static void DumpIL(MonoMod.Cil.ILContext il, string fileName) {
			string methodName = il.Method.FullName.Replace(':', '_');
			if (methodName.Contains('?')) {
				string text = methodName;
				int num = methodName.LastIndexOf('?') + 1;
				methodName = text.Substring(num, text.Length - num);
				num = methodName.IndexOf('_') + 1;
				methodName = methodName.Substring(0, num);
			}
			string filePath = Path.Combine(Logging.LogDir, "ILDumps", Instance.Name, methodName + fileName + ".txt");
			string folderPath = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(folderPath)) {
				Directory.CreateDirectory(folderPath);
			}
			File.WriteAllText(filePath, il.ToString());
		}
		static long lastLogTime = int.MinValue;
		static readonly StringBuilder logSummary = new();
		internal static void RateLimitedLog(string full, string compressed) {
			if (lastLogTime < Main.GameUpdateCount - 60) {
				Instance.Logger.Info($"{logSummary};\n{full}");
				lastLogTime = Main.GameUpdateCount;
				logSummary.Clear();
			} else {
				logSummary.Append(compressed);
			}
		}
		// for DevHelper
		static string DevHelpBrokenReason {
			get {
#if DEBUG
				return "Mod was last built in DEBUG configuration";
#endif
				return null;
			}
		}
	}
}
