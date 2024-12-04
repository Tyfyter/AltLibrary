using AltLibrary.Content.DevArmor.Cace;
using AltLibrary.Content.DevArmor.Fox;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace AltLibrary.Common
{
	internal class DevArmorFromBags : GlobalItem
	{
		public override void Load() {
			On_Player.TryGettingDevArmor += On_Player_TryGettingDevArmor;
		}

		private void On_Player_TryGettingDevArmor(On_Player.orig_TryGettingDevArmor orig, Player self, IEntitySource source) {
			orig(self, source);
			if (Main.rand.NextBool(Main.tenthAnniversaryWorld ? 10 : 20)) {
				switch (Main.rand.Next(2)) {
					case 0:
					self.QuickSpawnItem(source, ModContent.ItemType<FoxMask>());
					self.QuickSpawnItem(source, ModContent.ItemType<FoxMask>());
					self.QuickSpawnItem(source, ModContent.ItemType<FoxMask>());
					break;
					case 1:
					self.QuickSpawnItem(source, ModContent.ItemType<CaceEars>());
					break;
				}
			}
		}
	}
}