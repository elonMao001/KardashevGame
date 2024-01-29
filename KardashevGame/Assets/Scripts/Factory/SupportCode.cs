using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Diese Klasse wird benötigt um eine Coroutine nur dann weiterlaufen zu lassen, wenn es zu einem weiteren Mausklick kommt
public class WaitForNextMouseClick : CustomYieldInstruction
{
    public override bool keepWaiting
    {
        get
        {
            return !Input.GetMouseButtonDown(0);
        }
    }

    public WaitForNextMouseClick(){}
}

//Die Support-Klasse bietet ein paar allgemeine Methoden, die sich momentan alle mit Arrays beschäftigen
//Ansonsten kommen hier zum Beispiel komplexe und öfter benötigte mathematische Terme und Co. rein
public static class Support
{
    //Macht aus dem int[] array ein kleineres int[], in dem alle Werte nullInt (oft -1) entfernt wurden
    public static int[] ResizeArray(int[] array, int nullInt)
    {
        int size = ArrayFill(array, nullInt);
        if (size == array.Length)
            return array;
        if (size == 0)
            return new int[0];
        int[] ret = new int[size];
        int index = 0;
        for(int i = 0; i < array.Length; i++)
        {
            if(array[i] != nullInt)
            {
                ret[index] = array[i];
                index++;
            }
        }
        return ret;
    }

    //Überprüft, wieviele Werte von array nicht nullInt entsprechen
    public static int ArrayFill(int[] array, int nullInt)
    {
        int size = 0;
        foreach (int i in array)
        {
            if (i != nullInt)
                size++;
        }
        return size;
    }

    //Macht aus dem Good[] array ein kleineres Good[], in dem alle Werte nullGood (oft null) entfernt wurden
    public static Good[] ResizeArray(Good[] array, Good nullGood)
    {
        int size = ArrayFill(array, nullGood);
        if (size == array.Length)
            return array;
        if (size == 0)
            return new Good[0];
        Good[] ret = new Good[size];
        int index = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != nullGood)
            {
                ret[index] = array[i];
                index++;
            }
        }
        return ret;
    }

    //Überprüft, wieviele Werte von array nicht nullGood entsprechen
    public static int ArrayFill(Good[] array, Good nullGood)
    {
        int size = 0;
        foreach (Good g in array)
        {
            if (g != nullGood)
                size++;
        }
        return size;
    }

    //Überprüft, ob der char test in array vorkommt
    public static bool ArrayContains(char[] array, char test)
    {
        foreach (char c in array)
        {
            if (c == test)
                return true;
        }
        return false;
    }
}