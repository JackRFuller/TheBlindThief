using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class InputController : Singleton<InputController>
{
    private Vector3 targetPosition;
    public Vector3 TargetPosition
    {
        get { return targetPosition; }
    }

    public static event Action PlayerInput; //Registers when theres been a valid input

    void Update()
    {
        GetPlayerInput();
    }

    void GetPlayerInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            SendOutRaycastFromMousePosition();
        }
    }

    void SendOutRaycastFromMousePosition()
    {
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit _hit;

        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity))
        {
            if (_hit.collider.tag == "Node")
            {
                targetPosition = _hit.point;

                if (PlayerInput != null)
                    PlayerInput();
            }
            else
            {
                _hit.transform.SendMessage("HitByRaycast", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
