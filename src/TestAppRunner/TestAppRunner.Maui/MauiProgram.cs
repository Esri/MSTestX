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

using MSTestX;

namespace TestAppRunner.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseTestApp<App>((testOptions) =>
            {
#if __IOS__
                var logsdir = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "UnitTests");
#elif __ANDROID__
                var logsdir = System.IO.Path.Combine(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.ApplicationContext.FilesDir.Path, "UnitTests");
#elif WINDOWS
                var logsdir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UnitTests");
#endif
                testOptions.TestAssemblies = new System.Reflection.Assembly[] { typeof(UnitTests.AttachmentTests).Assembly };
                // configure default timeout
                testOptions.SettingsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>" +
                    "<RunSettings>" +
                    "<!--MSTest adapter-->" +
                    "<MSTestV2><!--If no timeout is specified on a test, use this as default-->" +
                    "<TestTimeout>30000</TestTimeout>" +
                    "</MSTestV2>" +
                    "</RunSettings>";
                if (!System.IO.Directory.Exists(logsdir))
                    System.IO.Directory.CreateDirectory(logsdir);

                testOptions.TrxOutputPath = System.IO.Path.Combine(logsdir, $"UnitTests.trx");
                testOptions.ProgressLogPath = System.IO.Path.Combine(logsdir, $"UnitTest.log");
                return testOptions;
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }
}
