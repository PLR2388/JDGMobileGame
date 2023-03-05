using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HandCardDisplay : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;

    [FormerlySerializedAs("Player1")] public GameObject player1;

    [FormerlySerializedAs("Player2")] public GameObject player2;

    private List<GameObject> createdCards;

    private void Awake()
    {
        GameObject.Find("GameLoop").GetComponent<GameLoop>();
        createdCards = new List<GameObject>();
    }

    private void Update()
    {
        DisplayHandCard();
    }

    private void DisplayHandCard()
    {
        var handCards = GameLoop.IsP1Turn
            ? player1.GetComponent<PlayerCards>().handCards
            : player2.GetComponent<PlayerCards>().handCards;
        if (createdCards.Count < handCards.Count)
        {
            foreach (var handCard in handCards)
            {
                var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity);
                newCard.transform.SetParent(transform, true);
                newCard.GetComponent<CardImageBuilder>().card = handCard.baseCard;
                newCard.GetComponent<CardImageBuilder>().inGameCard = handCard;
                newCard.GetComponent<OnHover>().bIsInGame = true;

                createdCards.Add(newCard);
            }

            var rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(642 * handCards.Count, rectTransform.sizeDelta.y);
        }
        else if (createdCards.Count > handCards.Count)
        {
            foreach (var createdCard in createdCards)
            {
                Destroy(createdCard);
            }

            createdCards.Clear();
        }
    }

    private void OnDisable()
    {
        if (createdCards.Count <= 0) return;
        foreach (var createdCard in createdCards)
        {
            Destroy(createdCard);
        }

        createdCards.Clear();
    }
}