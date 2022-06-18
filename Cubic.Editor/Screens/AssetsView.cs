using System.Numerics;
using Cubic.GUI;
using ImGuiNET;

namespace Cubic.Editor.Screens;

public class AssetsView : Screen
{
    protected override void Update()
    {
        base.Update();

        float height = MenuBarScreen.Height;
        ImGui.SetNextWindowPos(new Vector2(225, 520 + height));
        ImGui.SetNextWindowSize(new Vector2(805, 200 - height));
        if (ImGui.Begin("Assets", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse))
        {
            ImGui.End();
        }
    }
}