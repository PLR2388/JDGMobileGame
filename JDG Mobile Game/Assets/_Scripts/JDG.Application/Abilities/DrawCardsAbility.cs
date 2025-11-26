using JDG.Domain;
using JDG.Application.UseCases;

namespace JDG.Application.Abilities
{
    /// <summary>
    /// Ability that allows the player to draw cards from their deck.
    /// Modern implementation using use cases (no Unity dependencies).
    /// </summary>
    public class DrawCardsAbility : IAbility
    {
        private readonly DrawCardUseCase _drawCardUseCase;
        private readonly int _numberOfCards;

        public AbilityName Name { get; }
        public string Description { get; }

        /// <summary>
        /// Creates a new draw cards ability.
        /// </summary>
        /// <param name="numberOfCards">Number of cards to draw</param>
        /// <param name="drawCardUseCase">Use case for drawing cards</param>
        public DrawCardsAbility(int numberOfCards, DrawCardUseCase drawCardUseCase)
        {
            _numberOfCards = numberOfCards;
            _drawCardUseCase = drawCardUseCase;
            Name = AbilityName.Draw2Cards;
            Description = $"Draw {numberOfCards} card(s) from your deck";
        }

        public bool CanActivate(AbilityContext context)
        {
            // Can always attempt to draw (even if deck is empty, it just won't draw anything)
            return true;
        }

        public AbilityResult Execute(AbilityContext context)
        {
            int cardsDrawn = 0;

            for (int i = 0; i < _numberOfCards; i++)
            {
                var result = _drawCardUseCase.Execute(context.CurrentPlayerId);

                if (result.IsSuccess)
                {
                    cardsDrawn++;
                }
                else
                {
                    // Stop drawing if we fail (e.g., deck is empty)
                    break;
                }
            }

            if (cardsDrawn == 0)
            {
                return AbilityResult.Failure("No cards to draw from deck");
            }

            return AbilityResult.Success($"Drew {cardsDrawn} card(s)");
        }
    }

    /// <summary>
    /// Factory for creating draw card abilities with dependency injection.
    /// </summary>
    public class DrawCardsAbilityFactory
    {
        private readonly DrawCardUseCase _drawCardUseCase;

        public DrawCardsAbilityFactory(DrawCardUseCase drawCardUseCase)
        {
            _drawCardUseCase = drawCardUseCase;
        }

        /// <summary>
        /// Creates a Draw 2 Cards ability (most common).
        /// </summary>
        public DrawCardsAbility CreateDraw2Cards()
        {
            return new DrawCardsAbility(2, _drawCardUseCase);
        }

        /// <summary>
        /// Creates a draw ability for any number of cards.
        /// </summary>
        public DrawCardsAbility CreateDrawNCards(int n)
        {
            return new DrawCardsAbility(n, _drawCardUseCase);
        }
    }
}
