using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWallTrigger : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<CharController>() == null) return;

        GameActions.BossHitTheMax?.Invoke();
    }
}
