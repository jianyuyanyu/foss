// Copyright (c) Duende Software. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Diagnostics;
using System.Globalization;
using System.Text.Encodings.Web;

namespace Duende.RazorSlices.Tests;

// The upstream HtmlEncodeAndWriteSpanFormattable tests all use values shorter than
// BufferSizes.SmallFormattableWriteCharSize, so they take the stackalloc fast path and never rent from
// ArrayPool<char>. These tests use values longer than that threshold to exercise the pooled path,
// which must encode only the formatted chars rather than the whole rented buffer.
public class TextWriterHtmlExtensionsPooledPathTests
{
    [Fact]
    public void HtmlEncodeAndWriteSpanFormattable_PooledPath_WritesOnlyTheFormattedChars()
    {
        var value = new FixedStringFormattable(new string('a', BufferSizes.SmallFormattableWriteCharSize + 8));
        using var writer = new StringWriter();

        writer.HtmlEncodeAndWriteSpanFormattable(value, HtmlEncoder.Default);

        writer.ToString().ShouldBe(value.Value);
    }

    [Fact]
    public void HtmlEncodeAndWriteSpanFormattable_PooledPath_DoesNotLeakPreviousRenterContent()
    {
        // Dirty the shared pool with a long value whose tail is distinctive, then write a shorter value that
        // still exceeds the small-path threshold so it rents (and likely receives) the same pooled array.
        var longValue = new FixedStringFormattable(new string('X', 200));
        var shortValue = new FixedStringFormattable(new string('y', BufferSizes.SmallFormattableWriteCharSize + 8));

        using var warmup = new StringWriter();
        warmup.HtmlEncodeAndWriteSpanFormattable(longValue, HtmlEncoder.Default);

        using var writer = new StringWriter();
        writer.HtmlEncodeAndWriteSpanFormattable(shortValue, HtmlEncoder.Default);

        writer.ToString().ShouldBe(shortValue.Value);
        writer.ToString().ShouldNotContain("X");
    }

    [Fact]
    public void HtmlEncodeAndWriteSpanFormattable_PooledPath_EncodesHtml()
    {
        var value = new FixedStringFormattable("<" + new string('b', BufferSizes.SmallFormattableWriteCharSize + 8) + ">");
        using var writer = new StringWriter();

        writer.HtmlEncodeAndWriteSpanFormattable(value, HtmlEncoder.Default);

        writer.ToString().ShouldBe("&lt;" + new string('b', BufferSizes.SmallFormattableWriteCharSize + 8) + "&gt;");
    }

    [Fact]
    public async Task HtmlEncodeAndWriteSpanFormattable_PooledPath_EncodesNonAscii()
    {
        const string childProcessVariable = "DUENDE_RAZOR_SLICES_NON_ASCII_ENCODING_CHILD";

        if (Environment.GetEnvironmentVariable(childProcessVariable) == "1")
        {
            var value = new FixedStringFormattable(new string('\u00E9', 100));
            using var writer = new StringWriter();

            writer.HtmlEncodeAndWriteSpanFormattable(value, HtmlEncoder.Default);

            writer.ToString().ShouldBe(string.Concat(Enumerable.Repeat("&#xE9;", 100)));
            return;
        }

        // A task timeout cannot stop a synchronous encoding loop. Isolate it in a process we can terminate.
        var resultsDirectory = Directory.CreateTempSubdirectory("RazorSlices-encoding-");
        try
        {
            var startInfo = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            startInfo.ArgumentList.Add(typeof(TextWriterHtmlExtensionsPooledPathTests).Assembly.Location);
            startInfo.ArgumentList.Add("--filter-method");
            startInfo.ArgumentList.Add($"{typeof(TextWriterHtmlExtensionsPooledPathTests).FullName}.{nameof(HtmlEncodeAndWriteSpanFormattable_PooledPath_EncodesNonAscii)}");
            startInfo.ArgumentList.Add("--minimum-expected-tests");
            startInfo.ArgumentList.Add("1");
            startInfo.ArgumentList.Add("--exit-on-process-exit");
            startInfo.ArgumentList.Add(Environment.ProcessId.ToString(CultureInfo.InvariantCulture));
            startInfo.ArgumentList.Add("--results-directory");
            startInfo.ArgumentList.Add(resultsDirectory.FullName);
            startInfo.ArgumentList.Add("--no-progress");
            startInfo.ArgumentList.Add("--no-ansi");
            startInfo.Environment[childProcessVariable] = "1";

            using var process = new Process { StartInfo = startInfo };
            process.Start().ShouldBeTrue();
            var outputTask = process.StandardOutput.ReadToEndAsync(CancellationToken.None);
            var errorTask = process.StandardError.ReadToEndAsync(CancellationToken.None);
            var timedOut = false;
            string output;
            string error;

            try
            {
                await process.WaitForExitAsync(TestContext.Current.CancellationToken)
                    .WaitAsync(TimeSpan.FromSeconds(10), TestContext.Current.CancellationToken);
            }
            catch (TimeoutException)
            {
                timedOut = true;
            }
            finally
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                    await process.WaitForExitAsync(CancellationToken.None);
                }

                output = await outputTask;
                error = await errorTask;
            }

            timedOut.ShouldBeFalse($"Non-ASCII encoding did not complete within 10 seconds. The child process was terminated.\n{output}\n{error}");
            process.ExitCode.ShouldBe(0, $"The encoding regression test failed in the child process.\n{output}\n{error}");
        }
        finally
        {
            resultsDirectory.Delete(recursive: true);
        }
    }

    private readonly struct FixedStringFormattable(string value) : ISpanFormattable
    {
        public string Value { get; } = value;

        public string ToString(string? format, IFormatProvider? formatProvider) => Value;

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            if (destination.Length < Value.Length)
            {
                charsWritten = 0;
                return false;
            }

            Value.AsSpan().CopyTo(destination);
            charsWritten = Value.Length;
            return true;
        }
    }
}
