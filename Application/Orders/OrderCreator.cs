using Application.Addresses;
using Application.Customers;
using Application.Exceptions;
using Application.Orders.Mappers;
using Application.Orders.Validators;
using Application.Outbox;
using Client.Dtos;
using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public class OrderCreator(
    ICustomerProvider customerProvider,
    IAddressProvider addressProvider,
    ICreateOrderRequestValidator requestValidator,
    IRepository<Order> orderRepo,
    IRepository<Variant> variantRepo,
    IRepository<OutboxMessage> outboxRepo,
    IOutboxMessageCreator outboxMessageCreator,
    IOutboxMessageSender outboxMessageSender,
    IUnitOfWork unitOfWork)
    : IOrderCreator
{
    //Using a default constructor behind the scenes creates a private property for the class. However, this property isn't read-only.
    //To be more strict on what can be used, fall back to using a parametered constructor
    private readonly ICustomerProvider customerProvider = customerProvider;
    private readonly IAddressProvider addressProvider = addressProvider;
    private readonly IRepository<Order> _orderRepo = orderRepo;
    private readonly IRepository<OutboxMessage> _outboxRepo = outboxRepo;
    private readonly IOutboxMessageCreator _outboxMessageCreator = outboxMessageCreator;
    private readonly IOutboxMessageSender _outboxMessageSender = outboxMessageSender;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public OrderDto CreateOrder(CreateOrderRequestDto request)
    {
        var customer = customerProvider.GetCustomer(request.Customer);
        var addresses = addressProvider.GetAddresses(request.Customer.BillingAddress,
                                                     request.Customer.ShippingAddress);

        if (!requestValidator.TryValidate(customer, addresses.Item1, addresses.Item2, out var errors))
            throw new ValidationException("Validation failed", errors);

        //Addresses here are out of sync. Move back to getting data from the request.
        //Alternatively, don't use Tuples, and return anonymous types / parameters that are better named
        var order = new Order(customer,
                              addresses.Item1,
                              addresses.Item2);

        
        _orderRepo.Insert(order);

        var products = GetProducts(request.OrderItems);

        order.UpdateItems(products);
        _unitOfWork.Save();

        //This should call to the readonly property
        var outboxMessage = outboxMessageCreator.Create<Order>(order);

        _outboxRepo.Insert(outboxMessage);

        //Is there a reason why this is called multiple times here?
        _unitOfWork.Save();

        _outboxMessageSender.Send(outboxMessage);

        //You need to call and get the order information so that you can return the order ID (and any additional information) back to the caller
        return new OrderDtoMapper().Map(order);
    }

    private IDictionary<Variant, int> GetProducts(ICollection<CreateOrderItemDto> items)
    {
        //Depending on the make up of product & variant information in the back-end, it might be worth checking for
        //only distinct product entities, then re-mapping to the order item collection
        var variants = new HashSet<Variant>();
        foreach (var item in items)
        {
            var variant = variantRepo.Get(x => x.Sku == item.Sku)
                                     .Include(i => i.Product)
                                     .Single();
            variants.Add(variant);
        }

        var requestedSkus = items.Select(x => x.Sku);

        var missingSkus = requestedSkus.ExceptBy(variants.Select(x => x.Sku), sku => sku).ToList();

        //This would be a lot easier to validate when checking each variant first. If the first item isn't found out of 1000, you need to look up all 1000 items
        if (missingSkus.Any())
            throw new ValidationException("Request failed validation",
                                          new Dictionary<string, string>()
                                          {
                                              [nameof(missingSkus)] = string.Join(',', missingSkus)
                                          });

        return variants.Join(items,
                             x => x.Sku.ToUpperInvariant(),
                             x => x.Sku.ToUpperInvariant(),
                             (variant,
                              requestItem) => new
                              {
                                  variant,
                                  requestItem
                              })
                       .ToDictionary(x => x.variant, x => x.requestItem.Quantity);
    }
}