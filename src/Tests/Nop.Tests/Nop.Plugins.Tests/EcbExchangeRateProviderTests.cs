using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Nop.Core;
using Nop.Plugin.ExchangeRate.EcbExchange;
using Nop.Services.Localization;
using Nop.Services.Logging;
using NUnit.Framework;

namespace Nop.Tests.Nop.Plugins.Tests
{
    public class EcbExchangeRateProviderTests : BaseNopTest
    {
        private Mock<IHttpClientFactory> _httpClientFactory = new Mock<IHttpClientFactory>();
        private Mock<ILocalizationService> _localizationService = new Mock<ILocalizationService>();
        private Mock<ILogger> _logger = new Mock<ILogger>();
        
        
        [Test]
        public async Task GetCurrencyLiveRatesAsync_ShouldReturn_Currencies()
        {
            //prepare
            var httpClientMock = GetMockedHttpClient(xmlData);
            _httpClientFactory.Setup(s => s.CreateClient(It.IsAny<string>())).Returns(httpClientMock);

            //action
            var provider = new EcbExchangeRateProvider(_httpClientFactory.Object, _localizationService.Object, _logger.Object);
            var result = await provider.GetCurrencyLiveRatesAsync("EUR");

            //verify
            result.Should().NotBeNull();
            result.Count.Should().Be(7);
            
            var usdRate = result.FirstOrDefault(f => f.CurrencyCode == "USD");
            usdRate.Should().NotBeNull();
            usdRate?.Rate.Should().Be(1.1814M);
        }

        [Test]
        public async Task GetCurrencyLiveRatesAsync_Should_ErrorLog()
        {
            //prepare
            var httpClientMock = GetMockedHttpClient("");
            _httpClientFactory.Setup(s => s.CreateClient(It.IsAny<string>())).Returns(httpClientMock);
            
            //action
            var provider = new EcbExchangeRateProvider(_httpClientFactory.Object, _localizationService.Object, _logger.Object);
            var result = await provider.GetCurrencyLiveRatesAsync("EUR");

            //verify
            result.Count.Should().Be(1);
            _logger.Verify(v => v.ErrorAsync(It.IsAny<string>(), It.IsAny<Exception>(), null), Times.Once);
        }

        [Test]
        public void GetCurrencyLiveRatesAsync_Should_Throw_If_RatesNotFound()
        {
            //prepare
            var httpClientMock = GetMockedHttpClient("");
            _httpClientFactory.Setup(s => s.CreateClient(It.IsAny<string>())).Returns(httpClientMock);
            
            //action
            var provider = new EcbExchangeRateProvider(_httpClientFactory.Object, _localizationService.Object, _logger.Object);
            //var result = await provider.GetCurrencyLiveRatesAsync("RUB");

            ////verify
            Assert.ThrowsAsync<NopException>(async () => await provider.GetCurrencyLiveRatesAsync("RUB"));
        }

        private HttpClient GetMockedHttpClient(string content)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content),
                })
                .Verifiable();

            return new HttpClient(handlerMock.Object);
        }

        private string xmlData = @"<?xml version='1.0' encoding='UTF-8'?>
        <gesmes:Envelope xmlns:gesmes='http://www.gesmes.org/xml/2002-08-01' xmlns='http://www.ecb.int/vocabulary/2002-08-01/eurofxref'>
            <Cube>
                <Cube time='2021-09-14'>
                    <Cube currency='USD' rate='1.1814'/>
                    <Cube currency='JPY' rate='130.08'/>
                    <Cube currency='BGN' rate='1.9558'/>
                    <Cube currency='CZK' rate='25.389'/>
                    <Cube currency='DKK' rate='7.4361'/>
                    <Cube currency='GBP' rate='0.85260'/>
            </Cube>
            </Cube>
        </gesmes:Envelope>";
    }
}