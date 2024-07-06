using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Cascade.Content.UI.Dialogue.DialogueStyles
{
    public class BaseDialogueStyle
    {
        #region UI Creation Methods
        public virtual void PreUICreate(int treeIndex, int dialogueIndex)
        {

        }
        public virtual void PreSpeakerCreate(int treeIndex, int dialogueIndex, UIImage speaker)
        {

        }
        public virtual void PostSpeakerCreate(int treeIndex, int dialogueIndex, UIImage speaker)
        {

        }
        public virtual void PreSubSpeakerCreate(int treeIndex, int dialogueIndex, UIImage speaker, UIImage subSpeaker)
        {

        }
        public virtual void PostSubSpeakerCreate(int treeIndex, int dialogueIndex, UIImage speaker, UIImage subSpeaker)
        {

        }
        public virtual void OnTextboxCreate(UIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {

        }
        public virtual void OnResponseButtonCreate(UIPanel button, int responseCount, int buttonCounter)
        {

        }
        public virtual void OnResponseTextCreate(UIText text)
        {

        }
        public virtual void PostUICreate(int treeIndex, int dialogueIndex, UIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {

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
