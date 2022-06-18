using System;
using Cubic.Editor.DataStorage;
using Cubic.GUI;
using ImGuiNET;

namespace Cubic.Editor.Screens;

public class MenuBarScreen : Screen
{
    public static float Height;
    
    protected override void Update()
    {
        base.Update();

        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Save", "Ctrl+S"))
                {
                    Data.SaveProject();
                    ((Editor) CurrentScene).ChangesMade = false;
                }
                ImGui.EndMenu();
            }

            if (ImGui.MenuItem("Run", "", Data.ProjectRunning))
            {
                Data.RunProject();
            }
            
            Height = ImGui.GetWindowHeight();
            ImGui.EndMainMenuBar();
        }

        if (Input.KeyDown(Keys.LeftControl))
        {
            if (Input.KeyPressed(Keys.S))
            {
                Data.SaveProject();
                ((Editor) CurrentScene).ChangesMade = false;
            }
        }
    }
}