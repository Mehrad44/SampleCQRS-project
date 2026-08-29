using System;
using MediatR;
using OrdersAPI.Dtos;

namespace OrdersAPI.Commands;

public record CreateOrderCommand(string FirstName , string LastName , string Status,decimal TotalCost) : IRequest<OrderDto>;

