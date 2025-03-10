using AutoMapper;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Inventory.Product.API.Entities;
using Inventory.Product.API.Extensions;
using Inventory.Product.API.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Configurations;
using Shared.DTOs.Inventory;
using Shared.SeedWork;

namespace Inventory.Product.API.Services;

public class InventoryService : MongoDbRepository<InventoryEntry>, IInventoryService
{
    private readonly IMapper _mapper;
    public InventoryService(IMongoClient client, MongoDbSettings settings, IMapper mapper) : base(client, settings)
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo)
    {
        var entities = await FindAll()
            .Find(x => x.ItemNo == itemNo)
            .ToListAsync();
        var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);

        return result;
    }

    public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query)
    {
        var filterSearchTerm = Builders<InventoryEntry>.Filter.Empty;
        var filterItemNo = Builders<InventoryEntry>.Filter.Eq(s => s.ItemNo, query.ItemNo());
        if (!string.IsNullOrEmpty(query.SearchTerm))
            filterSearchTerm = Builders<InventoryEntry>.Filter.Eq(s => s.DocumentNo, query.SearchTerm);

        var andFilter = filterItemNo & filterSearchTerm; 
       
        var pagedList = await Collection.PaginatedListAsync(andFilter, pageIndex: query.PageNumber, pageSize: query.PageSize);        
        var items = _mapper.Map<IEnumerable<InventoryEntryDto>>(pagedList);
        var result = new PagedList<InventoryEntryDto>(items, pagedList.GetMetaData().TotalItems, query.PageNumber,
            query.PageSize);
        return result;
    }

    public async Task<InventoryEntryDto> GetByIdAsync(string id)
    {
        FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Eq(s => s.Id, id);
        var entity = await FindAll().Find(filter).FirstOrDefaultAsync();
        var result = _mapper.Map<InventoryEntryDto>(entity);

        return result;
    }

    public async Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model)
    {
        var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
        {
            ItemNo = itemNo,
            Quantity = model.Quantity,
            DocumentType = model.DocumentType,
        };
        var entity = _mapper.Map<InventoryEntry>(itemToAdd);
        await CreateAsync(entity);
        var result = _mapper.Map<InventoryEntryDto>(entity);
        
        return result;
    }

    public async Task<InventoryEntryDto> SalesItemAsync(string itemNo, SalesProductDto model)
    {
        var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
        {
            ItemNo = itemNo,
            Quantity = -model.Quantity,
            DocumentType = model.DocumentType,
            ExternalDocumentNo = model.ExternalDocNo,
        };
        
        var entity = _mapper.Map<InventoryEntry>(itemToAdd);
        await CreateAsync(entity);
        var result = _mapper.Map<InventoryEntryDto>(entity);
        
        return result;
    }

    public async Task<string> SaleOrderAsync(SaleOrderDto dto)
    {
        var docNo = Guid.NewGuid().ToString();
        foreach (var saleItem in dto.SaleItems)
        {
            var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
            {
                DocumentNo = docNo,
                ItemNo = saleItem.ItemNo,
                Quantity = saleItem.Quantity * -1,
                DocumentType = saleItem.Type,
            };
            await CreateAsync(itemToAdd);
        }

        return docNo;
    }

    public async Task DeleteByDocNoAsync(string docNo)
    {
        var filter = Builders<InventoryEntry>.Filter.Eq(x => x.DocumentNo, docNo);
        await Collection.DeleteOneAsync(filter);
    }
}