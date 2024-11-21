using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace CircleOfLife.Atlas
{
    [CreateAssetMenu]
    public class AtlasSO : ScriptableObject
    {
        public List<AtlasData> Data = new();
    }
    
    [Serializable]
    public class AtlasData
    {
        public string Title;
        [Multiline]
        public string Description;
        public SkeletonDataAsset SkeletonData;
    }
}
