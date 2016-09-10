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

    //Double Clicked
    private bool doubleClicked;
    public bool DoubleClicked
    {
        get { return doubleClicked; }
    }
    private bool oneClick = false;
    [SerializeField] private float timeForDoubleClick;

    public static event Action PlayerInput; //Registers when theres been a valid input

    void Update()
    {
        GetPlayerInput();
    }

    void GetPlayerInput()
    {
        float _delay = 0.25f;

        if (Input.GetMouseButtonDown(0))
        {
            DetectDoubleClick();            
        }
        if (oneClick)
        {
            if ((Time.time - timeForDoubleClick) > _delay)
            {
                oneClick = false;
                SendOutRaycastFromMousePosition();
            }
        }
    }

    void DetectDoubleClick()
    {
        if (!oneClick)
        {
            //Single Click
            oneClick = true;
            timeForDoubleClick = Time.time;
            doubleClicked = false;
        }
        else
        {
            //Double Clicked
            oneClick = false;
            doubleClicked = true;

            SendOutRaycastFromMousePosition();
            //Debug.Log("Double Clicked");
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
