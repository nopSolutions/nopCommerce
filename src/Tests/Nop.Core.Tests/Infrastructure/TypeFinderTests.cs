using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Nop.Core.Infrastructure;
using Nop.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Core.Tests.Infrastructure
{
    [TestFixture]
    public class TypeFinderTests
    {
        [Test]
        public void TypeFinder_Benchmark_Findings()
        {
            var hostingEnvironment = MockRepository.GenerateMock<IHostingEnvironment>();
            hostingEnvironment.Expect(x => x.ContentRootPath).Return(System.Reflection.Assembly.GetExecutingAssembly().Location);
            hostingEnvironment.Expect(x => x.WebRootPath).Return(System.IO.Directory.GetCurrentDirectory());
            CommonHelper.DefaultFileProvider = new NopFileProvider(hostingEnvironment);
            var finder = new AppDomainTypeFinder();
            var type = finder.FindClassesOfType<ISomeInterface>();
            type.Count().ShouldEqual(1);
            typeof(ISomeInterface).IsAssignableFrom(type.FirstOrDefault()).ShouldBeTrue();
        }

        public interface ISomeInterface
        {
        }

        public class SomeClass : ISomeInterface
        {
        }
    }
}
