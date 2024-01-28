using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Die Klasse SelectButton existiert nur, weil sich die geklonten Button nicht direkt an die UImanager-Klasse des GameObject Player verbinden lieﬂen
public class SelectButton : MonoBehaviour
{
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject g in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (g.CompareTag("Player"))
            {
                player = g;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RecipeChosen() {
        Debug.Log("chosen");
        player.GetComponent<UIManager>().RecipeButtonPressed(transform.parent.GetSiblingIndex()); //Muss von parent sein
    }

    public void ChooseRecipe()
    {
        Debug.Log("Choose");
        player.GetComponent<UIManager>().SelectButtonPressed();
    }
}