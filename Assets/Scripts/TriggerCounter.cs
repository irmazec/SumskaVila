using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerCounter : MonoBehaviour
{
    public UnityEvent<int> countUpdateFunction;

    void OnTriggerEnter(Collider collider)
    {
        countUpdateFunction.Invoke(1);
    }

    void OnTriggerExit(Collider collider)
    {
        countUpdateFunction.Invoke(-1);
    }
}
