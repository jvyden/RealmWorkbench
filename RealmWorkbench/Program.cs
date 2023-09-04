using Realms;
using Realms.Schema;

string path = string.Join(' ', args);
Console.Write("Path: ");
Console.WriteLine(path);

RealmConfiguration configuration = new RealmConfiguration(path)
{
    IsDynamic = true,
    ShouldDeleteIfMigrationNeeded = false,
    IsReadOnly = true,
};

Realm realm = Realm.GetInstance(configuration);

foreach (ObjectSchema schema in realm.Schema)
{
    Console.WriteLine($"{schema.Name} ({schema.Count})");
    foreach (Property property in schema)
    {
        string type = string.IsNullOrEmpty(property.ObjectType) ? property.Type.ToString() : property.ObjectType;
        if ((property.Type & PropertyType.Array) != 0) type += "[]";
        Console.WriteLine($"  {property.Name}: {type}");
    }
}