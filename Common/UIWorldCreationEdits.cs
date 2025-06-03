using AltLibrary.Common.AltBiomes;
using AltLibrary.Common.AltOres;
using AltLibrary.Common.Systems;
using AltLibrary.Core;
using AltLibrary.Core.Baking;
using AltLibrary.Core.UIs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Gamepad;
using static Terraria.ModLoader.ModContent;

namespace AltLibrary.Common
{
	public readonly struct ALDrawingStruct<T> where T : ModType
	{
		public readonly string UniqueID;
		internal readonly int type;
		internal readonly Func<Asset<Texture2D>, Asset<Texture2D>> func;
		internal readonly Func<Rectangle?> rect;
		internal readonly Func<string> onHoverName;
		internal readonly Func<string, string> onHoverMod;

		public ALDrawingStruct(string ID, int type, Func<Asset<Texture2D>, Asset<Texture2D>> func, Func<Rectangle?> rect, Func<string> onHoverName, Func<string, string> onHoverMod)
		{
			UniqueID = ID;
			this.type = type;
			this.func = func;
			this.rect = rect;
			this.onHoverName = onHoverName;
			this.onHoverMod = onHoverMod;
		}

		public ALDrawingStruct(ModType type, int enumType, Func<Asset<Texture2D>, Asset<Texture2D>> func, Func<Rectangle?> rect, Func<string> onHoverName, Func<string, string> onHoverMod)
		{
			UniqueID = type.FullName;
			this.type = enumType;
			this.func = func;
			this.rect = rect;
			this.onHoverName = onHoverName;
			this.onHoverMod = onHoverMod;
		}
	}
}
