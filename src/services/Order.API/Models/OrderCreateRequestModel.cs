namespace Order.API.Models;

public record OrderCreateRequestModel(int BuyerId, List<OrderItemRequestModel> OrderItems);

public record OrderItemRequestModel(int ProductId,int Count,decimal Price);