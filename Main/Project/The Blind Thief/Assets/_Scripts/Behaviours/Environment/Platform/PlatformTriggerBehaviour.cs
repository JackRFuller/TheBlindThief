using UnityEngine;
using System.Collections;

public class PlatformTriggerBehaviour : MonoBehaviour
{
    private bool hasBeenActivated;
    private bool hasEnemyOnIt;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && !hasBeenActivated)
        {
            if (hasEnemyOnIt)
                return;

            transform.parent.SendMessage("ActivatePlatform",false, SendMessageOptions.DontRequireReceiver);
            hasBeenActivated = true;
        }
        if(other.tag == "Enemy")
        {
            Debug.Log("Enemy Hit");
            transform.parent.SendMessage("ActivatePlatform", true, SendMessageOptions.DontRequireReceiver);
            hasEnemyOnIt = true;
            hasBeenActivated = false;
        } 
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag.Equals("Enemy"))
        {
            hasEnemyOnIt = false;
        }
    }
}
