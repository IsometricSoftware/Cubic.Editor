using System.Collections.Generic;
using Cubic.Windowing;

namespace Cubic.Editor.GameProject.DataStorage;

public struct Game
{
    public string Name;
    public Dictionary<string, string> Scenes;
    public GameSettings GameSettings;
}