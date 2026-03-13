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

    [Test]
    public void CanGetTokensInfo()
    {
        var responseText = @"{""id"": ""resp_096f41e878c7a1d20068ffc232491c8190ad765601ae8291dc"",""object"": ""response"",""created_at"": 1761591858,""status"": ""completed"",""background"": false,""billing"": {""payer"": ""developer""},""error"": null,""incomplete_details"": null,""instructions"": null,""max_output_tokens"": null,""max_tool_calls"": null,""model"": ""gpt-4.1-2025-04-14"",""output"": [{""id"": ""msg_096f41e878c7a1d20068ffc233b55081908e03c291f500b66a"",""type"": ""message"",""status"": ""completed"",""content"": [{""type"": ""output_text"",""annotations"": [],""logprobs"": [],""text"": ""**Apple MacBook Pro**\nExperience unmatched performance with the Apple MacBook Pro, engineered for professionals demanding speed and reliability. With advanced faster flash storage, launching apps and transferring large files are virtually instantaneous, boosting overall productivity. The high-load CPU effortlessly handles intensive multitasking and resource-heavy applications, making it the ideal choice for software developers, designers, and content creators. Choose the MacBook Pro to elevate your workflow with superior speed, efficiency, and unwavering stability.""}],""role"": ""assistant""}],""parallel_tool_calls"": true,""previous_response_id"": null,""prompt_cache_key"": null,""reasoning"": {""effort"": null,""summary"": null},""safety_identifier"": null,""service_tier"": ""default"",""store"": true,""temperature"": 1.0,""text"": {""format"": {""type"": ""text""},""verbosity"": ""medium""},""tool_choice"": ""auto"",""tools"": [],""top_logprobs"": 0,""top_p"": 1.0,""truncation"": ""disabled"",""usage"": {""input_tokens"": 50,""input_tokens_details"": {""cached_tokens"": 0},""output_tokens"": 94,""output_tokens_details"": {""reasoning_tokens"": 0},""total_tokens"": 144},""user"": null,""metadata"": {}}";

        //arrange & act
        var tokensInfo = _helper.GetTokensInfo(responseText);

        //assert
        tokensInfo.Should().NotBeNull();
        tokensInfo.Should().Be($"Input tokens: 50{Environment.NewLine}Output tokens: 94{Environment.NewLine}Total tokens: 144");
    }
}