using FluentAssertions;

namespace Functional.ResultType.Tests;

public class ResultExtensionsTests
{
    private static readonly FakeObject FakeObject = new() { Name = "fake" };

    #region Sync Tests Methods

    [Fact]
    public void ToResultSuccess_ShouldCreateSuccessfulResultWithDefaultValue()
    {
        var result = FakeObject.ToResultSuccess<FakeObject>();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(FakeObject);
        FakeObject.Name.Should().Be("fake");
    }

    [Fact]
    public void ToResultSuccess_WithReasons_ShouldCreateSuccessfulResultWithSuccessList()
    {
        var result = FakeObject.ToResultSuccess<FakeObject>(new ISuccess[] { Success.Create("Success test") });

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(FakeObject);
        result.Successes.Should().BeEquivalentTo(new ISuccess[] { Success.Create("Success test") });
    }

    [Fact]
    public void ToResultFail_WithReasons_ShouldCreateFailedResultWithErrorList()
    {
        var result = FakeObject.ToResultFail<FakeObject>(new IError[] { Error.Create("Error test") });

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeSameAs(FakeObject);
        result.Errors.Should().BeEquivalentTo(new IError[] { Error.Create("Error test") });
    }

    [Fact]
    public void FromReasons_WhenPassedOnlyError_ShouldCreateFailedResultWithErrorList()
    {
        var result = FakeObject.FromReasons<FakeObject>(new IReason[] { Error.Create("Error test") });

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeSameAs(FakeObject);
        result.Errors.Should().BeEquivalentTo(new IError[] { Error.Create("Error test") });
    }

    [Fact]
    public void FromReasons_WhenPassedOnlySuccess_ShouldCreateSuccessResultWithSuccessList()
    {
        var result = FakeObject.FromReasons<FakeObject>(new IReason[] { Success.Create("Success test") });

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(FakeObject);
        result.Successes.Should().BeEquivalentTo(new ISuccess[] { Success.Create("Success test") });
    }

    [Fact]
    public void FromReasons_ShouldThrowException_WhenReasonIsNull()
    {
        Action act = () => FakeObject.FromReasons<FakeObject>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromReasons_ShouldThrowException_WhenReasonIsEmpty()
    {
        // ReSharper disable once UseCollectionExpression
        Action act = () => FakeObject.FromReasons<FakeObject>(Enumerable.Empty<IReason>());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("The status cannot be defined! Reason: reasons has no ISuccess or IError items.");
    }

    [Fact]
    public void FromReasons_ShouldThrowException_WhenReasonHasBothStatuses()
    {
        Action act = () => FakeObject.FromReasons<FakeObject>(new IReason[]
        {
            Error.Create("Error"), Success.Create("Success")
        });

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("The status cannot be defined! Reason: reasons has both ISuccess and IError items.");
    }

    [Fact]
    public void FromException_ShouldCreateFailedResultWithErrorList()
    {
        var exception = new Exception("Test");

        var result = FakeObject.FromException<FakeObject>(exception);

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeSameAs(FakeObject);
        result.Errors.Should().BeEquivalentTo(new IError[] { Error.Create(exception.ToString()) });
    }

    [Fact]
    public void From_ShouldCreateFailedResultWhenIssSuccessIsSetToFalse()
    {
        var reasons = new IError[] { Error.Create("Error reason") };
        var result = FakeObject.From<FakeObject>(false, reasons);

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeSameAs(FakeObject);
        result.Errors.Should().ContainSingle().Which.Should().BeOfType<Error>().And.Subject.As<Error>().Message.Should()
            .Be("Error reason");
    }

    [Fact]
    public void From_ShouldCreateSuccessResultWhenIssSuccessIsSetToTrue()
    {
        var reasons = new ISuccess[] { Success.Create("Success reason") };
        var result = FakeObject.From<FakeObject>(true, reasons);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(FakeObject);
        result.Successes.Should().ContainSingle().Which.Should().BeOfType<Success>().And.Subject.As<Success>().Message
            .Should().Be("Success reason");
    }

    [Fact]
    public void From_WithNullValue_ShouldThrowArgumentNullException()
    {
        object value = null!;
        const bool isSuccess = true;
        var reasons = new ISuccess[] { Success.Create("Success reason") };

        Action act = () => value.From<int>(isSuccess, reasons);

        act.Should().Throw<ArgumentNullException>().WithMessage($"is null (Parameter '{nameof(value)}')");
    }

    [Fact]
    public void WhenSuccess_ShouldInvokeActionOnSuccess()
    {
        var successResult = FakeObject.ToResultSuccess<FakeObject>();
        var invoked = false;

        var result = successResult.WhenSuccess(() => invoked = true);

        invoked.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(FakeObject);
    }

    [Fact]
    public void WhenSuccess_ShouldNotInvokeActionOnFailure()
    {
        var failureResult = FakeObject.ToResultFail<FakeObject>();
        var invoked = false;

        var result = failureResult.WhenSuccess(() => invoked = true);

        invoked.Should().BeFalse();
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeSameAs(FakeObject);
    }

    [Fact]
    public void WhenFail_ShouldInvokeActionOnFailure()
    {
        var failureResult = FakeObject.ToResultFail<FakeObject>();
        var invoked = false;

        var result = failureResult.WhenFail(() => invoked = true);

        invoked.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeSameAs(FakeObject);
    }

    [Fact]
    public void WhenFail_ShouldNotInvokeActionOnSuccess()
    {
        var successResult = FakeObject.ToResultSuccess<FakeObject>();
        var invoked = false;

        var result = successResult.WhenFail(() => invoked = true);

        invoked.Should().BeFalse();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(FakeObject);
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

        invokedSuccess.Should().BeTrue(); // Should invoke actionSuccess on success
        invokedFail.Should().BeTrue(); // Should invoke actionFail on failure
        resultSuccess.Value.Should().BeSameAs(FakeObject);
        resultFail.Value.Should().BeSameAs(FakeObject);
    }

    [Fact]
    public void WhenSuccess_WithAction_ShouldInvokeActionOnSuccess()
    {
        var successResult = "success".ToResultSuccess<string>();
        var invoked = false;

        var act = new Action<string>((value) =>
        {
            value.Should().Be("success");
            invoked = true;
        });

        var result = successResult.WhenSuccess(value => act(value));

        invoked.Should().BeTrue();
        result.Value.Should().BeSameAs(successResult.Value);
    }

    [Fact]
    public void WhenSuccess_WithAction_ShouldNotInvokeActionOnFailure()
    {
        var failureResult = "error".ToResultFail<string>();
        var invoked = false;

        var act = new Action<string>((value) =>
        {
            value.Should().Be("error");
            invoked = true;
        });
        var result = failureResult.WhenSuccess(value => act(value));

        invoked.Should().BeFalse();
        result.Value.Should().BeSameAs(failureResult.Value);
    }

    [Fact]
    public void WhenFail_WithAction_ShouldInvokeActionOnFailure()
    {
        var failureResult = "error".ToResultFail<string>();
        var invoked = false;

        var act = new Action<string>((value) =>
        {
            value.Should().Be("error");
            invoked = true;
        });

        var result = failureResult.WhenFail(value => act(value));

        invoked.Should().BeTrue();
        result.Value.Should().BeSameAs(failureResult.Value);
    }

    [Fact]
    public void WhenFail_WithAction_ShouldNotInvokeActionOnSuccess()
    {
        var successResult = "success".ToResultSuccess<string>();
        var invoked = false;

        var act = new Action<string>((value) =>
        {
            value.Should().Be("success");
            invoked = true;
        });

        var result = successResult.WhenFail(value => act(value));

        invoked.Should().BeFalse();
        result.Value.Should().BeSameAs(successResult.Value);
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
            value.Should().Be("success");
            invokedSuccess = true;
        });

        var actFail = new Action<string>((value) =>
        {
            value.Should().Be("error");
            invokedFail = true;
        });

        var resultSuccess = successResult.WhenMatch(actionSuccess: value => actSuccess(value),
            actionFail: _ => { invokedFail = true; });

        var resultFail = failureResult.WhenMatch(actionSuccess: _ => { invokedSuccess = true; },
            actionFail: value => actFail(value));

        invokedSuccess.Should().BeTrue(); // Should invoke actionSuccess on success
        invokedFail.Should().BeTrue(); // Should invoke actionFail on failure
        resultSuccess.Should().BeSameAs(successResult);
        resultFail.Should().BeSameAs(failureResult);
    }

    [Fact]
    public void WhenSuccess_WithAction_ShouldInvokeActionOnSuccessAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;

        var newResult = successResult.WhenSuccess(result =>
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("success");
            invoked = true;
        });

        invoked.Should().BeTrue();
        newResult.Should().Be(successResult); // Validate the returned result is the same instance
    }

    [Fact]
    public void WhenSuccess_WithAction_ShouldNotInvokeActionOnFailureAndReturnSameResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;

        var newResult = failureResult.WhenSuccess(result =>
        {
            result.IsSuccess.Should().BeFalse();
            invoked = true;
        });

        invoked.Should().BeFalse();
        newResult.Should().Be(failureResult); // Validate the returned result is the same instance
    }

    [Fact]
    public void WhenFail_WithAction_ShouldInvokeActionOnFailureAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;

        var newResult = failureResult.WhenFail(result =>
        {
            result.IsSuccess.Should().BeFalse();
            result.Value.Should().Be("error");
            invoked = true;
        });

        invoked.Should().BeTrue();
        newResult.Should().Be(failureResult); // Validate the returned result is the same instance
    }

    [Fact]
    public void WhenFail_WithAction_ShouldNotInvokeActionOnSuccessAndReturnSameResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;

        var newResult = successResult.WhenFail(result =>
        {
            result.IsSuccess.Should().BeTrue();
            invoked = true;
        });

        invoked.Should().BeFalse();
        newResult.Should().Be(successResult); // Validate the returned result is the same instance
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
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("success");
            invokedSuccess = true;
        }, actionFail: _ => { invokedFail = true; });

        var newFailureResult = failureResult.WhenMatch(actionSuccess: _ => { invokedSuccess = true; },
            actionFail: result =>
            {
                result.IsSuccess.Should().BeFalse();
                result.Value.Should().Be("error");
                invokedFail = true;
            });

        invokedSuccess.Should().BeTrue(); // Should invoke actionSuccess on success
        invokedFail.Should().BeTrue(); // Should invoke actionFail on failure
        newSuccessResult.Should().Be(successResult); // Validate the returned result is the same instance
        newFailureResult.Should().Be(failureResult); // Validate the returned result is the same instance
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be("newSuccess");
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

        invoked.Should().BeFalse();
        newResult.Should().Be(failureResult);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be("newSuccess");
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

        invoked.Should().BeFalse();
        newResult.Should().Be(successResult);
    }

    [Fact]
    public void OnAny_WithFunc_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var failureResult = Result<string>.Fail("error");
        var invoked = false;

        var newSuccessResult = successResult.OnAny((value) =>
        {
            value.Value.Should().Be("success");
            invoked = true;
            return Result<string>.Success("newSuccess");
        });

        var newFailureResult = failureResult.OnAny(() =>
        {
            invoked = true;
            return Result<string>.Success("newSuccess");
        });

        invoked.Should().BeTrue();
        newSuccessResult.IsSuccess.Should().BeTrue();
        newSuccessResult.Value.Should().Be("newSuccess");
        newFailureResult.IsSuccess.Should().BeTrue();
        newFailureResult.Value.Should().Be("newSuccess");
    }

    [Fact]
    public void OnSuccess_WithFuncWithValue_ShouldInvokeFuncWithValueAndReturnResultOnSuccess()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;

        var newResult = successResult.OnSuccess(Fn);

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be("success_new");
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

        invoked.Should().BeFalse();
        newResult.Should().Be(failureResult);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be("error_new");
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

        invoked.Should().BeFalse();
        newResult.Should().Be(successResult);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be("success_new");
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

        invoked.Should().BeFalse();
        newResult.Should().Be(failureResult);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be("error_new");
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

        invoked.Should().BeFalse();
        newResult.Should().Be(successResult);
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

        invoked.Should().BeTrue();
        newSuccessResult.IsSuccess.Should().BeTrue();
        newSuccessResult.Value.Should().Be("success_new");
        newFailureResult.IsSuccess.Should().BeTrue();
        newFailureResult.Value.Should().Be("error_new");
    }

    #endregion

    #region Async Tests Methods

    [Fact]
    public async Task WhenSuccessAsync_WithAction_ShouldInvokeActionOnSuccessAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;

        var newResult = await successResult.WhenSuccessAsync(() => { invoked = true; });

        invoked.Should().BeTrue();
        newResult.Should().Be(successResult); // Validate the returned result is the same instance
    }

    [Fact]
    public async Task WhenSuccessAsync_WithAction_ShouldNotInvokeActionOnFailureAndReturnSameResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;

        var newResult = await failureResult.WhenSuccessAsync(() => { invoked = true; });

        invoked.Should().BeFalse();
        newResult.Should().Be(failureResult); // Validate the returned result is the same instance
    }

    [Fact]
    public async Task WhenSuccessAsyncTask_WithAction_ShouldInvokeActionOnSuccessAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;

        var newResultTask = Task.FromResult(successResult).WhenSuccessAsync(() => { invoked = true; });

        var newResult = await newResultTask;

        invoked.Should().BeTrue();
        newResult.Should().Be(successResult); // Validate the returned result is the same instance
    }

    [Fact]
    public async Task WhenSuccessAsyncTask_WithAction_ShouldNotInvokeActionOnFailureAndReturnSameResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;

        var newResultTask = Task.FromResult(failureResult).WhenSuccessAsync(() => { invoked = true; });

        var newResult = await newResultTask;

        invoked.Should().BeFalse();
        newResult.Should().Be(failureResult); // Validate the returned result is the same instance
    }

    [Fact]
    public async Task WhenFailAsync_WithAction_ShouldInvokeActionOnFailureAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;

        var newResult = await failureResult.WhenFailAsync(() => { invoked = true; });

        invoked.Should().BeTrue();
        newResult.Should().Be(failureResult); // Validate the returned result is the same instance
    }

    [Fact]
    public async Task WhenFailAsync_WithAction_ShouldNotInvokeActionOnSuccessAndReturnSameResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;

        var newResult = await successResult.WhenFailAsync(() => { invoked = true; });

        invoked.Should().BeFalse();
        newResult.Should().Be(successResult); // Validate the returned result is the same instance
    }

    [Fact]
    public async Task WhenFailAsyncTask_WithAction_ShouldInvokeActionOnFailureAndReturnResult()
    {
        var failureResult = Result<string>.Fail("error");
        var invoked = false;

        var newResultTask = Task.FromResult(failureResult).WhenFailAsync(() => { invoked = true; });

        var newResult = await newResultTask;

        invoked.Should().BeTrue();
        newResult.Should().Be(failureResult); // Validate the returned result is the same instance
    }

    [Fact]
    public async Task WhenFailAsyncTask_WithAction_ShouldNotInvokeActionOnSuccessAndReturnSameResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;

        var newResultTask = Task.FromResult(successResult).WhenFailAsync(() => { invoked = true; });

        var newResult = await newResultTask;

        invoked.Should().BeFalse();
        newResult.Should().Be(successResult); // Validate the returned result is the same instance
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

        invokedSuccess.Should().BeTrue();
        invokedFail.Should().BeTrue();
        newSuccessResult.Should().Be(successResult);
        newFailureResult.Should().Be(failureResult);
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

        invokedSuccess.Should().BeTrue();
        invokedFail.Should().BeTrue();
        newSuccessResult.Should().Be(successResult);
        newFailureResult.Should().Be(failureResult);
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

        invoked.Should().BeTrue();
        value.Should().Be("success");
        newResult.Should().Be(successResult); // Validate the returned result is the same instance
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

        invoked.Should().BeFalse();
        value.Should().BeEmpty();
        newResult.Should().Be(failureResult); // Validate the returned result is the same instance
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

        invoked.Should().BeTrue();
        value.Should().Be("success");
        newResult.Should().Be(successResult); // Validate the returned result is the same instance
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

        invoked.Should().BeFalse();
        value.Should().BeEmpty();
        newResult.Should().Be(failureResult); // Validate the returned result is the same instance
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

        invoked.Should().BeTrue();
        value.Should().Be("error");
        newResult.Should().Be(failureResult); // Validate the returned result is the same instance
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

        invoked.Should().BeFalse();
        value.Should().BeEmpty();
        newResult.Should().Be(successResult); // Validate the returned result is the same instance
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

        invoked.Should().BeTrue();
        value.Should().Be("error");
        newResult.Should().Be(failureResult); // Validate the returned result is the same instance
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

        invoked.Should().BeFalse();
        value.Should().BeEmpty();
        newResult.Should().Be(successResult); // Validate the returned result is the same instance
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

        invokedSuccess.Should().BeTrue();
        invokedFail.Should().BeTrue();
        valueSuccess.Should().Be(successResult.Value);
        valueFail.Should().Be(failureResult.Value);
        newSuccessResult.Should().Be(successResult);
        newFailureResult.Should().Be(failureResult);
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

        invokedSuccess.Should().BeTrue();
        invokedFail.Should().BeTrue();
        valueSuccess.Should().Be(successResult.Value);
        valueFail.Should().Be(failureResult.Value);
        newSuccessResult.Should().Be(successResult);
        newFailureResult.Should().Be(failureResult);
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

        invoked.Should().BeTrue();
        invokedResult.Should().Be(successResult);
        newResult.Should().Be(successResult);
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

        invoked.Should().BeFalse();
        invokedResult.Should().BeNull();
        newResult.Should().Be(failureResult);
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

        invoked.Should().BeTrue();
        invokedResult.Should().Be(successResult);
        newResult.Should().Be(successResult);
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

        invoked.Should().BeFalse();
        invokedResult.Should().BeNull();
        newResult.Should().Be(failureResult);
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

        invoked.Should().BeTrue();
        invokedResult.Should().Be(failureResult);
        newResult.Should().Be(failureResult);
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

        invoked.Should().BeFalse();
        invokedResult.Should().BeNull();
        newResult.Should().Be(successResult);
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

        invoked.Should().BeTrue();
        invokedResult.Should().Be(failureResult);
        newResult.Should().Be(failureResult);
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

        invoked.Should().BeFalse();
        invokedResult.Should().BeNull();
        newResult.Should().Be(successResult);
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

        invokedSuccess.Should().BeTrue();
        invokedFail.Should().BeTrue();
        invokedResultSuccess.Should().Be(successResult);
        invokedResultFail.Should().Be(failureResult);
        newSuccessResult.Should().Be(successResult);
        newFailureResult.Should().Be(failureResult);
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

        invokedSuccess.Should().BeTrue();
        invokedFail.Should().BeTrue();
        invokedResultSuccess.Should().Be(successResult);
        invokedResultFail.Should().Be(failureResult);
        newSuccessResult.Should().Be(successResult);
        newFailureResult.Should().Be(failureResult);
    }

    [Fact]
    public async Task OnSuccessAsync_WithFunc_ShouldInvokeFuncAndReturnResult()
    {
        var successResult = Result<string>.Success("success");
        var invoked = false;
        const string newResultValue = "new success value";

        var newResult = await successResult.OnSuccessAsync(OnSuccessFunc);

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeFalse();
        newResult.IsSuccess.Should().BeFalse();
        newResult.Value.Should().Be(failureResult.Value);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeFalse();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeFalse();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(successResult.Value);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeFalse();
        newResult.IsSuccess.Should().BeFalse();
        newResult.Value.Should().Be(failureResult.Value);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeFalse();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeFalse();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(successResult.Value);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeFalse();
        newResult.IsSuccess.Should().BeFalse();
        newResult.Value.Should().Be(failureResult.Value);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeFalse();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeFalse();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(successResult.Value);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeFalse();
        newResult.IsSuccess.Should().BeFalse();
        newResult.Value.Should().Be(failureResult.Value);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeFalse();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeFalse();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(successResult.Value);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeFalse();
        newResult.IsSuccess.Should().BeFalse();
        newResult.Value.Should().Be((await failureResult).Value);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeFalse();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeFalse();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be("success");
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeFalse();
        newResult.IsSuccess.Should().BeFalse();
        newResult.Value.Should().Be("error");
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(newResultValue);
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

        invoked.Should().BeFalse();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be("success");
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

        invoked.Should().BeTrue();
        newResult.IsSuccess.Should().BeTrue();
        newResult.Value.Should().Be(newResultValue);
        return;

        Task<Result<string>> OnAnyFunc(Result<string> result)
        {
            invoked = true;
            return Task.FromResult(Result<string>.Success(newResultValue));
        }
    }

    #endregion
}