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
    public static class DialogueHolder
    {
        #region Dialogue Tree Creation Tutorial!
        /*
         * Do YOU wanna make a Dialogue Tree? Well you've come to the right place! After this tutorial, you'll be a Daring Dialogue Doer!
         * 
         * Step 1: Setup
         * There's a little bit of house keeping you'll wanna get done before you can get to making your tree. (Unfortuantly, I can't automate everything :P)
         * First, you'll wanna take a look at the enum TreeIDs right below this section. TreeIDs holds identifiers for every tree the mod has! So if you wanna make a new one, you'll need to give it an ID!
         * TreeIDs is split up via Regions based on characters so if you know the character, or main character if there's multiple, of your dialogue, you'll wanna put your ID in it's respective region!
         * Naming it something distinct will help other people know what dialogue its refering to when they wanna call this Tree, so try and be a little creativre with naming it!
         * Lastly, be sure to take note of where in the TreeIDs enum you've put your ID. You'll need ro remember that for what comes next!
         * 
         * Next up for our setting up, you'll need to head to the PopulateDialogueTrees function. This function is where you'll be creating your Dialogue Tree!
         * If you remember the TreeIDs enum, you'll notice that this function is similarly organized. That's because as we went over, the TreeID you made matches a spot within the Dialogue Tree Array this function returns!
         * So, you'll want to find where in this function your TreeID leads to. (If you've happened to make your ID at the very end of the TreeIDs enum and no other IDs are beyond it, then you can simply go to the bottom to add your Tree!
         * Additionally, you'll notice that next to each tree's initializer "new DialogueTree" there SHOULD (so long as people do so) be a comment which matches up with and ID in TreeIDs. You can use this to help locate where in the function your Tree will go!
         * Once you've found the spot between two Trees where yours should go, place your cursor at the END of the preceeding Tree Initializer ")," and hit enter. This spot is where you'll be creating your Tree!!!
         * 
         * You're on the last bit of setup, and the easiest! In the spot you've created for your Tree, paste the following (Everything that doesnt have an * to the left of it):
           new DialogueTree //ID
                (
                    new Dialogue[]
                    {
                        new Dialogue //First Dialogue
                        (
                            message: "Things are about to get [c/FF0000:Calamitous!] Tremble in fear!",
                            responses: new Response[]
                            {
                                new Response("What?", 1, Main.LocalPlayer.direction == 1),
                                new Response("Huh?", 1, Main.LocalPlayer.direction == -1),
                            },
                            characterIndex: 0
                            expressionIndex: 0
                            styleID: -1
                            textScaleX: 1.5f
                            textScaleY: 1.5f
                            textDelay: -1
                            musicID: -1
                        ),
                        new Dialogue //Second Dialogue
                        (
                            "Witness the power of a [c/FF0000:True Calamity!!!] MWAHAHAHAHAHA!!!",
                            new Response[]
                            {
                                new Response("[c/FF0000:Okay...?]", -1)
                            },
                            expressionIndex: 1
                        ),
                    },
                    new Character[]
                    {
                        Characters[(int)CharacterIDs.TheCalamity]
                    }
                ),
         * And congradulations!!! You have the start of a dialogue tree!
         * 
         * Step 2: What the hell does all this mean???
         * This section will be going over what actually makes up a Dialogue Tree. So, if you're confused about what all that stuff I had you paste means, then just sit tight!
         * 
         * To start, we'll go over what a Dialogue Tree actually is. It's made up of two parts an Array of Dialogues and an Array of Characters. An array is basically just a list of variables, in our case these variables are Structures which can hold a buncha different values.
         * We'll start with what Characters are since they're quite a bit simpler. Characters are who is actually talking in your dialogue, and have a few values associated with them which affect how dialogue spoken by them appears.
         * For your tree, you'll need to put every character you intend to have speak in your dialogue tree in this array. That can of course be only one character or multiple. It's up to you!
         * In the template code, we use only one character: The Calamity by finding the value in the Characters Array associated with its ID. This is also how other people will be calling your Dialogue Tree, using its ID within the DisplayDialogueTree function!
         * We'll go into deeper detail on Characters and even how to make a new Character later on, but for now you know all you need to create your Dialogue Tree, so long as you use a pre-existing character.
         * 
         * The array of Dialogues is where you'll be doing the majority of the process creating your Dialogue Tree, it is after all where all the dialogue is stored.
         * Within the template code, we have Two dialogues, which start similarly to the Tree itself "new Dialogue".
         * Within the first Dialogue, you'll find a noticably greater amount of variables. That's because this Dialogue has every possible variable you can use within it. Most of these variables have a Default Value associated with them. That means if you dont want to change that default value you don't need to include that variable in your Dialogue.
         * Additionally in the first Dialogue, each variable is prefaced with the variable itself, however, as you can see in the second Dialogue, that isn't required. You only need to preface a value with its variable if you have gone outside the standard order of the variables. To see this order, simply hover over either of the "Dialogue"s highlighted in green.
         * By hovering over that text, you can see a window containing all the variables you see in the first dialogue, as well as a small description of what a Dialogue is. This can also be done for the variables, highlighted in light blue, within the first dialogue! I encourage you to look over each one, as I won't be summarizing all of their functions here.
         * And with that, you now know all of the tools you have at your disposal! Now we'll go over how to create a new Dialogue, just to make sure you understand everything.
         * 
         * Step 3: Let's get yapping!
         * To create a new Dialogue we'll need to make space for it. Go to the bottom of your Tree and mouse next to where you find ")," (Note: Do nto mistake this point for "},", that is what defines the end of the entire Array of Dialogues).
         * From there, you can paste the following:
           new Dialogue
           (
               //Code Goes Here
           ),
         * You now have where you can begin creating your Dialogue! To begin, lets make this Dialogue's Message. This is the main text that will appear inside the Dialogue Box
         * Where it says "//Code Goes Here" Replace that with two quotation marks. Within those quotation marks, write whatever you want!
         * Once you're happy with your message, after the second quotation mark, put a comma ",".
         * Lastly, we have to define the Responses the player can give to this Dialogue... That is... If you want to. Responses aren't actually required for a Dialogue! If you choose not to define a Response, clicking the textbox will either procees to the next Dialogue in the Array, if one exists, or close the dialogue!
         * However, for the sake of practice, let's assume you do indeed want to create some responses. To begin, press enter after the comma you place at the end of your Message and paste the following:
           new Response[]
           {
               //Code Goes Here
           },
         * This defines the Array of Responses the player can give. Any number (to a reasonable extent) of responses can be place within here!
         * To define your first Response, replace where it says "//Code Goes Here" with the following
           new Response()
         * 
         * Responses, similarly to the Dialogues that they reside in, take a handful of parameter that can affect them
         * Firstly, is the Title, which is simply the text that will be place onto the Response Button. 
         * Second is the dialogueIndex. The dialogueIndex is the Index within the Dialogue Array that selecting this response leads to. However, it has two unique functions. 
         * When set to -1, it will instead close the Dialogue Box and when set to -2 it will simply proceed to the next Dialogue within the Array. (If, for whatever reason, it is set to -2 but there are no Dialogues after it in the Array, it will also close the Dialogue) 
         * Lastly is the Requirements. This boolean defines a condition which must be met in order for this response to be available. If it is false at the time this Dialogue appear, then that response will not appear.
         * Do take note, if you ever need to see these definitions again, you can hover over the variable's text to see a definition on what it does!
         * 
         * With this knowledge, I'll leave you to define how your Response should opperate! Good luck, and return here once you're done!
         * 
         * Step 4: Does this actually work?
         * I imagine now that you've made your first Dialogue Tree, you want to actually see it in-game. Well lucky for you that's quite simple!
         * Just below here you may have noticed the variable DebugID, which is set to a TreeID. All you need to do is set that value to the TreeID you created at the start of this tutorial!
         * Once you've done that, open up the game and cheat in the UI Debug Item. Using this item will call DisplayDialogueTree using your Tree! Give it a click through and see if everything turned out as intended!
         * 
         * With that, you've reached the end of this tutorial. I hope you've found it informative and are ready to make a whole forest of Dialogue Trees!
        */
        #endregion
        public static int DebugID = (int)TreeIDs.Calamitous;
        internal enum TreeIDs
        {
            #region The Calamity IDs
            Calamitous,
            #endregion

            #region Ardiena IDs
            Eeveelution,
            #endregion
        }
        internal enum CharacterIDs
        {
            //These work similarly to TreeIDs, but coorispond to the characters within the Characters array instead.
            // Same as the TreeIDs: Make sure the order of these matches the order of the Characters array
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
                //adding comments to denote which ID your tree matches up with is helpful for maintaining the order
                #region The Calamity Trees
                new DialogueTree //Calamitous
                (
                    new Dialogue[]
                    {
                        new Dialogue
                        (
                            "Things are about to get [c/FF0000:Calamitous!] Tremble in fear!",
                            new Response[]
                            {
                                new Response("What?", 1, Main.LocalPlayer.direction == 1),
                                new Response("Huh?", 1, Main.LocalPlayer.direction == -1),
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
                new DialogueTree //Eeveelutions
                (
                    new Dialogue[]
                    {
                        new Dialogue
                        (
                            "What's your favorite Eeveelution?",
                            new Response[]
                            {
                                new Response("[c/FF6339:Flareon]", 1),
                                new Response("[c/F3FF39:Jolteon]", 2),
                                new Response("[c/39A5FF:Vaporeon]", 3),
                            },
                            expressionIndex: 0
                        ),
                        new Dialogue
                        (
                            "That's a fire answer!",
                            new Response[]
                            {
                                new Response("Thanks!")
                            },
                            expressionIndex: 0
                        ),
                        new Dialogue
                        (
                            "Quite a shocking choice!",
                            new Response[]
                            {
                                new Response("Thanks!")
                            },
                            expressionIndex: 0
                        ),
                        new Dialogue
                        (
                            "Okay, I'm dropping this.",
                            new Response[]
                            {
                                new Response("Oh..."),
                                new Response("Did you know-", 4)
                            },
                            expressionIndex: 0
                        ),
                        new Dialogue
                        (
                            "[c/FF0000:You shall come to regret that choice.]",
                            new Response[]
                            {
                                new Response("[c/FF0000:Okay...?]", 5)
                            },
                            1,
                            1
                        ),
                        new Dialogue
                        (
                            "Who the hell are you?",
                            characterIndex: 0,
                            expressionIndex: 0
                        ),
                        new Dialogue
                        (
                            "...and why do you look like an Ogscule?",
                            characterIndex: 0,
                            expressionIndex: 0
                        ),
                        new Dialogue
                        (
                            "[c/FF0000:...]",
                            new Response[]
                            {
                                new Response("[c/FF0000:Okay...?]", -1)
                            },
                            1,
                            1
                        ),
                    },
                    new Character[]
                    {
                        Characters[(int)CharacterIDs.Ardiena],
                        Characters[(int)CharacterIDs.TheCalamity],
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
        public bool justOpened = true;
        public bool isDialogueOpen = false;
        public bool newSpeaker = false;
        public bool newSubSpeaker = false;
        public bool returningSpeaker = false;
        public bool speakerRight = true;
        public Character CurrentSpeaker = new Character("None", new string[] { "None" });
        public Character SubSpeaker = new Character("None", new string[] { "None"});
        public int subSpeakerIndex = -1;

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
            justOpened = true;
            speakerRight = true;
            newSpeaker = false;
            newSubSpeaker = false;
            returningSpeaker = false;            
            
            //Update the DialogueTree array with any new changes (use Hot Reload to apply changes to the function). Use this while testing out your dialogue so you dont have to restart the program every time you add something!
            DialogueHolder.DialogueTrees = DialogueHolder.PopulateDialogueTrees();
            DialogueTree currentTree = DialogueHolder.DialogueTrees[TreeIndex];
            Dialogue currentDialogue = currentTree.Dialogues[DialogueIndex];

            CurrentSpeaker = currentTree.Characters[currentDialogue.CharacterIndex];
            SubSpeaker = new Character("None", new string[] { "None" });
            subSpeakerIndex = -1;

            DialogueUI = new UserInterface();
            DialogueUIState = new DialogueUIState();            
            DialogueUIState.DialogueTreeIndex = TreeIndex;
            DialogueUIState.DialogueIndex = DialogueIndex;           
            DialogueUIState.Activate();

            DialogueUI?.SetState(DialogueUIState);
        }
        
        public void UpdateDialogueUI(int TreeIndex, int DialogueIndex)
        {
            int formerSpeakerIndex = DialogueHolder.DialogueTrees[DialogueUIState.DialogueTreeIndex].Dialogues[DialogueUIState.DialogueIndex].CharacterIndex;
            DialogueTree currentTree = DialogueHolder.DialogueTrees[TreeIndex];
            Dialogue currentDialogue = currentTree.Dialogues[DialogueIndex];
            //Main.NewText("Correct Speaker ID: " + currentTree.Characters[currentDialogue.CharacterIndex].ID);
            //Main.NewText("Current ID: " + CurrentSpeaker.ID);
            //Main.NewText("Subspeaker ID: " + SubSpeaker.ID);
            if (currentTree.Characters[currentDialogue.CharacterIndex].ID == CurrentSpeaker.ID)
            {
                //Main.NewText("Speaker Unchanged");
                newSpeaker = false;
                newSubSpeaker = false;
                returningSpeaker = false;
            }
            else if (SubSpeaker.ID == "None")
            {
                //Main.NewText("New speaker! No subspeaker");
                newSpeaker = true;
                newSubSpeaker = true;
                returningSpeaker = false;
                SubSpeaker = CurrentSpeaker;
                subSpeakerIndex = formerSpeakerIndex;
                CurrentSpeaker = currentTree.Characters[currentDialogue.CharacterIndex];
                speakerRight = !speakerRight;
            }
            else
            {
                //Main.NewText("New speaker! Yes subspeaker.");
                newSpeaker = false;
                newSubSpeaker = true;
                returningSpeaker = true;
                Character temp = SubSpeaker;
                SubSpeaker = CurrentSpeaker;
                subSpeakerIndex = formerSpeakerIndex;
                CurrentSpeaker = temp;
                speakerRight = !speakerRight;
            }

            justOpened = false;
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
