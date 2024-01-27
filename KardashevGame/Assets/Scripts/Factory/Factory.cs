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

    void Work()
    {
        progress += Time.deltaTime;
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

    public void SetRecipe(int recipe) {
        this.recipe = new Recipe(recipe);
        InitializeGoods(this.recipe.inputIDs.Length, this.recipe.outputIDs.Length);
    }

    public void SetRecipe(Recipe newRecipe)
    {
        this.recipe = newRecipe;
        InitializeGoods(this.recipe.inputIDs.Length, this.recipe.outputIDs.Length);
    }

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

    public Recipe GetRecipe() {
        return recipe;
    }

    public void AddGoods(Good[] goods, int[] fill, int index,  int amount, int good)
    {
        for (int i = fill[index]; i < fill[index] + amount; i++) {
            goods[i] = new Good(good);
        }
        fill[index] += amount;
    }

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

    public Vector3 GetClosestAccess(Vector3 point, out bool isOut) {
        double curDist = double.MaxValue;
        Vector3 curAccess = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        isOut = true;
        float corFactor = transform.localScale.x;
        foreach (Vector3 v in inputPos)
        {
            if(curDist > Vector3.Distance(v, point))
            {
                curAccess = v * corFactor;
                curDist = Vector3.Distance(v, point);
                isOut = false;
            }
        }

        foreach (Vector3 v in outputPos)
        {
            if (curDist > Vector3.Distance(v, point))
            {
                curAccess = v * corFactor;
                curDist = Vector3.Distance(v, point);
                isOut = true;
            }
        }
        curAccess += transform.position;
        return curAccess;
    }

    public Vector3 GetClosesInput(Vector3 point) {
        double curDist = 999999999;
        Vector3 curInput = new Vector3(999999999, 999999999, 999999999);
        float corFactor = transform.localScale.x;
        foreach (Vector3 v in inputPos)
        {
            if (curDist > Vector3.Distance(v, point))
            {
                curInput = v * corFactor;
                curDist = Vector3.Distance(v, point);
            }
        }
        curInput += transform.position;
        return curInput;
    }

    public Vector3 GetClosestOutput(Vector3 point) {
        double curDist = 999999999;
        Vector3 curOutput = new Vector3(999999999, 999999999, 999999999);
        float corFactor = transform.localScale.x;
        foreach (Vector3 v in outputPos)
        {
            if (curDist > Vector3.Distance(v, point))
            {
                curOutput = v * corFactor;
                curDist = Vector3.Distance(v, point);
            }
        }
        curOutput += transform.position;
        return curOutput;
    }
}