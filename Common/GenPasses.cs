using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.Systems;
using Humanizer;
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

namespace AltLibrary.Common {
	internal static class GenPasses {
		private static MethodBase ResetInfo;
		private static MethodBase ShiniesInfo;
		private static MethodBase UnderworldInfo;
		private static MethodBase AltarsInfo;
		private static MethodBase MicroBiomesInfo;
		internal static MethodBase SpreadingGrassInfo;
		static FieldInfo _vanillaGenPassesField;
		static FieldInfo _method;

		internal static event ILContext.Manipulator HookGenPassReset {
			add => MonoModHooks.Modify(ResetInfo, value);
			remove { }
		}

		internal static event ILContext.Manipulator HookGenPassShinies {
			add => MonoModHooks.Modify(ShiniesInfo, value);
			remove { }
		}

		internal static event ILContext.Manipulator HookGenPassUnderworld {
			add => MonoModHooks.Modify(UnderworldInfo, value);
			remove { }
		}

		internal static event ILContext.Manipulator HookGenPassAltars {
			add => MonoModHooks.Modify(AltarsInfo, value);
			remove { }
		}

		internal static event ILContext.Manipulator HookGenPassMicroBiomes {
			add => MonoModHooks.Modify(MicroBiomesInfo, value);
			remove { }
		}

		internal static void ILGenerateWorld(ILContext il) {
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
			if (!c.TryGotoNext(i => i.MatchCall(typeof(Utils), nameof(Utils.LogAndChatAndConsoleInfoMessage)))) {
				AltLibrary.Instance.Logger.Error($"Could not find {nameof(Utils.LogAndChatAndConsoleInfoMessage)} call in {nameof(WorldGen)}.{nameof(WorldGen.GenerateWorld)}");
				return;
			}

			c.Index++;
			c.EmitDelegate<Action>(() => {
				Utils.LogAndChatAndConsoleInfoMessage("World alts: Evil - {0} {1}, Tropic - {2} {3}, Underworld - {4} {5}, Good - {6} {7}".FormatWith(
				[
					WorldBiomeManager.WorldEvilBiome.Type < 0 ? (WorldBiomeManager.IsCrimson ? -666 : -333) : WorldBiomeManager.WorldEvilBiome.Type,
					!WorldBiomeManager.IsAnyModdedEvil ? "NONE" : WorldBiomeManager.WorldEvilBiome.DisplayName,
					WorldBiomeManager.WorldJungle?.Type,
					WorldBiomeManager.WorldJungle?.FullName,
					WorldBiomeManager.WorldHell?.Type,
					WorldBiomeManager.WorldHell?.FullName,
					WorldBiomeManager.WorldHallowName == "" ? -1 : WorldBiomeManager.WorldHallowBiome.Type,
					WorldBiomeManager.WorldHallowName == "" ? "NONE" : WorldBiomeManager.WorldHallowBiome.DisplayName,
				]));
			});
		}
		public static void Unload() {
			ResetInfo = null;
			ShiniesInfo = null;
			UnderworldInfo = null;
			AltarsInfo = null;
			MicroBiomesInfo = null;
			SpreadingGrassInfo = null;
		}

		private static MethodInfo GetGenPassInfo(Dictionary<string, GenPass> _vanillaGenPasses, string name) {
			try {
				return (_method.GetValue(_vanillaGenPasses[name] as PassLegacy) as Delegate).GetMethodInfo();
			} catch (KeyNotFoundException e) {
				AltLibrary.Instance.Logger.Error($"Could not find GenPass with name {name}", e);
#if DEBUG
				throw;
#else
				return null;
#endif
			} catch (NullReferenceException e) {
				AltLibrary.Instance.Logger.Error($"GenPass {name} is not a legacy pass", e);
#if DEBUG
				throw;
#else
				return null;
#endif
			}
		}
	}
}
