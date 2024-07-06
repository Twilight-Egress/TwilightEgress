using CalamityMod.Events;
using Cascade.Content.Events.CosmostoneShowers;
using Cascade.Content.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cascade.Content.UI.Dialogue;

namespace Cascade.Content.Items
{
    public class UIDebugItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Misc";
        public override string Texture => "CalamityMod/Items/Weapons/Magic/LightGodsBrilliance";

        public override void SetDefaults()
        {
            Item.width = 108;
            Item.height = 108;
            Item.noMelee = true;
            Item.useAnimation = 1;
            Item.useTime = 1;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }
        public override bool? UseItem(Player player)
        {
            if(ModContent.GetInstance<DialogueUISystem>().isDialogueOpen)
                ModContent.GetInstance<DialogueUISystem>().isDialogueOpen = false;
            else
                ModContent.GetInstance<DialogueUISystem>().DisplayDialogueTree(DialogueHolder.DebugID);
            return true;
        }
    }
}
