using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateObjectActions : MonoBehaviour
{
    //Vevtor2 that size of will be added to other object, Side that which side object leave
    public static Action<Vector2, Side> IntermediateStartToMove;
    
    //GameObject that will be transferring, Side that which side object will arrive
    public static Action<Vector2, GameObject, Side> IntermediateObjectArrivedSuccessfully;
}
