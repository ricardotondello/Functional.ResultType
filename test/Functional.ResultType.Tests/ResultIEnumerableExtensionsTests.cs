using FluentAssertions;

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
        
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(new[]{1, 3});
    }
    
    [Fact]
    public void CollectSuccess_ShouldThrow_WhenListIsNul()
    {
        List<Result<int>> nullList = null!;

        Action act = () => nullList.CollectSuccess();

        act.Should().Throw<ArgumentNullException>();
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
        
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(new[]{2});
    }
    
    [Fact]
    public void CollectFails_ShouldThrow_WhenListIsNul()
    {
        List<Result<int>> nullList = null!;

        Action act = () => nullList.CollectFails();

        act.Should().Throw<ArgumentNullException>();
    }
}