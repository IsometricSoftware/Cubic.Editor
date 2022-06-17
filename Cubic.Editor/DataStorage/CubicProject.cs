using System.Collections.Generic;
using Cubic.Content.Serialization;

namespace Cubic.Editor.DataStorage;

public struct CubicProject
{
    public readonly int Version;
    
    public readonly string ProjectName;
    public readonly string CodePath;

    public readonly Dictionary<string, SerializableScene> Scenes;

    public CubicProject(string projectName)
    {
        Version = 1;
        ProjectName = projectName;
        CodePath = "Generated";
        Scenes = new Dictionary<string, SerializableScene>();
    }
}