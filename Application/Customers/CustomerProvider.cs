using Client.Dtos;
using DataAccess;
using Domain;

namespace Application.Customers;

public class CustomerProvider(IRepository<Customer> customerRepo,
                              IUnitOfWork unitOfWork) : ICustomerProvider
{
    private readonly IRepository<Customer> _customerRepo = customerRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public Customer GetCustomer(CustomerDto request)
    {
        //For the string comparison here, do x.Email.Equals(request.EmailAddress, StringComparison.OrdinalIgnoreCase) for less stack allocation, less GC calls
        var customer = _customerRepo.Get(x => x.Email == request.EmailAddress.ToLowerInvariant())
                                    .SingleOrDefault();

        if (customer is not null)
            return customer;

        //It could be argued that this method shouldn't create customer information. 
        //Try to uphold SRP. Might be worth returning back a null Customer (change response type to nullable), and allow the calling code to call this. 
        //Can also call to the shared outbox service in the case the caller knows the customer has been created. 
        //Might be a candidate for pushing to a seperate customer api / micro-service which handles this processing
        customer = new Customer(request.EmailAddress,
                                request.CustomerName,
                                request.PhoneNumber);

        //Calling to the constructor parameter, which is mutable. I think you either want to always call to the mutable instance,
        //Or, remove the default constructor, and use a legacy constructor, with properly defended read-only parameters.
        //Since it's legal to do something like customerRepo = null;
        customerRepo.Insert(customer);

        _unitOfWork.Save();

        return customer;
    }
}