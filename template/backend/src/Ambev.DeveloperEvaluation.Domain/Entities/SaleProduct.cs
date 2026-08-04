using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a product within a sale, linking a product to a sale with quantity and price.
/// </summary>
public class SaleProduct : BaseEntity
{
    /// <summary>
    /// Foreign key to the sale.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Foreign key to the product.
    /// </summary>
    public Guid ProductId { get; set; }

    private int _quantity;
    /// <summary>
    /// Quantity of the product sold.
    /// </summary>
    public int Quantity
    {
        get => _quantity;
        set
        { 
            _quantity = value;
            CalculateTotalAmount();
        } 
    }

    private decimal _unitPrice;
    /// <summary>
    /// Unit price for the product at the time of sale.
    /// </summary>
    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            _unitPrice = value;
            CalculateTotalAmount();
        }
    }

    private decimal _discounts;
    /// <summary>
    /// Discounts applied to the product, if any.
    /// </summary>
    public decimal Discounts
    {
        get => _discounts;
        set
        {
            _discounts = value;
            CalculateTotalAmount();
        }
    }

    /// <summary>
    /// Total amount for this product line after applying discounts.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Gets or sets the date and time when the record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property to the parent sale.
    /// </summary>
    public Sale? Sale { get; set; }

    /// <summary>
    /// Initializes a new instance of the SalesProducts class and sets the creation date.
    /// </summary>
    public SaleProduct()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private void CalculateTotalAmount()
    {
        TotalAmount = (UnitPrice * Quantity) - Discounts;
    }
}
