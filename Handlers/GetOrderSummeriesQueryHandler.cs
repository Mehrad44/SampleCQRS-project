using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrdersAPI.Data;
using OrdersAPI.Dtos;
using OrdersAPI.Queries;

namespace OrdersAPI.Handlers;

public class GetOrderSummeriesQueryHandler : IRequestHandler<GetOrderSummeriesQuery, List<OrderSummaryDto>>
{
    private readonly ReadDbContext _context;
    public GetOrderSummeriesQueryHandler(ReadDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderSummaryDto>> Handle(GetOrderSummeriesQuery request, CancellationToken cancellationToken)
    {
    {
       return await _context.Orders
                .AsNoTracking()
                .Select(o => new OrderSummaryDto(
                    o.Id,
                    o.FirstName + "" + o.LastName,
                    o.Status,
                    o.TotalCost
                )).ToListAsync();
    }
    }
}
