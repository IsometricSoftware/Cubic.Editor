using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Cubic.Editor.GameProject.DataStorage;

public static class Data
{
    public static Game LoadedGame { get; private set; }

    public static void LoadProject()
    {
        LoadedGame = JsonConvert.DeserializeObject<Game>(
            File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), "Content", "GAME")),
            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
    }
}