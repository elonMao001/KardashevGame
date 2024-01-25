using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Die Recipe-Klasse speichert Rezepte, damit man die darin enthaltenen Informationen nicht dauerhaft aus Text-Files herauslesen muss
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
