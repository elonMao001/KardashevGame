
using System;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

using static UnityEngine.Mathf;

[ExecuteAlways]
public class ObserverScript : MonoBehaviour {
    [SerializeField]
    private Transform centerOfRotation, cam;

    private PlanetGenerator planetGenerator;

    private float alat, along, w; 
    private int latVel, longVel, zoomVel;

    [SerializeField]
    private bool gameMovement;

    [SerializeField, Range(minObservationDistance, maxObservationDistance)]
    private float observationDistance = 0.1f;
    private const float minObservationDistance = 0.0001f, maxObservationDistance = 30f;

    [SerializeField, Range(0f, 10f)]
    private float speed;
    [SerializeField]
    private float zoomSpeed; 
    [SerializeField, Min(1)]
    private float zoomIntensity;

    [SerializeField, Range(0, PI * 0.5f)]
    private float latAngleLimit = PI * 0.5f;
    [SerializeField, Range(0.00001f, 10f)]
    private float velocityDampeningFactor;


    private void Start() {
        planetGenerator = centerOfRotation.GetComponent<PlanetGenerator>();
    }

    private void Update() {
        CheckKeyBinds();

        if (gameMovement) {
            UpdatePosition();
            UpdateCamera();
        } else KeepInOrbit();
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
        if (Input.GetKeyDown(KeyCode.Space)) zoomVel++;
        else if (Input.GetKeyUp(KeyCode.Space)) zoomVel--;
        if (Input.GetKeyDown(KeyCode.LeftShift)) zoomVel--;
        else if (Input.GetKeyUp(KeyCode.LeftShift)) zoomVel++;
    }
    
    private void UpdatePosition() {
        float radius = GetRadius();
        float latDiff = Abs(latAngleLimit * latVel - alat); 

        float aspeed = speed / centerOfRotation.localScale.x;

        if (gameMovement) {
            if (zoomVel != 0) {
                observationDistance -= zoomVel * GetZoomVelocity(radius - planetGenerator.GetRadius()) * Time.deltaTime;    
                observationDistance = Clamp(observationDistance, minObservationDistance, maxObservationDistance);
            }
        }

        alat += latVel * aspeed * DampenVelocity(latDiff) * Time.deltaTime;
        alat = Min(alat,  latAngleLimit);
        alat = Max(alat, -latAngleLimit);
        along += longVel * aspeed * Time.deltaTime;

        w = Cos(alat) * radius;
        transform.position = centerOfRotation.position + 
                             new Vector3(Cos(along) * w, 
                                         Sin(alat) * radius, 
                                         Sin(along) * w);
    }

    private float GetZoomVelocity(float dist) {
        dist = (float)(dist - minObservationDistance) / (maxObservationDistance - minObservationDistance);
        return Pow(dist, zoomIntensity) * zoomSpeed;
    }

    private float GetRadius() {
        return planetGenerator.GetRadius() + observationDistance;
    }

    private void KeepInOrbit() {
        transform.position = Vector3.Normalize(transform.position) * (planetGenerator.GetRadius() + observationDistance);

        cam.LookAt(centerOfRotation);
    }

    private void UpdateCamera() {
        cam.eulerAngles = new Vector3(alat * Rad2Deg, 270 - along * Rad2Deg);
    }

    private float DampenVelocity(float diff) {
        return 1 - Exp(-velocityDampeningFactor * PI * diff);
    }
}
