namespace OrderManagement.Application.Common.Security;

/// <summary>
/// Application-level role names used for authorization.
/// </summary>
public static class ApplicationRoles
{
    /// <summary>
    /// The customer
    /// </summary>
    public const string Customer = "Customer";

    /// <summary>
    /// The Kitchen  
    /// </summary>
    public const string Kitchen = "Kitchen";

    /// <summary>
    /// The courier
    /// </summary>
    public const string Delivery  = "Courier";

    /// <summary>
    /// The admin
    /// </summary>
    public const string Admin = "Admin";
}
