using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Die Good-Klasse beschreibt ein Gut, welches in Fabriken produziert werden kann. Momentan ist die Individualit�t eines Goods nicht sehr n�tzlich
public class Good
{
    int id;

    public Good(int id)
    {
        this.id = id;
    }
}
