using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateObjectActions : MonoBehaviour
{
    //GameObject that will be transferring, Side that which side object leave
    public static Action<GameObject, Side> IntermediateStartToMove;
    
    //GameObject that will be transferring, Side that which side object will arrive
    public static Action<GameObject, Side> IntermediateObjectArrivedSuccessfully;
}
