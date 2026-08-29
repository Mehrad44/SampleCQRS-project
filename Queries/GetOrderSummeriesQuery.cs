using System;
using MediatR;
using OrdersAPI.Dtos;

namespace OrdersAPI.Queries;

public record GetOrderSummeriesQuery():IRequest<List<OrderSummaryDto>>;
