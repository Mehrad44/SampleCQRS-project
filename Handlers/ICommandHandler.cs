using System;

namespace OrdersAPI.Handlers;

public interface ICommandHandler<TCommand , TResult> where TCommand : notnull
{
        Task<TResult> HandleAsync(TCommand command);
        

}
