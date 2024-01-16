using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speedMove = 10;
    public float speedRotate = 10;

    // Start is called before the first frame update
    void Start()
    {
        Test();
    }

    void Test() {
        Debug.Log(Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 trans = Vector3.zero;
        if (Input.GetKey("w")) {
            trans += Camera.main.transform.forward;
        }
        if (Input.GetKey("d"))
        {
            trans += Camera.main.transform.right;
        }
        if (Input.GetKey("a"))
        {
            trans -= Camera.main.transform.right;
        }
        if (Input.GetKey("s"))
        {
            trans -= Camera.main.transform.forward;
        }
        trans.y = 0;
        transform.position += trans * Time.deltaTime * speedMove;

        if (Input.mousePosition.x > Screen.width - 50) {
            transform.RotateAround(transform.position, transform.up, speedRotate * Time.deltaTime);
        } else if (Input.mousePosition.x < 50)
        {
            transform.RotateAround(transform.position, transform.up, -speedRotate * Time.deltaTime);
        }
        if (Input.mousePosition.y > Screen.height - 50) {
            transform.RotateAround(transform.position, Camera.main.transform.right, -speedRotate * Time.deltaTime / 2);
        } else if (Input.mousePosition.y < 50)
        {
            transform.RotateAround(transform.position, Camera.main.transform.right, speedRotate * Time.deltaTime / 2);
        }
    }
}