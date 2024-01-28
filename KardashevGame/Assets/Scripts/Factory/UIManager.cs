using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject factoryUI;
    public GameObject selectRecipeUI;
    public bool uIopen = false;

    GameObject openedFactory = null;
    GameObject openendUI = null;

    void Start()
    {
        
    }

    void Update()
    {
        if (uIopen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseUI();
                uIopen = false;
                openedFactory = null;
                openendUI = null;
                Builder.selected = 'I';
                return;
            }
            else
            {
                UpdateProgressAndCounts();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                CheckRay();
            }
        }
    }

    //Führt einen RayCast aus um zu schauen ob und welche Fabrik ausgeählt wurde
    void CheckRay() {
        RaycastHit hit;
        if(uIopen || !Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100) || Builder.selected != 'i')
            return;
        if (hit.collider.gameObject.CompareTag("Factory"))
        {
            OpenUI(hit.collider.gameObject);
        }
    }

    //Öffnet eine der beiden UI-Varianten
    void OpenUI(GameObject factory)
    {
        uIopen = true;
        Recipe recipe = factory.GetComponent<Factory>().GetRecipe();

        if (recipe == null || recipe.ID == DataManager.NULLINDEX) {
            OpenRecipeSelect(factory);
            return;
        }

        GameObject UI = Instantiate(factoryUI);
        UI.GetComponent<Canvas>().worldCamera = Camera.main;
        UI.GetComponent<Canvas>().planeDistance = 1;

        UI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = recipe.recipeRate + "s ";
        UI.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = recipe.recipeName;

        CreateItemSlots(UI, "ItemSlotOutput", recipe.outputImages, recipe.outputNames, recipe.outputNumbers, 1);
        if (recipe.inputIDs.Length == 0) {
            Destroy(UI.transform.GetChild(0).GetChild(3).gameObject);
        } 
        else {
            CreateItemSlots(UI, "ItemSlotInput", recipe.inputImages, recipe.inputNames, recipe.inputNumbers, -1);
        }

        UI.GetComponent<Canvas>().worldCamera = Camera.main;

        openedFactory = factory;
        openendUI = UI;
    }

    //Hilfsmethode um nicht für Input- und Output-Slots zweimal fast das gleiche zu schreiben
    //Kopiert das Original-Panel und initialisiert die Kopien
    private void CreateItemSlots(GameObject UI, string original, string[] images, string[] names, int[] numbers, int x)
    {
        GameObject origPanel = null;
        for (int i = 0; i < UI.transform.GetChild(0).childCount; i++)
        {
            if (UI.transform.GetChild(0).GetChild(i).name.Equals(original))
            {
                origPanel = UI.transform.GetChild(0).GetChild(i).gameObject;
                break;
            }
        }
        if (origPanel == null)
            return;

        origPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(-135 * x, (images.Length - 1) * 50, 0);
        origPanel.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Goods Sprites/" + images[0]);
        origPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = names[0];
        origPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = numbers[0] + "x ";
        for (int i = 1; i < images.Length; i++)
        {
            GameObject newPanel = Instantiate(origPanel);
            newPanel.transform.SetParent(UI.transform.GetChild(0));
            newPanel.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            newPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(-135 * x, (images.Length - 1) * 50 - 100 * i, 0);
            newPanel.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 0);
            newPanel.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Goods Sprites/" + images[i]);
            newPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = names[i];
            newPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = numbers[i] + "x ";
            if(x == -1)
                newPanel.transform.SetSiblingIndex(3 + 1);
        }
    }

    //Zweite UI-Variante. Wie bei CreateItemSlots() wird hier eine originelle Panel kopiert und die Kopien initialisiert
    void OpenRecipeSelect(GameObject factory) {
        CloseUI();
        int[] possibleRecipes = DataManager.GetRecipesForFactory(factory.GetComponent<Factory>().me);
        GameObject UI = Instantiate(selectRecipeUI);
        GameObject origPanel = UI.transform.GetChild(0).GetChild(0).gameObject;
        origPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(130, -100, 0);
        origPanel.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Goods Sprites/" + DataManager.GetNOutputImage(possibleRecipes[0], 0));
        origPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = DataManager.GetRecipeName(possibleRecipes[0]);
        origPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = DataManager.GetNOutputNumber(possibleRecipes[0], 0) + "x in " + DataManager.GetRecipeDuration(possibleRecipes[0]) + "s";
        for(int i = 1; i < possibleRecipes.Length; i++)
        {
            GameObject newPanel = Instantiate(origPanel);
            newPanel.transform.SetParent(UI.transform.GetChild(0));
            newPanel.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            newPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(130 + 225 * (i/4), -100 - i * 80 + 400 * (i/4), 0);
            newPanel.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 0);
            newPanel.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Goods Sprites/" + DataManager.GetNOutputImage(possibleRecipes[i], 0));
            newPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = DataManager.GetRecipeName(possibleRecipes[i]);
            newPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = DataManager.GetNOutputNumber(possibleRecipes[i], 0) + "x in " + DataManager.GetRecipeDuration(possibleRecipes[i]) + "s";
        }

        UI.GetComponent<Canvas>().worldCamera = Camera.main;
        UI.GetComponent<Canvas>().planeDistance = 1;

        openedFactory = factory;
        openendUI = UI;
    }

    void UpdateProgressAndCounts()
    {
        Factory factory = openedFactory.GetComponent<Factory>();
        Recipe recipe = factory.GetRecipe();
        if (recipe == null)
            return;
        Transform background = openendUI.transform.GetChild(0);

        string s;
        if (recipe.inputIDs.Length != 0) {
            for (int i = 3; i < recipe.inputIDs.Length + 3; i++)
            {
                s = background.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text;
                s = s.Split(" ")[0];
                s += " (" + Support.ResizeArray(factory.inputGoods[i - 3], null).Length + ")"; //Kann vereinfacht werden
                background.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text = s;
            }
        }
        
        for(int i = 3 + recipe.inputIDs.Length; i < recipe.inputIDs.Length + 3 + recipe.outputIDs.Length; i++)
        {
            s = background.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text;
            s = s.Split(" ")[0];
            s += " (" + Support.ResizeArray(factory.outputGoods[i - 3 - recipe.inputIDs.Length], null).Length + ")"; //man kann outputGoodFill benutzen
            background.GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().text = s;
        }

        s = background.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        s = s.Split(" ")[0];
        s += " (" + (int) factory.progress + "s)";
        background.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = s;

        background.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1) * factory.progress / recipe.recipeRate;
        Color c = background.GetChild(0).GetComponent<Image>().color;
        c = new Color(c.r, c.g, c.b, 255);
        background.GetChild(0).GetComponent<Image>().color = c;
    }

    public void RecipeButtonPressed(int button)
    {
        openedFactory.GetComponent<Factory>().SetRecipe(DataManager.GetRecipesForFactory(openedFactory.GetComponent<Factory>().me)[button]);
        CloseUI();
        OpenUI(openedFactory);
    }

    public void SelectButtonPressed()
    {
        CloseUI();
        OpenRecipeSelect(openedFactory);
    }

    void CloseUI() {
        if (uIopen)
        {
            foreach (GameObject ui in GameObject.FindGameObjectsWithTag("UI"))
            {
                Destroy(ui);
            }
        }
    }
}