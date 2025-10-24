using FluentAssertions;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Services.ArtificialIntelligence;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ArtificialIntelligence;

[TestFixture]
public class ChatGptHttpClientHelperTests
{
    private ChatGptHttpClientHelper _helper;
    private ArtificialIntelligenceSettings _settings;

    [SetUp]
    public void SetUp()
    {
        _helper = new ChatGptHttpClientHelper();
        _settings = new ArtificialIntelligenceSettings
        {
            ChatGptApiKey = "test-chatgpt-api-key"
        };
    }

    [Test]
    public void ConfigureClientShouldSetBaseAddress()
    {
        // Arrange
        var httpClient = new HttpClient();

        // Act
        _helper.ConfigureClient(httpClient);

        // Assert
        httpClient.BaseAddress.Should().NotBeNull();
        httpClient.BaseAddress.Should().Be(new Uri(ArtificialIntelligenceDefaults.ChatGptBaseApiUrl));
    }

    [Test]
    public void CreateRequestShouldReturnHttpRequestMessageWithPostMethod()
    {
        // Arrange & Act
        var request = _helper.CreateRequest(_settings, "test query");

        // Assert
        request.Should().NotBeNull();
        request.Method.Should().Be(HttpMethod.Post);
    }

    [Test]
    public void CreateRequestShouldAddBearerTokenToAuthorizationHeader()
    {
        // Arrange & Act
        var request = _helper.CreateRequest(_settings, "test query");

        // Assert
        request.Headers.Should().Contain(h => h.Key == HeaderNames.Authorization);
        var authHeader = request.Headers.GetValues(HeaderNames.Authorization).FirstOrDefault();
        authHeader.Should().Be("Bearer test-chatgpt-api-key");
    }

    [Test]
    public void CreateRequestShouldSetCorrectContentType()
    {
        // Arrange & Act
        var request = _helper.CreateRequest(_settings, "test query");

        // Assert
        request.Content.Should().NotBeNull();
        request.Content.Headers.ContentType.Should().NotBeNull();
        request.Content.Headers.ContentType.MediaType.Should().Be(MimeTypes.ApplicationJson);
    }

    [Test]
    public async Task CreateRequestShouldSerializeQueryInCorrectFormat()
    {
        // Arrange
        var query = "What is AI?";

        // Act
        var request = _helper.CreateRequest(_settings, query);
        var content = await request.Content.ReadAsStringAsync();

        // Assert
        content.Should().NotBeNullOrEmpty();
        content.Should().Contain("model");
        content.Should().Contain("input");
        content.Should().Contain(ArtificialIntelligenceDefaults.ChatGptApiModel);
        content.Should().Contain(query);
    }

    [Test]
    public async Task CreateRequestShouldIncludeModelInRequestBody()
    {
        // Arrange & Act
        var request = _helper.CreateRequest(_settings, "test query");
        var content = await request.Content.ReadAsStringAsync();

        // Assert
        content.Should().Contain(ArtificialIntelligenceDefaults.ChatGptApiModel);
    }

    [Test]
    public void ParseResponseShouldReturnTextFromValidResponse()
    {
        // Arrange
        var responseText = @"{
            ""output"": [
                {
                    ""content"": [
                        {
                            ""text"": ""This is the ChatGPT response""
                        }
                    ]
                }
            ]
        }";

        // Act
        var result = _helper.ParseResponse(responseText);

        // Assert
        result.Should().Be("This is the ChatGPT response");
    }

    [Test]
    public void ParseResponseShouldReturnFirstOutputText()
    {
        // Arrange
        var responseText = @"{
            ""output"": [
                {
                    ""content"": [
                        {
                            ""text"": ""First response""
                        }
                    ]
                },
                {
                    ""content"": [
                        {
                            ""text"": ""Second response""
                        }
                    ]
                }
            ]
        }";

        // Act
        var result = _helper.ParseResponse(responseText);

        // Assert
        result.Should().Be("First response");
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenOutputIsEmpty()
    {
        // Arrange
        var responseText = @"{
            ""output"": []
        }";

        // Act
        var result = _helper.ParseResponse(responseText);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenContentIsNull()
    {
        // Arrange
        var responseText = @"{
            ""output"": [
                {
                    ""content"": null
                }
            ]
        }";

        // Act
        var result = _helper.ParseResponse(responseText);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenTextIsEmpty()
    {
        // Arrange
        var responseText = @"{
            ""output"": [
                {
                    ""content"": [
                        {
                            ""text"": """"
                        }
                    ]
                }
            ]
        }";

        // Act
        var result = _helper.ParseResponse(responseText);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void ParseResponseShouldHandleMultipleContentItemsAndReturnFirst()
    {
        // Arrange
        var responseText = @"{
            ""output"": [
                {
                    ""content"": [
                        {
                            ""text"": ""First content""
                        },
                        {
                            ""text"": ""Second content""
                        }
                    ]
                }
            ]
        }";

        // Act
        var result = _helper.ParseResponse(responseText);

        // Assert
        result.Should().Be("First content");
    }

    [Test]
    public void CreateRequestShouldHandleEmptyQuery()
    {
        // Arrange & Act
        var request = _helper.CreateRequest(_settings, string.Empty);

        // Assert
        request.Should().NotBeNull();
        request.Content.Should().NotBeNull();
    }

    [Test]
    public void CreateRequestShouldHandleSpecialCharactersInQuery()
    {
        // Arrange
        var query = "What is \"AI\"? Can you explain <concepts> & ideas?";

        // Act
        var request = _helper.CreateRequest(_settings, query);

        // Assert
        request.Should().NotBeNull();
        request.Content.Should().NotBeNull();
    }

    [Test]
    public async Task CreateRequestShouldUseUtf8Encoding()
    {
        // Arrange
        var query = "Test with émojis 🤖 and spëcial çharacters";

        // Act
        var request = _helper.CreateRequest(_settings, query);
        var content = await request.Content.ReadAsStringAsync();

        // Assert
        request.Content.Headers.ContentType.CharSet.Should().Be("utf-8");
        content.Should().Contain(query);
    }
}