using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "New Card", menuName = "SuperInvocationCard")]
public class SuperInvocationCard : InvocationCard
{
    public List<InvocationCard> invocationCards;

    public void Init(List<InvocationCard> invocationCardsList) {
        invocationCards = invocationCardsList;
        var totalAttack = 0.0f;
        var totalDefense = 0.0f;
        var newName = "";
        Material material;
        foreach(var invocationCard in invocationCardsList) {
            totalAttack += invocationCard.GetAttack();
            totalDefense += invocationCard.GetDefense();
            newName += invocationCard.Nom;
        }
        attack = totalAttack;
        defense = totalDefense;
        nom = newName;

        // TODO Study this to create a text
        var perCharacterKernings = new PerCharacterKerning[]
        {
            new PerCharacterKerning("a",0.221f),
            new PerCharacterKerning("b",0.221f),
            new PerCharacterKerning("c",0.221f),
            new PerCharacterKerning("d",0.221f),
            new PerCharacterKerning("e",0.221f),
            new PerCharacterKerning("f",0.221f),
            new PerCharacterKerning("g",0.221f),
            new PerCharacterKerning("h",0.221f),
            new PerCharacterKerning("i",0.221f),
            new PerCharacterKerning("j",0.221f),
            new PerCharacterKerning("k",0.221f),
            new PerCharacterKerning("l",0.221f),
            new PerCharacterKerning("m",0.221f),
            new PerCharacterKerning("n",0.221f),
            new PerCharacterKerning("o",0.221f),
            new PerCharacterKerning("p",0.221f),
            new PerCharacterKerning("q",0.221f),
            new PerCharacterKerning("r",0.221f),
            new PerCharacterKerning("s",0.221f),
            new PerCharacterKerning("t",0.221f),
            new PerCharacterKerning("u",0.221f),
            new PerCharacterKerning("v",0.221f),
            new PerCharacterKerning("w",0.221f),
            new PerCharacterKerning("x",0.221f),
            new PerCharacterKerning("y",0.221f),
            new PerCharacterKerning("z",0.221f),
        };
   // var textToTexture = new TextToTexture(Font.CreateDynamicFontFromOSFont("Liberation Sans", 12), 10, 10,
    //       perCharacterKernings, true);
      //  var texture = textToTexture.CreateTextToTexture(name.ToLower(), 0, 0, 2000, 15, 1);
        //material = new Material(invocationCards[0].MaterialCard)
        //{
          //  mainTexture = texture
        //};
    }

    private void CombinePictureCards(List<Material> materials)
    {
        var material1 = materials[0];
        var material2 = materials[1];

        var gradient = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[10000];
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[10000];

        for (var i = 0; i < 10000; i++)
        {
            // Populate the color keys at the relative time 0 and 1 (0 and 100%)
            colorKey[i].color = material1.color;
            colorKey[i].time = 0.0f;

            // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
            alphaKey[i].alpha = 1.0f;
            alphaKey[i].time = 0.0f;
        }

   


        //var material = new Material();


    }
}
