﻿using static Cascade.Content.UI.Dialogue.DialogueUIState;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader.UI;

namespace Cascade.Content.UI.Dialogue.DialogueStyles
{
    public class DefaultDialogueStyle : BaseDialogueStyle
    {
        public override void OnTextboxCreate(UIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            bool speakerRight = ModContent.GetInstance<DialogueUISystem>().speakerRight;
            bool spawnBottom = ModContent.GetInstance<DialogueUISystem>().justOpened || ModContent.GetInstance<DialogueUISystem>().styleSwapped;
            bool newSubSpeaker = ModContent.GetInstance<DialogueUISystem>().newSubSpeaker;

            textbox.SetPadding(0);
            float startX = 0;
            if (newSubSpeaker && !ModContent.GetInstance<DialogueUISystem>().styleSwapped)
                startX = speakerRight ? 600f : 125f;
            else
                startX = speakerRight ? 125f : 600f;
            SetRectangle(textbox, left: startX, top: spawnBottom ? 1200f : 650f, width: 1200f, height: 300f);

            textbox.BackgroundColor = BackgroundColor;
        }
        public override void OnDialogueTextCreate(DialogueText text)
        {
            text.Top.Pixels = 25;
            text.Left.Pixels = 15;
        }
        public override void OnResponseButtonCreate(UIPanel button, MouseBlockingUIPanel textbox, int responseCount, int i)
        {
            button.Width.Set(20, 0);
            button.Height.Set(10, 0);
            button.HAlign = 1f / (responseCount + 1) * (i + 1);
            button.Top.Set(2000, 0);
        }
        public override void OnResponseTextCreate(UIText text)
        {
            text.HAlign = text.VAlign = 0.5f;
        }
        public override void OnResponseCostCreate(UIText text, UIPanel costHolder)
        {
            text.VAlign = 0f;
            costHolder.HAlign = 0.5f;
        }
        public override void PostUICreate(string treeKey, int dialogueIndex, UIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            DialogueTree CurrentTree = DialogueHolder.DialogueTrees[treeKey];
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
        public override void PostUpdateActive(MouseBlockingUIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            if (ModContent.GetInstance<DialogueUISystem>().swappingStyle)
            {
                if (!TextboxOffScreen(textbox))
                {
                    textbox.Top.Pixels += (1200f - textbox.Top.Pixels) / 20;
                    if (1100f - textbox.Top.Pixels < 10)
                        textbox.Top.Pixels = 1200f;
                }
                else
                {
                    ModContent.GetInstance<DialogueUISystem>().styleSwapped = true;
                    ModContent.GetInstance<DialogueUISystem>().swappingStyle = false;
                    textbox.RemoveAllChildren();
                    textbox.Remove();

                    ModContent.GetInstance<DialogueUISystem>().DialogueUIState.SpawnTextBox(this);
                }
            }
            else
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

                DialogueText dialogue = (DialogueText)textbox.Children.Where(c => c.GetType() == typeof(DialogueText)).First();
                UIElement[] responseButtons;
                if (ModContent.GetInstance<DialogueUISystem>().DialogueUIState.Children.Where(c => c.GetType() == typeof(UIPanel) && c.Children.First().GetType() == typeof(UIText)).Any())
                    responseButtons = ModContent.GetInstance<DialogueUISystem>().DialogueUIState.Children.Where(c => c.GetType() == typeof(UIPanel) && c.Children.First().GetType() == typeof(UIText)).ToArray();
                else
                    responseButtons = textbox.Children.Where(c => c.GetType() == typeof(UIPanel)).ToArray();
                for (int i = 0; i < responseButtons.Length; i++)
                {
                    UIElement button = responseButtons[i];
                    if (!dialogue.crawling && button.Width.Pixels < ButtonSize.X)
                    {
                        if (!textbox.HasChild(button))
                            textbox.AddOrRemoveChild(button, true);
                        button.Top.Set(0, 0);
                        button.HAlign = 1f / (responseButtons.Length + 1) * (i + 1);
                        button.VAlign = 0.8f;
                        button.Width.Pixels += ButtonSize.X / 50;
                        button.Height.Pixels += ButtonSize.Y / 50;
                        button.Top.Pixels += button.Height.Pixels / 2;

                        foreach (UIElement child in button.Children)
                        {
                            if (child.GetType() == typeof(UIText))
                            {
                                UIText textChild = (UIText)child;
                                textChild.SetText(textChild.Text, button.Width.Pixels / 100f, false);
                                if (button.Children.Count() > 1)
                                    textChild.Top.Pixels = -4;
                            }
                            else
                                child.Top.Pixels = -2500;
                        }
                    }
                    else if (button.Children.Count() > 1)
                    {
                        UIElement child = (UIElement)button.Children.Where(c => c.GetType() == typeof(UIPanel)).First();
                        child.Top.Pixels = child.Parent.Height.Pixels / 4;
                    }
                }
            }
        }
        public override void PostUpdateClosing(MouseBlockingUIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            if (!TextboxOffScreen(textbox))
            {
                textbox.Top.Pixels += (1200f - textbox.Top.Pixels) / 20;
                if (1100f - textbox.Top.Pixels < 10)
                    textbox.Top.Pixels = 1200f;
            }

        }
        public override bool TextboxOffScreen(UIPanel textbox)
        {
            return textbox.Top.Pixels == 1200f;
        }
    }
}
