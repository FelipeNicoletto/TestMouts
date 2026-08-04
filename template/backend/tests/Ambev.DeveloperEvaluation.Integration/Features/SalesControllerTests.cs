using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Integration.Features.TestData;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Features;

public class SalesControllerTests(ApiWebApplicationFactory webApplicationFactory) : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _httpClient = webApplicationFactory.CreateClient();

    [Fact]
    public async Task SalesControllerCreate_WhenValidDataIsProvided_ShouldCreateSale()
    {
        // Arrange
        var content = CreateSaleData.ValidCreateSaleRequest(1);

        // Act
        var response = await _httpClient.PostAsJsonAsync("api/sales", content);
        var saleResponse = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();

        // Assert
        Assert.True(saleResponse?.Success);

        var sale = await GetSaleByIdAsync(saleResponse!.Data!.Id);
        Assert.NotNull(sale);
        Assert.Equal(content.CustomerName, sale.CustomerName);
        Assert.Equal(content.Date.Date, sale.Date.Date);
        Assert.Equal(content.Branch, sale.Branch);
        Assert.Equal(content.Status, sale.Status);
        Assert.Equal(content.Products.Count, sale.Products.Count);

        for (int i = 0; i < content.Products.Count; i++)
        {
            var contentProduct = content.Products.ElementAt(i);
            var saleProduct = sale.Products.First(p => p.ProductId == contentProduct.ProductId);

            Assert.Equal(contentProduct.ProductId, saleProduct.ProductId);
            Assert.Equal(contentProduct.Quantity, saleProduct.Quantity);
            Assert.Equal(contentProduct.UnitPrice, saleProduct.UnitPrice);
            Assert.Equal(contentProduct.UnitPrice * contentProduct.Quantity, saleProduct.TotalAmount + saleProduct.Discounts);
        }
    }

    private async Task<Sale?> GetSaleByIdAsync(Guid id)
    {
        using var scope = webApplicationFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();

        return await context.Sales
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}
