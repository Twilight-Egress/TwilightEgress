using Luminance.Assets;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;

namespace TwilightEgress.Assets 
{
    public class AssetRegistry : ModSystem 
    {
        // Please keep things in alphabetical order.

        public static readonly string ExtraTexturesPath = $"{nameof(TwilightEgress)}/Assets/ExtraTextures/";
        public static readonly string SoundsPath = $"{nameof(TwilightEgress)}/Assets/Sounds/";

        public static class Textures
        {
            #region Objects
            public static readonly Asset<Texture2D> GreyscaleVortex = ModContent.Request<Texture2D>(ExtraTexturesPath + "GreyscaleObjects/GreyscaleVortex");
            public static readonly Asset<Texture2D> SoftStar = ModContent.Request<Texture2D>(ExtraTexturesPath + "GreyscaleObjects/SoftStar");

            public static readonly LazyAsset<Texture2D> EmptyPixel = MiscTexturesRegistry.InvisiblePixel;
            public static readonly LazyAsset<Texture2D> Pixel = MiscTexturesRegistry.Pixel;

            #region Lists
            public static readonly List<string> FourPointedStars =
            [
                ExtraTexturesPath + "GreyscaleObjects/FourPointedStar_Small",
                ExtraTexturesPath + "GreyscaleObjects/FourPointedStar_Small_2",
                ExtraTexturesPath + "GreyscaleObjects/FourPointedStar_Medium",
                ExtraTexturesPath + "GreyscaleObjects/FourPointedStar_Medium_2",
                ExtraTexturesPath + "GreyscaleObjects/FourPointedStar_Large",
                ExtraTexturesPath + "GreyscaleObjects/FourPointedStar_Large_2"
            ];

            public static readonly List<string> FourPointedStars_Atlas =
            [
                "TwilightEgress.FourPointedStar_Small.png",
                "TwilightEgress.FourPointedStar_Small_2.png",
                "TwilightEgress.FourPointedStar_Medium.png",
                "TwilightEgress.FourPointedStar_Medium_2.png",
                "TwilightEgress.FourPointedStar_Large.png",
                "TwilightEgress.FourPointedStar_Large_2.png"
            ];

            public static readonly List<string> Smokes =
            [
                ExtraTexturesPath + "GreyscaleObjects/SmokeCloud",
                ExtraTexturesPath + "GreyscaleObjects/SmokeCloud2",
                ExtraTexturesPath + "GreyscaleObjects/SmokeCloud3",
                ExtraTexturesPath + "GreyscaleObjects/SmokeCloud4",
                ExtraTexturesPath + "GreyscaleObjects/SmokeCloud5",
                ExtraTexturesPath + "GreyscaleObjects/SmokeCloud6"
            ];
            #endregion
            #endregion

            #region Gradients
            public static readonly Asset<Texture2D> BlueCosmicGalaxy = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/BlueCosmicGalaxy");

            public static readonly Asset<Texture2D> BlueCosmicGalaxyBlurred = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/BlueCosmicGalaxyBlurred");

            public static readonly Asset<Texture2D> CosmostoneShowersNebulaColors = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/CosmostoneShowersNebulaColors");

            public static readonly Asset<Texture2D> GrainyNoise = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/GrainyNoise");

            public static readonly Asset<Texture2D> MeltyNoise = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/MeltyNoise");

            public static readonly Asset<Texture2D> NeuronNebulaGalaxy = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/NeuronNebulaGalaxy");

            public static readonly Asset<Texture2D> NeuronNebulaGalaxyBlurred = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/NeuronNebulaGalaxyBlurred");

            public static readonly Asset<Texture2D> PerlinNoise = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/PerlinNoise");

            public static readonly Asset<Texture2D> PerlinNoise2 = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/PerlinNoise2");

            public static readonly Asset<Texture2D> PerlinNoise3 = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/PerlinNoise3");

            public static readonly Asset<Texture2D> PerlinNoise4 = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/PerlinNoise4");

            public static readonly Asset<Texture2D> PurpleBlueNebulaGalaxy = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/PurpleBlueNebulaGalaxy");

            public static readonly Asset<Texture2D> PurpleBlueNebulaGalaxyBlurred = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/PurpleBlueNebulaGalaxyBlurred");

            public static readonly Asset<Texture2D> RealisticClouds = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/RealisticClouds");

            public static readonly Asset<Texture2D> SmudgyNoise = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/SmudgyNoise");

            public static readonly Asset<Texture2D> StarryGalaxy = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/StarryGalaxy");

            public static readonly Asset<Texture2D> SwirlyNoise = ModContent.Request<Texture2D>(ExtraTexturesPath + "Gradients/SwirlyNoise");
            #endregion

            #region Trails
            public static readonly Asset<Texture2D> FadedStreak = ModContent.Request<Texture2D>(ExtraTexturesPath + "Trails/FadedStreak");

            public static readonly Asset<Texture2D> FlameStreak = ModContent.Request<Texture2D>(ExtraTexturesPath + "Trails/FlameStreak");

            public static readonly Asset<Texture2D> GenericStreak = ModContent.Request<Texture2D>(ExtraTexturesPath + "Trails/GenericStreak");

            public static readonly Asset<Texture2D> LightningStreak = ModContent.Request<Texture2D>(ExtraTexturesPath + "Trails/LightningStreak");

            public static readonly Asset<Texture2D> LightStreak = ModContent.Request<Texture2D>(ExtraTexturesPath + "Trails/LightStreak");

            public static readonly Asset<Texture2D> MagicStreak = ModContent.Request<Texture2D>(ExtraTexturesPath + "Trails/MagicStreak");

            public static readonly Asset<Texture2D> SwordSmearStreak = ModContent.Request<Texture2D>(ExtraTexturesPath + "Trails/SwordSmearStreak");

            public static readonly Asset<Texture2D> ThinGlowStreak = ModContent.Request<Texture2D>(ExtraTexturesPath + "Trails/ThinGlowStreak");
            #endregion
        }

        public static class Sounds 
        {
            #region Twilight Egress Sounds

            public static readonly SoundStyle GasterGone = new SoundStyle(SoundsPath + "Misc/MysteryManDisappear");

            public static readonly SoundStyle UndertaleExplosion = new SoundStyle(SoundsPath + "Misc/UndertaleExplosion");

            public static readonly SoundStyle PokemonThunderbolt = new SoundStyle(SoundsPath + "Items/MarvWeapon/PokemonThunderbolt");

            public static readonly SoundStyle SuperEffective = new SoundStyle(SoundsPath + "Items/MarvWeapon/SuperEffective");

            public static readonly SoundStyle PikachuCry = new SoundStyle(SoundsPath + "Items/MarvWeapon/PikachuCry");

            public static readonly SoundStyle ZekromCry = new SoundStyle(SoundsPath + "Items/MarvWeapon/ZekromCry");

            public static readonly SoundStyle BellbirdChirp = new SoundStyle(SoundsPath + "Items/LynelBirb/NotSoStunningBellbirdScream");

            public static readonly SoundStyle BellbirdStunningScream = new SoundStyle(SoundsPath + "Items/LynelBirb/TheCryOfGod");

            public static readonly SoundStyle AsrielTargetBeep = new SoundStyle(SoundsPath + "Items/JacobWeapon/DraedonBombBeep");

            public static readonly SoundStyle AnvilHit = new SoundStyle(SoundsPath + "Items/JacobWeapon/AnvilCompleteHit");

            public static readonly SoundStyle RequiemBouquetPerish = new SoundStyle(SoundsPath + "Items/MPGWeapon/RequiemBouquetPerish");

            public static readonly SoundStyle IceShock = new SoundStyle(SoundsPath + "Items/IceShock");

            public static readonly SoundStyle IceShockPetrify = new SoundStyle(SoundsPath + "Items/IceShockPetrify");

            public static readonly SoundStyle FleshySwordStab = new SoundStyle(SoundsPath + "Misc/FleshySwordStab");

            public static readonly SoundStyle FleshySwordStab2 = new SoundStyle(SoundsPath + "Misc/FleshySwordStab2");

            public static readonly SoundStyle FleshySwordStab3 = new SoundStyle(SoundsPath + "Misc/FleshySwordStab3");

            public static readonly SoundStyle FleshySwordRip = new SoundStyle(SoundsPath + "Misc/FleshySwordRip");

            public static readonly SoundStyle FleshySwordRip2 = new SoundStyle(SoundsPath + "Misc/FleshySwordRip2");

            public static readonly SoundStyle FlytrapMawSpawn = new SoundStyle(SoundsPath + "Items/RaeshWeapon/FlytrapMawSpawn");

            public static readonly SoundStyle FlytrapMawBounce = new SoundStyle(SoundsPath + "Items/RaeshWeapon/FlytrapMawBounce");

            public static readonly SoundStyle KibbyExplosion = new SoundStyle(SoundsPath + "Items/KibbyExplosion");

            #endregion

            #region Calamity Custom Sounds

            public static readonly SoundStyle YharonHurt = new SoundStyle("CalamityMod/Sounds/NPCHit/YharonHurt");

            public static readonly SoundStyle YharonRoar = new SoundStyle("CalamityMod/Sounds/Custom/Yharon/YharonRoar");

            public static readonly SoundStyle YharonRoarShort = new SoundStyle("CalamityMod/Sounds/Custom/Yharon/YharonRoarShort");

            public static readonly SoundStyle YharonFireBreath = new SoundStyle("CalamityMod/Sounds/Custom/Yharon/YharonFire");

            public static readonly SoundStyle CryogenShieldBreak = new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenShieldBreak");

            #endregion

            #region Commonly Used Vanilla Sounds

            public static readonly SoundStyle PerforatorHiveIchorBlobs = SoundID.NPCDeath23;

            /// <summary>
            /// Used for when things like Polterghast and its minions are killed.
            /// </summary>
            public static readonly SoundStyle PhantomDeath = SoundID.NPCDeath39;

            #endregion
        }

        public static class Effects 
        {

        }
    }
}
