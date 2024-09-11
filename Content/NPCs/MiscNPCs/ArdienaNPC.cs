using DialogueHelper.Content.UI.Dialogue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Content.NPCs.MiscNPCs
{
    public class ArdienaNPC : ModNPC
    {
        public new string LocalizationCategory => "NPCs.Misc";

        public override string Texture => "Cascade/Content/NPCs/MiscNPCs/NoxusVision";
        public override void SetDefaults()
        {
            NPC.width = 283;
            NPC.height = 287;
            NPC.lifeMax = 1;
            NPC.defense = 0;
            NPC.damage = 0;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.dontCountMe = true;
            NPC.noGravity = true;
            NPC.dontTakeDamageFromHostiles = true;
        }
        public override bool CanChat() => true;
        public override string GetChat()
        {
            Main.CloseNPCChatOrSign();

            if (ModContent.GetInstance<DialogueUISystem>().isDialogueOpen)
                ModContent.GetInstance<DialogueUISystem>().isDialogueOpen = false;
            else
                ModContent.GetInstance<DialogueUISystem>().DisplayDialogueTree("Cascade/Eeveelutions");

            return ":P";
        }
    }
}
