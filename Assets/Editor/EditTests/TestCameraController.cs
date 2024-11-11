using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
//此代码需要在空场景中运行！！！
namespace CircleOfLife.Tests
{
    public class CameraControllerTests
    {
        private GameObject cameraObject;
        private CameraController cameraController;

        [SetUp]
        public void Setup()
        {

            cameraObject = new GameObject();
            cameraController = cameraObject.AddComponent<CameraController>();

            cameraObject.AddComponent<Camera>();
            cameraController.FollowTarget = new GameObject();
        }

        [TearDown]
        public void Teardown()
        {
           GameObject.DestroyImmediate(cameraObject);
        }

        [UnityTest]
        public IEnumerator CameraFollowTargetPosition()
        {
            yield return new EnterPlayMode();

            Vector3 initialCameraPosition = cameraObject.transform.position;
            cameraController.FollowTarget.transform.position = new Vector3(10, 10, initialCameraPosition.z);

            cameraController.CameraFollow();

            Assert.AreNotEqual(initialCameraPosition, cameraObject.transform.position);
            yield return new ExitPlayMode();
        }

        

        [UnityTest]
        public IEnumerator CameraPositionWithinBounds()
        {
            
            cameraController.CameraLimit = true;
            cameraObject.transform.position = new Vector3(-15, -15, 0);
            yield return new EnterPlayMode();
            yield return null;

            Assert.IsTrue(cameraObject.transform.position.x >= cameraController.CameraMinX);
            Assert.IsTrue(cameraObject.transform.position.y >= cameraController.CameraMinY);
            yield return new ExitPlayMode();
        }
    }
}
