using PegasusLib;
using PegasusLib.Reflection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terraria.ModLoader;
using static Terraria.ModLoader.WallLoader;

namespace AltLibrary.Core.Reflection {
	[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Members assigned via reflection")]
	internal class WallLoaderReflection : ReflectionLoader {
		[ReflectionParentType(typeof(WallLoader))]
		static FastStaticFieldInfo<List<ConvertWall>[][]> wallConversionDelegates;
		public static bool HasConversion(int tileType, int conversionType) => wallConversionDelegates.Value[tileType]?[conversionType]?.Count > 0;
	}
}
