using RuiRuiTool;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using CircleOfLife;
using System.Linq;

public class NewGizmos : CustomEditorSelector
{
    [DllImport("User32.dll")]
    public static extern short GetAsyncKeyState(int vkey);//外部函数
    const int VK_MENU = 0x12;

    Object target;

    Event activeEvent;

    List<GizmosSetting> gizmosSettings;


    public void OnEnable(object target, Editor editor)
    {
        this.target = (Object)target;
        gizmosSettings = new List<GizmosSetting>();
        var fields = target.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public).ToList();
        var GizmosSetting_ = fields.FindAll(x => x.FieldType == (typeof(GizmosSetting)));
        var GizmosSetting_List = fields.FindAll(x => x.FieldType == (typeof(List<GizmosSetting>)));
        foreach (var a in GizmosSetting_)
        {
            if ((GizmosSetting)a.GetValue(target) != null)
                gizmosSettings.Add((GizmosSetting)a.GetValue(target));

        }
        foreach (var a in GizmosSetting_List)
        {

            foreach (var b in (List<GizmosSetting>)a.GetValue(target))
            {
                if (b != null) gizmosSettings.Add(b);
            }
        }



    }


    public void OnSceneGUI(object target, Editor editor)
    {
        activeEvent = Event.current;
        foreach (var gizmosSetting in gizmosSettings)
        {
            DrawGizmosFunc(gizmosSetting);
        }




    }

    private void DrawGizmosFunc(GizmosSetting gizmosSetting)
    {
        if (gizmosSetting.openGizmos)
        {
            Handles.color = gizmosSetting.gizmosColor;
            if (gizmosSetting.GizmosType == DrawGizmosType.circle)
            {
                Handles.DrawWireDisc((Vector2)gizmosSetting.point.position + gizmosSetting.offSet, Vector3.forward, gizmosSetting.radius);

            }
            else
            {
                Handles.DrawWireCube((Vector2)gizmosSetting.point.position + gizmosSetting.offSet, gizmosSetting.size);
            }
        }

        if (gizmosSetting.editorMode) EditorModeFunc(gizmosSetting);
    }


    /// <summary>
    /// scene中拖动大小
    /// </summary>
    private void EditorModeFunc(GizmosSetting setting)
    {



        Handles.color = setting.gizmosColor;
        if (setting.GizmosType == DrawGizmosType.circle)
        {
            HandlePartFunc(new Vector2(setting.radius, 0), setting, DrawGizmosType.circle);
            HandlePartFunc(new Vector2(-setting.radius, 0), setting, DrawGizmosType.circle);
            HandlePartFunc(new Vector2(0, setting.radius), setting, DrawGizmosType.circle);
            HandlePartFunc(new Vector2(0, -setting.radius), setting, DrawGizmosType.circle);
            HandlePartFunc(Vector2.zero, setting, DrawGizmosType.circle);
        }
        else
        {
            HandlePartFunc(new Vector2(setting.size.x / 2, 0), setting, DrawGizmosType.rectangle);
            HandlePartFunc(new Vector2(-setting.size.x / 2, 0), setting, DrawGizmosType.rectangle);
            HandlePartFunc(new Vector2(0, setting.size.y / 2), setting, DrawGizmosType.rectangle);
            HandlePartFunc(new Vector2(0, -setting.size.y / 2), setting, DrawGizmosType.rectangle);
            HandlePartFunc(Vector2.zero, setting, DrawGizmosType.rectangle);
        }

        Handles.color = Color.red; ;
    }

    private Vector2 NewCubeHandle(Vector2 position)
    {
         return (Vector2)Handles.FreeMoveHandle(position, 0.05f * HandleUtility.GetHandleSize(position), Vector3.one * 0.05f * HandleUtility.GetHandleSize(position), Handles.CubeHandleCap);
    }
    private void HandlePartFunc(Vector2 delta_position, GizmosSetting setting, DrawGizmosType type)
    {


        if (delta_position == Vector2.zero)
        {
            setting.offSet = NewCubeHandle(setting.gizmosCenter) - (Vector2)setting.point.position;
            return;
        }
        Vector2 mid_value;
        mid_value = NewCubeHandle(setting.gizmosCenter + delta_position) - (setting.gizmosCenter + delta_position);
        //activeEvent.functionKey
        if (GetAsyncKeyState(VK_MENU) != 0)
        {
            switch (type)
            {

                case DrawGizmosType.rectangle:
                    if (Mathf.Abs(mid_value.x * 2) > setting.size.x - 0.04) mid_value.x = Mathf.Sign(mid_value.x) * ((setting.size.x - 0.1f) / 2);
                    if (Mathf.Abs(mid_value.y * 2) > setting.size.y - 0.04) mid_value.y = Mathf.Sign(mid_value.y) * ((setting.size.y - 0.1f) / 2);

                    if (delta_position.x < 0)
                    {
                        setting.size.x -= mid_value.x * 2;
                    }
                    else if (delta_position.x > 0)
                    {
                        setting.size.x += mid_value.x * 2;
                    }
                    if (delta_position.y < 0)
                    {
                        setting.size.y -= mid_value.y * 2;
                    }
                    else if (delta_position.y > 0)
                    {
                        setting.size.y += mid_value.y * 2;
                    }
                    break;
                case DrawGizmosType.circle:

                    if (Mathf.Abs(mid_value.x) > setting.radius - 0.04) mid_value.x = Mathf.Sign(mid_value.x) * (setting.radius - 0.05f);
                    if (Mathf.Abs(mid_value.y) > setting.radius - 0.04) mid_value.y = Mathf.Sign(mid_value.y) * (setting.radius - 0.05f);

                    if (delta_position.x < 0)
                    {
                        setting.radius -= mid_value.x;
                    }
                    else if (delta_position.x > 0)
                    {
                        setting.radius += mid_value.x;
                    }
                    if (delta_position.y < 0)
                    {
                        setting.radius -= mid_value.y;
                    }
                    else if (delta_position.y > 0)
                    {
                        setting.radius += mid_value.y;
                    }
                    if (setting.radius < 0.05f) setting.radius = 0.05f;


                    break;
            }

        }
        else
        {
            switch (type)
            {
                #region rectangle  FUNC
                case DrawGizmosType.rectangle:

                    if (Mathf.Abs(mid_value.x) > setting.size.x - 0.04) mid_value.x = Mathf.Sign(mid_value.x) * (setting.size.x - 0.05f);
                    if (Mathf.Abs(mid_value.y) > setting.size.y - 0.04) mid_value.y = Mathf.Sign(mid_value.y) * (setting.size.y - 0.05f);


                    if (delta_position.x < 0)
                    {
                        setting.size.x -= mid_value.x;
                        setting.offSet.x += mid_value.x / 2;
                    }
                    else if (delta_position.x > 0)
                    {
                        setting.size.x += mid_value.x;
                        setting.offSet.x += mid_value.x / 2;
                    }
                    if (delta_position.y < 0)
                    {
                        setting.size.y -= mid_value.y;
                        setting.offSet.y += mid_value.y / 2;
                    }
                    else if (delta_position.y > 0)
                    {
                        setting.size.y += mid_value.y;
                        setting.offSet.y += mid_value.y / 2;
                    }

                    break;
                #endregion

                #region circle fUNC
                case DrawGizmosType.circle:
                    if (Mathf.Abs(mid_value.x) > setting.radius - 0.04) mid_value.x = Mathf.Sign(mid_value.x) * (setting.radius - 0.05f);
                    if (Mathf.Abs(mid_value.y) > setting.radius - 0.04) mid_value.y = Mathf.Sign(mid_value.y) * (setting.radius - 0.05f);

                    if (delta_position.x < 0)
                    {
                        setting.offSet.x += mid_value.x / 2;
                        setting.radius -= mid_value.x;
                    }
                    else if (delta_position.x > 0)
                    {
                        setting.offSet.x += mid_value.x / 2;
                        setting.radius += mid_value.x;
                    }
                    if (delta_position.y < 0)
                    {
                        setting.offSet.y += mid_value.y / 2;
                        setting.radius -= mid_value.y;
                    }
                    else if (delta_position.y > 0)
                    {
                        setting.offSet.y += mid_value.y / 2;
                        setting.radius += mid_value.y;
                    }
                    if (setting.radius < 0.05f) setting.radius = 0.05f;

                    break;
                    #endregion

            }


        }

        EditorUtility.SetDirty(target);

    }

    public bool IsMatch(object target, Editor editor)
    {
        return gizmosSettings.Count > 0;
    }

    public void OnInspectorGUI(object target, Editor editor)
    {


    }

    public void OnDisable(object target, Editor editor)
    {

    }

    public bool RequiresConstantRepaint(object target, Editor editor)
    {
        return EditorFramework.needToRepaint;
    }
}
