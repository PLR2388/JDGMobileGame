using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Card",menuName="FieldCard")]
public class FieldCard : Card
{
    private void Awake()
    {
        this.type= "field";
    }
}
