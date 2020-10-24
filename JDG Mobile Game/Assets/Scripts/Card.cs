using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Card",menuName="Card")]
public class Card : ScriptableObject
{
    [SerializeField] protected string nom="";
    [SerializeField] protected string description;
    [SerializeField] protected string descriptionDetaillee;
    [SerializeField] protected string type;
    [SerializeField] protected Material materialCard;
    [SerializeField] protected bool collector;

    public Material GetMaterialCard()
    {
        return materialCard;
    }

    public string GetNom()
    {
        return nom;
    }
}
