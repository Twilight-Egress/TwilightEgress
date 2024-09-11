using static DialogueHelper.Content.UI.Dialogue.DialogueUIState;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Cascade.Content.UI.Dialogue.UIElements;
using DialogueHelper.Content.UI.Dialogue;
using DialogueHelper.Content.UI.Dialogue.DialogueStyles;
using DialogueHelper.Content.UI;

namespace Cascade.Content.UI.Dialogue.DialogueStyles
{
    public class ArdiDialogueStyle : BaseDialogueStyle
    {
        public override Color? BackgroundColor => Color.Transparent;
        public override Color? BackgroundBorderColor => Color.Transparent;
        public override Color? ButtonColor => new(144, 115, 225);
        public override Color? ButtonBorderColor => new(247, 135, 89);
        public override void OnTextboxCreate(UIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            bool speakerRight = ModContent.GetInstance<DialogueUISystem>().speakerRight;
            bool spawnBottom = ModContent.GetInstance<DialogueUISystem>().justOpened || ModContent.GetInstance<DialogueUISystem>().styleSwapped;
            bool newSubSpeaker = ModContent.GetInstance<DialogueUISystem>().newSubSpeaker;
            textbox.SetPadding(0f);

            SetRectangle(textbox, left: 0, top: spawnBottom ? Main.screenHeight * 1.1f : Main.screenHeight / 1.75f, width: Main.screenWidth / 2.5f, height: Main.screenHeight / 4f);

            if (newSubSpeaker && !ModContent.GetInstance<DialogueUISystem>().styleSwapped)
                textbox.Left.Pixels = speakerRight ? Main.screenWidth - textbox.Width.Pixels - Main.screenWidth / 5f : Main.screenWidth / 5f;
            else
                textbox.Left.Pixels = speakerRight ? Main.screenWidth / 5f : Main.screenWidth - textbox.Width.Pixels - Main.screenWidth / 5f;

            ArdienaTextboxPrimitives textboxPrims = new();
            textbox.Append(textboxPrims);
            textboxPrims.OnInitialize();
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
        public override void PostUpdateActive(MouseBlockingUIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            if (ModContent.GetInstance<DialogueUISystem>().swappingStyle)
            {
                if (!TextboxOffScreen(textbox))
                {
                    float goalHeight = Main.screenHeight * 1.55f;
                    textbox.Top.Pixels += (goalHeight - textbox.Top.Pixels) / 20;
                    if (goalHeight - textbox.Top.Pixels < 10)
                        textbox.Top.Pixels = goalHeight;
                }
                else
                {
                    ModContent.GetInstance<DialogueUISystem>().styleSwapped = true;
                    ModContent.GetInstance<DialogueUISystem>().swappingStyle = false;
                    textbox.RemoveAllChildren();
                    textbox.Remove();

                    ModContent.GetInstance<DialogueUISystem>().DialogueUIState.SpawnTextBox();
                }
            }
            else
            {
                if (textbox.Top.Pixels > Main.screenHeight / 1.75f)
                {
                    textbox.Top.Pixels -= (textbox.Top.Pixels - Main.screenHeight / 1.75f) / 10;
                    if (textbox.Top.Pixels - Main.screenHeight / 1.75f < 1)
                        textbox.Top.Pixels = Main.screenHeight / 1.75f;

                }
                float goalRight = Main.screenWidth - textbox.Width.Pixels - Main.screenWidth / 5f;
                float goalLeft = Main.screenWidth / 5f;
                if (!ModContent.GetInstance<DialogueUISystem>().speakerRight && textbox.Left.Pixels > goalRight)
                {
                    textbox.Left.Pixels -= (textbox.Left.Pixels - goalRight) / 20;
                    if (textbox.Left.Pixels - goalRight < 1)
                        textbox.Left.Pixels = goalRight;
                }
                else if (ModContent.GetInstance<DialogueUISystem>().speakerRight && textbox.Left.Pixels < goalLeft)
                {
                    textbox.Left.Pixels += (goalLeft - textbox.Left.Pixels) / 20;
                    if (goalLeft - textbox.Left.Pixels < 1)
                        textbox.Left.Pixels = goalLeft;
                }
                DialogueText dialogue = (DialogueText)textbox.Children.Where(c => c.GetType() == typeof(DialogueText)).First();
                UIElement[] responseButtons = ModContent.GetInstance<DialogueUISystem>().DialogueUIState.Children.Where(c => c.GetType() == typeof(UIPanel) && c.Children.First().GetType() == typeof(UIText)).ToArray();
                for (int i = 0; i < responseButtons.Length; i++)
                {
                    UIElement button = responseButtons[i];
                    if (!dialogue.crawling && button.Width.Pixels < ButtonSize.X)
                    {
                        Vector2 rotation = Vector2.UnitY;
                        rotation = rotation.RotatedBy(TwoPi / responseButtons.Length * i);
                        button.HAlign = 0f;
                        button.Top.Set(textbox.Top.Pixels + (textbox.Height.Pixels / 2 - button.Height.Pixels / 2), 0);
                        button.Left.Set(textbox.Left.Pixels + (textbox.Width.Pixels / 2 - button.Width.Pixels / 2), 0);

                        button.Top.Pixels -= rotation.Y * (textbox.Height.Pixels / 1.5f);
                        button.Left.Pixels += rotation.X * (textbox.Width.Pixels / 1.5f);

                        button.Width.Pixels += ButtonSize.X / 50;
                        button.Height.Pixels += ButtonSize.Y / 50;
                        foreach (UIElement child in button.Children)
                        {
                            if (child.GetType() == typeof(UIText))
                            {
                                UIText textChild = (UIText)child;
                                textChild.SetText(textChild.Text, button.Width.Pixels / ButtonSize.X, false);
                                textChild.IsWrapped = false;
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
                    if (button.ContainsPoint(Main.MouseScreen))
                    {
                        Main.LocalPlayer.mouseInterface = true;
                        if (!dialogue.crawling && button.Width.Pixels >= ButtonSize.X && button.Width.Pixels < ButtonSize.X * 1.25f)
                        {
                            Vector2 rotation = Vector2.UnitY;
                            rotation = rotation.RotatedBy(TwoPi / responseButtons.Length * i);
                            button.HAlign = 0f;
                            button.Top.Set(textbox.Top.Pixels + (textbox.Height.Pixels / 2 - button.Height.Pixels / 2), 0);
                            button.Left.Set(textbox.Left.Pixels + (textbox.Width.Pixels / 2 - button.Width.Pixels / 2), 0);

                            button.Top.Pixels -= rotation.Y * (textbox.Height.Pixels / 1.5f);
                            button.Left.Pixels += rotation.X * (textbox.Width.Pixels / 1.5f);

                            button.Width.Pixels += 2f;
                            button.Height.Pixels += 1f;

                            foreach (UIElement child in button.Children)
                            {
                                if (child.GetType() == typeof(UIText))
                                {
                                    UIText textChild = (UIText)child;
                                    textChild.SetText(textChild.Text, button.Width.Pixels / ButtonSize.X, false);
                                    if (button.Children.Count() > 1)
                                        textChild.Top.Pixels = -4;
                                }
                            }
                        }
                    }
                    else if (!dialogue.crawling && button.Width.Pixels > ButtonSize.X)
                    {
                        Vector2 rotation = Vector2.UnitY;
                        rotation = rotation.RotatedBy(TwoPi / responseButtons.Length * i);
                        button.HAlign = 0f;
                        button.Top.Set(textbox.Top.Pixels + (textbox.Height.Pixels / 2 - button.Height.Pixels / 2), 0);
                        button.Left.Set(textbox.Left.Pixels + (textbox.Width.Pixels / 2 - button.Width.Pixels / 2), 0);

                        button.Top.Pixels -= rotation.Y * (textbox.Height.Pixels / 1.5f);
                        button.Left.Pixels += rotation.X * (textbox.Width.Pixels / 1.5f);

                        button.Width.Pixels -= 2f;
                        button.Height.Pixels -= 1f;

                        foreach (UIElement child in button.Children)
                        {
                            if (child.GetType() == typeof(UIText))
                            {
                                UIText textChild = (UIText)child;
                                textChild.SetText(textChild.Text, button.Width.Pixels / ButtonSize.X, false);
                                if (button.Children.Count() > 1)
                                    textChild.Top.Pixels = -4;
                            }
                        }
                    }
                }
            }
        }
        public override void PostUpdateClosing(MouseBlockingUIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            if (!TextboxOffScreen(textbox))
            {
                float goalHeight = Main.screenHeight * 1.15f;
                textbox.Top.Pixels += (goalHeight - textbox.Top.Pixels) / 20;
                if (goalHeight - textbox.Top.Pixels < 10)
                    textbox.Top.Pixels = goalHeight;
            }
            if (ModContent.GetInstance<DialogueUISystem>().DialogueUI?.CurrentState != null && ModContent.GetInstance<DialogueUISystem>().DialogueUIState.Children.Where(c => c.GetType() == typeof(UIPanel) && c.Children.First().GetType() == typeof(UIText)).Any())
            {
                UIElement[] responseButtons = ModContent.GetInstance<DialogueUISystem>().DialogueUIState.Children.Where(c => c.GetType() == typeof(UIPanel) && c.Children.First().GetType() == typeof(UIText)).ToArray();
                for (int i = 0; i < responseButtons.Length; i++)
                {
                    UIElement button = responseButtons[i];
                    Vector2 rotation = Vector2.UnitY;
                    rotation = rotation.RotatedBy(TwoPi / responseButtons.Length * i);
                    button.HAlign = 0f;
                    button.Top.Set(textbox.Top.Pixels + (textbox.Height.Pixels / 2 - button.Height.Pixels /  2), 0);
                    button.Left.Set(textbox.Left.Pixels + (textbox.Width.Pixels / 2 - button.Width.Pixels / 2), 0);

                    button.Top.Pixels -= rotation.Y * (textbox.Height.Pixels / 1.5f);
                    button.Left.Pixels += rotation.X * (textbox.Width.Pixels / 1.5f);
                }
            }
        }
        public override bool TextboxOffScreen(UIPanel textbox) => textbox.Top.Pixels >= Main.screenHeight * 1.1f;
    }
}
