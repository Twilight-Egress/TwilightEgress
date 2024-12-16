using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using TwilightEgress.Assets;
using TwilightEgress.Core.Globals.GlobalNPCs;

namespace TwilightEgress.Content.NPCs.Misc
{
    public class NoxusVision : ModNPC, ILocalizedModType
    {
        public const int GlowingBackIllusionsOutwardnessIndex = 1;

        public const int GlowingBackIllusionsAngleIndex = 2;

        public const int EyeGlareScaleIndex = 3;

        public new string LocalizationCategory => "NPCs.Misc";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetDefaults()
        {
            NPC.width = 283;
            NPC.height = 287;
            NPC.lifeMax = 1;
            NPC.defense = 0;
            NPC.damage = 0;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.dontCountMe = true;
            NPC.noGravity = true;
            NPC.dontTakeDamageFromHostiles = true;
        }

        public override bool CheckDead()
        {
            if (NPC.ai[0] != 1f)
            {
                // ooooooooooo spoooooooky
                SoundEngine.PlaySound(AssetRegistry.Sounds.Misc.MysteryManDisappear with { Volume = 3f }, NPC.Center);
                NPC.ai[0] = 1f;
                NPC.life = 1;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                return false;
            }
            return true;
        }

        public override void AI()
        {
            ref float glowingBackIllusionsAngle = ref NPC.TwilightEgress().ExtraAI[GlowingBackIllusionsAngleIndex];
            ref float glowingBackIllusionsOutwardness = ref NPC.TwilightEgress().ExtraAI[GlowingBackIllusionsOutwardnessIndex];
            ref float eyeGlareScale = ref NPC.TwilightEgress().ExtraAI[EyeGlareScaleIndex];

            // Vanish into thin air.
            if (NPC.ai[0] == 1f)
            {
                NPC.Opacity -= 0.02f;
                NPC.scale += 0.01f;
                eyeGlareScale = MathHelper.Clamp(eyeGlareScale + 1f, NPC.scale / 2f, NPC.scale * 3f);
                glowingBackIllusionsOutwardness = MathHelper.Clamp(glowingBackIllusionsOutwardness + 2f, 12f, 600f);
                if (NPC.Opacity <= 0f)
                {
                    NPC.active = false;
                    return;
                }
            }
            else
            {
                eyeGlareScale = NPC.scale / 2f;
                glowingBackIllusionsOutwardness = 20f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            ref float glowingBackIllusionsAngle = ref NPC.TwilightEgress().ExtraAI[GlowingBackIllusionsAngleIndex];
            ref float glowingBackIllusionsOutwardness = ref NPC.TwilightEgress().ExtraAI[GlowingBackIllusionsOutwardnessIndex];
            ref float eyeGlareScale = ref NPC.TwilightEgress().ExtraAI[EyeGlareScaleIndex];

            Texture2D noxus = TextureAssets.Npc[NPC.type].Value;
            Texture2D eyeGlare = AssetRegistry.Textures.GreyscaleObjects.SoftStar.Value;
            Vector2 drawPosition = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);

            // Lerp between magenta and blue.
            Color backEffectColor = Utilities.ColorSwap(Color.Magenta, Color.DarkBlue, 10f);

            for (int i = 0; i < 8; i++)
            {
                float targetAngle = (float)Main.timeForVisualEffects / 180f * MathHelper.TwoPi;
                glowingBackIllusionsAngle = glowingBackIllusionsAngle.AngleTowards(targetAngle, MathHelper.ToRadians(12f));
                Vector2 backEffectDrawPosition = drawPosition + Vector2.UnitY.RotatedBy(glowingBackIllusionsAngle + MathHelper.TwoPi * i / 8f) * glowingBackIllusionsOutwardness;
                Main.spriteBatch.UseBlendState(BlendState.Additive);
                Main.EntitySpriteDraw(noxus, backEffectDrawPosition, NPC.frame, NPC.GetAlpha(backEffectColor) * 0.45f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
                Main.spriteBatch.ResetToDefault();

            }

            // Draw the main texture.
            Main.EntitySpriteDraw(noxus, drawPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            // Draw the eye glare.
            Vector2 eyeGlareDrawPosition = drawPosition + new Vector2(2f, 10f);
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.EntitySpriteDraw(eyeGlare, eyeGlareDrawPosition, null, NPC.GetAlpha(backEffectColor), NPC.rotation, eyeGlare.Size() / 2f, eyeGlareScale, SpriteEffects.None, 0);
            Main.spriteBatch.ResetToDefault();
            return false;
        }
    }
}
