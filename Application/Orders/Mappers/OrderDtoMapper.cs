using Client.Dtos;
using Domain;

namespace Application.Orders.Mappers;

public class OrderDtoMapper : IOrderDtoMapper
{
    //For future extensions, consider having mapping processes for all the given entities here (address, order item etc).
    //This can then reduce the complexity of this mapper

    public OrderDto Map(Order order)
    {
        return new OrderDto
        {
            OrderNumber = order.OrderNumber,
            CustomerName = order.Customer.Name,
            PhoneNumber = order.Customer.PhoneNumber,
            BillingAddress = new AddressDto
            {
                AddressLineOne = order.BillingAddress.LineOne,
                AddressLineTwo = order.BillingAddress.LineTwo,
                AddressLineThree = order.BillingAddress.LineThree,
                PostCode = order.BillingAddress.PostCode,
            },
            ShippingAddress = new AddressDto
            {
                AddressLineOne = order.ShippingAddress.LineOne,
                AddressLineTwo = order.ShippingAddress.LineTwo,
                AddressLineThree = order.ShippingAddress.LineThree,
                PostCode = order.ShippingAddress.PostCode,
            },
            TotalCost = order.OrderItems.Sum(x => x.Quantity * x.Variant.Price),
            OrderItems = order.OrderItems.Select(x => new OrderItemDto
            {
                ProductId = x.Variant.Product.ProductId,
                ProductName = x.Variant.Product.Name,
                VariantId = x.Variant.VariantId,
                VariantName = x.Variant.Name,
                Sku = x.Variant.Sku,
                ImageUrl = x.Variant.Product.ImageUrl,
                Quantity = x.Quantity,
                UnitPrice = x.Variant.Price,
                TotalPrice = x.Variant.Price * x.Quantity // The value here doesn't preserve a pence value.
                                                          // Ie "£12.30" would be "12.3". Might be important depending on presentation
                                                          // Ideally, whatever client / consumer would handle the presentation

            }).ToList(),
            //Since ICollection can be an array or a list, and the client can't infer the underlying type, pass as an array to reduce overhead
        };
    }
}