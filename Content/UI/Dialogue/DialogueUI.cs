using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static Cascade.Content.UI.Dialogue.DialogueHolder;
using Terraria.UI.Chat;
using Cascade.Content.UI.Dialogue.DialogueStyles;

namespace Cascade.Content.UI.Dialogue
{
    internal class DialogueUIState : UIState
    {
        public MouseBlockingUIPanel Textbox;
        public UIImage Speaker;
        public UIImage SubSpeaker;


        public int DialogueTreeIndex;
        public int DialogueIndex;
        public override void OnInitialize()
        {
            if (DialogueTrees != null)
            {
                DialogueTree CurrentTree = DialogueTrees[DialogueTreeIndex];
                Dialogue CurrentDialogue = CurrentTree.Dialogues[DialogueIndex];
                Character CurrentSpeaker;
                Character CurrentSubSpeaker;

                int subSpeakerIndex = -1;
                bool justOpened = true;
                bool newSpeaker = false;
                bool newSubSpeaker = false;
                bool returningSpeaker = false;
                bool speakerRight = true;

                BaseDialogueStyle style = new BaseDialogueStyle();
                if(CurrentDialogue.StyleID == -1)
                    style = new DefaultDialogueStyle();
                else
                    style = new DefaultDialogueStyle();

                if (ModContent.GetInstance<DialogueUISystem>() != null)
                {
                    CurrentSpeaker = ModContent.GetInstance<DialogueUISystem>().CurrentSpeaker;
                    CurrentSubSpeaker = ModContent.GetInstance<DialogueUISystem>().SubSpeaker;
                    subSpeakerIndex = ModContent.GetInstance<DialogueUISystem>().subSpeakerIndex;
                    justOpened = ModContent.GetInstance<DialogueUISystem>().justOpened;
                    newSpeaker = ModContent.GetInstance<DialogueUISystem>().newSpeaker;
                    newSubSpeaker = ModContent.GetInstance<DialogueUISystem>().newSubSpeaker;
                    returningSpeaker = ModContent.GetInstance<DialogueUISystem>().returningSpeaker;
                    speakerRight = ModContent.GetInstance<DialogueUISystem>().speakerRight;
                }
                else
                {
                    CurrentSpeaker = Characters[CurrentDialogue.CharacterIndex];
                    CurrentSubSpeaker = new Character("None", new string[] { "None" });
                }
                style.PreUICreate(DialogueTreeIndex, DialogueIndex);
                if (CurrentDialogue.CharacterIndex != -1)
                {
                    //Main.NewText("Create Speaker: " + CurrentDialogue.CharacterIndex);
                    CurrentSpeaker = CurrentTree.Characters[CurrentDialogue.CharacterIndex];
                    string expressionID = CurrentSpeaker.ExpressionIDs[CurrentDialogue.ExpressionIndex];
                    Asset<Texture2D> speakerTexture = ModContent.Request<Texture2D>($"{nameof(Cascade)}/Content/UI/Dialogue/CharacterAssets/{CurrentSpeaker.ID}/{CurrentSpeaker.ID}_{expressionID}");
                    Speaker = new(speakerTexture);
                    Speaker.ImageScale = CurrentSpeaker.Scale;
                    float startPositionX = 0;
                    if (speakerRight)
                        startPositionX = returningSpeaker ? 1600f : 1500f;
                    else
                        startPositionX = returningSpeaker ? 100f : 200f;

                    if (justOpened || newSpeaker)
                        SetRectangle(Speaker, left: startPositionX, top: 1200f, width: speakerTexture.Width(), height: speakerTexture.Height());
                    else
                        SetRectangle(Speaker, left: startPositionX, top: 500f, width: speakerTexture.Width(), height: speakerTexture.Height());
                    style.PreSpeakerCreate(DialogueTreeIndex, DialogueIndex, Speaker);
                    Append(Speaker);
                    style.PostSpeakerCreate(DialogueTreeIndex, DialogueIndex, Speaker);
                }
                if (subSpeakerIndex != -1)
                {
                    //Main.NewText("Create Sub-Speaker: " + subSpeakerIndex);
                    CurrentSubSpeaker = CurrentTree.Characters[subSpeakerIndex];
                    string expressionID = CurrentSubSpeaker.ExpressionIDs[0];
                    Asset<Texture2D> subSpeakerTexture = ModContent.Request<Texture2D>($"{nameof(Cascade)}/Content/UI/Dialogue/CharacterAssets/{CurrentSubSpeaker.ID}/{CurrentSubSpeaker.ID}_{expressionID}");
                    SubSpeaker = new(subSpeakerTexture);
                    SubSpeaker.ImageScale = CurrentSubSpeaker.Scale;
                    float startPositionX = 0;
                    if (speakerRight)
                        startPositionX = newSpeaker || returningSpeaker ? 200f : 0f;
                    else
                        startPositionX = newSpeaker || returningSpeaker ? 1500f : 1700f;
                    SetRectangle(SubSpeaker, left: startPositionX, top: 500f, width: subSpeakerTexture.Width(), height: subSpeakerTexture.Height());
                    style.PreSubSpeakerCreate(DialogueTreeIndex, DialogueIndex, Speaker, SubSpeaker);
                    Append(SubSpeaker);
                    style.PostSubSpeakerCreate(DialogueTreeIndex, DialogueIndex, Speaker, SubSpeaker);
                }

                Textbox = new MouseBlockingUIPanel();
                style.OnTextboxCreate(Textbox, Speaker, SubSpeaker);
                Textbox.OnLeftClick += OnBoxClick;
                Append(Textbox);

                DialogueText DialogueText = new DialogueText();
                DialogueText.boxWidth = Textbox.Width.Pixels;
                DialogueText.Text = Language.GetTextValue(DialogueHolder.LocalizationPath + (TreeIDs)DialogueTreeIndex + ".Messages." + DialogueIndex);
                if (CurrentDialogue.TextDelay != -1)
                    DialogueText.textDelay = CurrentDialogue.TextDelay;
                else if (CurrentDialogue.CharacterIndex == -1)
                    DialogueText.textDelay = 3;
                else
                    DialogueText.textDelay = Characters[CurrentDialogue.CharacterIndex].TextDelay;
                DialogueText.Top.Pixels = 25;
                DialogueText.Left.Pixels = 15;
                Textbox.Append(DialogueText);

                if (CurrentDialogue.Responses != null)
                {
                    Response[] availableResponses = CurrentDialogue.Responses.Where(r => r.Requirement).ToArray();
                    int responseCount = availableResponses.Length;

                    for (int i = 0; i < responseCount; i++)
                    {
                        UIPanel button = new UIPanel();
                        style.OnResponseButtonCreate(button, responseCount, i);
                        button.OnLeftClick += OnButtonClick;
                        Textbox.Append(button);

                        UIText text = new UIText(Language.GetTextValue(DialogueHolder.LocalizationPath + (TreeIDs)DialogueTreeIndex + ".Responses." + availableResponses[i].Title));
                        style.OnResponseTextCreate(text);
                        button.Append(text);
                    }
                }

                style.PostUICreate(DialogueTreeIndex, DialogueIndex, Textbox, Speaker, SubSpeaker);

                justOpened = false;
            }
        }
        private int counter = 0;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            BaseDialogueStyle style = new BaseDialogueStyle();
            style = new DefaultDialogueStyle();
            
            if (ModContent.GetInstance<DialogueUISystem>().isDialogueOpen)
            {
                style.PostUpdateActive(Textbox, Speaker, SubSpeaker);               
            }
            else
            {
                style.PostUpdateClosing(Textbox, Speaker, SubSpeaker);
            }
            counter++;
        }
        private void OnBoxClick(UIMouseEvent evt, UIElement listeningElement)
        {
            DialogueText dialogue = (DialogueText)Textbox.Children.Where(c => c.GetType() == typeof(DialogueText)).First();
            if (DialogueTrees[DialogueTreeIndex].Dialogues[DialogueIndex].Responses == null && !dialogue.crawling)
            {
                if(DialogueTrees[DialogueTreeIndex].Dialogues.Length > DialogueIndex + 1)
                    ModContent.GetInstance<DialogueUISystem>().UpdateDialogueUI(DialogueTreeIndex, DialogueIndex + 1);
                else
                    ModContent.GetInstance<DialogueUISystem>().isDialogueOpen = false;
            }
            else if (dialogue.crawling)
                dialogue.textIndex = Language.GetTextValue(DialogueHolder.LocalizationPath + (TreeIDs)DialogueTreeIndex + ".Messages." + DialogueIndex).Length;
        }
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            int responseCount = DialogueTrees[DialogueTreeIndex].Dialogues[DialogueIndex].Responses.Count();
            int buttonID = (int)(listeningElement.HAlign / (1f / (responseCount + 1)));
            int heading = DialogueTrees[DialogueTreeIndex].Dialogues[DialogueIndex].Responses[buttonID - 1].DialogueIndex;
            if (heading == -1 || (heading == -2 && !(DialogueTrees[DialogueTreeIndex].Dialogues.Length > DialogueIndex + 1)))
                ModContent.GetInstance<DialogueUISystem>().isDialogueOpen = false;
            else if (heading == -2 && DialogueTrees[DialogueTreeIndex].Dialogues.Length > DialogueIndex + 1)
            {
                ModContent.GetInstance<DialogueUISystem>().UpdateDialogueUI(DialogueTreeIndex, DialogueIndex + 1);
            }
            else
                ModContent.GetInstance<DialogueUISystem>().UpdateDialogueUI(DialogueTreeIndex, heading);
        }
        public static void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
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
}
