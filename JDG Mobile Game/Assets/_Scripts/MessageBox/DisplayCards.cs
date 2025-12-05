using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Cards;
using UnityEngine;
using VContainer;

/// <summary>
/// Phase 9: Removed CardPoolManager singleton dependency via DI.
/// </summary>
public class DisplayCards : StaticInstance<DisplayCards>
{
    private readonly ObservableCollection<InGameCard> _cardsList = new ObservableCollection<InGameCard>();
    private readonly List<GameObject> associatedGameObject = new List<GameObject>();

    // Phase 9: Injected dependencies
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
    /// </summary>
    /// <param name="newItems">List of new cards to be displayed.</param>
    private void DisplayNewCards(IList newItems)
    {
        foreach (var card in newItems)
        {
            // Phase 9: Use injected service instead of CardPoolManager.Instance
            var newCardObject = _cardPoolService?.GetPooledObject(card as InGameCard);
            if (newCardObject != null)
            {
                newCardObject.transform.SetParent(transform, true);
                newCardObject.SetActive(true);
                associatedGameObject.Add(newCardObject);
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