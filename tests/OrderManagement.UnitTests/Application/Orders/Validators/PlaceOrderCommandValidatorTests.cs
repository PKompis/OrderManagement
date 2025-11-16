using FluentValidation.TestHelper;
using OrderManagement.Application.Orders.Commands.AssignOrder;
using OrderManagement.Application.Orders.Commands.PlaceOrder;
using OrderManagement.Application.Orders.Models;
using OrderManagement.Domain.Orders.Enums;

namespace OrderManagement.UnitTests.Application.Orders.Validators;

/// <summary>
/// Place Order Command Validator Tests
/// </summary>
public class PlaceOrderCommandValidatorTests
{
    private readonly PlaceOrderCommandValidator validator = new();

    [Fact]
    public void ValidCasePickup()
    {
        var cmd = new PlaceOrderCommand
        {
            Type = OrderType.Pickup,
            DeliveryAddress = null,
            Items = [
                new PlaceOrderItemModel {
                    MenuItemId = Guid.NewGuid(),
                    Notes = "",
                    Quantity = 1
                }
            ]
        };

        var result = validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidCaseDelivery()
    {
        var cmd = new PlaceOrderCommand
        {
            Type = OrderType.Pickup,
            DeliveryAddress = new DeliveryAddressModel
            {
                City = "Test",
                Zip = "Test",
                Street = "Test"
            },
            Items = [
                new PlaceOrderItemModel {
                    MenuItemId = Guid.NewGuid(),
                    Notes = "",
                    Quantity = 1
                }
            ]
        };

        var result = validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NotValidItems()
    {
        var cmd = new PlaceOrderCommand
        {
            Type = OrderType.Pickup,
            DeliveryAddress = null,
            Items = []
        };

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Fact]
    public void NotValidType()
    {
        var cmd = new PlaceOrderCommand
        {
            Type = OrderType.Delivery,
            DeliveryAddress = null,
            Items = [
                new PlaceOrderItemModel {
                    MenuItemId = Guid.NewGuid(),
                    Notes = "",
                    Quantity = 1
                }
            ]
        };

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.DeliveryAddress);
    }

    [Fact]
    public void NotValidStreet()
    {
        var cmd = new PlaceOrderCommand
        {
            Type = OrderType.Delivery,
            DeliveryAddress = new DeliveryAddressModel
            {
                 City = "Test",
                 Zip = "Test"
            },
            Items = [
                new PlaceOrderItemModel {
                    MenuItemId = Guid.NewGuid(),
                    Notes = "",
                    Quantity = 1
                }
            ]
        };

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.DeliveryAddress!.Street);
    }

    [Fact]
    public void NotValidCity()
    {
        var cmd = new PlaceOrderCommand
        {
            Type = OrderType.Delivery,
            DeliveryAddress = new DeliveryAddressModel
            {
                Street = "Test",
                Zip = "Test"
            },
            Items = [
                new PlaceOrderItemModel {
                    MenuItemId = Guid.NewGuid(),
                    Notes = "",
                    Quantity = 1
                }
            ]
        };

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.DeliveryAddress!.City);
    }

    [Fact]
    public void NotValidZip()
    {
        var cmd = new PlaceOrderCommand
        {
            Type = OrderType.Delivery,
            DeliveryAddress = new DeliveryAddressModel
            {
                Street = "Test",
                City = "Test"
            },
            Items = [
                new PlaceOrderItemModel {
                    MenuItemId = Guid.NewGuid(),
                    Notes = "",
                    Quantity = 1
                }
            ]
        };

        var result = validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.DeliveryAddress!.Zip);
    }
}
