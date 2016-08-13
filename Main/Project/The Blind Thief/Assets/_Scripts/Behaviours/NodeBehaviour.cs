using UnityEngine;
using System.Collections;

public class NodeBehaviour : MonoBehaviour {

	void Awake()
    {
        //RoundPosition();
    }

    void RoundPosition()
    {
        float x = Mathf.Round(transform.position.x);
        float y = Mathf.Round(transform.position.z);
        float z = Mathf.Round(transform.position.z);

        Vector3 newPosition = new Vector3(x, y, z);
        transform.position = newPosition;
    }

    
}
