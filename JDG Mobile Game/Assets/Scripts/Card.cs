using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public string GetType()
    {
        return type;
    }

    public string GetDescription()
    {
        return description;
    }

    public string GetDescriptionDetaillee()
    {
        return descriptionDetaillee;
    }

    public bool IsCollector()
    {
        return collector;
    }
}
