using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Cubic.Editor.DataStorage;
using Cubic.Editor.Screens;
using Cubic.GUI;
using Cubic.Scenes;

namespace Cubic.Editor;

public class Launcher : Scene
{
    protected override void Initialize()
    {
        base.Initialize();

        View mainView = new View(Anchor.TopLeft, Graphics.Viewport);

        Button newButton = new Button(Anchor.TopRight, new Rectangle(-70, 10, 50, 30), "New", 18);
        newButton.Click += () => OpenScreen("FileExplorerNew");
        mainView.AddElement("NewButton", newButton);
        
        Button openButton = new Button(Anchor.TopRight, new Rectangle(-10, 10, 50, 30), "Open", 18);
        openButton.Click += () => OpenScreen("FileExplorerLoad");
        mainView.AddElement("OpenButton", openButton);

        Label welcomeLabel = new Label(Anchor.TopLeft, new Point(10, 14), "Welcome to Cubic Editor");
        mainView.AddElement("WelcomeLabel", welcomeLabel);

        View filesView = new View(Anchor.Center, new Rectangle(0, 0, 450, 300));
        int y = 0;
        List<string> toRemove = null;
        foreach ((string path, _) in Data.EditorConfig.PreviousFiles.OrderByDescending(p => p.Value))
        {
            if (!Directory.Exists(path))
            {
                if (toRemove == null)
                    toRemove = new List<string>();
                toRemove.Add(path);
                continue;
            }
            Button b = new Button(Anchor.TopLeft, new Rectangle(0, y, 450, 30), new DirectoryInfo(path).Name, 18);
            b.Click += () => FileExplorerLoad(path);
            filesView.AddElement(path, b);
            y += b.Position.Height - b.Theme.BorderWidth;
        }

        if (toRemove != null)
        {
            foreach (string path in toRemove)
                Data.EditorConfig.PreviousFiles.Remove(path);
            Data.SaveEditorConfig();
        }

        mainView.AddElement("FilesView", filesView);
        UI.AddElement("MainView", mainView);

        FileExplorer feLoader = new FileExplorer(FileExplorer.ExplorerType.Open);
        feLoader.Load += FileExplorerLoad;
        AddScreen(feLoader, "FileExplorerLoad");
        FileExplorer feNew = new FileExplorer(FileExplorer.ExplorerType.Save);
        feNew.Load += FileExplorerNew;
        AddScreen(feNew, "FileExplorerNew");
    }

    private void FileExplorerNew(string path)
    {
        Data.CreateProject(path);
        if (!Data.EditorConfig.PreviousFiles.TryAdd(path, DateTime.Now))
            Data.EditorConfig.PreviousFiles[path] = DateTime.Now;
        SetScene("Editor");
    }

    private void FileExplorerLoad(string path)
    {
        if (!File.Exists(Path.Combine(path, "Project.cbproj")))
            Console.WriteLine("project doesn't exist");
        if (!Data.EditorConfig.PreviousFiles.TryAdd(path, DateTime.Now))
            Data.EditorConfig.PreviousFiles[path] = DateTime.Now;
        Data.LoadProject(path);
        SetScene("Editor");
    }

    protected override void Update()
    {
        base.Update();

        UI.GetElement<View>("MainView").Position = Graphics.Viewport;
    }

    private void SetScene(string name)
    {
        Data.EditorConfig.LauncherLocation = Game.Window.Location;
        Data.EditorConfig.LauncherSize = Game.Window.Size;
        Data.SaveEditorConfig();
        SceneManager.SetScene(name);
    }
}