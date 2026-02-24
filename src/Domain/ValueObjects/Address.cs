using Domain.Commons;

namespace Domain.ValueObjects;

public sealed class Address
{
    public string Street { get; }
    public string City { get; }
    public string Country { get; }
    public string ZipCode { get; }

    private Address(string street, string city, string country, string zipCode)
    {
        Street = street;
        City = city;
        Country = country;
        ZipCode = zipCode;
    }

    public static Result<Address> Create(string street, string city, string country, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            return Result<Address>.Failure(AddressErrors.StreetRequired);

        if (string.IsNullOrWhiteSpace(city))
            return Result<Address>.Failure(AddressErrors.CityRequired);

        if (string.IsNullOrWhiteSpace(country))
            return Result<Address>.Failure(AddressErrors.CountryRequired);

        return Result<Address>.Success(new Address(street.Trim(), city.Trim(), country.Trim(), zipCode?.Trim() ?? ""));
    }
}

public static class AddressErrors
{
    public static readonly Error StreetRequired = new("Address.StreetRequired", "La calle es obligatoria.");
    public static readonly Error CityRequired = new("Address.CityRequired", "La ciudad es obligatoria.");
    public static readonly Error CountryRequired = new("Address.CountryRequired", "El país es obligatorio.");
}
