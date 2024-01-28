using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Der ConveyorCheater umgeht das Problem, dass Conveyor gleichzeitig ein Monobehaviour als auch eine Klasse mit Konstruktoren sein muss
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

    //Erster Fall: Das Conveyor geht von einem Punkt auf dem Boden zu einem anderen Punkt auf dem Boden (momentan sinnlos)
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

    //Zweiter Fall: Das Conveyor geht von einem Punkt auf dem Boden zu einer Fabrik (momentan sinnlos)
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

    //Dritte Fall: Das Conveyor geht von einer Fabrik zu einer anderen Fabrik
    public Conveyor(float s, Vector3 start, Vector3 end, Factory input, Factory output)
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

    //Bewegt alle Güter, die in beiden Fabrikrezepten zu finden sind vom Output der einen zum Input der anderen
    void PullPush()
    {
        Recipe inputRec = input.GetRecipe();
        Recipe outputRec = output.GetRecipe();
        for (int i = 0; i < outputRec.outputIDs.Length; i++) {
            int good = outputRec.outputIDs[i];
            for (int j = 0; j < inputRec.inputIDs.Length; j++)
            {
                if (good == inputRec.inputIDs[j])
                {
                    if (output.outputGoodsFill[i] > 0 && input.inputGoodsFill[j] < input.FACTORYCAPACITY)
                    {
                        int temp = Mathf.Min(output.outputGoodsFill[i], input.FACTORYCAPACITY - input.inputGoodsFill[j]);
                        input.AddGoods(input.inputGoods[j], input.inputGoodsFill, j, temp, good); //Hier könnte man Conveyorspeed einbauen
                        output.SubtractGoods(output.outputGoods[i], output.outputGoodsFill, temp, 1); // *
                    }
                }
            }
        }
    }

    //Überprüft die Grundvoraussetzung um Güter zu übertragen
    bool CanPullPush()
    {
        if (input != null && output != null && input.GetRecipe() != null && output.GetRecipe() != null)
            return true;
        return false;
    }
}