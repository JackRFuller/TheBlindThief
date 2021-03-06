﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WireBehaviour : MonoBehaviour
{
    [Header("Platform Target")]
    [SerializeField] private GameObject[] targetPlatforms;

    [Header("Wire Objects")]
    [SerializeField] private Image[] wires;

    private bool activateWires;

    [Header("Fill Rate")]
    [SerializeField] private float fillRate;

    private Image activeWire;
    private int wireCount;

    void Start()
    {
        InitiateWireFill();
    }

    void InitiateWireFill()
    {
        wireCount = 0;
        activeWire = wires[0];
    }

    void ActivateWire()
    {
        activateWires = true;
    }

    void Update()
    {
        if(activateWires)
            FillWire();
    }

    void FillWire()
    {
        activeWire.fillAmount += fillRate*Time.deltaTime;

        if(activeWire.fillAmount >= 1)
        {
            IncrementWire();
        }
    }

    void IncrementWire()
    {
        if (wireCount < wires.Length - 1)
        {
            wireCount++;
            activeWire = wires[wireCount];
        }
        else
        {
            EndWireFill();
        }
        
    }

    void EndWireFill()
    {
        activateWires = false;
        
        for (int i = 0; i < targetPlatforms.Length; i++)
        {
            targetPlatforms[i].SendMessage("ActivatePlatform",SendMessageOptions.DontRequireReceiver);
        }
       
    }

}
