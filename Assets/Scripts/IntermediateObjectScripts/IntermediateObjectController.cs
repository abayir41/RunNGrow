using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class IntermediateObjectController : MonoBehaviour
{
    public static IntermediateObjectController Instance;

    [SerializeField] private GameObject intermediateObject;
    [SerializeField] private Transform poolObjectsParent;

    private List<GameObject> _objectsInPool;
    private List<TweenerCore<Vector3, Vector3, VectorOptions>> intermediateObjectsMoveAnims;

    private void Awake()
    {
        intermediateObjectsMoveAnims = new List<TweenerCore<Vector3, Vector3, VectorOptions>>();
        _objectsInPool = new List<GameObject>();
        Instance = this;
    }

    #region Subscription
    
    private void OnEnable()
    {
        IntermediateObjectActions.IntermediateObjectArrivedSuccessfully += IntermediateObjectArrivedSuccessfully;
    }
    
    private void OnDisable()
    {
        IntermediateObjectActions.IntermediateObjectArrivedSuccessfully -= IntermediateObjectArrivedSuccessfully;
    }
    

    private void IntermediateObjectArrivedSuccessfully(GameObject obj, Side side)
    {
        obj.SetActive(false);
        
        _objectsInPool.Add(obj);
    }

    #endregion

    
    public bool IsThereAnyIntermediateObjectMoving()
    {
        return intermediateObjectsMoveAnims.Count > 0;
    }

    /// <summary>
    /// Get from pool, and move objects
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="duration"></param>
    /// <param name="goingTo"></param> Side that which side object will arrive
    public void MoveIntermediateObject(Vector3 startPos, Vector3 endPos, float duration, Side goingTo)
    {
        //check if there is any object in the pool, if not create one 
        if(_objectsInPool.Count == 0)
            CreatePoolObject();

        //get one obj
        var obj = _objectsInPool[0];
        var objTransform = obj.transform;

        //make it visible
        objTransform.position = startPos;
        obj.SetActive(true);

        //determine which side obj leaving
        var leaveSide = Utilities.GetOtherSide(goingTo);
        
        //notify that object leave
        IntermediateObjectActions.IntermediateStartToMove?.Invoke(obj, leaveSide);
        
        //move anim, make command on one anim, they will complete same time
        var animHandler = objTransform.DOMoveX(endPos.x, duration).SetEase(Ease.Linear).OnComplete(() => IntermediateObjectActions.IntermediateObjectArrivedSuccessfully?.Invoke(obj, goingTo));
        objTransform.DOMoveZ(endPos.z, duration).SetEase(Ease.Linear);
        objTransform.DOMoveY(endPos.y + 1, duration).SetEase(Ease.OutBack, 20, 0);
        
        //Save/discard Anim
        intermediateObjectsMoveAnims.Add(animHandler);
        animHandler.OnKill(() => intermediateObjectsMoveAnims.Remove(animHandler));

        //remove from pool
        _objectsInPool.Remove(obj);
    }
    
    /// <summary>
    /// This method create object, make it disappear and adding to pool
    /// </summary>
    private void CreatePoolObject()
    {
        //Spawn
        var obj = Instantiate(intermediateObject,poolObjectsParent);
        
        //disappear in the game
        obj.SetActive(false);

        //Add to pool
        _objectsInPool.Add(obj);
    }
}
