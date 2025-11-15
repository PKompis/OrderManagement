using OrderManagement.Domain.Common.Exceptions;
using OrderManagement.Domain.Staff.Enums;

namespace OrderManagement.Domain.Staff.Entities;

/// <summary>
/// Staff entity
/// </summary>
public sealed class Staff
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
    /// Gets the role.
    /// </summary>
    public StaffRole Role { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is active.
    /// </summary>
    public bool IsActive { get; private set; }

    private Staff() { }

    private Staff(Guid id, string name, StaffRole role, bool isActive)
    {
        if (id == Guid.Empty) throw new ValidationException("Id is required.", "Staff.id");
        if (string.IsNullOrWhiteSpace(name)) throw new ValidationException("Name is required.", "Staff.NameRequired");

        Id = id;
        Name = name.Trim();
        Role = role;
        IsActive = isActive;
    }

    /// <summary>
    /// Creates the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="role">The role.</param>
    /// <param name="isActive">if set to <c>true</c> [is active].</param>
    public static Staff Create(string name, StaffRole role, bool isActive = true)
        => new(Guid.NewGuid(), name, role, isActive);
}