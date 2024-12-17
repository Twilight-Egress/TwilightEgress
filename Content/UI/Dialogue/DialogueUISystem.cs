﻿using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TwilightEgress.SubModules.DialogueHelper.UI.Dialogue;

namespace TwilightEgress.Content.UI.Dialogue
{
    public static class DialogueHolder
    {
        public static string DebugKey = "Eeveelutions"; //The ID used by the UIDebugItem. Update to the ID of the tree you're testing

        public static readonly string LocalizationPath = "Mods.Cascade.UI.Dialogue.";

        public static readonly Character[] Characters =
        [
            new Character("TheCalamity", [new Expression("Normal", 1, 0), new Expression("Finality",  1, 0)], "[c/FF0000:The Calamity]", styleID: 0),
            new Character("Ardiena", [new Expression("Default",  1, 0), new Expression("Blunt",  3, 10)], styleID: 1)
        ];

        public static Dictionary<string, DialogueTree> DialogueTrees; //Can be marked readonly once testing is done. Isnt so that it can be updated everytime dialogue is called for testing purposes.

        internal enum CharacterIDs
        {
            //Make sure the order of these matches the order of the Characters array
            TheCalamity,
            Ardiena,
        }

        public static Dictionary<string, DialogueTree> PopulateDialogueTrees()
        {
            Mod calamityMusic = ModLoader.GetMod("CalamityModMusic");
            Dictionary<string, DialogueTree> dialogueTrees = new Dictionary<string, DialogueTree>
            {
                #region The Calamity Trees
                {
                    "Calamitous",
                    new DialogueTree(
                    [
                        new Dialogue
                        (
                            [
                                new Response("What", 1, Main.LocalPlayer.direction == 1),
                                new Response("Huh", 1, Main.LocalPlayer.direction == -1),
                            ],
                            expressionIndex: 0,
                            musicID: MusicLoader.GetMusicSlot(calamityMusic, "Sounds/Music/CalamitasClone")
                        ),
                        new Dialogue
                        (
                            [
                                new Response("Okay", -2)
                            ],
                            expressionIndex: 1,
                            musicID: MusicLoader.GetMusicSlot(calamityMusic, "Sounds/Music/CalamitasClone")
                        ),
                        new Dialogue
                        (
                            expressionIndex: 1,
                            musicID: MusicLoader.GetMusicSlot(calamityMusic, "Sounds/Music/CalamitasClone")
                        ),
                        new Dialogue
                        (
                            expressionIndex: 1,
                            musicID: MusicLoader.GetMusicSlot(calamityMusic, "Sounds/Music/CalamitasClone")
                        ),
                    ],
                    [
                        Characters[(int)CharacterIDs.TheCalamity]
                    ])
                },
                #endregion
                #region Ardiena Trees
                {
                    "Eeveelutions",
                    new DialogueTree
                    (
                        [
                            new Dialogue
                            (
                                [
                                    new Response("Flareon", 1),
                                    new Response("Jolteon", 2, cost: new ItemStack(ModContent.ItemType<Exoblade>(), 10)),
                                    new Response("Vaporeon", 3),
                                ],
                                expressionIndex: 0,
                                musicID: MusicLoader.GetMusicSlot(TwilightEgress.Instance, "Assets/Sounds/Music/ArdienaTheme")
                            ),
                            new Dialogue
                            (
                                [
                                    new Response("Thanks")
                                ],
                                expressionIndex: 1,
                                musicID: MusicLoader.GetMusicSlot(TwilightEgress.Instance, "Assets/Sounds/Music/ArdienaTheme")
                            ),
                            new Dialogue
                            (
                                [
                                    new Response("Thanks")
                                ],
                                expressionIndex: 0,
                                musicID: MusicLoader.GetMusicSlot(TwilightEgress.Instance, "Assets/Sounds/Music/ArdienaTheme")
                            ),
                            new Dialogue
                            (
                                [
                                    new Response("Oh"),
                                    new Response("DidYouKnow", 4, dismissSubSpeaker: true)
                                ],
                                expressionIndex: 0,
                                musicID: MusicLoader.GetMusicSlot(TwilightEgress.Instance, "Assets/Sounds/Music/ArdienaTheme")
                            ),
                            new Dialogue
                            (
                                [
                                    new Response("Who", 5)
                                ],
                                1,
                                1,
                                musicID: MusicLoader.GetMusicSlot(calamityMusic, "Sounds/Music/CalamitasClone")
                            ),
                            new Dialogue
                            (
                                characterIndex: 0,
                                expressionIndex: 0
                            ),
                            new Dialogue
                            (
                                characterIndex: 0,
                                expressionIndex: 0
                            ),
                            new Dialogue
                            (
                                [
                                    new Response("Weirdo", -1)
                                ],
                                1,
                                1
                            ),
                        ],
                        [
                            Characters[(int)CharacterIDs.Ardiena],
                            Characters[(int)CharacterIDs.TheCalamity],
                        ]
                    )
                }
                #endregion
            };
            return dialogueTrees;
        }
    }

    public delegate void DialogueNotifier(string treeKey, int dialogueID, int buttonID);

    public class DialogueUISystem : ModSystem
    {
        internal DialogueUIState DialogueUIState;

        public UserInterface DialogueUI;

        public Character? CurrentSpeaker = null;

        public Character? SubSpeaker = null;

        public DialogueNotifier ButtonClick;

        public DialogueNotifier DialogueOpen;

        public DialogueNotifier DialogueClose;

        public bool justOpened = true;

        public bool isDialogueOpen = false;

        public bool newSpeaker = false;

        public bool newSubSpeaker = false;

        public bool returningSpeaker = false;

        public bool dismissSubSpeaker = false;

        public bool speakerRight = true;

        public bool swappingStyle = false;

        public bool styleSwapped = false;

        public int subSpeakerIndex = -1;

        public override void PostSetupContent()
        {
            DialogueHolder.DialogueTrees = DialogueHolder.PopulateDialogueTrees();
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

        public override void UpdateUI(GameTime gameTime)
        {
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

        public void DisplayDialogueTree(string TreeKey, int DialogueIndex = 0)
        {
            /*SoundEngine.PlaySound(UseSound with
            {
                Pitch = -0.25f,
                PitchVariance = 0.5f,
                MaxInstances = 5,
                SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
            });*/
            isDialogueOpen = true;
            justOpened = true;
            speakerRight = true;
            newSpeaker = false;
            newSubSpeaker = false;
            returningSpeaker = false;
            dismissSubSpeaker = false;
            swappingStyle = false;
            styleSwapped = false;

            //Update the DialogueTree array with any new changes (use Hot Reload to apply changes to the function). Use this while testing out your dialogue so you dont have to restart the program every time you add something!
            DialogueHolder.DialogueTrees = DialogueHolder.PopulateDialogueTrees(); //Can be removed once tesating is done
            DialogueTree currentTree = DialogueHolder.DialogueTrees[TreeKey];
            Dialogue currentDialogue = currentTree.Dialogues[DialogueIndex];

            CurrentSpeaker = currentTree.Characters[currentDialogue.CharacterIndex];
            SubSpeaker = null;
            subSpeakerIndex = -1;

            DialogueOpen?.Invoke(TreeKey, DialogueIndex, 0);

            DialogueUI = new UserInterface();
            DialogueUIState = new DialogueUIState();
            DialogueUIState.TreeKey = TreeKey;
            DialogueUIState.DialogueIndex = DialogueIndex;
            DialogueUIState.Activate();

            DialogueUI?.SetState(DialogueUIState);
        }

        public void UpdateDialogueUI(string TreeKey, int DialogueIndex)
        {
            int formerSpeakerIndex = DialogueHolder.DialogueTrees[DialogueUIState.TreeKey].Dialogues[DialogueUIState.DialogueIndex].CharacterIndex;
            DialogueTree currentTree = DialogueHolder.DialogueTrees[TreeKey];
            Dialogue currentDialogue = currentTree.Dialogues[DialogueIndex];
            //Main.NewText("Correct Speaker ID: " + currentTree.Characters[currentDialogue.CharacterIndex].ID);
            //Main.NewText("Current ID: " + CurrentSpeaker.ID);
            //Main.NewText("Subspeaker ID: " + SubSpeaker.ID);

            if (currentTree.Characters[currentDialogue.CharacterIndex].StyleID != ((Character)CurrentSpeaker).StyleID)
                swappingStyle = true;

            if (currentTree.Characters[currentDialogue.CharacterIndex].ID == ((Character)CurrentSpeaker).ID)
            {
                //Main.NewText("Speaker Unchanged");
                newSpeaker = false;
                newSubSpeaker = false;
                returningSpeaker = false;
            }
            else if (SubSpeaker == null)
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
                Character temp = (Character)SubSpeaker;
                SubSpeaker = CurrentSpeaker;
                subSpeakerIndex = formerSpeakerIndex;
                CurrentSpeaker = temp;
                speakerRight = !speakerRight;
            }


            justOpened = false;
            DialogueUI?.SetState(null);
            DialogueUI = new UserInterface();
            DialogueUIState = new DialogueUIState();
            DialogueUIState.TreeKey = TreeKey;
            DialogueUIState.DialogueIndex = DialogueIndex;

            DialogueUI?.SetState(DialogueUIState);
        }

        public void HideDialogueUI()
        {
            /*SoundEngine.PlaySound(UseSound with
            {
                Pitch = -0.25f,
                PitchVariance = 0.5f,
                MaxInstances = 5,
                SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
            });*/
            isDialogueOpen = false;
            DialogueUI?.SetState(null);
        }

    }
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
    public struct Character(string ID, Expression[] expressions, string name = null, float scale = 2f, int styleID = 0, int textDelay = 3)
    {
        public string ID = ID;
        public string Name = name ?? ID;
        public float Scale = scale;
        public Expression[] Expressions = expressions;
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

    /// <param name="responses">The array of <see cref="Response"/>s the player can give. If set to null, clicking on the Textbox itself will proceed to the next Dialogue within the <see cref="DialogueTree"/> or close the Dialogue if there are no more dialogues Defaults to <see cref="null"/>. </param>
    /// <param name="characterIndex">The index of a character within the <see cref="DialogueTree"/>'s <see cref="DialogueTree.Characters"/> array. Represents the character who will be speaking. Defaults to <see cref="0"/>, the first character in the <see cref="DialogueTree.Characters"/> array.</param>
    /// <param name="expressionIndex">The index of an expression within a <see cref="Character"/>'s <see cref="Character.Expressions"/> array. Represents the expression, or asset, the character will use while speaking. Defaults to <see cref="0"/>, the first expression in the <see cref="Character.Expressions"/> array.</param>
    /// <param name="styleID">The ID of the Textbox Style associated with this Dialogue. Is able to differ from that of its Character's Defaults to 0.</param>
    /// <param name="textScaleX">Scales the size of the text horizontally. Defaults to <see cref="1.5f"/>.</param>
    /// <param name="textScaleY">Scales the size of the text vertically. Defaults to <see cref="1.5f"/>.</param>
    /// <param name="textDelay">The Text Delay associated with this character. Affects how long between characters appearing in the Textbox. Defaults to <see cref="-1"/>, causing it to use the delay associated with the current <see cref="Character"/> if no Character is speaking, it defaults to <see cref="3"/>.</param>
    /// <param name="musicID">The ID of the music that will play during this dialogue. This shouldn't change too often throughout a tree. Defaults to <see cref="-1"/>, which wont interupt the current background music. </param>
    /// <returns>
    /// Represents a single dialogue state within a <see cref="DialogueTree"/>.
    /// </returns>
    public struct Dialogue(Response[] responses = null, int characterIndex = 0, int expressionIndex = 0, float textScaleX = 1.5f, float textScaleY = 1.5f, int textDelay = -1, int musicID = -1)
    {
        public Response[] Responses = responses;
        public int CharacterIndex = characterIndex;
        public int ExpressionIndex = expressionIndex;
        public Vector2 TextScale = new(textScaleX, textScaleY);
        public int TextDelay = textDelay;
        public int MusicID = musicID;
    }

    /// <param name="title">The text displayed on the Response Button.</param>
    /// <param name="dialogueIndex">The index within the <see cref="DialogueTree.Dialogues"/>s array this response leads to. Defaults to <see cref="-1"/>, which closes the dialogue.</param>
    /// <param name="requirement">A <see cref="bool"/> which determines whether or not this response can appear as an option Defaults to <see cref="true"/>.</param>
    /// <param name="cost">A <see cref="ItemStack"/> which applys a cost that is needed in order to select that response Defaults to <see cref="null"/>, meaning there is no cost.</param>
    /// <param name="dismissSubSpeaker">A <see cref="bool"/> which will remove the current SubSpeaker, if there is one, from the dialogue when this response is selected.</param>
    /// <returns>
    /// Represents a response the player is able to give to a <see cref="Dialogue"/>./>s.
    /// </returns>
    public struct Response(string title, int dialogueIndex = -1, bool requirement = true, ItemStack? cost = null, bool dismissSubSpeaker = false)
    {
        public string Title = title;
        public int DialogueIndex = dialogueIndex;
        public bool Requirement = requirement;
        public ItemStack? Cost = cost;
        public bool DismissSubSpeaker = dismissSubSpeaker;
    }

    public struct Expression(string title, int frameCount, int frameRate, bool loop = true)
    {
        public string Title = title;
        public int FrameCount = frameCount;
        public int FrameRate = frameRate;
        public bool Loop = loop;
    }

    public struct ItemStack(int id, int stack)
    {
        public int Type = id;
        public int Stack = stack;
    }
    #endregion
}
