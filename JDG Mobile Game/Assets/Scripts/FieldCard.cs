﻿using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "FieldCard")]
public class FieldCard : Card
{
    [SerializeField] private CardFamily family;
    [SerializeField] private FieldCardEffect fieldCardEffect;

    public FieldCardEffect FieldCardEffect => fieldCardEffect;

    private void Awake()
    {
        this.type = CardType.Field;
    }

    public CardFamily GETFamily()
    {
        return family;
    }
}