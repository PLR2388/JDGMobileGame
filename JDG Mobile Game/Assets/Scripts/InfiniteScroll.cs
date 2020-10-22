using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{

    [SerializeField] private GameObject prefabCard;

    [SerializeField] private List<Card> AllCards;
    // Start is called before the first frame update
    void Start()
    {
        DisplayCard(prefabCard);
    }

    private void DisplayCard(GameObject prefaGameObject)
    {
        for (int i = 0; i < AllCards.Count; i++)
        {
            GameObject newCard=Instantiate(prefaGameObject, Vector3.zero, Quaternion.identity);

            newCard.transform.SetParent(transform,true);
            
            //newCard.transform.localScale=new Vector3(0.3f,1,1);
            
            newCard.GetComponent<CardDisplay>().card = AllCards[i];
        }
        /* string path = Application.dataPath + "/Resources/Cards";
        foreach (string file in System.IO.Directory.GetFiles(path,"*.asset"))
        {
            GameObject newCard=Instantiate(prefaGameObject, Vector3.zero, Quaternion.identity);

            newCard.transform.SetParent(transform,true);
            //newCard.transform.localScale=new Vector3(0.3f,1,1);
            string nameCard = Path.GetFileNameWithoutExtension(file);
            Card Card = Resources.Load<Card>("Cards/"+nameCard);
            newCard.GetComponent<CardDisplay>().card = Card;


        }
       DirectoryInfo directoryInfo = new DirectoryInfo(path);
                 FileInfo[] allFiles = directoryInfo.GetFiles("*.*");
                 foreach (FileInfo file in allFiles)
                 {
                     StartCoroutine("LoadPlayerUI", file);
                     
                 }*/
    }
}
