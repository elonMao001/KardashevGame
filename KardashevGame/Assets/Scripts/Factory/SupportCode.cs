using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Diese CustomYieldInstruction wird benötigt, da das Erstellen neuer Conveyors zwei Maus-Klicks benötigt
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

public static class Support
{
    //Aus einem Array wird ein neues erstellt, das keine nullInt-Werte (oft -1 oder 0) enthält. Benutzt um NullpointerExceptions zu vermeiden
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

    //Sagt wie viele Werte eines arrays nicht dem nullInt-Wert entsprechen
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

    //Überprüft ob einer der Werte aus array dem Wert test entspricht
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