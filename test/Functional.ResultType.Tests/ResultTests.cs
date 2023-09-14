using FluentAssertions;

namespace Functional.ResultType.Tests;

public class ResultTests
{
    [Fact]
    public void Result_ShouldHaveTypeProperty()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        var obj = Result<FakeObject>.Success(fakeObject);

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
        result.Errors.ElementAt(0).Should().BeEquivalentTo(Error.Create("Type mismatch"));
    }

    [Fact]
    public void HasError_ShouldReturnTrue_WhenResultHasErrors()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        var obj = Result<FakeObject>.Fail(fakeObject, new IError[] { Error.Create("error test") });

        obj.HasErrors.Should().BeTrue();
        obj.HasSuccesses.Should().BeFalse();
        obj.Errors.ElementAt(0).Should().BeEquivalentTo(Error.Create("error test"));
        obj.Successes.Should().BeEmpty();
    }

    [Fact]
    public void HasSuccess_ShouldReturnTrue_WhenResultHasSuccess()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        var obj = Result<FakeObject>.Success(fakeObject, new ISuccess[] { Success.Create("Success test") });

        obj.HasSuccesses.Should().BeTrue();
        obj.HasErrors.Should().BeFalse();
        obj.Successes.ElementAt(0).Should().BeEquivalentTo(Success.Create("Success test"));
        obj.Errors.Should().BeEmpty();
    }
}