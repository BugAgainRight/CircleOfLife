using System;
using System.Collections;
using System.Collections.Generic;
using Milutools.Recycle;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CircleOfLife.Sample
{
    // 对象ID枚举，可以按情况分类；比如：子弹一个枚举，特效一个枚举 等。
    public enum MyID
    {
        Fire, Thunder
    }
    /// <summary>
    /// 对象池用法示例
    /// </summary>
    public class ObjectPoolSample : MonoBehaviour
    {
        // 预制体：这里为了演示方便放在这里，实际使用的时候怎么传预制体就是怎么方便怎么来
        public GameObject FirePrefab, ThunderPrefab;

        // 需要在使用这个物体前注册
        private void Awake()
        {
            // 参数：对象ID，预制体，最小物体个数，生命周期策略
            // *目标物体必须包含RecyclableObject组件
            // *最小物体个数：即一般情况下，这种物体会使用的个数。对象池的自动销毁多余物体会使用这个数值作为参考。
            // *生命周期策略：DestroyOnLoad - 仅在当前场景（默认），Eternity - 永久
            RecyclePool.EnsurePrefabRegistered(MyID.Fire, FirePrefab, 10, PoolLifeCyclePolicy.DestroyOnLoad);
            RecyclePool.EnsurePrefabRegistered(MyID.Thunder, ThunderPrefab, 10);
        }

        private void Update()
        {
            var pos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            if (Input.GetMouseButtonUp(0))
            {
                // 从对象池申请一个对象，第二个参数可选，第二个参数指定对象的父物体（默认是null）
                var go = RecyclePool.Request(MyID.Fire);
                go.transform.position = pos;
                go.SetActive(true);
            }
            else if (Input.GetMouseButtonUp(1))
            {
                // 另一种申请方法，可以申请到整个对象关联的信息，可以用来实现一些更复杂的逻辑
                var collection = RecyclePool.RequestWithCollection(MyID.Thunder);
                collection.Transform.position = pos;
                
                // 比如这里我们随机设置闪电中心的颜色，这里需要在 Inspector 把 ParticleSystem 设置成 RecyclableObject 的 MainComponent
                // 这个会在生成物体的时候提前缓存好组件引用，相比 GameObject.GetComponent 会快一些
                var particleMain = collection.GetMainComponent<ParticleSystem>().main;
                particleMain.startColor =
                    new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                
                collection.GameObject.SetActive(true);
                
                // 需要对其多个组件操作时，也可以用 collection.GetComponent<T>() 获得组件，
                // 但是这些组件必须在 Inspector 中放置在 RecyclableObject 的 Components 列表。
            }
        }
    }
}
