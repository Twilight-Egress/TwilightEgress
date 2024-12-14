using Luminance.Common.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.EntityOverrides.Items.HoneyComb
{
    public class BeeMovementOverride : ItemOverride
    {
        public override int TypeToOverride => ItemID.SweetheartNecklace;
        public override int[] AdditionalOverrideTypes => new int[]
        {
            ItemID.BeeCloak,
            ItemID.HoneyBalloon
        };

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.GetModPlayer<BeeFlightTimeBoostPlayer>().BeeFlightBoost = 2;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Utilities.EditTooltipByNum(0, item, tooltips, (t) => t.Text += "\nIncreases wing flight time by 12%");
        }
    }
}