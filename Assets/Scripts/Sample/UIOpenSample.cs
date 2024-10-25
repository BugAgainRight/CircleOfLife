using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.General;
using UnityEngine;

namespace CircleOfLife.Sample
{
    // 启动 UI 示例
    public class UIOpenSample : MonoBehaviour
    {
        public void Open()
        {
            // Inspector 中绑定 按下按钮时，触发此函数
            // 实现的UI的类名.Open
            UISample.Open((data) =>
            {
                MessageBox.Open(("成功", "你输入的账户名是：" + data.Account + "，你想要继续查看密码吗？"),
                    (operation) =>
                    {
                        if (operation == MessageBox.Operation.Deny)
                        {
                            return;
                        }

                        MessageBox.Open(("那就继续看吧", "你输入的密码是：" + data.Password));
                    });
            });
        }
    }
}
