using CalamityMod.Rarities;
using TwilightEgress.Common.Physics.Gravity;
using TwilightEgress.Common.Physics.VerletIntegration;
using Terraria;

namespace TwilightEgress.Core.Systems
{
    public class PlanetoidSystem : ModSystem
    {
        public static MassiveObject[] planetoids;

        public override void Load()
        {
            On_Main.DrawNPCs += DrawPlanetoids;
            planetoids = new MassiveObject[200];
        }

        public override void Unload()
        {
            On_Main.DrawNPCs -= DrawPlanetoids;
            planetoids = null;
        }

        public override void PreUpdatePlayers()
        {
            /*bool createVerlet = (int)(Main.GlobalTimeWrappedHourly * 60) % 60 == 0;
            Player player = Main.player[Main.myPlayer];

            //planetoids = new MassiveObject[200];

            if (createVerlet)
            {
                NewPlanetoid(player.Center - Main.rand.NextVector2Circular(1f, 1f) * 300f, Main.rand.NextVector2Circular(1f, 1f), 47f, 0f, 100f);
            }*/

            planetoids.UpdateVerlets(1);
            planetoids.ApplyGravity(1);
        }

        public static void NewPlanetoid(Vector2 position, Vector2 velocity, float radius, float rotationSpeed, float mass)
        {
            for (int i = 0; i < 200; i++)
            {
                if (planetoids[i] is not null && planetoids[i].Active) continue;

                planetoids[i] = new MassiveObject(position, velocity, radius, rotationSpeed, mass);
                planetoids[i].WhoAmI = i;
                break;
            }
        }

        private void DrawPlanetoids(On_Main.orig_DrawNPCs orig, Main self, bool behildTiles)
        {
            orig(self, behildTiles);

            Texture2D galileoTexture = ModContent.Request<Texture2D>("TwilightEgress/Content/NPCs/CosmostoneShowers/Planetoids/GalileoPlanetoid").Value;

            foreach (MassiveObject? planetoid in planetoids)
            {
                if (planetoid is not null && planetoid.Active)
                    Main.spriteBatch.Draw(galileoTexture, planetoid.Position - Main.screenPosition, galileoTexture.Frame(), Lighting.GetColor((planetoid.Position / 16f).ToPoint()), 0f, galileoTexture.Frame().Size() * 0.5f, planetoid.Radius / 47f, 0, 0f);
            }
        }
    }
}
