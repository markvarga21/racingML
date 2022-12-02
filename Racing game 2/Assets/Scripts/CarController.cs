using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class CarController : Agent
{
    [SerializeField]
    private float speed = 5f;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 0, 0);

    }


    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.ContinuousActions;

        float actionSpeed = (actionTaken[0] + 1) / 2;
        float actionSteering = actionTaken[1]; // [-1, +1]

        transform.Translate(actionSpeed * Vector3.forward * speed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Euler(new Vector3(0, actionSteering * 180, 0));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actions = actionsOut.ContinuousActions;

        actions[0] = -1;
        actions[1] = 0;

        if (Input.GetKey("w"))
            actions[0] = 1;

        if (Input.GetKey("d"))
            actions[1] = +0.5f;

        if (Input.GetKey("a"))
            actions[1] = -0.5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            AddReward(-1);
            ClearTags();
            EndEpisode();
        }
        if (other.gameObject.tag == "Finish")
        {
            AddReward(+1);
            ClearTags();
            EndEpisode();
        }
        if (other.gameObject.tag == "UnreachedCheckpoint")
        {
            AddReward(0.1f);
            other.gameObject.tag = "Checkpoint";
        }
    }

    private void ClearTags() {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Checkpoint");
        for (int i = 0; i < gameObjects.Length; i++) {
            gameObjects[i].tag = "UnreachedCheckpoint";
        }
    }
}
