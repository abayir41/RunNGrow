using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class NormalObstacleObject : MonoBehaviour
{
    private GameConfig Config => GameController.Config;
    public NormalObstacleType Type => _obstacleProperties.obstacleType;
    public GameObject[] cylinders;
    public GameObject[] quad;

    public int ObstaclePoint => _obstacleProperties.obstaclePoint;

    [SerializeField] private Transform leftArrow;
    [SerializeField] private Transform rightArrow;
    [SerializeField] private TextMeshPro text;

    [ShowInInspector]
    private ObstacleStruct _obstacleProperties;

    [SerializeField] private MeshRenderer middlePanel;
    [SerializeField] private GameObject normalObstacle;
    [SerializeField] private GameObject partRemover;
    [SerializeField] private GameObject destroyer;
    [SerializeField] private float destroyerRotPower;

    private void Update()
    {
        if (_obstacleProperties.obstacleType == NormalObstacleType.Destroyer)
        {
            destroyer.transform.Rotate(new Vector3(0,1,0) * (Time.deltaTime * destroyerRotPower));
        }
    }

    public void SetProperties(ObstacleStruct prop)
    {
        _obstacleProperties = prop;

        switch (_obstacleProperties.obstacleType)
        {
            case NormalObstacleType.AddHeight:
                normalObstacle.SetActive(true);
                middlePanel.material = Config.BlueColor;
                cylinders[0].SetActive(true);
                leftArrow.Rotate(new Vector3(0,0,-90));
                rightArrow.Rotate(new Vector3(0,0,-90));
                text.text = "+" + prop.obstaclePoint;
                break;
            
            case NormalObstacleType.AddWeight:
                normalObstacle.SetActive(true);
                middlePanel.material = Config.BlueColor;
                cylinders[0].SetActive(true);
                leftArrow.Rotate(new Vector3(0,0,0));
                rightArrow.Rotate(new Vector3(0,0,180));
                text.text = "+" + prop.obstaclePoint;
                break;
            
            case NormalObstacleType.DivideHeight:
                normalObstacle.SetActive(true);
                middlePanel.material = Config.RedColor;
                cylinders[1].SetActive(true);
                leftArrow.Rotate(new Vector3(0,0,90));
                rightArrow.Rotate(new Vector3(0,0,90));
                text.text = "÷" + prop.obstaclePoint;
                break;
            
            case NormalObstacleType.DivideWeight:
                normalObstacle.SetActive(true);
                middlePanel.material = Config.RedColor;
                cylinders[1].SetActive(true);
                leftArrow.Rotate(new Vector3(0,0,180));
                rightArrow.Rotate(new Vector3(0,0,0));
                text.text = "÷" + prop.obstaclePoint;
                break;
            
            case NormalObstacleType.ExtractHeight:
                normalObstacle.SetActive(true);
                middlePanel.material = Config.RedColor;
                cylinders[1].SetActive(true);
                leftArrow.Rotate(new Vector3(0,0,90));
                rightArrow.Rotate(new Vector3(0,0,90));
                text.text = "-" + prop.obstaclePoint;
                break;
            
            case NormalObstacleType.ExtractWeight:
                normalObstacle.SetActive(true);
                middlePanel.material = Config.RedColor;
                cylinders[1].SetActive(true);
                leftArrow.Rotate(new Vector3(0,0,180));
                rightArrow.Rotate(new Vector3(0,0,0));
                text.text = "-" + prop.obstaclePoint;
                break;
            
            case NormalObstacleType.MultiplyHeight:
                normalObstacle.SetActive(true);
                middlePanel.material = Config.BlueColor;
                cylinders[0].SetActive(true);
                leftArrow.Rotate(new Vector3(0,0,-90));
                rightArrow.Rotate(new Vector3(0,0,-90));
                text.text = "X" + prop.obstaclePoint;
                break;
            
            case NormalObstacleType.MultiplyWeight:
                normalObstacle.SetActive(true);
                middlePanel.material = Config.BlueColor;
                cylinders[0].SetActive(true);
                leftArrow.Rotate(new Vector3(0,0,0));
                rightArrow.Rotate(new Vector3(0,0,180));
                text.text = "X" + prop.obstaclePoint;
                break;
            
            case NormalObstacleType.Destroyer:
                destroyer.SetActive(true);
                for (int i = 0; i < quad.Length; i++)
                {
                    quad[i].SetActive(false);
                }
                break;
            
            case NormalObstacleType.PartRemover:
                partRemover.SetActive(true);
                for (int i = 0; i < quad.Length; i++)
                {
                    quad[i].SetActive(false);
                }
                break;
        }
    }
    
}   
