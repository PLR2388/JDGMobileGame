using UnityEngine;

public class FieldFunctions : MonoBehaviour
{
    private PlayerCards currentPlayerCard;
    private GameObject p1;
    private GameObject p2;

    [SerializeField] private GameObject miniCardMenu;

    // Start is called before the first frame update
    private void Start()
    {
        GameLoop.ChangePlayer.AddListener(ChangePlayer);
        InGameMenuScript.FieldCardEvent.AddListener(PutFieldCard);
        p1 = GameObject.Find("Player1");
        p2 = GameObject.Find("Player2");
        currentPlayerCard = p1.GetComponent<PlayerCards>();
    }

    private void PutFieldCard(FieldCard fieldCard)
    {
        if (currentPlayerCard.field != null) return;
        miniCardMenu.SetActive(false);
        currentPlayerCard.field = fieldCard;
        currentPlayerCard.handCards.Remove(fieldCard);
    }

    private void ChangePlayer()
    {
        currentPlayerCard = GameLoop.IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
    }
}