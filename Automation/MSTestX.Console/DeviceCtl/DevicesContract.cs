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

using System;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace MSTestX.Console.DeviceCtl
{
    public partial class Devices
    {
        [JsonPropertyName("info")]
        public Info Info { get; set; }

        [JsonPropertyName("result")]
        public Result Result { get; set; }
    }

    public partial class Result
    {
        [JsonPropertyName("devices")]
        public Device[] Devices { get; set; }
    }

    public partial class Device
    {
        [JsonPropertyName("capabilities")]
        public Capability[] Capabilities { get; set; }

        [JsonPropertyName("connectionProperties")]
        public ConnectionProperties ConnectionProperties { get; set; }

        [JsonPropertyName("deviceProperties")]
        public DeviceProperties DeviceProperties { get; set; }

        [JsonPropertyName("hardwareProperties")]
        public HardwareProperties HardwareProperties { get; set; }

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        [JsonPropertyName("tags")]
        public object[] Tags { get; set; }

        [JsonPropertyName("visibilityClass")]
        public string VisibilityClass { get; set; }
    }
}