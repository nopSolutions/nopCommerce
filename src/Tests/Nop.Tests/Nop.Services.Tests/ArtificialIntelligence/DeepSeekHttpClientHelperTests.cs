using FluentAssertions;
using Microsoft.Net.Http.Headers;
using Nop.Core;
using Nop.Core.Domain.ArtificialIntelligence;
using Nop.Services.ArtificialIntelligence;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.ArtificialIntelligence;

[TestFixture]
public class DeepSeekHttpClientHelperTests
{
    private DeepSeekHttpClientHelper _helper;
    private ArtificialIntelligenceSettings _settings;

    [SetUp]
    public void SetUp()
    {
        _helper = new DeepSeekHttpClientHelper();
        _settings = NopTestConfiguration.GetArtificialIntelligenceSettings(ArtificialIntelligenceProviderType.DeepSeek);
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
        httpClient.BaseAddress.Should().Be(new Uri(ArtificialIntelligenceDefaults.DeepSeekBaseApiUrl));
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
        authHeader.Should().Be("Bearer test-deepseek-api-key");
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
        content.Should().Contain("messages");
        content.Should().Contain("model");
        content.Should().Contain("content");
        content.Should().Contain("role");
        content.Should().Contain(query);
    }

    [Test]
    public async Task CreateRequestShouldIncludeSystemRoleInMessages()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var request = _helper.CreateRequest(_settings, "test query");
        var content = await request.Content.ReadAsStringAsync();

        //assert
        content.Should().Contain("\"role\":\"system\"");
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
        content.Should().Contain(ArtificialIntelligenceDefaults.DeepSeekApiModel);
    }

    [Test]
    public void ParseResponseShouldReturnTextFromValidResponse()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var responseText = @"{
            ""choices"": [
                {
                    ""message"": {
                        ""content"": ""This is the DeepSeek response""
                    }
                }
            ]
        }";
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().Be("This is the DeepSeek response");
    }

    [Test]
    public void ParseResponseShouldReturnFirstChoiceContent()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var responseText = @"{
            ""choices"": [
                {
                    ""message"": {
                        ""content"": ""First response""
                    }
                },
                {
                    ""message"": {
                        ""content"": ""Second response""
                    }
                }
            ]
        }";
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().Be("First response");
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenChoicesIsEmpty()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var responseText = @"{
            ""choices"": []
        }";
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().BeEmpty();
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenMessageIsNull()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var responseText = @"{
            ""choices"": [
                {
                    ""message"": null
                }
            ]
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
            ""choices"": [
                {
                    ""message"": {
                        ""content"": null
                    }
                }
            ]
        }";
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().BeEmpty();
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenContentIsEmpty()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var responseText = @"{
            ""choices"": [
                {
                    ""message"": {
                        ""content"": """"
                    }
                }
            ]
        }";
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().BeEmpty();
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
    public async Task CreateRequestShouldCreateMessageArrayWithSingleItem()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var request = _helper.CreateRequest(_settings, "test query");
        var content = await request.Content.ReadAsStringAsync();

        //assert
        content.Should().Contain("\"messages\":[");
    }

    [Test]
    public void CanGetTokensInfo()
    {
        var responseText = @"{""id"":""0ceecc18-1b18-4cb8-8697-6f48dc1e7bd0"",""object"":""chat.completion"",""created"":1761591960,""model"":""deepseek-chat"",""choices"":[{""index"":0,""message"":{""role"":""assistant"",""content"":""Of course. Here is a product description for the Apple MacBook Pro, crafted with an expert tone and incorporating the specified keywords.\n\n***\n\n### Engineered for the Uncompromising Professional\n\nThe Apple MacBook Pro is not merely an upgrade; it is a recalibration of the professional workflow. It is engineered for those for whom time is the ultimate currency and performance is non-negotiable. This machine dismantles bottlenecks as a core function, delivering a level of responsiveness that redefines productivity.\n\n**Faster Flash: The End of the Wait State**\n\nAt the heart of its blistering performance lies an advanced, **faster flash** storage architecture. We are discussing sequential read speeds that eclipse most desktop SSDs, effectively eliminating load times for multi-gigabyte projects, vast libraries, and complex datasets. This is not just about quick boot-ups; it's about instantaneous application launches, seamless 8K video scrubbing, and near-instantaneous project file access. When your storage subsystem operates at this velocity, your creative and analytical processes flow without interruption.\n\n**High-Load CPU: Sustained Performance Under Duress**\n\nWhere conventional laptops falter, the MacBook Pro excels. Its **high-load CPU** is engineered for sustained performance, not just short bursts. Whether you are compiling millions of lines of code, rendering intricate 3D models, running multiple virtual machines, or processing terabytes of scientific data, the thermal architecture ensures the processor maintains peak clock speeds under maximum load. This is computational integrity you can rely on—the assurance that when the deadline looms and the project complexity peaks, your machine will not throttle. It is the definitive tool for rendering, simulation, and development cycles that demand unwavering computational power.\n\n**The Synergy of Power**\n\nThe true mastery of the MacBook Pro is found in the seamless synergy between its **faster flash** and **high-load CPU**. The processor is fed data at an unprecedented rate, ensuring it is never starved for information, while its robust thermal management allows it to process that data without compromise. This creates a feedback loop of pure efficiency, enabling you to execute complex, multi-layered tasks with a fluidity that was previously the domain of high-end workstations.\n\nThis is the platform for those who push boundaries. Choose the tool that matches your ambition.""},""logprobs"":null,""finish_reason"":""stop""}],""usage"":{""prompt_tokens"":46,""completion_tokens"":466,""total_tokens"":512,""prompt_tokens_details"":{""cached_tokens"":0},""prompt_cache_hit_tokens"":0,""prompt_cache_miss_tokens"":46},""system_fingerprint"":""fp_ffc7281d48_prod0820_fp8_kvcache""}";
        
        //arrange & act
        var tokensInfo = _helper.GetTokensInfo(responseText);

        //assert
        tokensInfo.Should().NotBeNull();
        tokensInfo.Should().Be($"Prompt tokens: 46{Environment.NewLine}Completion tokens: 466{Environment.NewLine}Total tokens: 512");
    }
}