using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Die Good-Klasse muss zum jetzigen Zeitpunkt nichts k�nnen. Sie ist darauf konzipiert, dass Goods individuell sein k�nnen, anstatt nur eine eine Zahl
public class Good
{
    int id;

    public Good(int id)
    {
        this.id = id;
    }
}