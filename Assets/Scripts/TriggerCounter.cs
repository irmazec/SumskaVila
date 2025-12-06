using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerCounter : MonoBehaviour
{
    public string targetTag = null;
    public UnityEvent<int> countUpdateFunction;

    void OnTriggerEnter(Collider collider)
    {
        if (targetTag == null || collider.CompareTag(targetTag))
            countUpdateFunction.Invoke(1);
    }

    void OnTriggerExit(Collider collider)
    {
        if (targetTag == null || collider.CompareTag(targetTag))
            countUpdateFunction.Invoke(-1);
    }
}
