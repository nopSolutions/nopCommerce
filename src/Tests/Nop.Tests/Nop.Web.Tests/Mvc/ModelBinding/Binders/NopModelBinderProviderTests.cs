using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.ModelBinding.Binders;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Mvc.ModelBinding.Binders;

[TestFixture]
public class NopModelBinderProviderTests
{
    [Test]
    public void ShouldNotBindStringFromNonValueProviderSource()
    {
        GetBinder(BindingSource.Header).Should().BeNull();
        GetBinder(BindingSource.Body).Should().BeNull();
    }

    [Test]
    public void ShouldBindStringFromValueProviderSource()
    {
        GetBinder(null).Should().BeOfType<StringModelBinder>();
        GetBinder(BindingSource.Query).Should().BeOfType<StringModelBinder>();
    }

    private static IModelBinder GetBinder(BindingSource bindingSource)
    {
        var metadataProvider = new EmptyModelMetadataProvider();
        var context = new TestContext(
            metadataProvider.GetMetadataForType(typeof(string)),
            new BindingInfo { BindingSource = bindingSource },
            metadataProvider);

        return ((IModelBinderProvider)new NopModelBinderProvider()).GetBinder(context);
    }

    private sealed class TestContext(ModelMetadata metadata, BindingInfo bindingInfo, IModelMetadataProvider provider)
        : ModelBinderProviderContext
    {
        public override BindingInfo BindingInfo { get; } = bindingInfo;

        public override ModelMetadata Metadata { get; } = metadata;

        public override IModelMetadataProvider MetadataProvider { get; } = provider;

        public override IModelBinder CreateBinder(ModelMetadata metadata) => throw new NotImplementedException();
    }
}
