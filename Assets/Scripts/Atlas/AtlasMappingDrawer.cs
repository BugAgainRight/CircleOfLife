#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace CircleOfLife.Atlas
{
    [CustomPropertyDrawer(typeof(AtlasMapping))]
    public class AtlasMappingDrawer : PropertyDrawer
    {
        private float height;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            height = 0f;
            
            var typeProp = property.FindPropertyRelative("Type");
            height += EditorGUI.GetPropertyHeight(typeProp);
            position.height = height;
            
            EditorGUI.PropertyField(position, typeProp);
            
            position.y += height;

            var keyProp = property.FindPropertyRelative("Key");
            
            void drawEnumPopup<T>() where T : Enum
            {
                keyProp.intValue = (int)(object)EditorGUI.EnumPopup(position, "Target", (T)(object)keyProp.intValue);
            }
            
            switch (typeProp.intValue)
            {
                case (int)AtlasType.Enemy:
                    drawEnumPopup<EnemyStat>();
                    break;
                case (int)AtlasType.Animal:
                    drawEnumPopup<AnimalStat>();
                    break;
            }
            
            height += EditorGUI.GetPropertyHeight(keyProp);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return height;
        }
    }
}
#endif
