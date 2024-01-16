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

    public void RecipeChosen() {
        player.GetComponent<UIManager>().RecipeButtonPressed(transform.parent.GetSiblingIndex()); //Muss von parent sein
    }

    public void ChooseRecipe()
    {
        player.GetComponent<UIManager>().SelectButtonPressed();
    }
}
