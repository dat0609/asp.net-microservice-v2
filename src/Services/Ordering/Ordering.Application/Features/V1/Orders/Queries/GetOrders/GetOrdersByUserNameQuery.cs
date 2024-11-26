using MediatR;
using Ordering.Application.Common.Models;
using Ordering.Domain.Entities;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders.Queries.GetOrders;

public class GetOrdersByUserNameQuery : IRequest<List<Order>>
{
    public string UserName { get; set; }
}