// Copyright 2026 Esri
//
// Licensed under the Apache License Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace MSTestX.Console.Tests;

[TestClass]
public class DeviceCtlTests
{
    [TestMethod]
    public void CreateLaunchProcessStartInfo_PutsFixedArgumentsBeforeCustomArguments()
    {
        var startInfo = devicectl.CreateLaunchProcessStartInfo(
            "My Phone",
            "com.example.my tests",
            new[]
            {
                "--tag",
                "first",
                "--tag",
                "second",
                "--mode",
                "-diagnostic",
                "value with spaces",
                "",
                "\"literal quotes\""
            });

        Assert.AreEqual("xcrun", startInfo.FileName);
        CollectionAssert.AreEqual(
            new[]
            {
                "devicectl",
                "device",
                "process",
                "launch",
                "--device",
                "My Phone",
                "--terminate-existing",
                "--console",
                "com.example.my tests",
                "--",
                "--TestAdapterPort",
                "38300",
                "--AutoExit",
                "True",
                "--tag",
                "first",
                "--tag",
                "second",
                "--mode",
                "-diagnostic",
                "value with spaces",
                "",
                "\"literal quotes\""
            },
            startInfo.ArgumentList.ToArray());
    }

    [TestMethod]
    public void CreateLaunchProcessStartInfo_UsesOnlyFixedArgumentsWithoutCustomArguments()
    {
        var startInfo = devicectl.CreateLaunchProcessStartInfo(
            "device-id",
            "com.example.tests",
            Array.Empty<string>());

        CollectionAssert.AreEqual(
            new[]
            {
                "devicectl",
                "device",
                "process",
                "launch",
                "--device",
                "device-id",
                "--terminate-existing",
                "--console",
                "com.example.tests",
                "--",
                "--TestAdapterPort",
                "38300",
                "--AutoExit",
                "True"
            },
            startInfo.ArgumentList.ToArray());
    }

    [TestMethod]
    public void CreateProcessStartInfo_PreservesPathsAndDeviceIdsWithSpaces()
    {
        var startInfo = devicectl.CreateProcessStartInfo(
            new[] { "device", "install", "app", "--device", "My Phone", "/tmp/My Tests.app" });

        CollectionAssert.AreEqual(
            new[] { "devicectl", "device", "install", "app", "--device", "My Phone", "/tmp/My Tests.app" },
            startInfo.ArgumentList.ToArray());
    }
}
