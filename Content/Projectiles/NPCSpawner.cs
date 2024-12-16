using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Assets;

namespace TwilightEgress.Content.Projectiles
{
    public class NPCSpawner : ModProjectile, ILocalizedModType
    {
        public ref float NPCTypeToSpawn => ref Projectile.ai[0];

        public new string LocalizationCategory => "Projectiles";

        public override string Texture => "TwilightEgress/Assets/Textures/Extra/EmptyPixel";

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1;
        }

        public override void OnKill(int timeLeft)
        {
            // Spawn the required NPC on death.
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int n = NPC.NewNPC(new EntitySource_SpawnNPC(), (int)Projectile.Center.X, (int)Projectile.Center.Y, (int)NPCTypeToSpawn);
                if (Main.npc.IndexInRange(n))
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                }
            }
        }
    }
}
