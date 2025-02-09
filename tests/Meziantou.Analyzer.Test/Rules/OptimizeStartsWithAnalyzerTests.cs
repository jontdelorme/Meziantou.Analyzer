﻿using System.Threading.Tasks;
using Meziantou.Analyzer.Rules;
using TestHelper;
using Xunit;

namespace Meziantou.Analyzer.Test.Rules;

public sealed class OptimizeStartsWithAnalyzerTests
{
    private static ProjectBuilder CreateProjectBuilder()
    {
        return new ProjectBuilder()
            .WithAnalyzer<OptimizeStartsWithAnalyzer>()
            .WithTargetFramework(TargetFramework.Net7_0);
    }

    [Theory]
    [InlineData("null")]
    [InlineData(@"""""")]
    [InlineData(@"str")]
    [InlineData(@"""abc""")]
    [InlineData(@"""abc"", ignoreCase: true, null")]
    [InlineData(@"""a"", StringComparison.OrdinalIgnoreCase")]
    [InlineData(@"""a"", StringComparison.CurrentCultureIgnoreCase")]
    [InlineData(@"""a"", StringComparison.InvariantCultureIgnoreCase")]
    [InlineData(@"""a""")]
    [InlineData(@"[|""a""|], StringComparison.Ordinal")]
    [InlineData(@"""a"", StringComparison.CurrentCulture")]
    [InlineData(@"""a"", StringComparison.InvariantCulture")]
    public async Task StartsWith(string method)
    {
        var sourceCode = @"
using System;
class Test
{
    void A(string str)
    {
        _ = str.StartsWith(" + method + @");
    }
}";
        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .ValidateAsync();
    }

    [Theory]
    [InlineData("null")]
    [InlineData(@"""""")]
    [InlineData(@"str")]
    [InlineData(@"""abc""")]
    [InlineData(@"""abc"", ignoreCase: true, null")]
    [InlineData(@"""a"", StringComparison.OrdinalIgnoreCase")]
    [InlineData(@"""a"", StringComparison.CurrentCultureIgnoreCase")]
    [InlineData(@"""a"", StringComparison.InvariantCultureIgnoreCase")]
    [InlineData(@"""a""")]
    [InlineData(@"[|""a""|], StringComparison.Ordinal")]
    [InlineData(@"""a"", StringComparison.CurrentCulture")]
    [InlineData(@"""a"", StringComparison.InvariantCulture")]
    public async Task EndsWith(string method)
    {
        var sourceCode = @"
using System;
class Test
{
    void A(string str)
    {
        _ = str.EndsWith(" + method + @");
    }
}";
        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .ValidateAsync();
    }

    [Theory]
    [InlineData(@"""a"", StringComparison.Ordinal")]
    [InlineData(@"""a"", StringComparison.CurrentCulture")]
    [InlineData(@"""a"", 1, 2, StringComparison.Ordinal")]
    [InlineData(@"""a"", 1, StringComparison.Ordinal")]
    public async Task IndexOf_Report(string method)
    {
        var sourceCode = @"
using System;
class Test
{
    void A(string str)
    {
        _ = [||]str.IndexOf(" + method + @");
    }
}";
        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .ValidateAsync();
    }
    
    [Theory]
    [InlineData("null")]
    [InlineData(@"""""")]
    [InlineData(@"str")]
    [InlineData(@"""abc""")]
    [InlineData(@"""a""")]
    [InlineData(@"""a"", 1")]
    [InlineData(@"""a"", 1, 2")]
    [InlineData(@"""a"", 1, 2, StringComparison.OrdinalIgnoreCase")]
    [InlineData(@"""a"", 1, StringComparison.OrdinalIgnoreCase")]
    public async Task IndexOf_NoReport(string method)
    {
        var sourceCode = @"
using System;
class Test
{
    void A(string str)
    {
        _ = str.IndexOf(" + method + @");
    }
}";
        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .ValidateAsync();
    }
    
    [Theory]
    [InlineData(@"""a"", StringComparison.OrdinalIgnoreCase")]
    public async Task IndexOf_NoReport_Netstandard2_0(string method)
    {
        var sourceCode = @"
using System;
class Test
{
    void A(string str)
    {
        _ = str.IndexOf(" + method + @");
    }
}";
        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .WithTargetFramework(TargetFramework.NetStandard2_0)
              .ValidateAsync();
    }
    
    [Theory]
    [InlineData(@"""a"", StringComparison.Ordinal")]
    [InlineData(@"""a"", 1, 2, StringComparison.Ordinal")]
    [InlineData(@"""a"", 1, StringComparison.Ordinal")]
    public async Task LastIndexOf_Report(string method)
    {
        var sourceCode = @"
using System;
class Test
{
    void A(string str)
    {
        _ = [||]str.LastIndexOf(" + method + @");
    }
}";
        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .ValidateAsync();
    }
    
    [Theory]
    [InlineData("null")]
    [InlineData(@"""""")]
    [InlineData(@"str")]
    [InlineData(@"""abc""")]
    [InlineData(@"""a""")]
    [InlineData(@"""a"", 1")]
    [InlineData(@"""a"", 1, 2")]
    [InlineData(@"""a"", StringComparison.CurrentCulture")]
    [InlineData(@"""a"", 1, 2, StringComparison.OrdinalIgnoreCase")]
    [InlineData(@"""a"", 1, StringComparison.OrdinalIgnoreCase")]
    public async Task LastIndexOf_NoReport(string method)
    {
        var sourceCode = @"
using System;
class Test
{
    void A(string str)
    {
        _ = str.LastIndexOf(" + method + @");
    }
}";
        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .ValidateAsync();
    }
    
    [Theory]
    [InlineData(@"""a"", StringComparison.OrdinalIgnoreCase")]
    public async Task LastIndexOf_NoReport_Netstandard2_0(string method)
    {
        var sourceCode = @"
using System;
class Test
{
    void A(string str)
    {
        _ = str.LastIndexOf(" + method + @");
    }
}";
        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .WithTargetFramework(TargetFramework.NetStandard2_0)
              .ValidateAsync();
    }

    [Theory]
    [InlineData(@"""ab"", """"")]
    [InlineData(@"""ab"", ""c""")]
    [InlineData(@"""a"", ""bc""")]
    [InlineData(@"""a"", ""b"", StringComparison.OrdinalIgnoreCase")]
    [InlineData(@"""a"", ""b"", StringComparison.CurrentCulture")]
    [InlineData(@"""a"", ""b"", false, null")]
    public async Task Replace_NoReport(string method)
    {
        var sourceCode = @"
using System;
class Test
{
    void A(string str)
    {
        _ = str.Replace(" + method + @");
    }
}";
        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .ValidateAsync();
    }

    [Theory]
    [InlineData(@"""a"", ""b""")]
    [InlineData(@"""a"", ""b"", StringComparison.Ordinal")]
    public async Task Replace_Report(string method)
    {
        var sourceCode = @"
using System;
class Test
{
    void A(string str)
    {
        _ = [||]str.Replace(" + method + @");
    }
}";
        await CreateProjectBuilder()
              .WithSourceCode(sourceCode)
              .ValidateAsync();
    }
}
