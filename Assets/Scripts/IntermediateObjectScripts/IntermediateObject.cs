using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateObject : MonoBehaviour
{
    
    public Action Action;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharController>() == null)
        {
            Action?.Invoke();
        }
    }
}
