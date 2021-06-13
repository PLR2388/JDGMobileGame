using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Card",menuName="FieldCard")]
public class FieldCard : Card
{
    [SerializeField] private string family;
    private void Awake()
    {
        this.type= "field";
    }

    public string getFamily()
    {
        return family;
    }
}
