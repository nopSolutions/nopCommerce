using FluentAssertions;
using Nop.Core.Domain.Media;
using Nop.Services.Media;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Media;

[TestFixture]
public class PictureServiceFileDescriptorTests : ServiceTest
{
    private IPictureService _pictureService;
    private IThumbService _thumbService;
    private readonly List<string> _generatedThumbPaths = [];

    [OneTimeSetUp]
    public void SetUp()
    {
        _pictureService = GetService<IPictureService>();
        _thumbService = GetService<IThumbService>();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        foreach (var path in _generatedThumbPaths)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
                // ignored — best-effort cleanup
            }
        }
    }

    [Test]
    public async Task GetDefaultPictureUrlAsync_DoesNotLeakFileDescriptors()
    {
        if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
            Assert.Ignore("File descriptor counting requires /proc/self/fd (Linux) or /dev/fd (macOS).");

        const int warmupIterations = 10;
        const int testIterations = 100;
        const int baseSize = 990_000;

        for (var i = 0; i < warmupIterations; i++)
        {
            var size = baseSize + i;
            await _pictureService.GetDefaultPictureUrlAsync(size, PictureType.Entity);
            _generatedThumbPaths.Add(await GetDefaultThumbPathAsync(size));
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var fdsBefore = CountOpenFileDescriptors();

        for (var i = 0; i < testIterations; i++)
        {
            var size = baseSize + warmupIterations + i;
            await _pictureService.GetDefaultPictureUrlAsync(size, PictureType.Entity);
            _generatedThumbPaths.Add(await GetDefaultThumbPathAsync(size));
        }

        // Snapshot BEFORE forcing GC — finalizers would close the leaked fds and mask the bug.
        // Production symptom is exactly this "live" state: SKCodec instances sitting in older
        // generations holding open fds until Gen2 GC runs (hours, eventually hitting ulimit -n).
        var fdsAfterLoop = CountOpenFileDescriptors();
        var leakedLive = fdsAfterLoop - fdsBefore;

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var fdsAfterGc = CountOpenFileDescriptors();
        var leakedAfterGc = fdsAfterGc - fdsBefore;

        TestContext.Out.WriteLine($"[fd leak probe] live delta = {leakedLive}, after-GC delta = {leakedAfterGc}");

        leakedLive.Should().Be(0,
            $"GetDefaultPictureUrlAsync leaked {leakedLive} file descriptors across {testIterations} thumb generations " +
            $"(after forced GC: {leakedAfterGc}). Likely an IDisposable (SKCodec/SKBitmap) is not wrapped in `using`.");
    }

    private async Task<string> GetDefaultThumbPathAsync(int targetSize)
    {
        var thumbFileName = $"default-image_{targetSize}.png";
        return await _thumbService.GetThumbLocalPathByFileNameAsync(thumbFileName);
    }

    private static int CountOpenFileDescriptors()
    {
        var fdDir = OperatingSystem.IsLinux() ? "/proc/self/fd" : "/dev/fd";
        return System.IO.Directory.EnumerateFileSystemEntries(fdDir).Count();
    }
}
