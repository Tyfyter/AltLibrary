using AltLibrary.Content.DevArmor.Cace;
using AltLibrary.Content.DevArmor.Fox;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AltLibrary.Common {
	internal class DevArmorFromBags : GlobalItem {
		public override void Load() {
			On_Player.TryGettingDevArmor += On_Player_TryGettingDevArmor;
		}

		private void On_Player_TryGettingDevArmor(On_Player.orig_TryGettingDevArmor orig, Player self, IEntitySource source) {
			orig(self, source);
			if (Main.rand.NextBool(Main.tenthAnniversaryWorld ? 10 : 20)) {
				switch (Main.rand.Next(3)) {
					case 0:
					self.QuickSpawnItem(source, ModContent.ItemType<FoxMask>());
					self.QuickSpawnItem(source, ModContent.ItemType<FoxMask>());
					self.QuickSpawnItem(source, ModContent.ItemType<FoxMask>());
					break;
					case 1:
					self.QuickSpawnItem(source, ModContent.ItemType<CaceEars>());
					break;
					case 2:
					self.QuickSpawnItem(source, ItemID.ArchaeologistsHat);
					self.QuickSpawnItem(source, ItemID.CowboyJacket);
					self.QuickSpawnItem(source, ItemID.SandBoots);
					break;
				}
			}
		}
	}
}