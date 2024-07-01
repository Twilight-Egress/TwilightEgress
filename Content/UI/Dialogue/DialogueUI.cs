using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static Cascade.Content.UI.Dialogue.DialogueUISystem;
using static Cascade.Content.UI.Dialogue.DialogueTreeID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;
using System.Security.Permissions;

namespace Cascade.Content.UI.Dialogue
{
    internal class DialogueUIState : UIState
    {
        public MouseBlockingUIPanel TextBox;
        public MouseBlockingUIPanel NameBox;
        public UIImage Speaker;
        public Character CurrentSpeaker;

        public int DialogueTreeIndex;
        public int DialogueIndex;
        public bool justOpened = false;
        public override void OnInitialize()
        {
            Dialogue CurrentDialogue = DialogueTrees[DialogueTreeIndex].Dialogues[DialogueIndex];
            if (CurrentDialogue.CharacterIndex != -1)
            {
                CurrentSpeaker = Characters[CurrentDialogue.CharacterIndex];
                string expressionID = CurrentSpeaker.ExpressionIDs[CurrentDialogue.ExpressionIndex];
                Asset <Texture2D> speakerTexture = ModContent.Request<Texture2D>($"{nameof(Cascade)}/Content/UI/Dialogue/CharacterAssets/{CurrentSpeaker.ID}/{CurrentSpeaker.ID}_{expressionID}");
                Speaker = new(speakerTexture);
                Speaker.ImageScale = CurrentSpeaker.Scale;
                if(!justOpened)
                    SetRectangle(Speaker, left: 1500f, top: 500f, width: speakerTexture.Width(), height: speakerTexture.Height());
                else
                    SetRectangle(Speaker, left: 1500f, top: 1200f, width: speakerTexture.Width(), height: speakerTexture.Height());
                Append(Speaker);
            }
            
            TextBox = new MouseBlockingUIPanel();
            TextBox.SetPadding(0);
            if(!justOpened)
                SetRectangle(TextBox, left: 125f, top: 650f, width: 1200f, height: 300f);
            else
                SetRectangle(TextBox, left: 125f, top: 1200f, width: 1200f, height: 300f);
            TextBox.BackgroundColor = new Color(73, 94, 171);
            TextBox.OnLeftClick += OnBoxClick;
            Append(TextBox);

            NameBox = new MouseBlockingUIPanel();
            NameBox.SetPadding(0);
            SetRectangle(NameBox, left: -25f, top: -25f, width: 300f, height: 60f);
            NameBox.BackgroundColor = new Color(73, 94, 171);
            TextBox.Append(NameBox);

            DialogueText DialogueText = new DialogueText();
                DialogueText.boxWidth = TextBox.Width.Pixels;
                DialogueText.Text = CurrentDialogue.Message;
            if (CurrentDialogue.TextDelay != -1)
                DialogueText.textDelay = CurrentDialogue.TextDelay;
            else if (CurrentDialogue.CharacterIndex == -1)
                DialogueText.textDelay = 3;
            else
                DialogueText.textDelay = Characters[CurrentDialogue.CharacterIndex].TextDelay;
            DialogueText.Top.Pixels = 25;
            DialogueText.Left.Pixels = 15;
            TextBox.Append(DialogueText);

            UIText NameText;
            if (CurrentDialogue.CharacterIndex == -1)
                NameText = new UIText("...");
            else
                NameText = new UIText(Characters[CurrentDialogue.CharacterIndex].Name, 1f, true);
            NameText.Width.Pixels = NameBox.Width.Pixels;
            NameText.HAlign = 0.5f;
            NameText.Top.Set(15, 0);
            NameBox.Append(NameText);

            if (CurrentDialogue.Responses != null)
            {
                Response[] availableResponses = CurrentDialogue.Responses.Where(r => r.Requirement).ToArray();
                int responseCount = availableResponses.Count();
                
                for (int i = 0; i < responseCount; i++)
                {
                    UIPanel button = new UIPanel();
                    button.Width.Set(100, 0);
                    button.Height.Set(50, 0);
                    button.HAlign = 1f / (responseCount + 1) * (i + 1);
                    button.Top.Set(2000, 0);
                    button.OnLeftClick += OnButtonClick;
                    TextBox.Append(button);

                    UIText text = new UIText(availableResponses[i].Title);
                    text.HAlign = text.VAlign = 0.5f;
                    button.Append(text);
                }
            }
            justOpened = false;
        }
        private int counter = 0;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ModContent.GetInstance<DialogueUISystem>().isDialogueOpen)
            {
                if (TextBox.Top.Pixels > 650f)
                {
                    TextBox.Top.Pixels -= (TextBox.Top.Pixels - 650f) / 10;
                    if (TextBox.Top.Pixels - 650f < 1)
                        TextBox.Top.Pixels = 650f;

                }
                if (Speaker != null && Speaker.Top.Pixels > 500f)
                {
                    Speaker.Top.Pixels -= (Speaker.Top.Pixels - 500f) / 15;
                    if (TextBox.Top.Pixels - 650f < 1)
                        TextBox.Top.Pixels = 650f;
                }
                DialogueText dialogue = (DialogueText)TextBox.Children.Where(c => c.GetType() == typeof(DialogueText)).First();
                if (!dialogue.crawling)
                {
                    UIElement[] responseButtons = TextBox.Children.Where(c => c.GetType() == typeof(UIPanel)).ToArray();
                    for (int i = 0; i < responseButtons.Length; i++)
                    {
                        UIElement button = responseButtons[i];
                        button.Top.Set(TextBox.Height.Pixels - button.Height.Pixels, 0);
                    }
                }
            }
            else
            {
                if (TextBox.Top.Pixels < 1200f)
                {
                    TextBox.Top.Pixels += (1200f - TextBox.Top.Pixels) / 20;
                    if (1100f - TextBox.Top.Pixels < 10)
                        TextBox.Top.Pixels = 1200f;

                }
                if (Speaker != null && Speaker.Left.Pixels < 2200f)
                {
                    Speaker.Left.Pixels += (2200 - Speaker.Left.Pixels) / 20;
                    if (2100f - Speaker.Left.Pixels < 10)
                        Speaker.Left.Pixels = 2100f;                    
                }
                if ((Speaker == null || Speaker.Left.Pixels == 2100f) && TextBox.Top.Pixels == 1200f)
                    ModContent.GetInstance<DialogueUISystem>().HideDialogueUI();
            }
            counter++;
        }
        private void OnBoxClick(UIMouseEvent evt, UIElement listeningElement)
        {
            DialogueText dialogue = (DialogueText)TextBox.Children.Where(c => c.GetType() == typeof(DialogueText)).First();
            if (DialogueTrees[DialogueTreeIndex].Dialogues[DialogueIndex].Responses == null)
            {
                if(DialogueTrees[DialogueTreeIndex].Dialogues.Length > DialogueIndex + 1)
                    ModContent.GetInstance<DialogueUISystem>().UpdateDialogueUI(DialogueTreeIndex, DialogueIndex + 1);
                else
                    ModContent.GetInstance<DialogueUISystem>().isDialogueOpen = false;
            }
            else if (dialogue.crawling)
            {
                /*
                List<TextSnippet> fullSnippets = ChatManager.ParseMessage(DialogueTrees[DialogueTreeIndex].Dialogues[DialogueIndex].Message, Color.White);
                int textLength = 0;
                for (int i = 0; i < fullSnippets.Count; i++)
                {
                    string text = fullSnippets[i].TextOriginal;
                    if (text.Contains('['))
                        text = fullSnippets[i].Text;
                    textLength += text.Length;
                }
                */
                dialogue.textIndex = DialogueTrees[DialogueTreeIndex].Dialogues[DialogueIndex].Message.Length;
            }
        }
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            int responseCount = DialogueTrees[DialogueTreeIndex].Dialogues[DialogueIndex].Responses.Count();
            int buttonID = (int)(listeningElement.HAlign / (1f / (responseCount + 1)));
            int heading = DialogueTrees[DialogueTreeIndex].Dialogues[DialogueIndex].Responses[buttonID - 1].DialogueIndex;
            if (heading == -1)
                ModContent.GetInstance<DialogueUISystem>().isDialogueOpen = false;
            else if (heading == -2 && DialogueTrees[DialogueTreeIndex].Dialogues.Length > DialogueIndex + 1)
            {
                ModContent.GetInstance<DialogueUISystem>().UpdateDialogueUI(DialogueTreeIndex, DialogueIndex + 1);
            }
            else
                ModContent.GetInstance<DialogueUISystem>().UpdateDialogueUI(DialogueTreeIndex, heading);
        }
        private static void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
        uiElement.Left.Set(left, 0f);
        uiElement.Top.Set(top, 0f);
        uiElement.Width.Set(width, 0f);
        uiElement.Height.Set(height, 0f);
        }
        public class DialogueText : UIElement
        {           
            public string Text = "";
            public bool crawling = true;
            internal float boxWidth = 0f;
            internal int textDelay = 10;
            internal Vector2 textScale = new(1.5f, 1.5f);
            internal int textIndex = 0;
            private int counter = -30;
            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                float xResolutionScale = Main.screenWidth / 2560f;
                float yResolutionScale = Main.screenHeight / 1440f;

                CalculatedStyle innerDimensions = GetInnerDimensions();
                float xPageTop = innerDimensions.X;
                float yPageTop = innerDimensions.Y;               

                int textWidth = (int)((int)((1200f)) - 6f);
                textWidth = (int)(textWidth * xResolutionScale);

                if (++counter % textDelay == 0 && counter >= 0)
                    textIndex++;
                List<TextSnippet> fullSnippets = ChatManager.ParseMessage(Text, Color.White);
                List<TextSnippet> printedSnippets = new List<TextSnippet>();
                if (textIndex < Text.Length)
                {
                    crawling = true;
                    int textLength = 0;
                    for (int i = 0; i < fullSnippets.Count; i++)
                    {
                        string text = fullSnippets[i].TextOriginal;
                        if (text.Contains('['))
                            text = fullSnippets[i].Text;
                        textLength += text.Length;
                    }
                    if (textIndex < textLength)
                    {
                        int CurrentLength = 0;
                        for (int i = 0; i < fullSnippets.Count; i++)
                        {
                            string text = fullSnippets[i].TextOriginal;
                            if (text.Contains('['))
                                text = fullSnippets[i].Text;

                            if (CurrentLength + text.Length < textIndex)
                            {
                                CurrentLength += text.Length;
                                printedSnippets.Add(fullSnippets[i]);
                            }
                            else
                            {
                                string newText = text.Remove(textIndex - CurrentLength);
                                if (newText.Any() && fullSnippets[i].Color != Color.White)
                                {
                                    List<TextSnippet> snippets = ChatManager.ParseMessage($"[C/{fullSnippets[i].Color.Hex3()}:{newText}]", Color.White);
                                    TextSnippet snippet = snippets[0];
                                    printedSnippets.Add(snippet);
                                }
                                else
                                    printedSnippets.Add(new TextSnippet(newText, Color.White));
                                break;
                            }
                        }
                    }
                    else
                    {
                        crawling = false;
                        printedSnippets = fullSnippets;
                    }

                }
                else
                {
                    crawling = false;
                    printedSnippets = fullSnippets;
                }

                string TextToPrint = "";
                for(int i = 0; i < printedSnippets.Count; i++)
                {                   
                    TextToPrint += printedSnippets[i].TextOriginal;
                }
                List<string> textLines = Utils.WordwrapString(TextToPrint, FontAssets.MouseText.Value, (int)((boxWidth + this.Width.Pixels) / 1.5f), 250, out _).ToList();
                textLines.RemoveAll(text => string.IsNullOrEmpty(text));

                string PreviousLineColorHex = "";
                for (int i = 0; i < textLines.Count; i++)
                {
                    int LeftBracketCounter = 0;
                    int RightBracketCounter = 0;
                    bool gettingHex = false;
                    for(int j = 0; j < textLines[i].Length; j++)
                    {
                        if (textLines[i][j] == '[')
                            LeftBracketCounter++;
                        if (textLines[i][j] == ']')
                            RightBracketCounter++;
                    }
                    if (LeftBracketCounter != RightBracketCounter)
                    {
                        //Main.NewText("Line Imbalance at line: " + i);
                        for (int j = 0; j < textLines[i].Length; j++)
                        {
                            if (LeftBracketCounter > RightBracketCounter)
                            {
                                if (gettingHex && textLines[i][j] != ':')
                                    PreviousLineColorHex += textLines[i][j];
                                if (textLines[i][j] == '/')
                                    gettingHex = true;
                                else if (textLines[i][j] == ':')
                                    gettingHex = false;
                            }
                        }
                        if (LeftBracketCounter > RightBracketCounter)
                        {
                            //Main.NewText("Closing Bracket needed!");
                            textLines[i] += ']';                           
                        }
                        else
                        {
                            //Main.NewText("Color Tag needed!");
                            if (PreviousLineColorHex.Length > 6)
                                PreviousLineColorHex = PreviousLineColorHex.Remove(6);
                            string newText = $"[C/{PreviousLineColorHex}:" + textLines[i];
                            //Main.NewText("Added Tag: " + PreviousLineColorHex);
                            textLines[i] = newText;
                        }
                        //Main.NewText("Updated line: " + textLines[i]);
                    }
                }

                List<List<TextSnippet>> dialogLines = new List<List<TextSnippet>>();
                for (int i = 0; i < textLines.Count; i++)
                {
                    dialogLines.Add(ChatManager.ParseMessage(textLines[i], Color.White));
                }
                
                float yOffsetPerLine = 32f * textScale.Y;
                int yScale = (int)(42 * yResolutionScale);
                int yScale2 = (int)(yOffsetPerLine * yResolutionScale);
                for (int i = 0; i < dialogLines.Count; i++)
                    if (dialogLines[i] != null)
                    {
                        int textDrawPositionY = yScale + i * yScale2 + (int)yPageTop;
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, dialogLines[i].ToArray(), new Vector2(xPageTop, textDrawPositionY), 0f, Vector2.Zero, textScale, out int hoveredSnippet);
                        //Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, dialogLines[i], xPageTop, textDrawPositionY, Color.White, Color.Black, Vector2.Zero, 1.5f);
                    }
            }
        }
    }
    //CalamityUtils.GetTextValue("UI.RevengeanceExpandedInfo");
}
