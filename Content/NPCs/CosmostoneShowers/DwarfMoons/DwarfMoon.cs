using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core.Systems.OrbitalGravitySystem;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.DwarfMoons
{
    public abstract class DwarfMoon : ModNPC, ISpawnAvoidZone
    {
        public float GravitationalVariable = 0f;

        public OrbitalGravityPlayer ModPlayer = null;

        public ref float StoredMaximumAttractionRadius => ref NPC.localAI[0];

        public ref float StoredWalkableRadius => ref NPC.localAI[1];

        public abstract float MaximumAttractionRadius { get; }

        public abstract float WalkableRadius { get; }

        public float RadiusCovered => (MaximumAttractionRadius + WalkableRadius) * 1.5f;

        public Vector2 Position => NPC.Center;

        public bool Active => NPC.active;

        public sealed override string LocalizationCategory => "NPCs.CosmostoneShowers.Planetoids";

        public sealed override void SetStaticDefaults()
        {
            NPCID.Sets.ProjectileNPC[Type] = true;
            NPCID.Sets.CannotDropSouls[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TeleportationImmune[Type] = true;
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
            SafeSetStaticDefaults();
        }

        public sealed override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            SafeSetDefaults();
        }

        public sealed override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < 2; i++)
                writer.Write(NPC.localAI[i]);
            SafeSendExtraData(writer);
        }

        public sealed override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < 2; i++)
                NPC.localAI[i] = reader.ReadSingle();
            SafeReceiveExtraAI(reader);
        }

        public override bool CheckActive() => false;

        public sealed override bool PreAI()
        {
            // Loop through all players and search for those who are active and within distance of a planetoid.
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active)
                    continue;

                ModPlayer = player.GetModPlayer<OrbitalGravityPlayer>();

                float totalAttractionRadius = MaximumAttractionRadius + WalkableRadius;
                float distanceBetweenBodies = Vector2.Distance(player.Center, NPC.Center);

                if (distanceBetweenBodies < totalAttractionRadius && ModPlayer.AttractionCooldown <= 0 && ModPlayer.DwarfMoon is null)
                {
                    // Set the planetoid index and player angle.
                    ModPlayer.DwarfMoon = this;
                    ModPlayer.PlayerAngle = (player.Center - NPC.Center).ToRotation();
                    NPC.netUpdate = true;
                }

                // Despawn if the player is too far away.
                if (distanceBetweenBodies >= totalAttractionRadius + 2500f)
                {
                    NPC.active = false;
                    NPC.checkDead();
                    NPC.netUpdate = true;
                }
            }

            // Store these values for access outside of this Base class.
            StoredMaximumAttractionRadius = MaximumAttractionRadius;
            StoredWalkableRadius = WalkableRadius;

            return true;
        }

        public sealed override void AI()
        {
            if (ModPlayer.DwarfMoon is not null && ModPlayer.DwarfMoon == this)
            {
                // Increment the gravitational variable slowly. This will give the gravity the player experiences a more
                // gradual effect, to give them a more realistic feeling of being pulled into a planet's atmosphere.
                // Full equation is explained in OrbitalGravityPlayer.
                GravitationalVariable += GravitationalIncrement;
                GravitationalVariable = MathHelper.Clamp(GravitationalVariable, 0f, MaxGravitationalIncrease);
            }
            else
            {
                // Reset if this planetoid instance is not being utilized at the moment.
                GravitationalVariable = MathHelper.Clamp(GravitationalVariable - 1f, 0f, MaxGravitationalIncrease);
            }

            if (!SafePreAI())
                return;
            SafeAI();
        }

        public virtual float MaxGravitationalIncrease => 4f;

        public virtual float GravitationalIncrement => 0.2f;

        public virtual float PlanetoidEjectionSpeed => 12f;

        public virtual Vector2? GetWalkablePlanetoidPosition(Player player) => null;

        public virtual void SafeSetStaticDefaults() { }

        public virtual void SafeSetDefaults() { }

        public virtual void SafeSendExtraData(BinaryWriter writer) { }

        public virtual void SafeReceiveExtraAI(BinaryReader reader) { }

        public virtual bool SafePreAI() => true;

        public virtual void SafeAI() { }
    }
}
