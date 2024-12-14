﻿using Terraria;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.Dedicated.Lynel
{
    public class EarmuffFruit : ModItem
    {
        public new string LocalizationCategory => "Items.Dedicated";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.DefaultToVanitypet(ModContent.ProjectileType<EarPiercingBellbird>(), ModContent.BuffType<BellbirdBuff>());
        }

        public override bool? UseItem(Player player)
        {
            player.AddBuff(Item.buffType, 2);
            return base.UseItem(player);
        }
    }
}
