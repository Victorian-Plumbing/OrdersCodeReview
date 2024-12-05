namespace Client.Dtos;

public class CreateOrderItemDto
{
    //Unable to pass through the value of what has been paid for the individual items.
    //Might be covered in a larger app at an "orderTotal" layer, but for consistency, I would always
    //try and track what has been paid for at a unit level for the following
    // - Unit price without tax
    // - Unit tax value
    // - Quantity
    //  - Line total (unit * quantity) without tax
    // - Unit tax value
    // - Line tax total
    // - Line reductions (marketing promos / coupons / sales / markdowns)
    // - Line tax % (thinking of tax write offs in airports / VAT registered companies needing invoices
    // - Additionally, tax implications in fiscal countries, but unsure on implications online
    public string Sku { get; set; }
    public int Quantity { get; set; }
}