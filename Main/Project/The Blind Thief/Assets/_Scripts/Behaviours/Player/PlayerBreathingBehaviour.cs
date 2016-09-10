using UnityEngine;
using System.Collections;

public class PlayerBreathingBehaviour : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Enemy"))
        {
            Debug.Log("Hit Enemy");
        }
    }
}
