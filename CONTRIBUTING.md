# Contributing to Roncav Budget

First off, thank you for considering contributing to Roncav Budget! It's people like you that make Roncav Budget such a great tool.

[🇧🇷 Versão em Português](CONTRIBUTING.pt-BR.md)

## Code of Conduct

This project and everyone participating in it is governed by our [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code. Please report unacceptable behavior to contato@avila.inc.

## How Can I Contribute?

### Reporting Bugs

This section guides you through submitting a bug report for Roncav Budget. Following these guidelines helps maintainers and the community understand your report, reproduce the behavior, and find related reports.

**Before Submitting A Bug Report:**
- Check the [documentation](docs/) for solutions
- Check the [FAQ](docs/RESUMO_EXECUTIVO.md) 
- Search [existing issues](https://github.com/avilaops/roncav-budget/issues) to see if the problem has already been reported

**How Do I Submit A Good Bug Report?**

Bugs are tracked as [GitHub issues](https://github.com/avilaops/roncav-budget/issues). Create an issue and provide the following information:

- **Use a clear and descriptive title**
- **Describe the exact steps to reproduce the problem**
- **Provide specific examples** to demonstrate the steps
- **Describe the behavior you observed** and point out what exactly is the problem
- **Explain which behavior you expected to see instead** and why
- **Include screenshots or GIFs** if possible
- **Include your environment details:**
  - OS version (Windows 11, Android 13, iOS 17, etc.)
  - .NET version
  - App version

### Suggesting Enhancements

This section guides you through submitting an enhancement suggestion for Roncav Budget.

**Before Submitting An Enhancement Suggestion:**
- Check if the enhancement has already been suggested in [issues](https://github.com/avilaops/roncav-budget/issues)
- Check the [roadmap](README.md#roadmap) to see if it's already planned

**How Do I Submit A Good Enhancement Suggestion?**

Enhancement suggestions are tracked as [GitHub issues](https://github.com/avilaops/roncav-budget/issues). Create an issue and provide the following information:

- **Use a clear and descriptive title**
- **Provide a step-by-step description** of the suggested enhancement
- **Provide specific examples** to demonstrate the steps
- **Describe the current behavior** and explain which behavior you expected to see instead
- **Explain why this enhancement would be useful**
- **Include mockups or sketches** if possible

### Pull Requests

The process described here has several goals:
- Maintain Roncav Budget's quality
- Fix problems that are important to users
- Engage the community in working toward the best possible Roncav Budget
- Enable a sustainable system for maintainers to review contributions

**Before Starting Work:**
1. Check if there's an open issue for what you want to work on
2. If not, create an issue first to discuss your proposed changes
3. Wait for feedback from maintainers before starting work

**Pull Request Process:**

1. **Fork the repo** and create your branch from `main`
   ```bash
   git checkout -b feature/amazing-feature
   ```

2. **Set up your development environment**
   - Install [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
   - Install [Visual Studio 2022](https://visualstudio.microsoft.com/) with .NET MAUI workload
   - Run `dotnet restore` and `dotnet build`

3. **Make your changes**
   - Follow the existing code style
   - Add or update tests as needed
   - Update documentation if needed
   - Keep your changes focused - one feature/fix per PR

4. **Test your changes**
   - Build the solution successfully
   - Test on at least one platform (Windows/Android/iOS/macOS)
   - Ensure existing functionality isn't broken

5. **Commit your changes**
   - Use clear and descriptive commit messages
   - Reference issue numbers in commit messages (e.g., "Fix #123: Description")
   - Follow [Conventional Commits](https://www.conventionalcommits.org/) format:
     ```
     feat: add PIX transaction filtering
     fix: correct balance calculation for transfers
     docs: update installation instructions
     style: format code according to style guide
     refactor: reorganize service layer
     test: add unit tests for transaction service
     chore: update dependencies
     ```

6. **Push to your fork** and submit a pull request
   ```bash
   git push origin feature/amazing-feature
   ```

7. **Create a Pull Request**
   - Use a clear and descriptive title
   - Describe your changes in detail
   - Reference related issues (e.g., "Closes #123")
   - Include screenshots for UI changes
   - List any breaking changes

8. **Wait for review**
   - Maintainers will review your PR
   - Address any feedback or requested changes
   - Once approved, your PR will be merged

## Coding Standards

### C# Style Guide

- Follow [Microsoft's C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use PascalCase for class names and method names
- Use camelCase for local variables and parameters
- Use meaningful and descriptive names
- Keep methods small and focused
- Add XML documentation comments for public APIs

### XAML Style Guide

- Use consistent indentation (4 spaces)
- Keep XAML files readable and well-organized
- Use data binding instead of code-behind when possible
- Follow MVVM pattern strictly

### Project Structure

- Place models in `Models/` folder
- Place services in `Services/` folder
- Place view models in `ViewModels/` folder
- Place views in `Views/` folder
- Keep platform-specific code in platform projects

### Git Commit Messages

- Use present tense ("Add feature" not "Added feature")
- Use imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit first line to 72 characters
- Reference issues and pull requests in commit body

## Development Setup

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022 17.8+](https://visualstudio.microsoft.com/) with:
  - .NET MAUI workload
  - Mobile development with .NET workload (for Android/iOS)
- [Git](https://git-scm.com/)

### Setup Steps

1. Clone your fork:
   ```bash
   git clone https://github.com/YOUR-USERNAME/roncav-budget.git
   cd roncav-budget
   ```

2. Add upstream remote:
   ```bash
   git remote add upstream https://github.com/avilaops/roncav-budget.git
   ```

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Build the solution:
   ```bash
   dotnet build
   ```

5. Run the app (Windows):
   ```bash
   dotnet run --project Roncav_Budget/Roncav_Budget.csproj -f net9.0-windows10.0.19041.0
   ```

### Running Tests

```bash
dotnet test
```

### Building for Different Platforms

**Windows:**
```bash
dotnet build Roncav_Budget/Roncav_Budget.csproj -f net9.0-windows10.0.19041.0
```

**Android:**
```bash
dotnet build Roncav_Budget/Roncav_Budget.csproj -f net9.0-android
```

**iOS (requires Mac):**
```bash
dotnet build Roncav_Budget/Roncav_Budget.csproj -f net9.0-ios
```

**macOS:**
```bash
dotnet build Roncav_Budget/Roncav_Budget.csproj -f net9.0-maccatalyst
```

## Additional Resources

- [.NET MAUI Documentation](https://docs.microsoft.com/dotnet/maui/)
- [CommunityToolkit.MVVM Documentation](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [SQLite Documentation](https://www.sqlite.org/docs.html)

## Questions?

Feel free to:
- Open a [GitHub Discussion](https://github.com/avilaops/roncav-budget/discussions)
- Email us at contato@avila.inc

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing to Roncav Budget! 🎉
