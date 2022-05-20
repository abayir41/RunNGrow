using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalPartController : MonoBehaviour
{
    
    public static FinalPartController Instance { get; private set; }
    
    [SerializeField] private Transform lastWall;
    [SerializeField] private Transform finalPlatformParent;
    [SerializeField] private Transform finalPlatformStart;
    [SerializeField] private GameObject platformObj;
    
    [SerializeField] private int amountOfPlatform;

    private float _distance;
    [SerializeField] private Transform distanceElement1;
    [SerializeField] private Transform distanceElement2;

    private List<GameObject> _multiplierBlocks;
    [SerializeField] private float diffBetweenMultiplier;

    public List<FinalPlatform> finalPlatforms;
     private void Awake()
    {
        Instance = this;
        
        _multiplierBlocks = new List<GameObject>();

        _distance = Vector3.Distance(distanceElement1.position, distanceElement2.position);

    }

    private bool _activationPart;
    public void InstantiateFinalPart()
    {
        for (var i = 0; i < amountOfPlatform; i++)
        {
            var platform = Instantiate(platformObj, finalPlatformParent);

            _activationPart = !_activationPart;
            
            _multiplierBlocks.Add(platform);
            
            platform.GetComponent<FinalPlatform>().multiplier = 1 + i * diffBetweenMultiplier;

            if (_activationPart)
            {
                platform.GetComponent<FinalPlatform>().leftObject.SetActive(false);
                platform.GetComponent<FinalPlatform>().rightObject.transform.localScale *=
                    1 + i * diffBetweenMultiplier > 2 ? 2 : 1 + i * diffBetweenMultiplier;
                platform.GetComponent<FinalPlatform>().pos = platform.GetComponent<FinalPlatform>().rightObject.transform.GetChild(0);
            }
            else
            {
                platform.GetComponent<FinalPlatform>().rightObject.SetActive(false);
                platform.GetComponent<FinalPlatform>().leftObject.transform.localScale *=
                    1 + i * diffBetweenMultiplier > 2 ? 2 : 1 + i * diffBetweenMultiplier;
                platform.GetComponent<FinalPlatform>().pos = platform.GetComponent<FinalPlatform>().leftObject.transform.GetChild(0);
            }
                
            
            
            switch (GameController.Instance.MapAlongAxis)
            {
                case Axis.X:
                    var finalStartPosition = finalPlatformStart.position;
                    platform.transform.position = new Vector3(finalStartPosition.x + (i+1)*_distance, finalStartPosition.y, finalStartPosition.z);
                    lastWall.position = new Vector3(finalStartPosition.x + (i+1)*_distance, finalStartPosition.y, finalStartPosition.z);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            finalPlatforms.Add(platform.GetComponent<FinalPlatform>());
        }
        
        
        switch (GameController.Instance.MapAlongAxis)
        {
            case Axis.X:
                finalPlatformParent.position = new Vector3(ObstacleController.Instance.LastObstaclePos.x + GameController.Config.DistanceBetweenFinalAndLastObstacle, GameController.Instance.MiddleSideVec.y, GameController.Instance.MiddleSideVec.z);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        
        //painting and text setting
        var step = 0.0f;
        var matBlock = new MaterialPropertyBlock();
        for (var i = 0; i < _multiplierBlocks.Count; i++)
        {
            var multiplierBlock = _multiplierBlocks[i];
            var color = Color.HSVToRGB(step, 1, 1);
            matBlock.SetColor("_BaseColor", color);
            multiplierBlock.GetComponent<Renderer>().SetPropertyBlock(matBlock);
            step += 1.0f / _multiplierBlocks.Count;

            //Make one decimal
            multiplierBlock.GetComponentInChildren<TextMeshPro>().text = $"{_multiplierBlocks[i].GetComponent<FinalPlatform>().multiplier:0.0}" + "X";
        }
        
        
    }

    private void Update()
    {
        if(!GameController.Instance.gameStarted) return;
        if(GameController.Instance.gameFailed) return;
        
        var along = GameController.Instance.MapAlongAxis switch
        {
            Axis.X => Vector3.right,
            Axis.Z => Vector3.forward,
            _ => Vector3.zero
        };
        
        finalPlatformParent.position -= along * (GameController.Instance.SpeedOfFinalPart * Time.deltaTime);
    }
}
