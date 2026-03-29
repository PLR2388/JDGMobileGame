using System.Collections.Generic;

/// <summary>
/// Represents a collection of distinct card identifiers.
/// </summary>
public enum CardNames
{
    AlphaMan,
    AlphaVDeGelganech,
    AmiralOméga3,
    ArchibaldVonGrenier,
    ArmureTropLourde,
    Assist,
    AttaqueDeLaTourEiffel,
    Babs,
    BarryRedfield,
    BenzaieJeune,
    Benzaie,
    BlagueInterdite,
    BloueTouche,
    BolossageGratuit,
    BonFlic,
    BébéTerreurNocturne,
    Canarang,
    CanardSignal,
    Canardcity,
    Canardman,
    CaptAinIglou,
    CaptainURSSAF,
    CaroleDuServiceMarketing,
    CassetteVHS,
    Cependant,
    CicatriceMaudite,
    ClaquementDeDoigts,
    ClasseDeSegpa,
    ClichéRaciste,
    ClochetteDe10Grammes,
    ClodoDuCoin,
    ConvocationAuLycée,
    CosplayPourri,
    CroisementDesEffluves,
    CrotteDeNez,
    Daffy,
    DanseDeLaVictoire,
    DavidGnouf,
    DavidGoodenough,
    Deltaplane,
    DemiPizza,
    DictateurSympa,
    DresseurDeBidulmon,
    EnfantDeJuron,
    FantômeDeJaponaise,
    Fatalité,
    FauxRaccord,
    Feuille,
    FiltreDégueulasseFMV,
    Fisti,
    Fistiland,
    ForêtDesElfesSylvains,
    Fourchette,
    Frangipanus,
    GeorgesTuséki,
    GillesValentin,
    Granolax,
    GérardChoixpeau,
    HenryPotdebeurre,
    HitboxBuguée,
    HypnoBoobs,
    Incendie,
    InjonctionDuTribunal,
    InspecteurMagret,
    JacquesChiracEn3D,
    JeanClaude,
    JeanCule,
    JeanGuy,
    JeanLouisLaChaussette,
    JeanMarcSoul,
    JeanMichelBruitages,
    JeanPhilippeHardfist,
    JeuPourri,
    JoueurDuGrenier,
    Jérôme,
    Karim,
    KebabMagique,
    Kechua,
    Kenavo,
    KoaloutreOrnithambasLapinzordNainDeCalifornie,
    LAigleDeLaNuit,
    LElfette,
    LHommeBanane,
    LaDrooogue,
    LaMort,
    LaPetiteFille,
    LeCancer,
    LeChevalierNoir,
    LeGrenier,
    LeHardCorner,
    LeHobbit,
    LeJapon,
    LeMotDePasse,
    LePygargue,
    LePyroBarbare,
    LeSalami,
    LeTripleDwa,
    LeVoisin,
    Lolhitler,
    LycéeMagiqueGeorgesPompidou,
    MagasinDeJeuxVidéoDuCoin,
    MainBionique,
    Maman,
    ManiabilitéPourrie,
    ManuelFerrara,
    MauvaisDoublage,
    MauvaisFlic,
    MauvaiseTraduction,
    Max,
    MechaGranolax,
    MerdeMagiqueEnPlastiqueRose,
    MerdeTournoyanteEnPlastiqueBleu,
    MJCorrompu,
    Mohammad,
    MoïseLePlusGrandDeTousLesHébreux,
    MusiqueDeMegaDrive,
    Nounours,
    PainsAuxRaisinsÀLaPlaceDesMains,
    PapyGrenier,
    ParachuteDoré,
    PassageSecretDansLeMurDuVoisin,
    Patate,
    PatronDInfogrames,
    PetiteCaille,
    PetiteCulotte,
    PetitePortionsDeRiz,
    Player,
    PlumeDePhénix,
    PoignéeDePorte,
    ProfDeSVT,
    ProfesseurHumblebundledore,
    ProfesseurRogueLike,
    PropriétaireDeFistiland,
    QuartDeSmileySurLaMain,
    Repop,
    ReversDuDroit,
    Rémi,
    SailorJustice,
    SalutMétalleux,
    SandrineLePorteManteauExtraterrestre,
    Sangoku,
    SauvegardeEffacée,
    SceauxMagiques,
    SebDuGrenier,
    SheikPoint,
    Slime,
    Spaghetti,
    Squalala,
    StarlightUnicorn,
    SteelbookVide,
    StudioDeDéveloppement,
    StudioDeScénaristesCanadien,
    SylvainFilsDeSylvainRoiDesElfes,
    TatouageGoldé,
    Tentacules,
    TexasGranger,
    Théodule,
    TortureNinja,
    TurboCradozaure,
    UnBonTuyau,
    UnDélicieuxRisotto,
    VieuxSage,
    YoutubeMoney,
    ZozanKebab,
    ÉcranDeFinNul,
    ÉtoufféSousLesJeuxDeMerde
}

/// <summary>
/// Provides a mapping between the CardNames enumeration and their respective human-friendly string representations.
/// </summary>
public static class CardNameMappings
{
    public static readonly Dictionary<CardNames, string> CardNameMap = new Dictionary<CardNames, string>
    {
        {
            CardNames.AlphaMan, "Alpha Man"
        },
        {
            CardNames.AlphaVDeGelganech, "Alpha V De Gelganech"
        },
        {
            CardNames.AmiralOméga3, "Amiral Oméga 3"
        },
        {
            CardNames.ArchibaldVonGrenier, "Archibald Von Grenier"
        },
        {
            CardNames.ArmureTropLourde, "Armure trop lourde"
        },
        {
            CardNames.Assist, "Assist' !"
        },
        {
            CardNames.AttaqueDeLaTourEiffel, "Attaque de la tour Eiffel"
        },
        {
            CardNames.Babs, "Babs"
        },
        {
            CardNames.BarryRedfield, "Barry Redfield"
        },
        {
            CardNames.BenzaieJeune, "Benzaie jeune"
        },
        {
            CardNames.Benzaie, "Benzaie"
        },
        {
            CardNames.BlagueInterdite, "Blague interdite"
        },
        {
            CardNames.BloueTouche, "Bloue touche"
        },
        {
            CardNames.BolossageGratuit, "Bolossage gratuit"
        },
        {
            CardNames.BonFlic, "Bon flic"
        },
        {
            CardNames.BébéTerreurNocturne, "Bébé Terreur-Nocturne"
        },
        {
            CardNames.Canarang, "Canarang"
        },
        {
            CardNames.CanardSignal, "Canard-signal"
        },
        {
            CardNames.Canardcity, "Canardcity"
        },
        {
            CardNames.Canardman, "Canardman"
        },
        {
            CardNames.CaptAinIglou, "Cap'tain Iglou"
        },
        {
            CardNames.CaptainURSSAF, "Captain URSSAF"
        },
        {
            CardNames.CaroleDuServiceMarketing, "Carole du service marketing"
        },
        {
            CardNames.CassetteVHS, "Cassette VHS"
        },
        {
            CardNames.Cependant, "Cependant !"
        },
        {
            CardNames.CicatriceMaudite, "Cicatrice Maudite"
        },
        {
            CardNames.ClaquementDeDoigts, "Claquement de Doigts"
        },
        {
            CardNames.ClasseDeSegpa, "Classe de Segpa"
        },
        {
            CardNames.ClichéRaciste, "Cliché Raciste"
        },
        {
            CardNames.ClochetteDe10Grammes, "Clochette de 10 grammes"
        },
        {
            CardNames.ClodoDuCoin, "Clodo du coin"
        },
        {
            CardNames.ConvocationAuLycée, "Convocation au lycée"
        },
        {
            CardNames.CosplayPourri, "Cosplay pourri"
        },
        {
            CardNames.CroisementDesEffluves, "Croisement des effluves"
        },
        {
            CardNames.CrotteDeNez, "Crotte de nez"
        },
        {
            CardNames.Daffy, "Daffy"
        },
        {
            CardNames.DanseDeLaVictoire, "Danse de la victoire"
        },
        {
            CardNames.DavidGnouf, "David Gnouf"
        },
        {
            CardNames.DavidGoodenough, "David Goodenough"
        },
        {
            CardNames.Deltaplane, "Deltaplane"
        },
        {
            CardNames.DemiPizza, "Demi-pizza"
        },
        {
            CardNames.DictateurSympa, "Dictateur Sympa"
        },
        {
            CardNames.DresseurDeBidulmon, "Dresseur de Bidulmon"
        },
        {
            CardNames.EnfantDeJuron, "Enfant De Juron"
        },
        {
            CardNames.FantômeDeJaponaise, "Fantôme De Japonaise"
        },
        {
            CardNames.Fatalité, "Fatalité"
        },
        {
            CardNames.FauxRaccord, "Faux raccord"
        },
        {
            CardNames.Feuille, "Feuille"
        },
        {
            CardNames.FiltreDégueulasseFMV, "Filtre dégueulasse FMV"
        },
        {
            CardNames.Fisti, "Fisti"
        },
        {
            CardNames.Fistiland, "Fistiland"
        },
        {
            CardNames.ForêtDesElfesSylvains, "Forêt des elfes sylvains"
        },
        {
            CardNames.Fourchette, "Fourchette"
        },
        {
            CardNames.Frangipanus, "Frangipanus"
        },
        {
            CardNames.GeorgesTuséki, "Georges Tuséki"
        },
        {
            CardNames.GillesValentin, "Gilles Valentin"
        },
        {
            CardNames.Granolax, "Granolax"
        },
        {
            CardNames.GérardChoixpeau, "Gérard Choixpeau"
        },
        {
            CardNames.HenryPotdebeurre, "Henry Potdebeurre"
        },
        {
            CardNames.HitboxBuguée, "Hitbox buguée"
        },
        {
            CardNames.HypnoBoobs, "Hypno-boobs"
        },
        {
            CardNames.Incendie, "Incendie"
        },
        {
            CardNames.InjonctionDuTribunal, "Injonction du tribunal"
        },
        {
            CardNames.InspecteurMagret, "Inspecteur Magret"
        },
        {
            CardNames.JacquesChiracEn3D, "Jacques Chirac en 3D"
        },
        {
            CardNames.JeanClaude, "Jean-Claude"
        },
        {
            CardNames.JeanCule, "Jean-Cule"
        },
        {
            CardNames.JeanGuy, "Jean-Guy"
        },
        {
            CardNames.JeanLouisLaChaussette, "Jean-Louis La Chaussette"
        },
        {
            CardNames.JeanMarcSoul, "Jean-Marc Soul"
        },
        {
            CardNames.JeanMichelBruitages, "Jean-Michel Bruitages"
        },
        {
            CardNames.JeanPhilippeHardfist, "Jean-Philippe Hardfist"
        },
        {
            CardNames.JeuPourri, "Jeu pourri"
        },
        {
            CardNames.JoueurDuGrenier, "Joueur Du Grenier"
        },
        {
            CardNames.Jérôme, "Jérôme"
        },
        {
            CardNames.Karim, "Karim"
        },
        {
            CardNames.KebabMagique, "Kebab magique"
        },
        {
            CardNames.Kechua, "Kechua"
        },
        {
            CardNames.Kenavo, "Kenavo !"
        },
        {
            CardNames.KoaloutreOrnithambasLapinzordNainDeCalifornie, "Koaloutre-Ornithambas Lapinzord nain de Californie"
        },
        {
            CardNames.LAigleDeLaNuit, "L'Aigle De La Nuit"
        },
        {
            CardNames.LElfette, "L'Elfette"
        },
        {
            CardNames.LHommeBanane, "L'homme-banane"
        },
        {
            CardNames.LaDrooogue, "La drooogue !"
        },
        {
            CardNames.LaMort, "La Mort"
        },
        {
            CardNames.LaPetiteFille, "La Petite Fille"
        },
        {
            CardNames.LeCancer, "Le cancer"
        },
        {
            CardNames.LeChevalierNoir, "Le chevalier noir"
        },
        {
            CardNames.LeGrenier, "Le grenier"
        },
        {
            CardNames.LeHardCorner, "Le Hard Corner"
        },
        {
            CardNames.LeHobbit, "Le Hobbit"
        },
        {
            CardNames.LeJapon, "Le Japon"
        },
        {
            CardNames.LeMotDePasse, "Le mot de passe"
        },
        {
            CardNames.LePygargue, "Le Pygargue"
        },
        {
            CardNames.LePyroBarbare, "Le Pyro-Barbare"
        },
        {
            CardNames.LeSalami, "Le salami"
        },
        {
            CardNames.LeTripleDwa, "Le triple dwa"
        },
        {
            CardNames.LeVoisin, "Le voisin"
        },
        {
            CardNames.Lolhitler, "Lolhitler"
        },
        {
            CardNames.LycéeMagiqueGeorgesPompidou, "Lycée magique Georges Pompidou"
        },
        {
            CardNames.MagasinDeJeuxVidéoDuCoin, "Magasin de jeux vidéo du coin"
        },
        {
            CardNames.MainBionique, "Main bionique"
        },
        {
            CardNames.Maman, "Maman"
        },
        {
            CardNames.ManiabilitéPourrie, "Maniabilité pourrie"
        },
        {
            CardNames.ManuelFerrara, "Manuel Ferrara"
        },
        {
            CardNames.MauvaisDoublage, "Mauvais doublage"
        },
        {
            CardNames.MauvaisFlic, "Mauvais flic"
        },
        {
            CardNames.MauvaiseTraduction, "Mauvaise traduction"
        },
        {
            CardNames.Max, "Max"
        },
        {
            CardNames.MechaGranolax, "Mecha-Granolax"
        },
        {
            CardNames.MerdeMagiqueEnPlastiqueRose, "Merde magique en plastique rose"
        },
        {
            CardNames.MerdeTournoyanteEnPlastiqueBleu, "Merde tournoyante en plastique bleu"
        },
        {
            CardNames.MJCorrompu, "MJ corrompu"
        },
        {
            CardNames.Mohammad, "Mohammad"
        },
        {
            CardNames.MoïseLePlusGrandDeTousLesHébreux, "Moïse, le plus grand de tous les hébreux"
        },
        {
            CardNames.MusiqueDeMegaDrive, "Musique de Mega Drive"
        },
        {
            CardNames.Nounours, "Nounours"
        },
        {
            CardNames.PainsAuxRaisinsÀLaPlaceDesMains, "Pains aux raisins à la place des mains"
        },
        {
            CardNames.PapyGrenier, "Papy Grenier"
        },
        {
            CardNames.ParachuteDoré, "Parachute doré"
        },
        {
            CardNames.PassageSecretDansLeMurDuVoisin, "Passage secret dans le mur du voisin"
        },
        {
            CardNames.Patate, "Patate"
        },
        {
            CardNames.PatronDInfogrames, "Patron D'Infogrames"
        },
        {
            CardNames.PetiteCaille, "Petite caille"
        },
        {
            CardNames.PetiteCulotte, "Petite culotte"
        },
        {
            CardNames.PetitePortionsDeRiz, "Petite portions de « riz »"
        },
        {
            CardNames.Player, "Player"
        },
        {
            CardNames.PlumeDePhénix, "Plume de phénix"
        },
        {
            CardNames.PoignéeDePorte, "Poignée de porte"
        },
        {
            CardNames.ProfDeSVT, "Prof de SVT"
        },
        {
            CardNames.ProfesseurHumblebundledore, "Professeur Humblebundledore"
        },
        {
            CardNames.ProfesseurRogueLike, "Professeur Rogue-Like"
        },
        {
            CardNames.PropriétaireDeFistiland, "Propriétaire De Fistiland"
        },
        {
            CardNames.QuartDeSmileySurLaMain, "Quart de smiley sur la main"
        },
        {
            CardNames.Repop, "Repop"
        },
        {
            CardNames.ReversDuDroit, "Revers du droit"
        },
        {
            CardNames.Rémi, "Rémi"
        },
        {
            CardNames.SailorJustice, "Sailor Justice"
        },
        {
            CardNames.SalutMétalleux, "Salut métalleux"
        },
        {
            CardNames.SandrineLePorteManteauExtraterrestre, "Sandrine le porte-manteau extraterrestre"
        },
        {
            CardNames.Sangoku, "Sangoku"
        },
        {
            CardNames.SauvegardeEffacée, "Sauvegarde effacée"
        },
        {
            CardNames.SceauxMagiques, "Sceaux magiques"
        },
        {
            CardNames.SebDuGrenier, "Seb Du Grenier"
        },
        {
            CardNames.SheikPoint, "Sheik Point"
        },
        {
            CardNames.Slime, "Slime"
        },
        {
            CardNames.Spaghetti, "Spaghetti"
        },
        {
            CardNames.Squalala, "Squalala"
        },
        {
            CardNames.StarlightUnicorn, "Starlight Unicorn"
        },
        {
            CardNames.SteelbookVide, "Steelbook vide"
        },
        {
            CardNames.StudioDeDéveloppement, "Studio de développement"
        },
        {
            CardNames.StudioDeScénaristesCanadien, "Studio de scénaristes Canadien"
        },
        {
            CardNames.SylvainFilsDeSylvainRoiDesElfes, "Sylvain, Fils de Sylvain, Roi des Elfes"
        },
        {
            CardNames.TatouageGoldé, "Tatouage goldé"
        },
        {
            CardNames.Tentacules, "Tentacules"
        },
        {
            CardNames.TexasGranger, "Texas Granger"
        },
        {
            CardNames.Théodule, "Théodule"
        },
        {
            CardNames.TortureNinja, "Torture Ninja"
        },
        {
            CardNames.TurboCradozaure, "Turbo Cradozaure"
        },
        {
            CardNames.UnBonTuyau, "Un bon tuyau"
        },
        {
            CardNames.UnDélicieuxRisotto, "Un délicieux risotto"
        },
        {
            CardNames.VieuxSage, "Vieux Sage"
        },
        {
            CardNames.YoutubeMoney, "Youtube Money"
        },
        {
            CardNames.ZozanKebab, "Zozan Kebab"
        },
        {
            CardNames.ÉcranDeFinNul, "Écran de fin nul"
        },
        {
            CardNames.ÉtoufféSousLesJeuxDeMerde, "Étouffé sous les jeux de merde"
        }
    };
}