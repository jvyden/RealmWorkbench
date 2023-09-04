using Eto.Drawing;
using Eto.Forms;
using Realms;
using Realms.Schema;

namespace RealmWorkbench.Forms;

public class RealmViewerForm : Form
{
    private readonly Realm _realm;
    private readonly ListBox _tables;
    
    public RealmViewerForm(string realmPath)
    {
        this.Title = "Realm Workbench - " + Path.GetFileName(realmPath);
        this.Size = new Size(1280, 720);

        this.Content = new StackLayout
        (
            _tables = new ListBox
            {
                Size = new Size(256, -1),
            }
        )
        {
            Orientation = Orientation.Horizontal,
            VerticalContentAlignment = VerticalAlignment.Stretch,
            Padding = 10,
            Spacing = 5,
        };
        
        RealmConfiguration configuration = new RealmConfiguration(realmPath)
        {
            IsDynamic = true,
            ShouldDeleteIfMigrationNeeded = false,
            IsReadOnly = true
        };

        this._realm = Realm.GetInstance(configuration);
        this.UpdateSchema();

        // foreach (ObjectSchema schema in realm.Schema)
        // {
        //     Console.WriteLine($"{schema.Name} ({schema.Count})");
        //     foreach (Property property in schema)
        //     {
        //         string type = string.IsNullOrEmpty(property.ObjectType) ? property.Type.ToString() : property.ObjectType;
        //         if ((property.Type & PropertyType.Array) != 0) type += "[]";
        //         Console.WriteLine($"  {property.Name}: {type}");
        //     }
        // }
    }

    private void UpdateSchema()
    {
        foreach (ObjectSchema schema in this._realm.Schema)
        {
            if(schema.BaseType == ObjectSchema.ObjectType.EmbeddedObject) continue;
            this._tables.Items.Add(new ListItem { Text = schema.Name });
        }
    }
}