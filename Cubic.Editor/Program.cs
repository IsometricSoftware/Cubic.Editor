using System.Drawing;
using Cubic.Editor;
using Cubic.Scenes;
using Cubic.Windowing;

GameSettings settings = new GameSettings()
{
    Title = "Cubic Editor - Launcher",
    Size = new Size(500, 500),
    Resizable = false
};

using CubicGame game = new CubicGame(settings);
SceneManager.RegisterScene<Launcher>("Launcher");
SceneManager.RegisterScene<Editor>("Editor");
game.Run();