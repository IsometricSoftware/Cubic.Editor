using System;
using System.IO;
using System.Reflection;
using Cubic.Content.Serialization;
using Cubic.Editor.GameProject.DataStorage;
using Cubic.Entities;
using Cubic.Scenes;
using Newtonsoft.Json;

namespace Cubic.Editor.GameProject;

public class GameScene : Scene
{
    protected override void Initialize()
    {
        base.Initialize();

        SerializableScene scene =
            JsonConvert.DeserializeObject<SerializableScene>(
                File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), "Content",
                    Data.LoadedGame.Scenes[Name])),
                new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });

        World = scene.World;

        foreach ((string name, SerializableEntity entity) in scene.Entities)
        {
            AddEntity(name, new Entity(entity));
        }
    }
}