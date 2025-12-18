using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Orders;

namespace Nop.Plugin.Theme.KungFu.Services;

/// <summary>
/// AI Sage service for generating philosophical messages about orders using Azure OpenAI
/// </summary>
public class AISageService : IAISageService
{
    private readonly ILogger<AISageService> _logger;
    private readonly ISettingService _settingService;
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly IHttpClientFactory _httpClientFactory;

    public AISageService(
        ILogger<AISageService> logger,
        ISettingService settingService,
        IOrderService orderService,
        IProductService productService,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _settingService = settingService;
        _orderService = orderService;
        _productService = productService;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GenerateSageMessageAsync(Order order)
    {
        var settings = await _settingService.LoadSettingAsync<ThemeKungFuSettings>();

        if (!await IsConfiguredAsync())
        {
            _logger.LogWarning("AI Sage service is not configured. Cannot generate message.");
            return GetFallbackMessage();
        }

        try
        {
            // Get order items with product details
            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
            var productDetails = new StringBuilder();
            var totalItems = 0;

            foreach (var item in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                if (product != null)
                {
                    productDetails.AppendLine($"- {product.Name} (Quantity: {item.Quantity})");
                    totalItems += item.Quantity;
                }
            }

            // Create the prompt for the AI
            var prompt = $@"You are an ancient Chinese sage, wise and contemplative like Lao Tzu or Confucius. 
A customer has just completed payment for an order from a martial arts and meditation store called Kung Fu Store. 

Their order includes:
{productDetails}

Total items: {totalItems}
Order total: {order.OrderTotal:C}

Write a short, meaningful message (3-4 sentences) that:
1. Acknowledges their order with gentle wisdom
2. Relates their purchase to concepts of discipline, balance, or the path to mastery
3. Offers a brief meditation or philosophical insight inspired by their order
4. Ends with encouragement for their journey

Keep the tone peaceful, wise, and encouraging. Do not mention prices or transaction details directly. 
Focus on the philosophical and spiritual aspects of their choices.";

            var message = await CallAzureOpenAIAsync(settings, prompt);
            return string.IsNullOrWhiteSpace(message) ? GetFallbackMessage() : message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating AI sage message for order {OrderId}", order.Id);
            return GetFallbackMessage();
        }
    }

    public async Task<bool> IsConfiguredAsync()
    {
        var settings = await _settingService.LoadSettingAsync<ThemeKungFuSettings>();
        
        return !string.IsNullOrWhiteSpace(settings.AzureOpenAIEndpoint) &&
               !string.IsNullOrWhiteSpace(settings.AzureOpenAIKey) &&
               !string.IsNullOrWhiteSpace(settings.AzureOpenAIDeploymentName) &&
               settings.EnableAISageMessages;
    }

    private async Task<string> CallAzureOpenAIAsync(ThemeKungFuSettings settings, string prompt)
    {
        var client = _httpClientFactory.CreateClient();
        var apiVersion = "2024-02-15-preview";
        var url = $"{settings.AzureOpenAIEndpoint.TrimEnd('/')}/openai/deployments/{settings.AzureOpenAIDeploymentName}/chat/completions?api-version={apiVersion}";

        var requestBody = new
        {
            messages = new[]
            {
                new { role = "system", content = "You are a wise Chinese sage, speaking with the wisdom of Lao Tzu and Confucius." },
                new { role = "user", content = prompt }
            },
            max_tokens = 300,
            temperature = 0.7
        };

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("api-key", settings.AzureOpenAIKey);
        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonDocument.Parse(responseContent);
        
        if (jsonResponse.RootElement.TryGetProperty("choices", out var choices) && 
            choices.GetArrayLength() > 0)
        {
            var firstChoice = choices[0];
            if (firstChoice.TryGetProperty("message", out var message) &&
                message.TryGetProperty("content", out var content))
            {
                return content.GetString();
            }
        }

        return null;
    }

    private static string GetFallbackMessage()
    {
        var messages = new[]
        {
            "Your order has been received with gratitude. As Lao Tzu teaches, 'A journey of a thousand miles begins with a single step.' May these tools serve you well on your path to mastery. Train with purpose, rest with peace.",
            
            "The ancient masters understood that true strength comes from within. Your commitment to this path honors the traditions of those who came before. May your practice bring you clarity and inner peace.",
            
            "Confucius said, 'It does not matter how slowly you go as long as you do not stop.' Your order reflects your dedication to continuous growth. May each session bring you closer to harmony and balance.",
            
            "In selecting these items, you have shown wisdom in understanding that mastery requires proper tools and sincere effort. As the ancients knew, the path unfolds one practice at a time. Be patient with yourself.",
            
            "Your journey toward excellence continues with this order. Remember the words of Lao Tzu: 'When I let go of what I am, I become what I might be.' May your training reveal your true potential."
        };

        var random = new Random();
        return messages[random.Next(messages.Length)];
    }
}
