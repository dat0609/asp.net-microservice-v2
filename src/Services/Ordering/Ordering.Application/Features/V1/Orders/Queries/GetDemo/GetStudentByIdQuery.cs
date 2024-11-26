using MediatR;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.V1.Orders.Queries.GetDemo;

public class GetStudentByIdQuery: IRequest<Order>
{
    public int Id { get; set; }
}