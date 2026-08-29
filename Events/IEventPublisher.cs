using System;

namespace OrdersAPI.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent evt);

    }
}