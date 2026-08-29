using System;
using MediatR;
using OrdersAPI.Dtos;

namespace OrdersAPI.Queries;

public record GetOrderByIdQuery(int OrderId): IRequest<OrderDto?>;