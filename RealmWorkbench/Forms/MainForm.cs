using System.Collections;
using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace RealmWorkbench.Forms;

public class MainForm : Form
{
    private readonly FilePicker _filePicker;
    private readonly Button _openButton;
    
    public MainForm()
    {
        this.Title = "Realm Workbench";
        this.Content = new StackLayout
        (
            _filePicker = new FilePicker(),
            _openButton = new Button(this.OpenRealm)
            {
                Text = "Open",
                Enabled = false,
            }
        )
        {
            Size = new Size(512, -1),
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Orientation = Orientation.Vertical,
            Padding = 10,
            Spacing = 5,
        };

        _filePicker.FileAction = FileAction.OpenFile;
        _filePicker.Filters.Add(new FileFilter("Realm Files", "*.realm", "*.rlm", "*.db"));
        _filePicker.Filters.Add(new FileFilter("All Files", "*.*", "*"));

        _filePicker.FilePathChanged += (_, _) =>
        {
            this._openButton.Enabled = File.Exists(_filePicker.FilePath);
        };
    }

    private void OpenRealm(object? sender, EventArgs e)
    {
        string path = _filePicker.FilePath;
        RealmViewerForm form = new(path);
        form.Show();
        this.Visible = false;
    }
}