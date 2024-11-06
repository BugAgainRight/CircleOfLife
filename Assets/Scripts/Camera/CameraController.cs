using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Key;
using UnityEngine;

namespace CircleOfLife
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public GameObject FollowTarget;
        private new Camera camera;

        [Header("相机模式")]
        public bool ChangeState = true;
        public CameraMoveMode CameraMode = CameraMoveMode.Follow;

        [Header("相机跟随")]
        public float CameraMinVelocity = 1;
        private float currentCameraVelocity;
        public float CatchUpTime = 0.1f;
        private float distance;
        private Vector2 moveDirection;

        [Header("自由移动")]

        public bool MouseByKeyBoard = true;
        public float FreeMoveByKeyBoardVelocity = 10;
        public bool MoveByMouse = true;
        public float FreeMoveByMouseVelocity = 10;
        private float freeMoveByMouseVelocity = 0;
        private Vector3 mousePosition;

        [Header("相机缩放")]
        public float ZoomInSpeed = 20;
        public float ZoomOutSpeed = 20;
        public float MaxOrthographicSize = 20;
        public float MinOrthographicSize = 5;

        [Header("旋转镜头")]
        public float RotateSpeed = 10;

        [Header("相机边界")]
        public bool CameraLimit = true;
        public bool ShowEdge = true;
        public float CameraMinX = -10;
        public float CameraMaxX = 10;
        public float CameraMinY = -10;
        public float CameraMaxY = 10;

        void Awake()
        {
            camera = GetComponent<Camera>();
        }

        void Update()
        {
            ChangeMode();
            if (CameraLimit)
            {
                CameraPositionLimit();
            }

        }

        private void FixedUpdate()
        {

            CameraMove();
            ZoomInAndZoomOut();
            Rotate();
        }

        public void ChangeMode()
        {
            if (!ChangeState)
            {
                return;
            }
            if (Input.GetKeyDown(KeyboardSet.GetKeyCode(KeyEnum.CameraMoveUp)) ||
                Input.GetKeyDown(KeyboardSet.GetKeyCode(KeyEnum.CameraMoveDown)) ||
                Input.GetKeyDown(KeyboardSet.GetKeyCode(KeyEnum.CameraMoveLeft)) ||
                Input.GetKeyDown(KeyboardSet.GetKeyCode(KeyEnum.CameraMoveRight)) ||
                Input.GetKeyDown(KeyboardSet.GetKeyCode(KeyEnum.CameraModeFree)))
            {
                CameraMode = CameraMoveMode.Free;
            }
            if (Input.GetKeyDown(KeyboardSet.GetKeyCode(KeyEnum.CameraModeFollow)))
            {
                CameraMode = CameraMoveMode.Follow;
            }
            if (Input.GetKeyDown(KeyboardSet.GetKeyCode(KeyEnum.CameraModeFreeze)))
            {
                CameraMode = CameraMoveMode.Freeze;
            }

        }

        public void CameraMove()
        {
            switch (CameraMode)
            {
                case CameraMoveMode.Follow:
                    CameraFollow();
                    break;
                case CameraMoveMode.Free:
                    CameraFreeMove();
                    break;
                default:
                    break;
            }
        }
        //跟踪目标
        public void CameraFollow()
        {
            if (FollowTarget == null)
            {
                try
                {
                    FollowTarget = GameObject.FindWithTag("Player");
                }
                catch (System.Exception)
                {
                    return;
                }
            }
            Vector3 targetPos = new Vector3(FollowTarget.transform.position.x, FollowTarget.transform.position.y, transform.position.z);
            moveDirection = (targetPos - transform.position).normalized;
            distance = Vector3.Distance(transform.position, targetPos);
            currentCameraVelocity = Mathf.Max(distance / CatchUpTime, CameraMinVelocity);
            if (distance < 0.1f)
            {
                currentCameraVelocity = 0;
            }
            transform.position = new Vector3(
                transform.position.x + moveDirection.x * currentCameraVelocity * Time.deltaTime,
                transform.position.y + moveDirection.y * currentCameraVelocity * Time.deltaTime,
                transform.position.z);
        }

        //自由视角
        public void CameraFreeMove()
        {
            if (MoveByMouse)
            {
                MoveByMouseFunc();
            }
            if (MouseByKeyBoard)
            {
                MoveByKeyBoardFunc();
            }
        }

        private void MoveByMouseFunc()
        {
            if (Input.GetKey(KeyboardSet.KeyboardDict[KeyEnum.CameraModeFree]))
            {
                Vector3 offset = Input.mousePosition - mousePosition;
                transform.Translate(-offset.x * freeMoveByMouseVelocity * Time.deltaTime, -offset.y * freeMoveByMouseVelocity * Time.deltaTime, 0.0f, Space.Self);
                mousePosition = Input.mousePosition;
                freeMoveByMouseVelocity = FreeMoveByMouseVelocity;
            }
            else
            {
                freeMoveByMouseVelocity = 0;
            }
        }

        private void MoveByKeyBoardFunc()
        {
            if (Input.GetKey(KeyboardSet.GetKeyCode(KeyEnum.CameraMoveUp)))
            {
                transform.Translate(0.0f, FreeMoveByKeyBoardVelocity * Time.deltaTime, 0.0f, Space.Self);
            }
            if (Input.GetKey(KeyboardSet.GetKeyCode(KeyEnum.CameraMoveDown)))
            {
                transform.Translate(0.0f, -FreeMoveByKeyBoardVelocity * Time.deltaTime, 0.0f, Space.Self);
            }
            if (Input.GetKey(KeyboardSet.GetKeyCode(KeyEnum.CameraMoveLeft)))
            {
                transform.Translate(-FreeMoveByKeyBoardVelocity * Time.deltaTime, 0.0f, 0.0f, Space.Self);
            }
            if (Input.GetKey(KeyboardSet.GetKeyCode(KeyEnum.CameraMoveRight)))
            {
                transform.Translate(FreeMoveByKeyBoardVelocity * Time.deltaTime, 0.0f, 0.0f, Space.Self);
            }
        }

        //转移目标
        public void ChangeTarget(GameObject gameObject)
        {
            FollowTarget = gameObject;
        }
        //放大缩小

        public void ZoomInAndZoomOut()
        {
            if (Input.GetKey(KeyboardSet.GetKeyCode(KeyEnum.ZoomIn)) && camera.orthographicSize < MaxOrthographicSize)
            {
                camera.orthographicSize += ZoomOutSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyboardSet.GetKeyCode(KeyEnum.ZoomOut)) && camera.orthographicSize > MinOrthographicSize)
            {
                camera.orthographicSize -= ZoomInSpeed * Time.deltaTime;
            }
        }

        //旋转镜头
        public void Rotate()
        {
            if (Input.GetKey(KeyboardSet.GetKeyCode(KeyEnum.RotatetToRight)))
            {
                transform.Rotate(0.0f, 0.0f, RotateSpeed * Time.deltaTime, Space.Self);
            }
            if (Input.GetKey(KeyboardSet.GetKeyCode(KeyEnum.RotatetToLeft)))
            {
                transform.Rotate(0.0f, 0.0f, -RotateSpeed * Time.deltaTime, Space.Self);
            }
        }

        //相机边界

        private void CameraPositionLimit()
        {
            float orthographicSize = camera.orthographicSize;
            float aspectRatio = camera.aspect;
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, CameraMinX + orthographicSize * aspectRatio, CameraMaxX - orthographicSize * aspectRatio),
                Mathf.Clamp(transform.position.y, CameraMinY + orthographicSize, CameraMaxY - orthographicSize),
                transform.position.z);
        }
        void OnDrawGizmos()
        {
            if (ShowEdge)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(CameraMinX, CameraMinY, transform.position.z), new Vector3(CameraMaxX, CameraMinY, transform.position.z));
                Gizmos.DrawLine(new Vector3(CameraMaxX, CameraMinY, transform.position.z), new Vector3(CameraMaxX, CameraMaxY, transform.position.z));
                Gizmos.DrawLine(new Vector3(CameraMaxX, CameraMaxY, transform.position.z), new Vector3(CameraMinX, CameraMaxY, transform.position.z));
                Gizmos.DrawLine(new Vector3(CameraMinX, CameraMaxY, transform.position.z), new Vector3(CameraMinX, CameraMinY, transform.position.z));
            }

        }
    }
}

