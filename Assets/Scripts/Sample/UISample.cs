using System.Collections;
using System.Collections.Generic;
using Milutools.Milutools.UI;
using TMPro;
using UnityEngine;

namespace CircleOfLife.Sample
{
    // UI 管理器使用示例（登录框）
    // 假设 UI 需要在打开时额外传入数据，则需要实现 ManagedUI<UISample, 传入数据类型>
    // 假设 UI 需要在打开时额外传入数据，又需要返回数据，则需要实现 ManagedUI<UISample, 传入数据类型, 返回数据类型>
    // 假设 UI 只需要返回数据，则需要实现 ManagedUIReturnValueOnly<UISample, 返回数据类型> 
    // 【重要】 UI还需要到 Configuration/GameBuilder 中进行注册。
    // 在场景中的设置可参考预制体：Resources/UI/UISample
    public class UISample : ManagedUIReturnValueOnly<UISample, UIDataSample>
    {
        // Inspector 中绑定 用户名输入框 和 密码输入框
        public TMP_InputField Account, Password;

        public void Login()
        {
            // Inspector 中绑定 确定 按钮按下时，调用此函数
            
            // 关闭并返回数据
            Close(new UIDataSample()
            {
                Account = Account.text,
                Password = Password.text
            });
        }
        
        protected override void Begin()
        {
            // 替代 Awake() 函数
        }

        protected override void AboutToClose()
        {
            // 即将关闭时执行，可以放置额外的 UI 关闭动画，没有则留空
            // 留空的时候会自动有一层 淡出 的动画
        }

        public override void AboutToOpen()
        {
            // 即将开始时执行，可以放置额外的 UI 打开动画，没有则留空
            // 留空的时候会自动有一层 淡入 的动画
            
            // 如果是带参数的UI，这里还可以获得到参数的具体内容
        }
    }
}
