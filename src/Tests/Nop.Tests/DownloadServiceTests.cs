using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Services.Media;
using NUnit.Framework;


using System.IO;
using System.Threading.Tasks;

namespace Nop.Tests;
[TestFixture]
public class DownloadServiceTests
{
    private Mock<IRepository<Download>> _downloadRepositoryMock;
    private IDownloadService _downloadService;

    [SetUp]
    public void Setup()
    {
        _downloadRepositoryMock = new Mock<IRepository<Download>>();
        _downloadService = new DownloadService(_downloadRepositoryMock.Object);
    }




    [Test]
    public async Task GetDownloadByGuidAsync_ShouldReturnDownload_WhenValidGuidProvided()
    {
        // Arrange
        var downloadGuid = Guid.NewGuid();
        var expectedDownload = new Download { DownloadGuid = downloadGuid };
        _downloadRepositoryMock.Setup(repo => repo.Table).Returns(new[] { expectedDownload }.AsQueryable());

        // Act
        var result = await _downloadService.GetDownloadByGuidAsync(downloadGuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(downloadGuid, result.DownloadGuid);
    }

    [Test]
    public async Task GetDownloadByGuidAsync_ShouldReturnNull_WhenEmptyGuidProvided()
    {
        // Arrange
        Guid emptyGuid = Guid.Empty;

        // Act
        var result = await _downloadService.GetDownloadByGuidAsync(emptyGuid);

        // Assert
        Assert.IsNull(result);
    }





    [Test]
    public async Task GetDownloadBitsAsync_ShouldReturnFileBytes_WhenValidFormFileProvided()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var fileBytes = new byte[] { 1, 2, 3 };
        fileMock.Setup(file => file.OpenReadStream()).Returns(new MemoryStream(fileBytes));

        // Act
        var result = await _downloadService.GetDownloadBitsAsync(fileMock.Object);

        // Assert
        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(fileBytes, result);
    }
}

