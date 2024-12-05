namespace Inventory.Product.API.Extensions;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class BsonCollectionAttribute : Attribute
{
    public string CollectionName { get; set; }

    public BsonCollectionAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }
}