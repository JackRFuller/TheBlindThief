using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

public class ResetController : Singleton<ResetController>
{
    public static event Action ResetLevel;

    public void Reset()
    {
        ResetLevel();
    }

}
