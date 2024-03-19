using AltLibrary.Common.Systems;
using Mono.Cecil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AltLibrary.Common
{
	internal static class GenPasses
	{
		private static MethodBase ResetInfo;
		private static MethodBase ShiniesInfo;
		private static MethodBase UnderworldInfo;
		private static MethodBase AltarsInfo;
		private static MethodBase MicroBiomesInfo;
		internal static MethodBase SpreadingGrassInfo;
		static FieldInfo _vanillaGenPassesField;
		static FieldInfo _method;

		internal static event ILContext.Manipulator HookGenPassReset
		{
			add => MonoModHooks.Modify(ResetInfo, value);
			remove { }
		}

		internal static event ILContext.Manipulator HookGenPassShinies
		{
			add => MonoModHooks.Modify(ShiniesInfo, value);
			remove { }
		}

		internal static event ILContext.Manipulator HookGenPassUnderworld
		{
			add => MonoModHooks.Modify(UnderworldInfo, value);
			remove { }
		}

		internal static event ILContext.Manipulator HookGenPassAltars
		{
			add => MonoModHooks.Modify(AltarsInfo, value);
			remove { }
		}

		internal static event ILContext.Manipulator HookGenPassMicroBiomes
		{
			add => MonoModHooks.Modify(MicroBiomesInfo, value);
			remove { }
		}

		internal static void ILGenerateWorld(ILContext il)
		{
			_vanillaGenPassesField = typeof(WorldGen).GetField("_vanillaGenPasses", BindingFlags.NonPublic | BindingFlags.Static);
			Dictionary<string, GenPass> _vanillaGenPasses = (Dictionary<string, GenPass>)_vanillaGenPassesField.GetValue(null);
			_method = typeof(PassLegacy).GetField("_method", BindingFlags.NonPublic | BindingFlags.Instance);
			ResetInfo = GetGenPassInfo(_vanillaGenPasses, "Reset");
			ShiniesInfo = GetGenPassInfo(_vanillaGenPasses, "Shinies");
			UnderworldInfo = GetGenPassInfo(_vanillaGenPasses, "Underworld");
			AltarsInfo = GetGenPassInfo(_vanillaGenPasses, "Altars");
			MicroBiomesInfo = GetGenPassInfo(_vanillaGenPasses, "Micro Biomes");
			SpreadingGrassInfo = GetGenPassInfo(_vanillaGenPasses, "Spreading Grass");

			ILCursor c = new(il);
			if (!c.TryGotoNext(i => i.MatchCall(typeof(Console).GetMethod(nameof(Console.WriteLine), BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(string), typeof(object[]) }))))
			{
				AltLibrary.Instance.Logger.Info("11 $ 1");
				return;
			}

			c.Index++;
			c.EmitDelegate<Action>(() =>
			{
				Console.WriteLine("World alts: Evil - {0} {1}, Tropic - {2} {3}, Underworld - {4} {5}, Good - {6} {7}", new object[]
				{
					WorldBiomeManager.WorldEvilBiome.Type < 0 ? (WorldBiomeManager.IsCrimson ? -666 : -333) : WorldBiomeManager.WorldEvilBiome.Type,
					!WorldBiomeManager.IsAnyModdedEvil ? "NONE" : WorldBiomeManager.WorldEvilBiome.DisplayName,
					WorldBiomeManager.WorldJungle == "" ? -1 : AltLibrary.Biomes.Find(x => x.FullName == WorldBiomeManager.WorldJungle).Type,
					WorldBiomeManager.WorldJungle == "" ? "NONE" : WorldBiomeManager.WorldJungle,
					WorldBiomeManager.WorldHell == "" ? -1 : AltLibrary.Biomes.Find(x => x.FullName == WorldBiomeManager.WorldHell).Type,
					WorldBiomeManager.WorldHell == "" ? "NONE" : WorldBiomeManager.WorldHell,
					WorldBiomeManager.WorldHallowName == "" ? -1 : WorldBiomeManager.WorldHallowBiome.Type,
					WorldBiomeManager.WorldHallowName == "" ? "NONE" : WorldBiomeManager.WorldHallowBiome.DisplayName,
				});
			});
		}
		public static void Unload()
		{
			ResetInfo = null;
			ShiniesInfo = null;
			UnderworldInfo = null;
			AltarsInfo = null;
			MicroBiomesInfo = null;
			SpreadingGrassInfo = null;
		}

		private static MethodBase GetGenPassInfo(Dictionary<string, GenPass> _vanillaGenPasses, string name)
		{
			try {
				return (_method.GetValue(_vanillaGenPasses[name] as PassLegacy) as Delegate).GetMethodInfo();
			}
			catch (KeyNotFoundException e) {
				AltLibrary.Instance.Logger.Error($"Could not find GenPass with name {name}", e);
				return null;
			} catch (NullReferenceException e) {
				AltLibrary.Instance.Logger.Error($"GenPass {name} is not a legacy pass", e);
				return null;
			}
		}
	}
}
