using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace AltLibrary.Common.Hooks
{
	internal class DrunkCrimsonFix
	{
		public static void Load()
		{
			Terraria.IL_Main.UpdateTime_StartDay += Main_UpdateTime_StartDay;
		}

		public static void Unload()
		{
		}

		private static void Main_UpdateTime_StartDay(ILContext il)
		{
			ILCursor c = new(il);
			if (!c.TryGotoNext(i => i.MatchLdsfld<Main>(nameof(Main.drunkWorld))))
			{
				AltLibrary.Instance.Logger.Info("7 $ 1");
				return;
			}

			ILLabel skipVanilla = c.DefineLabel();

			if (!c.TryGotoNext(i => i.MatchLdsfld<WorldGen>(nameof(WorldGen.crimson))))
			{
				AltLibrary.Instance.Logger.Info("7 $ 2");
				return;
			}

			c.Emit(OpCodes.Br, skipVanilla);

			if (!c.TryGotoNext(i => i.MatchStsfld<WorldGen>(nameof(WorldGen.crimson))))
			{
				AltLibrary.Instance.Logger.Info("7 $ 2");
				return;
			}

			c.Index++;
			c.MarkLabel(skipVanilla);
			c.EmitDelegate<Action>(() =>
			{
				List<int> AllBiomes = new() { -333, -666 };
				AltLibrary.Biomes.Where(x => x.BiomeType == BiomeType.Evil).ToList().ForEach(x => AllBiomes.Add(x.Type));
				int gotIndex = AllBiomes[WorldBiomeManager.drunkIndex % AllBiomes.Count];
				if (gotIndex < 0)
				{
					WorldBiomeManager.WorldEvilName = "";
				}
				else
				{
					WorldBiomeManager.WorldEvilBiome = AltLibrary.Biomes.Find(x => x.Type == gotIndex);
				}
				WorldGen.crimson = gotIndex == -666;
				gotIndex = AllBiomes[(WorldBiomeManager.drunkIndex + 1) % AllBiomes.Count];
				if (gotIndex < 0)
				{
					WorldBiomeManager.DrunkEvil = gotIndex == -666 ? ModContent.GetInstance<CrimsonAltBiome>() : ModContent.GetInstance<CorruptionAltBiome>();
				}
				else
				{
					WorldBiomeManager.DrunkEvil = AltLibrary.Biomes.Find(x => x.Type == gotIndex);
				}
				WorldBiomeManager.drunkIndex++;
				if (WorldBiomeManager.drunkIndex >= AllBiomes.Count)
				{
					WorldBiomeManager.drunkIndex = 0;
				}
			});
		}
	}
}
