using System;
using System.Collections.Generic;
using Cubic.Content.Serialization;
using Cubic.Editor.GameProject.DataStorage;
using Cubic.Windowing;

namespace Cubic.Editor.DataStorage;

public struct CubicProject
{
    public readonly int Version;
    
    public readonly string ProjectName;
    public readonly string CodePath;

    public readonly Dictionary<string, SerializableScene> Scenes;

    public GSettings GameSettings;

    public CubicProject(string projectName)
    {
        Version = 1;
        ProjectName = projectName;
        CodePath = "Project";
        Scenes = new Dictionary<string, SerializableScene>();
        GameSettings = new GSettings(new GameSettings()
        {
            Title = projectName
        });
    }
}