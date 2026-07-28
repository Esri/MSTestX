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

namespace TestAppRunner.Views
{
    /// <summary>
    /// Generates the proper icon to show for a test result
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class OutcomeIcon : Label
    {
        /// <summary>
        /// Identifies the <see cref="Result"/> Bindable property.
        /// </summary>
        public static readonly BindableProperty ResultProperty =
            BindableProperty.Create(nameof(Result), typeof(TestResult), typeof(OutcomeIcon), null, BindingMode.OneWay, null, OnResultPropertyChanged);

        /// <summary>
        /// Gets or sets the test result
        /// </summary>
        public TestResult Result
        {
            get { return (TestResult)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        private static void OnResultPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var icon = bindable as OutcomeIcon;
            icon.UpdateIcon();
        }

        private void UpdateIcon()
        {
            if(Result == null)
            {
                Text = "";
            }
            else { 
                switch (Result.Outcome)
                {
                    case TestOutcome.NotFound:
                        Text = "❔";
                        TextColor = Colors.Orange;
                        break;
                    case TestOutcome.Failed:
                        if (Result.ErrorStackTrace == null && Result.ErrorMessage != null &&  Result.ErrorMessage.Contains("timeout"))
                            Text = "⏱";
                        else
                            Text = "⛔"; //⛔⨯"
                        TextColor = Colors.Red;
                        break;
                    case TestOutcome.Passed:
                        Text = "✔";
                        TextColor = Colors.Green;
                        break;
                    case TestOutcome.Skipped:
                        Text = "⚠"; 
                        TextColor = Colors.Gray;
                        break;
                    case TestOutcome.None:
                    default:
                        Text = "";
                        break;
                }
            }
        }
    }
}
