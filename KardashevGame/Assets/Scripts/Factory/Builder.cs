using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class Builder : MonoBehaviour
{
    static public char selected = 'i';
    static public char[] existingKeys = { 'i', 'f', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };


    public GameObject[] defaultBuildings = new GameObject[4];
    public GameObject inventory;

    bool waiting;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckRay();
        }
    }

    //Führt einen Raycast aus um nach einem geeigneten Ort für das ausgewählte Gebäude zu suchen oder ein bereits vorhandenes abzureißen
    void CheckRay()
    {
        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100) || gameObject.GetComponent<UIManager>().uIopen || selected == 'i')
            return;

        if (hit.collider.gameObject.CompareTag("Ground") || selected == '0')
        {
            if (selected != '0') {
                ConstructFactory(hit);
            } else if(!waiting)
            {
                StartCoroutine(ConstructConveyor(hit));
            }
            return;
        }
        if ((hit.collider.gameObject.CompareTag("Factory") || hit.collider.gameObject.CompareTag("Conveyor")) && selected == 'f') {
            RemoveBuilding(hit.collider.gameObject);
        }
    }

    //Erstellt aus einem der 3d-Modelle eine neue Fabrik und platziert diese am Ort des Raycasthits
    void ConstructFactory(RaycastHit hit)
    {
        GameObject g = Instantiate(defaultBuildings[selected - '0']);

        g.AddComponent<Factory>();
        g.AddComponent<BoxCollider>();

        g.tag = "Factory";

        g.transform.position = hit.point + new Vector3(0, 0, 0);
        g.transform.localScale = new Vector3(1, 1, 1) / 3;
        g.transform.Rotate(new Vector3(90, transform.rotation.eulerAngles.y, 0));

        g.GetComponent<Factory>().Init(selected - '0');
    }

    //Wandelt das berechnete Conveyor in ein neues Szenenobjekt um
    void DrawConveyor(Conveyor c)
    {
        GameObject g = Instantiate(defaultBuildings[0]);
        g.AddComponent<ConveyorCheater>().conveyor = c;

        g.transform.position = (c.inputPos + c.outputPos) / 2;
        Debug.Log(g.transform.position);
        g.transform.localScale = new Vector3(1, 1, c.length / 5 * 10) / 10;

        // Calculate rotation based on the direction from inputPos to outputPos
        Vector3 direction = c.outputPos - c.inputPos;
        Quaternion rotation = Quaternion.LookRotation(direction);

        g.transform.rotation = rotation;


        g.name = "Conveyor";
        g.tag = "Conveyor";
    }

    //Wartet auf zwei Maus-Klicks um dann ein neues Conveyor zwischen den Punkten und oder Fabriken zu erstellen
    private IEnumerator ConstructConveyor(RaycastHit hit1)
    {
        while (true)
        {
            yield return null;
            waiting = true;
            yield return new WaitForNextMouseClick();
            waiting = false;


            RaycastHit hit2;
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit2, 100))
            {
                continue;
            }


            Conveyor c;
            if (hit1.collider.gameObject.CompareTag("Factory"))
            {
                Factory f1 = hit1.collider.gameObject.GetComponent<Factory>();
                Vector3 v1 = f1.GetClosestAccess(hit1.point, out bool isOut);
                Debug.Log(v1);

                if (hit2.collider.gameObject.CompareTag("Factory"))
                {
                    Factory f2 = hit2.collider.gameObject.GetComponent<Factory>();
                    if (isOut)
                    {
                        Debug.Log(f2.GetClosestOutput(hit2.point));
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
                if (hit2.collider.gameObject.CompareTag("Factory"))
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

    //Zerstört eine angeklickte Fabrik
    void RemoveBuilding(GameObject building)
    {
        Destroy(building);
        gameObject.GetComponent<UIManager>().uIopen = false;
    }

    //Wird aufgerufe, wenn Unity irgendein GUI-Event erfährt
    private void OnGUI()
    {
        CheckSelection();
    }

    //Überprüft ob man das erhaltene Event zu einem der selected-Werte umwandeln kann
    //Der lange Anhang war ein freundlich gemeinter Autofill-Vorfall (Er wird in unserer Zeilenzahl nicht berücksichtigt (Es sind 717))
    void CheckSelection()  // highly inefficient upon expansion
    {
        Event e = Event.current;
        if (e.type != EventType.KeyDown || e.character == selected || !Support.ArrayContains(existingKeys, e.character))
            return;
        if (e.character == 'i')
        {
            inventory.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Vector4(200, 100, 100, 255);
            if(selected == 'f')
            {
                inventory.transform.GetChild(0).GetChild(13).GetComponent<Image>().color = new Vector4(255, 255, 255, 255);
            }
            else
            {
                inventory.transform.GetChild(0).GetChild(selected - '0' + 1).GetComponent<Image>().color = new Vector4(200, 100, 100, 255);
            }
        } else if(e.character == 'f')
        {
            inventory.transform.GetChild(0).GetChild(13).GetComponent<Image>().color = new Vector4(200, 100, 100, 255);
            if (selected == 'i')
            {
                inventory.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Vector4(255, 255, 255, 255);
            }
            else
            {
                inventory.transform.GetChild(0).GetChild(selected - '0' + 1).GetComponent<Image>().color = new Vector4(200, 100, 100, 255);
            }
        }
        else
        {
            if (selected == 'i' || selected == 'f')
            {
                inventory.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Vector4(255, 255, 255, 255);
                inventory.transform.GetChild(0).GetChild(13).GetComponent<Image>().color = new Vector4(255, 255, 255, 255);
            }
            else
            {
                inventory.transform.GetChild(0).GetChild(selected - '0' + 1).GetComponent<Image>().color = new Vector4(200, 100, 100, 255);
            }
        }

        selected = e.character;
        Debug.Log(selected);
        return;
        switch (e.keyCode)
        {
            case KeyCode.None:
                break;
            /*case KeyCode.Backspace:
                break;
            case KeyCode.Delete:
                break;
            case KeyCode.Tab:
                break;
            case KeyCode.Clear:
                break;
            case KeyCode.Return:
                break;
            case KeyCode.Pause:
                break;
            case KeyCode.Escape:
                break;
            case KeyCode.Space:
                break;
            case KeyCode.Keypad0:
                break;
            case KeyCode.Keypad1:
                break;
            case KeyCode.Keypad2:
                break;
            case KeyCode.Keypad3:
                break;
            case KeyCode.Keypad4:
                break;
            case KeyCode.Keypad5:
                break;
            case KeyCode.Keypad6:
                break;
            case KeyCode.Keypad7:
                break;
            case KeyCode.Keypad8:
                break;
            case KeyCode.Keypad9:
                break;
            case KeyCode.KeypadPeriod:
                break;
            case KeyCode.KeypadDivide:
                break;
            case KeyCode.KeypadMultiply:
                break;
            case KeyCode.KeypadMinus:
                break;
            case KeyCode.KeypadPlus:
                break;
            case KeyCode.KeypadEnter:
                break;
            case KeyCode.KeypadEquals:
                break;
            case KeyCode.UpArrow:
                break;
            case KeyCode.DownArrow:
                break;
            case KeyCode.RightArrow:
                break;
            case KeyCode.LeftArrow:
                break;
            case KeyCode.Insert:
                break;
            case KeyCode.Home:
                break;
            case KeyCode.End:
                break;
            case KeyCode.PageUp:
                break;
            case KeyCode.PageDown:
                break;
            case KeyCode.F1:
                break;
            case KeyCode.F2:
                break;
            case KeyCode.F3:
                break;
            case KeyCode.F4:
                break;
            case KeyCode.F5:
                break;
            case KeyCode.F6:
                break;
            case KeyCode.F7:
                break;
            case KeyCode.F8:
                break;
            case KeyCode.F9:
                break;
            case KeyCode.F10:
                break;
            case KeyCode.F11:
                break;
            case KeyCode.F12:
                break;
            case KeyCode.F13:
                break;
            case KeyCode.F14:
                break;
            case KeyCode.F15:
                break;*/
            case KeyCode.Alpha0:
                selected = '0';
                break;
            case KeyCode.Alpha1:
                selected = '1';
                break;
            case KeyCode.Alpha2:
                selected = '2';
                break;
            case KeyCode.Alpha3:
                selected = '3';
                break;
            case KeyCode.Alpha4:
                selected = '4';
                break;
            case KeyCode.Alpha5:
                selected = '5';
                break;
            case KeyCode.Alpha6:
                selected = '6';
                break;
            case KeyCode.Alpha7:
                selected = '7';
                break;
            case KeyCode.Alpha8:
                selected = '8';
                break;
            case KeyCode.Alpha9:
                selected = '9';
                break;
            /*case KeyCode.Exclaim:
                break;
            case KeyCode.DoubleQuote:
                break;
            case KeyCode.Hash:
                break;
            case KeyCode.Dollar:
                break;
            case KeyCode.Percent:
                break;
            case KeyCode.Ampersand:
                break;
            case KeyCode.Quote:
                break;
            case KeyCode.LeftParen:
                break;
            case KeyCode.RightParen:
                break;
            case KeyCode.Asterisk:
                break;
            case KeyCode.Plus:
                break;
            case KeyCode.Comma:
                break;
            case KeyCode.Minus:
                break;
            case KeyCode.Period:
                break;
            case KeyCode.Slash:
                break;
            case KeyCode.Colon:
                break;
            case KeyCode.Semicolon:
                break;
            case KeyCode.Less:
                break;
            case KeyCode.Equals:
                break;
            case KeyCode.Greater:
                break;
            case KeyCode.Question:
                break;
            case KeyCode.At:
                break;
            case KeyCode.LeftBracket:
                break;
            case KeyCode.Backslash:
                break;
            case KeyCode.RightBracket:
                break;
            case KeyCode.Caret:
                break;
            case KeyCode.Underscore:
                break;
            case KeyCode.BackQuote:
                break;*/
            case KeyCode.A:
                break;
            case KeyCode.B:
                break;
            case KeyCode.C:
                break;
            case KeyCode.D:
                break;
            case KeyCode.E:
                break;
            case KeyCode.F:
                break;
            case KeyCode.G:
                break;
            case KeyCode.H:
                break;
            case KeyCode.I:
                selected = 'I';
                break;
            case KeyCode.J:
                break;
            case KeyCode.K:
                break;
            case KeyCode.L:
                break;
            case KeyCode.M:
                break;
            case KeyCode.N:
                break;
            case KeyCode.O:
                break;
            case KeyCode.P:
                break;
            case KeyCode.Q:
                break;
            case KeyCode.R:
                break;
            case KeyCode.S:
                break;
            case KeyCode.T:
                break;
            case KeyCode.U:
                break;
            case KeyCode.V:
                break;
            case KeyCode.W:
                break;
            case KeyCode.X:
                break;
            case KeyCode.Y:
                break;
            case KeyCode.Z:
                break;
            /*case KeyCode.LeftCurlyBracket:
                break;
            case KeyCode.Pipe:
                break;
            case KeyCode.RightCurlyBracket:
                break;
            case KeyCode.Tilde:
                break;
            case KeyCode.Numlock:
                break;
            case KeyCode.CapsLock:
                break;
            case KeyCode.ScrollLock:
                break;
            case KeyCode.RightShift:
                break;
            case KeyCode.LeftShift:
                break;
            case KeyCode.RightControl:
                break;
            case KeyCode.LeftControl:
                break;
            case KeyCode.RightAlt:
                break;
            case KeyCode.LeftAlt:
                break;
            case KeyCode.LeftMeta:
                break;
            case KeyCode.LeftCommand:
                break;
            case KeyCode.LeftApple:
                break;
            case KeyCode.LeftWindows:
                break;
            case KeyCode.RightMeta:
                break;
            case KeyCode.RightCommand:
                break;
            case KeyCode.RightApple:
                break;
            case KeyCode.RightWindows:
                break;
            case KeyCode.AltGr:
                break;
            case KeyCode.Help:
                break;
            case KeyCode.Print:
                break;
            case KeyCode.SysReq:
                break;
            case KeyCode.Break:
                break;
            case KeyCode.Menu:
                break;
            case KeyCode.Mouse0:
                break;
            case KeyCode.Mouse1:
                break;
            case KeyCode.Mouse2:
                break;
            case KeyCode.Mouse3:
                break;
            case KeyCode.Mouse4:
                break;
            case KeyCode.Mouse5:
                break;
            case KeyCode.Mouse6:
                break;
            case KeyCode.JoystickButton0:
                break;
            case KeyCode.JoystickButton1:
                break;
            case KeyCode.JoystickButton2:
                break;
            case KeyCode.JoystickButton3:
                break;
            case KeyCode.JoystickButton4:
                break;
            case KeyCode.JoystickButton5:
                break;
            case KeyCode.JoystickButton6:
                break;
            case KeyCode.JoystickButton7:
                break;
            case KeyCode.JoystickButton8:
                break;
            case KeyCode.JoystickButton9:
                break;
            case KeyCode.JoystickButton10:
                break;
            case KeyCode.JoystickButton11:
                break;
            case KeyCode.JoystickButton12:
                break;
            case KeyCode.JoystickButton13:
                break;
            case KeyCode.JoystickButton14:
                break;
            case KeyCode.JoystickButton15:
                break;
            case KeyCode.JoystickButton16:
                break;
            case KeyCode.JoystickButton17:
                break;
            case KeyCode.JoystickButton18:
                break;
            case KeyCode.JoystickButton19:
                break;
            case KeyCode.Joystick1Button0:
                break;
            case KeyCode.Joystick1Button1:
                break;
            case KeyCode.Joystick1Button2:
                break;
            case KeyCode.Joystick1Button3:
                break;
            case KeyCode.Joystick1Button4:
                break;
            case KeyCode.Joystick1Button5:
                break;
            case KeyCode.Joystick1Button6:
                break;
            case KeyCode.Joystick1Button7:
                break;
            case KeyCode.Joystick1Button8:
                break;
            case KeyCode.Joystick1Button9:
                break;
            case KeyCode.Joystick1Button10:
                break;
            case KeyCode.Joystick1Button11:
                break;
            case KeyCode.Joystick1Button12:
                break;
            case KeyCode.Joystick1Button13:
                break;
            case KeyCode.Joystick1Button14:
                break;
            case KeyCode.Joystick1Button15:
                break;
            case KeyCode.Joystick1Button16:
                break;
            case KeyCode.Joystick1Button17:
                break;
            case KeyCode.Joystick1Button18:
                break;
            case KeyCode.Joystick1Button19:
                break;
            case KeyCode.Joystick2Button0:
                break;
            case KeyCode.Joystick2Button1:
                break;
            case KeyCode.Joystick2Button2:
                break;
            case KeyCode.Joystick2Button3:
                break;
            case KeyCode.Joystick2Button4:
                break;
            case KeyCode.Joystick2Button5:
                break;
            case KeyCode.Joystick2Button6:
                break;
            case KeyCode.Joystick2Button7:
                break;
            case KeyCode.Joystick2Button8:
                break;
            case KeyCode.Joystick2Button9:
                break;
            case KeyCode.Joystick2Button10:
                break;
            case KeyCode.Joystick2Button11:
                break;
            case KeyCode.Joystick2Button12:
                break;
            case KeyCode.Joystick2Button13:
                break;
            case KeyCode.Joystick2Button14:
                break;
            case KeyCode.Joystick2Button15:
                break;
            case KeyCode.Joystick2Button16:
                break;
            case KeyCode.Joystick2Button17:
                break;
            case KeyCode.Joystick2Button18:
                break;
            case KeyCode.Joystick2Button19:
                break;
            case KeyCode.Joystick3Button0:
                break;
            case KeyCode.Joystick3Button1:
                break;
            case KeyCode.Joystick3Button2:
                break;
            case KeyCode.Joystick3Button3:
                break;
            case KeyCode.Joystick3Button4:
                break;
            case KeyCode.Joystick3Button5:
                break;
            case KeyCode.Joystick3Button6:
                break;
            case KeyCode.Joystick3Button7:
                break;
            case KeyCode.Joystick3Button8:
                break;
            case KeyCode.Joystick3Button9:
                break;
            case KeyCode.Joystick3Button10:
                break;
            case KeyCode.Joystick3Button11:
                break;
            case KeyCode.Joystick3Button12:
                break;
            case KeyCode.Joystick3Button13:
                break;
            case KeyCode.Joystick3Button14:
                break;
            case KeyCode.Joystick3Button15:
                break;
            case KeyCode.Joystick3Button16:
                break;
            case KeyCode.Joystick3Button17:
                break;
            case KeyCode.Joystick3Button18:
                break;
            case KeyCode.Joystick3Button19:
                break;
            case KeyCode.Joystick4Button0:
                break;
            case KeyCode.Joystick4Button1:
                break;
            case KeyCode.Joystick4Button2:
                break;
            case KeyCode.Joystick4Button3:
                break;
            case KeyCode.Joystick4Button4:
                break;
            case KeyCode.Joystick4Button5:
                break;
            case KeyCode.Joystick4Button6:
                break;
            case KeyCode.Joystick4Button7:
                break;
            case KeyCode.Joystick4Button8:
                break;
            case KeyCode.Joystick4Button9:
                break;
            case KeyCode.Joystick4Button10:
                break;
            case KeyCode.Joystick4Button11:
                break;
            case KeyCode.Joystick4Button12:
                break;
            case KeyCode.Joystick4Button13:
                break;
            case KeyCode.Joystick4Button14:
                break;
            case KeyCode.Joystick4Button15:
                break;
            case KeyCode.Joystick4Button16:
                break;
            case KeyCode.Joystick4Button17:
                break;
            case KeyCode.Joystick4Button18:
                break;
            case KeyCode.Joystick4Button19:
                break;
            case KeyCode.Joystick5Button0:
                break;
            case KeyCode.Joystick5Button1:
                break;
            case KeyCode.Joystick5Button2:
                break;
            case KeyCode.Joystick5Button3:
                break;
            case KeyCode.Joystick5Button4:
                break;
            case KeyCode.Joystick5Button5:
                break;
            case KeyCode.Joystick5Button6:
                break;
            case KeyCode.Joystick5Button7:
                break;
            case KeyCode.Joystick5Button8:
                break;
            case KeyCode.Joystick5Button9:
                break;
            case KeyCode.Joystick5Button10:
                break;
            case KeyCode.Joystick5Button11:
                break;
            case KeyCode.Joystick5Button12:
                break;
            case KeyCode.Joystick5Button13:
                break;
            case KeyCode.Joystick5Button14:
                break;
            case KeyCode.Joystick5Button15:
                break;
            case KeyCode.Joystick5Button16:
                break;
            case KeyCode.Joystick5Button17:
                break;
            case KeyCode.Joystick5Button18:
                break;
            case KeyCode.Joystick5Button19:
                break;
            case KeyCode.Joystick6Button0:
                break;
            case KeyCode.Joystick6Button1:
                break;
            case KeyCode.Joystick6Button2:
                break;
            case KeyCode.Joystick6Button3:
                break;
            case KeyCode.Joystick6Button4:
                break;
            case KeyCode.Joystick6Button5:
                break;
            case KeyCode.Joystick6Button6:
                break;
            case KeyCode.Joystick6Button7:
                break;
            case KeyCode.Joystick6Button8:
                break;
            case KeyCode.Joystick6Button9:
                break;
            case KeyCode.Joystick6Button10:
                break;
            case KeyCode.Joystick6Button11:
                break;
            case KeyCode.Joystick6Button12:
                break;
            case KeyCode.Joystick6Button13:
                break;
            case KeyCode.Joystick6Button14:
                break;
            case KeyCode.Joystick6Button15:
                break;
            case KeyCode.Joystick6Button16:
                break;
            case KeyCode.Joystick6Button17:
                break;
            case KeyCode.Joystick6Button18:
                break;
            case KeyCode.Joystick6Button19:
                break;
            case KeyCode.Joystick7Button0:
                break;
            case KeyCode.Joystick7Button1:
                break;
            case KeyCode.Joystick7Button2:
                break;
            case KeyCode.Joystick7Button3:
                break;
            case KeyCode.Joystick7Button4:
                break;
            case KeyCode.Joystick7Button5:
                break;
            case KeyCode.Joystick7Button6:
                break;
            case KeyCode.Joystick7Button7:
                break;
            case KeyCode.Joystick7Button8:
                break;
            case KeyCode.Joystick7Button9:
                break;
            case KeyCode.Joystick7Button10:
                break;
            case KeyCode.Joystick7Button11:
                break;
            case KeyCode.Joystick7Button12:
                break;
            case KeyCode.Joystick7Button13:
                break;
            case KeyCode.Joystick7Button14:
                break;
            case KeyCode.Joystick7Button15:
                break;
            case KeyCode.Joystick7Button16:
                break;
            case KeyCode.Joystick7Button17:
                break;
            case KeyCode.Joystick7Button18:
                break;
            case KeyCode.Joystick7Button19:
                break;
            case KeyCode.Joystick8Button0:
                break;
            case KeyCode.Joystick8Button1:
                break;
            case KeyCode.Joystick8Button2:
                break;
            case KeyCode.Joystick8Button3:
                break;
            case KeyCode.Joystick8Button4:
                break;
            case KeyCode.Joystick8Button5:
                break;
            case KeyCode.Joystick8Button6:
                break;
            case KeyCode.Joystick8Button7:
                break;
            case KeyCode.Joystick8Button8:
                break;
            case KeyCode.Joystick8Button9:
                break;
            case KeyCode.Joystick8Button10:
                break;
            case KeyCode.Joystick8Button11:
                break;
            case KeyCode.Joystick8Button12:
                break;
            case KeyCode.Joystick8Button13:
                break;
            case KeyCode.Joystick8Button14:
                break;
            case KeyCode.Joystick8Button15:
                break;
            case KeyCode.Joystick8Button16:
                break;
            case KeyCode.Joystick8Button17:
                break;
            case KeyCode.Joystick8Button18:
                break;
            case KeyCode.Joystick8Button19:
                break;*/
            default:
                break;
        }

        /*if (Input.GetKeyDown(KeyCode.Alpha0))
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
            selected = 9;*/
    }
}
