using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateObject : MonoBehaviour
{
    
    public Action Action;


    public IEnumerator MoveToPosition(Transform target1, float timeToMove)
    {
        var startPos = transform.position;
        var t = 0f;
        while (t < timeToMove)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, target1.position, t / timeToMove);
            yield return null;
        }
        
        
        Action?.Invoke();
        
    }
}
