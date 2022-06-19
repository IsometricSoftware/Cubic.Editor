using System;
using System.Collections.Generic;
using System.Drawing;

namespace Cubic.Editor.DataStorage;

public struct EditorConfig
{
    public Size LauncherSize;
    public Size EditorSize;
    public Point LauncherLocation;
    public Point EditorLocation;
    public Dictionary<string, DateTime> PreviousFiles;

    public EditorConfig()
    {
        LauncherSize = new Size(500, 500);
        EditorSize = new Size(1280, 720);
        LauncherLocation = new Point(-1, -1);
        EditorLocation = new Point(-1, -1);
        PreviousFiles = new Dictionary<string, DateTime>();
    }
}