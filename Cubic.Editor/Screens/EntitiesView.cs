using System;
using System.Linq;
using System.Numerics;
using Cubic.Content.Serialization;
using Cubic.GUI;
using ImGuiNET;

namespace Cubic.Editor.Screens;

public class EntitiesView : Screen
{
    private string _entityName;
    private string _rightClickName;

    protected override void Initialize()
    {
        base.Initialize();

        _entityName = "";
    }

    protected override void Update()
    {
        base.Update();

        Editor editor = (Editor) CurrentScene;
        
        float height = MenuBarScreen.Height;
        ImGui.SetNextWindowPos(new Vector2(0, height));
        ImGui.SetNextWindowSize(new Vector2(225, Graphics.Viewport.Height - 200));
        if (ImGui.Begin("Entities", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoBringToFrontOnFocus))
        {
            if (ImGui.CollapsingHeader("Add entity"))
            {
                ImGui.InputText("Name", ref _entityName, 50);
                if (ImGui.Button("Add"))
                {
                    editor.CreateEntity(_entityName);
                    editor.ChangesMade = true;
                    _entityName = "";
                }
            }
            
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            bool itemClicked = false;
            
            foreach ((string name, SerializableEntity entity) in editor.ActiveScene.Entities)
            {
                if (ImGui.Selectable(name, editor.CurrentEntity.HasValue && editor.CurrentEntity.Value.Name == name))
                    editor.CurrentEntity = entity;

                if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && !itemClicked)
                {
                    if (ImGui.IsItemHovered())
                    {
                        itemClicked = true;
                        _rightClickName = name;
                    }
                    else
                    {
                        _rightClickName = "";
                    }
                }
            }

            if (_rightClickName != "" && ImGui.BeginPopupContextWindow("cMenu"))
            {
                if (ImGui.MenuItem("Delete"))
                {
                    editor.CurrentEntity = null;
                    editor.ActiveScene.Entities.Remove(_rightClickName);
                    editor.RemoveEntity(_rightClickName);
                    editor.ChangesMade = true;
                    _rightClickName = "";
                }

                if (ImGui.MenuItem("Duplicate"))
                {
                    editor.Duplicate(_rightClickName);
                    editor.ChangesMade = true;
                    _rightClickName = "";
                }

                ImGui.EndPopup();
            }

            ImGui.End();
        }
    }
}