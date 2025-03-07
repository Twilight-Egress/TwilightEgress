﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.CosmostoneShowers
{
    public class Cosmostone : ModItem
    {
        public override string LocalizationCategory => "Items.CosmostoneShowers";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 16));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 9999;
            Item.material = true;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.LightBlue.ToVector3());
            if (Item.Center.Y <= Main.maxTilesY - 750f)
                ItemID.Sets.ItemNoGravity[Type] = true;
            else
                ItemID.Sets.ItemNoGravity[Type] = false;
        }
    }
}
