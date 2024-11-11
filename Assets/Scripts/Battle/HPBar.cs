using System;
using System.Collections;
using System.Collections.Generic;
using CircleOfLife.Battle;
using CircleOfLife.Utils;
using Milease.Enums;
using Milease.Utils;
using TMPro;
using UnityEngine;

namespace CircleOfLife
{
    public class HPBar : MonoBehaviour
    {
        public TMP_Text Level;
        public SpriteRenderer Fill, LateFill;
        
        public BattleStats Stats;
        public BuildBase BindBuilding;

        public GameObject Container;

        public Sprite[] FillSprite;
        
        private float lastHP = -1f;
        private float lastMaxHP = -1f;
        private int lastLevel = -1;
        private float showTime = 0f;

        private bool isPlayer;

        private Renderer render;
        private MeshFilter meshFilter;
        private float lastRotation = -1f;
        private Transform targetTrans;

        private float startHP, endHP, animatedTime = 0.5f;

        private Vector3 direction;

        public void Initialize(BattleStats stats)
        {
            Stats = stats;
            if (stats.GameObject.TryGetComponent<BuildBase>(out var build))
            {
                BindBuilding = build;
            }

            isPlayer = stats.GameObject.IsPlayer() || stats.GameObject.CompareTag("ProtectAnimal");

            Fill.sprite = FillSprite[(int)stats.BattleEntity.FactionType];
            LateFill.sprite = Fill.sprite;

            render = stats.GameObject.GetComponent<Renderer>();
            stats.GameObject.TryGetComponent(out meshFilter);
            
            transform.SetParent(stats.Transform);

            targetTrans = stats.Transform;
            
            var scale = stats.Transform.localScale;
            transform.localScale = Vector3.one * (1f / scale.x);

            UpdatePosition();
            
            Container.SetActive(BindBuilding || isPlayer);
            Level.gameObject.SetActive(BindBuilding);
        }

        private void OnEnable()
        {
            lastHP = -1f;
            Fill.size = new Vector2(5f, Fill.size.y);
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
            lastMaxHP = Stats.Max.Hp;

            Fill.size = new Vector2(5f * (lastHP / Mathf.Max(lastMaxHP, 1f)), Fill.size.y);

            showTime = 1.5f;
            Container.SetActive(true);

            startHP = LateFill.size.x;
            endHP = Fill.size.x;
            animatedTime = 0f;
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

            if (targetTrans.localScale != direction)
            {
                direction = targetTrans.localScale;
                var scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
                transform.localScale = scale;
            }

            if (animatedTime < 0.5f)
            {
                animatedTime += Time.deltaTime;
                var pro = EaseUtility.GetEasedProgress(animatedTime, 0.5f, EaseType.In, EaseFunction.Quad);
                LateFill.size = new Vector2(startHP + (endHP - startHP) * pro, LateFill.size.y);
            }

            if (showTime > 0f && !BindBuilding && !isPlayer)
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
