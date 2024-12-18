using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Core.Graphics.Primitives;
using TwilightEgress.Core.Physics.Gravity;

namespace TwilightEgress.Core.Systems.OrbitalGravitySystem
{
    public class PlanetoidSystem : ModSystem
    {
        public static MassiveObject[] planetoids;

        public override void Load()
        {
            On_Main.DrawPlayers_AfterProjectiles += DrawPrims;
        }

        public override void Unload()
        {
            On_Main.DrawPlayers_AfterProjectiles -= DrawPrims;
        }

        public override void PreUpdatePlayers()
        {
            //PrimitiveBatch.Instance.Update();
        }

        private void DrawPrims(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);
            PrimitiveBatch.Instance.DrawPrimitives();
        }
    }
}
