using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalCardDisplay : MonoBehaviour
{
public Card card;
public Material defaultMaterial;
public bool bIsFaceHidden;
private MeshRenderer meshRenderer;

private void Awake()
{
    meshRenderer = GetComponent<MeshRenderer>();
}

private void Update()
{
        if (bIsFaceHidden)
        {
            meshRenderer.material = defaultMaterial;
        }
        else
        {
            if (card)
            {
                meshRenderer.material = card.GetMaterialCard();
            }
            else
            {
                meshRenderer.material = defaultMaterial;
            }
        }
}

}
