using System;
using RuiRuiAstar;
using RuiRuiMathTool;
using RuiRuiSTL;
using RuiRuiVectorField;
using UnityEngine;

namespace CircleOfLife.Battle
{
    public class PathFinderBootstrapper : MonoBehaviour
    {
        public CameraController CameraController;
        public LayerMask ObstacleLayer;
        public Transform ProtectAnimal;
        private void Awake()
        {
            Astar.Initialize(
                new Range2Int(Mathf.RoundToInt(CameraController.CameraMinX - 5f), Mathf.RoundToInt(CameraController.CameraMaxX + 5f)),
                new Range2Int(Mathf.RoundToInt(CameraController.CameraMinY - 5f), Mathf.RoundToInt(CameraController.CameraMaxY + 5f)),
                ObstacleLayer
            );
            VectorField.Initialize(ObstacleLayer, 
                new Range2Int(Mathf.RoundToInt(CameraController.CameraMinX - 5f), Mathf.RoundToInt(CameraController.CameraMaxX + 5f)),
                new Range2Int(Mathf.RoundToInt(CameraController.CameraMinY - 5f), Mathf.RoundToInt(CameraController.CameraMaxY + 5f))
                );
            VectorField.UpdateVectorFieldTotal(ProtectAnimal.position.RoundToVector2Int());
        }
    }
}
