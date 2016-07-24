using UnityEngine;
using System.Collections;

public class PlatformTriggerBehaviour : MonoBehaviour
{
    private bool hasBeenActivated;

    void OnTriggerEnter(Collider other)
    {
        if (!hasBeenActivated)
        {
            if (other.tag == "Player")
            {
                transform.parent.SendMessage("ActivatePlatform", SendMessageOptions.DontRequireReceiver);
                hasBeenActivated = true;
            }
            
        }
    }
}
