using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static Cascade.Content.UI.Dialogue.DialogueUIState;

namespace Cascade.Content.UI.Dialogue.DialogueStyles
{
    public class BaseDialogueStyle
    {
        #region UI Creation Methods
        public virtual void PreUICreate(string treeIndex, int dialogueIndex)
        {

        }
        public virtual void PreSpeakerCreate(string treeIndex, int dialogueIndex, UIImage speaker)
        {

        }
        public virtual void PostSpeakerCreate(string treeIndex, int dialogueIndex, UIImage speaker)
        {

        }
        public virtual void PreSubSpeakerCreate(string treeIndex, int dialogueIndex, UIImage speaker, UIImage subSpeaker)
        {

        }
        public virtual void PostSubSpeakerCreate(string treeIndex, int dialogueIndex, UIImage speaker, UIImage subSpeaker)
        {

        }
        public virtual void OnTextboxCreate(UIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {

        }
        public virtual void OnDialogueTextCreate(DialogueText text)
        {

        }
        public virtual void OnResponseButtonCreate(UIPanel button, MouseBlockingUIPanel textbox, int responseCount, int buttonCounter)
        {

        }
        public virtual void OnResponseTextCreate(UIText text)
        {

        }
        public virtual void OnResponseCostCreate(UIText text, UIPanel costHolder)
        { }
        public virtual void PostUICreate(string treeIndex, int dialogueIndex, UIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {

        }
        public virtual bool TextboxOffScreen(UIPanel textbox)
        {
            return false;
        }
        #endregion
        #region Update Methods
        public virtual void PostUpdateActive(MouseBlockingUIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {

        }
        public virtual void PostUpdateClosing(MouseBlockingUIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {

        }
        #endregion
    }
}
