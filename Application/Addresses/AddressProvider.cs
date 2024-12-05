using Client.Dtos;
using DataAccess;
using Domain;

namespace Application.Addresses;

public class AddressProvider(IRepository<Address> addressRepo,
                             IUnitOfWork unitOfWork) : IAddressProvider
{
    private readonly IRepository<Address> _addressRepo = addressRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public Tuple<Address, Address> GetAddresses(AddressDto billingAddressDto,
                                                AddressDto shippingAddressDto)
    {
        var billingAddressHash = Address.GenerateAddressHash(billingAddressDto.AddressLineOne,
                                                             billingAddressDto.AddressLineTwo!,
                                                             billingAddressDto.AddressLineThree!,
                                                             billingAddressDto.PostCode);

        var shippingAddressHash = Address.GenerateAddressHash(shippingAddressDto.AddressLineOne,
                                                              shippingAddressDto.AddressLineTwo!,
                                                              shippingAddressDto.AddressLineThree!,
                                                              shippingAddressDto.PostCode);

        var lookupHashes = new Guid[] { billingAddressHash, shippingAddressHash };
        //This seems like a poor way of checking for addres information in the dB. 
        //If address records are being stored individually (as addresses, and not split by billing and shipping), look them up independantly
        //This will allow for more performant lookups
        //You can also check programatically if the addresses are the same and reduce data lookups (arguable if this is beneficial)
        var addresses = addressRepo.Get(x => lookupHashes.Contains(x.Hash));

        var billingAddress = addresses.SingleOrDefault(x => x.Hash == billingAddressHash)
                             ?? CreateAddress(billingAddressDto.AddressLineOne,
                                              billingAddressDto.AddressLineTwo!,
                                              billingAddressDto.AddressLineThree!,
                                              billingAddressDto.PostCode);

        var shippingAddress = addresses.SingleOrDefault(x => x.Hash == shippingAddressHash)
                              ?? CreateAddress(shippingAddressDto.AddressLineOne,
                                               shippingAddressDto.AddressLineTwo!,
                                               shippingAddressDto.AddressLineThree!,
                                               shippingAddressDto.PostCode);

        return new Tuple<Address, Address>(billingAddress, shippingAddress);
    }

    private Address CreateAddress(string lineOne,
                                  string lineTwo,
                                  string lineThree,
                                  string postCode)
    {
        var address = new Address(lineOne,
                                  lineTwo,
                                  lineThree,
                                  postCode);

        _addressRepo.Insert(address);

        _unitOfWork.Save();

        return address;
    }
}