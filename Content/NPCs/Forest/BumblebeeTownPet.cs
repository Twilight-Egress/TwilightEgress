using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.Utilities;

namespace TwilightEgress.Content.NPCs.Forest
{
    [AutoloadHead]
    public class BumblebeeTownPet : ModNPC
    {
        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        private static ITownNPCProfile NPCProfile;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
            NPCID.Sets.ExtraFramesCount[Type] = 0;
            NPCID.Sets.AttackFrameCount[Type] = 0;
            NPCID.Sets.DangerDetectRange[Type] = 250;
            NPCID.Sets.HatOffsetY[Type] = -2;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Shimmer] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.NPCFramingGroup[Type] = 8;

            NPCID.Sets.IsTownPet[Type] = true;
            NPCID.Sets.TownNPCBestiaryPriority.Add(Type);
            NPCID.Sets.PlayerDistanceWhilePetting[Type] = 30;
            NPCID.Sets.IsPetSmallForPetting[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 0.25f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCProfile = new BumblebeeTownNPCProfile();
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 20;
            NPC.height = 20;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.knockBackResist = 0.5f;
            NPC.housingCategory = 1;
            AnimationType = NPCID.TownBunny;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs) => true;

        public override ITownNPCProfile TownNPCProfile() => NPCProfile;

        public override List<string> SetNPCNameList() => ["Barry B. Benson"];

        public override void SetChatButtons(ref string button, ref string button2) => button = Language.GetTextValue("UI.PetTheAnimal");

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange
            ([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.TwilightEgress.Bestiary.BumblebeeTownPet")
            ]);
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();

            chat.Add("*Buzzing*");
            chat.Add("*Buzz buzz*");
            chat.Add("*Bzzzzzzzzzzzzz*");
            chat.Add("*Buzzing noises*");
            chat.Add("*Bee noises*");
            chat.Add("*Bumblebee noises*");
            chat.Add("*Bzzz bzzz*");

            return chat;
        }

        public override void FindFrame(int frameHeight)
        {
            // laziest possible way to do this
            if (NPC.velocity.X == 0)
                NPC.frame.Y = 0;
            else 
                NPC.frame.Y = NPC.frame.Y % (frameHeight * 5);
        }
    }

    public class BumblebeeTownNPCProfile : ITownNPCProfile
    {
        private static readonly string filePath = "TwilightEgress/Assets/Textures/NPCs/Forest/BumblebeeTownPet";

        public int RollVariation() => 0;

        public string GetNameForVariant(NPC npc) => npc.getNewNPCName(); // Reroll the name each time the Town Pet spawns or changes variant.

        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) => ModContent.Request<Texture2D>(filePath);

        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot($"{filePath}_Head");
    }
}
