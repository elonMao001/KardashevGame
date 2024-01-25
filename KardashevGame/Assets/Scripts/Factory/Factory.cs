using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    public int FACTORYCAPACITY = 100;

    public Good[][] outputGoods;
    public int[] outputGoodsFill;
    public Good[][] inputGoods;
    public int[] inputGoodsFill;

    Vector3[] inputPos;
    Vector3[] outputPos;

    Recipe recipe = null;

    public int me;

    public float progress; //in seconds

    //Die Fabrik muss wissen welche sie ist 
    public void Init(int id)
    {
        me = id;
        inputPos = DataManager.GetAllBuildingInput(id);
        outputPos = DataManager.GetAllBuildingOutput(id);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(recipe != null && !NotWork())
            Work();
    }

    //Erhöht den Arbeitsfortschritt. Bei Fertigstellung werden die Edukte zerstört und neue Produkte erschaffen
    void Work()
    {
        progress += Time.deltaTime * 20;
        if(progress >= recipe.recipeRate)
        {
            for(int i = 0; i < inputGoodsFill.Length; i++)
            {
                SubtractGoods(inputGoods[i], inputGoodsFill, i, recipe.inputNumbers[i]);
            }
            for(int i = 0; i < outputGoodsFill.Length; i++)
            {
                AddGoods(outputGoods[i], outputGoodsFill, i, recipe.outputNumbers[i], recipe.outputIDs[i]);
            }
            progress = 0;
        }
    }

    //Überprüft ob es Gründe gibt, warum die Fabrik derzeit nicht arbeiten kann
    private bool NotWork()
    {
        for(int i = 0; i < inputGoodsFill.Length; i++)
        {
            if (inputGoodsFill[i] < recipe.inputNumbers[i])
                return true;
        }
        for (int i = 0; i < outputGoodsFill.Length; i++)
        {
            if (outputGoodsFill[i] + recipe.outputNumbers[i] > FACTORYCAPACITY)
                return true;
        }
        return false;
    }

    //Legt eine neues Rezept basierend auf einem Index fest
    public void SetRecipe(int recipe) {
        this.recipe = new Recipe(recipe);
        InitializeGoods(this.recipe.inputIDs.Length, this.recipe.outputIDs.Length);
    }

    //Legt eine neues Rezept basierend auf einem Rezept-Objekt fest
    public void SetRecipe(Recipe newRecipe)
    {
        this.recipe = newRecipe;
        InitializeGoods(this.recipe.inputIDs.Length, this.recipe.outputIDs.Length);
    }

    //Die beiden Arrays inputGoods und outputGoods, sowie die Arrays, die ihren jeweiligen Füllstand anzeigen müssen instanziert werden. Leider geht  = new Good[a][b]  nicht
    private void InitializeGoods(int inputSize, int outputSize)
    {
        inputGoods = new Good[inputSize][];
        outputGoods = new Good[outputSize][];
        inputGoodsFill = new int[inputSize];
        outputGoodsFill = new int[outputSize];

        for (int i = 0; i < Mathf.Max(inputSize, outputSize); i++)
        {
            if (i < inputSize)
                inputGoods[i] = new Good[FACTORYCAPACITY];
            if (i < outputSize)
                outputGoods[i] = new Good[FACTORYCAPACITY];
        }
    }

    //Eigentlich sinnlos, da man auch auf eine public Recipe Variable zugreifen könnte
    public Recipe GetRecipe() {
        return recipe;
    }

    //Vereinfacht das hinzufügen neuer Objekte zu den beiden Good-Arrays
    public void AddGoods(Good[] goods, int[] fill, int index, int amount, int good)
    {
        for (int i = fill[index]; i < fill[index] + amount; i++) {
            goods[i] = new Good(good);
        }
        fill[index] += amount;
    }

    //Vereinfacht das Entfernen von enthaltenen Objekten in beiden Good-Arrays. Gibt alle entfernten Goods aus
    public Good[] SubtractGoods(Good[] goods, int[] fill, int index, int amount)
    {
        Good[] ret = new Good[amount];
        for(int i = fill[index] - amount; i < fill[index]; i++)
        {
            Good g = goods[i];
            goods[i] = null;
            ret[i - (fill[index] - amount)] = g;
        }
        fill[index] -= amount;
        return ret;
    }

    //Conveyor sind nur an bestimmten Orten an Fabriken angeschlossen. Nach dem Zugang, der am nächsten zu einem point liegt wird hier gesucht
    public Vector3 GetClosestAccess(Vector3 point, out bool isOut) {
        double curDist = double.MaxValue;
        Vector3 curAccess = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        isOut = true;
        foreach(Vector3 v in inputPos)
        {
            if(curDist > Vector3.Distance(v, point))
            {
                curAccess = v;
                curDist = Vector3.Distance(v, point);
                isOut = false;
            }
        }

        foreach (Vector3 v in outputPos)
        {
            if (curDist > Vector3.Distance(v, point))
            {
                curAccess = v;
                curDist = Vector3.Distance(v, point);
                isOut = true;
            }
        }
        curAccess *= transform.localScale.x;
        curAccess += transform.position;
        return curAccess;
    }

    //Anstatt einem generellen Zugang wird hier nur ein Input gesucht
    public Vector3 GetClosesInput(Vector3 point) {
        double curDist = 999999999;
        Vector3 curInput = new Vector3(999999999, 999999999, 999999999);
        foreach (Vector3 v in inputPos)
        {
            if (curDist > Vector3.Distance(v, point))
            {
                curInput = v;
                curDist = Vector3.Distance(v, point);
            }
        }
        curInput *= transform.localScale.x;
        curInput += transform.position;
        return curInput;
    }

    //Anstatt einem generellen Zugang wird hier nur ein Output gesucht
    public Vector3 GetClosestOutput(Vector3 point) {
        double curDist = 999999999;
        Vector3 curOutput = new Vector3(999999999, 999999999, 999999999);
        foreach (Vector3 v in outputPos)
        {
            if (curDist > Vector3.Distance(v, point))
            {
                curOutput = v;
                curDist = Vector3.Distance(v, point);
            }
        }
        curOutput *= transform.localScale.x;
        curOutput += transform.position;
        return curOutput;
    }
}
