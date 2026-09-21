using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;

public class CinemachineManager : MonoBehaviour
{
    CinemachineCamera cam;
    bool blended;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponentInChildren<CinemachineCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("player pass");
            Flip();
        }
        
    }


    void Flip()
    {
        blended = !blended;
        if (blended)
            cam.Priority.Value = 1;
        else
            cam.Priority.Value = -1;
    }

}
