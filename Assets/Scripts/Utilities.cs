using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    // Start is called before the first frame update
    public static Side GetOtherSide(Side side)
    {
        return side == Side.Right ? Side.Left : Side.Right;
    }
}
