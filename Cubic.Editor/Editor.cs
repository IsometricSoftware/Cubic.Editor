using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Cubic.Content.Serialization;
using Cubic.Editor.DataStorage;
using Cubic.Editor.Screens;
using Cubic.Entities;
using Cubic.Entities.Components;
using Cubic.Render;
using Cubic.Scenes;
using Cubic.Utilities;
using ImGuiNET;
using Newtonsoft.Json;

namespace Cubic.Editor;

public class Editor : Scene
{
    public SerializableScene ActiveScene;
    public RenderTarget Viewport;
    public SerializableEntity? CurrentEntity;

    private bool _changesMade;
    public bool ChangesMade
    {
        get => _changesMade;
        set
        {
            if (_changesMade != value)
            {
                if (value)
                    Game.Window.Title = $"* {ActiveScene.Name} - {Data.LoadedProject.ProjectName} - Cubic Editor";
                else
                    Game.Window.Title = $"{ActiveScene.Name} - {Data.LoadedProject.ProjectName} - Cubic Editor";
            }

            _changesMade = value;
        }
    }
    
    protected override void Initialize()
    {
        base.Initialize();
        
        (string name, ActiveScene) = Data.LoadedProject.Scenes.ElementAt(0);
        
        Game.Window.Size = new Size(1280, 720);
        Game.Window.Title = $"{name} - {Data.LoadedProject.ProjectName} - Cubic Editor";
        Game.Window.CenterWindow();
        
        ImGui.StyleColorsLight();
        Game.ImGui.AddFont("Roboto", "Roboto-Regular.ttf", 18);
        
        World.ClearColor = Color.Black;

        Viewport = new RenderTarget(Game.Window.Size, false);
        CurrentEntity = null;
        
        foreach ((string eName, SerializableEntity entity) in ActiveScene.Entities)
            AddEntity(eName, new Entity(entity));
        
        // TODO: These screens have fixed sizes - try to fix the docking of imgui.net at some point
        AddScreen(new SceneView(), "Scene");
        AddScreen(new EntitiesView(), "Entities");
        AddScreen(new EntityView(), "Entity");
        AddScreen(new AssetsView(), "Assets");
        AddScreen(new ViewportView(), "Viewport");
        AddScreen(new MenuBarScreen(), "MenuBar");
        AddScreen(new GameSettingsScreen(), "GameSettings");
        OpenScreen("Scene");
        OpenScreen("Entities");
        OpenScreen("Entity");
        OpenScreen("Assets");
        OpenScreen("Viewport");
        OpenScreen("MenuBar");
    }

    protected override void Update()
    {
        Game.ImGui.SetFont("Roboto");
        World.SampleType = ActiveScene.World.SampleType;

        base.Update();

        MouseCursor cursor = ImGui.GetMouseCursor() switch
        {
            ImGuiMouseCursor.TextInput => MouseCursor.IBeam,
            ImGuiMouseCursor.ResizeAll => MouseCursor.Crosshair,
            ImGuiMouseCursor.ResizeNS => MouseCursor.HorizontalResize,
            ImGuiMouseCursor.ResizeEW => MouseCursor.VerticalResize,
            ImGuiMouseCursor.ResizeNESW => MouseCursor.HorizontalResize,
            ImGuiMouseCursor.ResizeNWSE => MouseCursor.VerticalResize,
            ImGuiMouseCursor.Hand => MouseCursor.Hand,
            _ => MouseCursor.Normal
        };
        
        Input.SetMouseCursor(cursor);
    }

    protected override void Draw()
    {
        Graphics.SetRenderTarget(Viewport);
        Graphics.Clear(ActiveScene.World.ClearColor);
        base.Draw();
        if (CurrentEntity.HasValue)
        {
            Graphics.SpriteRenderer.Begin(Camera2D.Main.TransformMatrix);
            Entity entity = GetEntity(CurrentEntity.Value.Name);
            Sprite sprite = entity.GetComponent<Sprite>();
            const int borderWidth = 5;
            Graphics.SpriteRenderer.DrawBorder(entity.Transform.Position.ToVector2() - new Vector2(borderWidth),
                entity.Transform.Scale.ToVector2() * sprite.SpriteTexture.Size.ToVector2() +
                new Vector2(borderWidth * 2), borderWidth, Color.White, entity.Transform.SpriteRotation, entity.Transform.Origin);
            Graphics.SpriteRenderer.End();
        }

        Graphics.SetRenderTarget(null);
    }

    public void CreateEntity(string name)
    {
        SerializableEntity entity = new SerializableEntity(name);
        entity.Components.Add(new Sprite(Texture2D.Blank));
        ActiveScene.Entities.Add(name, entity);
        
        AddEntity(name, new Entity(entity));
    }

    public void RenameEntity(string originalName, string newName)
    {
        SerializableEntity entity = ActiveScene.Entities[originalName];
        entity.Name = newName;
        ActiveScene.Entities.Remove(originalName);
        ActiveScene.Entities.Add(newName, entity);
        RemoveEntity(originalName);
        AddEntity(newName, new Entity(entity));
        CurrentEntity = entity;
    }

    public void Duplicate(string name)
    {
        SerializableEntity entity = Data.DeserializeObject<SerializableEntity>(Data.SerializeObject(ActiveScene.Entities[name]));

        int i = 0;
        string entName;
        do
        {
            entName = entity.Name + $" ({++i})";
        } while (ActiveScene.Entities.ContainsKey(entName));

        entity.Name = entName;
        ActiveScene.Entities.Add(entity.Name, entity);
        AddEntity(entity.Name, new Entity(entity));
    }
}