using System.Collections.Generic;

namespace Functional.ResultType.Tests;

public class ReasonBaseTests
{
    [Fact]
    public void Error_ShouldUseEmptyMetadata_WhenMetadataIsNotProvided()
    {
        var error = Error.Create("error test");

        Assert.NotNull(error.Metadata);
        Assert.Empty(error.Metadata);
    }

    [Fact]
    public void Error_ShouldUseProvidedMetadata_WhenMetadataIsProvided()
    {
        var metadata = new Dictionary<string, object> { { "key", "value" } };

        var error = Error.Create("error test", metadata);

        Assert.Same(metadata, error.Metadata);
        Assert.Equal("value", error.Metadata["key"]);
    }

    [Fact]
    public void Success_ShouldUseEmptyMetadata_WhenMetadataIsNotProvided()
    {
        var success = Success.Create("success test");

        Assert.NotNull(success.Metadata);
        Assert.Empty(success.Metadata);
    }

    [Fact]
    public void Success_ShouldUseProvidedMetadata_WhenMetadataIsProvided()
    {
        var metadata = new Dictionary<string, object> { { "key", "value" } };

        var success = Success.Create("success test", metadata);

        Assert.Same(metadata, success.Metadata);
        Assert.Equal("value", success.Metadata["key"]);
    }
}
