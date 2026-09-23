using UnityEngine;
using System.Collections.Generic;

public class CheckpointManage : MonoBehaviour
{
    public Checkpoint currentCheckpoint;
    Checkpoint lastCheckpoint;
    [SerializeField] GameObject player;
    [SerializeField] public List<Checkpoint> checkpointList;

    void Awake()
    {
        GlobalValues.SetCM(this);
    }

    public void SetCheckpoint(Checkpoint value)
    {
        currentCheckpoint = value;
    }

    public void ToLastCheckpoint()
    {
        Debug.Log("ToLastTP");
        player.transform.position = currentCheckpoint.transform.position;
    }

    public void RevertCheckpoint()
    {
        currentCheckpoint = lastCheckpoint;
        currentCheckpoint = null;
    }
}
