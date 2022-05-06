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
    private List<TweenObjectAnims> _intermediateObjectsMoveAnims;
    private List<GameObject> _activeObjects;

    private readonly struct TweenObjectAnims
    {
        public TweenObjectAnims(TweenerCore<Vector3, Vector3, VectorOptions> animX,
            TweenerCore<Vector3, Vector3, VectorOptions> animY, 
            TweenerCore<Vector3, Vector3, VectorOptions> animZ)
        {
            AnimX = animX;
            AnimY = animY;
            AnimZ = animZ;
        }

        private TweenerCore<Vector3, Vector3, VectorOptions> AnimX { get; }
        private TweenerCore<Vector3, Vector3, VectorOptions> AnimY { get; }
        private TweenerCore<Vector3, Vector3, VectorOptions> AnimZ { get; }

        public void SetOnComplete(Action action)
        {
            AnimX.OnComplete(() => action?.Invoke());
        }

        public void KillAllAnims()
        {
            AnimX.Kill();
            AnimY.Kill();
            AnimZ.Kill();
        }
    }

    private void Awake()
    {
        _intermediateObjectsMoveAnims = new List<TweenObjectAnims>();
        _objectsInPool = new List<GameObject>();
        _activeObjects = new List<GameObject>();
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
    

    private void IntermediateObjectArrivedSuccessfully(Vector2 size, GameObject obj, Side side)
    {
        obj.SetActive(false);
        
        _objectsInPool.Add(obj);
    }

    #endregion

    
    public bool IsThereAnyIntermediateObjectMoving()
    {
        return _intermediateObjectsMoveAnims.Count > 0;
    }

    /// <summary>
    /// Get from pool, and move objects
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="duration"></param>
    /// <param name="goingTo"></param> Side that which side object will arrive
    public void MoveIntermediateObject(Vector3 startPos, Vector3 endPos, float duration, Side goingTo, Vector2 size)
    {
        var obj = GetObject();
        
        var objTransform = obj.transform;

        //make it visible
        objTransform.position = startPos;
        obj.SetActive(true);

        //determine which side obj leaving
        var leaveSide = Utilities.GetOtherSide(goingTo);
        
        //fix the target y, because if diff is 0, twenn cant make Ease.Outback, so ve have to add mini diff to make effect
        var targetY = startPos.y - endPos.y == 0 ? endPos.y + 0.001f : endPos.y;
        
        //move anim, make command on one anim, they will complete same time
        var animX = objTransform.DOMoveX(endPos.x, duration).SetEase(Ease.Linear);
        var animY = objTransform.DOMoveZ(endPos.z, duration).SetEase(Ease.Linear);
        var animZ = objTransform.DOMoveY(targetY, duration).SetEase(Ease.OutBack, 20 * 1000f, 0);

        var anims = new TweenObjectAnims(animX, animY, animZ);
        //Save/discard Anim
        _intermediateObjectsMoveAnims.Add(anims);
        anims.SetOnComplete(() =>
        {
            _intermediateObjectsMoveAnims.Remove(anims);
            IntermediateObjectActions.IntermediateObjectArrivedSuccessfully?.Invoke(size, obj, goingTo);
        });

        //remove from pool, add actives
        _objectsInPool.Remove(obj);
        _activeObjects.Add(obj);
    }

    //Getting object, integrated with pool system
    private GameObject GetObject()
    {
        //check if there is any object in the pool, if not create one 
        if(_objectsInPool.Count == 0)
            CreatePoolObject();

        //get one obj
        return _objectsInPool[0];
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

    public void KillTheAnimations()
    {
        foreach (var obj in _activeObjects)
        {
            obj.SetActive(false); 
            _objectsInPool.Add(obj);
        }
        
        _activeObjects.Clear();

        foreach (var moveAnim in _intermediateObjectsMoveAnims)
        {
            moveAnim.KillAllAnims();
        }
        
        _intermediateObjectsMoveAnims.Clear();
    }
}
