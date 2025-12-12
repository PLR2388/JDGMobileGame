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

namespace JDG.Infrastructure.DI
{
    /// <summary>
    /// Main VContainer lifetime scope for the game.
    /// Registers all repositories, use cases, and services for dependency injection.
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // ============================================
            // INFRASTRUCTURE LAYER - Repositories & Services
            // ============================================

            // Event Bus (Singleton)
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);

            // Repositories (Singleton - maintain state throughout game session)
            builder.Register<ICardRepository, CardRepository>(Lifetime.Singleton);
            builder.Register<IDeckRepository, DeckRepository>(Lifetime.Singleton);
            builder.Register<IPlayerRepository, PlayerRepository>(Lifetime.Singleton);
            builder.Register<IGameStateRepository, GameStateRepository>(Lifetime.Singleton);

            // Services (Singleton - application-wide services)
            // Phase 15: Event-Driven Game State
            builder.Register<GameStateService>(Lifetime.Singleton);

            // Note: Legacy wrapper services (AudioService, DialogService, InputService, LocalizationService, RaycastService)
            // are registered in a separate Legacy assembly scope to avoid circular dependencies.

            // ============================================
            // APPLICATION LAYER - Use Cases
            // ============================================

            // Game Flow Use Cases (Transient - create new instance per use)
            builder.Register<StartGameUseCase>(Lifetime.Transient);
            builder.Register<EndTurnUseCase>(Lifetime.Transient);

            // Card Use Cases (Transient)
            builder.Register<DrawCardUseCase>(Lifetime.Transient);
            builder.Register<PlayCardUseCase>(Lifetime.Transient);

            // Combat Use Cases (Transient)
            builder.Register<AttackUseCase>(Lifetime.Transient);

            // Note: Phase 21-22 use cases (SummonPlayerEntityUseCase, ResetCardsForNewTurnUseCase)
            // are registered in LegacyServicesScope because they depend on legacy types from
            // the default assembly.

            // ============================================
            // ABILITY SYSTEM - Phase 14
            // ============================================

            // Ability Core (Singleton)
            builder.Register<AbilityRegistry>(Lifetime.Singleton);
            builder.Register<AbilityManager>(Lifetime.Singleton);

            // Ability Factories (Singleton - can be reused to create abilities)
            // Phase 7: Comprehensive ability migration
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
        }

        /// <summary>
        /// Registers all game abilities with the AbilityRegistry.
        /// Phase 29: Complete ability registration for all AbilityName enum values.
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

            // ============================================
            // DRAW ABILITIES
            // ============================================
            registry.Register(Domain.AbilityName.Draw1Card, () => drawFactory.CreateDrawNCards(1));
            registry.Register(Domain.AbilityName.Draw2Cards, () => drawFactory.CreateDraw2Cards());
            registry.Register(Domain.AbilityName.Draw3Cards, () => drawFactory.CreateDrawNCards(3));

            // ============================================
            // DESTROY ABILITIES
            // ============================================
            registry.Register(Domain.AbilityName.KillOpponentInvocation, () => destroyFactory.CreateKillOpponentInvocation());
            registry.Register(Domain.AbilityName.DestroyFieldATK, () => destroyFactory.CreateDestroyField());
            registry.Register(Domain.AbilityName.DestroyFieldDEF, () => destroyFactory.CreateDestroyField());
            registry.Register(Domain.AbilityName.KillEnemyIfDestroy, () => combatFactory.CreateMutualDestruction(Domain.AbilityName.KillEnemyIfDestroy));

            // ============================================
            // DECK SEARCH ABILITIES - Get specific cards from deck
            // ============================================
            registry.Register(Domain.AbilityName.AddSpatialFromDeck, () => deckSearchFactory.CreateGetSpecificCard(Domain.AbilityName.AddSpatialFromDeck, "Spatial"));
            registry.Register(Domain.AbilityName.GetNounoursFromDeck, () => deckSearchFactory.CreateGetSpecificCard(Domain.AbilityName.GetNounoursFromDeck, "Nounours"));
            registry.Register(Domain.AbilityName.GetPetitePortionDeRizFromDeck, () => deckSearchFactory.CreateGetSpecificCard(Domain.AbilityName.GetPetitePortionDeRizFromDeck, "Petite Portion de Riz"));
            registry.Register(Domain.AbilityName.GetLycéeMagiqueGeorgesPompidouFromDeck, () => deckSearchFactory.CreateGetSpecificCard(Domain.AbilityName.GetLycéeMagiqueGeorgesPompidouFromDeck, "Lycée Magique Georges Pompidou"));
            registry.Register(Domain.AbilityName.GetZozanKebabFromDeck, () => deckSearchFactory.CreateGetSpecificCard(Domain.AbilityName.GetZozanKebabFromDeck, "Zozan Kebab"));
            registry.Register(Domain.AbilityName.GetConvocationAuLyceeFromDeck, () => deckSearchFactory.CreateGetSpecificCard(Domain.AbilityName.GetConvocationAuLyceeFromDeck, "Convocation au Lycée"));
            registry.Register(Domain.AbilityName.GetCanardSignal, () => deckSearchFactory.CreateGetSpecificCard(Domain.AbilityName.GetCanardSignal, "Canard Signal"));
            registry.Register(Domain.AbilityName.GetForetElfesSylvains, () => deckSearchFactory.CreateGetSpecificCard(Domain.AbilityName.GetForetElfesSylvains, "Forêt des Elfes Sylvains"));
            registry.Register(Domain.AbilityName.GetBenzaieJeuneFromDeck, () => deckSearchFactory.CreateGetSpecificCard(Domain.AbilityName.GetBenzaieJeuneFromDeck, "Benzaie Jeune"));
            registry.Register(Domain.AbilityName.GetPatronInfogramesFromDeckYellowTrash, () => deckSearchFactory.CreateGetSpecificCard(Domain.AbilityName.GetPatronInfogramesFromDeckYellowTrash, "Patron Infogrames"));
            registry.Register(Domain.AbilityName.GetEquipmentCardWithoutAttack, () => specialFactory.CreateSearchEquipment());

            // ============================================
            // SACRIFICE ABILITIES
            // ============================================
            registry.Register(Domain.AbilityName.SacrificeArchibaldVonGrenier, () => sacrificeFactory.CreateSacrificeCard(Domain.AbilityName.SacrificeArchibaldVonGrenier, "Archibald Von Grenier"));
            registry.Register(Domain.AbilityName.SacrificeBenzaieJeune, () => sacrificeFactory.CreateSacrificeCard(Domain.AbilityName.SacrificeBenzaieJeune, "Benzaie Jeune"));
            registry.Register(Domain.AbilityName.SacrificeJoueurDuGrenier, () => sacrificeFactory.CreateSacrificeCard(Domain.AbilityName.SacrificeJoueurDuGrenier, "Joueur Du Grenier"));
            registry.Register(Domain.AbilityName.SacrificeWizard, () => sacrificeFactory.CreateSacrificeCard(Domain.AbilityName.SacrificeWizard, "Wizard"));
            registry.Register(Domain.AbilityName.SacrificeSebDuGrenier, () => sacrificeFactory.CreateSacrificeCard(Domain.AbilityName.SacrificeSebDuGrenier, "Seb Du Grenier"));
            registry.Register(Domain.AbilityName.SacrificeGranolax, () => sacrificeFactory.CreateSacrificeCard(Domain.AbilityName.SacrificeGranolax, "Granolax"));
            registry.Register(Domain.AbilityName.SacrificeClicheRaciste, () => sacrificeFactory.CreateSacrificeCard(Domain.AbilityName.SacrificeClicheRaciste, "Cliché Raciste"));
            registry.Register(Domain.AbilityName.SacrificeToInvoke, () => sacrificeFactory.CreateSacrificeToInvoke());

            // Sacrifice with ATK/DEF gain
            registry.Register(Domain.AbilityName.SacrificeSebDuGrenierOnHardCornerForAtkDef, () => specialFactory.CreateOptionalSacrificeForStats(Domain.AbilityName.SacrificeSebDuGrenierOnHardCornerForAtkDef, 3, 3));
            registry.Register(Domain.AbilityName.SacrificeJDGOnStudioDevForAtkDef, () => specialFactory.CreateOptionalSacrificeForStats(Domain.AbilityName.SacrificeJDGOnStudioDevForAtkDef, 3, 3));
            registry.Register(Domain.AbilityName.Sacrifice3Atk3Def, () => specialFactory.CreateOptionalSacrificeForStats(Domain.AbilityName.Sacrifice3Atk3Def, 3, 3));
            registry.Register(Domain.AbilityName.SacrificeDeveloper3Atk3Def, () => specialFactory.CreateOptionalSacrificeForStats(Domain.AbilityName.SacrificeDeveloper3Atk3Def, 3, 3));
            registry.Register(Domain.AbilityName.SacrificeHardCorner3Atk3Def, () => specialFactory.CreateOptionalSacrificeForStats(Domain.AbilityName.SacrificeHardCorner3Atk3Def, 3, 3));
            registry.Register(Domain.AbilityName.Sacrifice2Japan, () => specialFactory.CreateConditionalSacrifice(Domain.AbilityName.Sacrifice2Japan, 0, 0, Domain.Enums.CardFamily.Japan, 2));
            registry.Register(Domain.AbilityName.Sacrifice2Incarnation, () => specialFactory.CreateConditionalSacrifice(Domain.AbilityName.Sacrifice2Incarnation, 0, 0, Domain.Enums.CardFamily.Incarnation, 2));

            // ============================================
            // INVOKE ABILITIES - Invoke specific cards from deck
            // ============================================
            registry.Register(Domain.AbilityName.InvokeTentacules, () => sacrificeFactory.CreateInvokeSpecificCard(Domain.AbilityName.InvokeTentacules, "Tentacules"));
            registry.Register(Domain.AbilityName.InvokeDresseurBidulmon, () => sacrificeFactory.CreateInvokeSpecificCard(Domain.AbilityName.InvokeDresseurBidulmon, "Dresseur Bidulmon"));
            registry.Register(Domain.AbilityName.InvokeSebOrJDG, () => sacrificeFactory.CreateInvokeSpecificCard(Domain.AbilityName.InvokeSebOrJDG, "Seb Du Grenier"));

            // ============================================
            // STAT MODIFIER ABILITIES
            // ============================================
            registry.Register(Domain.AbilityName.GiveAtkDefToComics, () => statModifierFactory.CreateGiveFamilyStats(Domain.AbilityName.GiveAtkDefToComics, Domain.Enums.CardFamily.Comics, 1, 1));
            registry.Register(Domain.AbilityName.GiveAktDefToRpgMember, () => statModifierFactory.CreateGiveFamilyStats(Domain.AbilityName.GiveAktDefToRpgMember, Domain.Enums.CardFamily.Rpg, 1, 1));
            registry.Register(Domain.AbilityName.GiveAktDefToFistilandMember, () => statModifierFactory.CreateGiveFamilyStats(Domain.AbilityName.GiveAktDefToFistilandMember, Domain.Enums.CardFamily.Fistiland, 1, 1));
            registry.Register(Domain.AbilityName.Win1Atk1DefDeveloper, () => statModifierFactory.CreateGiveFamilyStats(Domain.AbilityName.Win1Atk1DefDeveloper, Domain.Enums.CardFamily.Developer, 1, 1));
            registry.Register(Domain.AbilityName.Win1Atk1DefFistiland, () => statModifierFactory.CreateGiveFamilyStats(Domain.AbilityName.Win1Atk1DefFistiland, Domain.Enums.CardFamily.Fistiland, 1, 1));
            registry.Register(Domain.AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition, () => statModifierFactory.CreateConditionalStats(Domain.AbilityName.Win1ATK1DefJaponWith2ATK2DEFCondition, Domain.Enums.CardFamily.Japan, 2, 2, 1, 1));
            registry.Register(Domain.AbilityName.CopyBenzaieJeune, () => statModifierFactory.CreateCopyStats(Domain.AbilityName.CopyBenzaieJeune, "Benzaie Jeune"));

            // ============================================
            // PROTECTION ABILITIES
            // ============================================
            registry.Register(Domain.AbilityName.CantBeAttackIfComics, () => protectionFactory.CreateCantBeAttacked(Domain.AbilityName.CantBeAttackIfComics, Domain.Enums.CardFamily.Comics));
            registry.Register(Domain.AbilityName.CantBeAttackKill, () => protectionFactory.CreateCantBeAttacked(Domain.AbilityName.CantBeAttackKill, null));
            registry.Register(Domain.AbilityName.ProtectedBehindStarlightUnicorn, () => protectionFactory.CreateProtectBehind(Domain.AbilityName.ProtectedBehindStarlightUnicorn, null));
            registry.Register(Domain.AbilityName.ProtectBehindGreaterDef, () => protectionFactory.CreateProtectBehind(Domain.AbilityName.ProtectBehindGreaterDef, null));
            registry.Register(Domain.AbilityName.CanOnlyAttackItself, () => protectionFactory.CreateCanOnlyAttackItself());

            // ============================================
            // DEPENDENCY ABILITIES - Can't live without specific cards
            // ============================================
            registry.Register(Domain.AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune, () => protectionFactory.CreateDependency(Domain.AbilityName.CantLiveWithoutBenzaieOrBenzaieJeune, "Benzaie", "Benzaie Jeune"));
            registry.Register(Domain.AbilityName.CantLiveWithoutJDG, () => protectionFactory.CreateDependency(Domain.AbilityName.CantLiveWithoutJDG, "Joueur Du Grenier"));
            registry.Register(Domain.AbilityName.CantLiveWithoutComics, () => protectionFactory.CreateDependency(Domain.AbilityName.CantLiveWithoutComics, "Comics"));
            registry.Register(Domain.AbilityName.CantLiveWithoutHuman, () => protectionFactory.CreateDependency(Domain.AbilityName.CantLiveWithoutHuman, "Human"));
            registry.Register(Domain.AbilityName.CantLiveWithoutJapon, () => protectionFactory.CreateDependency(Domain.AbilityName.CantLiveWithoutJapon, "Japon"));
            registry.Register(Domain.AbilityName.CantLiveWithoutGranolaxOrMechaGranolax, () => protectionFactory.CreateDependency(Domain.AbilityName.CantLiveWithoutGranolaxOrMechaGranolax, "Granolax", "Mecha Granolax"));

            // ============================================
            // LIFECYCLE ABILITIES
            // ============================================
            registry.Register(Domain.AbilityName.SurviveOneTurn, () => protectionFactory.CreateLimitedLifetime(Domain.AbilityName.SurviveOneTurn, 1));
            registry.Register(Domain.AbilityName.ComesBackFromDeath, () => combatFactory.CreateResurrection(Domain.AbilityName.ComesBackFromDeath, 1, false));
            registry.Register(Domain.AbilityName.ComesBackFromDeath5Times, () => combatFactory.CreateResurrection(Domain.AbilityName.ComesBackFromDeath5Times, 5, false));
            registry.Register(Domain.AbilityName.GiveDeathWhenDie, () => combatFactory.CreateDeathTrigger());

            // ============================================
            // COMBAT ABILITIES
            // ============================================
            registry.Register(Domain.AbilityName.SkipOpponentAttackEveryTurn, () => combatFactory.CreateSkipAttack(Domain.AbilityName.SkipOpponentAttackEveryTurn, true));

            // ============================================
            // SPECIAL ABILITIES
            // ============================================
            registry.Register(Domain.AbilityName.SendAllCardToHands, () => specialFactory.CreateSendAllToHand());
            registry.Register(Domain.AbilityName.ChangeFieldWithFieldFromDeck, () => specialFactory.CreateOptionalChangeField());

            // ============================================
            // DEFAULT ABILITY
            // ============================================
            registry.Register(Domain.AbilityName.Default, () => new DefaultAbility());
        }
    }

    /// <summary>
    /// Default ability implementation for cards without special abilities.
    /// </summary>
    public class DefaultAbility : IAbility
    {
        public Domain.AbilityName Name => Domain.AbilityName.Default;
        public string Description => "No special ability";

        public bool CanActivate(AbilityContext context) => false;
        public AbilityResult Execute(AbilityContext context) => AbilityResult.Failure("No ability to execute");
    }
}
