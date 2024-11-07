using RuiRuiTool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CircleOfLife;
using UnityEditor.EditorTools;
using Milutools.Milutools.General;
using CircleOfLife.Configuration;
using System;
using CircleOfLife.ScriptObject;

public class AllPrefabSettingEditor<T1,T2> : CustomEditorSelector, IUseInspectorEditorTool where T1 : Enum
{
    UnityEngine.Object target;
    AllSettingSo<T1,T2> allPrefabsSetting;
    InspectorArrayEditorTool allPrefabTool, allBuildSettingTool;
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
        return target is AllSettingSo<T1, T2>;
    }

    public void OnDisable(object target, Editor editor)
    {
       
    }

    public void OnEnable(object target, Editor editor)
    {
     
        this.target = (UnityEngine.Object)target;
        if (target is AllSettingSo<T1, T2>)
        {
            allPrefabsSetting = (AllSettingSo<T1, T2>)target;
        }
        allPrefabTool = ScriptableObject.CreateInstance<InspectorArrayEditorTool>();
        allBuildSettingTool = ScriptableObject.CreateInstance<InspectorArrayEditorTool>();
    }
    public void OnSceneGUI(object target, Editor editor)
    {

    }

    public void OnInspectorGUI(object target, Editor editor)
    {
        SerializedObject serializedObject = new SerializedObject(allPrefabsSetting);
        SerializedProperty serializedProperty_AllPrefabs = serializedObject.FindProperty("AllBuildSettings");
   

      

        AllPrefabsShow(serializedProperty_AllPrefabs);
      

        serializedObject.ApplyModifiedProperties();
    }

   
    private void AllPrefabsShow(SerializedProperty serializedProperty)
    {
        allPrefabTool.SearchAndShow(serializedProperty, SearchMatch);
        allPrefabTool.AddArray(serializedProperty);
        allPrefabTool.DeleteByCount(serializedProperty);
        allPrefabTool.DeleteArrayAtIndex(serializedProperty);
        allPrefabTool.SortArray(serializedProperty, "value1", "value2");
        if (GUILayout.Button("添加全部已注册枚举"))
        {
            allBuildSettingTool.SortArrayImmediately(serializedProperty, "value1", "value2");
            serializedProperty.arraySize = typeof(T1).Length();
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("value1").enumValueIndex = i;
            }
        }
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
