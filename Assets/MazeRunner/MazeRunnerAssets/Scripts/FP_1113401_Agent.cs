using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class FP_1113401_Agent : Agent
{
    public float moveSpeed;
    public GameObject[] SpawnPointsarr;
    public GameObject Ground;
    public GameObject goal;
    private Bounds bndFloor;


    Rigidbody agent;


    public override void Initialize()
    {
        agent = GetComponent<Rigidbody>();
        bndFloor = Ground.GetComponent<Collider>().bounds;        
    }

    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation(goal.transform.localPosition);
        sensor.AddObservation(agent.transform.localPosition);
        sensor.AddObservation(agent.velocity);
        sensor.AddObservation(Vector3.Distance(goal.transform.localPosition, agent.transform.localPosition));
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];

        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                dirToGo = transform.right * 1f;
                break;
            case 4:
                dirToGo = transform.right * -1f;
                break;
        }
        agent.AddForce(dirToGo * moveSpeed,
            ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers.DiscreteActions);
        AddReward(-1f/MaxStep);
        BoundCheck();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            discreteActionsOut[0] = 2;
        }
    }

    public override void OnEpisodeBegin()
    {
        spawnAgent();
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("goal") == true)
        {
            SetReward(10f);
            EndEpisode();
        }
    }


    public void spawnAgent(){

        int point = Random.Range(0, SpawnPointsarr.Length-1);
        gameObject.transform.position = new Vector3(SpawnPointsarr[point].transform.position.x, SpawnPointsarr[point].transform.position.y, SpawnPointsarr[point].transform.position.z);
    }


    private void BoundCheck()
    {
        if (gameObject.transform.position.x < bndFloor.min.x || gameObject.transform.position.x > bndFloor.max.x)
        {
            AddReward(-1f);
            EndEpisode();
        }

        if (gameObject.transform.position.z < bndFloor.min.z || gameObject.transform.position.z > bndFloor.max.z)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

}
