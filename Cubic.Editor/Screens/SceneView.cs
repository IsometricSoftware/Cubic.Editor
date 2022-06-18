using System.Drawing;
using System.Numerics;
using Cubic.GUI;
using Cubic.Utilities;
using ImGuiNET;

namespace Cubic.Editor.Screens;

public class SceneView : Screen
{
    private string _sceneName;

    protected override void Initialize()
    {
        base.Initialize();

        _sceneName = "";
    }

    protected override void Update()
    {
        base.Update();

        Editor editor = (Editor) CurrentScene;

        float height = MenuBarScreen.Height;
        ImGui.SetNextWindowPos(new Vector2(0, 520 + height));
        ImGui.SetNextWindowSize(new Vector2(225, 200 - height));
        if (ImGui.Begin("Scene", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse))
        {
            /*if (ImGui.CollapsingHeader("Clear Color"))
            {
                Vector3 color = editor.ActiveScene.World.ClearColor.Normalize().ToVector3();
                if (ImGui.ColorPicker3("Color", ref color))
                    editor.ChangesMade = true;
                editor.ActiveScene.World.ClearColor = Color.FromArgb((int) (color.X * 255),
                    (int) (color.Y * 255), (int) (color.Z * 255));
            }*/

            if (EntityView.ShowObject(editor.ActiveScene.World))
                editor.ChangesMade = true;
            
            ImGui.End();
        }
    }
}