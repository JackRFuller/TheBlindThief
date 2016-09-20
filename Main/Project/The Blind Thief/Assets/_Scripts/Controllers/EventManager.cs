using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;
    public static EventManager Instance { get { return instance; } }

    private Dictionary<string, UnityEvent> eventDictionary;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Init();
    }
    

    void Init()
    {
        if (eventDictionary == null)
            eventDictionary = new Dictionary<string, UnityEvent>();
    }

    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;

        if (Instance.eventDictionary == null)
            Instance.eventDictionary = new Dictionary<string, UnityEvent>();

        //Check if Event exists
        if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else //Create the Event
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            Instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener)
    {
        if (Instance == null)
            return;

        UnityEvent thisEvent = null;
        if(Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if(Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}
