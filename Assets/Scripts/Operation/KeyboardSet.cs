using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife.Key
{
    public class KeyboardSet
    {
        public static Dictionary<KeyEnum, KeyCode> KeyboardDict = new Dictionary<KeyEnum, KeyCode>()
        {
            //player
            {KeyEnum.Up,KeyCode.W},
            {KeyEnum.Down,KeyCode.S},
            {KeyEnum.Left,KeyCode.A},
            {KeyEnum.Right,KeyCode.D},
            {KeyEnum.Attack,KeyCode.J},
            {KeyEnum.Fire,KeyCode.K},
            {KeyEnum.Interact,KeyCode.F},
            {KeyEnum.Skill1,KeyCode.I},
            //camera
            {KeyEnum.Click1,KeyCode.Mouse0},
            {KeyEnum.Click2,KeyCode.Mouse1},
            {KeyEnum.ZoomOut,KeyCode.KeypadMinus},
            {KeyEnum.ZoomIn,KeyCode.KeypadPlus},
            {KeyEnum.RotatetToRight,KeyCode.Q},
            {KeyEnum.RotatetToLeft,KeyCode.E},
            {KeyEnum.CameraMoveUp,KeyCode.UpArrow},
            {KeyEnum.CameraMoveDown,KeyCode.DownArrow},
            {KeyEnum.CameraMoveLeft,KeyCode.LeftArrow},
            {KeyEnum.CameraMoveRight,KeyCode.RightArrow},
            {KeyEnum.CameraModeFollow,KeyCode.Alpha1},
            {KeyEnum.CameraModeFreeze,KeyCode.Alpha2},
            {KeyEnum.CameraModeFree,KeyCode.Mouse2},
        };

        public static void ChangeKey(KeyEnum key, KeyCode keyCode)
        {
            if (KeyboardDict.ContainsValue(keyCode))
            {
                //提示用户:The key is already in use"
                return;
            }
            KeyboardDict[key] = keyCode;
        }

        public static void ResetKey(KeyEnum key)
        {
            KeyboardDict[key] = KeyCode.None;
        }

        public static KeyCode GetKeyCode(KeyEnum key)
        {
            return KeyboardDict[key];
        }
    }
}
