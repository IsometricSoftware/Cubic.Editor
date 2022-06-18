using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cubic.Content.Serialization;
using Cubic.Editor.GameProject.DataStorage;
using Cubic.Scenes;
using Cubic.Windowing;
using Newtonsoft.Json;

namespace Cubic.Editor.DataStorage;

public static class Data
{
    public static bool ProjectRunning { get; private set; }
    
    public static CubicProject LoadedProject { get; private set; }
    
    public static string ProjectPath { get; private set; }

    public const string FileProject = "Project.cbproj";

    public static void CreateProject(string dirPath)
    {
        ProjectPath = dirPath;
        
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        LoadedProject = new CubicProject(Path.GetFileName(dirPath));
        LoadedProject.Scenes.Add("Scene1", new SerializableScene("Scene1"));
        
        File.WriteAllText(Path.Combine(dirPath, FileProject), SerializeObject(LoadedProject));

        string projPath = Path.Combine(dirPath, "Project");
        Directory.CreateDirectory(projPath);
        RunDotnetCommand($"new classlib -n \"{LoadedProject.ProjectName.Replace(" ", "_")}\" -o \"{projPath}\"");
        RunDotnetCommand($"add \"{projPath}\" package Cubic --prerelease");
        File.Delete(Path.Combine(projPath, "Class1.cs"));
    }

    public static void LoadProject(string dirPath)
    {
        ProjectPath = dirPath;
        LoadedProject = DeserializeObject<CubicProject>(File.ReadAllText(Path.Combine(dirPath, "Project.cbproj")));
    }

    public static void SaveProject()
    {
        File.WriteAllText(Path.Combine(ProjectPath, FileProject), SerializeObject(LoadedProject));
    }

    public static void RunProject()
    {
        string buildDir = Path.Combine(ProjectPath, "Build");
        string projDir = Path.Combine(ProjectPath, "Project");
        if (!Directory.Exists(buildDir))
            Directory.CreateDirectory(buildDir);
        
        RunDotnetCommand($"build \"../../../../Cubic.Editor.GameProject\" -o \"{buildDir}\"");
        RunDotnetCommand($"build \"{projDir}\" -o \"{buildDir}\"");

        string contentDir = Path.Combine(buildDir, "Content");
        if (!Directory.Exists(contentDir))
            Directory.CreateDirectory(contentDir);

        Dictionary<string, string> scenes = new Dictionary<string, string>();
        int i = 0;
        foreach ((string name, SerializableScene scene) in LoadedProject.Scenes)
        {
            File.WriteAllText(Path.Combine(contentDir, $"SCENE{++i}"), SerializeObject(scene));
            scenes.Add(name, $"SCENE{i}");
        }

        Game game = new Game()
        {
            Scenes = scenes,
            GameSettings = new GameSettings()
            {
                Title = LoadedProject.ProjectName
            },
            Name = LoadedProject.ProjectName
        };

        File.WriteAllText(Path.Combine(contentDir, "GAME"), SerializeObject(game));
        
        Task.Run(() =>
        {
            ProjectRunning = true;
            RunProcess($"{buildDir}/Cubic.Editor.GameProject");
            ProjectRunning = false;
        });
    }

    public static string SerializeObject(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented,
            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
    }

    public static T DeserializeObject<T>(string text)
    {
        return JsonConvert.DeserializeObject<T>(text,
            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
    }

    public static void RunDotnetCommand(string command)
    {
        ProcessStartInfo psi = new ProcessStartInfo("/usr/bin/dotnet")
        {
            Arguments = command,
            RedirectStandardOutput = false,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process? process = Process.Start(psi);
        process?.WaitForExit();
    }

    public static void RunProcess(string processName)
    {
        ProcessStartInfo psi = new ProcessStartInfo(processName)
        {
            RedirectStandardOutput = false,
            UseShellExecute = false,
        };

        using Process? process = Process.Start(psi);
        process?.WaitForExit();
    }
}