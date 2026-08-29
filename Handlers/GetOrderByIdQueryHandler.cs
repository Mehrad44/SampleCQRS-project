using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrdersAPI.Data;
using OrdersAPI.Dtos;
using OrdersAPI.Models;
using OrdersAPI.Queries;

namespace OrdersAPI.Handlers;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery , OrderDto?>
{
    private readonly ReadDbContext _context;

    // public static async Task<Order?> Handle(GetOrderByIdQuery query , AppDbContext context)
    // {
    //     return await context.Orders.FirstOrDefaultAsync(o => o.Id == query.OrderId );
    // }

    public GetOrderByIdQueryHandler(ReadDbContext context)
    {
        _context = context;
    }


    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
      var order = await _context.Orders
      .AsNoTracking()
      .FirstOrDefaultAsync(o => o.Id == request.OrderId);

      if(order == null)
            return null;


      return new OrderDto
      (
          order.Id,
          order.FirstName,
          order.LastName,
          order.Status,
          order.CreatedAt,
          order.TotalCost
      );
    }
    
}
