using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateObject : MonoBehaviour
{
    
    public Action Action;
    public Transform target;
    public float speed;
    public bool objectArrived;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharController>() == null)
        {
            Action?.Invoke();
        }
    }

    private void Update()
    {
        if (target == null) return;
        if(objectArrived) return;

        var step =  speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        // Check if the position of the cube and sphere are approximately equal.
        if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            objectArrived = true;
            // Swap the position of the cylinder.
            Action?.Invoke();
        }
    }
}
