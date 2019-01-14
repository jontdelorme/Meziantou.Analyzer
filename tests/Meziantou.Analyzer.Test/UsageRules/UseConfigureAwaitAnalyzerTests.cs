﻿using System;
using Meziantou.Analyzer.UsageRules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Meziantou.Analyzer.Test.UsageRules
{
    [TestClass]
    public class UseConfigureAwaitAnalyzerTests : CodeFixVerifier
    {
        private static readonly string UsingTasksContext = "using System.Threading.Tasks;" + Environment.NewLine;
        private static readonly string WpfWindowContext = "namespace System.Windows { namespace Input { interface ICommand {} } namespace Threading { class DispatcherObject {} } class Window : Threading.DispatcherObject {} }" + Environment.NewLine;

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new UseConfigureAwaitAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new UseConfigureAwaitFixer();

        [TestMethod]
        public void EmptyString_ShouldNotReportDiagnosticForEmptyString()
        {
            var test = "";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void MissingConfigureAwait_ShouldReportError()
        {
            var test = @"using System.Threading.Tasks;
async Task Test()
{
    await Task.Delay(1);
}";

            var expected = new DiagnosticResult
            {
                Id = "MA0004",
                Message = "Use ConfigureAwait(false) as the current SynchronizationContext is not needed",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", line: 4, column: 5)
                }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System.Threading.Tasks;
async Task Test()
{
    await Task.Delay(1).ConfigureAwait(false);
}";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void ConfigureAwaitIsPresent_ShouldNotReportError()
        {
            var test = @"using System.Threading.Tasks;
async Task Test()
{
    await Task.Delay(1).ConfigureAwait(true);
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void ConfigureAwaitOfTIsPresent_ShouldNotReportError()
        {
            var test = @"using System.Threading.Tasks;
async Task Test()
{
    await Task.Run(() => 10).ConfigureAwait(true);
}";
            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void MissingConfigureAwaitInWpfWindowClass_ShouldNotReportError()
        {
            var test = UsingTasksContext + WpfWindowContext + @"
class MyClass : System.Windows.Window
{
    async Task Test()
    {
        await Task.Delay(1);
    }
}";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void MissingConfigureAwaitInWpfCommandClass_ShouldNotReportError()
        {
            var test = UsingTasksContext + WpfWindowContext + @"
class MyClass : System.Windows.Input.ICommand
{
    async Task Test()
    {
        await Task.Delay(1);
    }
}";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void AfterConfigureAwaitFalse_AllAwaitShouldUseConfigureAwait()
        {
            var test = @"using System.Threading.Tasks;
namespace System.Windows
{
    namespace Threading
    {
        class DispatcherObject
        {
        }
    }

    class Window : Threading.DispatcherObject
    {
    }
}

class MyClass : System.Windows.Window
{
    async Task Test()
    {
        await Task.Delay(1);
        await Task.Delay(1).ConfigureAwait(false);
        await Task.Delay(1);
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "MA0004",
                Message = "Use ConfigureAwait(false) as the current SynchronizationContext is not needed",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", line: 22, column: 9)
                }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System.Threading.Tasks;
namespace System.Windows
{
    namespace Threading
    {
        class DispatcherObject
        {
        }
    }

    class Window : Threading.DispatcherObject
    {
    }
}

class MyClass : System.Windows.Window
{
    async Task Test()
    {
        await Task.Delay(1);
        await Task.Delay(1).ConfigureAwait(false);
        await Task.Delay(1).ConfigureAwait(false);
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void AfterConfigureAwaitFalseInANonAccessibleBranch_ShouldNotReportDiagnostic()
        {
            var test = UsingTasksContext + WpfWindowContext + @"
class MyClass : System.Windows.Window
{
    async Task Test()
    {
        bool a = true;
        if (a)
        {
            await Task.Delay(1).ConfigureAwait(false);
            return;
        }

        await Task.Delay(1);
    }
}";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void AfterConfigureAwaitFalseInNonAccessibleBranch2_ShouldReportDiagnostic()
        {
            var test = UsingTasksContext + WpfWindowContext + @"
class MyClass : System.Windows.Window
{
    async Task Test()
    {
        bool a = true;
        if (a)
        {
            await Task.Delay(1).ConfigureAwait(false);
        }
        else
        {
            await Task.Delay(1);
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "MA0004",
                Message = "Use ConfigureAwait(false) as the current SynchronizationContext is not needed",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                {
                    new DiagnosticResultLocation("Test0.cs", line: 15, column: 13)
                }
            };

            VerifyCSharpDiagnostic(test, expected);
        }
    }
}
