using MediatR;
using Ordering.Application.Common.Models;

namespace Ordering.Application.Features.V1.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<OrderDto>
{
    public long Id { get; set; }

    public GetOrderByIdQuery(long id)
    {
        Id = id;
    }
}