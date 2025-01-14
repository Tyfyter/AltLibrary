using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader;
using Terraria.UI;
using PegasusLib;

namespace AltLibrary.Core
{
	internal class ALReflection
	{
		internal static WorldGenScanTileColumnAndRemoveClumps WorldGen_ScanTileColumnAndRemoveClumps = null;
		internal static FastFieldInfo<UIList, UIElement> UIList__innerList = null;
		internal static FieldInfo UIWorldCreation__evilButtons = null;
		internal static PropertyInfo ModType_Mod = null;
		internal static FastFieldInfo<NPCShop.Entry, List<Condition>> ShopEntry_conditions = null;
		internal static FastFieldInfo<UIWorldListItem, UIElement> UIWorldListItem__worldIcon = null;
		internal static FastFieldInfo<Recipe, bool> Recipe_alchemy = null;


		internal delegate void WorldGenScanTileColumnAndRemoveClumps(int x);

		internal static void Init()
		{
			WorldGen_ScanTileColumnAndRemoveClumps = typeof(WorldGen).GetMethod("ScanTileColumnAndRemoveClumps", BindingFlags.NonPublic | BindingFlags.Static, new Type[] { typeof(int) }).CreateDelegate<WorldGenScanTileColumnAndRemoveClumps>();
			UIList__innerList = new("_innerList", BindingFlags.NonPublic);
			ModType_Mod = typeof(ModType).GetProperty("Mod");
			UIWorldCreation__evilButtons = typeof(UIWorldCreation).GetField("_evilButtons", BindingFlags.NonPublic | BindingFlags.Instance);
			ShopEntry_conditions = new("conditions", BindingFlags.NonPublic);
			UIWorldListItem__worldIcon = new("_worldIcon", BindingFlags.NonPublic);
			Recipe_alchemy = new("alchemy", BindingFlags.NonPublic);
		}

		internal static void Unload()
		{
			WorldGen_ScanTileColumnAndRemoveClumps = null;
			UIList__innerList = null;
			ModType_Mod = null;
			UIWorldCreation__evilButtons = null;
			ShopEntry_conditions = null;
			UIWorldListItem__worldIcon = null;
			Recipe_alchemy = null;
		}
	}
	[Obsolete("Use PegasusLib")]
	public class FastFieldInfo<TParent, T> {
		public readonly FieldInfo field;
		Func<TParent, T> getter;
		Action<TParent, T> setter;
		public FastFieldInfo(string name, BindingFlags bindingFlags, bool init = false) {
			field = typeof(TParent).GetField(name, bindingFlags | BindingFlags.Instance);
			if (field is null) throw new ArgumentException($"could not find {name} in type {typeof(TParent)} with flags {bindingFlags.ToString()}");
			if (init) {
				getter = CreateGetter();
				setter = CreateSetter();
			}
		}
		public FastFieldInfo(FieldInfo field, bool init = false) {
			this.field = field;
			if (init) {
				getter = CreateGetter();
				setter = CreateSetter();
			}
		}
		public T GetValue(TParent parent) {
			return (getter ??= CreateGetter())(parent);
		}
		public void SetValue(TParent parent, T value) {
			(setter ??= CreateSetter())(parent, value);
		}
		private Func<TParent, T> CreateGetter() {
			if (field.FieldType != typeof(T)) throw new InvalidOperationException($"type of {field.Name} does not match provided type {typeof(T)}");
			string methodName = field.ReflectedType.FullName + ".get_" + field.Name;
			DynamicMethod getterMethod = new DynamicMethod(methodName, typeof(T), new Type[] { typeof(TParent) }, true);
			ILGenerator gen = getterMethod.GetILGenerator();

			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, field);
			gen.Emit(OpCodes.Ret);

			return (Func<TParent, T>)getterMethod.CreateDelegate(typeof(Func<TParent, T>));
		}
		private Action<TParent, T> CreateSetter() {
			if (field.FieldType != typeof(T)) throw new InvalidOperationException($"type of {field.Name} does not match provided type {typeof(T)}");
			string methodName = field.ReflectedType.FullName + ".set_" + field.Name;
			DynamicMethod setterMethod = new DynamicMethod(methodName, null, new Type[] { typeof(TParent), typeof(T) }, true);
			ILGenerator gen = setterMethod.GetILGenerator();

			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Stfld, field);
			gen.Emit(OpCodes.Ret);

			return (Action<TParent, T>)setterMethod.CreateDelegate(typeof(Action<TParent, T>));
		}
	}
}
