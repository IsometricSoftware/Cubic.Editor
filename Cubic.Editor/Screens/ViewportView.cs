using System;
using System.Numerics;
using Cubic.Entities;
using Cubic.GUI;
using Cubic.Render;
using Cubic.Utilities;
using ImGuiNET;

namespace Cubic.Editor.Screens;

public class ViewportView : Screen
{
    private Vector2 _lastMousePos;

    protected override void Initialize()
    {
        base.Initialize();

        _lastMousePos = new Vector2(-1);
    }

    protected override void Update()
    {
        base.Update();

        float height = MenuBarScreen.Height;
        ImGui.SetNextWindowPos(new Vector2(225, height));
        ImGui.SetNextWindowSize(new Vector2(Graphics.Viewport.Width - 475, Graphics.Viewport.Height - 200));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        if (ImGui.Begin("Viewport", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoBringToFrontOnFocus))
        {
            RenderTarget rt = ((Editor) CurrentScene).Viewport;
            if (ImGui.GetContentRegionAvail().ToSize() != rt.Size)
            {
                rt.Dispose();
                ((Editor) CurrentScene).Viewport = new RenderTarget(ImGui.GetContentRegionAvail().ToSize(), false);
            }
            ImGui.Image(Game.ImGui.TextureToImGui(rt), ImGui.GetContentRegionAvail(), Vector2.UnitY, Vector2.UnitX);

            if (ImGui.IsItemHovered())
            {
                Camera2D.Main.Transform.Scale += new Vector3(Input.ScrollWheelDelta.Y, Input.ScrollWheelDelta.Y, 0) * 0.1f;
                
                ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeAll);
                if (Input.MouseButtonDown(MouseButtons.Left))
                {
                    if (_lastMousePos == new Vector2(-1))
                        _lastMousePos = Input.MousePosition;
                    else
                    {
                        Camera2D.Main.Transform.Position -= new Vector3(Input.MousePosition - _lastMousePos, 0);
                        _lastMousePos = Input.MousePosition;
                    }
                }
                else
                    _lastMousePos = new Vector2(-1);
            }

            ImGui.End();
        }
        ImGui.PopStyleVar();
    }
}