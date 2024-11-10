namespace TwilightEgress.Assets.Sounds
{
    // Why does regular calamity not have some type of proper sound registry grahhhhh
    // no im NOT going to cache them as fields in each individual npc file i want EASY ACCESS TO ALL ACROSS THE ENTIRE PROJECT.
    // frickyoy john calamity

    public static class TwilightEgressSoundRegistry
    {
        #region TwilightEgress Sounds

        public static readonly SoundStyle GasterGone = new SoundStyle("TwilightEgress/Assets/Sounds/Misc/MysteryManDisappear");

        public static readonly SoundStyle UndertaleExplosion = new SoundStyle("TwilightEgress/Assets/Sounds/Misc/UndertaleExplosion");

        public static readonly SoundStyle PokemonThunderbolt = new SoundStyle("TwilightEgress/Assets/Sounds/Items/MarvWeapon/PokemonThunderbolt");

        public static readonly SoundStyle SuperEffective = new SoundStyle("TwilightEgress/Assets/Sounds/Items/MarvWeapon/SuperEffective");

        public static readonly SoundStyle PikachuCry = new SoundStyle("TwilightEgress/Assets/Sounds/Items/MarvWeapon/PikachuCry");

        public static readonly SoundStyle ZekromCry = new SoundStyle("TwilightEgress/Assets/Sounds/Items/MarvWeapon/ZekromCry");

        public static readonly SoundStyle BellbirdChirp = new SoundStyle("TwilightEgress/Assets/Sounds/Items/LynelBirb/NotSoStunningBellbirdScream");

        public static readonly SoundStyle BellbirdStunningScream = new SoundStyle("TwilightEgress/Assets/Sounds/Items/LynelBirb/TheCryOfGod");

        public static readonly SoundStyle AsrielTargetBeep = new SoundStyle("TwilightEgress/Assets/Sounds/Items/JacobWeapon/DraedonBombBeep");

        public static readonly SoundStyle AnvilHit = new SoundStyle("TwilightEgress/Assets/Sounds/Items/JacobWeapon/AnvilCompleteHit");

        public static readonly SoundStyle RequiemBouquetPerish = new SoundStyle("TwilightEgress/Assets/Sounds/Items/MPGWeapon/RequiemBouquetPerish");

        public static readonly SoundStyle IceShock = new SoundStyle("TwilightEgress/Assets/Sounds/Items/IceShock");

        public static readonly SoundStyle IceShockPetrify =  new SoundStyle("TwilightEgress/Assets/Sounds/Items/IceShockPetrify");

        public static readonly SoundStyle FleshySwordStab = new SoundStyle("TwilightEgress/Assets/Sounds/Misc/FleshySwordStab");

        public static readonly SoundStyle FleshySwordStab2 = new SoundStyle("TwilightEgress/Assets/Sounds/Misc/FleshySwordStab2");

        public static readonly SoundStyle FleshySwordStab3 = new SoundStyle("TwilightEgress/Assets/Sounds/Misc/FleshySwordStab3");

        public static readonly SoundStyle FleshySwordRip = new SoundStyle("TwilightEgress/Assets/Sounds/Misc/FleshySwordRip");

        public static readonly SoundStyle FleshySwordRip2 = new SoundStyle("TwilightEgress/Assets/Sounds/Misc/FleshySwordRip2");

        public static readonly SoundStyle FlytrapMawSpawn = new SoundStyle("TwilightEgress/Assets/Sounds/Items/RaeshWeapon/FlytrapMawSpawn");

        public static readonly SoundStyle FlytrapMawBounce = new SoundStyle("TwilightEgress/Assets/Sounds/Items/RaeshWeapon/FlytrapMawBounce");

        public static readonly SoundStyle KibbyExplosion = new SoundStyle("TwilightEgress/Assets/Sounds/Items/KibbyExplosion");

        #endregion

        #region Vanilla Calamity Custom Sounds

        public static readonly SoundStyle HiveMindRoar = new SoundStyle("CalamityMod/Sounds/Custom/HiveMindRoar");

        public static readonly SoundStyle HiveMindFastRoar = new SoundStyle("CalamityMod/Sounds/Custom/HiveMindRoarFast");

        public static readonly SoundStyle PolterghastSpawn = new SoundStyle("CalamityMod/Sounds/Custom/PolterghastSpawn");

        public static readonly SoundStyle PolterghastPhantomSpawn = new SoundStyle("CalamityMod/Sounds/Custom/PolterghastPhantomSpawn");

        /// <summary>
        /// When Polterghast transitions into Necroghast, phase two of the Polterghast fight.
        /// </summary>
        public static readonly SoundStyle PolterghastPhaseTransition = new SoundStyle("CalamityMod/Sounds/Custom/PolterghastP2Transition");

        /// <summary>
        /// When Necroghast transitions into Necroplasm, phasee three of the Polterghast fight.
        /// </summary>
        public static readonly SoundStyle NecroghastPhaseTransition = new SoundStyle("CalamityMod/Sounds/Custom/PolterghastP3Transition");

        public static readonly SoundStyle PolterghastHitSound = new SoundStyle("CalamityMod/Sounds/NPCHit/PolterghastHit");

        public static readonly SoundStyle YharonHurt = new SoundStyle("CalamityMod/Sounds/NPCHit/YharonHurt");

        public static readonly SoundStyle YharonRoar = new SoundStyle("CalamityMod/Sounds/Custom/Yharon/YharonRoar");

        public static readonly SoundStyle YharonRoarShort = new SoundStyle("CalamityMod/Sounds/Custom/Yharon/YharonRoarShort");

        public static readonly SoundStyle YharonFireBreath = new SoundStyle("CalamityMod/Sounds/Custom/Yharon/YharonFire");

        public static readonly SoundStyle CryogenHurt1 = new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenHit1");

        public static readonly SoundStyle CryogenHurt2 = new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenHit2");

        public static readonly SoundStyle CryogenHurt3 = new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenHit3");

        public static readonly SoundStyle CryogenPhaseTransitionCrack = new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenPhaseTransitionCrack");

        public static readonly SoundStyle CryogenShieldRegenerate = new SoundStyle("CalamityMod/Sounds/Custom/CryogenShieldRegenerate");

        public static readonly SoundStyle CryogenShieldBreak = new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenShieldBreak");

        public static readonly SoundStyle CryogenDeath = new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenDeath");

        #endregion

        #region Commonly Used Vanilla Sounds

        public static readonly SoundStyle PerforatorHiveIchorBlobs = SoundID.NPCDeath23;

        /// <summary>
        /// Used for when things like Polterghast and its minions are killed.
        /// </summary>
        public static readonly SoundStyle PhantomDeath = SoundID.NPCDeath39;

        #endregion
    }
}
