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

using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using System;
using System.Linq;

namespace MSTestX.Console
{
    public static class TestRunnerExtensions
    {
        public static string GetDisplayName(this TestCaseStartEventArgs args)
        {
            return string.IsNullOrEmpty(args.TestCaseName) ? args.TestElement.DisplayName : args.TestCaseName;
        }

        public static Guid GetParentExecId(this TestCaseStartEventArgs args)
        {
            var parentExecIdProperty = args.TestElement.Properties.FirstOrDefault(t => t.Id == "ParentExecId");
            return parentExecIdProperty is null ? Guid.Empty : args.TestElement.GetPropertyValue<Guid>(parentExecIdProperty, Guid.Empty);
        }

        public static string GetDisplayName(this Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection.TestResultEventArgs args)
        {
            return string.IsNullOrEmpty(args.TestResult.DisplayName) ? args.TestElement.DisplayName : args.TestResult.DisplayName;
        }

        public static Guid GetParentExecId(this Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection.TestResultEventArgs args)
        {
            var parentExecIdProperty = args.TestResult.Properties.FirstOrDefault(t => t.Id == "ParentExecId");
            return parentExecIdProperty is null ? Guid.Empty : args.TestResult.GetPropertyValue<Guid>(parentExecIdProperty, Guid.Empty);
        }

        public static string FormatDuration(this TimeSpan duration)
        {
            if (duration.TotalMilliseconds < 1)
                return "[< 1ms]";
            if (duration.TotalSeconds < 1)
                return $"[{duration.Milliseconds}ms]";
            if (duration.TotalMinutes < 1)
                return $"[{duration.Seconds}s {duration.Milliseconds:0}ms]";
            if (duration.TotalHours < 1)
                return $"[{duration.Minutes}m {duration.Seconds}s {duration.Milliseconds:0}ms]";
            if (duration.TotalDays < 1)
                return $"[{duration.Hours}h {duration.Minutes}m {duration.Seconds}s {duration.Milliseconds:0}ms]";

            return $"[{Math.Floor(duration.TotalDays)}d {duration.Hours}h {duration.Minutes}m {duration.Seconds}s {duration.Milliseconds:0}ms]";
        }
    }
}
