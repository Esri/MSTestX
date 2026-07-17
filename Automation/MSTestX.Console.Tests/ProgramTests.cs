using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;

namespace MSTestX.Console.Tests;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void GetLaunchMode_UsesRemoteAdapter_WhenRemoteIpIsProvided()
    {
        var arguments = new Dictionary<string, string?>
        {
            ["remoteIp"] = "127.0.0.1:38300"
        };

        Assert.AreEqual(LaunchMode.RemoteAdapter, Program.GetLaunchMode(arguments));
    }

    [TestMethod]
    public void GetLaunchMode_UsesRemoteAdapter_WhenWaitForRemoteIsProvided()
    {
        var arguments = new Dictionary<string, string?>
        {
            ["waitForRemote"] = null
        };

        Assert.AreEqual(LaunchMode.RemoteAdapter, Program.GetLaunchMode(arguments));
    }

    [TestMethod]
    public void GetLaunchMode_PrefersRemoteAdapter_OverAppPath()
    {
        var arguments = new Dictionary<string, string?>
        {
            ["remoteIp"] = "127.0.0.1:38300",
            ["apppath"] = "/tmp/Test.app"
        };

        Assert.AreEqual(LaunchMode.RemoteAdapter, Program.GetLaunchMode(arguments));
    }

    [TestMethod]
    public void GetLaunchMode_UsesAppleApp_WhenAppPathIsProvidedWithoutRemoteArguments()
    {
        var arguments = new Dictionary<string, string?>
        {
            ["apppath"] = "/tmp/Test.app"
        };

        Assert.AreEqual(LaunchMode.AppleApp, Program.GetLaunchMode(arguments));
    }

    [TestMethod]
    public void GetLaunchMode_DefaultsToAndroidAdb_WhenNoRemoteOrAppleArgumentsAreProvided()
    {
        var arguments = new Dictionary<string, string?>();

        Assert.AreEqual(LaunchMode.AndroidAdb, Program.GetLaunchMode(arguments));
    }

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void GetLaunchMode_DevicectlTimeout_DoesNotAffectModeSelection(bool useRemoteAdapter)
    {
        var arguments = Program.ParseArguments(["--devicectlTimeout", "invalid"]);
        if (useRemoteAdapter)
            arguments["remoteIp"] = "127.0.0.1:38300";
        LaunchMode expectedMode = useRemoteAdapter ? LaunchMode.RemoteAdapter : LaunchMode.AndroidAdb;

        Assert.AreEqual(expectedMode, Program.GetLaunchMode(arguments));
    }

    [DataTestMethod]
    [DataRow("-devicectlTimeout", "120")]
    [DataRow("--devicectlTimeout", "240")]
    public void ParseArguments_ParsesDevicectlTimeout(string option, string value)
    {
        var arguments = Program.ParseArguments([option, value]);

        Assert.AreEqual(value, arguments["devicectlTimeout"]);
    }

    [DataTestMethod]
    [DataRow(null, 3600)]
    [DataRow("7200", 7200)]
    public void TryParseDevicectlTimeout_UsesDefaultOrCustomPositiveInt32(string? value, int expectedTimeoutSeconds)
    {
        var arguments = new Dictionary<string, string?>();
        if (value is not null)
            arguments["devicectlTimeout"] = value;

        bool result = Program.TryParseDevicectlTimeout(arguments, out int timeoutSeconds, out string? errorMessage);

        Assert.IsTrue(result);
        Assert.AreEqual(expectedTimeoutSeconds, timeoutSeconds);
        Assert.IsNull(errorMessage);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("-1")]
    public void TryParseDevicectlTimeout_RejectsMissingOrNegativeCliValue(string? value)
    {
        var cliArguments = new List<string> { "-apppath", "/tmp/Test.app", "-devicectlTimeout" };
        if (value is not null)
            cliArguments.Add(value);
        var arguments = Program.ParseArguments(cliArguments.ToArray());

        bool result = Program.TryParseDevicectlTimeout(arguments, out _, out string? errorMessage);

        Assert.IsFalse(result);
        StringAssert.Contains(errorMessage, "-devicectlTimeout");
        StringAssert.Contains(errorMessage, "requires");
    }

    [DataTestMethod]
    [DataRow("not-a-number")]
    [DataRow("0")]
    [DataRow("-1")]
    [DataRow("2147483648")]
    public void TryParseDevicectlTimeout_RejectsInvalidValue(string value)
    {
        var arguments = new Dictionary<string, string?>
        {
            ["devicectlTimeout"] = value
        };

        bool result = Program.TryParseDevicectlTimeout(arguments, out _, out string? errorMessage);

        Assert.IsFalse(result);
        StringAssert.Contains(errorMessage, "-devicectlTimeout");
        StringAssert.Contains(errorMessage, value);
    }

    [TestMethod]
    public void BuildLaunchArguments_UsesDefaultTimeoutBeforeBundleAndPreservesAppArgumentOrder()
    {
        IReadOnlyList<string> arguments = devicectl.BuildLaunchArguments(
            "device with spaces",
            "com.example tests",
            Program.DefaultDevicectlTimeoutSeconds,
            ["--TestAdapterPort", "38300", "--AutoExit", "True"]);

        CollectionAssert.AreEqual(
            new[]
            {
                "device", "process", "launch", "--device", "device with spaces", "--terminate-existing", "--console",
                "--timeout", "3600", "com.example tests", "--TestAdapterPort", "38300", "--AutoExit", "True"
            },
            arguments.ToArray());
        Assert.AreEqual(1, arguments.Count(argument => argument == "--timeout"));
    }

    [TestMethod]
    public void BuildLaunchArguments_UsesCustomTimeoutExactlyOnce()
    {
        IReadOnlyList<string> arguments = devicectl.BuildLaunchArguments(
            "device-2",
            "com.example.custom",
            90,
            ["--first", "one", "--second", "two"]);

        CollectionAssert.AreEqual(
            new[]
            {
                "device", "process", "launch", "--device", "device-2", "--terminate-existing", "--console",
                "--timeout", "90", "com.example.custom", "--first", "one", "--second", "two"
            },
            arguments.ToArray());
        Assert.AreEqual(1, arguments.Count(argument => argument == "--timeout"));
    }

    [TestMethod]
    public void FormatDuration_FormatsSubMillisecondDuration()
    {
        Assert.AreEqual("[< 1ms]", TimeSpan.Zero.FormatDuration());
    }

    [TestMethod]
    public void FormatDuration_FormatsSecondScaleDuration()
    {
        Assert.AreEqual("[1s 250ms]", TimeSpan.FromMilliseconds(1250).FormatDuration());
    }

    [TestMethod]
    public void GetRedirectedOutcomeLabel_UsesStableLabels()
    {
        Assert.AreEqual("PASS", TestRunner.GetRedirectedOutcomeLabel(TestOutcome.Passed));
        Assert.AreEqual("FAIL", TestRunner.GetRedirectedOutcomeLabel(TestOutcome.Failed));
        Assert.AreEqual("SKIP", TestRunner.GetRedirectedOutcomeLabel(TestOutcome.Skipped));
        Assert.AreEqual("NONE", TestRunner.GetRedirectedOutcomeLabel(TestOutcome.None));
        Assert.AreEqual("NOTFOUND", TestRunner.GetRedirectedOutcomeLabel(TestOutcome.NotFound));
    }

    [TestMethod]
    public void GetAppLogFilename_UsesSiblingLogFile()
    {
        Assert.AreEqual("/tmp/results/run.log", Program.GetAppLogFilename("/tmp/results/run.trx"));
    }

    [TestMethod]
    public void GetAppLogFilename_ReturnsNull_WhenTrxPathIsMissing()
    {
        Assert.IsNull(Program.GetAppLogFilename(null));
        Assert.IsNull(Program.GetAppLogFilename(""));
    }

    [TestMethod]
    public void MobileDevice_IsUnexpectedTunnelExit_ReturnsFalse_ForIntentionalShutdown()
    {
        Assert.IsFalse(MobileDevice.IsUnexpectedTunnelExit(isIntentionalShutdown: true));
        Assert.IsTrue(MobileDevice.IsUnexpectedTunnelExit(isIntentionalShutdown: false));
    }

    [TestMethod]
    public void MobileDevice_FormatTunnelExitMessage_LabelsIntentionalShutdown()
    {
        var message = MobileDevice.FormatTunnelExitMessage(
            exitCode: 0,
            lastMessage: "closed by test runner",
            isIntentionalShutdown: true);

        StringAssert.Contains(message, "stopped intentionally");
        StringAssert.Contains(message, "closed by test runner");
    }

    [TestMethod]
    public void MobileDevice_FormatTunnelExitMessage_LabelsUnexpectedExit()
    {
        var message = MobileDevice.FormatTunnelExitMessage(
            exitCode: 1,
            lastMessage: "lost connection",
            isIntentionalShutdown: false);

        StringAssert.Contains(message, "exited unexpectedly");
        StringAssert.Contains(message, "lost connection");
    }
}
