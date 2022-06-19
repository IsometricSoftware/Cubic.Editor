using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using Cubic.Content.Serialization;
using Cubic.Entities.Components;
using Cubic.GUI;
using Cubic.Render;
using Cubic.Utilities;
using ImGuiNET;

namespace Cubic.Editor.Screens;

public class EntityView : Screen
{
    private string _entityName;
    private string _lastEntityName;
    
    protected override void Initialize()
    {
        base.Initialize();

        _entityName = null;
        _lastEntityName = "";
    }

    protected override void Update()
    {
        base.Update();
        
        SerializableEntity? entity = ((Editor) CurrentScene).CurrentEntity;
        
        float height = MenuBarScreen.Height;
        ImGui.SetNextWindowPos(new Vector2(Graphics.Viewport.Width - 250, height));
        ImGui.SetNextWindowSize(new Vector2(250, Graphics.Viewport.Height - height));
        if (ImGui.Begin("Entity", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoBringToFrontOnFocus))
        {
            if (entity.HasValue)
            {
                if (_lastEntityName != entity.Value.Name)
                {
                    _lastEntityName = entity.Value.Name;
                    _entityName = _lastEntityName;
                }

                if (ImGui.InputText("Name", ref _entityName, 50, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    ((Editor) CurrentScene).RenameEntity(_lastEntityName, _entityName);
                    ((Editor) CurrentScene).ChangesMade = true;
                }

                if (ImGui.CollapsingHeader("Transform"))
                {
                    if (ShowObject(entity.Value.Transform, false))
                        ((Editor) CurrentScene).ChangesMade = true;
                }

                foreach (Component component in entity.Value.Components)
                {
                    if (ImGui.CollapsingHeader(component.GetType().Name))
                    {
                        if (ShowObject(component))
                            ((Editor) CurrentScene).ChangesMade = true;
                    }
                }
            }

            ImGui.End();
        }
    }

    public static bool ShowObject(object obj, bool properties = true)
    {
        const float speed = 0.1f;

        bool edited = false;
        foreach (MemberInfo fi in obj.GetType().GetMembers())
        {
            string fieldName = fi.Name;
            for (int i = 0; i < fieldName.Length; i++)
            {
                char c = fieldName[i];
                if (char.IsUpper(c) && i != 0)
                {
                    fieldName = fieldName.Remove(i, 1);
                    fieldName = fieldName.Insert(i, $" {char.ToLower(c)}");
                    i++;
                }
            }

            object getObj = GetValue(fi, obj, properties);
            if (getObj == null)
                continue;
            switch (getObj)
            {
                case bool value:
                    if (ImGui.Checkbox(fieldName, ref value))
                        edited = SetValue(fi, obj, value);
                    break;
                case int value:
                    if (ImGui.DragInt(fieldName, ref value, speed))
                        edited = SetValue(fi, obj, value);
                    break;
                case uint value:
                    int v = (int) value;
                    if (ImGui.DragInt(fieldName, ref v, speed, 0))
                        edited = SetValue(fi, obj, (uint) v);
                    break;
                case float value:
                    if (ImGui.DragFloat(fieldName, ref value, speed))
                        edited = SetValue(fi, obj, value);
                    break;
                case string value:
                    if (ImGui.InputText(fieldName, ref value, 3000))
                        edited = SetValue(fi, obj, value);
                    break;
                case Size value:
                    FieldSize s = new FieldSize(value);
                    if (ImGui.DragInt2(fieldName, ref s.Width, speed))
                        edited = SetValue(fi, obj, new Size(s.Width, s.Height));
                    break;
                case Point value:
                    FieldPoint p = new FieldPoint(value);
                    if (ImGui.DragInt2(fieldName, ref p.X, speed))
                        edited = SetValue(fi, obj, new Point(p.X, p.Y));
                    break;
                case Vector2 value:
                    if (ImGui.DragFloat2(fieldName, ref value, speed))
                        edited = SetValue(fi, obj, value);
                    break;
                case Vector3 value:
                    if (ImGui.DragFloat3(fieldName, ref value, speed))
                        edited = SetValue(fi, obj, value);
                    break;
                case Vector4 value:
                    if (ImGui.DragFloat4(fieldName, ref value, speed))
                        edited = SetValue(fi, obj, value);
                    break;
                case Quaternion value:
                    Vector3 eulerAngles = value.ToEulerAngles();
                    eulerAngles.X = CubicMath.ToDegrees(eulerAngles.X);
                    eulerAngles.Y = CubicMath.ToDegrees(eulerAngles.Y);
                    eulerAngles.Z = CubicMath.ToDegrees(eulerAngles.Z);
                    if (ImGui.DragFloat3(fieldName, ref eulerAngles, speed))
                    {
                        eulerAngles.X = CubicMath.ToRadians(eulerAngles.X);
                        eulerAngles.Y = CubicMath.ToRadians(eulerAngles.Y);
                        eulerAngles.Z = CubicMath.ToRadians(eulerAngles.Z);
                        edited = SetValue(fi, obj, Quaternion.CreateFromYawPitchRoll(eulerAngles.X, eulerAngles.Y, eulerAngles.Z));
                    }
                    break;
                case Color value:
                    Vector4 vValue = value.Normalize();
                    if (ImGui.ColorEdit4(fieldName, ref vValue))
                    {
                        value = Color.FromArgb((int) (vValue.W * 255), (int) (vValue.X * 255), (int) (vValue.Y * 255),
                            (int) (vValue.Z * 255));
                        edited = SetValue(fi, obj, value);
                    }
                    break;
                case Enum value:
                    if (ImGui.BeginCombo(fieldName, value.ToString()))
                    {
                        foreach (string name in Enum.GetNames(value.GetType()))
                        {
                            if (ImGui.Selectable(name, name == value.ToString()))
                                edited = SetValue(fi, obj, Enum.Parse(value.GetType(), name));
                        }
                        ImGui.EndCombo();
                    }

                    break;
                case Texture2D value:
                    string path = value.Path ?? "";
                    string prevPath = path;
                    if (ImGui.InputText(fieldName, ref path, 5000))
                    {
                        if (path != prevPath && File.Exists(path))
                        {
                            if (value != Texture2D.Blank && value != Texture2D.Void)
                                value.Dispose();
                            value = new Texture2D(path);
                            edited = SetValue(fi, obj, value);
                        }
                    }
                    break;
                default:
                    //ImGui.Text(fieldName);
                    if (ImGui.CollapsingHeader(fieldName))
                    {
                        ShowObject(getObj);
                        ImGui.Spacing();
                    }

                    break;
            }
        }

        return edited;
    }

    private static bool SetValue(MemberInfo info, object obj, object value)
    {
        switch (info.MemberType)
        {
            case MemberTypes.Field:
                ((FieldInfo) info).SetValue(obj, value);
                break;
            case MemberTypes.Property:
                ((PropertyInfo) info).SetValue(obj, value);
                break;
        }

        return true;
    }
    
    private static object GetValue(MemberInfo info, object obj, bool properties)
    {
        switch (info.MemberType)
        {
            case MemberTypes.Field:
                return ((FieldInfo) info).GetValue(obj);
            case MemberTypes.Property:
                if (!properties)
                    return null;
                PropertyInfo inf = (PropertyInfo) info;
                if (!inf.CanWrite)
                    return null;
                return inf.GetValue(obj);
        }

        return null;
    }

    private ref struct FieldSize
    {
        public int Width;
        public int Height;

        public FieldSize(Size size)
        {
            Width = size.Width;
            Height = size.Height;
        }
    }
    
    private ref struct FieldPoint
    {
        public int X;
        public int Y;

        public FieldPoint(Point point)
        {
            X = point.X;
            Y = point.Y;
        }
    }
}