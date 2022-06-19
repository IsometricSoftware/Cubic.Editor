using System.Drawing;
using Cubic.Editor;
using Cubic.Editor.DataStorage;
using Cubic.Scenes;
using Cubic.Windowing;

Data.GetEditorConfig();

GameSettings settings = new GameSettings()
{
    Title = "Cubic Editor - Launcher",
    Size = Data.EditorConfig.LauncherSize,
    Location = Data.EditorConfig.LauncherLocation,
    Resizable = true
};

using CubicGame game = new CubicGame(settings);
SceneManager.RegisterScene<Launcher>("Launcher");
SceneManager.RegisterScene<Editor>("Editor");
game.Run();