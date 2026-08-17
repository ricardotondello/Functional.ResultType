namespace Functional.ResultType.Tests;

public class ResultIEnumerableExtensionsTests
{
    [Fact]
    public void CollectSuccess_ShouldReturn_SuccessValues()
    {
        var list = new List<Result<int>>
        {
            Result<int>.Success(1),
            Result<int>.Fail(2),
            Result<int>.Success(3)
        };
    
        var result = list.CollectSuccess()
            .ToList();
    
        var expectedValue = new[] { 1, 3 };
        Assert.Equal(2, result.Count);
        Assert.Equivalent(expectedValue, result);
    }
    
    [Fact]
    public void CollectSuccess_ShouldThrow_WhenListIsNul()
    {
        List<Result<int>> nullList = null!;
        Assert.Throws<ArgumentNullException>(() => nullList.CollectSuccess());
    }
    
    [Fact]
    public void CollectFails_ShouldReturn_FailValues()
    {
        var list = new List<Result<int>>
        {
            Result<int>.Success(1),
            Result<int>.Fail(2),
            Result<int>.Success(3)
        };
    
        var result = list.CollectFails()
            .ToList();
    
        var expectedValue = new[] { 2 };
        Assert.Single(result);
        Assert.Equivalent(expectedValue, result);
    }
    
    [Fact]
    public void CollectFails_ShouldThrow_WhenListIsNul()
    {
        List<Result<int>> nullList = null!;
    
        Assert.Throws<ArgumentNullException>(() => nullList.CollectFails());
    }
}