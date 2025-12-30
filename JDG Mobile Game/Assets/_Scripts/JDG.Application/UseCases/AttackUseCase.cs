using JDG.Application.Repositories;
using JDG.Domain.Entities;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;

namespace JDG.Application.UseCases
{
    /// <summary>
    /// Use case for executing an attack between two cards.
    /// Handles combat resolution and publishes AttackExecutedEvent.
    /// </summary>
    public class AttackUseCase
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IEventBus _eventBus;

        public AttackUseCase(
            IPlayerRepository playerRepository,
            ICardRepository cardRepository,
            IEventBus eventBus)
        {
            _playerRepository = playerRepository;
            _cardRepository = cardRepository;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Executes an attack from attacker to defender.
        /// </summary>
        public AttackResult Execute(PlayerId attackerId, CardId attackerCardId, CardId defenderCardId)
        {
            var attacker = _playerRepository.GetPlayer(attackerId);
            var defender = _playerRepository.GetPlayer(attackerId == PlayerId.Player1 ? PlayerId.Player2 : PlayerId.Player1);

            if (attacker == null || defender == null)
                return AttackResult.Failure("Player not found");

            if (attacker.BlockAttack)
                return AttackResult.Failure("Attack is blocked");

            var attackerCard = _cardRepository.GetCard(attackerCardId);
            var defenderCard = _cardRepository.GetCard(defenderCardId);

            if (attackerCard == null || defenderCard == null)
                return AttackResult.Failure("Card not found");

            if (!attackerCard.IsInvocation || !defenderCard.IsInvocation)
                return AttackResult.Failure("Only invocation cards can battle");

            if (!attackerCard.Stats.HasValue || !defenderCard.Stats.HasValue)
                return AttackResult.Failure("Cards must have stats to battle");

            // Execute combat
            var attackPower = attackerCard.Stats.Value.Attack;
            var defenderDefense = defenderCard.Stats.Value.Defense;

            // Reduce defender defense by attack power
            defenderCard.ModifyStats(0, -attackPower);

            // Reduce attacker defense by defender attack
            var counterAttackPower = defenderCard.Stats.Value.Attack;
            attackerCard.ModifyStats(0, -counterAttackPower);

            bool defenderDestroyed = defenderCard.IsDestroyed;
            bool attackerDestroyed = attackerCard.IsDestroyed;

            // Handle destroyed cards
            if (defenderDestroyed)
            {
                defender.DestroyCardFromField(defenderCard);
            }

            if (attackerDestroyed)
            {
                attacker.DestroyCardFromField(attackerCard);
            }

            // Save state
            _playerRepository.SavePlayer(attacker);
            _playerRepository.SavePlayer(defender);

            // Publish event
            _eventBus.Publish(new AttackExecutedEvent
            {
                AttackerId = attackerCardId.ToGuid(),
                DefenderId = defenderCardId.ToGuid(),
                Damage = attackPower,
                DefenderDestroyed = defenderDestroyed,
                AttackerDestroyed = attackerDestroyed
            });

            return AttackResult.Success(attackPower, defenderDestroyed, attackerDestroyed);
        }

        /// <summary>
        /// Executes a direct attack on a player.
        /// </summary>
        public AttackResult ExecuteDirectAttack(PlayerId attackerId, CardId attackerCardId, PlayerId defenderId)
        {
            var attacker = _playerRepository.GetPlayer(attackerId);
            var defender = _playerRepository.GetPlayer(defenderId);

            if (attacker == null || defender == null)
                return AttackResult.Failure("Player not found");

            var attackerCard = _cardRepository.GetCard(attackerCardId);
            if (attackerCard == null || !attackerCard.IsInvocation || !attackerCard.Stats.HasValue)
                return AttackResult.Failure("Invalid attacker card");

            var damage = attackerCard.Stats.Value.Attack;
            float healthDamage = defender.TakeDamage(damage);

            _playerRepository.SavePlayer(defender);

            _eventBus.Publish(new PlayerDamagedEvent
            {
                PlayerId = defenderId.ToCardOwner(),
                Damage = damage,
                HealthDamage = healthDamage,
                CurrentHealth = defender.Health,
                IsDefeated = defender.IsDefeated
            });

            return AttackResult.DirectAttackSuccess(damage, healthDamage, defender.IsDefeated);
        }
    }

    /// <summary>
    /// Result of an attack operation.
    /// </summary>
    public class AttackResult
    {
        public bool IsSuccess { get; private set; }
        public int Damage { get; private set; }
        public bool DefenderDestroyed { get; private set; }
        public bool AttackerDestroyed { get; private set; }
        public bool IsDirectAttack { get; private set; }
        public float HealthDamage { get; private set; }
        public bool PlayerDefeated { get; private set; }
        public string Message { get; private set; }

        public static AttackResult Success(int damage, bool defenderDestroyed, bool attackerDestroyed) => new AttackResult
        {
            IsSuccess = true,
            Damage = damage,
            DefenderDestroyed = defenderDestroyed,
            AttackerDestroyed = attackerDestroyed,
            IsDirectAttack = false,
            Message = "Attack executed successfully"
        };

        public static AttackResult DirectAttackSuccess(int damage, float healthDamage, bool playerDefeated) => new AttackResult
        {
            IsSuccess = true,
            Damage = damage,
            HealthDamage = healthDamage,
            PlayerDefeated = playerDefeated,
            IsDirectAttack = true,
            Message = "Direct attack executed successfully"
        };

        public static AttackResult Failure(string message) => new AttackResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}
