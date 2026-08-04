namespace Ambev.DeveloperEvaluation.Application.Messaging;

public class SaleCreated
{
    public Guid Id { get; set; }
    public long Number { get; set; }
}

public class SaleModified
{
    public Guid Id { get; set; }
    public long Number { get; set; }
}

public class SaleCancelled
{
    public Guid Id { get; set; }
    public long Number { get; set; }
}
