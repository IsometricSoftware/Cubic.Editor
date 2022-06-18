using Cubic.Editor.GameProject.DataStorage;
using Cubic.Editor.GameProject;
using Cubic.Scenes;
using Cubic.Windowing;

Data.LoadProject();

GameSettings settings = Data.LoadedGame.GameSettings;
using CubicGame game = new CubicGame(settings);
foreach ((string name, _) in Data.LoadedGame.Scenes)
{
    SceneManager.RegisterScene<GameScene>(name);
}
game.Run();

