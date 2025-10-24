using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Services.ArtificialIntelligence;
using NUnit.Framework;
using System.Net;

namespace Nop.Tests.Nop.Services.Tests.ArtificialIntelligence;

[TestFixture]
public class ArtificialIntelligenceHttpClientTests
{
    private ArtificialIntelligenceSettings _settings;
    private HttpClient _httpClient;
    private HttpMessageHandler _messageHandler;

    [SetUp]
    public void SetUp()
    {
        _settings = new ArtificialIntelligenceSettings
        {
            RequestTimeout = 30,
            ProviderType = ArtificialIntelligenceProviderType.ChatGpt,
            ChatGptApiKey = "test-chatgpt-key",
            GeminiApiKey = "test-gemini-key",
            DeepSeekApiKey = "test-deepseek-key"
        };
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
        (_messageHandler as IDisposable)?.Dispose();
    }

    [Test]
    public void ShouldConfigureHttpClientTimeoutFromSettings()
    {
        // Arrange
        _settings.RequestTimeout = 45;
        _messageHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);

        // Act
        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

        // Assert
        _httpClient.Timeout.Should().Be(TimeSpan.FromSeconds(45));
    }

    [Test]
    public void ShouldUseDefaultTimeoutWhenSettingIsNull()
    {
        // Arrange
        _settings.RequestTimeout = null;
        _messageHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);

        // Act
        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

        // Assert
        _httpClient.Timeout.Should().Be(TimeSpan.FromSeconds(ArtificialIntelligenceDefaults.RequestTimeout));
    }

    [Test]
    public void ShouldCreateGeminiHelperWhenProviderIsGemini()
    {
        // Arrange
        _settings.ProviderType = ArtificialIntelligenceProviderType.Gemini;
        _messageHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);

        // Act & Assert
        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);
        client.Should().NotBeNull();
    }

    [Test]
    public void ShouldCreateChatGptHelperWhenProviderIsChatGpt()
    {
        // Arrange
        _settings.ProviderType = ArtificialIntelligenceProviderType.ChatGpt;
        _messageHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);

        // Act & Assert
        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);
        client.Should().NotBeNull();
    }

    [Test]
    public void ShouldCreateDeepSeekHelperWhenProviderIsDeepSeek()
    {
        // Arrange
        _settings.ProviderType = ArtificialIntelligenceProviderType.DeepSeek;
        _messageHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);

        // Act & Assert
        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);
        client.Should().NotBeNull();
    }

    [Test]
    public void ShouldThrowArgumentOutOfRangeExceptionForInvalidProviderType()
    {
        // Arrange
        _settings.ProviderType = (ArtificialIntelligenceProviderType)999;
        _messageHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);

        // Act & Assert
        Action act = () => new ArtificialIntelligenceHttpClient(_settings, _httpClient);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task SendQueryAsyncShouldReturnResponseForGeminiProvider()
    {
    // Arrange
    _settings.ProviderType = ArtificialIntelligenceProviderType.Gemini;
    
    // Gemini uses "candidates" not "output"
    var mockResponse = @"{
        ""candidates"": [
            {
                ""content"": {
                    ""parts"": [
                        {
                            ""text"": ""gemini test response""
                        }
                    ]
                }
            }
        ]
    }";
    
    _messageHandler = new MockHttpMessageHandler(HttpStatusCode.OK, mockResponse);
    _httpClient = new HttpClient(_messageHandler);
    
    var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

    // Act
    var result = await client.SendQueryAsync("test query");

    // Assert
    result.Should().NotBeNullOrEmpty();
    result.Should().Be("gemini test response");
    }

    [Test]
    public async Task SendQueryAsyncShouldReturnResponseForChatGptProvider()
    {
        // Arrange
        _settings.ProviderType = ArtificialIntelligenceProviderType.ChatGpt;
        
        var mockResponse = @"{
            ""output"": [
                {
                    ""content"": [
                        {
                            ""text"": ""chatgpt test response""
                        }
                    ]
                }
            ]
        }";
        
        _messageHandler = new MockHttpMessageHandler(HttpStatusCode.OK, mockResponse);
        _httpClient = new HttpClient(_messageHandler);
        
        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

        // Act
        var result = await client.SendQueryAsync("test query");

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be("chatgpt test response");
    }

    [Test]
    public async Task SendQueryAsyncShouldReturnResponseForDeepSeekProvider()
    {
        // Arrange
        _settings.ProviderType = ArtificialIntelligenceProviderType.DeepSeek;
    
        // DeepSeek uses "choices" with "message.content" structure
        var mockResponse = @"{
            ""choices"": [
                {
                    ""message"": {
                    ""content"": ""deepseek test response""
                }
            }
        ]
    }";
    
    _messageHandler = new MockHttpMessageHandler(HttpStatusCode.OK, mockResponse);
    _httpClient = new HttpClient(_messageHandler);
    
    var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

    // Act
    var result = await client.SendQueryAsync("test query");

    // Assert
    result.Should().NotBeNullOrEmpty();
    result.Should().Be("deepseek test response");
}

    [Test]
    public async Task SendQueryAsyncShouldThrowNopExceptionWhenRequestFails()
    {
        // Arrange
        var errorMessage = "Bad Request";
        _messageHandler = new MockHttpMessageHandler(HttpStatusCode.BadRequest, errorMessage);
        _httpClient = new HttpClient(_messageHandler);
        
        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

        // Act
        Func<Task> act = async () => await client.SendQueryAsync("test query");

        // Assert
        await act.Should().ThrowAsync<NopException>();
    }

    [Test]
    public async Task SendQueryAsyncShouldThrowNopExceptionWhenUnauthorized()
    {
        // Arrange
        _messageHandler = new MockHttpMessageHandler(HttpStatusCode.Unauthorized, "Unauthorized");
        _httpClient = new HttpClient(_messageHandler);
        
        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

        // Act
        Func<Task> act = async () => await client.SendQueryAsync("test query");

        // Assert
        await act.Should().ThrowAsync<NopException>();
    }

    [Test]
    public async Task SendQueryAsyncShouldThrowNopExceptionWhenServerError()
    {
        // Arrange
        _messageHandler = new MockHttpMessageHandler(HttpStatusCode.InternalServerError, "Server Error");
        _httpClient = new HttpClient(_messageHandler);
        
        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

        // Act
        Func<Task> act = async () => await client.SendQueryAsync("test query");

        // Assert
        await act.Should().ThrowAsync<NopException>();
    }

    #region Mock HttpMessageHandler

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;

        public MockHttpMessageHandler(HttpStatusCode statusCode = HttpStatusCode.OK, string content = "{}")
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content)
            };

            return Task.FromResult(response);
        }
    }

    #endregion
}