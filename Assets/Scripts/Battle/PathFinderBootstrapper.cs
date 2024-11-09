using System;
using RuiRuiSTL;
using RuiRuiVectorField;
using UnityEngine;

namespace CircleOfLife.Battle
{
    public class PathFinderBootstrapper : MonoBehaviour
    {
        public CameraController CameraController;
        public LayerMask ObstacleLayer;
        private void Awake()
        {
            VectorField.Initialize(ObstacleLayer, 
                new Range2Int(Mathf.RoundToInt(CameraController.CameraMinX - 5f), Mathf.RoundToInt(CameraController.CameraMaxX + 5f)),
                new Range2Int(Mathf.RoundToInt(CameraController.CameraMinY - 5f), Mathf.RoundToInt(CameraController.CameraMaxY + 5f))
                );
        }
    }
}
