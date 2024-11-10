namespace TwilightEgress.Assets {
    public class TwilightEgressAssetRegistry : ModSystem {

        public static readonly string ExtraTexturesPath = $"{nameof(TwilightEgress)}/Assets/ExtraTextures/";

        public static class Textures {
            public static readonly Asset<Texture2D> SoftStar = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/SoftStar");
            public static readonly Asset<Texture2D> GreyscaleVortex = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/GreyscaleVortex");

            public static readonly LazyAsset<Texture2D> EmptyPixel = MiscTexturesRegistry.InvisiblePixel;
        }

        public static class Sounds {

        }

        public static class Effects {

        }
    }
}
