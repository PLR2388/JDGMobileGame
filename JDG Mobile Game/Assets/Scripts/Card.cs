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

    public string Nom
    {
        get => nom;
    }

    public string Description
    {
        get => description;
    }

    public string DetailedDescription
    {
        get => descriptionDetaillee;
    }

    public string Type
    {
        get => type;
    }
    
    public Material MaterialCard
    {
        get => materialCard;
    }
    
    public bool Collector
    {
        get => collector;
    }

    public bool isValid()
    {
        return this != null && !string.IsNullOrEmpty(nom);
    }
}