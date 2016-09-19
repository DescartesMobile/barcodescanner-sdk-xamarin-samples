// Copyright 2016 Scandit AG
//
// Licensed under the Apache License, Version 2.0 (the "License");
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

using Xamarin.Forms;
using Scandit.BarcodePicker.Unified;
using Scandit.BarcodePicker.Unified.Abstractions;
using System.Linq;

namespace SimpleSample
{
	public partial class SimpleSamplePage : ContentPage
	{
		void OnDidScan(ScanSession session)
		{
			// guaranteed to always have at least one element, so we don't have to 
			// check the size of the NewlyRecognizedCodes array.
			var firstCode = session.NewlyRecognizedCodes.First();
			var message = string.Format("Code Scanned:\n {0}\n({1})", firstCode.Data,
										firstCode.SymbologyString.ToUpper());
			// Because this event handler is called from an scanner-internal thread, 
			// you must make sure to dispatch to the main thread for any UI work.
			Device.BeginInvokeOnMainThread(() => this.ResultLabel.Text = message);
			session.StopScanning();
		}

		async void OnScanButtonClicked(object sender, System.EventArgs e)
		{
			// The scanning behavior of the barcode picker is configured through scan
			// settings. We start with empty scan settings and enable a very generous
			// set of symbologies. In your own apps, only enable the symbologies you
			// actually need.
			var settings = _picker.GetDefaultScanSettings();
			var symbologiesToEnable = new Symbology[] {
				Symbology.Qr,
				Symbology.Ean13,
				Symbology.Upce,
				Symbology.Ean8,
				Symbology.Upca,
				Symbology.Qr,
				Symbology.DataMatrix
			};
			foreach (var sym in symbologiesToEnable)
				settings.EnableSymbology(sym, true);
			await _picker.ApplySettingsAsync(settings);
			// This will open the scanner in full-screen mode. 
			await _picker.StartScanningAsync(false);
		}

		IBarcodePicker _picker;

		public SimpleSamplePage()
		{
			InitializeComponent();
			// must be set before you use the picker for the first time.
			ScanditService.ScanditLicense.AppKey = "--- ENTER YOUR SCANDIT APP KEY HERE ---";
			// retrieve the actual native barcode picker implementation. The concrete implementation 
			// will be different depending on the platform the code runs on.
			_picker = ScanditService.BarcodePicker;
			// will get invoked whenever a code has been scanned.
			_picker.DidScan += OnDidScan;
		}
	}
}

