using System.Drawing;
using System.Linq;
using Cubic.Content.Serialization;
using Cubic.Editor.DataStorage;
using Cubic.Editor.Screens;
using Cubic.Entities;
using Cubic.Entities.Components;
using Cubic.Render;
using Cubic.Scenes;
using ImGuiNET;

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
        
        // TODO: These screens have fixed sizes - try to fix the docking of imgui.net at some point
        AddScreen(new SceneView(), "Scene");
        AddScreen(new EntitiesView(), "Entities");
        AddScreen(new EntityView(), "Entity");
        AddScreen(new AssetsView(), "Assets");
        AddScreen(new ViewportView(), "Viewport");
        AddScreen(new MenuBarScreen(), "MenuBar");
        OpenScreen("Scene");
        OpenScreen("Entities");
        OpenScreen("Entity");
        OpenScreen("Assets");
        OpenScreen("Viewport");
        OpenScreen("MenuBar");

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
    }

    protected override void Update()
    {
        Game.ImGui.SetFont("Roboto");
        World.SampleType = ActiveScene.World.SampleType;
        
        base.Update();
    }

    protected override void Draw()
    {
        Graphics.SetRenderTarget(Viewport);
        Graphics.Clear(ActiveScene.World.ClearColor);
        base.Draw();
        Graphics.SetRenderTarget(null);
    }

    public void CreateEntity(string name)
    {
        SerializableEntity entity = new SerializableEntity(name);
        entity.Components.Add(new Sprite(Texture2D.Blank));
        ActiveScene.Entities.Add(name, entity);
        
        AddEntity(name, new Entity(entity));
    }
}