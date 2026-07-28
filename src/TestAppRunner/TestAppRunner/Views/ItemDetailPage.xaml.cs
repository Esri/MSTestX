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

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using MSTestX.UnitTestRunner.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAppRunner.ViewModels;
#if MAUI
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
#else
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endif

namespace TestAppRunner.Views
{
    /// <summary>
    /// Shows the results for a single test
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public partial class ItemDetailPage : ContentPage
	{
		internal ItemDetailPage (TestResultVM vm)
		{
            this.BindingContext = vm;
            InitializeComponent();
            loopIterationLabel.BindingContext = TestRunnerVM.Instance;
            startStopButton.BindingContext = TestRunnerVM.Instance;
            runUntilFailureButton.BindingContext = TestRunnerVM.Instance;
		}

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (!TestRunnerVM.Instance.IsBusy)
            {
                try
                {
                    await TestRunnerVM.Instance.Run(new[] { ((TestResultVM)BindingContext).Test });
                }
                catch (Exception ex)
                {
                    Logger.Log($"Run command failed: {ex}");
                    await DisplayAlert("Test Run Error", ex.Message, "OK");
                }
            }
        }

        private async void RunUntilFailureButton_Clicked(object sender, EventArgs e)
        {
            if (TestRunnerVM.Instance.IsBusy)
            {
                TestRunnerVM.Instance.Cancel();
            }
            else
            {
                try
                {
                    await TestRunnerVM.Instance.RunUntilFailure(new[] { ((TestResultVM)BindingContext).Test });
                }
                catch (Exception ex)
                {
                    Logger.Log($"Run-until-failure command failed: {ex}");
                    await DisplayAlert("Test Run Error", ex.Message, "OK");
                }
            }
        }

        private async void list_ItemSelected(object sender, SelectionChangedEventArgs args)
        {
            var item = args.CurrentSelection?.FirstOrDefault() as TestResult;
            if (item == null)
                return;
            await Navigation.PushAsync(new ItemDetailPage( new TestResultVM(item.TestCase) { Result = item }));

            // Manually deselect item.
            (sender as CollectionView).SelectedItem = null;
        }

        private async void attachment_Selected(object sender, SelectionChangedEventArgs e)
        {
            var attachment = e.CurrentSelection?.FirstOrDefault() as UriDataAttachment;
            if(attachment != null)
            {
                await Navigation.PushAsync(new AttachmentPage(attachment));
            }

            // Manually deselect item.
            (sender as CollectionView).SelectedItem = null;
        }
    }

    /// <summary>
    /// Internal use
    /// </summary>
    public class AttachmentNameConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is UriDataAttachment uda)
            {
                return uda.Uri.OriginalString.Split('\\', '/').LastOrDefault();
            }
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
