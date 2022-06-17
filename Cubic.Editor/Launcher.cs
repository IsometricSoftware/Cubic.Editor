using System.Drawing;
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
        mainView.AddElement("NewButton", newButton);
        
        Button openButton = new Button(Anchor.TopRight, new Rectangle(-10, 10, 50, 30), "Open", 18);
        mainView.AddElement("OpenButton", openButton);

        Label welcomeLabel = new Label(Anchor.TopLeft, new Point(10, 14), "Welcome to Cubic Editor");
        mainView.AddElement("WelcomeLabel", welcomeLabel);

        View filesView = new View(Anchor.Center, new Rectangle(0, 0, 450, 300));
        filesView.AddElement("temp", new Label(Anchor.Center, Point.Empty, "Recent files will go here..."));

        mainView.AddElement("FilesView", filesView);
        UI.AddElement("MainView", mainView);
    }

    protected override void Update()
    {
        base.Update();

        UI.GetElement<View>("MainView").Position = Graphics.Viewport;
    }
}