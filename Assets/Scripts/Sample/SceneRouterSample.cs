using System;
using CircleOfLife.Configuration;
using CircleOfLife.General;
using Milutools.SceneRouter;
using UnityEngine;

namespace CircleOfLife.Sample
{
    // 场景路由使用示例
    public class SceneRouterSample : MonoBehaviour
    {
        public bool TestFetchParameter;
        private void Awake()
        {
            if (!TestFetchParameter)
            {
                return;
            }
            
            // 下一个场景在这里接收参数
            MessageBox.Open(("跨场景参数传递", "参数字符串：" + SceneRouter.FetchParameters<string>()));
        }

        public void GoTo()
        {
            SceneRouter.GoTo(SceneIdentifier.SceneRouterSample)
                       .Parameters("测试测试测试测试"); // 传递参数给下一个场景
        }
        
        public void Back()
        {
            // 返回到上一级场景
            // 例如，在 GameBuilder 中设置了当前场景的路径是 sample/scene-router，则在这个场景返回会回到 路径是 sample 的场景；
            // 例如，在 GameBuilder 中设置了当前场景的路径是 sample，则在这个场景返回会回到设置的【根节点】场景；
            SceneRouter.Back();
        }
    }
}
