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

using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace TestAppRunner
{
    internal class TrxWriter : TestLoggerEvents, ITestLogger, ITestLoggerWithParameters
    {
        Microsoft.VisualStudio.TestPlatform.Extensions.TrxLogger.TrxLogger logger;

        public TrxWriter(string trxOutputPath)
        {
            logger = new Microsoft.VisualStudio.TestPlatform.Extensions.TrxLogger.TrxLogger();
            var testRunDirectory = new FileInfo(trxOutputPath).Directory.FullName;
            var parameters = new Dictionary<string, string>() { { "TestRunDirectory", testRunDirectory } };
            if (!string.IsNullOrEmpty(trxOutputPath))
                parameters.Add("LogFileName", trxOutputPath);
            logger.Initialize(this, parameters);
        }

        internal static void GenerateReport(string trxOutputPath, IEnumerable<TestResult> tests)
        {
            var loggerEvents = new TrxWriter(trxOutputPath);
            foreach (var t in tests)
            {
                loggerEvents.OnTestResult(new TestResultEventArgs(t));
            }
            var result = new TestRunCompleteEventArgs(null, false, true, null, null, TimeSpan.Zero); //TRXLogger doesn't use these values anyway
            loggerEvents?.OnTestRunComplete(result);
        }

        public void OnTestRunMessage(TestRunMessageEventArgs e) => TestRunMessage?.Invoke(this, e);
        public override event EventHandler<TestRunMessageEventArgs> TestRunMessage;

        public void OnTestRunStart(TestRunStartEventArgs e) => TestRunStart?.Invoke(this, e);
        public override event EventHandler<TestRunStartEventArgs> TestRunStart;

        public void OnTestResult(TestResultEventArgs e) => TestResult?.Invoke(this, e);
        public override event EventHandler<TestResultEventArgs> TestResult;

        public void OnTestRunComplete(TestRunCompleteEventArgs e) => TestRunComplete?.Invoke(this, e);
        public override event EventHandler<TestRunCompleteEventArgs> TestRunComplete;

        public void OnDiscoveryStart(DiscoveryStartEventArgs e) => DiscoveryStart?.Invoke(this, e);
        public override event EventHandler<DiscoveryStartEventArgs> DiscoveryStart;

        public void OnDiscoveryMessage(TestRunMessageEventArgs e) => DiscoveryMessage?.Invoke(this, e);
        public override event EventHandler<TestRunMessageEventArgs> DiscoveryMessage;

        public void OnDiscoveredTests(DiscoveredTestsEventArgs e) => DiscoveredTests?.Invoke(this, e);
        public override event EventHandler<DiscoveredTestsEventArgs> DiscoveredTests;

        public void OnDiscoveryComplete(DiscoveryCompleteEventArgs e) => DiscoveryComplete?.Invoke(this, e);

        void ITestLogger.Initialize(TestLoggerEvents events, string testRunDirectory) => logger.Initialize(events, testRunDirectory);

        void ITestLoggerWithParameters.Initialize(TestLoggerEvents events, Dictionary<string, string> parameters) => logger.Initialize(events, parameters);

        public override event EventHandler<DiscoveryCompleteEventArgs> DiscoveryComplete;

    }
}