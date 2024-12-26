using CalamityMod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Utilities;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace TwilightEgress.Content.NPCs.Misc
{
    public class PantsEnemy : ModNPC
    {
        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 52;
            NPC.damage = 12;
            NPC.defense = 6;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 3;

            AIType = NPCID.Zombie;
        }
    }
}
