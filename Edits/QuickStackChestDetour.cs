﻿using MagicStorage.Common.Systems;
using Terraria;
using Terraria.ID;

namespace MagicStorage.Edits {
	internal class QuickStackChestDetour : Edit {
		public override void LoadEdits() {
			On.Terraria.Chest.ServerPlaceItem += Chest_ServerPlaceItem;
		}

		public override void UnloadEdits() {
			On.Terraria.Chest.ServerPlaceItem -= Chest_ServerPlaceItem;
		}

		private void Chest_ServerPlaceItem(On.Terraria.Chest.orig_ServerPlaceItem orig, int plr, int slot) {
			Player player = Main.player[plr];

			ref Item item = ref player.inventory[slot];

			//Basically manually doing Chest.ServerPlaceItem, but inserting TryPlaceItemInNearbyStorageSystems before the SendData call
			item = Chest.PutItemInNearbyChest(item, player.Center);

			bool playSound = false;
			bool success = Netcode.TryQuickStackItemIntoNearbyStorageSystems(player, item, ref playSound);

			NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, plr, slot, item.prefix);

			if (success)
				NetHelper.SendQuickStackToStorage(plr);
		}
	}
}