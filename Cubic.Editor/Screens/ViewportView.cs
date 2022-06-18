using System;
using System.Numerics;
using Cubic.GUI;
using Cubic.Render;
using Cubic.Utilities;
using ImGuiNET;

namespace Cubic.Editor.Screens;

public class ViewportView : Screen
{
    protected override void Update()
    {
        base.Update();

        float height = MenuBarScreen.Height;
        ImGui.SetNextWindowPos(new Vector2(225, height));
        ImGui.SetNextWindowSize(new Vector2(805, 520));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        if (ImGui.Begin("Viewport", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse))
        {
            RenderTarget rt = ((Editor) CurrentScene).Viewport;
            if (ImGui.GetContentRegionAvail().ToSize() != rt.Size)
            {
                rt.Dispose();
                ((Editor) CurrentScene).Viewport = new RenderTarget(ImGui.GetContentRegionAvail().ToSize(), false);
            }
            ImGui.Image(Game.ImGui.TextureToImGui(rt), ImGui.GetContentRegionAvail(), Vector2.UnitY, Vector2.UnitX);
            ImGui.End();
        }
        ImGui.PopStyleVar();
    }
}