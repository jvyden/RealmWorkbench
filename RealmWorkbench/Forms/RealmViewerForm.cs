using Eto.Drawing;
using Eto.Forms;
using MongoDB.Bson;
using Realms;
using Realms.Schema;
using RealmWorkbench.ListItems;

namespace RealmWorkbench.Forms;

public class RealmViewerForm : Form
{
    private readonly Realm _realm;
    private readonly ListBox _tables;
    private readonly GridView _data;
    
    
    public RealmViewerForm(string realmPath)
    {
        this.Title = "Realm Workbench - " + Path.GetFileName(realmPath);
        this.Size = new Size(1280, 720);

        _tables = new ListBox
        {
            Size = new Size(256, -1),
        };
        _data = new GridView
        {
            Size = new Size(-1, -1),
        };

        this.Content = new TableLayout
        {
            Padding = 10,
            Spacing = new Size(5, 5),
            Rows =
            {
                new TableRow
                {
                    Cells =
                    {
                        _tables,
                        _data,
                    }
                }
            }
        };

        this._tables.SelectedValueChanged += (_, _) =>
        {
            this.UpdateData();
        };
        
        RealmConfiguration configuration = new RealmConfiguration(realmPath)
        {
            IsDynamic = true,
            ShouldDeleteIfMigrationNeeded = false,
            IsReadOnly = true
        };

        this._realm = Realm.GetInstance(configuration);
        this.UpdateSchema();
    }

    private void UpdateSchema()
    {
        this._tables.Items.Clear();
        foreach (ObjectSchema schema in this._realm.Schema)
        {
            if(schema.BaseType == ObjectSchema.ObjectType.EmbeddedObject) continue;
            this._tables.Items.Add(new SchemaListItem(schema));
        }
    }

    private void UpdateData()
    {
        SchemaListItem schemaItem = (this._tables.SelectedValue as SchemaListItem)!;
        this._data.Columns.Clear();

        int i = 0;
        foreach (Property property in schemaItem.Schema)
        {
            string name = property.Name;
            if ((property.Type & PropertyType.Array) != 0) name += "[]";
            
            this._data.Columns.Add(new GridColumn
            {
                HeaderText = name,
                DataCell = new TextBoxCell(i)
            });

            i++;
        }

        List<GridItem> items = new();
        foreach (IRealmObject realmObject in this._realm.DynamicApi.All(schemaItem.Schema.Name))
        {
            DynamicObjectApi accessor = realmObject.DynamicApi;

            List<object> values = new();
            foreach (Property property in schemaItem.Schema)
            {
                object? obj = null;
                PropertyType type = property.Type;

                if (TypeMatches(type, PropertyType.Object))
                {
                    values.Add("");
                    continue;
                }
                
                if (TypeMatches(type, PropertyType.ObjectId)) obj = accessor.Get<ObjectId?>(property.Name).ToString();
                else if (TypeMatches(type, PropertyType.String)) obj = accessor.Get<string?>(property.Name);
                else if (TypeMatches(type, PropertyType.Bool)) obj = accessor.Get<bool?>(property.Name);
                else if (TypeMatches(type, PropertyType.Date)) obj = accessor.Get<DateTimeOffset?>(property.Name).ToString();
                else if (TypeMatches(type, PropertyType.Int)) obj = accessor.Get<long?>(property.Name);
                else obj = $"Unimplemented type '{type}'";
                    
                if(obj == null) obj = "null";
                values.Add(obj);
            }
            
            items.Add(new GridItem
            {
                Values = values.ToArray()
            });
        }

        this._data.DataStore = items;
    }

    private static bool TypeMatches(PropertyType type, PropertyType type2) => (type & type2) == type2;
}