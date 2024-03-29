using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgentwithWall : Agent
{
    public GameObject planerewardOutputColor;
    public Material winMat;
    public Material lossMaterial;

    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;
    public override void OnEpisodeBegin()
    {
        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, -2.5f);
        }
        float xRandomValue = Random.value * 8 - 4;
        float yRandomValue = Random.value * 8 - 4;

        // Move the target to a new spot
        Target.localPosition = new Vector3(xRandomValuefun(),
                                           0.5f,
                                           zRandomValuefun());
    }
    public float xRandomValuefun()
    {
        float xRandomValue = Random.value * 8 - 4;
        while (-2.5f < xRandomValue && xRandomValue < 2.5f)
        {
            xRandomValue = Random.value * 8 - 4;
        }
        return xRandomValue;
    }

    public float zRandomValuefun()
    {
        float zRandomValue = Random.value * 8 - 4;
        while (-0.5f < zRandomValue && zRandomValue < 0.5f)
        {
            zRandomValue = Random.value * 8 - 4;
        }
        return zRandomValue;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            // Get the Renderer component from the new cube
            var cubeRenderer = planerewardOutputColor.GetComponent<MeshRenderer>();

            // Call SetColor using the shader property name "_Color" and setting the color to red
            cubeRenderer.material = winMat;
            EndEpisode();
        }

        // Fell off platform
        else if (this.transform.localPosition.y < 0)
        {
            // Get the Renderer component from the new cube
            var cubeRenderer = planerewardOutputColor.GetComponent<MeshRenderer>();

            // Call SetColor using the shader property name "_Color" and setting the color to red
            cubeRenderer.material = lossMaterial;
            EndEpisode();
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
