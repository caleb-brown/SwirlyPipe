using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public PipeSystem pipeSystem;
    public float velocity = 1;

    Transform world;
    Pipe currentPipe;
    float distanceTraveled, deltaToRotation, systemRotation;

    void Start()
    {
        world = pipeSystem.transform.parent;
        currentPipe = pipeSystem.SetupFirstPipe();
        deltaToRotation = 360.0f / (2.0f * Mathf.PI * currentPipe.CurveRadius);
    }

    void Update()
    {
        float delta = velocity * Time.deltaTime;
        distanceTraveled += delta;
        systemRotation += delta * deltaToRotation;

        if (systemRotation >= currentPipe.CurveAngle)
        {
            delta = (systemRotation - currentPipe.CurveAngle) / deltaToRotation;
            currentPipe = pipeSystem.SetupNextPipe();
            deltaToRotation = 360.0f / (2.0f * Mathf.PI * currentPipe.CurveRadius);
        }

        pipeSystem.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, systemRotation);
    }
}
