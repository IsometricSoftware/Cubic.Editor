using System.Numerics;
using Cubic.Content.Serialization;
using Cubic.GUI;
using ImGuiNET;

namespace Cubic.Editor.Screens;

public class EntitiesView : Screen
{
    private string _entityName;

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
        ImGui.SetNextWindowSize(new Vector2(225, 520));
        if (ImGui.Begin("Entities", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize))
        {
            if (ImGui.BeginMenu("Add entity"))
            {
                ImGui.InputText("Name", ref _entityName, 50);
                if (ImGui.Button("Add"))
                {
                    editor.CreateEntity(_entityName);
                    editor.ChangesMade = true;
                    _entityName = "";
                }
                
                ImGui.EndMenu();
            }

            if (ImGui.BeginListBox("Entities"))
            {
                foreach ((string name, SerializableEntity entity) in editor.ActiveScene.Entities)
                {
                    if (ImGui.Selectable(name, editor.CurrentEntity.HasValue && editor.CurrentEntity.Value.Name == name))
                        editor.CurrentEntity = entity;
                }
                
                ImGui.EndListBox();
            }
            
            ImGui.End();
        }
    }
}