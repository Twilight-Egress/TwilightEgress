using Luminance.Common.Utilities;
using System.Collections.Generic;
using TwilightEgress.Core.EntityOverridingSystem;
using TwilightEgress.Core.Players;

namespace TwilightEgress.Content.EntityOverrides.Items.HoneyComb
{
    public class StingerNecklaceOverride : ItemOverride
    {
        public override int TypeToOverride => ItemID.StingerNecklace;

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
	    {
            player.GetModPlayer<BeeFlightTimeBoostPlayer>().BeeFlightBoost = 1; 
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            Utilities.EditTooltipByNum(1, item, tooltips, (t) => t.Text += "\nIncreases wing flight time by 7%");
        }
    }
}