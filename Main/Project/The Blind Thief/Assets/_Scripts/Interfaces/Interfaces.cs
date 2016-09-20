using UnityEngine;
using System.Collections;

public interface IReset
{
    void Reset();
}

public interface IEvent
{
    void OnEnable();
    void OnDisable();
}