﻿using System.Linq;
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
        ApplyFieldCardEffect(fieldCard);
    }

    private void ApplyFieldCardEffect(FieldCard fieldCard)
    {
        var fieldCardEffect = fieldCard.FieldCardEffect;

        var keys = fieldCardEffect.Keys;
        var values = fieldCardEffect.Values;
        
        var family = fieldCard.GETFamily();
        for (var i = 0; i < keys.Count; i++)
        {
            switch (keys[i])
            {
                case FieldEffect.ATK:
                    // Must be called also when invocationCards change
                    var atk = float.Parse(values[i]);
                    foreach (var invocationCard in currentPlayerCard.invocationCards)
                    {
                        if (!invocationCard.GetFamily().Contains(family)) continue;
                        var newBonusAttack = invocationCard.GetBonusAttack() + atk;
                        invocationCard.SetBonusAttack(newBonusAttack);
                    }
                    break;
                case FieldEffect.DEF:
                    // Must be called also when invocationCards change
                    var def = float.Parse(values[i]);
                    foreach (var invocationCard in currentPlayerCard.invocationCards)
                    {
                        if (!invocationCard.GetFamily().Contains(family)) continue;
                        var newBonusAttack = invocationCard.GetBonusDefense() + def;
                        invocationCard.SetBonusDefense(newBonusAttack);
                    }
                    break;
                case FieldEffect.GetCard:
                    // Move to Draw phase
                    break;
                case FieldEffect.DrawCard:
                    // Move to Draw phase
                    break;
                case FieldEffect.Life:
                    // Move to Draw phase
                    break;
                case FieldEffect.Change:
                    // Must be called also when invocationCards change
                    var names = values[i].Split(';');
                    foreach (var invocationCard in currentPlayerCard.invocationCards)
                    {
                        if (names.Contains(invocationCard.Nom))
                        {
                            invocationCard.SetCurrentFamily(family);
                        }
                    }
                    break;
            }
        }
    }

    private void ChangePlayer()
    {
        currentPlayerCard = GameLoop.IsP1Turn ? p1.GetComponent<PlayerCards>() : p2.GetComponent<PlayerCards>();
    }
}