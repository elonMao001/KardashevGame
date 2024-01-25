using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Da man nur MonoBehaviours an Szenenobjekte anhängen kann, Monobehaviours aber keine Konstruktoren haben dürfen, existieren zwei Klassen für die Conveyor-Funktion
public class ConveyorCheater : MonoBehaviour
{
    public Conveyor conveyor = null;

    private void Update()
    {
        if (conveyor != null)
            conveyor.Update();
    }
}

public class Conveyor
{
    public float length;
    Factory input = null;
    Factory output = null;
    float speed;
    public Vector3 inputPos;
    public Vector3 outputPos;
    Vector3[] segments; //obsolet
    Good[] contents;

    //1. Fall: Das Conveyor berührt keine Fabriken, sondern nur Boden. Momentan sinnlos
    public Conveyor(float s, Vector3 start, Vector3 end) {
        speed = s;
        length = Vector3.Distance(start, end);
        contents = new Good[(int)length];
        segments = new Vector3[(int) length];

        inputPos = start;
        outputPos = end;

        Vector3 sToE = end - start;
        for (int i = 0; i < length; i++) {
            segments[0] = start + i * sToE;
        }
    }

    //2. Fall: Man berührt auf einer Seite eine Fabrik, auf der anderen nur Boden. Momentan sinnlos
    public Conveyor(float s, Vector3 start, Vector3 end, Factory fac, bool input)
    {
        speed = s;
        length = Vector3.Distance(start, end);
        contents = new Good[(int)length];
        segments = new Vector3[(int)length];

        inputPos = start;
        outputPos = end;

        Vector3 sToE = end - start;
        for (int i = 0; i < length; i++)
        {
            segments[0] = start + i * sToE;
        }

        if (input)
        {
            this.input = fac;
        }
        else {
            this.output = fac;
        }
    }

    //3. Fall: Das Conveyor geht von einer Fabrik zu einer anderem. Momentan sinnvoll
    public Conveyor(float s, Vector3 start, Vector3 end, Factory input, Factory output)
    {
        speed = s;
        Debug.Log(start);
        Debug.Log(end);
        Debug.Log(Vector3.Distance(start, end));
        length = Vector3.Distance(start, end);
        contents = new Good[(int)length];
        segments = new Vector3[(int)length];

        inputPos = start;
        outputPos = end;

        /*Vector3 sToE = end - start;
        for (int i = 0; i < length; i++)
        {
            segments[0] = start + i * sToE;
        }*/

        this.input = input;
        this.output = output;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        if(CanPullPush())
        {
            PullPush();
        }
    }

    //Das Conveyor prüft, ob es Goods gibt, die es von der Fabrik output zur Fabrik input transportieren kann und macht es dann
    void PullPush()
    {
        Recipe inputRec = input.GetRecipe();
        Recipe outputRec = output.GetRecipe();
        for (int i = 0; i < inputRec.inputIDs.Length; i++) {
            int good = inputRec.inputIDs[i];
            for (int j = 0; j < outputRec.outputIDs.Length; j++)
            {
                if (good == outputRec.outputIDs[j])
                {

                    if (output.outputGoodsFill[j] > 0 && input.inputGoodsFill[i] < input.FACTORYCAPACITY)
                    {
                        input.AddGoods(input.inputGoods[i], input.inputGoodsFill, i, Mathf.Min(output.outputGoodsFill[j], input.FACTORYCAPACITY- input.inputGoodsFill[i]), good);
                        output.SubtractGoods(output.outputGoods[j], output.outputGoodsFill, j, Mathf.Min(output.outputGoodsFill[j], input.FACTORYCAPACITY - input.inputGoodsFill[i]));
                    }
                }
            }
        }
    }

    //Überprüft, ob die simplesten Bedingungen für PullPush() erfüllt worden sind
    bool CanPullPush()
    {
        if (input != null && output != null && input.GetRecipe() != null && output.GetRecipe() != null)
            return true;
        return false;
    }
}