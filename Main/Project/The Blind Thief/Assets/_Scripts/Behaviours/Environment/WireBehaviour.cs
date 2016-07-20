using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WireBehaviour : MonoBehaviour
{
    [Header("Background Wires")]
    [SerializeField] private Transform[] backgroundWirePoints;
    [SerializeField] private LineRenderer[] backgroundWireLines;

    [Header("Foreground Wires")]
    [SerializeField] private Transform[] foregroundWirePoints;
    [SerializeField] private LineRenderer[] foregroundWireLines;

    [Header("Increase")]
    [SerializeField] private float increase;

    void Start()
    {
        SetBackgroundWire();
    }

    void SetBackgroundWire()
    {
        for (int i = 0; i < backgroundWirePoints.Length; i++)
        {
            backgroundWireLines[i].SetPosition(0,backgroundWirePoints[i].position);

            if (i < backgroundWirePoints.Length - 1)
            { 
                Vector3 _targetPos = DeterminePositionalIncrease(backgroundWirePoints[i + 1].position, backgroundWirePoints[i].position);
                backgroundWireLines[i].SetPosition(1, _targetPos);
            }
        }
    }

    Vector3 DeterminePositionalIncrease(Vector3 _wire2, Vector3 _wire1)
    {
        Vector3 _newPos = _wire2 - _wire1;

        if (_wire2.y > _wire1.y)
        {
            _newPos.y += increase;
        }
        else if (_wire2.y < _wire1.y)
        {
            _newPos.y -= increase;
        }

        if (_wire2.x > _wire1.x)
        {
            _newPos.x += increase;
        }

        if (_wire2.x < _wire1.x)
        {
            _newPos.x -= increase;
        }

        return _newPos;
    }
	
}
