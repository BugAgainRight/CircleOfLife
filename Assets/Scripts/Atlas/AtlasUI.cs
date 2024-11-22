using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Milutools.Milutools.UI;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace CircleOfLife.Atlas
{
    public class AtlasUIData
    {
        public AtlasData Atlas;
        public bool Unlocked;
    }
    public class AtlasUI : ManagedUI<AtlasUI, List<AtlasUIData>>
    {
        public static AtlasUI Instance;
        
        public SkeletonGraphic SkeletonGraphic;
        public TMP_Text Title, Description;
        public GameObject ItemPrefab;
        
        public static void Open()
        {
            Open(AtlasManager.Data.Select(x => new AtlasUIData()
            {
                Atlas = x,
                Unlocked = x.Mapping.Type switch
                {
                    AtlasType.Enemy => SaveManagement.UseSaveData.IsUnlocked((EnemyStat)x.Mapping.Key),
                    AtlasType.Animal => SaveManagement.UseSaveData.IsUnlocked((AnimalStat)x.Mapping.Key),
                    _ => throw new ArgumentOutOfRangeException()
                }
            }).ToList());
        }
        
        protected override void Begin()
        {
            
        }

        protected override void AboutToClose()
        {

        }

        public override void AboutToOpen(List<AtlasUIData> parameter)
        {
            Instance = this;
            var selected = false;
            foreach (var item in parameter)
            {
                var go = Instantiate(ItemPrefab, ItemPrefab.transform.parent);
                go.SetActive(true);
                var listItem = go.GetComponent<AtlasListItem>();
                listItem.SkeletonGraphic.skeletonDataAsset = item.Atlas.SkeletonData;
                listItem.SkeletonGraphic.color = item.Unlocked ? Color.white : Color.black;
                listItem.Data = item;
                go.SetActive(true);
                if (!selected)
                {
                    selected = true;
                    listItem.Select();
                }
            }
        }
    }
}
