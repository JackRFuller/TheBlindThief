using UnityEngine;
using System.Collections;

public class PlayerBreathingBehaviour : MonoBehaviour {

    [SerializeField]
    private Transform player;

	void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Enemy"))
        {
            other.gameObject.SendMessage("HitByMainCharacter", player, SendMessageOptions.DontRequireReceiver);
        }
    }
}
