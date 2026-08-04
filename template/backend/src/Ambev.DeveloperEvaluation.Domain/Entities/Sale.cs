using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sales order in the system.
/// Contains header information and navigation to the sold products.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique number identifying the sale.
    /// </summary>
    public long Number { get; set; }

    /// <summary>
    /// Gets or sets the name of the customer associated with the sale.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the sale occurred.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the total amount for the sale.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the branch where the sale was made.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the sale (e.g., Pending, Completed, Cancelled).
    /// </summary>
    public SalesStatus Status { get; set; } = SalesStatus.Pending;

    /// <summary>
    /// Gets or sets the date and time when the record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property for the collection of products associated with this sale.
    /// </summary>
    public ICollection<SaleProduct> Products { get; set; } = new List<SaleProduct>();

    /// <summary>
    /// Initializes a new instance of the Sales class and sets the creation date to UTC now.
    /// </summary>
    public Sale()
    {
        Date = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
