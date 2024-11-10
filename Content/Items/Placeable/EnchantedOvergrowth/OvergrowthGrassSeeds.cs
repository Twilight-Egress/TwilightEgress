<<<<<<<< HEAD:Content/Items/EnchantedOvergrowth/ManaGrassSeeds.cs
﻿using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.EnchantedOvergrowth
========
﻿namespace TwilightEgress.Content.Items.Placeable.EnchantedOvergrowth
>>>>>>>> 3546090 (add overgrowth dirt):Content/Items/Placeable/EnchantedOvergrowth/OvergrowthGrassSeeds.cs
{
    public class OvergrowthGrassSeeds : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.maxStack = 9999;
        }
    }
}
