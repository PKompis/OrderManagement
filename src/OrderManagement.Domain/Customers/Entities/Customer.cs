using OrderManagement.Domain.Common.Exceptions;

namespace OrderManagement.Domain.Customers.Entities;

/// <summary>
/// Customer Entitity
/// </summary>
public sealed class Customer
{
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the phone.
    /// </summary>
    public string PhoneNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the email.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    private Customer() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Customer"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="phoneNumber">The phone.</param>
    /// <param name="email">The email.</param>
    /// <exception cref="ValidationException">
    /// Name is required. - Customer.NameRequired
    /// or
    /// Phone is required. - Customer.PhoneRequired
    /// or
    /// Email is required. - Customer.EmailRequired
    /// </exception>
    public Customer(Guid id, string name, string phoneNumber, string email)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;

        if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Name is required.", "Customer.NameRequired");
        if (string.IsNullOrWhiteSpace(phoneNumber)) throw new ValidationException("Phone is required.", "Customer.PhoneRequired");
        if (string.IsNullOrWhiteSpace(email)) throw new ValidationException("Email is required.", "Customer.EmailRequired");

        Name = name.Trim();
        PhoneNumber = phoneNumber.Trim();
        Email = email.Trim();
    }

    /// <summary>
    /// Factory method to create a new Customer with validation.
    /// </summary>
    public static Customer Create(string name, string phoneNumber, string email)
        => new(Guid.NewGuid(), name, phoneNumber, email);
}
