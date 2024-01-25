using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //Die Buttons in den Recipe-Slots wollten sich nicht direkt mit der UI-Manager-Klasse verbinden lassen, weshalb hier ein umständlicher Weg gewählt wurde um die Zielfunktionen aufzurufen

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
