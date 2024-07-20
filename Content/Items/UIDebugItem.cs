using Cascade.Content.UI.Dialogue;

namespace Cascade.Content.Items
{
    public class UIDebugItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Misc";
        public override string Texture => "CalamityMod/Items/Weapons/Magic/LightGodsBrilliance";
        public override void SetStaticDefaults()
        {
            ModContent.GetInstance<DialogueUISystem>().ButtonClick += ClickEffect;
        }
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
            if (ModContent.GetInstance<DialogueUISystem>().isDialogueOpen)
                ModContent.GetInstance<DialogueUISystem>().isDialogueOpen = false;
            else
            {
                ModContent.GetInstance<DialogueUISystem>().DisplayDialogueTree(DialogueHolder.DebugKey);               
            }
            return true;
        }
        private static void ClickEffect(string treeKey, int dialogueID, int buttonID)
        {
            Main.NewText("A button has been clicked!");
        }
    }
}
