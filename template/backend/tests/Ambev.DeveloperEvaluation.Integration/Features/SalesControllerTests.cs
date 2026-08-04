using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Integration.Features.TestData;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
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

    [Fact]
    public async Task SalesControllerGet_WhenSaleExists_ShouldReturnSale()
    {
        // Arrange - create sale first
        var content = CreateSaleData.ValidCreateSaleRequest(2);
        var createResponse = await _httpClient.PostAsJsonAsync("api/sales", content);
        var createSaleResponse = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var id = createSaleResponse!.Data!.Id;

        // Act
        var response = await _httpClient.GetAsync($"api/sales/{id}");
        var saleResponse = await response.Content.ReadFromJsonAsync<ApiResponseWithData<GetSaleResponse>>();

        // Assert
        Assert.True(saleResponse?.Success);
        Assert.NotNull(saleResponse?.Data);
        Assert.Equal(id, saleResponse!.Data!.Id);
    }

    [Fact]
    public async Task SalesControllerUpdate_WhenValidDataIsProvided_ShouldUpdateSale()
    {
        // Arrange - create sale first
        var content = CreateSaleData.ValidCreateSaleRequest(3);
        var createResponse = await _httpClient.PostAsJsonAsync("api/sales", content);
        var createSaleResponse = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var id = createSaleResponse!.Data!.Id;

        var updateRequest = new UpdateSaleRequest
        {
            Id = id,
            Number = content.Number,
            CustomerName = "Updated Customer",
            Date = content.Date,
            Branch = "Updated Branch",
            Status = content.Status,
            Products = [.. content.Products.Select(p => new UpdateSaleProductRequest
            {
                Id = Guid.Empty,
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                UnitPrice = p.UnitPrice
            })]
        };

        // Act
        var response = await _httpClient.PutAsJsonAsync($"api/sales/{id}", updateRequest);
        var updateResponse = await response.Content.ReadFromJsonAsync<ApiResponseWithData<UpdateSaleResponse>>();

        // Assert
        Assert.True(updateResponse?.Success);
        Assert.NotNull(updateResponse?.Data);
        Assert.Equal("Updated Customer", updateResponse!.Data!.CustomerName);

        // Verify in database
        var sale = await GetSaleByIdAsync(id);
        Assert.NotNull(sale);
        Assert.Equal("Updated Customer", sale!.CustomerName);
        Assert.Equal("Updated Branch", sale.Branch);
    }

    [Fact]
    public async Task SalesControllerDelete_WhenSaleExists_ShouldDeleteSale()
    {
        // Arrange - create sale first
        var content = CreateSaleData.ValidCreateSaleRequest(4);
        var createResponse = await _httpClient.PostAsJsonAsync("api/sales", content);
        var createSaleResponse = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var id = createSaleResponse!.Data!.Id;

        // Act
        var response = await _httpClient.DeleteAsync($"api/sales/{id}");
        var deleteResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();

        // Assert
        Assert.True(deleteResponse?.Success ?? false);

        var sale = await GetSaleByIdAsync(id);
        Assert.Null(sale);
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
