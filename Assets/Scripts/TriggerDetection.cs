using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerDetection : MonoBehaviour
{
    public string targetTag = null;
    public UnityEvent<int> countUpdateFunction;
    public UnityEvent<bool> boolUpdateFunction;

    void OnTriggerEnter(Collider collider)
    {
        if (targetTag == null || collider.CompareTag(targetTag))
        {
            countUpdateFunction.Invoke(1);
            boolUpdateFunction.Invoke(true);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (targetTag == null || collider.CompareTag(targetTag))
            countUpdateFunction.Invoke(-1);
    }
}
