using NUnit.Framework; // 使用 NUnit 测试框架
using UnityEngine;
using UnityEngine.TestTools; // 使用 Unity Test Framework
using System.Collections; // 使用 System.Collections 命名空间
using CircleOfLife; // 使用 CircleOfLife 命名空间
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
public class DialogManagerTests
{
    [UnityTest]
    public IEnumerator ReadDialogDataSuccess()
    {
        // 创建一个包含 DialogManager 的 GameObject
        GameObject dialogManagerObject = new GameObject("DialogManagerObject");
        DialogManager dialogManager = dialogManagerObject.AddComponent<DialogManager>();

        TextAsset dialogDataFile = Resources.Load<TextAsset>("Texts/test");

        dialogManager.DialogDataFile = dialogDataFile;
        dialogManager.ReadText(dialogManager.DialogDataFile);

        Object.DestroyImmediate(dialogManagerObject);

        yield return null;
    }
    [UnityTest]
    public IEnumerator TestDialogManager_InitialDialog()
    {
        // 创建一个包含 DialogManager 的 GameObject
        GameObject dialogManagerObject = new GameObject("DialogManagerObject");
        DialogManager dialogManager = dialogManagerObject.AddComponent<DialogManager>();
        
        // 设置 DialogDataFile 属性，这里需要一个实际的 TextAsset
        TextAsset dialogDataFile = Resources.Load<TextAsset>("Texts/test");
        
        

        // 清理
        Object.DestroyImmediate(dialogManagerObject);

        yield return null;
    }
}
