namespace Domain;

public class Variant
{
    private Variant(){}

    //A number of the properties here aren't being set, causes issues for the orderItem return when calling the API
    public Variant(Product product,
                   string name,
                   string sku,
                   decimal price)
    {
        ExternalId = Guid.NewGuid();
        
        ProductId = product.ProductId;
        
        Name = name;
        Sku = sku;
        Price = price;
    }

    public int VariantId { get; set; /* trade off for seeding */}
    public Guid ExternalId { get; private set; }
    public int ProductId { get; private set; }
    public string Name { get; private set; }
    public string Sku { get; private set; }

    //Depending on the domain, it might be preferable to use long to represent the price. 
    //In addition, is this the unit price, the line price, a sub-total pre-tax, a price with a discount?
    //I've found being overly explicit with the naming for stuff like this to pay off in the long-run
    public decimal Price { get; private set; }

    public Product Product { get; private set; }
    public ICollection<OrderItem> OrderItems { get; private set; }
}