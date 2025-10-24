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
        _settings = NopTestConfiguration.GetArtificialIntelligenceSettings();
    }

    [Test]
    public void ConfigureClientShouldSetBaseAddress()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var httpClient = new HttpClient();
        _helper.ConfigureClient(httpClient);

        //assert
        httpClient.BaseAddress.Should().NotBeNull();
        httpClient.BaseAddress.Should().Be(new Uri(ArtificialIntelligenceDefaults.ChatGptBaseApiUrl));
    }

    [Test]
    public void CreateRequestShouldReturnHttpRequestMessageWithPostMethod()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var request = _helper.CreateRequest(_settings, "test query");

        //assert
        request.Should().NotBeNull();
        request.Method.Should().Be(HttpMethod.Post);
    }

    [Test]
    public void CreateRequestShouldAddBearerTokenToAuthorizationHeader()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var request = _helper.CreateRequest(_settings, "test query");

        //assert
        request.Headers.Should().Contain(h => h.Key == HeaderNames.Authorization);
        var authHeader = request.Headers.GetValues(HeaderNames.Authorization).FirstOrDefault();
        authHeader.Should().Be("Bearer test-chatgpt-api-key");
    }

    [Test]
    public void CreateRequestShouldSetCorrectContentType()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var request = _helper.CreateRequest(_settings, "test query");

        //assert
        request.Content.Should().NotBeNull();
        request.Content.Headers.ContentType.Should().NotBeNull();
        request.Content.Headers.ContentType.MediaType.Should().Be(MimeTypes.ApplicationJson);
    }

    [Test]
    public async Task CreateRequestShouldSerializeQueryInCorrectFormat()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var query = "What is AI?";
        var request = _helper.CreateRequest(_settings, query);
        var content = await request.Content.ReadAsStringAsync();

        //assert
        content.Should().NotBeNullOrEmpty();
        content.Should().Contain("model");
        content.Should().Contain("input");
        content.Should().Contain(ArtificialIntelligenceDefaults.ChatGptApiModel);
        content.Should().Contain(query);
    }

    [Test]
    public async Task CreateRequestShouldIncludeModelInRequestBody()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var request = _helper.CreateRequest(_settings, "test query");
        var content = await request.Content.ReadAsStringAsync();

        //assert
        content.Should().Contain(ArtificialIntelligenceDefaults.ChatGptApiModel);
    }

    [Test]
    public void ParseResponseShouldReturnTextFromValidResponse()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
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
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().Be("This is the ChatGPT response");
    }

    [Test]
    public void ParseResponseShouldReturnFirstOutputText()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
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
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().Be("First response");
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenOutputIsEmpty()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var responseText = @"{
            ""output"": []
        }";
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().BeEmpty();
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenContentIsNull()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var responseText = @"{
            ""output"": [
                {
                    ""content"": null
                }
            ]
        }";
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().BeEmpty();
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenTextIsEmpty()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
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
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().BeEmpty();
    }

    [Test]
    public void ParseResponseShouldHandleMultipleContentItemsAndReturnFirst()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
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
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().Be("First content");
    }

    [Test]
    public void CreateRequestShouldHandleEmptyQuery()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var request = _helper.CreateRequest(_settings, string.Empty);

        //assert
        request.Should().NotBeNull();
        request.Content.Should().NotBeNull();
    }

    [Test]
    public void CreateRequestShouldHandleSpecialCharactersInQuery()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var query = "What is \"AI\"? Can you explain <concepts> & ideas?";
        var request = _helper.CreateRequest(_settings, query);

        //assert
        request.Should().NotBeNull();
        request.Content.Should().NotBeNull();
    }

    [Test]
    public async Task CreateRequestShouldUseUtf8Encoding()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var query = "Test with émojis 🤖 and spëcial çharacters";
        var request = _helper.CreateRequest(_settings, query);
        var content = await request.Content.ReadAsStringAsync();

        //assert
        request.Content.Headers.ContentType.CharSet.Should().Be("utf-8");
        content.Should().Contain(query);
    }
}