using FluentAssertions;

namespace Functional.ResultType.Tests;

public class ResultTests
{
    [Fact]
    public void Result_ShouldHaveTypeProperty()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        var obj = fakeObject.ToResultSuccess<FakeObject>();

        obj.Type.Should().Be(typeof(FakeObject));
    }
    
    [Fact]
    public void TryParse_ShouldParseAndReturn_WhenPrimitiveObjectIsValid()
    {
        const string obj = "test";
        var parsed = Result<string>.TryParse(obj, out var result);

        parsed.Should().BeTrue();
        result.Value.Should().BeSameAs(obj);
    }

    [Fact]
    public void TryParse_ShouldParseAndReturn_WhenObjectClassIsValid()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        var parsed = Result<FakeObject>.TryParse(fakeObject, out var result);

        parsed.Should().BeTrue();
        result.Value.Should().BeSameAs(fakeObject);
    }

    [Fact]
    public void TryParse_ShouldNotParseAndReturnFalse_WhenObjectIsInvalid()
    {
        FakeObject fakeObject = null!;
        var parsed = Result<FakeObject>.TryParse(fakeObject, out var result);

        parsed.Should().BeFalse();
        result.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public void TryParse_ShouldNotParseAndReturnFalse_WhenObjectTypeIsMismatched()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        var parsed = Result<string>.TryParse(fakeObject, out var result);

        parsed.Should().BeFalse();
        result.IsSuccess.Should().BeFalse();
    }
    
    [Fact]
    public void TryParseWithMessage_ShouldParseAndReturn_WhenPrimitiveObjectIsValid()
    {
        const string obj = "test";
        var parsed = Result<string>.TryParse(obj, () => "success message", () => "fail message", out var result);

        parsed.Should().BeTrue();
        result.Value.Should().BeSameAs(obj);
        result.Message.Should().Be("success message");
    }

    [Fact]
    public void TryParseWithMessage_ShouldParseAndReturn_WhenObjectClassIsValid()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        var parsed = Result<FakeObject>.TryParse(fakeObject, () => "success message", () => "fail message", out var result);

        parsed.Should().BeTrue();
        result.Value.Should().BeSameAs(fakeObject);
        result.Message.Should().Be("success message");
    }

    [Fact]
    public void TryParseWithMessage_ShouldNotParseAndReturnFalse_WhenObjectIsInvalid()
    {
        FakeObject fakeObject = null!;
        var parsed = Result<FakeObject>.TryParse(fakeObject, () => "success message", () => "fail message", out var result);

        parsed.Should().BeFalse();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("fail message");
    }
    
    [Fact]
    public void TryParseWithMessage_ShouldNotParseAndReturnFalse_WhenObjectTypeIsMismatched()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        var parsed = Result<string>.TryParse(fakeObject, () => "success message", () => "fail message", out var result);

        parsed.Should().BeFalse();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Type mismatch");
    }
}