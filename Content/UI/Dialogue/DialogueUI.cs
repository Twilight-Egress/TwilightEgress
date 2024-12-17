﻿using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static Cascade.Content.UI.Dialogue.DialogueHolder;
using Terraria.UI.Chat;
using Cascade.Content.UI.Dialogue.DialogueStyles;
using Terraria;
using System.Linq.Expressions;

namespace Cascade.Content.UI.Dialogue
{
    
    public class DialogueUIState : UIState
    {
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

                int textWidth = (int)Parent.Width.Pixels - 6;
                textWidth = (int)(textWidth * xResolutionScale);

                if (++counter % textDelay == 0 && counter >= 0)
                    textIndex++;
                List<TextSnippet> fullSnippets = ChatManager.ParseMessage(Text, Color.White);
                List<TextSnippet> printedSnippets = new();
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
                for (int i = 0; i < printedSnippets.Count; i++)
                {
                    TextToPrint += printedSnippets[i].TextOriginal;
                }
                List<string> textLines = Utils.WordwrapString(TextToPrint, FontAssets.MouseText.Value, (int)((Parent.Width.Pixels + this.Width.Pixels) / 1.5f), 250, out _).ToList();
                textLines.RemoveAll(text => string.IsNullOrEmpty(text));

                string PreviousLineColorHex = "";
                for (int i = 0; i < textLines.Count; i++)
                {
                    int LeftBracketCounter = 0;
                    int RightBracketCounter = 0;
                    bool gettingHex = false;
                    for (int j = 0; j < textLines[i].Length; j++)
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

                List<List<TextSnippet>> dialogLines = new();
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

        public MouseBlockingUIPanel Textbox;
        public UIImage Speaker;
        public UIImage SubSpeaker;

        public string TreeKey;
        public int DialogueIndex;
        private int counter = 0;
        private int frameCounter = 1;
        public override void OnInitialize()
        {
            if (DialogueTrees != null)
            {
                counter = 0;

                DialogueTree CurrentTree = DialogueTrees[TreeKey];
                Dialogue CurrentDialogue = CurrentTree.Dialogues[DialogueIndex];
                Character CurrentSpeaker;
                Character CurrentSubSpeaker;

                int subSpeakerIndex = -1;
                bool justOpened = true;
                bool newSpeaker = false;
                bool newSubSpeaker = false;
                bool returningSpeaker = false;
                bool speakerRight = true;

                BaseDialogueStyle style = new();
                if(CurrentDialogue.StyleID == -1)
                    switch(CurrentTree.Characters[CurrentDialogue.CharacterIndex].StyleID)
                    {
                        case 0:
                            style = new DefaultDialogueStyle();
                            break;
                        case 1:
                            style = new ArdiDialogueStyle();
                            break;
                        default: 
                            style = new DefaultDialogueStyle();
                            break;
                    }
                else
                    switch (CurrentDialogue.StyleID)
                    {
                        case 0:
                            style = new DefaultDialogueStyle();
                            break;
                        case 1:
                            style = new ArdiDialogueStyle();
                            break;
                        default:
                            style = new DefaultDialogueStyle();
                            break;
                    }

                if (ModContent.GetInstance<DialogueUISystem>() != null)
                {
                    if(ModContent.GetInstance<DialogueUISystem>().CurrentSpeaker != null)
                        CurrentSpeaker = (Character)ModContent.GetInstance<DialogueUISystem>().CurrentSpeaker;
                    if(ModContent.GetInstance<DialogueUISystem>().SubSpeaker != null)
                        CurrentSubSpeaker = (Character)ModContent.GetInstance<DialogueUISystem>().SubSpeaker;
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
                    CurrentSubSpeaker = new Character("None", new Expression[] { new() });
                }
                style.PreUICreate(TreeKey, DialogueIndex);
                if (CurrentDialogue.CharacterIndex != -1)
                {
                    //Main.NewText("Create Speaker: " + CurrentDialogue.CharacterIndex);
                    CurrentSpeaker = CurrentTree.Characters[CurrentDialogue.CharacterIndex];
                    string expressionID = CurrentSpeaker.Expressions[CurrentDialogue.ExpressionIndex].Title;
                    
                    Texture2D speakerTexture = ModContent.Request<Texture2D>($"{nameof(Cascade)}/Content/UI/Dialogue/CharacterAssets/{CurrentSpeaker.ID}/{CurrentSpeaker.ID}_{expressionID}", AssetRequestMode.ImmediateLoad).Value;
                    Rectangle speakerFrame = new Rectangle(0, 0, speakerTexture.Bounds.Width, speakerTexture.Bounds.Height / CurrentSpeaker.Expressions[CurrentDialogue.ExpressionIndex].FrameCount);

                    Texture2D speakerFrameTexture = new(Main.graphics.GraphicsDevice, speakerFrame.Width, speakerFrame.Height);
                    Color[] data = new Color[speakerFrame.Width * speakerFrame.Height];
                    speakerTexture.GetData(0, speakerFrame, data, 0, data.Length);
                    speakerFrameTexture.SetData(data);

                    Speaker = new(speakerFrameTexture)
                    {
                        ImageScale = CurrentSpeaker.Scale
                    };
                    float startPositionX = 0;
                    if (speakerRight)
                        startPositionX = returningSpeaker ? 1600f : 1500f;
                    else
                        startPositionX = returningSpeaker ? 100f : 200f;

                    if (justOpened || newSpeaker)
                        SetRectangle(Speaker, left: startPositionX, top: 1200f, width: speakerFrameTexture.Width, height: speakerFrameTexture.Height);
                    else
                        SetRectangle(Speaker, left: startPositionX, top: 500f, width: speakerFrameTexture.Width, height: speakerFrameTexture.Height);
                    style.PreSpeakerCreate(TreeKey, DialogueIndex, Speaker);
                    Append(Speaker);
                    style.PostSpeakerCreate(TreeKey, DialogueIndex, Speaker);
                }
                if (subSpeakerIndex != -1)
                {
                    //Main.NewText("Create Sub-Speaker: " + subSpeakerIndex);
                    CurrentSubSpeaker = CurrentTree.Characters[subSpeakerIndex];
                    string expressionID = CurrentSubSpeaker.Expressions[0].Title;
                    Texture2D subSpeakerTexture = ModContent.Request<Texture2D>($"{nameof(Cascade)}/Content/UI/Dialogue/CharacterAssets/{CurrentSubSpeaker.ID}/{CurrentSubSpeaker.ID}_{expressionID}", AssetRequestMode.ImmediateLoad).Value;
                    Rectangle subSpeakerFrame = new Rectangle(0, 0, subSpeakerTexture.Bounds.Width, subSpeakerTexture.Bounds.Height / CurrentSubSpeaker.Expressions[0].FrameCount);

                    Texture2D subSpeakerFrameTexture = new(Main.graphics.GraphicsDevice, subSpeakerFrame.Width, subSpeakerFrame.Height);
                    Color[] data = new Color[subSpeakerFrame.Width * subSpeakerFrame.Height];
                    subSpeakerTexture.GetData(0, subSpeakerFrame, data, 0, data.Length);
                    subSpeakerFrameTexture.SetData(data);

                    SubSpeaker = new(subSpeakerFrameTexture)
                    {
                        ImageScale = CurrentSubSpeaker.Scale
                    };
                    float startPositionX = 0;
                    if (speakerRight)
                        startPositionX = newSpeaker || returningSpeaker ? 200f : 0f;
                    else
                        startPositionX = newSpeaker || returningSpeaker ? 1500f : 1700f;
                    SetRectangle(SubSpeaker, left: startPositionX, top: 500f, width: subSpeakerFrameTexture.Width, height: subSpeakerFrameTexture.Height);
                    style.PreSubSpeakerCreate(TreeKey, DialogueIndex, Speaker, SubSpeaker);
                    Append(SubSpeaker);
                    style.PostSubSpeakerCreate(TreeKey, DialogueIndex, Speaker, SubSpeaker);
                }

                Textbox = new MouseBlockingUIPanel();
                style.OnTextboxCreate(Textbox, Speaker, SubSpeaker);
                Textbox.OnLeftClick += OnBoxClick;
                Append(Textbox);

                DialogueText DialogueText = new()
                {
                    boxWidth = Textbox.Width.Pixels,
                    Text = Language.GetTextValue(LocalizationPath + TreeKey + ".Messages." + DialogueIndex)
                };
                if (CurrentDialogue.TextDelay != -1)
                    DialogueText.textDelay = CurrentDialogue.TextDelay;
                else if (CurrentDialogue.CharacterIndex == -1)
                    DialogueText.textDelay = 3;
                else
                    DialogueText.textDelay = Characters[CurrentDialogue.CharacterIndex].TextDelay;
                style.OnDialogueTextCreate(DialogueText);
                Textbox.Append(DialogueText);

                if (CurrentDialogue.Responses != null)
                {
                    Response[] availableResponses = CurrentDialogue.Responses.Where(r => r.Requirement).ToArray();
                    int responseCount = availableResponses.Length;
                    
                    for (int i = 0; i < responseCount; i++)
                    {
                        UIPanel button = new();
                        style.OnResponseButtonCreate(button, Textbox, responseCount, i);
                        button.OnLeftClick += OnButtonClick;
                        Append(button);

                        UIText text = new(Language.GetTextValue(LocalizationPath + TreeKey + ".Responses." + availableResponses[i].Title));
                        style.OnResponseTextCreate(text);
                        button.Append(text);
                        if (availableResponses[i].Cost != null)
                        {
                            ItemStack cost = (ItemStack)availableResponses[i].Cost;
                            UIPanel costHolder = new();
                            costHolder.BorderColor = Color.Transparent;
                            costHolder.BackgroundColor = Color.Transparent;

                            UIText stackText = new($"x{cost.Stack}");
                            stackText.HAlign = 1f;
                            stackText.VAlign = 0.5f;

                            Texture2D itemTexture = (Texture2D)ModContent.Request<Texture2D>(ItemLoader.GetItem(cost.Type).Texture);
                            UIImage itemIcon = new(itemTexture);
                            itemIcon.Width.Pixels = itemTexture.Width;
                            itemIcon.Height.Pixels = itemTexture.Height;
                            itemIcon.ImageScale = 18f / itemIcon.Height.Pixels;

                            itemIcon.Top.Pixels -= itemIcon.Height.Pixels / 2;
                            itemIcon.Left.Pixels -= itemIcon.Width.Pixels / 2;

                            costHolder.Height.Pixels = itemIcon.Height.Pixels > stackText.Height.Pixels ? itemIcon.Height.Pixels : stackText.Height.Pixels;
                            costHolder.Height.Pixels *= 10;
                            costHolder.Width.Pixels = (itemIcon.Width.Pixels * itemIcon.ImageScale) + (15 * stackText.Text.Length);

                            costHolder.Append(itemIcon);
                            costHolder.Append(stackText);

                            style.OnResponseCostCreate(text, costHolder);
                            
                            button.Append(costHolder);
                        }
                    }
                }

                style.PostUICreate(TreeKey, DialogueIndex, Textbox, Speaker, SubSpeaker);

                justOpened = false;
            }
        }       
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            DialogueTree CurrentTree = DialogueTrees[TreeKey];
            Dialogue CurrentDialogue = CurrentTree.Dialogues[DialogueIndex];
            BaseDialogueStyle style;
            if (CurrentDialogue.StyleID == -1)
                switch (CurrentTree.Characters[CurrentDialogue.CharacterIndex].StyleID)
                {
                    case 0:
                        style = new DefaultDialogueStyle();
                        break;
                    case 1:
                        style = new ArdiDialogueStyle();
                        break;
                    default:
                        style = new DefaultDialogueStyle();
                        break;
                }
            else
                switch (CurrentDialogue.StyleID)
                {
                    case 0:
                        style = new DefaultDialogueStyle();
                        break;
                    case 1:
                        style = new ArdiDialogueStyle();
                        break;
                    default:
                        style = new DefaultDialogueStyle();
                        break;
                }

            if (ModContent.GetInstance<DialogueUISystem>().isDialogueOpen)
            {
                style.PostUpdateActive(Textbox, Speaker, SubSpeaker);                
                if (Speaker != null)
                {
                    if (Speaker.Top.Pixels > 500f)
                    {
                        Speaker.Top.Pixels -= (Speaker.Top.Pixels - 500f) / 15;
                        if (Speaker.Top.Pixels - 500f < 1)
                            Speaker.Top.Pixels = 500f;
                    }
                    if (ModContent.GetInstance<DialogueUISystem>().speakerRight && Speaker.Left.Pixels > 1500f)
                    {
                        Speaker.Left.Pixels -= (Speaker.Left.Pixels - 1500f) / 20;
                        if (Speaker.Left.Pixels - 1500f < 1)
                            Speaker.Left.Pixels = 1500f;
                    }
                    else if (!ModContent.GetInstance<DialogueUISystem>().speakerRight && Speaker.Left.Pixels < 200f)
                    {
                        Speaker.Left.Pixels += (200f - Speaker.Left.Pixels) / 20;
                        if (200f - Speaker.Left.Pixels < 1)
                            Speaker.Left.Pixels = 200f;
                    }
                    Character speakerCharacter = CurrentTree.Characters[CurrentDialogue.CharacterIndex];
                    Expression currentExpression = speakerCharacter.Expressions[CurrentDialogue.ExpressionIndex];                   
                    if (currentExpression.FrameRate != 0 && counter % currentExpression.FrameRate == 0)
                    {
                        frameCounter++;
                        if (frameCounter > currentExpression.FrameCount)
                        {
                            if (currentExpression.Loop)
                                frameCounter = 1;
                            else
                                frameCounter = currentExpression.FrameCount;
                        }
                        Texture2D speakerTexture = ModContent.Request<Texture2D>($"{nameof(Cascade)}/Content/UI/Dialogue/CharacterAssets/{speakerCharacter.ID}/{speakerCharacter.ID}_{currentExpression.Title}", AssetRequestMode.ImmediateLoad).Value;
                        Rectangle speakerFrame = speakerTexture.Frame(1, speakerCharacter.Expressions[CurrentDialogue.ExpressionIndex].FrameCount, 0, frameCounter - 1);

                        Texture2D speakerFrameTexture = new(Main.graphics.GraphicsDevice, speakerFrame.Width, speakerFrame.Height);
                        Color[] data = new Color[speakerFrame.Width * speakerFrame.Height];
                        speakerTexture.GetData(0, speakerFrame, data, 0, data.Length);
                        speakerFrameTexture.SetData(data);

                        Speaker.SetImage(speakerFrameTexture);
                    }
                }
                if (SubSpeaker != null)
                {
                    if (ModContent.GetInstance<DialogueUISystem>().speakerRight && SubSpeaker.Left.Pixels > 0f)
                    {
                        SubSpeaker.Left.Pixels -= (SubSpeaker.Left.Pixels - 0f) / 20;
                        if (SubSpeaker.Left.Pixels - 0f < 1)
                            SubSpeaker.Left.Pixels = 0f;
                    }
                    else if (!ModContent.GetInstance<DialogueUISystem>().speakerRight && SubSpeaker.Left.Pixels < 1700f)
                    {
                        SubSpeaker.Left.Pixels += (1700f - SubSpeaker.Left.Pixels) / 20;
                        if (1700f - SubSpeaker.Left.Pixels < 1)
                            SubSpeaker.Left.Pixels = 1700f;

                    }
                }
            }
            else
            {
                style.PostUpdateClosing(Textbox, Speaker, SubSpeaker);
                if (Speaker != null)
                {
                    if (ModContent.GetInstance<DialogueUISystem>().speakerRight && Speaker.Left.Pixels < 2200f)
                    {
                        Speaker.Left.Pixels += (2200 - Speaker.Left.Pixels) / 20;
                        if (2100f - Speaker.Left.Pixels < 10)
                            Speaker.Left.Pixels = 2100f;
                    }
                    else if (!ModContent.GetInstance<DialogueUISystem>().speakerRight && Speaker.Left.Pixels > -600)
                    {
                        Speaker.Left.Pixels -= (Speaker.Left.Pixels + 600) / 20;
                        if (Speaker.Left.Pixels + 500 < 10)
                            Speaker.Left.Pixels = -600;
                    }
                }
                if (SubSpeaker != null)
                {
                    if (!ModContent.GetInstance<DialogueUISystem>().speakerRight && SubSpeaker.Left.Pixels < 2200f)
                    {
                        SubSpeaker.Left.Pixels += (2200 - SubSpeaker.Left.Pixels) / 20;
                        if (2100f - SubSpeaker.Left.Pixels < 10)
                            SubSpeaker.Left.Pixels = 2100f;
                    }
                    else if (ModContent.GetInstance<DialogueUISystem>().speakerRight && SubSpeaker.Left.Pixels > -600)
                    {
                        SubSpeaker.Left.Pixels -= (SubSpeaker.Left.Pixels + 600) / 20;
                        if (SubSpeaker.Left.Pixels + 500 < 10)
                            SubSpeaker.Left.Pixels = -600;
                    }
                }
                if
                (
                    (Speaker == null || (Speaker.Left.Pixels == 2100f && ModContent.GetInstance<DialogueUISystem>().speakerRight) || (Speaker.Left.Pixels == -600 && !ModContent.GetInstance<DialogueUISystem>().speakerRight))
                    &&
                    (SubSpeaker == null || (SubSpeaker.Left.Pixels == 2100f && !ModContent.GetInstance<DialogueUISystem>().speakerRight) || (SubSpeaker.Left.Pixels == -600 && ModContent.GetInstance<DialogueUISystem>().speakerRight))
                    &&
                    style.TextboxOffScreen(Textbox)
                )
                    ModContent.GetInstance<DialogueUISystem>().HideDialogueUI();

            }
            counter++;
        }
        public static void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }
        internal void OnBoxClick(UIMouseEvent evt, UIElement listeningElement)
        {
            DialogueText dialogue = (DialogueText)Textbox.Children.Where(c => c.GetType() == typeof(DialogueText)).First();
            if (DialogueTrees[TreeKey].Dialogues[DialogueIndex].Responses == null && !dialogue.crawling)
            {
                ModContent.GetInstance<DialogueUISystem>().ButtonClick?.Invoke(TreeKey, DialogueIndex, 0);

                if (DialogueTrees[TreeKey].Dialogues.Length > DialogueIndex + 1)
                    ModContent.GetInstance<DialogueUISystem>().UpdateDialogueUI(TreeKey, DialogueIndex + 1);
                else
                {
                    ModContent.GetInstance<DialogueUISystem>().isDialogueOpen = false;
                    ModContent.GetInstance<DialogueUISystem>().DialogueClose?.Invoke(TreeKey, DialogueIndex, 0);
                }
            }
            else if (dialogue.crawling)
                dialogue.textIndex = Language.GetTextValue(LocalizationPath + TreeKey + ".Messages." + DialogueIndex).Length;
        }
        internal void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {            
            int responseCount = DialogueTrees[TreeKey].Dialogues[DialogueIndex].Responses.Count();
            UIText text = (UIText)listeningElement.Children.ToArray().First();
            int buttonID = 0;
            for (int i = 0; i < responseCount; i++)
            {
                if (text.Text == Language.GetTextValue(LocalizationPath + TreeKey + ".Responses." + DialogueTrees[TreeKey].Dialogues[DialogueIndex].Responses[i].Title))
                    buttonID = i;
            }
            Response response = DialogueTrees[TreeKey].Dialogues[DialogueIndex].Responses[buttonID];
            if (response.Cost == null || CanAffordCost(Main.LocalPlayer, response.Cost.Value))
            {
                ModContent.GetInstance<DialogueUISystem>().ButtonClick?.Invoke(TreeKey, DialogueIndex, buttonID);

                int heading = response.DialogueIndex;
                if (heading == -1 || (heading == -2 && !(DialogueTrees[TreeKey].Dialogues.Length > DialogueIndex + 1)))
                {
                    ModContent.GetInstance<DialogueUISystem>().isDialogueOpen = false;
                    ModContent.GetInstance<DialogueUISystem>().DialogueClose?.Invoke(TreeKey, DialogueIndex, buttonID);
                }
                else if (heading == -2 && DialogueTrees[TreeKey].Dialogues.Length > DialogueIndex + 1)
                    ModContent.GetInstance<DialogueUISystem>().UpdateDialogueUI(TreeKey, DialogueIndex + 1);
                else
                    ModContent.GetInstance<DialogueUISystem>().UpdateDialogueUI(TreeKey, heading);
            }
        }
        private static bool CanAffordCost(Player player, ItemStack price)
        {
            int amount = price.Stack;
            foreach (Item item in player.inventory.Where(i => i.type == price.Type))
            {
                if (item.stack >= amount)
                {
                    amount = 0;
                    break;
                }
                else
                    amount -= item.stack;
            }
            if (amount == 0)
            {
                foreach (Item item in player.inventory.Where(i => i.type == price.Type))
                {
                    amount = price.Stack;
                    if (item.stack >= amount)
                    {
                        item.stack -= amount;
                        amount = 0;
                        break;
                    }
                    else
                    {
                        amount -= item.stack;
                        item.stack = 0;
                    }
                }
                return true;
            }
            else
                return false;
        }
        
    }
}
