using Eto.Forms;
using Realms.Schema;

namespace RealmWorkbench.ListItems;

public class SchemaListItem : ListItem
{
    public ObjectSchema Schema { get; init; }
    
    public SchemaListItem(ObjectSchema schema)
    {
        this.Text = schema.Name;
        this.Schema = schema;
    }
}