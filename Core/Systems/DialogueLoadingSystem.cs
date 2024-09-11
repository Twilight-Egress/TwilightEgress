using CalamityMod.Items.Weapons.Melee;
using MonoMod.Utils;
using DialogueHelper.Content.UI.Dialogue;
using Cascade.Content.UI.Dialogue.DialogueStyles;

namespace Cascade.Core.Systems
{
    public class DialogueLoadingSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            string LocalizationPath = "Mods.Cascade.UI.Dialogue.";
            DialogueHolder.CharacterAssetPathes.Add($"{nameof(Cascade)}", $"{nameof(Cascade)}/Assets/CharacterAssets");

            Dictionary<string, Character> Characters = new()
            {
                { 
                    "Cascade/TheCalamity", 
                    new Character
                    (
                        "[c/FF0000:The Calamity]",
                        [
                            new Expression("Normal", 1, 0), 
                            new Expression("Finality",  1, 0)
                        ],                         
                        1.5f, 
                        primaryColor: Color.Black, 
                        secondaryColor: Color.Red
                    )
                },
                { 
                    "Cascade/Ardiena", 
                    new Character
                    (
                        "Ardiena",
                        [
                            new Expression("Default",  1, 0), 
                            new Expression("Blunt",  3, 10), 
                            new Expression("DefaultOpen",  1, 0), 
                            new Expression("Frown",  1, 0), new Expression("FrownOpen",  1, 0), 
                            new Expression("Surprise",  1, 0)
                        ],                          
                        scale: 1f, 
                        style: typeof(ArdiDialogueStyle), 
                        primaryColor: Color.Orange, 
                        secondaryColor: Color.Violet
                        ) 
                }
            };

            DialogueHolder.Characters.AddRange(Characters);

            Mod calamityMusic = ModLoader.GetMod("CalamityModMusic");
            Dictionary<string, DialogueTree> dialogueTrees = new()
            {
                #region The Calamity Trees
                {
                    "Cascade/Calamitous",
                    new DialogueTree
                    (
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
                            "Cascade/TheCalamity"
                        ],
                        LocalizationPath
                    )
                },
                #endregion
                #region Ardiena Trees
                {
                    "Cascade/Eeveelutions",
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
                                musicID: MusicLoader.GetMusicSlot(Cascade.Instance, "Assets/Sounds/Music/ArdienaTheme")
                            ),
                            new Dialogue
                            (
                                [
                                    new Response("Thanks")
                                ],
                                expressionIndex: 1,
                                musicID: MusicLoader.GetMusicSlot(Cascade.Instance, "Assets/Sounds/Music/ArdienaTheme")
                            ),
                            new Dialogue
                            (
                                [
                                    new Response("Thanks")
                                ],
                                expressionIndex: 5,
                                musicID: MusicLoader.GetMusicSlot(Cascade.Instance, "Assets/Sounds/Music/ArdienaTheme")
                            ),
                            new Dialogue
                            (
                                [
                                    new Response("Oh"),
                                    new Response("DidYouKnow", 4, dismissSubSpeaker: true)
                                ],
                                expressionIndex: 3,
                                musicID: MusicLoader.GetMusicSlot(Cascade.Instance, "Assets/Sounds/Music/ArdienaTheme")
                            ),
                            new Dialogue
                            (
                                [
                                    new Response("Who", 5)
                                ],
                                characterIndex: 1,
                                expressionIndex: 0,
                                musicID: MusicLoader.GetMusicSlot(calamityMusic, "Sounds/Music/CalamitasClone")
                            ),
                            new Dialogue
                            (
                                characterIndex: 0,
                                expressionIndex: 4
                            ),
                            new Dialogue
                            (
                                characterIndex: 0,
                                expressionIndex: 3
                            ),
                            new Dialogue
                            (
                                [
                                    new Response("Weirdo", -1)
                                ],
                                characterIndex: 1,
                                expressionIndex: 0
                            ),
                        ],
                        [
                            "Cascade/Ardiena",
                            "Cascade/TheCalamity",                            
                        ],
                        LocalizationPath
                    )
                }
                #endregion
            };

            DialogueHolder.DialogueTrees.AddRange(dialogueTrees);
        }
    }
}
