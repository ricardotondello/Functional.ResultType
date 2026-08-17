namespace Functional.ResultType.Tests;

public class ResultTests
{
    [Fact]
    public void Result_ShouldHaveTypeProperty()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        var obj = Result<FakeObject>.Success(fakeObject);

        Assert.Equal(typeof(FakeObject), obj.Type);
    }

    [Fact]
    public void TryParse_ShouldParseAndReturn_WhenPrimitiveObjectIsValid()
    {
        const string obj = "test";
        var parsed = Result<string>.TryParse(obj, out var result);
    
        Assert.True(parsed);
        Assert.Same(obj, result.Value);
    }
    
    [Fact]
    public void TryParse_ShouldParseAndReturn_WhenObjectClassIsValid()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        var parsed = Result<FakeObject>.TryParse(fakeObject, out var result);
    
        Assert.True(parsed);
        Assert.Same(fakeObject, result.Value);
    }
    
    [Fact]
    public void TryParse_ShouldNotParseAndReturnFalse_WhenObjectIsInvalid()
    {
        FakeObject fakeObject = null!;
        var parsed = Result<FakeObject>.TryParse(fakeObject, out var result);
    
        Assert.False(parsed);
        Assert.False(result.IsSuccess);
    }
    
    [Fact]
    public void TryParse_ShouldNotParseAndReturnFalse_WhenObjectTypeIsMismatched()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        var parsed = Result<string>.TryParse(fakeObject, out var result);
    
        Assert.False(parsed);
        Assert.False(result.IsSuccess);
        Assert.Equivalent(Error.Create("Type mismatch"), result.Errors.ElementAt(0));
    }
    
    [Fact]
    public void HasError_ShouldReturnTrue_WhenResultHasErrors()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        // ReSharper disable once UseCollectionExpression
        var obj = Result<FakeObject>.Fail(fakeObject, new IError[] { Error.Create("error test") });
    
        Assert.True(obj.HasErrors);
        Assert.False(obj.HasSuccesses);
        Assert.Equivalent(Error.Create("error test"), obj.Errors.ElementAt(0));
        Assert.Empty(obj.Successes);
    }
    
    [Fact]
    public void HasSuccess_ShouldReturnTrue_WhenResultHasSuccess()
    {
        var fakeObject = new FakeObject { Name = "fake" };
        // ReSharper disable once UseCollectionExpression
        var obj = Result<FakeObject>.Success(fakeObject, new ISuccess[] { Success.Create("Success test") });
    
        Assert.True(obj.HasSuccesses);
        Assert.False(obj.HasErrors);
        Assert.Equivalent(Success.Create("Success test"), obj.Successes.ElementAt(0));
        Assert.Empty(obj.Errors);
    }
}