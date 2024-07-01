using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace Cascade.Content.UI.Dialogue
{
    #region Structures
    /// <param name="ID">The identifier for this Character, commonly their name. This is used primarilly to locate this character's Expression Assets within the Character Assets folder.</param>
    /// <param name="expressions">An array of identifiers, smililar to a Character's <see cref="ID"/>, used to find individual Expression Assets within the Character Assets Folder.</param>
    /// <param name="name">The actual name of the character, used by the Textbox. Can include spaces and other formatting, unlike the <see cref="ID"/>. Will default to the Character's <see cref="ID"/> if not set.</param>
    /// <param name="scale">Determines the scale the character portrait will be drawn at by the Dialogue System. Defaults to <see cref="2f"/>.</param>
    /// <param name="styleID">The ID of the Textbox Style associated with this Character. Defaults to <see cref="0"/>.</param>
    /// <param name="textDelay">The Text Delay associated with this character. Affects how long between characters appearing in the Textbox. Defaults to <see cref="3"/>.</param>
    /// <returns>
    /// Represents a character able to be used within a <see cref="DialogueTree"/>.
    /// </returns>
    public struct Character(string ID, string[] expressions, string name = null, float scale = 2f, int styleID = 0, int textDelay = 3)
    {
        public string ID = ID;
        public string Name = name ?? ID;
        public float Scale = scale;
        public string[] ExpressionIDs = expressions;
        public int StyleID = styleID;
        public int TextDelay = textDelay;
    }
    /// <param name="dialogues">The array of <see cref="Dialogue"/>s the Tree manages.</param>
    /// <param name="characters">The array of <see cref="Character"/>s the Tree is able to use.</param>
    /// <returns>
    /// Represents a Dialogue Tree able to be displayed via the <see cref="DialogueUISystem.DisplayDialogueTree(int, int)"/> function./>s.
    /// </returns>
    public struct DialogueTree(Dialogue[] dialogues, Character[] characters)
    {
        public Dialogue[] Dialogues = dialogues;
        public Character[] Characters = characters;
    }
    /// <param name="message">The Text to be displayed.</param>
    /// <param name="responses">The array of <see cref="Response"/>s the player can give. If set to null, clicking on the Textbox itself will proceed to the next Dialogue within the <see cref="DialogueTree"/> or close the Dialogue if there are no more dialogues Defaults to <see cref="null"/>. </param>
    /// <param name="characterIndex">The index of a character within the <see cref="DialogueTree"/>'s <see cref="DialogueTree.Characters"/> array. Represents the character who will be speaking. Defaults to <see cref="0"/>, the first character in the <see cref="DialogueTree.Characters"/> array.</param>
    /// <param name="expressionIndex">The index of an expression within a <see cref="Character"/>'s <see cref="Character.ExpressionIDs"/> array. Represents the expression, or asset, the character will use while speaking. Defaults to <see cref="0"/>, the first expression in the <see cref="Character.ExpressionIDs"/> array.</param>
    /// <param name="styleID">The ID of the Textbox Style associated with this Dialogue. Is able to differ from that of its Character's Defaults to 0.</param>
    /// <param name="textScaleX">Scales the size of the text horizontally. Defaults to <see cref="1.5f"/>.</param>
    /// <param name="textScaleY">Scales the size of the text vertically. Defaults to <see cref="1.5f"/>.</param>
    /// <param name="textDelay">The Text Delay associated with this character. Affects how long between characters appearing in the Textbox. Defaults to <see cref="-1"/>, causing it to use the delay associated with the current <see cref="Character"/> if no Character is speaking, it defaults to <see cref="3"/>.</param>
    /// <param name="musicID">Unimplemented.</param>
    /// <returns>
    /// Represents a single dialogue state within a <see cref="DialogueTree"/>.
    /// </returns>
    public struct Dialogue(string message, Response[] responses = null, int characterIndex = 0, int expressionIndex = 0, int styleID = -1, float textScaleX = 1.5f, float textScaleY = 1.5f, int textDelay = -1, int musicID = -1)
    {
        public string Message = message;
        public Response[] Responses = responses;
        public int CharacterIndex = characterIndex;
        public int ExpressionIndex = expressionIndex;
        public Vector2 TextScale = new(textScaleX, textScaleY);
        public int StyleID = styleID;
        public int TextDelay = textDelay;
        public int MusicID = musicID;
    }
    /// <param name="title">The text displayed on the Response Button.</param>
    /// <param name="dialogueIndex">The index within the <see cref="DialogueTree.Dialogues"/>s array this response leads to. Defaults to <see cref="-1"/>, which closes the dialogue.</param>
    /// <param name="requirement">A <see cref="Boolean"/> which determines whether or not this response can appear as an option Defaults to <see cref="true"/>.</param>
    /// <returns>
    /// Represents a response the player is able to give to a <see cref="Dialogue"/>./>s.
    /// </returns>
    public struct Response(string title, int dialogueIndex = -1, bool requirement = true)
    {
        public string Title = title;
        public int DialogueIndex = dialogueIndex;
        public bool Requirement = requirement;
    }
    #endregion
    public static class DialogueTreeID
    {
        internal enum TreeIDs
        {
            //When adding a new Dialogue Tree you MUST give it an ID which is stored here. This ID coorisponds with its placement within the PopulateDialogueTrees function and will be used to call your DialogueTree. 
            //TreeIDs and the  should be organized based on their character (as seen below). If they have multiple either use what you consider the 'primary' character or make a section for dialogues involving those two characters.
            #region The Calamity IDs
            Calamitous,
            #endregion

            #region Ardiena IDs
            Eeveelution,
            #endregion
        }
        internal enum CharacterIDs
        {
            //These work similarly to TreeIDs, but coorispon to the Characters array instead
            TheCalamity,
            Ardiena,
        }
        public static readonly Character[] Characters =
        {
            new Character("TheCalamity", new string[] {"Normal", "Finality"}, "[c/FF0000:The Calamity]"),
            new Character("Ardiena", new string[] {"Default", "Happy"})
        };
        public static DialogueTree[] DialogueTrees = PopulateDialogueTrees();
        public static DialogueTree[] PopulateDialogueTrees()
        {
            return new DialogueTree[]
            {
                #region The Calamity Trees
                new DialogueTree
                (
                    new Dialogue[]
                    {
                        new Dialogue
                        (
                            "Things are about to get [c/FF0000:Calamitous!] Tremble in fear!",
                            new Response[]
                            {
                                new Response("What?", 1),
                                new Response("Huh?", 1),
                            },
                            expressionIndex: 0
                        ),
                        new Dialogue
                        (
                            "Witness the power of a [c/FF0000:Calamity!!!] MWAHAHAHAHAHA!!! SKIBDIDI [c/FF0000:TRISZ EDHEJF DHDJE DFHEDHFDJH] teehee",
                            new Response[]
                            {
                                new Response("[c/FF0000:Okay...?]", -2)
                            },
                            expressionIndex: 1
                        ),
                        new Dialogue
                        (
                            "Fuck you.",
                            expressionIndex: 1
                        ),
                        new Dialogue
                        (
                            "Bitch.",
                            expressionIndex: 1
                        ),
                    },
                    new Character[]
                    {
                        Characters[(int)CharacterIDs.TheCalamity]
                    }
                ),              
                #endregion

                #region Ardiena Trees
                new DialogueTree
                (
                    new Dialogue[]
                    {
                        new Dialogue
                        (
                            "What's your favorite Eeveelution?",
                            new Response[]
                            {
                                new Response("Flareon", 1),
                                new Response("Jolteon", 2),
                                new Response("Vaporeon", 3),
                            },
                            -1,
                            expressionIndex: 0
                        ),
                        new Dialogue
                        (
                            "That's a fire answer!",
                            new Response[]
                            {
                                new Response("Thanks!")
                            },
                            -1,
                            expressionIndex: 0
                        ),
                        new Dialogue
                        (
                            "Quite a shocking choice!",
                            new Response[]
                            {
                                new Response("Thanks!")
                            },
                            -1,
                            expressionIndex: 0
                        ),
                        new Dialogue
                        (
                            "Okay, I'm dropping this.",
                            new Response[]
                            {
                                new Response("Oh..."),
                                new Response("Did you know-")
                            },
                            -1,
                            expressionIndex: 0
                        ),
                    },
                    new Character[]
                    {
                        Characters[(int)CharacterIDs.Ardiena]
                    }
                ),
                #endregion
            };
        }
    }
    public class DialogueUISystem : ModSystem
    {
        public static readonly SoundStyle UseSound = new("Windfall/Assets/Sounds/Items/JournalPageTurn");
        internal DialogueUIState DialogueUIState;
        public UserInterface DialogueUI;
        public bool isDialogueOpen = false;
        /*
        public void ShowDialogueUI()
        {
            SoundEngine.PlaySound(UseSound with
            {
                Pitch = -0.25f,
                PitchVariance = 0.5f,
                MaxInstances = 5,
                SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
            });
            Main.NewText("Opening UI");
            isDialogueOpen = true;
            DialogueUI?.SetState(DialogueUIState);
        }
        */
        public void DisplayDialogueTree(int TreeIndex, int DialogueIndex = 0)
        {
            SoundEngine.PlaySound(UseSound with
            {
                Pitch = -0.25f,
                PitchVariance = 0.5f,
                MaxInstances = 5,
                SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
            });
            isDialogueOpen = true;
            //Update the DialogueTree array with any new changes (use Hot Reload to apply changes to the function). Use this while testing out your dialogue so you dont have to restart the program every time you add something!
            DialogueTreeID.DialogueTrees = DialogueTreeID.PopulateDialogueTrees();
            DialogueUI = new UserInterface();
            DialogueUIState = new DialogueUIState();
            DialogueUIState.DialogueTreeIndex = TreeIndex;
            DialogueUIState.DialogueIndex = DialogueIndex;
            DialogueUIState.justOpened = true;
            DialogueUIState.Activate();

            DialogueUI?.SetState(DialogueUIState);
        }
        
        public void UpdateDialogueUI(int TreeIndex, int DialogueIndex)
        {
            DialogueUI?.SetState(null);
            DialogueUI = new UserInterface();
            DialogueUIState = new DialogueUIState();
            DialogueUIState.DialogueTreeIndex = TreeIndex;
            DialogueUIState.DialogueIndex = DialogueIndex;

            DialogueUI?.SetState(DialogueUIState);
        }
        
        public void HideDialogueUI()
        {
            SoundEngine.PlaySound(UseSound with
            {
                Pitch = -0.25f,
                PitchVariance = 0.5f,
                MaxInstances = 5,
                SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
            });
            isDialogueOpen = false;
            DialogueUI?.SetState(null);           
        }
        public override void Load()
        {
            if (!Main.dedServ)
            {
                DialogueUI = new UserInterface();
                DialogueUIState = new DialogueUIState();
                DialogueUIState.Activate();
            }
        }
        private GameTime _lastUpdateUiGameTime;

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (DialogueUI?.CurrentState != null)
            {
                DialogueUI?.Update(gameTime);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Cascade: Displays the Dialogue UI",
                    delegate
                    {
                        DialogueUI.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
