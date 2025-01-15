using Application.Orders.Mappers;
using Client.Dtos;
using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public class OrderReader(
    IRepository<Order> orderRepo,
    IUnitOfWork unitOfWork)
    : IOrderReader
{
    private readonly IRepository<Order> _orderRepo = orderRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public OrderDto ReadOrder(string orderNumber)
    {
        var order = _orderRepo.Get(x => x.OrderNumber == orderNumber)
                              .Include(x => x.OrderItems)
                              .ThenInclude(x => x.Variant)
                              .ThenInclude(x => x.Product)
                              .Include(x => x.Customer)
                              .Include(x => x.ShippingAddress)
                              .Include(x => x.BillingAddress)
                              .SingleOrDefault()
            ;

        _unitOfWork.Save();

        return new OrderDtoMapper().Map(order);
    }
}