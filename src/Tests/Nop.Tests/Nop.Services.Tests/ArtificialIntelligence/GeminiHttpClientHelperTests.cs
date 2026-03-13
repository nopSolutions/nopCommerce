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
        _settings = NopTestConfiguration.GetArtificialIntelligenceSettings(ArtificialIntelligenceProviderType.Gemini);
    }

    [Test]
    public void ConfigureClientShouldSetBaseAddress()
    {
        if(!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var httpClient = new HttpClient();
        _helper.ConfigureClient(httpClient);

        //assert
        httpClient.BaseAddress.Should().NotBeNull();
        httpClient.BaseAddress.Should().Be(new Uri(ArtificialIntelligenceDefaults.GeminiBaseApiUrl));
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
    public void CreateRequestShouldAddApiKeyToHeaders()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var request = _helper.CreateRequest(_settings, "test query");

        //assert
        request.Headers.Should().Contain(h => h.Key == ArtificialIntelligenceDefaults.GeminiApiKeyHeader);
        var apiKeyHeader = request.Headers.GetValues(ArtificialIntelligenceDefaults.GeminiApiKeyHeader).FirstOrDefault();
        apiKeyHeader.Should().Be("test-gemini-api-key");
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
        content.Should().Contain("contents");
        content.Should().Contain("parts");
        content.Should().Contain("text");
        content.Should().Contain(query);
    }

    [Test]
    public void ParseResponseShouldReturnTextFromValidResponse()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
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
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().Be("This is the AI response");
    }

    [Test]
    public void ParseResponseShouldReturnFirstCandidateText()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
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
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().Be("First response");
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenCandidatesIsEmpty()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var responseText = @"{
            ""candidates"": []
        }";
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().BeEmpty();
    }

    [Test]
    public void ParseResponseShouldReturnEmptyStringWhenPartsIsNull()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
        var responseText = @"{
            ""candidates"": [
                {
                    ""content"": {
                        ""parts"": null
                    }
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
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().BeEmpty();
    }

    [Test]
    public void ParseResponseShouldHandleMultiplePartsAndReturnFirst()
    {
        if (!_settings.IsProviderConfigured())
            return;

        //arrange & act
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
        var result = _helper.ParseResponse(responseText);

        //assert
        result.Should().Be("First part");
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
        var responseText = @"{""candidates"": [{""content"": {""parts"": [{""text"": ""The Apple MacBook Pro is not merely a laptop; it is a meticulously engineered platform, meticulously designed to elevate the professional workflow of the most demanding users. It stands as a testament to uncompromising power and precision, purpose-built for those whose work mandates absolute performance.\n\nAt its core lies a **high-load CPU**, a cutting-edge processor architecture engineered for relentless computational tasks. This formidable processing unit doesn't just manage demanding applications; it dominates them. Whether compiling vast codebases, rendering complex 3D models, executing intricate data analyses, or orchestrating multi-track audio production, the MacBook Pro delivers sustained peak performance, ensuring your most critical tasks proceed without compromise. It is designed to operate under intensive, continuous loads, providing the processing headroom essential for professional-grade productivity and innovation.\n\nComplementing this computational prowess is the integrated, **faster flash storage**. This isn't merely quick; it redefines speed, delivering unparalleled I/O performance that dramatically diminishes latency across the entire system. Experience instantaneous boot times, near-zero application launch delays, and rapid file transfers – capabilities crucial for professionals handling gigabytes of high-resolution media, expansive virtual machine environments, or large-scale project files. The accelerated data throughput fundamentally transforms the responsiveness of every interaction, enabling fluid multitasking and reducing wait times to an absolute minimum.\n\nThe synergy between this potent **high-load CPU** and the blistering **faster flash** storage transforms the MacBook Pro into an indispensable tool, a meticulously optimized machine capable of tackling the most intensive and complex professional challenges with unwavering stability and speed. For the professional whose work demands the absolute best, the MacBook Pro represents the zenith of portable computing, delivering a robust and reliable foundation for peak performance.""}],""role"": ""model""},""finishReason"": ""STOP"",""index"": 0}],""usageMetadata"": {""promptTokenCount"": 43,""candidatesTokenCount"": 347,""totalTokenCount"": 1679,""promptTokensDetails"": [{""modality"": ""TEXT"",""tokenCount"": 43}],""thoughtsTokenCount"": 1289},""modelVersion"": ""gemini-2.5-flash"",""responseId"": ""fsH_aNehCaq_vdIPnqK3gQs""}";

        //arrange & act
        var tokensInfo = _helper.GetTokensInfo(responseText);

        //assert
        tokensInfo.Should().NotBeNull();
        tokensInfo.Should().Be($"Prompt tokens: 43{Environment.NewLine}Candidate tokens: 347{Environment.NewLine}Thought tokens: 1289{Environment.NewLine}Total tokens: 1679");
    }
}