using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationThrowEvent : MonoBehaviour
{
    public void ThrowOne()
    {
        GameController.Instance.MiddleCharSendOneIntermediate();
    }
}
