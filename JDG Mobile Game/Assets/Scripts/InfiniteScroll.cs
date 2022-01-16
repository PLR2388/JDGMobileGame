using Cards;
using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] private GameObject prefabCard;
    [SerializeField] private GameObject messageBox;

    private int numberSelected;
    private int numberRare;

    // Start is called before the first frame update
    private void Start()
    {
        var allCards = FindObjectOfType<GameState>().allCards;
        foreach (var card in allCards)
        {
            if (card.Type == CardType.Contre) continue;
            if(card.Nom == "Attaque de la tour Eiffel" || card.Nom == "Blague interdite" || card.Nom == "Un bon tuyau") continue;
            var newCard = Instantiate(prefabCard, Vector3.zero, Quaternion.identity);

            newCard.GetComponent<OnHover>().bIsInGame = false;
            newCard.transform.SetParent(transform, true);
            newCard.GetComponent<CardDisplay>().card = card;
        }
    }

    private void DisplayMessageBox(string msg)
    {
        var message = Instantiate(messageBox);
        message.GetComponent<MessageBox>().title = "Modifie ton deck";
        message.GetComponent<MessageBox>().isInformation = true;
        message.GetComponent<MessageBox>().description = msg;
    }

    private void Update()
    {
        numberSelected = 0;
        numberRare = 0;
        var children = GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            var childGameObject = child.gameObject;
            if (childGameObject.GetComponent<OnHover>() == null) continue;
            var isSelected = childGameObject.GetComponent<OnHover>().bIsSelected;
            if (!isSelected) continue;
            if (numberSelected < GameState.MaxDeckCards)
            {
                numberSelected++;
                if (!childGameObject.GetComponent<CardDisplay>().card.Collector) continue;
                if (numberRare < GameState.MaxRare)
                {
                    numberRare++;
                }
                else
                {
                    childGameObject.GetComponent<OnHover>().bIsSelected = false;
                    DisplayMessageBox("Tu ne peux pas avoir plus de 5 cartes brillantes !");
                }
            }
            else
            {
                childGameObject.GetComponent<OnHover>().bIsSelected = false;
                DisplayMessageBox("Tu ne peux pas avoir plus de 30 cartes !");
            }
        }
    }
}