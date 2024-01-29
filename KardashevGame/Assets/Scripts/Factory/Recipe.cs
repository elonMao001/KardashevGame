using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Die Klasse Recipe dient als anschaulichere Zusammenfassung eines Rezeptes im Vergleich zum DataManager
//Außederm verhindert sie redundante und möglicherweise aufwendige Aufrufe der DataManager-Methoden
public class Recipe
{
    public int[] inputIDs;
    public int[] outputIDs;
    public int[] inputNumbers;
    public int[] outputNumbers;
    public string[] inputNames;
    public string[] outputNames;
    public string recipeName;
    public string[] inputImages;
    public string[] outputImages;
    public int ID;
    public float recipeRate;

    public Recipe(int recipe)
    {
        SetRecipe(recipe);
    }

    public void SetRecipe(int recipe)
    {
        inputIDs = DataManager.GetAllInput(recipe);
        outputIDs = DataManager.GetAllOutput(recipe);
        inputNumbers = DataManager.GetAllInputNumber(recipe);
        outputNumbers = DataManager.GetAllOutputNumber(recipe);
        inputNames = DataManager.GetAllInputName(recipe);
        outputNames = DataManager.GetAllOutputName(recipe);
        recipeName = DataManager.GetRecipeName(recipe);
        inputImages = DataManager.GetAllInputImage(recipe);
        outputImages = DataManager.GetAllOutputImage(recipe);
        ID = recipe;
        recipeRate = DataManager.GetRecipeDuration(recipe);
    }

    public int GetRecipe()
    {
        return ID;
    }
}