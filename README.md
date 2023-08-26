# 📦 Functional.ResultType 🚀

[![Build](https://github.com/ricardotondello/Functional.ResultType/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/ricardotondello/Functional.ResultType/actions/workflows/dotnet.yml)
[![Qodana](https://github.com/ricardotondello/Functional.ResultType/actions/workflows/code_quality.yml/badge.svg)](https://qodana.cloud/projects/AN25j/reports/dloDM)
[![Code Coverage](https://img.shields.io/badge/Code_Coverage-Click_Here-green)](https://ricardotondello.github.io/Functional.ResultType/unittests)
[![NuGet latest version](https://badgen.net/nuget/v/Functional.ResultType/latest)](https://nuget.org/packages/Functional.ResultType)
[![NuGet downloads](https://img.shields.io/nuget/dt/Functional.ResultType)](https://www.nuget.org/packages/Functional.ResultType)

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

## Contributing 👥

Contributions are welcome! If you find a bug or have a feature request, please open an issue on GitHub.
If you would like to contribute code, please fork the repository and submit a pull request.

## License 📄

This project is licensed under the MIT License.
See [LICENSE](https://github.com/ricardotondello/Functional.ResultType/blob/main/LICENSE) for more information.

## Support ☕

<a href="https://www.buymeacoffee.com/ricardotondello" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 60px !important;width: 217px !important;" ></a>
