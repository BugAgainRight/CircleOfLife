using CircleOfLife;
using CircleOfLife.ScriptObject;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RuiRuiTool
{

    #region 编辑器依赖

    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class CustomEditorManager : Editor
    {
        private List<CustomEditorSelector> Editors = new();

        private void OnEnable()
        {
            Editors.Add(new NewGizmos());
            Editors.Add(new NewBuffShowDataEditorSo());
            Editors.Add(new AllPrefabSettingEditor<SharedPrefab, GameObject>());
            Editors.Add(new AllPrefabSettingEditor<AnimalStat, GameObject>());
            Editors.Add(new AllPrefabSettingEditor<EnemyStat, GameObject>());
            Editors.Add(new AllPrefabSettingEditor<AnimatonPrefab, GameObject>());
            Editors.Add(new AllPrefabSettingEditor<EnemySkillType, GameObject>());
            Editors.Add(new AllPrefabSettingEditor<PlayerSkillType, GameObject>());
            Editors.Add(new AllPrefabSettingEditor<BuildSkillType, GameObject>());
            Editors.Add(new AllPrefabSettingEditor<AnimalSkillType, GameObject>());
            Editors.Add(new AllPrefabSettingEditor<BuildStat,BuildSoData>());
            foreach (var e in Editors)
                e.OnEnable(target, this);
        }

        private void OnDisable()
        {
            foreach (var e in Editors)
                e.OnDisable(target, this);
            Editors.Clear();

        }

        private void OnSceneGUI()
        {
            foreach (var e in Editors)
            {
                if (e.IsMatch(target, this))
                {
                    e.OnSceneGUI(target, this);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            var has = false;
            foreach (var e in Editors)
            {
                if (e.IsMatch(target, this))
                {
                    e.OnInspectorGUI(target, this);
                    has = true;
                }
            }
            //if (!has)
                base.OnInspectorGUI();
        }

        public override bool RequiresConstantRepaint()
        {
            var ret = false;
            foreach (var e in Editors)
                ret |= e.RequiresConstantRepaint(target, this);
            return ret;
        }
    }

    [InitializeOnLoad]
    public static class EditorFramework
    {
        internal static bool needToRepaint;

        internal static Event currentEvent;
        internal static float t;

        static EditorFramework()
        {
            EditorApplication.update += Updating;
        }


        static void Updating()
        {
            CheckMouse();

            if (needToRepaint)
            {
                t += Time.deltaTime;

                if (t >= 0.3f)
                {
                    t -= 0.3f;
                    needToRepaint = false;
                }
            }
        }

        static void CheckMouse()
        {
            var ev = currentEvent;
            if (ev == null) return;

            if (ev.type == EventType.MouseMove)
                needToRepaint = true;
        }
    }


    public delegate bool SearchMatch(SerializedProperty serializedProperty, string search);
    public delegate void AddMatch(SerializedProperty serializedProperty);
    public delegate void AutoUpdateMatch(SerializedProperty serializedProperty);
    public delegate string ForeachDoMatch(SerializedProperty serializedProperty);
    public interface IUseInspectorEditorTool
    {
        public bool SearchMatch(SerializedProperty serializedProperty, string search);
        public void AddMatch(SerializedProperty serializedProperty);
        public void AutoUpdateMatch(SerializedProperty serializedProperty);
        public string ForeachDoFunc(SerializedProperty serializedProperty);

    }

    public interface CustomEditorSelector
    {
        public bool IsMatch(object target, Editor editor);
        public void OnInspectorGUI(object target, Editor editor);
        public void OnSceneGUI(object target, Editor editor);
        public void OnEnable(object target, Editor editor);
        public void OnDisable(object target, Editor editor);
        public bool RequiresConstantRepaint(object target, Editor editor);
    }

    public class InspectorArrayEditorTool : Editor
    {
        private int deleteIndex;
        public void DeleteArrayAtIndex(SerializedProperty serializedProperty)
        {
            EditorGUILayout.BeginHorizontal();
            {
                deleteIndex = EditorGUILayout.IntField("根据index删除", deleteIndex);
                if (GUILayout.Button("删除"))
                {
                    if (deleteIndex < 0) deleteIndex = 0;
                    if (deleteIndex >= serializedProperty.arraySize) deleteIndex = serializedProperty.arraySize - 1;
                    serializedProperty.DeleteArrayElementAtIndex(deleteIndex);
                    AssetDatabase.SaveAssets();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private int deleteCount;
        public void DeleteByCount(SerializedProperty serializedProperty)
        {
            EditorGUILayout.BeginHorizontal();
            {
                deleteCount = EditorGUILayout.IntField("减少数量", deleteCount);
                deleteCount = deleteCount > 0 ? deleteCount : 1;
                if (GUILayout.Button("减少"))
                {
                    if (serializedProperty.arraySize - deleteCount > 0) serializedProperty.arraySize -= deleteCount;
                    else serializedProperty.arraySize = 0;
                    AssetDatabase.SaveAssets();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        string search;
        Vector2 searchScroll;

        int addCount;
        public void SearchAndShow(SerializedProperty serializedProperty, SearchMatch match)
        {

            search = EditorGUILayout.TextField("我是搜索框", search);


            searchScroll = EditorGUILayout.BeginScrollView(searchScroll);
            {
               
                for (int i = 0; i < serializedProperty.arraySize; i++)
                {
                    SerializedProperty element = serializedProperty.GetArrayElementAtIndex(i);
                    if (string.IsNullOrEmpty(search) || match(element, search))
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(i.ToString(), GUILayout.Width(30));
                        EditorGUILayout.PropertyField(element, GUIContent.none);
                        EditorGUILayout.EndHorizontal();
                    }

                }


            }
            EditorGUILayout.EndScrollView();

        }

        public void AddArray(SerializedProperty serializedProperty, AddMatch match = null)
        {
            EditorGUILayout.BeginHorizontal();
            {
                addCount = EditorGUILayout.IntField("添加数量", addCount);
                addCount = addCount > 0 ? addCount : 1;
                if (GUILayout.Button("添加"))
                {
                    serializedProperty.arraySize += addCount;
                    AssetDatabase.SaveAssets();
                }
                if (match != null && GUILayout.Button("根据选择添加"))
                {
                    match(serializedProperty);
                    AssetDatabase.SaveAssets();
                }
            }
            EditorGUILayout.EndHorizontal();
        }





        public void AutoUpdate(SerializedProperty serializedProperty, AutoUpdateMatch match)
        {
            if (GUILayout.Button("自动更新赋值"))
            {
                match(serializedProperty);
                EditorUtility.SetDirty(serializedProperty.serializedObject.targetObject);
                AssetDatabase.SaveAssets();
            }
        }
        #region 废弃
        /// <summary>
        /// 特供two，three value 且value1为string使用
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <exception cref="System.Exception"></exception>
        //public void SortArray<T1>(SerializedProperty serializedProperty)where T1:Object
        //{

        //    if (!serializedProperty.isArray) throw new System.Exception($"{serializedProperty.name}不是Array!");

        //    List<(string,T1)> tempList = new();
        //    for(int i=0;i< serializedProperty.arraySize;i++ )
        //    {
        //        tempList.Add((serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("value1").stringValue, serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("value2").objectReferenceValue as T1));
        //    }

        //    tempList.OrderBy(x=>x.Item1).ToList();

        //    serializedProperty.ClearArray();
        //    for (int i = 0; i < tempList.Count; i++)
        //    {
        //        serializedProperty.InsertArrayElementAtIndex(i);
        //        SerializedProperty element = serializedProperty.GetArrayElementAtIndex(i);
        //        element.FindPropertyRelative("value1").stringValue = tempList[i].Item1;
        //        element.FindPropertyRelative("value2").objectReferenceValue = tempList[i].Item2;
        //    }
        //}
        //public void SortArray<T1,T2>(SerializedProperty serializedProperty) where T1 : Object where T2 : Object
        //{

        //    if (!serializedProperty.isArray) throw new System.Exception($"{serializedProperty.name}不是Array!");

        //    List<(string, T1,T2)> tempList = new();
        //    for (int i = 0; i < serializedProperty.arraySize; i++)
        //    {
        //        tempList.Add((serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("value1").stringValue, serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("value2").objectReferenceValue as T1, serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("value3").objectReferenceValue as T2));
        //    }

        //    tempList.OrderBy(x => x.Item1).ToList();

        //    serializedProperty.ClearArray();
        //    for (int i = 0; i < tempList.Count; i++)
        //    {
        //        serializedProperty.InsertArrayElementAtIndex(i);
        //        SerializedProperty element = serializedProperty.GetArrayElementAtIndex(i);
        //        element.FindPropertyRelative("value1").stringValue = tempList[i].Item1;
        //        element.FindPropertyRelative("value2").objectReferenceValue = tempList[i].Item2;
        //        element.FindPropertyRelative("value3").objectReferenceValue = tempList[i].Item3;
        //    }
        //}




        //public void SortArray<T1>(SerializedProperty serializedProperty) where T1 : Object
        //{
        //    SortArrayInternal<T1, Object>(serializedProperty,
        //        (element) => (element.FindPropertyRelative("value1").stringValue,
        //                      element.FindPropertyRelative("value2").objectReferenceValue as T1,
        //                      null));
        //}

        //public void SortArray<T1, T2>(SerializedProperty serializedProperty) where T1 : Object where T2 : Object
        //{
        //    SortArrayInternal<T1, T2>(serializedProperty,
        //        (element) => (element.FindPropertyRelative("value1").stringValue,
        //                      element.FindPropertyRelative("value2").objectReferenceValue as T1,
        //                      element.FindPropertyRelative("value3").objectReferenceValue as T2));
        //}

        //private void SortArrayInternal<T1, T2>(SerializedProperty serializedProperty, System.Func<SerializedProperty, (string, T1, T2)> extractValues) where T1 : Object where T2 : Object
        //{
        //    if (!serializedProperty.isArray) throw new System.Exception($"{serializedProperty.name} 不是 Array!");

        //    List<(string, T1, T2)> tempList = new();
        //    for (int i = 0; i < serializedProperty.arraySize; i++)
        //    {
        //        SerializedProperty element = serializedProperty.GetArrayElementAtIndex(i);
        //        tempList.Add(extractValues(element));
        //    }

        //    tempList = tempList.OrderBy(x => x.Item1).ToList();

        //    serializedProperty.ClearArray();
        //    for (int i = 0; i < tempList.Count; i++)
        //    {
        //        serializedProperty.InsertArrayElementAtIndex(i);
        //        SerializedProperty element = serializedProperty.GetArrayElementAtIndex(i);
        //        element.FindPropertyRelative("value1").stringValue = tempList[i].Item1;
        //        element.FindPropertyRelative("value2").objectReferenceValue = tempList[i].Item2;
        //        if (tempList[i].Item3 != null)
        //        {
        //            element.FindPropertyRelative("value3").objectReferenceValue = tempList[i].Item3;
        //        }
        //    }
        //}
        #endregion

        /// <summary>
        /// 根据第一个值排序
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyNames"></param>
        /// <exception cref="System.Exception"></exception>
        public void SortArray(SerializedProperty serializedProperty, params string[] propertyNames)
        {
            if (GUILayout.Button("根据第一个元素排序"))
            {
                if (!serializedProperty.isArray) throw new System.Exception($"{serializedProperty.name} 不是 Array!");
                if (propertyNames.Length == 0) throw new System.Exception($"传入参数错误!");

                var tempList = new List<Dictionary<string, object>>();

                // 读取 SerializedProperty 中的元素到临时列表
                for (int i = 0; i < serializedProperty.arraySize; i++)
                {
                    var element = serializedProperty.GetArrayElementAtIndex(i);
                    var elementData = new Dictionary<string, object>();

                    foreach (var propertyName in propertyNames)
                    {
                        var property = element.FindPropertyRelative(propertyName);
                        if (property != null)
                        {
                            if (property.propertyType == SerializedPropertyType.Enum)
                            {
                                elementData[propertyName] = property.enumValueIndex;
                            }
                            else if (property.propertyType == SerializedPropertyType.String)
                            {
                                elementData[propertyName] = property.stringValue;
                            }
                            else if (property.propertyType == SerializedPropertyType.ObjectReference)
                            {
                                elementData[propertyName] = property.objectReferenceValue;
                            }
                            // 你可以在这里添加对其他类型的处理
                        }
                    }

                    tempList.Add(elementData);
                }

                // 对临时列表进行排序（按第一个属性）
                tempList = tempList.OrderBy(x => x[propertyNames[0]]).ToList();

                // 清空 SerializedProperty 中的原始元素
                serializedProperty.ClearArray();

                // 将排序后的元素重新添加到 SerializedProperty
                for (int i = 0; i < tempList.Count; i++)
                {
                    serializedProperty.InsertArrayElementAtIndex(i);
                    var element = serializedProperty.GetArrayElementAtIndex(i);
                    foreach (var propertyName in propertyNames)
                    {
                        var property = element.FindPropertyRelative(propertyName);
                        if (property != null)
                        {
                            if (property.propertyType == SerializedPropertyType.Enum)
                            {
                                property.enumValueIndex = (int)tempList[i][propertyName];
                            }
                            else if (property.propertyType == SerializedPropertyType.String)
                            {
                                property.stringValue = tempList[i][propertyName] as string;
                            }
                            else if (property.propertyType == SerializedPropertyType.ObjectReference)
                            {
                                property.objectReferenceValue = tempList[i][propertyName] as Object;
                            }
                            // 你可以在这里添加对其他类型的处理
                        }
                    }
                }
                AssetDatabase.SaveAssets();
            }

        }


        /// <summary>
        /// 根据第一个值排序
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyNames"></param>
        /// <exception cref="System.Exception"></exception>
        public void SortArrayImmediately(SerializedProperty serializedProperty, params string[] propertyNames)
        {
            if (!serializedProperty.isArray) throw new System.Exception($"{serializedProperty.name} 不是 Array!");
            if (propertyNames.Length == 0) throw new System.Exception($"传入参数错误!");

            var tempList = new List<Dictionary<string, object>>();

            // 读取 SerializedProperty 中的元素到临时列表
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                var element = serializedProperty.GetArrayElementAtIndex(i);
                var elementData = new Dictionary<string, object>();
                foreach (var propertyName in propertyNames)
                {
                 
                    var property = element.FindPropertyRelative(propertyName);
                    if (property != null)
                    {
                 
                        if (property.propertyType == SerializedPropertyType.Enum)
                        {
                            elementData[propertyName] = property.enumValueIndex;
                        }
                        else if (property.propertyType == SerializedPropertyType.String)
                        {
                            elementData[propertyName] = property.stringValue;
                        }
                        else if (property.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            elementData[propertyName] = property.objectReferenceValue;
                          
                        }else if (property.propertyType == SerializedPropertyType.Generic)
                        {
                            elementData[propertyName] = property.boxedValue;
                        }
                        // 你可以在这里添加对其他类型的处理
                    }
                }

                tempList.Add(elementData);
            }

            // 对临时列表进行排序（按第一个属性）
            tempList = tempList.OrderBy(x => x[propertyNames[0]]).ToList();

            // 清空 SerializedProperty 中的原始元素
            serializedProperty.ClearArray();

            // 将排序后的元素重新添加到 SerializedProperty
            for (int i = 0; i < tempList.Count; i++)
            {
                serializedProperty.InsertArrayElementAtIndex(i);
                var element = serializedProperty.GetArrayElementAtIndex(i);
                foreach (var propertyName in propertyNames)
                {
                    var property = element.FindPropertyRelative(propertyName);
                    if (property != null)
                    {
                        if (property.propertyType == SerializedPropertyType.Enum)
                        {
                            property.enumValueIndex = (int)tempList[i][propertyName];
                        }
                        else if (property.propertyType == SerializedPropertyType.String)
                        {
                            property.stringValue = tempList[i][propertyName] as string;
                        }
                        else if (property.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            property.objectReferenceValue = tempList[i][propertyName] as Object;
                        }else if(property.propertyType == SerializedPropertyType.Generic)
                        {
                            property.boxedValue = tempList[i][propertyName];
                        }
                        // 你可以在这里添加对其他类型的处理
                    }
                }
            }
            AssetDatabase.SaveAssets();


        }

        public static JsonSerializerSettings jsonSetting = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

        };

        /// <summary>
        /// 传入的委托返回修改后的日志，该函数会生成一个日志文件夹存放过往数据
        /// </summary>
        /// <param name="label"></param>
        /// <param name="serializedProperty"></param>
        /// <param name="match"></param>
        /// <exception cref="System.Exception"></exception>
        public void ForeachDo(string label, SerializedProperty serializedProperty, ForeachDoMatch match)
        {
            if (!serializedProperty.isArray) throw new System.Exception($"ForeachDo中传入参数{serializedProperty.name} 不是 Array!");
            List<string> dialog = new();
            if (GUILayout.Button(label))
            {
                for (int i = 0; i < serializedProperty.arraySize; i++)
                {
                    SerializedProperty element = serializedProperty.GetArrayElementAtIndex(i);
                    dialog.Add(match(element));
                }
                string path = "Assets/Resources/SoFileDialogs/" + serializedProperty.serializedObject.targetObject.name;

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                int version = 0;
                if (File.Exists(path + "/Version"))
                {
                    version = JsonConvert.DeserializeObject<int>(File.ReadAllText(path + "/Version"), jsonSetting);
                }
                version++;
                File.WriteAllText(path + "/" + version.ToString(), JsonConvert.SerializeObject(dialog, jsonSetting));
                File.WriteAllText(path + "/Version", JsonConvert.SerializeObject(version));
                //AssetDatabase.GetAssetPath();
                Debug.Log($"{serializedProperty.serializedObject.targetObject.name}   ForeachDo操作成功!");
            }

        }


    }


    #endregion


    [CustomPropertyDrawer(typeof(TwoValue<,>))]
    public class TwoValueDrawer : PropertyDrawer
    {
        private float propH;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect position2 = position;
            
            position.width /= 2;
            position2.width /= 2;

            position2.x = position.x + position2.width;

            EditorGUI.BeginProperty(position, label, property);
            AddProperty("value1", position,property);
            AddProperty("value2", position2,property);
           
            //EditorGUI.PropertyField(position, property.FindPropertyRelative("value1"));
            //EditorGUI.PropertyField(position2, property.FindPropertyRelative("value2"));

            EditorGUI.EndProperty();
        }
        private void AddProperty(string name, Rect rect, SerializedProperty property)
        {
            
            var sp_name = property.FindPropertyRelative(name);
            rect.height = EditorGUI.GetPropertyHeight(sp_name);
            EditorGUI.PropertyField(rect, sp_name,GUIContent.none);
            propH =Mathf.Max(propH, EditorGUI.GetPropertyHeight(sp_name));
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return propH;
        }
    }

    [CustomPropertyDrawer(typeof(BuildSoData))]
    public class BuildPrefabDrawer : PropertyDrawer
    {
        float propH = EditorGUIUtility.singleLineHeight;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            #region 注释
            //EditorGUILayout.BeginVertical();
            //{
            //    EditorGUI.BeginProperty(position, label, property);
            //    {
            //        EditorGUILayout.BeginHorizontal();
            //        {
            //            GUILayout.Label("Prefab");
            //            EditorGUILayout.PropertyField(property.FindPropertyRelative("Prefab"), GUIContent.none);
            //        }
            //        EditorGUILayout.EndHorizontal();

            //        EditorGUILayout.BeginHorizontal();
            //        {
            //            GUILayout.Label("Icon");
            //            EditorGUILayout.PropertyField(property.FindPropertyRelative("Icon"), GUIContent.none);
            //        }
            //        EditorGUILayout.EndHorizontal();

            //        EditorGUILayout.BeginHorizontal();
            //        {
            //            GUILayout.Label("BuildSize");
            //            EditorGUILayout.PropertyField(property.FindPropertyRelative("BuildSize"), GUIContent.none);
            //        }
            //        EditorGUILayout.EndHorizontal();

            //        EditorGUILayout.BeginHorizontal();
            //        {
            //            GUILayout.Label("Cost");
            //            EditorGUILayout.PropertyField(property.FindPropertyRelative("Cost"), GUIContent.none);
            //        }
            //        EditorGUILayout.EndHorizontal();

            //        EditorGUILayout.BeginHorizontal();
            //        {
            //            GUILayout.Label("WhetherRotate");
            //            EditorGUILayout.PropertyField(property.FindPropertyRelative("WhetherRotate"), GUIContent.none);
            //        }
            //        EditorGUILayout.EndHorizontal();

            //        EditorGUILayout.BeginHorizontal();
            //        {
            //            GUILayout.Label("Name");
            //            EditorGUILayout.PropertyField(property.FindPropertyRelative("Name"), GUIContent.none);
            //        }
            //        EditorGUILayout.EndHorizontal();


            //    }
            //    EditorGUI.EndProperty();
            //}
            //EditorGUILayout.EndVertical();
            #endregion

            var Space_Height = 2;

            var rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = position.width - 10; //右边距
        
            AddProperty("Prefab", ref rect, property, Space_Height);
            AddProperty("Icon", ref rect, property, Space_Height);
            

            
            var iconTexture = AssetPreview.GetAssetPreview(property.FindPropertyRelative("Icon").objectReferenceValue);
            if (iconTexture != null)
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                float showH, showW;
                showH = iconTexture.height;
                showW = iconTexture.width;

                float smallRate = Mathf.Max(showH, showW) / 50f;
                showH /= smallRate;
                showW /= smallRate;

                EditorGUI.DrawPreviewTexture(new Rect(rect) { width = showW, height = showH }, iconTexture);
                rect.y += showH;
            }          
            AddProperty("BuildSize", ref rect, property, Space_Height);
            AddProperty("Cost", ref rect, property, Space_Height);
            AddProperty("WhetherRotate", ref rect, property, Space_Height);
            AddProperty("Name", ref rect, property, Space_Height);

            propH = rect.y - position.y + EditorGUIUtility.singleLineHeight; //3行+3行space

        }


        private void AddProperty(string name,ref Rect rect,SerializedProperty property,float Space_Height)
        {
            rect.y += EditorGUIUtility.singleLineHeight + Space_Height; //第3行
            Rect label=rect, content=rect;
            label.width = rect.width * 0.2f;
            content.width = rect.width * 0.8f;
            content.position += Vector2.right * label.width;
            var sp_name = property.FindPropertyRelative(name);
            EditorGUI.LabelField(label, name);
            EditorGUI.PropertyField(content, sp_name,GUIContent.none);
        }
    

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return propH;
        }

    }





    public class NewBuffShowDataEditorSo : CustomEditorSelector, IUseInspectorEditorTool
    {
        Object target;
        TestSo_Chen newTestSo;
        InspectorArrayEditorTool editorTool, editorTool2, editorTool3;
        public void AddMatch(SerializedProperty serializedProperty)
        {
            throw new System.NotImplementedException();
        }

        public void AutoUpdateMatch(SerializedProperty serializedProperty)
        {
            throw new System.NotImplementedException();
        }

        public string ForeachDoFunc(SerializedProperty serializedProperty)
        {
            throw new System.NotImplementedException();
        }

        public bool IsMatch(object target, Editor editor)
        {
            return target.GetType() == typeof(TestSo_Chen);
        }

        public void OnDisable(object target, Editor editor)
        {

        }

        public void OnEnable(object target, Editor editor)
        {
            this.target = (Object)target;
            if (target.GetType() == typeof(TestSo_Chen))
            {
                newTestSo = (TestSo_Chen)target;
            }
            editorTool = new();
            editorTool2 = new();
            editorTool3 = new();
        }

        bool allPlayerBool;
        bool allEnemyBool;
        bool allBuildBool;
        public void OnInspectorGUI(object target, Editor editor)
        {
            SerializedObject serializedObject = new SerializedObject(newTestSo);

            SerializedProperty serializedProperty_Player = serializedObject.FindProperty("allPlayerSkill");
            SerializedProperty serializedProperty_Enemy = serializedObject.FindProperty("allEnemySkill");
            SerializedProperty serializedProperty_Build = serializedObject.FindProperty("allBuildSkill");
            SerializedProperty serializedProperty_Dictionary = serializedObject.FindProperty("allSkills");

            allPlayerBool = EditorGUILayout.Foldout(allPlayerBool, "PlayerSkillType");
            if (allPlayerBool)
            {
                editorTool.SearchAndShow(serializedProperty_Player, SearchMatch);
                editorTool.AddArray(serializedProperty_Player);
                editorTool.DeleteByCount(serializedProperty_Player);
                editorTool.DeleteArrayAtIndex(serializedProperty_Player);
                editorTool.SortArray(serializedProperty_Player, "value1", "value2");
                if (GUILayout.Button("添加全部枚举"))
                {
                    editorTool.SortArrayImmediately(serializedProperty_Player, "value1", "value2");
                    serializedProperty_Player.arraySize = typeof(PlayerSkillType).Length();
                    for (int i = 0; i < serializedProperty_Player.arraySize; i++)
                    {
                        serializedProperty_Player.GetArrayElementAtIndex(i).FindPropertyRelative("value1").enumValueIndex = i;
                    }
                }

            }
            allEnemyBool = EditorGUILayout.Foldout(allEnemyBool, "EnemySkillType");
            if (allEnemyBool)
            {
                editorTool2.SearchAndShow(serializedProperty_Enemy, SearchMatch);
                editorTool2.AddArray(serializedProperty_Enemy);
                editorTool2.DeleteByCount(serializedProperty_Enemy);
                editorTool2.DeleteArrayAtIndex(serializedProperty_Enemy);
                editorTool2.SortArray(serializedProperty_Enemy, "value1", "value2");
                if (GUILayout.Button("添加全部枚举"))
                {
                    editorTool2.SortArrayImmediately(serializedProperty_Enemy, "value1", "value2");
                    serializedProperty_Enemy.arraySize = typeof(EnemySkillType).Length();
                    for (int i = 0; i < serializedProperty_Enemy.arraySize; i++)
                    {
                        serializedProperty_Enemy.GetArrayElementAtIndex(i).FindPropertyRelative("value1").enumValueIndex = i;
                    }
                }

            }
            allBuildBool = EditorGUILayout.Foldout(allBuildBool, "BuildSkillType");
            if (allBuildBool)
            {
                editorTool3.SearchAndShow(serializedProperty_Build, SearchMatch);
                editorTool3.AddArray(serializedProperty_Build);
                editorTool3.DeleteByCount(serializedProperty_Build);
                editorTool3.DeleteArrayAtIndex(serializedProperty_Build);
                editorTool3.SortArray(serializedProperty_Build, "value1", "value2");
                if (GUILayout.Button("添加全部枚举"))
                {
                    editorTool3.SortArrayImmediately(serializedProperty_Build, "value1", "value2");
                    serializedProperty_Build.arraySize = typeof(BuildSkillType).Length();
                    for (int i = 0; i < serializedProperty_Build.arraySize; i++)
                    {
                        serializedProperty_Build.GetArrayElementAtIndex(i).FindPropertyRelative("value1").enumValueIndex = i;
                    }
                }

            }
 
            serializedObject.ApplyModifiedProperties();
        }


        public void OnSceneGUI(object target, Editor editor)
        {

        }

        public bool RequiresConstantRepaint(object target, Editor editor)
        {
            return EditorFramework.needToRepaint;
        }

        public bool SearchMatch(SerializedProperty serializedProperty, string search)
        {
            return serializedProperty.FindPropertyRelative("value1").enumNames[serializedProperty.FindPropertyRelative("value1").enumValueIndex].ToString().IndexOf(search, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }



}


