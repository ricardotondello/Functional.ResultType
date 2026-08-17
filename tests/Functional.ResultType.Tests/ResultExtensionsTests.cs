namespace Functional.ResultType.Tests;

// ReSharper disable UseCollectionExpression
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
public class ResultExtensionsTests
{
    private static readonly FakeObject FakeObject = new() { Name = "fake" };

    #region Sync Tests Methods
    
    [Fact]
    public void ToResultSuccess_ShouldCreateSuccessfulResultWithDefaultValue()
    {
        var result = FakeObject.ToResultSuccess<FakeObject>();
    
        Assert.True(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
        Assert.Equal("fake", FakeObject.Name);
    }
    
    [Fact]
    public void ToResultSuccess_WithReasons_ShouldCreateSuccessfulResultWithSuccessList()
    {
        var result = FakeObject.ToResultSuccess<FakeObject>(new ISuccess[] { Success.Create("Success test") });
    
        Assert.True(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
        Assert.Equivalent(new ISuccess[] { Success.Create("Success test") }, result.Successes);
    }
    
    [Fact]
    public void ToResultFail_WithReasons_ShouldCreateFailedResultWithErrorList()
    {
        var result = FakeObject.ToResultFail<FakeObject>(new IError[] { Error.Create("Error test") });
    
        Assert.False(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
        Assert.Equivalent(new IError[] { Error.Create("Error test") }, result.Errors);
    }
    
    [Fact]
    public void FromReasons_WhenPassedOnlyError_ShouldCreateFailedResultWithErrorList()
    {
        var result = FakeObject.FromReasons<FakeObject>(new IReason[] { Error.Create("Error test") });
    
        Assert.False(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
        Assert.Equivalent(new IError[] { Error.Create("Error test") }, result.Errors);
    }
    
    [Fact]
    public void FromReasons_WhenPassedOnlySuccess_ShouldCreateSuccessResultWithSuccessList()
    {
        var result = FakeObject.FromReasons<FakeObject>(new IReason[] { Success.Create("Success test") });
    
        Assert.True(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
        Assert.Equivalent(new ISuccess[] { Success.Create("Success test") }, result.Successes);
    }
    
    [Fact]
    public void FromReasons_ShouldThrowException_WhenReasonIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => FakeObject.FromReasons<FakeObject>(null!));
    }
    
    [Fact]
    public void FromReasons_ShouldThrowException_WhenReasonIsEmpty()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => FakeObject.FromReasons<FakeObject>(Enumerable.Empty<IReason>()));
    
        Assert.Equal("The status cannot be defined! Reason: reasons has no ISuccess or IError items.", ex.Message);
    }
    
    [Fact]
    public void FromReasons_ShouldThrowException_WhenReasonHasBothStatuses()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => FakeObject.FromReasons<FakeObject>(new IReason[]
        {
            Error.Create("Error"), Success.Create("Success")
        }));

        Assert.Equal("The status cannot be defined! Reason: reasons has both ISuccess and IError items.", ex.Message);
    }
    
    [Fact]
    public void FromException_ShouldCreateFailedResultWithErrorList()
    {
        var exception = new Exception("Test");
    
        var result = FakeObject.FromException<FakeObject>(exception);
    
        Assert.False(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
        Assert.Equivalent(new IError[] { Error.Create(exception.ToString()) }, result.Errors);
    }
    
    [Fact]
    public void From_ShouldCreateFailedResultWhenIssSuccessIsSetToFalseWithNoReason()
    {
        // ReSharper disable once RedundantArgumentDefaultValue
        var result = FakeObject.From<FakeObject>(false, null);
    
        Assert.False(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
        Assert.Empty(result.Errors);
    }
    
    [Fact]
    public void From_ShouldCreateFailedResultWhenIssSuccessIsSetToFalse()
    {
        var reasons = new IError[] { Error.Create("Error reason") };
        var result = FakeObject.From<FakeObject>(false, reasons);
    
        Assert.False(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
        Assert.Single(result.Errors);
        Assert.IsType<Error>(result.Errors.Single());
        Assert.Equal("Error reason", result.Errors.Single().Message);
    }
    
    [Fact]
    public void From_ShouldCreateSuccessResultWhenIssSuccessIsSetToTrue()
    {
        var reasons = new ISuccess[] { Success.Create("Success reason") };
        var result = FakeObject.From<FakeObject>(true, reasons);
    
        Assert.True(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
        Assert.Single(result.Successes);
        Assert.IsType<Success>(result.Successes.Single());
        Assert.Equal("Success reason", result.Successes.Single().Message);
    }
    
    [Fact]
    public void From_WithNullValue_ShouldThrowArgumentNullException()
    {
        object value = null!;
        const bool isSuccess = true;
        var reasons = new ISuccess[] { Success.Create("Success reason") };

        var ex = Assert.Throws<ArgumentNullException>(() => value.From<int>(isSuccess, reasons));
    
        Assert.Equal($"is null (Parameter '{nameof(value)}')", ex.Message);
    }
    
    [Fact]
    public void WhenSuccess_ShouldInvokeActionOnSuccess()
    {
        var successResult = FakeObject.ToResultSuccess<FakeObject>();
        var invoked = false;
    
        var result = successResult.WhenSuccess(() => invoked = true);
    
        Assert.True(invoked);
        Assert.True(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
    }
    
    [Fact]
    public void WhenSuccess_ShouldNotInvokeActionOnFailure()
    {
        var failureResult = FakeObject.ToResultFail<FakeObject>();
        var invoked = false;
    
        var result = failureResult.WhenSuccess(() => invoked = true);
    
        Assert.False(invoked);
        Assert.False(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
    }
    
    [Fact]
    public void WhenFail_ShouldInvokeActionOnFailure()
    {
        var failureResult = FakeObject.ToResultFail<FakeObject>();
        var invoked = false;
    
        var result = failureResult.WhenFail(() => invoked = true);
    
        Assert.True(invoked);
        Assert.False(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
    }
    
    [Fact]
    public void WhenFail_ShouldNotInvokeActionOnSuccess()
    {
        var successResult = FakeObject.ToResultSuccess<FakeObject>();
        var invoked = false;
    
        var result = successResult.WhenFail(() => invoked = true);
    
        Assert.False(invoked);
        Assert.True(result.IsSuccess);
        Assert.Same(FakeObject, result.Value);
    }
    
    [Fact]
    public void WhenMatch_ShouldInvokeCorrectActionBasedOnResult()
    {
        var successResult = FakeObject.ToResultSuccess<FakeObject>();
        var failureResult = FakeObject.ToResultFail<FakeObject>();
        var invokedSuccess = false;
        var invokedFail = false;
    
        var resultSuccess = successResult.WhenMatch(actionSuccess: () => invokedSuccess = true,
            actionFail: () => invokedFail = true);
    
        var resultFail = failureResult.WhenMatch(actionSuccess: () => invokedSuccess = true,
            actionFail: () => invokedFail = true);
    
        Assert.True(invokedSuccess); // Should invoke actionSuccess on success
        Assert.True(invokedFail); // Should invoke actionFail on failure
        Assert.Same(FakeObject, resultSuccess.Value);
        Assert.Same(FakeObject, resultFail.Value);
    }
    
    [Fact]
    public void WhenSuccess_WithAction_ShouldInvokeActionOnSuccess()
    {
        var successResult = "success".ToResultSuccess<string>();
        var invoked = false;
    
        var act = new Action<string>((value) =>
        {
            Assert.Equal("success", value);
            invoked = true;
        });
    
        var result = successResult.WhenSuccess(value => act(value));
    
        Assert.True(invoked);
        Assert.Same(successResult.Value, result.Value);
    }
    
    [Fact]
    public void WhenSuccess_WithAction_ShouldNotInvokeActionOnFailure()
    {
        var failureResult = "error".ToResultFail<string>();
        var invoked = false;
        
        var act = new Action<string>((value) =>
        {
            Assert.Equal("error", value);
            invoked = true;
        });
        var result = failureResult.WhenSuccess(value => act(value));
    
        Assert.False(invoked);
        Assert.Same(failureResult.Value, result.Value);
    }
    
    [Fact]
    public void WhenFail_WithAction_ShouldInvokeActionOnFailure()
    {
        var failureResult = "error".ToResultFail<string>();
        var invoked = false;
    
        var act = new Action<string>((value) =>
        {
            Assert.Equal("error", value);
            invoked = true;
        });
    
        var result = failureResult.WhenFail(value => act(value));
    
        Assert.True(invoked);
        Assert.Same(failureResult.Value, result.Value);
    }
    
    [Fact]
    public void WhenFail_WithAction_ShouldNotInvokeActionOnSuccess()
    {
        var successResult = "success".ToResultSuccess<string>();
        var invoked = false;
    
        var act = new Action<string>((value) =>
        {
            Assert.Equal("success", value);
            invoked = true;
        });
    
        var result = successResult.WhenFail(value => act(value));
    
        Assert.False(invoked);
        Assert.Same(successResult.Value, result.Value);
    }
    
    [Fact]
    public void WhenMatch_WithActions_ShouldInvokeCorrectActionBasedOnResult()
    {
        var successResult = "success".ToResultSuccess<string>();
        var failureResult = "error".ToResultFail<string>();
        var invokedSuccess = false;
        var invokedFail = false;
    
        var actSuccess = new Action<string>((value) =>
        {
            Assert.Equal("success", value);
            invokedSuccess = true;
        });
    
        var actFail = new Action<string>((value) =>
        {
            Assert.Equal("error", value);
            invokedFail = true;
        });
    
        var resultSuccess = successResult.WhenMatch(actionSuccess: value => actSuccess(value),
            actionFail: _ => { invokedFail = true; });
    
        var resultFail = failureResult.WhenMatch(actionSuccess: _ => { invokedSuccess = true; },
            actionFail: value => actFail(value));
    
        Assert.True(invokedSuccess); // Should invoke actionSuccess on success
        Assert.True(invokedFail); // Should invoke actionFail on failure
        Assert.Same(successResult, resultSuccess);
        Assert.Same(failureResult, resultFail);
    }
    
    [Fact]
    public void WhenSuccess_WithAction_ShouldInvokeActionOnSuccessAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
    
        var newResult = successResult.WhenSuccess(result =>
        {
            Assert.True(result.IsSuccess);
            Assert.Equal("success", result.Value);
            invoked = true;
        });
    
        Assert.True(invoked);
        Assert.Equal(successResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public void WhenSuccess_WithAction_ShouldNotInvokeActionOnFailureAndReturnSameResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newResult = failureResult.WhenSuccess(result =>
        {
            Assert.False(result.IsSuccess);
            invoked = true;
        });
    
        Assert.False(invoked);
        Assert.Equal(failureResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public void WhenFail_WithAction_ShouldInvokeActionOnFailureAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newResult = failureResult.WhenFail(result =>
        {
            Assert.False(result.IsSuccess);
            Assert.Equal("error", result.Value);
            invoked = true;
        });
    
        Assert.True(invoked);
        Assert.Equal(failureResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public void WhenFail_WithAction_ShouldNotInvokeActionOnSuccessAndReturnSameResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
    
        var newResult = successResult.WhenFail(result =>
        {
            Assert.True(result.IsSuccess);
            invoked = true;
        });
    
        Assert.False(invoked);
        Assert.Equal(successResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public void WhenMatch_WithActions_ShouldInvokeCorrectActionBasedOnResultAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var failureResult = Result<string>.Fail("error");
        var invokedSuccess = false;
        var invokedFail = false;
    
        var newSuccessResult = successResult.WhenMatch(actionSuccess: result =>
        {
            Assert.True(result.IsSuccess);
            Assert.Equal("success", result.Value);
            invokedSuccess = true;
        }, actionFail: _ => { invokedFail = true; });
    
        var newFailureResult = failureResult.WhenMatch(actionSuccess: _ => { invokedSuccess = true; },
            actionFail: result =>
            {
                Assert.False(result.IsSuccess);
                Assert.Equal("error", result.Value);
                invokedFail = true;
            });
    
        Assert.True(invokedSuccess); // Should invoke actionSuccess on success
        Assert.True(invokedFail); // Should invoke actionFail on failure
        Assert.Equal(successResult, newSuccessResult); // Validate the returned result is the same instance
        Assert.Equal(failureResult, newFailureResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public void OnSuccess_WithFunc_ShouldInvokeFuncAndReturnResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
    
        var newResult = successResult.OnSuccess(() =>
        {
            invoked = true;
            return Result<string>.Success("newSuccess");
        });
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal("newSuccess", newResult.Value);
    }
    
    [Fact]
    public void OnSuccess_WithFunc_ShouldNotInvokeFuncAndReturnSameResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newResult = failureResult.OnSuccess(() =>
        {
            invoked = true;
            return Result<string>.Success("newSuccess");
        });
    
        Assert.False(invoked);
        Assert.Equal(failureResult, newResult);
    }
    
    [Fact]
    public void OnFail_WithFunc_ShouldInvokeFuncAndReturnResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newResult = failureResult.OnFail(() =>
        {
            invoked = true;
            return Result<string>.Success("newSuccess");
        });
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal("newSuccess", newResult.Value);
    }
    
    [Fact]
    public void OnFail_WithFunc_ShouldNotInvokeFuncAndReturnSameResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
    
        var newResult = successResult.OnFail(() =>
        {
            invoked = true;
            return Result<string>.Success("newSuccess");
        });
    
        Assert.False(invoked);
        Assert.Equal(successResult, newResult);
    }
    
    [Fact]
    public void OnAny_WithFunc_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newSuccessResult = successResult.OnAny((value) =>
        {
            Assert.Equal("success", value.Value);
            invoked = true;
            return Result<string>.Success("newSuccess");
        });
    
        var newFailureResult = failureResult.OnAny(() =>
        {
            invoked = true;
            return Result<string>.Success("newSuccess");
        });
    
        Assert.True(invoked);
        Assert.True(newSuccessResult.IsSuccess);
        Assert.Equal("newSuccess", newSuccessResult.Value);
        Assert.True(newFailureResult.IsSuccess);
        Assert.Equal("newSuccess", newFailureResult.Value);
    }
    
    [Fact]
    public void OnSuccess_WithFuncWithValue_ShouldInvokeFuncWithValueAndReturnResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
    
        var newResult = successResult.OnSuccess(Fn);
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal("success_new", newResult.Value);
        return;
    
        Result<string> Fn(string value)
        {
            invoked = true;
            return Result<string>.Success(value + "_new");
        }
    }
    
    [Fact]
    public void OnSuccess_WithFuncWithValue_ShouldNotInvokeFuncAndReturnSameResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newResult = failureResult.OnSuccess(Fn);
    
        Assert.False(invoked);
        Assert.Equal(failureResult, newResult);
        return;
    
        Result<string> Fn(string value)
        {
            invoked = true;
            return Result<string>.Success(value + "_new");
        }
    }
    
    [Fact]
    public void OnFail_WithFuncWithValue_ShouldInvokeFuncWithValueAndReturnResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newResult = failureResult.OnFail(Fn);
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal("error_new", newResult.Value);
        return;
    
        Result<string> Fn(string value)
        {
            invoked = true;
            return Result<string>.Success(value + "_new");
        }
    }
    
    [Fact]
    public void OnFail_WithFuncWithValue_ShouldNotInvokeFuncAndReturnSameResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
    
        var newResult = successResult.OnFail(Fn);
    
        Assert.False(invoked);
        Assert.Equal(successResult, newResult);
        return;
    
        Result<string> Fn(string value)
        {
            invoked = true;
            return Result<string>.Success(value + "_new");
        }
    }
    
    [Fact]
    public void OnSuccess_WithFuncWithResult_ShouldInvokeFuncWithResultAndReturnResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
    
        var newResult = successResult.OnSuccess(result =>
        {
            invoked = true;
            return Result<string>.Success(result.Value + "_new");
        });
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal("success_new", newResult.Value);
    }
    
    [Fact]
    public void OnSuccess_WithFuncWithResult_ShouldNotInvokeFuncAndReturnSameResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newResult = failureResult.OnSuccess(result =>
        {
            invoked = true;
            return Result<string>.Success(result.Value + "_new");
        });
    
        Assert.False(invoked);
        Assert.Equal(failureResult, newResult);
    }
    
    [Fact]
    public void OnFail_WithFuncWithResult_ShouldInvokeFuncWithResultAndReturnResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newResult = failureResult.OnFail(result =>
        {
            invoked = true;
            return Result<string>.Success(result.Value + "_new");
        });
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal("error_new", newResult.Value);
    }
    
    [Fact]
    public void OnFail_WithFuncWithResult_ShouldNotInvokeFuncAndReturnSameResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
    
        var newResult = successResult.OnFail(result =>
        {
            invoked = true;
            return Result<string>.Success(result.Value + "_new");
        });
    
        Assert.False(invoked);
        Assert.Equal(successResult, newResult);
    }
    
    [Fact]
    public void OnAny_WithFuncWithResult_ShouldInvokeFuncWithResultAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newSuccessResult = successResult.OnAny(result =>
        {
            invoked = true;
            return Result<string>.Success(result.Value + "_new");
        });
    
        var newFailureResult = failureResult.OnAny(result =>
        {
            invoked = true;
            return Result<string>.Success(result.Value + "_new");
        });
    
        Assert.True(invoked);
        Assert.True(newSuccessResult.IsSuccess);
        Assert.Equal("success_new", newSuccessResult.Value);
        Assert.True(newFailureResult.IsSuccess);
        Assert.Equal("error_new", newFailureResult.Value);
    }
    
    #endregion
    
    #region Async Tests Methods

    [Fact]
    public async Task WhenSuccessAsync_WithAction_ShouldInvokeActionOnSuccessAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
    
        var newResult = await successResult.WhenSuccessAsync(() => { invoked = true; });
    
        Assert.True(invoked);
        Assert.Equal(successResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenSuccessAsync_WithAction_ShouldNotInvokeActionOnFailureAndReturnSameResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newResult = await failureResult.WhenSuccessAsync(() => { invoked = true; });
    
        Assert.False(invoked);
        Assert.Equal(failureResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenSuccessAsyncTask_WithAction_ShouldInvokeActionOnSuccessAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
    
        var newResultTask = Task.FromResult(successResult).WhenSuccessAsync(() => { invoked = true; });
    
        var newResult = await newResultTask;
    
        Assert.True(invoked);
        Assert.Equal(successResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenSuccessAsyncTask_WithAction_ShouldNotInvokeActionOnFailureAndReturnSameResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newResultTask = Task.FromResult(failureResult).WhenSuccessAsync(() => { invoked = true; });
    
        var newResult = await newResultTask;
    
        Assert.False(invoked);
        Assert.Equal(failureResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenFailAsync_WithAction_ShouldInvokeActionOnFailureAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newResult = await failureResult.WhenFailAsync(() => { invoked = true; });
    
        Assert.True(invoked);
        Assert.Equal(failureResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenFailAsync_WithAction_ShouldNotInvokeActionOnSuccessAndReturnSameResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
    
        var newResult = await successResult.WhenFailAsync(() => { invoked = true; });
    
        Assert.False(invoked);
        Assert.Equal(successResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenFailAsyncTask_WithAction_ShouldInvokeActionOnFailureAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
    
        var newResultTask = Task.FromResult(failureResult).WhenFailAsync(() => { invoked = true; });
    
        var newResult = await newResultTask;
    
        Assert.True(invoked);
        Assert.Equal(failureResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenFailAsyncTask_WithAction_ShouldNotInvokeActionOnSuccessAndReturnSameResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
    
        var newResultTask = Task.FromResult(successResult).WhenFailAsync(() => { invoked = true; });
    
        var newResult = await newResultTask;
    
        Assert.False(invoked);
        Assert.Equal(successResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenMatchAsync_WithActions_ShouldInvokeCorrectActionAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var failureResult = Result<string>.Fail("error");
    
        var invokedSuccess = false;
        var invokedFail = false;
    
        var newSuccessResult =
            await successResult.WhenMatchAsync(() => invokedSuccess = true, () => invokedFail = true);
    
        var newFailureResult =
            await failureResult.WhenMatchAsync(() => invokedSuccess = true, () => invokedFail = true);
    
        Assert.True(invokedSuccess);
        Assert.True(invokedFail);
        Assert.Equal(successResult, newSuccessResult);
        Assert.Equal(failureResult, newFailureResult);
    }
    
    [Fact]
    public async Task WhenMatchAsyncTask_WithActions_ShouldInvokeCorrectActionAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var failureResult = Result<string>.Fail("error");
    
        var invokedSuccess = false;
        var invokedFail = false;
    
        var newSuccessResultTask = Task.FromResult(successResult)
            .WhenMatchAsync(() => invokedSuccess = true, () => invokedFail = true);
    
        var newFailureResultTask = Task.FromResult(failureResult)
            .WhenMatchAsync(() => invokedSuccess = true, () => invokedFail = true);
    
        var newSuccessResult = await newSuccessResultTask;
        var newFailureResult = await newFailureResultTask;
    
        Assert.True(invokedSuccess);
        Assert.True(invokedFail);
        Assert.Equal(successResult, newSuccessResult);
        Assert.Equal(failureResult, newFailureResult);
    }
    
    [Fact]
    public async Task WhenSuccessAsyncTask_WithAction_ShouldInvokeActionWithValueOnSuccessAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        var value = string.Empty;
    
        var newResult = await successResult.WhenSuccessAsync(val =>
        {
            invoked = true;
            value = val;
        });
    
        Assert.True(invoked);
        Assert.Equal("success", value);
        Assert.Equal(successResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenSuccessAsync_WithAction_ShouldNotInvokeActionAndReturnSameResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        var value = string.Empty;
    
        var newResult = await failureResult.WhenSuccessAsync(val =>
        {
            invoked = true;
            value = val;
        });
    
        Assert.False(invoked);
        Assert.Empty(value);
        Assert.Equal(failureResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenSuccessAsync_WithAction_ShouldInvokeActionWithValueOnSuccessAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        var value = string.Empty;
    
        var newResultTask = Task.FromResult(successResult).WhenSuccessAsync(val =>
        {
            invoked = true;
            value = val;
        });
    
        var newResult = await newResultTask;
    
        Assert.True(invoked);
        Assert.Equal("success", value);
        Assert.Equal(successResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenSuccessAsyncTask_WithAction_ShouldNotInvokeActionAndReturnSameResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        var value = string.Empty;
    
        var newResultTask = Task.FromResult(failureResult).WhenSuccessAsync(val =>
        {
            invoked = true;
            value = val;
        });
    
        var newResult = await newResultTask;
    
        Assert.False(invoked);
        Assert.Empty(value);
        Assert.Equal(failureResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenFailAsync_WithAction_ShouldInvokeActionWithValueOnFailureAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        var value = string.Empty;
    
        var newResult = await failureResult.WhenFailAsync(val =>
        {
            invoked = true;
            value = val;
        });
    
        Assert.True(invoked);
        Assert.Equal("error", value);
        Assert.Equal(failureResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenFailAsync_WithAction_ShouldNotInvokeActionAndReturnSameResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        var value = string.Empty;
    
        var newResult = await successResult.WhenFailAsync(val =>
        {
            invoked = true;
            value = val;
        });
    
        Assert.False(invoked);
        Assert.Empty(value);
        Assert.Equal(successResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenFailAsyncTask_WithAction_ShouldInvokeActionWithValueOnFailureAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        var value = string.Empty;
    
        var newResultTask = Task.FromResult(failureResult).WhenFailAsync(val =>
        {
            invoked = true;
            value = val;
        });
    
        var newResult = await newResultTask;
    
        Assert.True(invoked);
        Assert.Equal("error", value);
        Assert.Equal(failureResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenFailAsyncTask_WithAction_ShouldNotInvokeActionAndReturnSameResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        var value = string.Empty;
    
        var newResultTask = Task.FromResult(successResult).WhenFailAsync(val =>
        {
            invoked = true;
            value = val;
        });
    
        var newResult = await newResultTask;
    
        Assert.False(invoked);
        Assert.Empty(value);
        Assert.Equal(successResult, newResult); // Validate the returned result is the same instance
    }
    
    [Fact]
    public async Task WhenMatchAsync_WithActionsSuccessAndFail_ShouldInvokeCorrectActionAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var failureResult = Result<string>.Fail("error");
    
        var invokedSuccess = false;
        var invokedFail = false;
        var valueSuccess = string.Empty;
        var valueFail = string.Empty;
    
        var newSuccessResult = await successResult.WhenMatchAsync(val =>
        {
            invokedSuccess = true;
            valueSuccess = val;
        }, val =>
        {
            invokedFail = true;
            valueSuccess = val;
        });
    
        var newFailureResult = await failureResult.WhenMatchAsync(val =>
        {
            invokedSuccess = true;
            valueFail = val;
        }, val =>
        {
            invokedFail = true;
            valueFail = val;
        });
    
        Assert.True(invokedSuccess);
        Assert.True(invokedFail);
        Assert.Equal(successResult.Value, valueSuccess);
        Assert.Equal(failureResult.Value, valueFail);
        Assert.Equal(successResult, newSuccessResult);
        Assert.Equal(failureResult, newFailureResult);
    }
    
    [Fact]
    public async Task WhenMatchAsync_WithActionSuccessAndFail_ShouldInvokeCorrectActionAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var failureResult = Result<string>.Fail("error");
    
        var invokedSuccess = false;
        var invokedFail = false;
        var valueSuccess = string.Empty;
        var valueFail = string.Empty;
    
        var newSuccessResultTask = Task.FromResult(successResult).WhenMatchAsync(val =>
        {
            invokedSuccess = true;
            valueSuccess = val;
        }, val =>
        {
            invokedFail = true;
            valueSuccess = val;
        });
    
        var newFailureResultTask = Task.FromResult(failureResult).WhenMatchAsync(val =>
        {
            invokedSuccess = true;
            valueFail = val;
        }, val =>
        {
            invokedFail = true;
            valueFail = val;
        });
    
        var newSuccessResult = await newSuccessResultTask;
        var newFailureResult = await newFailureResultTask;
    
        Assert.True(invokedSuccess);
        Assert.True(invokedFail);
        Assert.Equal(successResult.Value, valueSuccess);
        Assert.Equal(failureResult.Value, valueFail);
        Assert.Equal(successResult, newSuccessResult);
        Assert.Equal(failureResult, newFailureResult);
    }
    
    [Fact]
    public async Task WhenSuccessAsync_WithActionTask_ShouldInvokeActionOnSuccessAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        Result<string> invokedResult = null!;
    
        var newResultTask = Task.FromResult(successResult).WhenSuccessAsync(res =>
        {
            invoked = true;
            invokedResult = res;
        });
    
        var newResult = await newResultTask;
    
        Assert.True(invoked);
        Assert.Equal(successResult, invokedResult);
        Assert.Equal(successResult, newResult);
    }
    
    [Fact]
    public async Task WhenSuccessAsync_WithActionTask_ShouldNotInvokeActionAndReturnSameResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        Result<string> invokedResult = null!;
    
        var newResultTask = Task.FromResult(failureResult).WhenSuccessAsync(res =>
        {
            invoked = true;
            invokedResult = res;
        });
    
        var newResult = await newResultTask;
    
        Assert.False(invoked);
        Assert.Null(invokedResult);
        Assert.Equal(failureResult, newResult);
    }
    
    [Fact]
    public async Task WhenSuccessAsyncTask_WithActionResult_ShouldInvokeActionOnSuccessAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        Result<string> invokedResult = null!;
    
        var newResultTask = successResult.WhenSuccessAsync(res =>
        {
            invoked = true;
            invokedResult = res;
        });
    
        var newResult = await newResultTask;
    
        Assert.True(invoked);
        Assert.Equal(successResult, invokedResult);
        Assert.Equal(successResult, newResult);
    }
    
    [Fact]
    public async Task WhenSuccessAsyncTask_WithActionResult_ShouldNotInvokeActionAndReturnSameResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        Result<string> invokedResult = null!;
    
        var newResultTask = failureResult.WhenSuccessAsync(res =>
        {
            invoked = true;
            invokedResult = res;
        });
    
        var newResult = await newResultTask;
    
        Assert.False(invoked);
        Assert.Null(invokedResult);
        Assert.Equal(failureResult, newResult);
    }
    
    [Fact]
    public async Task WhenFailAsync_WithActionResult_ShouldInvokeActionOnFailureAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        Result<string> invokedResult = null!;
    
        var newResultTask = failureResult.WhenFailAsync(res =>
        {
            invoked = true;
            invokedResult = res;
        });
    
        var newResult = await newResultTask;
    
        Assert.True(invoked);
        Assert.Equal(failureResult, invokedResult);
        Assert.Equal(failureResult, newResult);
    }
    
    [Fact]
    public async Task WhenFailAsync_WithActionResult_ShouldNotInvokeActionAndReturnSameResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        Result<string> invokedResult = null!;
    
        var newResultTask = successResult.WhenFailAsync(res =>
        {
            invoked = true;
            invokedResult = res;
        });
    
        var newResult = await newResultTask;
    
        Assert.False(invoked);
        Assert.Null(invokedResult);
        Assert.Equal(successResult, newResult);
    }
    
    [Fact]
    public async Task WhenFailAsyncTask_WithActionResult_ShouldInvokeActionOnFailureAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        Result<string> invokedResult = null!;
    
        var newResultTask = Task.FromResult(failureResult).WhenFailAsync(res =>
        {
            invoked = true;
            invokedResult = res;
        });
    
        var newResult = await newResultTask;
    
        Assert.True(invoked);
        Assert.Equal(failureResult, invokedResult);
        Assert.Equal(failureResult, newResult);
    }
    
    [Fact]
    public async Task WhenFailAsyncTask_WithActionResult_ShouldNotInvokeActionAndReturnSameResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        Result<string> invokedResult = null!;
    
        var newResultTask = Task.FromResult(successResult).WhenFailAsync(res =>
        {
            invoked = true;
            invokedResult = res;
        });
    
        var newResult = await newResultTask;
    
        Assert.False(invoked);
        Assert.Null(invokedResult);
        Assert.Equal(successResult, newResult);
    }
    
    [Fact]
    public async Task WhenMatchAsync_WithActionsResult_ShouldInvokeCorrectActionAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var failureResult = Result<string>.Fail("error");
    
        var invokedSuccess = false;
        var invokedFail = false;
        Result<string> invokedResultSuccess = null!;
        Result<string> invokedResultFail = null!;
    
        var successResultTask = successResult.WhenMatchAsync(res =>
        {
            invokedSuccess = true;
            invokedResultSuccess = res;
        }, res =>
        {
            invokedFail = true;
            invokedResultFail = res;
        });
    
        var failureResultTask = failureResult.WhenMatchAsync(res =>
        {
            invokedSuccess = true;
            invokedResultSuccess = res;
        }, res =>
        {
            invokedFail = true;
            invokedResultFail = res;
        });
    
        var newSuccessResult = await successResultTask;
        var newFailureResult = await failureResultTask;
    
        Assert.True(invokedSuccess);
        Assert.True(invokedFail);
        Assert.Equal(successResult, invokedResultSuccess);
        Assert.Equal(failureResult, invokedResultFail);
        Assert.Equal(successResult, newSuccessResult);
        Assert.Equal(failureResult, newFailureResult);
    }
    
    [Fact]
    public async Task WhenMatchAsync_WithActionsResultTask_ShouldInvokeCorrectActionAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var failureResult = Result<string>.Fail("error");
    
        var invokedSuccess = false;
        var invokedFail = false;
        Result<string> invokedResultSuccess = null!;
        Result<string> invokedResultFail = null!;
    
        var successResultTask = Task.FromResult(successResult).WhenMatchAsync(res =>
        {
            invokedSuccess = true;
            invokedResultSuccess = res;
        }, res =>
        {
            invokedFail = true;
            invokedResultFail = res;
        });
    
        var failureResultTask = Task.FromResult(failureResult).WhenMatchAsync(res =>
        {
            invokedSuccess = true;
            invokedResultSuccess = res;
        }, res =>
        {
            invokedFail = true;
            invokedResultFail = res;
        });
    
        var newSuccessResult = await successResultTask;
        var newFailureResult = await failureResultTask;
    
        Assert.True(invokedSuccess);
        Assert.True(invokedFail);
        Assert.Equal(successResult, invokedResultSuccess);
        Assert.Equal(failureResult, invokedResultFail);
        Assert.Equal(successResult, newSuccessResult);
        Assert.Equal(failureResult, newFailureResult);
    }
    
    [Fact]
    public async Task OnSuccessAsync_WithFunc_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        const string newResultValue = "new success value";
    
        var newResult = await successResult.OnSuccessAsync(OnSuccessFunc);
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnSuccessFunc(string value)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnSuccessAsync_WithFunc_ShouldNotInvokeFuncAndReturnSameResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        const string newResultValue = "new success value";
    
        var newResult = await failureResult.OnSuccessAsync(OnSuccessFunc);
    
        Assert.False(invoked);
        Assert.False(newResult.IsSuccess);
        Assert.Equal(failureResult.Value, newResult.Value);
        return;
    
        Task<Result<string>> OnSuccessFunc(string value)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnFailAsync_WithFunc_ShouldInvokeFuncAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        const string newResultValue = "new failure value";
    
        var newResult = await failureResult.OnFailAsync(OnFailFunc);
    
        Assert.True(invoked);
        Assert.False(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnFailFunc(string value)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Fail(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnFailAsync_WithFunc_ShouldNotInvokeFuncAndReturnSameResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        const string newResultValue = "new failure value";
    
        var newResult = await successResult.OnFailAsync(OnFailFunc);
    
        Assert.False(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(successResult.Value, newResult.Value);
        return;
    
        Task<Result<string>> OnFailFunc(string value)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Fail(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnSuccessAsync_WithTaskResultAndFunc_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        const string newResultValue = "new success value";
    
        var newResultTask = Task.FromResult(successResult).OnSuccessAsync(OnSuccessFunc);
    
        var newResult = await newResultTask;
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnSuccessFunc(string value)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnSuccessAsync_WithTaskResultAndFunc_ShouldNotInvokeFuncAndReturnSameResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        const string newResultValue = "new success value";
    
        var newResultTask = Task.FromResult(failureResult).OnSuccessAsync(OnSuccessFunc);
    
        var newResult = await newResultTask;
    
        Assert.False(invoked);
        Assert.False(newResult.IsSuccess);
        Assert.Equal(failureResult.Value, newResult.Value);
        return;
    
        Task<Result<string>> OnSuccessFunc(string value)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnFailAsync_WithTaskResultAndFunc_ShouldInvokeFuncAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        const string newResultValue = "new failure value";
    
        var newResultTask = Task.FromResult(failureResult).OnFailAsync(OnFailFunc);
    
        var newResult = await newResultTask;
    
        Assert.True(invoked);
        Assert.False(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnFailFunc(string value)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Fail(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnFailAsync_WithTaskResultAndFunc_ShouldNotInvokeFuncAndReturnSameResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        const string newResultValue = "new failure value";
    
        var newResultTask = Task.FromResult(successResult).OnFailAsync(OnFailFunc);
    
        var newResult = await newResultTask;
    
        Assert.False(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(successResult.Value, newResult.Value);
        return;
    
        Task<Result<string>> OnFailFunc(string value)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Fail(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnSuccessAsync_WithFunc_TaskResult_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        const string newResultValue = "new success value";
    
        var newResult = await successResult.OnSuccessAsync(OnSuccessFunc);
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnSuccessFunc()
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnSuccessAsync_WithFunc_TaskResult_ShouldNotInvokeFuncAndReturnSameResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        const string newResultValue = "new success value";
    
        var newResult = await failureResult.OnSuccessAsync(OnSuccessFunc);
    
        Assert.False(invoked);
        Assert.False(newResult.IsSuccess);
        Assert.Equal(failureResult.Value, newResult.Value);
        return;
    
        Task<Result<string>> OnSuccessFunc()
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnFailAsync_WithFunc_TaskResult_ShouldInvokeFuncAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        const string newResultValue = "new failure value";
    
        var newResult = await failureResult.OnFailAsync(OnFailFunc);
    
        Assert.True(invoked);
        Assert.False(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnFailFunc()
        {
            invoked = true;
            return Task.FromResult(Result<string>.Fail(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnFailAsync_WithFunc_TaskResult_ShouldNotInvokeFuncAndReturnSameResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        const string newResultValue = "new failure value";
    
        var newResult = await successResult.OnFailAsync(OnFailFunc);
    
        Assert.False(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(successResult.Value, newResult.Value);
        return;
    
        Task<Result<string>> OnFailFunc()
        {
            invoked = true;
            return Task.FromResult(Result<string>.Fail(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnAnyAsync_WithFunc_TaskResult_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        const string newResultValue = "new success value";
    
        var newResult = await successResult.OnAnyAsync(OnAnyFunc);
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnAnyFunc()
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnSuccessAsync_WithFunc_TaskResultWithInput_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        const string newResultValue = "new success value";
    
        var newResult = await successResult.OnSuccessAsync(OnSuccessFunc);
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnSuccessFunc(Result<string> input)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnSuccessAsync_WithFunc_TaskResultWithInput_ShouldNotInvokeFuncAndReturnSameResultOnFailure()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        const string newResultValue = "new success value";
    
        var newResult = await failureResult.OnSuccessAsync(OnSuccessFunc);
    
        Assert.False(invoked);
        Assert.False(newResult.IsSuccess);
        Assert.Equal(failureResult.Value, newResult.Value);
        return;
    
        Task<Result<string>> OnSuccessFunc(Result<string> input)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnFailAsync_WithFunc_TaskResultWithInput_ShouldInvokeFuncAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;
        const string newResultValue = "new failure value";
    
        var newResult = await failureResult.OnFailAsync(OnFailFunc);
    
        Assert.True(invoked);
        Assert.False(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnFailFunc(Result<string> input)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Fail(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnFailAsync_WithFunc_TaskResultWithInput_ShouldNotInvokeFuncAndReturnSameResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        const string newResultValue = "new failure value";
    
        var newResult = await successResult.OnFailAsync(OnFailFunc);
    
        Assert.False(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(successResult.Value, newResult.Value);
        return;
    
        Task<Result<string>> OnFailFunc(Result<string> input)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Fail(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnAnyAsync_WithFunc_TaskResultWithInput_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        const string newResultValue = "new result value";
    
        var newResult = await successResult.OnAnyAsync(OnAnyFunc);
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnAnyFunc(Result<string> input)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnSuccessAsync_WithFunc_TaskResultTask_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Task.FromResult(Result<string>.Success("success"));
        var invoked = false;
        const string newResultValue = "new success value";
    
        var newResult = await successResult.OnSuccessAsync(OnSuccessFunc);
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnSuccessFunc()
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnSuccessAsync_WithFunc_TaskResultTask_ShouldNotInvokeFuncAndReturnSameResultOnFailure()
    {
        var failureResult = Task.FromResult(Result<string>.Fail("error"));
        var invoked = false;
        const string newResultValue = "new success value";
    
        var newResult = await failureResult.OnSuccessAsync(OnSuccessFunc);
    
        Assert.False(invoked);
        Assert.False(newResult.IsSuccess);
        Assert.Equal((await failureResult).Value, newResult.Value);
        return;
    
        Task<Result<string>> OnSuccessFunc()
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnFailAsyncTask_WithFunc_TaskResultTask_ShouldInvokeFuncAndReturnResult()
    {
        var failureResult = Task.FromResult(Result<string>.Fail("error"));
        var invoked = false;
        const string newResultValue = "new failure value";
    
        var newResult = await failureResult.OnFailAsync(OnFailFunc);
    
        Assert.True(invoked);
        Assert.False(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnFailFunc()
        {
            invoked = true;
            return Task.FromResult(Result<string>.Fail(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnFailAsyncTask_WithFuncTask_TaskResultTask_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Task.FromResult(Result<string>.Success("success"));
        var invoked = false;
        const string newResultValue = "new failure value";
    
        var newResult = await successResult.OnFailAsync(OnFailFunc);
    
        Assert.False(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal("success", newResult.Value);
        return;
    
        Task<Result<string>> OnFailFunc()
        {
            invoked = true;
            return Task.FromResult(Result<string>.Fail(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnAnyAsync_WithFunc_TaskResultTask_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Task.FromResult(Result<string>.Success("success"));
        var invoked = false;
        const string newResultValue = "new value";
    
        var newResult = await successResult.OnAnyAsync(OnAnyFunc);
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnAnyFunc()
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnSuccessAsyncTask_WithFuncFn_TaskResultTask_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Task.FromResult(Result<string>.Success("success"));
        var invoked = false;
        const string newResultValue = "new value";
    
        var newResult = await successResult.OnSuccessAsync(OnSuccessFunc);
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnSuccessFunc(Result<string> result)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnSuccessAsyncTask_WithFunc_TaskResultTask_ShouldNotInvokeFuncAndReturnResult()
    {
        var errorResult = Task.FromResult(Result<string>.Fail("error"));
        var invoked = false;
        const string newResultValue = "new value";
    
        var newResult = await errorResult.OnSuccessAsync(OnSuccessFunc);
    
        Assert.False(invoked);
        Assert.False(newResult.IsSuccess);
        Assert.Equal("error", newResult.Value);
        return;
    
        Task<Result<string>> OnSuccessFunc(Result<string> result)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnFailAsync_WithFunc_TaskResultTask_ShouldInvokeFuncAndReturnResult()
    {
        var failResult = Task.FromResult(Result<string>.Fail("failure"));
        var invoked = false;
        const string newResultValue = "new value";
    
        var newResult = await failResult.OnFailAsync(OnFailFunc);
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnFailFunc(Result<string> result)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnFailAsync_WithFunc_TaskResultTask_ShouldNotInvokeFuncAndReturnResult()
    {
        var successResult = Task.FromResult(Result<string>.Success("success"));
        var invoked = false;
        const string newResultValue = "new value";
    
        var newResult = await successResult.OnFailAsync(OnFailFunc);
    
        Assert.False(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal("success", newResult.Value);
        return;
    
        Task<Result<string>> OnFailFunc(Result<string> result)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    [Fact]
    public async Task OnAnyAsyncTask_WithFunc_TaskResultTask_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Task.FromResult(Result<string>.Success("success"));
        var invoked = false;
        const string newResultValue = "new value";
    
        var newResult = await successResult.OnAnyAsync(OnAnyFunc);
    
        Assert.True(invoked);
        Assert.True(newResult.IsSuccess);
        Assert.Equal(newResultValue, newResult.Value);
        return;
    
        Task<Result<string>> OnAnyFunc(Result<string> result)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }
    
    #endregion
}