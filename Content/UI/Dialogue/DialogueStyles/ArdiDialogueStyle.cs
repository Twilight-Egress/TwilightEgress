using static Cascade.Content.UI.Dialogue.DialogueUIState;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Cascade.Content.UI.Dialogue.UIElements;

namespace Cascade.Content.UI.Dialogue.DialogueStyles
{
    public class ArdiDialogueStyle : BaseDialogueStyle
    {        
        public override void OnTextboxCreate(UIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            bool speakerRight = ModContent.GetInstance<DialogueUISystem>().speakerRight;
            bool justOpened = ModContent.GetInstance<DialogueUISystem>().justOpened;
            bool newSubSpeaker = ModContent.GetInstance<DialogueUISystem>().newSubSpeaker;
            textbox.SetPadding(0f);
            float startX;
            if (newSubSpeaker)
                startX = speakerRight ? 500f : 600f;
            else
                startX = speakerRight ? 600f : 500f;
            SetRectangle(textbox, left: startX, top: justOpened ? 1200f : 500f, width: 600f, height: 200f);

            // No auto-drawn Terraria textbox cause this one is entirely drawn with prims.
            textbox.BackgroundColor = Color.Transparent;
            textbox.BorderColor = Color.Transparent;

            ArdienaTextboxPrimitives textboxPrims = new();
            textbox.Append(textboxPrims);
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

        public override void PostUpdateActive(MouseBlockingUIPanel textbox, UIImage speaker, UIImage subSpeaker)
        {
            if (textbox.Top.Pixels > 500f)
            {
                textbox.Top.Pixels -= (textbox.Top.Pixels - 500f) / 10;
                if (textbox.Top.Pixels - 500f < 1)
                    textbox.Top.Pixels = 500f;

            }
            if (!ModContent.GetInstance<DialogueUISystem>().speakerRight && textbox.Left.Pixels > 500f)
            {
                textbox.Left.Pixels -= (textbox.Left.Pixels - 500f) / 20;
                if (textbox.Left.Pixels - 500f < 1)
                    textbox.Left.Pixels = 500f;
            }
            else if (ModContent.GetInstance<DialogueUISystem>().speakerRight && textbox.Left.Pixels < 600f)
            {
                textbox.Left.Pixels += (600f - textbox.Left.Pixels) / 20;
                if (600f - textbox.Left.Pixels < 1)
                    textbox.Left.Pixels = 600f;
            }

            DialogueText dialogue = (DialogueText)textbox.Children.Where(c => c.GetType() == typeof(DialogueText)).First();
            UIElement[] responseButtons = ModContent.GetInstance<DialogueUISystem>().DialogueUIState.Children.Where(c => c.GetType() == typeof(UIPanel) && c.Children.First().GetType() == typeof(UIText)).ToArray();
            for (int i = 0; i < responseButtons.Length; i++)
            {
                UIElement button = responseButtons[i];
                if (!dialogue.crawling && button.Width.Pixels < 100)
                {
                    Vector2 rotation = Vector2.UnitY;
                    rotation = rotation.RotatedBy(TwoPi / responseButtons.Length * i);
                    button.HAlign = 0f;
                    button.Top.Set(textbox.Top.Pixels + (textbox.Height.Pixels / 2 - button.Height.Pixels / 2), 0);
                    button.Left.Set(textbox.Left.Pixels + (textbox.Width.Pixels / 2 - button.Width.Pixels / 2), 0);                   

                    button.Top.Pixels -= rotation.Y * (textbox.Height.Pixels/1.5f);
                    button.Left.Pixels += rotation.X * (textbox.Width.Pixels/1.5f);

                    button.Width.Pixels += 2;
                    button.Height.Pixels += 1;

                    if (button.Children.Any())
                    {
                        UIText oldText = (UIText)button.Children.First();
                        button.RemoveAllChildren();

                        UIText text = new UIText(oldText.Text, button.Width.Pixels / 100);
                        text.HAlign = 0.5f;
                        text.VAlign = 0.5f;
                        button.Append(text);
                    }
                }
                if (button.ContainsPoint(Main.MouseScreen))
                    Main.LocalPlayer.mouseInterface = true;
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
        public override bool TextboxOffScreen(UIPanel textbox) => textbox.Top.Pixels == 1200f;
    }
}
