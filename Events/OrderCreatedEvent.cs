using System;
using MediatR;

namespace OrdersAPI.Events
{
    public record OrderCreatedEvent
    (
        int OrderId,
        string FirstName,
        string LastName,
        decimal TotalCost
    ) : INotification;
}