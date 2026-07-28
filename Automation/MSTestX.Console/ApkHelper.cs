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

using AndroidXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace MSTestX.Console
{
    internal static class ApkHelper
    {
        public static void GetAPKInfo(string path, out string apk_id, out string activity)
        {
            apk_id = "";
            activity = "";
            using (MemoryStream ms = new MemoryStream())
            {
                using (var file = System.IO.Compression.ZipFile.OpenRead(path))
                {
                    var entry = file.GetEntry("AndroidManifest.xml");
                    if (entry != null)
                    {
                        using (var manifestStream = entry.Open())
                        {
                            manifestStream.CopyTo(ms);
                            ms.Seek(0, SeekOrigin.Begin);
                        }
                    }
                }
                var reader = new AndroidXmlReader(ms);
                while (reader.Read())
                {
                    if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name == "manifest")
                    {
                        if (reader.MoveToAttribute("package"))
                        {
                            apk_id = reader.Value;
                        }
                        
                    }
                    else if(reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name == "activity" && activity == "")
                    {
                        if (reader.MoveToAttribute("name", "http://schemas.android.com/apk/res/android"))
                        {
                            activity = reader.Value;
                        }
                    }
                }
            }
        }
    }
}
