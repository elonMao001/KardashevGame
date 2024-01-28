using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Die Good-Klasse muss zum jetzigen Zeitpunkt nichts können. Sie ist darauf konzipiert, dass Goods individuell sein können, anstatt nur eine eine Zahl
public class Good
{
    int id;

    public Good(int id)
    {
        this.id = id;
    }
}