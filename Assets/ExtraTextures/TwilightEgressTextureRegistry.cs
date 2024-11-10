namespace TwilightEgress.Assets.ExtraTextures
{
    public static class TwilightEgressTextureRegistry
    {
        // Please keep things in alphabetical order.
        // - fryzahh

        #region Objects
        public static readonly LazyAsset<Texture2D> EmptyPixel = MiscTexturesRegistry.InvisiblePixel;

        public static readonly Asset<Texture2D> GreyscaleVortex = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/GreyscaleVortex");

        public static readonly Asset<Texture2D> SoftStar = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/SoftStar");

        #region Lists
        public static readonly List<string> FourPointedStars = new()
        {
            "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Small",
            "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Small_2",
            "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Medium",
            "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Medium_2",
            "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Large",
            "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/FourPointedStar_Large_2"
        };

        public static readonly List<string> FourPointedStars_Atlas = new()
        {
            "TwilightEgress.FourPointedStar_Small.png",
            "TwilightEgress.FourPointedStar_Small_2.png",
            "TwilightEgress.FourPointedStar_Medium.png",
            "TwilightEgress.FourPointedStar_Medium_2.png",
            "TwilightEgress.FourPointedStar_Large.png",
            "TwilightEgress.FourPointedStar_Large_2.png"
        };

        public static readonly List<string> Smokes = new()
        {
            "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/SmokeCloud",
            "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/SmokeCloud2",
            "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/SmokeCloud3",
            "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/SmokeCloud4",
            "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/SmokeCloud5",
            "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/SmokeCloud6"
        };
        #endregion
        #endregion

        #region Gradients
        public static readonly Asset<Texture2D> BlueCosmicGalaxy = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/BlueCosmicGalaxy");

        public static readonly Asset<Texture2D> BlueCosmicGalaxyBlurred = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/BlueCosmicGalaxyBlurred");

        public static readonly Asset<Texture2D> CosmostoneShowersNebulaColors = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/CosmostoneShowersNebulaColors");

        public static readonly Asset<Texture2D> GrainyNoise = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/GrainyNoise");

        public static readonly Asset<Texture2D> MeltyNoise = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/MeltyNoise");

        public static readonly Asset<Texture2D> NeuronNebulaGalaxy = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/NeuronNebulaGalaxy");

        public static readonly Asset<Texture2D> NeuronNebulaGalaxyBlurred = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/NeuronNebulaGalaxyBlurred");

        public static readonly Asset<Texture2D> PerlinNoise = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/PerlinNoise");

        public static readonly Asset<Texture2D> PerlinNoise2 = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/PerlinNoise2");

        public static readonly Asset<Texture2D> PerlinNoise3 = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/PerlinNoise3");

        public static readonly Asset<Texture2D> PerlinNoise4 = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/PerlinNoise4");

        public static readonly Asset<Texture2D> PurpleBlueNebulaGalaxy = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/PurpleBlueNebulaGalaxy");

        public static readonly Asset<Texture2D> PurpleBlueNebulaGalaxyBlurred = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/PurpleBlueNebulaGalaxyBlurred");

        public static readonly Asset<Texture2D> RealisticClouds = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/RealisticClouds");

        public static readonly Asset<Texture2D> SmudgyNoise = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/SmudgyNoise");

        public static readonly Asset<Texture2D> StarryGalaxy = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/StarryGalaxy");

        public static readonly Asset<Texture2D> SwirlyNoise = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Gradients/SwirlyNoise");
        #endregion

        #region Trails
        public static readonly Asset<Texture2D> FadedStreak = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Trails/FadedStreak");

        public static readonly Asset<Texture2D> FlameStreak = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Trails/FlameStreak");

        public static readonly Asset<Texture2D> GenericStreak = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Trails/GenericStreak");

        public static readonly Asset<Texture2D> LightningStreak = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Trails/LightningStreak");

        public static readonly Asset<Texture2D> LightStreak = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Trails/LightStreak");

        public static readonly Asset<Texture2D> MagicStreak = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Trails/MagicStreak");

        public static readonly Asset<Texture2D> SwordSmearStreak = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Trails/SwordSmearStreak");

        public static readonly Asset<Texture2D> ThinGlowStreak = ModContent.Request<Texture2D>("TwilightEgress/Assets/ExtraTextures/Trails/ThinGlowStreak");
        #endregion
    }
}
