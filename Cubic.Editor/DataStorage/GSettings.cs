using System.Drawing;
using Cubic.Audio;
using Cubic.Render;
using Cubic.Utilities;
using Cubic.Windowing;

namespace Cubic.Editor.DataStorage;

public class GSettings
{
    public Size Size;
    
    public string Title;
    
    public WindowMode WindowMode;
    
    public bool Resizable;
    
    public uint RefreshRate;
    
    public bool Vsync;
    
    public uint TargetFps;
    
    public Point Location;
    
    public GraphicsApi GraphicsApi;
    
    public int AudioChannels;
    
    public Bitmap Icon;
    
    public uint MsaaSamples;
    
    public bool CreateDefaultFont;

    public GSettings(GameSettings settings)
    {
        Size = settings.Size;
        Title = settings.Title;
        WindowMode = settings.WindowMode;
        Resizable = settings.Resizable;
        RefreshRate = settings.RefreshRate;
        Vsync = settings.VSync;
        TargetFps = settings.TargetFps;
        Location = settings.Location;
        GraphicsApi = settings.GraphicsApi;
        AudioChannels = settings.AudioChannels;
        Icon = settings.Icon;
        MsaaSamples = settings.MsaaSamples;
        CreateDefaultFont = settings.CreateDefaultFont;
    }

    public GameSettings ToGameSettings()
    {
        return new GameSettings()
        {
            Size = Size,
            Title = Title,
            WindowMode = WindowMode,
            Resizable = Resizable,
            RefreshRate = RefreshRate,
            VSync = Vsync,
            TargetFps = TargetFps,
            Location = Location,
            GraphicsApi = GraphicsApi,
            AudioChannels = AudioChannels,
            Icon = Icon,
            MsaaSamples = MsaaSamples,
            CreateDefaultFont = CreateDefaultFont
        };
    }
}