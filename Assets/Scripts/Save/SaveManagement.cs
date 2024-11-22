using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CircleOfLife
{
    public class SaveManagement : MonoBehaviour
    {
        public static SaveManagement Instance;
        private static SaveCombine saveData;
        public static SaveCombine.SaveData UseSaveData;

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            Debug.Log("存档位置:" + Path.Combine(Application.persistentDataPath, "SaveData.json"));
            if (!File.Exists(Path.Combine(Application.persistentDataPath, "SaveData.json")))
            {
                saveData = new SaveCombine();
                saveData.AllSaveData = new() { null,null,null,null};

                File.WriteAllText(Path.Combine(Application.persistentDataPath, "SaveData.json"),
                    JsonConvert.SerializeObject(saveData));
            }
            saveData = JsonConvert.DeserializeObject<SaveCombine>(
                File.ReadAllText(Path.Combine(Application.persistentDataPath, "SaveData.json")));
          
        }

        /// <summary>
        /// 保存saveData数据
        /// </summary>
        public static void Save(int index)
        {
            UseSaveData.LastSaveDate = DateTime.Now;
            if (index < 4)
            {
                saveData.AllSaveData[index] ??= new SaveCombine.SaveData();
                saveData.AllSaveData[index].Copy(UseSaveData);
            }
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "SaveData.json"),
                JsonConvert.SerializeObject(saveData));
        }

        /// <summary>
        /// 选择存档，
        /// </summary>
        /// <param name="index"></param>
        public static void SelectSaveData(int index)
        {
            if (index < 0)
            {
                UseSaveData = new SaveCombine.SaveData();
            }
            else if (index < 4)
            {
                UseSaveData = new SaveCombine.SaveData();
                if (saveData.AllSaveData[index] != null) UseSaveData.Copy(saveData.AllSaveData[index]);
            }
        }


        /// <summary>
        /// 获取所有存档信息
        /// </summary>
        /// <returns></returns>
        public static List<SaveCombine.SaveData> GetAllSaveData()
        {
            return saveData.AllSaveData;
        }

        private void FixedUpdate()
        {

            if (UseSaveData!=null)
            {
                UseSaveData.AddTimer();
            }

        }

    }

    [Serializable]
    public class SaveCombine
    {
        [Serializable]
        public class SaveData
        {
            public string UserName;
            public DateTime LastSaveDate;
            public int CurrentDay;
            
            [JsonProperty]
            private long timer;
            [JsonProperty]
            private float midTimer;
            [JsonIgnore]
            public TimeSpan Timer => TimeSpan.FromSeconds(timer);


            ////////////其他存档数据




            ////////////
            

            public void AddTimer()
            {
                midTimer += Time.fixedDeltaTime;
                if (midTimer >= 1)
                {
                    timer += (int)midTimer;
                    midTimer -= (int)midTimer;
                }
            }
            public void Copy(SaveData other)
            {
                timer=other.timer;
                midTimer = other.midTimer;
            }

        }
        public List<SaveData> AllSaveData = new();

    }
}
