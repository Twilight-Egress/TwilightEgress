using Luminance.Common.Utilities;
using System.Collections.Generic;
using TwilightEgress.Core.Players;

namespace TwilightEgress.Content.EntityOverrides.Items.HoneyComb
{
    public class HoneyCombOverride : ItemOverride
    {
        public override int TypeToOverride => ItemID.HoneyComb;

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
	    {
            player.GetModPlayer<BeeFlightTimeBoostPlayer>().BeeFlightBoost = 1; 
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Utilities.EditTooltipByNum(0, item, tooltips, (t) => t.Text += "\nIncreases wing flight time by 10%");
        }
    }
}