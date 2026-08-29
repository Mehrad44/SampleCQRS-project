using System;

namespace OrdersAPI.Dtos;

public record OrderDto
(
    int Id,
    string FirstName,
    string LastName,
    string Status,
    DateTime CreatedAt,
    Decimal TotalCost

);
