using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public int selected = 0;

    public GameObject[] defaultBuildings = new GameObject[4];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        CheckSelection();
        if (Input.GetMouseButtonDown(0))
        {
            CheckRay();
        }
    }
    
    void CheckRay()
    {
        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100) || gameObject.GetComponent<UIManager>().uIopen)
            return;

        if (hit.collider.gameObject.name == "Ground" || selected == 0)
        {
            if (selected != 0) { 
                ConstructFactory(hit);
            } else
            {
                StartCoroutine(ConstructConveyor(hit));
            }
        }
    }

    void ConstructFactory(RaycastHit hit)
    {
        GameObject g = Instantiate(defaultBuildings[selected]);

        g.AddComponent<Factory>();
        g.AddComponent<BoxCollider>();

        g.tag = "factory";

        g.transform.position = Vector3.zero;

        g.transform.position = hit.point + new Vector3(0, 10, 0);
        g.transform.localScale = new Vector3(1, 1, 1);
        g.transform.Rotate(new Vector3(90, transform.rotation.eulerAngles.y, 0));

        g.GetComponent<Factory>().Init(selected);
    }

    void DrawConveyor(Conveyor c)
    {
        GameObject g = Instantiate(defaultBuildings[0]);

        g.transform.position = (c.inputPos + c.outputPos) / 2;
        g.transform.localScale = new Vector3(1, 1, c.length / 5) / 10;

        // Calculate rotation based on the direction from inputPos to outputPos
        Vector3 direction = c.outputPos - c.inputPos;
        Quaternion rotation = Quaternion.LookRotation(direction);

        // Apply the rotation to the GameObject
        g.transform.rotation = rotation;



        g.name = "Conveyor";
    }

    private IEnumerator ConstructConveyor(RaycastHit hit1)
    {
        while (true)
        {
            yield return null;
            yield return new WaitForNextMouseClick();
            RaycastHit hit2;
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit2, 100))
            {
                continue;
            }


            Conveyor c;
            if (hit1.collider.gameObject.CompareTag("factory"))
            {
                Factory f1 = hit1.collider.gameObject.GetComponent<Factory>();
                Vector3 v1 = f1.GetClosestAccess(hit1.point, out bool isOut);

                if (hit2.collider.gameObject.CompareTag("factory"))
                {
                    Factory f2 = hit2.collider.gameObject.GetComponent<Factory>();
                    if (isOut)
                    {
                        c = new Conveyor(/*speed*/ 10, v1, f2.GetClosestOutput(hit2.point), f2, f1);
                    }
                    else
                    {
                        c = new Conveyor(/*speed*/ 10, f2.GetClosesInput(hit2.point), v1, f1, f2);
                    }
                    DrawConveyor(c);
                    yield break;
                }

                if (isOut)
                {
                    c = new Conveyor(/*speed*/ 10, v1, hit2.point + new Vector3(0, 1, 0), f1, false); //Achtung: Möglicherweise wurden hier input und output vertauscht
                }
                else
                {
                    c = new Conveyor(/*speed*/ 10, hit2.point + new Vector3(0, 1, 0), v1, f1, true);
                }
                DrawConveyor(c);
                yield break;
            }
            else
            {
                if (hit2.collider.gameObject.CompareTag("factory"))
                {
                    Factory f2 = hit2.collider.gameObject.GetComponent<Factory>();
                    Vector3 v2 = f2.GetClosestAccess(hit2.point, out bool isOut);
                    if (isOut)
                    {
                        c = new Conveyor(/*speed*/ 10, v2, hit1.point + new Vector3(0, 1, 0), f2, false);
                    }
                    else
                    {
                        c = new Conveyor(/*speed*/ 10, hit1.point + new Vector3(0, 1, 0), v2, f2, true);
                    }
                    DrawConveyor(c);
                    yield break;
                }
                c = new Conveyor(/*speed*/ 10, hit1.point + new Vector3(0, 1, 0), hit2.point + new Vector3(0, 1, 0));
                DrawConveyor(c);
                yield break;
            }
        }
    }

    void CheckSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            selected = 0;
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selected = 1;
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selected = 2;
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selected = 3;
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selected = 4;
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selected = 5;
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selected = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            selected = 7;
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            selected = 8;
        } else
        if (Input.GetKeyDown(KeyCode.Alpha9))
            selected = 9;
    }
}
