using Eto.Forms;
using Realms;
using Realms.Schema;
using RealmWorkbench.Forms;

namespace RealmWorkbench;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        using Application app = new();
        
        string path = string.Join(' ', args);
        if (!string.IsNullOrWhiteSpace(path))
        {
            app.Run(new RealmViewerForm(path));
        }
        
        app.Run(new MainForm());
    }
}