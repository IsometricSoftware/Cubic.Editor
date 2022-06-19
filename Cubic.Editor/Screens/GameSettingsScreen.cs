using System;
using Cubic.Editor.DataStorage;
using Cubic.GUI;
using ImGuiNET;

namespace Cubic.Editor.Screens;

public class GameSettingsScreen : Screen
{
    private bool _isOpen;

    protected override void Open()
    {
        base.Open();

        _isOpen = true;
    }

    protected override void Update()
    {
        base.Update();

        Editor editor = (Editor) CurrentScene;

        if (ImGui.Begin("Game Settings", ref _isOpen))
        {
            if (EntityView.ShowObject(Data.LoadedProject.GameSettings))
                editor.ChangesMade = true;
            ImGui.End();
        }
        
        if (!_isOpen)
            Close();
    }
}