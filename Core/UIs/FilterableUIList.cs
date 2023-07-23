using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace AltLibrary.Core.UIs {
	public class FilterableUIList : UIList {
		public List<UIElement> list = new();
		public static bool AllFilter(UIElement _) => true;
		public void SetFilter(Func<UIElement, bool> filter = null) {
			filter ??= AllFilter;
			Clear();
			AddRange(list.Where(filter));
			/*UIElement innerList = ALReflection.UIList_innerList.GetValue(this);
			innerList.RemoveAllChildren();
			innerList.Recalculate();
			foreach (var item in _items) {
				if (filter(item)) innerList.Append(item);
			}
			UpdateOrder();
			innerList.Recalculate();*/
		}
	}
}
