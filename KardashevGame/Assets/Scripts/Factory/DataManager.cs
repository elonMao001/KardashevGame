using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataManager
{
    static readonly TextAsset R = (TextAsset) Resources.Load("Data/Recipes");
    static readonly TextAsset G = (TextAsset) Resources.Load("Data/Goods");
    static readonly TextAsset B = (TextAsset) Resources.Load("Data/Buildings");

    static readonly string ROWSEPARATOR = "   ";
    static readonly string COLOUMNSEPARATOR = "  ";

    static readonly string[] recipes = R.text.Split(ROWSEPARATOR);
    static readonly string[] goods = G.text.Split(ROWSEPARATOR);
    static readonly string[] buildings = B.text.Split(ROWSEPARATOR);

    public static readonly int NULLINDEX = -1;
    public static readonly Vector3 NULLVECTOR = Vector3.negativeInfinity;

    public static int GetRecipeIndex(string name) {
        foreach(string s in recipes)
        {
            if (s == "")
                break;
            string[] split = s.Split(COLOUMNSEPARATOR);
            if (split[1] == name)
                return int.Parse(split[0]);
        }
        return NULLINDEX;
    }

    public static int GetGoodIndex(string name) {
        foreach (string s in goods)
        {
            if (s == "")
                break;
            string[] split = s.Split(COLOUMNSEPARATOR);
            if (split[1] == name)
                return int.Parse(split[0]);
        }
        return NULLINDEX;
    }

    public static int GetBuildingIndex(string name)
    {
        foreach (string s in buildings)
        {
            if (s == "")
                break;
            string[] split = s.Split(COLOUMNSEPARATOR);
            if (split[1] == name)
                return int.Parse(split[0]);
        }
        return NULLINDEX;
    }

    public static string GetRecipeName(int recipe) {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        return s[1];
    }

    public static string GetGoodName(int good) {
        if (good > goods.Length - 1)
            return "";
        string[] s = goods[good].Split(COLOUMNSEPARATOR);
        return s[1];
    }

    public static string GetGoodImage(int good)
    {
        if (good > goods.Length - 1)
            return "";
        string[] s = goods[good].Split(COLOUMNSEPARATOR);
        return s[2];
    }

    public static int GetInputAmount(int recipe)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return NULLINDEX;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        return int.Parse(s[2]);
    }

    public static int GetOutputAmount(int recipe) {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return NULLINDEX;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        int location = 3 + 2 * int.Parse(s[2]);
        return int.Parse(s[location]);
    }

    public static int GetNInput(int recipe, int n)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return NULLINDEX;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        if (n > int.Parse(s[2]) - 1 )
            return -1;
        int location = 3 + n * 2;
        return int.Parse(s[location]);
    }

    public static int GetNOutput(int recipe, int n)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return NULLINDEX;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        //No Error correction
        int location = 4 + int.Parse(s[2]) * 2 + n * int.Parse(s[3 + int.Parse(s[2]) + 2]);
        return int.Parse(s[location]);
    }

    public static int GetNInputNumber(int recipe, int n) {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return NULLINDEX;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        if (n > int.Parse(s[2]) - 1)
            return -1;
        int location = 3 + n * 2;
        return int.Parse(s[location + 1]);
    }

    public static int GetNOutputNumber(int recipe, int n) {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return NULLINDEX;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        //No Error correction
        int location = 4 + int.Parse(s[2]) * 2 + n * int.Parse(s[3 + int.Parse(s[2]) + 2]);
        return int.Parse(s[location + 1]);
    }

    public static int[] GetAllInput(int recipe)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        int inAm = GetInputAmount(recipe);
        int[] ret = new int[inAm];
        for(int i = 0; i < inAm; i++)
        {
            ret[i] = int.Parse(s[3 + i * 2]);
        }
        return ret;
    }

    public static int[] GetAllOutput(int recipe)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        int ouAm = GetOutputAmount(recipe);
        int[] ret = new int[ouAm];
        for (int i = 0; i < ouAm; i++)
        {
            ret[i] = int.Parse(s[4 + int.Parse(s[2]) * 2 + i * int.Parse(s[3 + int.Parse(s[2]) + 2])]);
        }
        return ret;
    }

    public static int[] GetAllInputNumber(int recipe)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        int inAm = GetInputAmount(recipe);
        int[] ret = new int[inAm];
        for (int i = 0; i < inAm; i++)
        {
            ret[i] = int.Parse(s[3 + i * 2 + 1]);
        }
        return ret;
    }

    public static int[] GetAllOutputNumber(int recipe)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        int ouAm = GetOutputAmount(recipe);
        int[] ret = new int[ouAm];
        for (int i = 0; i < ouAm; i++)
        {
            ret[i] = 3 + 2 * int.Parse(s[2]);
            ret[i] += 2 * int.Parse(s[ret[i]]);
            ret[i] = int.Parse(s[ret[i]]);
        }
        return ret;
    }

    public static int GetRecipeBuilding(int recipe)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return NULLINDEX;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        return int.Parse(s[s.Length - 2]);
    }

    public static float GetRecipeDuration(int recipe) {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return NULLINDEX;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        return float.Parse(s[s.Length-1]);
    }

    public static string GetBuildingName(int building)
    {
        if (building == NULLINDEX || building < 0 || building >= buildings.Length)
            return null;
        string[] s = buildings[building].Split(COLOUMNSEPARATOR);
        return s[1];
    }

    public static int GetBuildingInputAmount(int building)
    {
        if (building == NULLINDEX || building < 0 || building >= buildings.Length)
            return NULLINDEX;
        string[] s = buildings[building].Split(COLOUMNSEPARATOR);
        return int.Parse(s[2]);
    }

    public static int GetBuildingOutputAmount(int building)
    {
        if (building == NULLINDEX || building < 0 || building >= buildings.Length)
            return NULLINDEX;
        string[] s = buildings[building].Split(COLOUMNSEPARATOR);
        return int.Parse(s[3 + int.Parse(s[2]) * 2]);
    }

    public static Vector3 GetNBuildingInput(int building, int n)
    {
        if (building == NULLINDEX || building < 0 || building >= buildings.Length)
            return NULLVECTOR;
        if (n < 0)
            return Vector3.zero;
        string[] s = buildings[building].Split(COLOUMNSEPARATOR);
        return new Vector3(float.Parse(s[n * 3 + 3]), float.Parse(s[n * 3 + 4]), float.Parse(s[n * 3 + 5]));
    }

    public static Vector3 GetNBuildingOutput(int building, int n)
    {
        if (building == NULLINDEX || building < 0 || building >= buildings.Length)
            return NULLVECTOR;
        string[] s = buildings[building].Split(COLOUMNSEPARATOR);
        int offset = int.Parse(s[2]) * 3 + 3;
        return new Vector3(float.Parse(s[offset + n * 3]), float.Parse(s[offset + n * 3 + 1]), float.Parse(s[offset + n * 3]));
    }

    public static Vector3[] GetAllBuildingInput(int building)
    {
        if (building == NULLINDEX || building < 0 || building >= buildings.Length)
            return null;
        string[] s = buildings[building].Split(COLOUMNSEPARATOR);
        int amount = int.Parse(s[2]);
        Vector3[] ret = new Vector3[amount];
        for(int i = 0; i < amount; i++)
        {
            ret[i] = new Vector3(float.Parse(s[3 + i * 3]), float.Parse(s[4 + i * 3]), float.Parse(s[5 + i * 3]));
        }
        return ret;
    }

    public static Vector3[] GetAllBuildingOutput(int building)
    {
        if (building == NULLINDEX || building < 0 || building >= buildings.Length)
            return null;
        string[] s = buildings[building].Split(COLOUMNSEPARATOR);
        int offset = 3 + 3 * int.Parse(s[2]);
        int amount = int.Parse(s[offset]);
        Vector3[] ret = new Vector3[amount];
        for(int i = 0; i < amount; i++)
        {
            ret[i] = new Vector3(float.Parse(s[offset + i * 3 + 1]), float.Parse(s[offset + i * 3 + 2]), float.Parse(s[offset + i * 3 + 3]));
        }
        return ret;
    }

    public static string GetNInputName(int recipe, int n)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        return GetGoodName(GetNInput(recipe, n));
    }

    public static string GetNOutputName(int recipe, int n) {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        return GetGoodName(GetNOutput(recipe, n));
    }

    public static string[] GetAllInputName(int recipe)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        int[] indices = GetAllInput(recipe);
        string[] ret = new string[indices.Length];
        for (int i = 0; i < indices.Length; i++)
        {
            ret[i] = GetGoodName(indices[i]);
        }
        return ret;
    }

    public static string[] GetAllOutputName(int recipe) {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        int[] indices = GetAllOutput(recipe);
        string[] ret = new string[indices.Length];
        for (int i = 0; i < indices.Length; i++)
        {
            ret[i] = GetGoodName(indices[i]);
        }
        return ret;
    }

    public static string GetNInputImage(int recipe, int n)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        return GetGoodImage(GetNInput(recipe, n));
    }

    public static string GetNOutputImage(int recipe, int n)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        return GetGoodImage(GetNOutput(recipe, n));
    }

    public static string[] GetAllInputImage(int recipe)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        int[] indices = GetAllInput(recipe);
        string[] ret = new string[indices.Length];
        for (int i = 0; i < indices.Length; i++)
        {
            ret[i] = GetGoodImage(indices[i]);
        }
        return ret;
    }

    public static string[] GetAllOutputImage(int recipe)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return null;
        int[] indices = GetAllOutput(recipe);
        string[] ret = new string[indices.Length];
        for (int i = 0; i < indices.Length; i++)
        {
            ret[i] = GetGoodImage(indices[i]);
        }
        return ret;
    }

    public static int GetFactory(int recipe)
    {
        if (recipe == NULLINDEX || recipe < 0 || recipe >= recipes.Length)
            return NULLINDEX;
        string[] s = recipes[recipe].Split(COLOUMNSEPARATOR);
        int location = int.Parse(s[2]) * 2 + 4 + int.Parse(s[int.Parse(s[2]) * 2 + 3]) * 2;
        return int.Parse(s[location]);
    }

    public static int[] GetRecipesForFactory(int factory)
    {
        int[] ret = new int[recipes.Length-1];
        for (int i = 0; i < recipes.Length-1; i++)
        {
            ret[i] = NULLINDEX;
            if(GetFactory(i) == factory)
            {
                ret[i] = i;
            }
        }

        return Support.ResizeArray(ret, NULLINDEX);
    }
}