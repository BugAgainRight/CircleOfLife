using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CircleOfLife.Key
{
    public static class KeyboardSet
    {
        public static readonly Dictionary<KeyEnum, KeyCode> KeyboardDict = new()
        {
            // Player
            { KeyEnum.Up, KeyCode.W },
            { KeyEnum.Down, KeyCode.S },
            { KeyEnum.Left, KeyCode.A },
            { KeyEnum.Right, KeyCode.D },
            { KeyEnum.Attack, KeyCode.J },
            { KeyEnum.Fire, KeyCode.K },
            { KeyEnum.Interact, KeyCode.F },
            { KeyEnum.Skill1, KeyCode.Alpha3 },
            { KeyEnum.Skill2, KeyCode.Alpha4 },
            { KeyEnum.Skill3, KeyCode.Alpha5 },
            { KeyEnum.Skill4, KeyCode.Alpha6 },
            { KeyEnum.Skill5, KeyCode.Alpha7 },
            { KeyEnum.Skill6, KeyCode.Alpha8 },
            { KeyEnum.Skill7, KeyCode.Alpha9 },

            { KeyEnum.Running, KeyCode.P },
            { KeyEnum.Struggle, KeyCode.Space },
            // Camera
            { KeyEnum.Click1, KeyCode.Mouse0 },
            { KeyEnum.Click2, KeyCode.Mouse1 },
            { KeyEnum.ZoomOut, KeyCode.KeypadMinus },
            { KeyEnum.ZoomIn, KeyCode.KeypadPlus },
            { KeyEnum.RotatetToRight, KeyCode.Q },
            { KeyEnum.RotatetToLeft, KeyCode.E },
            { KeyEnum.CameraMoveUp, KeyCode.UpArrow },
            { KeyEnum.CameraMoveDown, KeyCode.DownArrow },
            { KeyEnum.CameraMoveLeft, KeyCode.LeftArrow },
            { KeyEnum.CameraMoveRight, KeyCode.RightArrow },
            { KeyEnum.CameraModeFollow, KeyCode.Alpha1 },
            { KeyEnum.CameraModeFreeze, KeyCode.Alpha2 },
            { KeyEnum.CameraModeFree, KeyCode.Mouse2 },
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static KeyCode GetKeyCode(KeyEnum key)
        {
            return KeyboardDict[key];
        }

        /// <summary>
        /// 当前按键是否处于按下的状态
        /// </summary>
        public static bool IsPressing(this KeyEnum key)
            => Input.GetKey(GetKeyCode(key));

        /// <summary>
        /// 当前是否是按键按下的第一帧
        /// </summary>
        public static bool IsKeyDown(this KeyEnum key)
            => Input.GetKeyDown(GetKeyCode(key));

        /// <summary>
        /// 当前是否是按键抬起前的最后一帧
        /// </summary>
        public static bool IsKeyUp(this KeyEnum key)
            => Input.GetKeyUp(GetKeyCode(key));
    }
}
