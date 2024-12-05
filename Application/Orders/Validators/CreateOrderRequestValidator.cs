using System.Text.RegularExpressions;
using Domain;

namespace Application.Orders.Validators;

public class CreateOrderRequestValidator : ICreateOrderRequestValidator
{
    //I'd move the regex patterns to the appsettings layer, then pass in as an IOptions<RequestValidator>
    //or IOptionsSnapshot if you want to use some type of cached / distributed appsettings sink

    //This validator is ensuring consistency of information from the dB layer when retreiving information, this looks like trying to validate from the wrong approach.
    //The data should have been validated before it is submitted. Not saying that doing additional validation is incorrect, as being defensive is good.
    //However, imagine that somebody runs a migration script against your data, or an update is ran without a where clause.
    //You aren't returning bad data, but you also won't return any data.
    //I guess it's a discussion between breaking early or trying to be flexible with (possibly) wrong response data

    //^ Depends a lot on business context & current behaviour in the domain

    public bool TryValidate(Customer customer,
                            Address billingAddress,
                            Address shippingAddress,
                            out IDictionary<string, string> errors)
    {
        errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(customer.Name))
            errors.Add(nameof(customer.Name), "Name is required");

        if (!Regex.IsMatch(customer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            errors.Add(nameof(customer.Email), "Email is not valid");

        //Depending on the domain, you can have errors with matching dateTime information
        //Consider different timezones for the hosted API and the dB layer
        if (customer.Created > DateTime.Now)
            errors.Add(nameof(customer.Created), "Customer cannot be from the future");

        if (string.IsNullOrWhiteSpace(billingAddress.LineOne))
            errors.Add(nameof(billingAddress.LineOne), "First address line is required");

        if (string.IsNullOrWhiteSpace(billingAddress.PostCode))
            errors.Add(nameof(billingAddress.PostCode), "PostCode is required");

        if (!Regex.IsMatch(billingAddress.PostCode,
                           @"^(GIR 0AA|((([A-Z]{1,2}[0-9][0-9A-Z]?)|(([A-Z]{1,2}[0-9][0-9A-Z]?)))(\s?[0-9][A-Z]{2})))$",
                           RegexOptions.IgnoreCase))
            errors.Add(nameof(billingAddress.PostCode), "Postcode is not valid");

        if (string.IsNullOrWhiteSpace(shippingAddress.LineOne))
            errors.Add(nameof(shippingAddress.LineOne), "First address line is required");

        if (string.IsNullOrWhiteSpace(shippingAddress.PostCode))
            errors.Add(nameof(shippingAddress.PostCode), "PostCode is required");

        if (!Regex.IsMatch(shippingAddress.PostCode,
                           @"^(GIR 0AA|((([A-Z]{1,2}[0-9][0-9A-Z]?)|(([A-Z]{1,2}[0-9][0-9A-Z]?)))(\s?[0-9][A-Z]{2})))$",
                           RegexOptions.IgnoreCase))
            errors.Add(nameof(shippingAddress.PostCode), "Postcode is not valid");

        return !errors.Any();
    }
}