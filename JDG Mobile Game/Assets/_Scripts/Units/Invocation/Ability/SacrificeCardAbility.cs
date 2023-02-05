using _Scripts.Units.Invocation;
using UnityEngine;

public class SacrificeCardAbility : Ability
{
    protected readonly string cardName;

    public SacrificeCardAbility(AbilityName name, string description, string cardName)
    {
        Name = name;
        Description = description;
        this.cardName = cardName;
    }
    
    protected static void DisplayOkMessage(Transform canvas, GameObject messageBox)
    {
        messageBox.SetActive(false);
        GameObject messageBox1 = MessageBox.CreateOkMessageBox(canvas, "Attention",
            "Tu dois sélectionner une carte à sacrifier");
        messageBox1.GetComponent<MessageBox>().OkAction = () =>
        {
            messageBox.SetActive(true);
            Object.Destroy(messageBox1);
        };
    }
    public override void ApplyEffect(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        InGameInvocationCard invocationCard = playerCards.invocationCards.Find(card => cardName == card.Title);
        // TODO Centralize death invocation Card
        if (invocationCard.EquipmentCard != null)
        {
            playerCards.yellowTrash.Add(invocationCard.EquipmentCard);
        }
        playerCards.invocationCards.Remove(invocationCard);
        playerCards.yellowTrash.Add(invocationCard);
    }

    public override void OnTurnStart(Transform canvas, PlayerCards playerCards, PlayerCards opponentPlayerCards)
    {
        
    }

    public override void OnCardAdded(Transform canvas, InGameInvocationCard newCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
   
    }

    public override void OnCardRemove(Transform canvas, InGameInvocationCard removeCard, PlayerCards playerCards,
        PlayerCards opponentPlayerCards)
    {
        
    }
}
