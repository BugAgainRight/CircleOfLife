using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleOfLife
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;
        public List<Transform> EntityAppearPoint;
        public LevelSO LevelInfo;
        public string LevelID;

        public static LevelManager GetInstance()
        {
            return Instance;
        }
        private void Awake()
        {
            Instance = this;
        }
        public void LoadLevel()
        {
            //加载关卡
        }
        public void UnLoadLevel()
        {
            //卸载关卡
        }
        public LevelContext GetLevelContext()
        {
            foreach (LevelContext l in LevelInfo.LevelContexts)
            {
                if (l.ID == LevelID) return l;
            }
            Debug.LogWarning("Countn't find LevelID");
            return null;
        }
    }
}
