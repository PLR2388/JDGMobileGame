using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
public Card card;
public Material defaultMaterial;
public bool bIsFaceHidden;
private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        image.material = defaultMaterial;
    }

    private void Update()
    {
        if (bIsFaceHidden)
        {
            image.material = defaultMaterial;
        }
        else
        {
            if (card)
            {
                image.material = card.GetMaterialCard();
            }
        }
    }
}
