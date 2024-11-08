using NUnit.Framework; // 使用 NUnit 测试框架
using UnityEngine;
using UnityEngine.TestTools; // 使用 Unity Test Framework
using System.Collections; // 使用 System.Collections 命名空间
using CircleOfLife; // 使用 CircleOfLife 命名空间
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;
using System.Reflection;
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

        GameObject.DestroyImmediate(dialogManagerObject);

        yield return null;
    }
    [UnityTest]
    public IEnumerator DialogUpdateSuccess()
    {
        // 创建一个包含 DialogManager 的 GameObject
        GameObject dialogManagerObject = new GameObject("DialogManagerObject");
        DialogManager dialogManager = dialogManagerObject.AddComponent<DialogManager>();

        TextAsset dialogDataFile = Resources.Load<TextAsset>("Texts/test");
        dialogManager.ReadText(dialogDataFile);

        GameObject NameTextObject = new GameObject("NameTextObject");
        GameObject ContentTextObject = new GameObject("ContentTextObject");

        dialogManager.NameText = NameTextObject.AddComponent<TextMeshProUGUI>();
        dialogManager.ContentText = ContentTextObject.AddComponent<TextMeshProUGUI>();

        dialogManager.UpdateText("test1", "test2");
        
        Assert.AreEqual("test1", dialogManager.NameText.text);
        Assert.AreEqual("test2", dialogManager.ContentText.text);


        GameObject.DestroyImmediate(dialogManagerObject);// 清理
        yield return null;
    }
    [UnityTest]
    public IEnumerator ButtonCreateCorrect()
    {
        // 创建一个包含 DialogManager 的 GameObject
        GameObject dialogManagerObject = new GameObject("DialogManagerObject");
        DialogManager dialogManager = dialogManagerObject.AddComponent<DialogManager>();

        TextAsset dialogDataFile = Resources.Load<TextAsset>("Texts/test");
        dialogManager.ReadText(dialogDataFile);

        dialogManager.GenerateOption(1);

        GameObject.DestroyImmediate(dialogManagerObject);// 清理
        yield return null;
    }
}
