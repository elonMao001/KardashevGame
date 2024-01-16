
using System;
using Unity.VisualScripting;
using UnityEngine;

using static UnityEngine.Mathf;

[ExecuteAlways]
public class ObserverScript : MonoBehaviour {
    [SerializeField]
    private Transform centerOfRotation, cam;

    private PlanetGenerator planetGenerator;

    private float alat, along, latVel, longVel;

    [SerializeField, Range(0f, 2f)]
    private float observationRadius = 0.1f;

    [SerializeField, Range(0f, 10f)]
    private float speed;

    [SerializeField, Range(0, PI * 0.5f)]
    private float latAngleLimit = PI * 0.5f;
    [SerializeField, Range(0.00001f, 10f)]
    private float velocityDampeningFactor;

    private void Start() {
        planetGenerator = centerOfRotation.GetComponent<PlanetGenerator>();
    }

    private void Update() {
        CheckKeyBinds();
        //UpdatePosition();
        UpdateCamera();

        KeepInOrbit();
    }

    private void OnValidate() {
        latVel = longVel = 0;
    }
    
    private void CheckKeyBinds() {
        if (Input.GetKeyDown(KeyCode.W)) latVel++;
        else if (Input.GetKeyUp(KeyCode.W)) latVel--;
        if (Input.GetKeyDown(KeyCode.S)) latVel--;
        else if (Input.GetKeyUp(KeyCode.S)) latVel++;
        if (Input.GetKeyDown(KeyCode.A)) longVel--;
        else if (Input.GetKeyUp(KeyCode.A)) longVel++;
        if (Input.GetKeyDown(KeyCode.D)) longVel++;
        else if (Input.GetKeyUp(KeyCode.D)) longVel--;
    }
    
    private void UpdatePosition() {
        float radius = GetRadius();
        float latDiff = Abs(latAngleLimit * latVel - alat); 

        float aspeed = speed / centerOfRotation.localScale.x;

        alat += latVel * aspeed * DampenVelocity(latDiff) * Time.deltaTime;
        alat = Min(alat,  latAngleLimit);
        alat = Max(alat, -latAngleLimit);
        along += longVel * aspeed * Time.deltaTime;

        float w = Cos(alat) * radius;
        transform.position = centerOfRotation.position + 
                             new Vector3(Cos(along) * w, 
                                         Sin(alat) * radius, 
                                         Sin(along) * w);
    }

    private float GetRadius() {
        return (1 + observationRadius) * planetGenerator.GetRadius();
    }

    private void KeepInOrbit() {
        transform.position = Vector3.Normalize(transform.position) * planetGenerator.GetRadius();
    }

    private void UpdateCamera() {
        cam.eulerAngles = new Vector3(alat * Rad2Deg, 270 - along * Rad2Deg);
    }

    private float DampenVelocity(float diff) {
        return 1 - Exp(-velocityDampeningFactor * PI * diff);
    }
}
