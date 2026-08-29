using System;

namespace OrdersAPI.Handlers;

public interface IQueryHandler<TQuery,TResult>
{
    Task<TResult?> HandleAsync(TQuery query);
}
