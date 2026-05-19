using UnityEngine;

public class Level_End : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Game_Controller.instance.EndReached();
        }   
    }
}
