using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class RollerAgent : Agent {

    public Transform Target;

    Rigidbody rBody;

    void Start() {
        rBody = GetComponent<Rigidbody>();
    }

    public override void AgentReset() {
        
        if (this.transform.position.y < 0) {
            // The Agent fell
            this.transform.position = new Vector3(0, 0.5f, 0);
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
        }
        else {
            // Move the target to a new spot
            Target.position = new Vector3(Random.value * 8 - 4,
                                          0.5f,
                                          Random.value * 8 - 4);
        }
    }

    public override void CollectObservations() {

        // Target and Agent positions
        AddVectorObs(Target.position);
        AddVectorObs(this.transform.position);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);

    }

    public float speed = 10;
    private float previousDistance = float.MaxValue;


    public override void AgentAction(float[] vectorAction, string textAction) {

        previousDistance = Vector3.Distance(this.transform.position,
                                                  Target.position);
        // Actions, size = 2
        // => action[0]: x axis, action[1]: z axis
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position,
                                                  Target.position);
        
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            Done();
        }

        // Time penalty
        //AddReward(-0.0005f);

        // Fell off platform
        if (this.transform.position.y < 0)
        {
            AddReward(-1.0f);
            Done();
        }

        /*if (distanceToTarget < previousDistance){
            AddReward(0.5f);
        }*/
    }
}
