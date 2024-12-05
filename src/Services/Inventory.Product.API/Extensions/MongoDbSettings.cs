using Shared.Configurations;

namespace Inventory.Product.API.Extensions;

public class MongoDbSettings : DatabaseSettings
{
    public string DatabaseName { get; set; }
}