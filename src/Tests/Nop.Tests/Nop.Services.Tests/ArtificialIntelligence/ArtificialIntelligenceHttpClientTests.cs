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
        _settings = NopTestConfiguration.GetArtificialIntelligenceSettings();
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
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        _settings.RequestTimeout = 45;
        _messageHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);
        _ = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

        //assert
        _httpClient.Timeout.Should().Be(TimeSpan.FromSeconds(45));
    }

    [Test]
    public void ShouldUseDefaultTimeoutWhenSettingIsNull()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        _settings.RequestTimeout = null;
        _messageHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);
        _ = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

        //assert
        _httpClient.Timeout.Should().Be(TimeSpan.FromSeconds(ArtificialIntelligenceDefaults.RequestTimeout));
    }

    [Test]
    public void ShouldCreateGeminiHelperWhenProviderIsGemini()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        _settings.ProviderType = ArtificialIntelligenceProviderType.Gemini;
        _messageHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);
        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

        //assert
        client.Should().NotBeNull();
    }

    [Test]
    public void ShouldCreateChatGptHelperWhenProviderIsChatGpt()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        _settings.ProviderType = ArtificialIntelligenceProviderType.ChatGpt;
        _messageHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);
        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

        //assert
        client.Should().NotBeNull();
    }

    [Test]
    public void ShouldCreateDeepSeekHelperWhenProviderIsDeepSeek()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        _settings.ProviderType = ArtificialIntelligenceProviderType.DeepSeek;
        _messageHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);
        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);

        //assert

        client.Should().NotBeNull();
    }

    [Test]
    public void ShouldThrowArgumentOutOfRangeExceptionForInvalidProviderType()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        _settings.ProviderType = (ArtificialIntelligenceProviderType)999;
        _messageHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_messageHandler);
        Action act = () => new ArtificialIntelligenceHttpClient(_settings, _httpClient);

        //assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task SendQueryAsyncShouldReturnResponseForGeminiProvider()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
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

        _messageHandler = new TestHttpMessageHandler(HttpStatusCode.OK, mockResponse);
        _httpClient = new HttpClient(_messageHandler);

        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);
        var result = await client.SendQueryAsync("test query");

        //assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be("gemini test response");
    }

    [Test]
    public async Task SendQueryAsyncShouldReturnResponseForChatGptProvider()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
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

        _messageHandler = new TestHttpMessageHandler(HttpStatusCode.OK, mockResponse);
        _httpClient = new HttpClient(_messageHandler);

        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);
        var result = await client.SendQueryAsync("test query");

        //assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be("chatgpt test response");
    }

    [Test]
    public async Task SendQueryAsyncShouldReturnResponseForDeepSeekProvider()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
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

        _messageHandler = new TestHttpMessageHandler(HttpStatusCode.OK, mockResponse);
        _httpClient = new HttpClient(_messageHandler);

        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);
        var result = await client.SendQueryAsync("test query");

        //assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be("deepseek test response");
    }

    [Test]
    public async Task SendQueryAsyncShouldThrowNopExceptionWhenRequestFails()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var errorMessage = "Bad Request";
        _messageHandler = new TestHttpMessageHandler(HttpStatusCode.BadRequest, errorMessage);
        _httpClient = new HttpClient(_messageHandler);

        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);
        Func<Task> act = async () => await client.SendQueryAsync("test query");

        //assert
        await act.Should().ThrowAsync<NopException>();
    }

    [Test]
    public async Task SendQueryAsyncShouldThrowNopExceptionWhenUnauthorized()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        _messageHandler = new TestHttpMessageHandler(HttpStatusCode.Unauthorized, "Unauthorized");
        _httpClient = new HttpClient(_messageHandler);

        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);
        Func<Task> act = async () => await client.SendQueryAsync("test query");

        //assert
        await act.Should().ThrowAsync<NopException>();
    }

    [Test]
    public async Task SendQueryAsyncShouldThrowNopExceptionWhenServerError()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        _messageHandler = new TestHttpMessageHandler(HttpStatusCode.InternalServerError, "Server Error");
        _httpClient = new HttpClient(_messageHandler);

        var client = new ArtificialIntelligenceHttpClient(_settings, _httpClient);
        Func<Task> act = async () => await client.SendQueryAsync("test query");

        //assert
        await act.Should().ThrowAsync<NopException>();
    }

    #region Nested class

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _content;

        public TestHttpMessageHandler(HttpStatusCode statusCode = HttpStatusCode.OK, string content = "{}")
        {
            _statusCode = statusCode;
            _content = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode) { Content = new StringContent(_content) };

            return Task.FromResult(response);
        }
    }

    #endregion
}