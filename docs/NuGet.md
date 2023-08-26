# 📦 Functional.ResultType 🚀

A lightweight Result type for functional programming in C#

## About

Functional.ResultType is a C# library that provides a simple and lightweight Result type for handling functional programming concepts. 
It's designed to help you manage success and failure cases more elegantly, promoting better error handling and control flow in your applications.

## Features

- ✅ Simple and intuitive API for handling success and failure cases.
- ⚡️ Lightweight and dependency-free, making it easy to integrate into your project.
- 🧰 Helpful utilities for mapping, transforming, and composing results.
- 🛠️ Designed to enhance code readability and maintainability.

## Getting Started

To get started with Functional.ResultType, follow these steps:

1. **Installation**: You can install the package via NuGet Package Manager:
   ```shell
   Install-Package Functional.ResultType
   
2. Usage: Import the namespace in your C# file and start using the Result type:

```csharp
public class FakeObject
{
    public string Name { get; init; } = string.Empty;
}

var fakeObject FakeObject = new() { Name = "fake" };

var failureResult = Result<FakeObject>.Fail(fakeObject);
var successResult = Result<FakeObject>.Success(fakeObject);

//Chech the ResultExtensions.cs file for many more extensions methods.
```