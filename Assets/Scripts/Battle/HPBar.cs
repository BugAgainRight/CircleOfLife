using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Battle;
using TMPro;
using UnityEngine;

namespace CircleOfLife
{
    public class HPBar : MonoBehaviour
    {
        public TMP_Text Level;
        public SpriteRenderer Fill;
        
        public BattleStats Stats;
        public BuildBase BindBuilding;

        public GameObject Container;

        public Sprite[] FillSprite;
        
        private float lastHP = -1f;
        private float lastMaxHP = -1f;
        private int lastLevel = -1;
        private float showTime = 0f;

        private Renderer render;
        private MeshFilter meshFilter;
        private float lastRotation = -1f;
        private Transform targetTrans;

        public void Initialize(BattleStats stats)
        {
            Stats = stats;
            if (stats.GameObject.TryGetComponent<BuildBase>(out var build))
            {
                BindBuilding = build;
            }

            Fill.sprite = FillSprite[(int)stats.BattleEntity.FactionType];

            render = stats.GameObject.GetComponent<Renderer>();
            stats.GameObject.TryGetComponent(out meshFilter);
            
            transform.SetParent(stats.Transform);

            targetTrans = stats.Transform;
            
            var scale = stats.Transform.localScale;
            transform.localScale = Vector3.one * (1f / scale.x);

            UpdatePosition();
            
            Container.SetActive(BindBuilding);
            Level.gameObject.SetActive(BindBuilding);
        }

        private void UpdatePosition()
        {
            if (lastRotation == targetTrans.localEulerAngles.z)
            {
                return;
            }

            lastRotation = targetTrans.localEulerAngles.z;
            
            transform.localEulerAngles = new Vector3(0f, 0f, -1f * targetTrans.localEulerAngles.z);

            var height = 0f;
            if (meshFilter)
            {
                var vertices = meshFilter.sharedMesh.vertices;
                var minY = float.MaxValue;
                var maxY = float.MinValue;
                foreach (var vertex in vertices)
                {
                    if (vertex.y < minY) minY = vertex.y;
                    if (vertex.y > maxY) maxY = vertex.y;
                }

                height = (maxY - minY) * 2f;
            }
            else
            {
                height = render.bounds.size.y;
            }
            
            var vertical = Mathf.RoundToInt(targetTrans.localEulerAngles.z) % 180 == 0;
            transform.localPosition = (vertical ?
                                        new Vector3(0f, height / 2f, 0f) :
                                        new Vector3(height / 2f, 0f, 0f));
        }
        
        private void UpdateHP()
        {
            if (lastHP == Stats.Current.Hp && lastMaxHP == Stats.Max.Hp)
            {
                return;
            }

            lastHP = Stats.Current.Hp;
            lastMaxHP = Mathf.Max(Stats.Max.Hp, 1f);

            Fill.size = new Vector2(5f * (lastHP / lastMaxHP), Fill.size.y);

            showTime = 1.5f;
            Container.SetActive(true);
        }

        private void UpdateLevel()
        {
            if (!BindBuilding)
            {
                return;
            }
            if (lastLevel == BindBuilding.Level)
            {
                return;
            }

            lastLevel = BindBuilding.Level;
            Level.text = "Lv. " + lastLevel;
        }
        
        private void Update()
        {
            UpdatePosition();
            UpdateHP();
            UpdateLevel();

            if (showTime > 0f && !BindBuilding)
            {
                showTime -= Time.deltaTime;
                if (showTime <= 0f)
                {
                    Container.SetActive(false);
                }
            }
        }
    }
}
