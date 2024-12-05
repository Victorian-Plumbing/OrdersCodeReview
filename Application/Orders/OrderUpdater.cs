using Application.Orders.Mappers;
using Application.Outbox;
using Client.Dtos;
using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public class OrderUpdater(
    IRepository<Order> orderRepo,
    IRepository<Variant> variantRepo,
    IRepository<OutboxMessage> outboxRepo,
    IOutboxMessageCreator outboxMessageCreator,
    IOutboxMessageSender outboxMessageSender,
    IUnitOfWork unitOfWork)
    : IOrderUpdater
{
    private readonly IRepository<Order> _orderRepo = orderRepo;
    private readonly IRepository<Variant> _variantRepo = variantRepo;
    private readonly IRepository<OutboxMessage> _outboxRepo = outboxRepo;
    private readonly IOutboxMessageCreator _outboxMessageCreator = outboxMessageCreator;
    private readonly IOutboxMessageSender _outboxMessageSender = outboxMessageSender;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public OrderDto UpdateOrder(UpdateOrderRequestDto request)
    {
        //What is the order doesn't exist?
        //Can do a GET here to check existsance, throw a 404 if not found
        var order = _orderRepo.Get(x => x.OrderNumber == request.OrderNumber)
                              .Include(x => x.OrderItems)
                                .ThenInclude(x => x.Variant)
                                .ThenInclude(x => x.Product)
                              .Include(x => x.Customer)
                              .Include(x => x.ShippingAddress)
                              .Include(x => x.BillingAddress)
                              .SingleOrDefault()
            ;

        var skus = request.OrderItems.Select(x => x.Sku).ToList();
        var variants = _variantRepo.Get(x => skus.Contains(x.Sku))
                                   .Include(i => i.Product)
                                   .ToList();

        var orderItems = variants.Join(request.OrderItems,
                                       var => var.Sku,
                                       item => item.Sku,
                                       (variant, item) => new
                                       {
                                           variant,
                                           item
                                       })
                                 .ToDictionary(x => x.variant, x => x.item.Quantity);
        //The null! op here hasn't been checked
        //Since this is coming from a PUT command, it should do a full update of order items. Currently, it adds to a collection.
        //An existing order with 2 items, with an update command of 3 items will end up with 5 items. Should be changed to a PATCH command
        order!.UpdateItems(orderItems);
        order.UpdateShippingAddress(request.ShippingAddress.AddressLineOne,
                                    request.ShippingAddress.AddressLineTwo!,
                                    request.ShippingAddress.AddressLineThree!,
                                    request.ShippingAddress.PostCode);

        _unitOfWork.Save();

        var outboxMessage = outboxMessageCreator.Create<Order>(order);

        _outboxRepo.Insert(outboxMessage);

        _unitOfWork.Save();

        _outboxMessageSender.Send(outboxMessage);

        return new OrderDtoMapper().Map(order);
    } 
}
//Put this into it's own class ideally. It's not internal to the updater.
public class OrderReader(
    IRepository<Order> orderRepo,
    IUnitOfWork unitOfWork)
    : IOrderReader
{
    private readonly IRepository<Order> _orderRepo = orderRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public OrderDto ReadOrder(string orderNumber)
    {
        //This method is only ever used to read information from the dB (no writes)
        //Use the AsNoTracking to improve performance and not have to track model binding
        var order = _orderRepo.Get(x => x.OrderNumber == orderNumber)
                              .Include(x => x.OrderItems)
                                .ThenInclude(x => x.Variant)
                                .ThenInclude(x => x.Product)
                              .Include(x => x.Customer)
                              .Include(x => x.ShippingAddress)
                              .Include(x => x.BillingAddress)
                              .SingleOrDefault();

        _unitOfWork.Save();
        //Want to check if the order is null before doing any data mapping
        return new OrderDtoMapper().Map(order);
    }
}