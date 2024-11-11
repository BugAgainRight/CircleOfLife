#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace CircleOfLife.Level
{
    [CustomPropertyDrawer(typeof(LevelEnemy))]
    public class LevelEnemyDrawer : PropertyDrawer
    {
        // 设置每个字段的间距
        private const float padding = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 计算各个字段的位置
            float fullWidth = position.width;
            float singleWidth = (fullWidth - 2 * padding) / 3;

            // 取得各个字段
            SerializedProperty enemyProp = property.FindPropertyRelative("Enemy");
            SerializedProperty summonCountProp = property.FindPropertyRelative("SummonCount");
            SerializedProperty appearPointsProp = property.FindPropertyRelative("AppearPoints");

            // 绘制 Enemy 字段
            Rect enemyRect = new Rect(position.x, position.y, singleWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(enemyRect, enemyProp, GUIContent.none);

            // 绘制 SummonCount 字段
            Rect summonCountRect = new Rect(position.x + singleWidth + padding, position.y, singleWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(summonCountRect, summonCountProp, GUIContent.none);

            // 绘制 AppearPoints 字段
            Rect appearPointsRect = new Rect(position.x + 2 * (singleWidth + padding), position.y, singleWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(appearPointsRect, appearPointsProp, appearPointsProp.arraySize switch
            {
                0 => new GUIContent("任意方位"),
                1 => new GUIContent(appearPointsProp.GetArrayElementAtIndex(0).enumDisplayNames
                    [appearPointsProp.GetArrayElementAtIndex(0).enumValueIndex]),
                _ => new GUIContent($"{appearPointsProp.arraySize}抽1")
            });

            // 如果 AppearPoints 展开了，根据元素数量绘制额外高度
            if (appearPointsProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                position.y += 5f;
                for (int i = 0; i < appearPointsProp.arraySize; i++)
                {
                    SerializedProperty element = appearPointsProp.GetArrayElementAtIndex(i);
                    position.y += EditorGUIUtility.singleLineHeight + padding;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, fullWidth, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + padding;

            SerializedProperty appearPointsProp = property.FindPropertyRelative("AppearPoints");
            if (appearPointsProp.isExpanded)
            {
                // 计算 AppearPoints 的高度
                height += (Math.Max(appearPointsProp.arraySize, 1) * (EditorGUIUtility.singleLineHeight + padding)) 
                          + (EditorGUIUtility.singleLineHeight + 15f);
            }

            return height;
        }
    }

    [CustomEditor(typeof(LevelScriptableObject))]
    public class LevelScriptableObjectEditor : Editor
    {
        SerializedProperty roundsProp;

        private void OnEnable()
        {
            roundsProp = serializedObject.FindProperty("Rounds");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var materialProp = serializedObject.FindProperty("InitialMaterial");
            EditorGUILayout.PropertyField(materialProp);
            
            EditorGUILayout.LabelField("所有回合", EditorStyles.boldLabel);

            for (int i = 0; i < roundsProp.arraySize; i++)
            {
                if (i >= roundsProp.arraySize)
                {
                    break;
                }
                SerializedProperty roundProp = roundsProp.GetArrayElementAtIndex(i);

                // 绘制 LevelRound 分组标题
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"回合 {i + 1}", EditorStyles.boldLabel);
                if (EditorGUILayout.LinkButton("⚠ 删除该回合..."))
                {
                    roundsProp.DeleteArrayElementAtIndex(i);
                    i--;
                    continue;
                }
                
                var plotProp = roundProp.FindPropertyRelative("BeforePlot");
                EditorGUILayout.PropertyField(plotProp);
                var skipProp = roundProp.FindPropertyRelative("SkipBuildPlace");
                EditorGUILayout.PropertyField(skipProp);
                
                // 获取 Waves 列表
                SerializedProperty wavesProp = roundProp.FindPropertyRelative("Waves");

                // 绘制 LevelWave 内容（平铺展示，不缩进）
                EditorGUILayout.PropertyField(wavesProp);
                
                EditorGUILayout.EndVertical();
            }
            
            if (GUILayout.Button("+ 添加回合"))
            {
                roundsProp.InsertArrayElementAtIndex(roundsProp.arraySize);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
