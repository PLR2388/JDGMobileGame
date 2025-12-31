using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Cards;
using UnityEngine;
using VContainer;
using JDG.Application.Services;

/// <summary>
/// Phase 9: Removed CardPoolManager singleton dependency via DI.
/// Phase 41: Migrated to clean JDG.Application.Services.ICardSelectionService.
/// Phase 91: Converted from StaticInstance<T> to regular MonoBehaviour.
/// All dependencies are injected via VContainer.
/// </summary>
public class DisplayCards : MonoBehaviour
{
    private readonly ObservableCollection<InGameCard> _cardsList = new ObservableCollection<InGameCard>();
    private readonly List<GameObject> associatedGameObject = new List<GameObject>();

    // Phase 9 & 41: Injected dependencies
    private ICardPoolService _cardPoolService;
    private ICardSelectionService _cardSelectionService;

    /// <summary>
    /// Sets the list of cards to be displayed and triggers the card display.
    /// </summary>
    public List<InGameCard> CardsList
    {
        set
        {
            _cardsList.Clear();
            foreach (var card in value)
            {
                _cardsList.Add(card);
            }
            DisplayNewCards(_cardsList);
        }
    }

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 9: Inject services instead of using singletons.
    /// Phase 41: Migrated to clean JDG.Application.Services.ICardSelectionService.
    /// </summary>
    [Inject]
    public void Construct(ICardPoolService cardPoolService, ICardSelectionService cardSelectionService)
    {
        _cardPoolService = cardPoolService;
        _cardSelectionService = cardSelectionService;
    }

    /// <summary>
    /// Sets up event listener on start.
    /// </summary>
    private void Start()
    {
        _cardsList.CollectionChanged += CardList_CollectionChanged;
    }

    /// <summary>
    /// Reverts all changes and cleans up before destruction.
    /// </summary>
    private void OnDestroy()
    {
        foreach (var cardGameObject in associatedGameObject)
        {
            // Phase 9: Use injected service instead of CardPoolManager.Instance
            if (_cardPoolService?.CardPoolHolder != null)
            {
                cardGameObject.transform.SetParent(_cardPoolService.CardPoolHolder, true);
            }

            // Phase 9: Use injected service instead of CardSelectionManager.Instance
            _cardSelectionService?.UnselectCard(cardGameObject.GetComponent<CardDisplay>().InGameCard);

            cardGameObject.SetActive(false);
        }
        associatedGameObject.Clear();
        _cardsList.CollectionChanged -= CardList_CollectionChanged;
    }

    /// <summary>
    /// Displays new cards from the given list.
    /// Phase 140: Added tracing for debugging missing card display.
    /// </summary>
    /// <param name="newItems">List of new cards to be displayed.</param>
    private void DisplayNewCards(IList newItems)
    {
        Debug.Log($"DisplayCards.DisplayNewCards() - START, count: {newItems?.Count ?? -1}");
        foreach (var card in newItems)
        {
            var inGameCard = card as InGameCard;
            Debug.Log($"DisplayCards.DisplayNewCards() - Card: {inGameCard?.Title ?? "NULL"}, Type: {card?.GetType().FullName ?? "NULL"}");

            // Phase 9: Use injected service instead of CardPoolManager.Instance
            var newCardObject = _cardPoolService?.GetPooledObject(inGameCard);
            Debug.Log($"DisplayCards.DisplayNewCards() - GetPooledObject returned: {(newCardObject != null ? "FOUND" : "NULL")}");

            if (newCardObject != null)
            {
                newCardObject.transform.SetParent(transform, true);
                newCardObject.SetActive(true);
                associatedGameObject.Add(newCardObject);
                Debug.Log($"DisplayCards.DisplayNewCards() - Card displayed successfully: {inGameCard?.Title}");
            }
            else
            {
                Debug.LogWarning($"DisplayCards.DisplayNewCards() - Card NOT in pool! Cannot display: {inGameCard?.Title ?? "NULL"}");
            }
        }
        UpdateRectSize();
    }

    /// <summary>
    /// Handles changes to the card collection.
    /// </summary>
    private void CardList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                DisplayNewCards(e.NewItems);
                break;
            case NotifyCollectionChangedAction.Move:
                break;
            case NotifyCollectionChangedAction.Remove:
                HideCard(e.OldItems);
                break;
            case NotifyCollectionChangedAction.Replace:
                break;
            case NotifyCollectionChangedAction.Reset:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        UpdateRectSize();
    }
    
    /// <summary>
    /// Hides the specified cards.
    /// </summary>
    /// <param name="e">List of cards to be hidden.</param>
    private void HideCard(IList e)
    {
        foreach (var card in e)
        {
            // Phase 9: Use injected service instead of CardPoolManager.Instance
            _cardPoolService?.GetPooledObject(card as InGameCard)?.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the size of the card display area based on the number of cards.
    /// </summary>
    private void UpdateRectSize()
    {
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300 * _cardsList.Count, rectTransform.sizeDelta.y);
    }
}