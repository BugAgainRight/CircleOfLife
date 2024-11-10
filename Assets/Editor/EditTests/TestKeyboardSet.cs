using NUnit.Framework; // 使用 NUnit 测试框架
using UnityEngine; // 使用 UnityEngine 命名空间
using UnityEngine.TestTools; // 使用 Unity Test Framework
using CircleOfLife.Key;
using System.Collections; // 使用 CircleOfLife.Key 命名空间
public class KeyboardSetTests
{
    [UnityTest]
    public IEnumerator TestKeyboardMapping()
    {
        // 测试键盘映射是否正确
        Assert.AreEqual(KeyCode.W, KeyboardSet.GetKeyCode(KeyEnum.Up));
        Assert.AreEqual(KeyCode.S, KeyboardSet.GetKeyCode(KeyEnum.Down));
        Assert.AreEqual(KeyCode.A, KeyboardSet.GetKeyCode(KeyEnum.Left));
        Assert.AreEqual(KeyCode.D, KeyboardSet.GetKeyCode(KeyEnum.Right));

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestChangeKey()
    {
        // 测试更改键位
        KeyboardSet.ChangeKey(KeyEnum.Up, KeyCode.L);
        Assert.AreEqual(KeyCode.L, KeyboardSet.GetKeyCode(KeyEnum.Up));
        KeyboardSet.ChangeKey(KeyEnum.Up, KeyCode.W);
        Assert.AreEqual(KeyCode.W, KeyboardSet.GetKeyCode(KeyEnum.Up));
        yield return null;
        // 测试重复键位是否被阻止
        KeyboardSet.ChangeKey(KeyEnum.Up, KeyCode.D);
        Assert.AreNotEqual(KeyCode.D, KeyboardSet.GetKeyCode(KeyEnum.Up));
        KeyboardSet.ChangeKey(KeyEnum.Up, KeyCode.W);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestResetKey()
    {
        // 测试重置键位
        KeyboardSet.ResetKey(KeyEnum.Up);
        Assert.AreEqual(KeyCode.None, KeyboardSet.GetKeyCode(KeyEnum.Up));

        yield return null;
    }
}

