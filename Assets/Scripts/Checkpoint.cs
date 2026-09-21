using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] CheckpointManage cm;

    void Start()
    {
        cm = GlobalValues.cm;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (cm.checkpointList.IndexOf(this) > cm.checkpointList.IndexOf(cm.currentCheckpoint))
            {
                cm.SetCheckpoint(this);
            }
            
        }
    }
}
