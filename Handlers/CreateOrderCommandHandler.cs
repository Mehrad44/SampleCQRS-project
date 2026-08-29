using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using MediatR;
using Microsoft.VisualBasic;
using OrdersAPI.Commands;
using OrdersAPI.Data;
using OrdersAPI.Dtos;
using OrdersAPI.Events;
using OrdersAPI.Models;

namespace OrdersAPI.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand,OrderDto>
{
    // public static async Task<Order> Handle(CreateOrderCommand  command , AppDbContext context)
    // {
    //     var order = new Order
    //     {
    //       FirstName = command.FirstName,
    //       LastName = command.LastName,
    //       Status = command.Status,
    //       CreatedAt = DateTime.Now,
    //       TotalCost = command.TotalCost,


    //     };

    //     await context.Orders.AddAsync(order);
    //     await context.SaveChangesAsync();

    //     return order;
        

    // }

    private readonly WriteDbContext _context;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly IMediator _mediator;
    public CreateOrderCommandHandler(
        WriteDbContext context,
        IValidator<CreateOrderCommand> validator, 
        IMediator mediator)
    {
        _context = context;
        _validator = validator;
        _mediator = mediator;
    }


    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
    {
        var validatorResult = await _validator.ValidateAsync(request,cancellationToken);

        if (!validatorResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validatorResult.Errors);
        }

        var order = new Order
        {
          FirstName = request.FirstName,
          LastName = request.LastName,
          Status = request.Status,
          CreatedAt = DateTime.Now,
          TotalCost = request.TotalCost,


        };

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync(cancellationToken);

        var orderCreatedEvent = new OrderCreatedEvent
        (
            order.Id,
            order.FirstName,
            order.LastName,
            order.TotalCost

        );

        // await _eventPublisher.PublishAsync(orderCreatedEvent);
        
        await _mediator.Publish(orderCreatedEvent);

        return new OrderDto(
                  order.Id,
          order.FirstName,
          order.LastName,
          order.Status,
          order.CreatedAt,
          order.TotalCost
        );
    }
    }
}
