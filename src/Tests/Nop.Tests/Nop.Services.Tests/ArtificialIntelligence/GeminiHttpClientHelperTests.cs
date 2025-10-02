using FluentAssertions;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Services.ArtificialIntelligence;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ArtificialIntelligence;

[TestFixture]
public class GeminiHttpClientHelperTests
{
    private GeminiHttpClientHelper _helper;
    private ArtificialIntelligenceSettings _settings;

    [SetUp]
    public void SetUp()
    {
        _helper = new GeminiHttpClientHelper();
        _settings = new ArtificialIntelligenceSettings
        {
            GeminiApiKey = "test-gemini-api-key"
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
        httpClient.BaseAddress.Should().Be(new Uri(ArtificialIntelligenceDefaults.GeminiBaseApiUrl));
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
    public void CreateRequestShouldAddApiKeyToHeaders()
    {
        // Arrange & Act
        var request = _helper.CreateRequest(_settings, "test query");

        // Assert
        request.Headers.Should().Contain(h => h.Key == ArtificialIntelligenceDefaults.GeminiApiKeyHeader);
        var apiKeyHeader = request.Headers.GetValues(ArtificialIntelligenceDefaults.GeminiApiKeyHeader).FirstOrDefault();
        apiKeyHeader.Should().Be("test-gemini-api-key");
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
        content.Should().Contain("contents");
        content.Should().Contain("parts");
        content.Should().Contain("text");
        content.Should().Contain(query);
    }

    [Test]
    public void ParseResponseShouldReturnTextFromValidResponse()
    {
        // Arrange
        var responseText = @"{
            ""candidates"": [
                {
                    ""content"": {
                        ""parts"": [
                            {
                                ""text"": ""This is the AI response""
                            }
                        ]
                    }
                }
            ]
        }";

        // Act
        var result = _helper.ParseResponse(responseText);

        // Assert
        result.Should().Be("This is the AI response");
    }

    [Test]
    public void ParseResponseShouldReturnFirstCandidateText()
    {
        // Arrange
        var responseText = @"{
            ""candidates"": [
                {
                    ""content"": {
                        ""parts"": [
                            {
                                ""text"": ""First response""
                            }
                        ]
                    }
                },
                {
                    ""content"": {
                        ""parts"": [
                            {
                                ""text"": ""Second response""
                            }
                        ]
                    }
                }
            ]
        }";

        // Act
        var result = _helper.ParseResponse(responseText);

        // Assert
        result.Should().Be("First response");
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenCandidatesIsEmpty()
    {
        // Arrange
        var responseText = @"{
            ""candidates"": []
        }";

        // Act
        var result = _helper.ParseResponse(responseText);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenPartsIsNull()
    {
        // Arrange
        var responseText = @"{
            ""candidates"": [
                {
                    ""content"": {
                        ""parts"": null
                    }
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
            ""candidates"": [
                {
                    ""content"": {
                        ""parts"": [
                            {
                                ""text"": """"
                            }
                        ]
                    }
                }
            ]
        }";

        // Act
        var result = _helper.ParseResponse(responseText);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void ParseResponseShouldHandleMultiplePartsAndReturnFirst()
    {
        // Arrange
        var responseText = @"{
            ""candidates"": [
                {
                    ""content"": {
                        ""parts"": [
                            {
                                ""text"": ""First part""
                            },
                            {
                                ""text"": ""Second part""
                            }
                        ]
                    }
                }
            ]
        }";

        // Act
        var result = _helper.ParseResponse(responseText);

        // Assert
        result.Should().Be("First part");
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