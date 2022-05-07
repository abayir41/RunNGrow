using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class IntermediateObjectController : MonoBehaviour
{
    public static IntermediateObjectController Instance { get; private set; }

    [SerializeField] private GameObject intermediateObject;
    [SerializeField] private Transform poolObjectsParent;

    private List<GameObject> _objectsInPool;
    private List<TweenObjectAnims> _intermediateObjectsMoveAnims;
    private List<GameObject> _activeObjects;

    private readonly struct TweenObjectAnims
    {
        public TweenObjectAnims(TweenerCore<Vector3, Vector3, VectorOptions> animX,
            TweenerCore<Vector3, Vector3, VectorOptions> animYp1,
            TweenerCore<Vector3, Vector3, VectorOptions> animYp2,
            TweenerCore<Vector3, Vector3, VectorOptions> animZ)
        {
            AnimX = animX;
            AnimYp1 = animYp1;
            AnimYp2 = animYp2;
            AnimZ = animZ;
        }

        private TweenerCore<Vector3, Vector3, VectorOptions> AnimX { get; }
        private TweenerCore<Vector3, Vector3, VectorOptions> AnimYp1 { get; }
        private TweenerCore<Vector3, Vector3, VectorOptions> AnimYp2 { get; }
        private TweenerCore<Vector3, Vector3, VectorOptions> AnimZ { get; }

        public void SetOnComplete(Action action)
        {
            AnimX.OnComplete(() => action?.Invoke());
        }

        public void KillAllAnims()
        {
            AnimX.Kill();
            AnimYp1.Kill();
            AnimYp2.Kill();
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
    /// <param name="goingTo">Side that which side object will arrive</param>
    /// <param name="size"></param>
    public void MoveIntermediateObject(Vector3 startPos, Vector3 endPos, float duration, Side goingTo, Vector2 size)
    {
        var obj = GetObject();
        
        var objTransform = obj.transform;

        //make it visible
        objTransform.position = startPos;
        obj.SetActive(true);

        var highestY = Math.Max(startPos.y, endPos.y);

        //move anim, make command on one anim, they will complete same time
        var animX = objTransform.DOMoveX(endPos.x, duration).SetEase(Ease.Linear);
        var animYp1 = objTransform.DOMoveY(highestY + GameController.Config.Amplitude, duration/2).SetEase(Ease.OutCirc);
        var animYp2 = objTransform.DOMoveY(endPos.y, duration/2).SetDelay(duration/2).SetEase(Ease.InCirc);
        var animZ = objTransform.DOMoveZ(endPos.z, duration).SetEase(Ease.Linear);

        var anims = new TweenObjectAnims(animX, animYp1, animYp2, animZ);
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

    /// <summary>
    /// Getting object, integrated with pool system
    /// </summary>
    /// <returns>Pool Object</returns>
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
