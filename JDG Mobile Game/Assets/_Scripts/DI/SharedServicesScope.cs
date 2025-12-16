using VContainer;
using VContainer.Unity;
using JDG.Application;
using JDG.Application.Abilities;
using JDG.Application.Abilities.Implementations;
using JDG.Application.Repositories;
using JDG.Application.Services;
using JDG.Application.UseCases;
using JDG.Infrastructure.Events;
using JDG.Infrastructure.Repositories;
using JDG.Infrastructure.Services;

namespace JDG.DI
{
    /// <summary>
    /// ROOT VContainer lifetime scope for the entire game.
    /// Contains ALL service registrations (merged from GameLifetimeScope + old SharedServicesScope).
    /// Phase 46: Simplified to single root scope for reliable DI.
    ///
    /// Place this in the preload scene with Auto Run = true, Parent = None.
    /// Scene scopes (MainScreenScope, GameSceneScope) will auto-find this as parent.
    /// Uses DontDestroyOnLoad to persist across scene loads.
    /// </summary>
    public class SharedServicesScope : LifetimeScope
    {
        protected override void Awake()
        {
            // Persist across scene loads (scenes use LoadSceneMode.Single)
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            UnityEngine.Debug.Log("SharedServicesScope: Configuring ROOT scope...");

            // ============================================
            // INFRASTRUCTURE LAYER - Event Bus & Repositories
            // ============================================

            // Event Bus (Singleton) - core messaging system
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);

            // Repositories (Singleton - maintain state throughout game session)
            builder.Register<ICardRepository, CardRepository>(Lifetime.Singleton);
            builder.Register<IDeckRepository, DeckRepository>(Lifetime.Singleton);
            builder.Register<IPlayerRepository, PlayerRepository>(Lifetime.Singleton);
            builder.Register<IGameStateRepository, GameStateRepository>(Lifetime.Singleton);

            // Game State Service
            builder.Register<GameStateService>(Lifetime.Singleton);

            // ============================================
            // LEGACY WRAPPER SERVICES
            // ============================================

            // Audio, Localization, Dialog services (wrap singletons)
            builder.Register<IAudioService, AudioService>(Lifetime.Singleton);
            builder.Register<ILocalizationService, LocalizationService>(Lifetime.Singleton);
            builder.Register<IDialogService, DialogService>(Lifetime.Singleton);

            // Card Visual Service - abstracts Unity Material dependencies
            builder.Register<ICardVisualService, CardVisualService>(Lifetime.Singleton);

            // Raycast Service
            builder.Register<IRaycastService, RaycastService>(Lifetime.Singleton);

            // Player Service - manages player state
            builder.Register<IPlayerService, PlayerService>(Lifetime.Singleton);

            // Card Placement Service - business logic for card placement
            builder.Register<ICardPlacementService, CardPlacementService>(Lifetime.Singleton);

            // Deck Initialization Service
            builder.Register<IDeckInitializationService, DeckInitializationService>(Lifetime.Singleton);

            // Card Data Provider - replaces ResourceSystem.Instance
            builder.Register<JDG.Application.Services.ICardDataProvider, JDG.Infrastructure.Services.CardDataProvider>(Lifetime.Singleton);

            // Deck Management Service - deck data storage
            builder.Register<IDeckManagementService, DeckManagementService>(Lifetime.Singleton);

            // Card Instantiation Service - GameObject creation
            builder.Register<ICardInstantiationService, CardInstantiationService>(Lifetime.Singleton);

            // Combat Service - combat operations
            builder.Register<CombatService>(Lifetime.Singleton);
            builder.Register<ICombatService>(c => c.Resolve<CombatService>(), Lifetime.Singleton);
            builder.Register<ICombatQueryService>(c => c.Resolve<CombatService>(), Lifetime.Singleton);

            // Card Selection Service - pure C#, no MonoBehaviour needed
            builder.Register<JDG.Application.Services.ICardSelectionService, CardSelectionService>(Lifetime.Singleton);

            // ============================================
            // APPLICATION LAYER - Use Cases
            // ============================================

            // Game Flow Use Cases
            builder.Register<StartGameUseCase>(Lifetime.Transient);
            builder.Register<EndTurnUseCase>(Lifetime.Transient);

            // Card Use Cases
            builder.Register<DrawCardUseCase>(Lifetime.Transient);
            builder.Register<PlayCardUseCase>(Lifetime.Transient);

            // Combat Use Cases
            builder.Register<AttackUseCase>(Lifetime.Transient);

            // Legacy Use Cases (depend on legacy types)
            builder.Register<SummonPlayerEntityUseCase>(Lifetime.Transient);
            builder.Register<ResetCardsForNewTurnUseCase>(Lifetime.Transient);
            builder.Register<HandleCardDeathUseCase>(Lifetime.Transient);
            builder.Register<HandleCardAddedToFieldUseCase>(Lifetime.Transient);
            builder.Register<HandleCardRemovedFromFieldUseCase>(Lifetime.Transient);
            builder.Register<HandleHandCardsChangeUseCase>(Lifetime.Transient);
            builder.Register<HandleFieldCardChangedUseCase>(Lifetime.Transient);

            // ============================================
            // ABILITY SYSTEM
            // ============================================

            // Ability Core
            builder.Register<AbilityRegistry>(Lifetime.Singleton);
            builder.Register<AbilityManager>(Lifetime.Singleton);

            // Ability Provider (bridges modern and legacy)
            builder.Register<IAbilityProvider, AbilityProviderService>(Lifetime.Singleton);

            // Ability Factories
            builder.Register<DrawCardsAbilityFactory>(Lifetime.Singleton);
            builder.Register<DestroyCardAbilityFactory>(Lifetime.Singleton);
            builder.Register<DeckSearchAbilityFactory>(Lifetime.Singleton);
            builder.Register<SacrificeAbilityFactory>(Lifetime.Singleton);
            builder.Register<StatModifierAbilityFactory>(Lifetime.Singleton);
            builder.Register<ProtectionAbilityFactory>(Lifetime.Singleton);
            builder.Register<CombatAbilityFactory>(Lifetime.Singleton);
            builder.Register<EffectAbilityFactory>(Lifetime.Singleton);
            builder.Register<EquipmentAbilityFactory>(Lifetime.Singleton);
            builder.Register<FieldAbilityFactory>(Lifetime.Singleton);
            builder.Register<SpecialAbilityFactory>(Lifetime.Singleton);

            // Register abilities after container is built
            builder.RegisterBuildCallback(container =>
            {
                RegisterAllAbilities(container);
            });

            UnityEngine.Debug.Log("SharedServicesScope: ROOT scope configuration complete");
        }

        /// <summary>
        /// Registers all game abilities with the AbilityRegistry.
        /// </summary>
        private static void RegisterAllAbilities(IObjectResolver container)
        {
            var registry = container.Resolve<AbilityRegistry>();
            var drawFactory = container.Resolve<DrawCardsAbilityFactory>();
            var destroyFactory = container.Resolve<DestroyCardAbilityFactory>();
            var deckSearchFactory = container.Resolve<DeckSearchAbilityFactory>();
            var sacrificeFactory = container.Resolve<SacrificeAbilityFactory>();
            var statModifierFactory = container.Resolve<StatModifierAbilityFactory>();
            var protectionFactory = container.Resolve<ProtectionAbilityFactory>();
            var combatFactory = container.Resolve<CombatAbilityFactory>();
            var specialFactory = container.Resolve<SpecialAbilityFactory>();

            // DRAW ABILITIES
            registry.Register(JDG.Domain.AbilityName.Draw1Card, () => drawFactory.CreateDrawNCards(1));
            registry.Register(JDG.Domain.AbilityName.Draw2Cards, () => drawFactory.CreateDraw2Cards());
            registry.Register(JDG.Domain.AbilityName.Draw3Cards, () => drawFactory.CreateDrawNCards(3));

            // DESTROY ABILITIES
            registry.Register(JDG.Domain.AbilityName.KillOpponentInvocation, () => destroyFactory.CreateKillOpponentInvocation());
            registry.Register(JDG.Domain.AbilityName.DestroyFieldATK, () => destroyFactory.CreateDestroyField());
            registry.Register(JDG.Domain.AbilityName.DestroyFieldDEF, () => destroyFactory.CreateDestroyField());
            registry.Register(JDG.Domain.AbilityName.KillEnemyIfDestroy, () => combatFactory.CreateMutualDestruction(JDG.Domain.AbilityName.KillEnemyIfDestroy));

            // DECK SEARCH ABILITIES
            registry.Register(JDG.Domain.AbilityName.AddSpatialFromDeck, () => deckSearchFactory.CreateGetSpecificCard(JDG.Domain.AbilityName.AddSpatialFromDeck, "Spatial"));
            registry.Register(JDG.Domain.AbilityName.GetNounoursFromDeck, () => deckSearchFactory.CreateGetSpecificCard(JDG.Domain.AbilityName.GetNounoursFromDeck, "Nounours"));
            registry.Register(JDG.Domain.AbilityName.GetPetitePortionDeRizFromDeck, () => deckSearchFactory.CreateGetSpecificCard(JDG.Domain.AbilityName.GetPetitePortionDeRizFromDeck, "Petite Portion de Riz"));
            registry.Register(JDG.Domain.AbilityName.GetLycéeMagiqueGeorgesPompidouFromDeck, () => deckSearchFactory.CreateGetSpecificCard(JDG.Domain.AbilityName.GetLycéeMagiqueGeorgesPompidouFromDeck, "Lycée Magique Georges Pompidou"));
            registry.Register(JDG.Domain.AbilityName.GetZozanKebabFromDeck, () => deckSearchFactory.CreateGetSpecificCard(JDG.Domain.AbilityName.GetZozanKebabFromDeck, "Zozan Kebab"));
            registry.Register(JDG.Domain.AbilityName.GetConvocationAuLyceeFromDeck, () => deckSearchFactory.CreateGetSpecificCard(JDG.Domain.AbilityName.GetConvocationAuLyceeFromDeck, "Convocation au Lycée"));
            registry.Register(JDG.Domain.AbilityName.GetCanardSignal, () => deckSearchFactory.CreateGetSpecificCard(JDG.Domain.AbilityName.GetCanardSignal, "Canard Signal"));
            registry.Register(JDG.Domain.AbilityName.GetForetElfesSylvains, () => deckSearchFactory.CreateGetSpecificCard(JDG.Domain.AbilityName.GetForetElfesSylvains, "Forêt des Elfes Sylvains"));
            registry.Register(JDG.Domain.AbilityName.GetBenzaieJeuneFromDeck, () => deckSearchFactory.CreateGetSpecificCard(JDG.Domain.AbilityName.GetBenzaieJeuneFromDeck, "Benzaie Jeune"));
            registry.Register(JDG.Domain.AbilityName.GetPatronInfogramesFromDeckYellowTrash, () => deckSearchFactory.CreateGetSpecificCard(JDG.Domain.AbilityName.GetPatronInfogramesFromDeckYellowTrash, "Patron Infogrames"));
            registry.Register(JDG.Domain.AbilityName.GetEquipmentCardWithoutAttack, () => specialFactory.CreateSearchEquipment());

            // SACRIFICE ABILITIES
            registry.Register(JDG.Domain.AbilityName.SacrificeArchibaldVonGrenier, () => sacrificeFactory.CreateSacrificeCard(JDG.Domain.AbilityName.SacrificeArchibaldVonGrenier, "Archibald Von Grenier"));
            registry.Register(JDG.Domain.AbilityName.SacrificeBenzaieJeune, () => sacrificeFactory.CreateSacrificeCard(JDG.Domain.AbilityName.SacrificeBenzaieJeune, "Benzaie Jeune"));
            registry.Register(JDG.Domain.AbilityName.SacrificeJoueurDuGrenier, () => sacrificeFactory.CreateSacrificeCard(JDG.Domain.AbilityName.SacrificeJoueurDuGrenier, "Joueur Du Grenier"));
            registry.Register(JDG.Domain.AbilityName.SacrificeWizard, () => sacrificeFactory.CreateSacrificeCard(JDG.Domain.AbilityName.SacrificeWizard, "Wizard"));
            registry.Register(JDG.Domain.AbilityName.SacrificeSebDuGrenier, () => sacrificeFactory.CreateSacrificeCard(JDG.Domain.AbilityName.SacrificeSebDuGrenier, "Seb Du Grenier"));
            registry.Register(JDG.Domain.AbilityName.SacrificeGranolax, () => sacrificeFactory.CreateSacrificeCard(JDG.Domain.AbilityName.SacrificeGranolax, "Granolax"));
            registry.Register(JDG.Domain.AbilityName.SacrificeClicheRaciste, () => sacrificeFactory.CreateSacrificeCard(JDG.Domain.AbilityName.SacrificeClicheRaciste, "Cliché Raciste"));
            registry.Register(JDG.Domain.AbilityName.SacrificeToInvoke, () => sacrificeFactory.CreateSacrificeToInvoke());

            // Sacrifice with ATK/DEF gain
            registry.Register(JDG.Domain.AbilityName.SacrificeSebDuGrenierOnHardCornerForAtkDef, () => specialFactory.CreateOptionalSacrificeForStats(JDG.Domain.AbilityName.SacrificeSebDuGrenierOnHardCornerForAtkDef, 3, 3));
            registry.Register(JDG.Domain.AbilityName.SacrificeJDGOnStudioDevForAtkDef, () => specialFactory.CreateOptionalSacrificeForStats(JDG.Domain.AbilityName.SacrificeJDGOnStudioDevForAtkDef, 3, 3));
            registry.Register(JDG.Domain.AbilityName.Sacrifice3Atk3Def, () => specialFactory.CreateOptionalSacrificeForStats(JDG.Domain.AbilityName.Sacrifice3Atk3Def, 3, 3));
            registry.Register(JDG.Domain.AbilityName.SacrificeDeveloper3Atk3Def, () => specialFactory.CreateOptionalSacrificeForStats(JDG.Domain.AbilityName.SacrificeDeveloper3Atk3Def, 3, 3));
            registry.Register(JDG.Domain.AbilityName.SacrificeHardCorner3Atk3Def, () => specialFactory.CreateOptionalSacrificeForStats(JDG.Domain.AbilityName.SacrificeHardCorner3Atk3Def, 3, 3));
            registry.Register(JDG.Domain.AbilityName.Sacrifice2Japan, () => specialFactory.CreateConditionalSacrifice(JDG.Domain.AbilityName.Sacrifice2Japan, 0, 0, JDG.Domain.Enums.CardFamily.Japan, 2));
            registry.Register(JDG.Domain.AbilityName.Sacrifice2Incarnation, () => specialFactory.CreateConditionalSacrifice(JDG.Domain.AbilityName.Sacrifice2Incarnation, 0, 0, JDG.Domain.Enums.CardFamily.Incarnation, 2));

            // INVOKE ABILITIES
            registry.Register(JDG.Domain.AbilityName.InvokeTentacules, () => sacrificeFactory.CreateInvokeSpecificCard(JDG.Domain.AbilityName.InvokeTentacules, "Tentacules"));
            registry.Register(JDG.Domain.AbilityName.InvokeDresseurBidulmon, () => sacrificeFactory.CreateInvokeSpecificCard(JDG.Domain.AbilityName.InvokeDresseurBidulmon, "Dresseur Bidulmon"));
            registry.Register(JDG.Domain.AbilityName.InvokeSebOrJDG, () => sacrificeFactory.CreateInvokeSpecificCard(JDG.Domain.AbilityName.InvokeSebOrJDG, "Seb Du Grenier"));

            // STAT MODIFIER ABILITIES
            registry.Register(JDG.Domain.AbilityName.GiveAtkDefToComics, () => statModifierFactory.CreateGiveFamilyStats(JDG.Domain.AbilityName.GiveAtkDefToComics, JDG.Domain.Enums.CardFamily.Comics, 1, 1));
            registry.Register(JDG.Domain.AbilityName.GiveAktDefToRpgMember, () => statModifierFactory.CreateGiveFamilyStats(JDG.Domain.AbilityName.GiveAktDefToRpgMember, JDG.Domain.Enums.CardFamily.Rpg, 1, 1));
            registry.Register(JDG.Domain.AbilityName.GiveAktDefToFistilandMember, () => statModifierFactory.CreateGiveFamilyStats(JDG.Domain.AbilityName.GiveAktDefToFistilandMember, JDG.Domain.Enums.CardFamily.Fistiland, 1, 1));
            registry.Register(JDG.Domain.AbilityName.Win1Atk1DefDeveloper, () => statModifierFactory.CreateGiveFamilyStats(JDG.Domain.AbilityName.Win1Atk1DefDeveloper, JDG.Domain.Enums.CardFamily.Developer, 1, 1));
            registry.Register(JDG.Domain.AbilityName.Win1Atk1DefFistiland, () => statModifierFactory.CreateGiveFamilyStats(JDG.Domain.AbilityName.Win1Atk1DefFistiland, JDG.Domain.Enums.CardFamily.Fistiland, 1, 1));
            registry.Register(JDG.Domain.AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition, () => statModifierFactory.CreateConditionalStats(JDG.Domain.AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition, JDG.Domain.Enums.CardFamily.Japan, 2, 2, 1, 1));
            registry.Register(JDG.Domain.AbilityName.CopyBenzaieJeune, () => statModifierFactory.CreateCopyStats(JDG.Domain.AbilityName.CopyBenzaieJeune, "Benzaie Jeune"));

            // PROTECTION ABILITIES
            registry.Register(JDG.Domain.AbilityName.CantBeAttackIfComics, () => protectionFactory.CreateCantBeAttacked(JDG.Domain.AbilityName.CantBeAttackIfComics, JDG.Domain.Enums.CardFamily.Comics));
            registry.Register(JDG.Domain.AbilityName.CantBeAttackKill, () => protectionFactory.CreateCantBeAttacked(JDG.Domain.AbilityName.CantBeAttackKill, null));
            registry.Register(JDG.Domain.AbilityName.ProtectedBehindStarlightUnicorn, () => protectionFactory.CreateProtectBehind(JDG.Domain.AbilityName.ProtectedBehindStarlightUnicorn, null));
            registry.Register(JDG.Domain.AbilityName.ProtectBehindGreaterDef, () => protectionFactory.CreateProtectBehind(JDG.Domain.AbilityName.ProtectBehindGreaterDef, null));
            registry.Register(JDG.Domain.AbilityName.CanOnlyAttackItself, () => protectionFactory.CreateCanOnlyAttackItself());

            // DEPENDENCY ABILITIES
            registry.Register(JDG.Domain.AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune, () => protectionFactory.CreateDependency(JDG.Domain.AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune, "Benzaie", "Benzaie Jeune"));
            registry.Register(JDG.Domain.AbilityName.CantLiveWithoutJDG, () => protectionFactory.CreateDependency(JDG.Domain.AbilityName.CantLiveWithoutJDG, "Joueur Du Grenier"));
            registry.Register(JDG.Domain.AbilityName.CantLiveWithoutComics, () => protectionFactory.CreateDependency(JDG.Domain.AbilityName.CantLiveWithoutComics, "Comics"));
            registry.Register(JDG.Domain.AbilityName.CantLiveWithoutHuman, () => protectionFactory.CreateDependency(JDG.Domain.AbilityName.CantLiveWithoutHuman, "Human"));
            registry.Register(JDG.Domain.AbilityName.CantLiveWithoutJapon, () => protectionFactory.CreateDependency(JDG.Domain.AbilityName.CantLiveWithoutJapon, "Japon"));
            registry.Register(JDG.Domain.AbilityName.CantLiveWithoutGranolaxOrMechaGranolax, () => protectionFactory.CreateDependency(JDG.Domain.AbilityName.CantLiveWithoutGranolaxOrMechaGranolax, "Granolax", "Mecha Granolax"));

            // LIFECYCLE ABILITIES
            registry.Register(JDG.Domain.AbilityName.SurviveOneTurn, () => protectionFactory.CreateLimitedLifetime(JDG.Domain.AbilityName.SurviveOneTurn, 1));
            registry.Register(JDG.Domain.AbilityName.ComesBackFromDeath, () => combatFactory.CreateResurrection(JDG.Domain.AbilityName.ComesBackFromDeath, 1, false));
            registry.Register(JDG.Domain.AbilityName.ComesBackFromDeath5Times, () => combatFactory.CreateResurrection(JDG.Domain.AbilityName.ComesBackFromDeath5Times, 5, false));
            registry.Register(JDG.Domain.AbilityName.GiveDeathWhenDie, () => combatFactory.CreateDeathTrigger());

            // COMBAT ABILITIES
            registry.Register(JDG.Domain.AbilityName.SkipOpponentAttackEveryTurn, () => combatFactory.CreateSkipAttack(JDG.Domain.AbilityName.SkipOpponentAttackEveryTurn, true));

            // SPECIAL ABILITIES
            registry.Register(JDG.Domain.AbilityName.SendAllCardToHands, () => specialFactory.CreateSendAllToHand());
            registry.Register(JDG.Domain.AbilityName.ChangeFieldWithFieldFromDeck, () => specialFactory.CreateOptionalChangeField());

            // DEFAULT ABILITY
            registry.Register(JDG.Domain.AbilityName.Default, () => new JDG.Infrastructure.DI.DefaultAbility());

            UnityEngine.Debug.Log("SharedServicesScope: Abilities registered");
        }
    }
}
