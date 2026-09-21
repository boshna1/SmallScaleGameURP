using UnityEngine;

public class InstakillObstacle : MonoBehaviour
{ 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Touchinsta");
            collision.gameObject.GetComponent<PlayerHp>().Instakill();
        }
    }
}
