using System.Drawing;
using System.IO;
using Cubic.GUI;

namespace Cubic.Editor.Screens;

public class FileExplorer : Screen
{
    public event OnFileExplorerLoad Load;

    private ExplorerType _type;
    
    public FileExplorer(ExplorerType type)
    {
        _type = type;
    }
    
    protected override void Initialize()
    {
        base.Initialize();

        View view = new View(Anchor.Center, new Rectangle(0, 0, 500, 500))
        {
            Visible = false
        };

        TextBox fileLocation = new TextBox(Anchor.TopLeft, new Rectangle(10, 10, 300, 30), "File location", 18);
        view.AddElement("FileLocation", fileLocation);

        Button loadButton = new Button(Anchor.TopRight, new Rectangle(-10, 10, 50, 30), _type.ToString(), 18);
        loadButton.Click += LoadClick;
        view.AddElement("LoadButton", loadButton);

        UI.AddElement($"FileExplorerView{_type}", view);
    }

    private void LoadClick()
    {
        string path = UI.GetElement<View>($"FileExplorerView{_type}").GetElement<TextBox>("FileLocation").Text;

        switch (_type)
        {
            case ExplorerType.Open:
                if (Directory.Exists(path))
                    Load?.Invoke(path);
                break;
            case ExplorerType.Save:
                Load?.Invoke(path);
                break;
        }
    }

    protected override void Open()
    {
        base.Open();

        UI.GetElement<View>($"FileExplorerView{_type}").Visible = true;
    }

    protected override void Close()
    {
        base.Close();

        UI.GetElement<View>($"FileExplorerView{_type}").Visible = false;
    }

    protected override void Update()
    {
        base.Update();
        
        if (Input.KeyPressed(Keys.Escape))
            Close();
    }

    public delegate void OnFileExplorerLoad(string path);

    public enum ExplorerType
    {
        Open,
        Save
    }
}