using static Cascade.Content.UI.Dialogue.DialogueUIState;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Cascade.Content.UI.Dialogue.DialogueStyles
{
    public class DefaultDialogueStyle : BaseDialogueStyle
    {
        public override void PostUpdateActive(MouseBlockingUIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            if (textbox.Top.Pixels > 650f)
            {
                textbox.Top.Pixels -= (textbox.Top.Pixels - 650f) / 10;
                if (textbox.Top.Pixels - 650f < 1)
                    textbox.Top.Pixels = 650f;

            }
            if (ModContent.GetInstance<DialogueUISystem>().speakerRight && textbox.Left.Pixels > 125f)
            {
                textbox.Left.Pixels -= (textbox.Left.Pixels - 125f) / 20;
                if (textbox.Left.Pixels - 125f < 1)
                    textbox.Left.Pixels = 125f;
            }
            else if (!ModContent.GetInstance<DialogueUISystem>().speakerRight && textbox.Left.Pixels < 600f)
            {
                textbox.Left.Pixels += (600f - textbox.Left.Pixels) / 20;
                if (600f - textbox.Left.Pixels < 1)
                    textbox.Left.Pixels = 600f;
            }
            if (speaker != null)
            {
                if (speaker.Top.Pixels > 500f)
                {
                    speaker.Top.Pixels -= (speaker.Top.Pixels - 500f) / 15;
                    if (textbox.Top.Pixels - 650f < 1)
                        textbox.Top.Pixels = 650f;
                }
                if (ModContent.GetInstance<DialogueUISystem>().speakerRight && speaker.Left.Pixels > 1500f)
                {
                    speaker.Left.Pixels -= (speaker.Left.Pixels - 1500f) / 20;
                    if (speaker.Left.Pixels - 1500f < 1)
                        speaker.Left.Pixels = 1500f;
                }
                else if (!ModContent.GetInstance<DialogueUISystem>().speakerRight && speaker.Left.Pixels < 200f)
                {
                    speaker.Left.Pixels += (200f - speaker.Left.Pixels) / 20;
                    if (200f - speaker.Left.Pixels < 1)
                        speaker.Left.Pixels = 200f;
                }
            }
            if (subSpeaker != null)
            {
                if (ModContent.GetInstance<DialogueUISystem>().speakerRight && subSpeaker.Left.Pixels > 0f)
                {
                    subSpeaker.Left.Pixels -= (subSpeaker.Left.Pixels - 0f) / 20;
                    if (subSpeaker.Left.Pixels - 0f < 1)
                        subSpeaker.Left.Pixels = 0f;
                }
                else if (!ModContent.GetInstance<DialogueUISystem>().speakerRight && subSpeaker.Left.Pixels < 1700f)
                {
                    subSpeaker.Left.Pixels += (1700f - subSpeaker.Left.Pixels) / 20;
                    if (1700f - subSpeaker.Left.Pixels < 1)
                        subSpeaker.Left.Pixels = 1700f;

                }
            }
            DialogueText dialogue = (DialogueText)textbox.Children.Where(c => c.GetType() == typeof(DialogueText)).First();
            if (!dialogue.crawling)
            {
                UIElement[] responseButtons = textbox.Children.Where(c => c.GetType() == typeof(UIPanel)).ToArray();
                for (int i = 0; i < responseButtons.Length; i++)
                {
                    UIElement button = responseButtons[i];
                    button.Top.Set(textbox.Height.Pixels - button.Height.Pixels, 0);
                }
            }
        }
        public override void PostUpdateClosing(MouseBlockingUIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            if (textbox.Top.Pixels < 1200f)
            {
                textbox.Top.Pixels += (1200f - textbox.Top.Pixels) / 20;
                if (1100f - textbox.Top.Pixels < 10)
                    textbox.Top.Pixels = 1200f;

            }
            if (speaker != null)
            {
                if (ModContent.GetInstance<DialogueUISystem>().speakerRight && speaker.Left.Pixels < 2200f)
                {
                    speaker.Left.Pixels += (2200 - speaker.Left.Pixels) / 20;
                    if (2100f - speaker.Left.Pixels < 10)
                        speaker.Left.Pixels = 2100f;
                }
                else if (!ModContent.GetInstance<DialogueUISystem>().speakerRight && speaker.Left.Pixels > -600)
                {
                    speaker.Left.Pixels -= (speaker.Left.Pixels + 600) / 20;
                    if (speaker.Left.Pixels + 500 < 10)
                        speaker.Left.Pixels = -600;
                }
            }
            if (subSpeaker != null)
            {
                if (!ModContent.GetInstance<DialogueUISystem>().speakerRight && subSpeaker.Left.Pixels < 2200f)
                {
                    subSpeaker.Left.Pixels += (2200 - subSpeaker.Left.Pixels) / 20;
                    if (2100f - subSpeaker.Left.Pixels < 10)
                        subSpeaker.Left.Pixels = 2100f;
                }
                else if (ModContent.GetInstance<DialogueUISystem>().speakerRight && subSpeaker.Left.Pixels > -600)
                {
                    subSpeaker.Left.Pixels -= (subSpeaker.Left.Pixels + 600) / 20;
                    if (subSpeaker.Left.Pixels + 500 < 10)
                        subSpeaker.Left.Pixels = -600;
                }
            }
            if
            (
                (speaker == null || (speaker.Left.Pixels == 2100f && ModContent.GetInstance<DialogueUISystem>().speakerRight) || (speaker.Left.Pixels == -600 && !ModContent.GetInstance<DialogueUISystem>().speakerRight))
                &&
                (subSpeaker == null || (subSpeaker.Left.Pixels == 2100f && !ModContent.GetInstance<DialogueUISystem>().speakerRight) || (subSpeaker.Left.Pixels == -600 && ModContent.GetInstance<DialogueUISystem>().speakerRight))
                &&
                textbox.Top.Pixels == 1200f
            )
                ModContent.GetInstance<DialogueUISystem>().HideDialogueUI();
        }
        public override void OnTextboxCreate(UIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            bool speakerRight = ModContent.GetInstance<DialogueUISystem>().speakerRight;
            bool justOpened = ModContent.GetInstance<DialogueUISystem>().justOpened;
            bool newSubSpeaker = ModContent.GetInstance<DialogueUISystem>().newSubSpeaker;

            textbox.SetPadding(0);
            float startX = 0;
            if (newSubSpeaker)
                startX = speakerRight ? 600f : 125f;
            else
                startX = speakerRight ? 125f : 600f;
            SetRectangle(textbox, left: startX, top: justOpened ? 1200f : 650f, width: 1200f, height: 300f);

            textbox.BackgroundColor = new Color(73, 94, 171);
        }
        public override void OnResponseButtonCreate(UIPanel button, int responseCount, int i)
        {
            button.Width.Set(100, 0);
            button.Height.Set(50, 0);
            button.HAlign = 1f / (responseCount + 1) * (i + 1);
            button.Top.Set(2000, 0);

        }
        public override void OnResponseTextCreate(UIText text)
        {
            text.HAlign = text.VAlign = 0.5f; ;
        }
        public override void PostUICreate(int treeIndex, int dialogueIndex, UIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            DialogueTree CurrentTree = DialogueHolder.DialogueTrees[treeIndex];
            Dialogue CurrentDialogue = CurrentTree.Dialogues[dialogueIndex];

            MouseBlockingUIPanel NameBox;
            NameBox = new MouseBlockingUIPanel();
            NameBox.SetPadding(0);
            SetRectangle(NameBox, left: -25f, top: -25f, width: 300f, height: 60f);
            NameBox.BackgroundColor = new Color(73, 94, 171);
            textbox.Append(NameBox);

            UIText NameText;
            if (CurrentDialogue.CharacterIndex == -1)
                NameText = new UIText("...");
            else
                NameText = new UIText(CurrentTree.Characters[CurrentDialogue.CharacterIndex].Name, 1f, true);
            NameText.Width.Pixels = NameBox.Width.Pixels;
            NameText.HAlign = 0.5f;
            NameText.Top.Set(15, 0);
            NameBox.Append(NameText);
        }        
    }
}
