namespace UnicornHack.Models.GameDefinitions
{
    public enum SimpleActorPropertyType : byte
    {
        Default = 0,
        // Boons
        FireResistance,
        ColdResistance,
        ElectricityResistance,
        AcidResistance,
        VenomResistance,
        DisintegrationResistance,
        DrainResistance,
        SicknessResistance,
        StoningResistance,
        SlimingResistance,
        SleepResistance,
        FreeAction,
        Reflection,
        Protection,
        ProtectionFromShapeChangers,
        MagicalBreathing, // TODO: Replace with breathless?
        LifeSaving,
        Invulnerability,
        // Senses
        InvisibilityDetection,
        Infravision,
        Telepathy,
        Clairvoyance,
        DangerAwareness,
        Perception, // Can detect hidden monsters
        // Impairments
        Sleepiness,
        Stuning,
        Confusion,
        Hallucination,
        Blindness,
        Deafness,
        Vomiting,
        Sickness,
        Stoning,
        Sliming,
        Suffocating,
        Slickness,
        Clumsiness,
        // Appearance
        Invisibility,
        Infravisibility,
        Displacement,
        Stealthiness,
        Concealment,
        Camouflage,
        // Behavior alteration
        MonsterAggravation,
        Conflict,
        // Transportation
        Jumping,
        Teleportation,
        TeleportationControl,
        Levitation,
        Flight,
        WaterWalking,
        Swimming,
        Phasing,
        Amorphism,
        Clinginess,
        Tunneling,
        ToolTunneling,
        // Nutrition
        SlowDigestion,
        Hunger,
        PoisonResistance,
        FoodPoisoning,
        Carnivorism,
        Herbivorism,
        Omnivorism,
        Metallivorism,
        // Physical attributes
        SilverWeakness,
        WaterWeakness,
        Regeneration,
        EnergyRegeneration,
        Polymorphism,
        PolymorphControl,
        Unchanging,
        AbilitySustainment,
        ThickHide,
        Amphibiousness,
        Breathlessness,
        NoInventory,
        Eyelessness,
        Handlessness,
        Limblessness,
        Headlessness,
        Mindlessness,
        Humanoidness,
        HumanoidTorso,
        AnimalBody,
        SerpentlikeBody,
        NonSolidBody,
        NonAnimal,
        NoCorpse,
        DecayResistance,
        Reanimation,
        // Reproduction
        Asexuality,
        Maleness,
        Femaleness,
        Oviparity
    }
}