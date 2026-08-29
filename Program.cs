using System.Reflection.Metadata;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrdersAPI.Commands;
using OrdersAPI.Data;
using OrdersAPI.Dtos;
using OrdersAPI.Events;
using OrdersAPI.Handlers;
using OrdersAPI.Models;
using OrdersAPI.Projections;
using OrdersAPI.Queries;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddDbContext<AppDbContext>(opt =>
//     opt.UseSqlite(builder.Configuration.GetConnectionString("BaseConnection")));
builder.Services.AddDbContext<ReadDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("ReadDbConnection")));
builder.Services.AddDbContext<WriteDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("WriteDbConnection")));

// builder.Services.AddScoped<ICommandHandler<CreateOrderCommand,OrderDto>,CreateOrderCommandHandler>();
// builder.Services.AddScoped<IQueryHandler<GetOrderByIdQuery,OrderDto>,GetOrderByIdQueryHandler>();
builder.Services.AddScoped<IValidator<CreateOrderCommand>,CreateOrderCommandValidator>();
// builder.Services.AddScoped<IQueryHandler<GetOrderSummeriesQuery,List<OrderSummaryDto>>,GetOrderSummeriesQueryHandler>();
// builder.Services.AddSingleton<IEventPublisher,InProcessEventPublisher>();
// builder.Services.AddScoped<IEventHandler<OrderCreatedEvent>,OrderCreatedProjectionHandler>();


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

app.MapPost("/api/orders",async(IMediator mediator,CreateOrderCommand command) =>
{
    try
    {
            var createdOrder = await mediator.Send(command);
    if(createdOrder == null)
    {
        return Results.BadRequest("Failed to create order");
    }

    return Results.Created($"/api/orders{createdOrder.Id}",createdOrder);
    }
    catch(ValidationException ex)
    {
        var errors = ex.Errors
            .Select(e => (PropertyName: e.PropertyName, ErrorMessage: e.ErrorMessage))
            .ToList();

        return Results.BadRequest(errors);
    }

    
});

app.MapGet("/api/orders/{id}",async(IMediator mediator ,int id) =>
{

    // var order = await GetOrderByIdQueryHandler.Handle(new OrdersAPI.Queries.GetOrderByIdQuery(id),context);
    // var order = await handler.HandleAsync(new GetOrderByIdQuery(id));
    var order = await mediator.Send(new GetOrderByIdQuery(id));
   if(order == null )
        return Results.NotFound();

    return Results.Ok(order);
});

app.MapGet("/api/orders", async (IMediator mediator) =>
{
   var summeries = await mediator.Send(new GetOrderSummeriesQuery());

    return Results.Ok(summeries);
});


app.Run();

