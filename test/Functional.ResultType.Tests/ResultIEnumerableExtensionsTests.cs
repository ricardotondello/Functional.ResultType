﻿using FluentAssertions;

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
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedValue);
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

        var expectedValue = new[] { 2 };
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(expectedValue);
    }
    
    [Fact]
    public void CollectFails_ShouldThrow_WhenListIsNul()
    {
        List<Result<int>> nullList = null!;

        Action act = () => nullList.CollectFails();

        act.Should().Throw<ArgumentNullException>();
    }
}