using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Configuration;
using CircleOfLife.General;
using Milutools.Milutools.UI;
using UnityEngine;

namespace CircleOfLife
{
    public class MessageBoxTest : MonoBehaviour
    {
        public string Title;
        [Multiline]
        public string Content;

        public void Test()
        {
            MessageBox.Open((Title, Content), (result) =>
            {
                Debug.Log("你按下了：" + result);
            });
        }
    }
}
