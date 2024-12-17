﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Content.UI.Dialogue
{
    public class DialogueMusicPlayer : ModPlayer
    {
        public override void PostUpdateEquips()
        {
            if (ModContent.GetInstance<DialogueUISystem>() != null && !ModContent.GetInstance<DialogueUISystem>().isDialogueOpen)
                return;
            
            DialogueUISystem dialogueUISystem = ModContent.GetInstance<DialogueUISystem>();
            DialogueUIState UI = dialogueUISystem.DialogueUIState;
            Dialogue CurrentDialogue = DialogueHolder.DialogueTrees[UI.DialogueTreeIndex].Dialogues[UI.DialogueIndex];
            if (CurrentDialogue.MusicID == -1 || !(!Main.gameMenu && !Main.dedServ))
                return;
            
            Main.musicBox2 = CurrentDialogue.MusicID;
        }
    }
}
