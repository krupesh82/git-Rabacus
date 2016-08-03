using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;

using Xamarin.Forms;

namespace Rabacus
{
	public partial class Disclaimer : ContentPage
	{
		public Disclaimer ()
		{
			InitializeComponent ();

			if (Device.OS == TargetPlatform.iOS) {
				gridDisclaimer.RowDefinitions [0].Height = new GridLength (20);
			}
			if (Device.OS == TargetPlatform.Android) {
				gridDisclaimer.RowDefinitions [0].Height = new GridLength (0);
				gridDisclaimer.RowDefinitions [1].Height = new GridLength (50);
			}

			var assembly = typeof(Disclaimer).GetTypeInfo ().Assembly;
			Stream stream = assembly.GetManifestResourceStream ("Rabacus.Resources.RABACUS_TOS.htm");
			string htmlText = "";
			byte[] stext = new byte[stream.Length];
			stream.Read (stext, 0, stext.Length - 1);
			htmlText = Encoding.UTF8.GetString (stext, 0, stext.Length);
			var htmlSource = new HtmlWebViewSource ();
			htmlSource.Html = htmlText;
			disclaimerPage.Source = htmlSource;
		}
	}
}