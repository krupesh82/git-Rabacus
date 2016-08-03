﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Rabacus
{
	public partial class Calculator : ContentPage
	{
		string resourcePrefix = "";
		double percentageComplete = 0;
		double savings = 0.0;
		double percentageTax = 0.0;
		double width;
		double height;

		public Calculator ()
		{
			InitializeComponent ();

			if (Device.OS == TargetPlatform.iOS)
			{
				resourcePrefix = "Rabacus.iOS.";
				gridCalc.RowDefinitions[0].Height = new GridLength(20);
				gridPercComplete.RowDefinitions[1].Height = new GridLength(130);
				gridPercComplete.ColumnDefinitions[1].Width = new GridLength(130);
				btnSave.HeightRequest = 30;
				btnCalculate.HeightRequest = 30;
			}
			if (Device.OS == TargetPlatform.Android) {
				resourcePrefix = "Rabacus.Droid.";
				gridCalc.RowDefinitions [0].Height = new GridLength (0);
				gridCalc.RowDefinitions [1].Height = new GridLength (50);
				gridCalc.RowDefinitions [3].Height = new GridLength (40);
				gridCalc.RowDefinitions [4].Height = new GridLength (40);
				gridCalc.RowDefinitions [5].Height = new GridLength (40);
				gridCalc.RowDefinitions [6].Height = new GridLength (40);
				gridCalc.RowDefinitions [7].Height = new GridLength (40);
				gridCalc.RowDefinitions [8].Height = new GridLength (40);
				gridCalc.RowDefinitions [10].Height = new GridLength (0);
				gridPercComplete.RowDefinitions [1].Height = new GridLength (140);
				gridPercComplete.ColumnDefinitions [1].Width = new GridLength (140);
				btnSave.HeightRequest = 40;
				btnCalculate.HeightRequest = 40;
				//blankText.HeightRequest = 0;
			}

			PopulateStates ();
			PopulateMaritalStatus ();
			PopulateIncome ();

			if (percentageComplete != 100.0)
				btnCalculate.IsEnabled = false;
			else
				btnCalculate.IsEnabled = true;

			var s = new FormattedString ();
			s.Spans.Add (new Span{ Text = percentageTax.ToString () + "%", FontSize= 27, FontAttributes = FontAttributes.Bold });
			s.Spans.Add (new Span { Text = Environment.NewLine });
			s.Spans.Add (new Span{ Text = "Tax Rate", FontSize= 16 });
			lblPercTax.FormattedText = s;

			s = new FormattedString ();
			s.Spans.Add (new Span{ Text = percentageTax.ToString () + "%", FontSize= 34, FontAttributes = FontAttributes.Bold });
			s.Spans.Add (new Span { Text = Environment.NewLine });
			s.Spans.Add (new Span{ Text = "Complete", FontSize= 16 });
			lblPercComplete.FormattedText = s;
		}

		protected override void OnSizeAllocated (double width, double height)
		{
			base.OnSizeAllocated (width, height);
			if (width != this.width || height != this.height) {
				this.width = width;
				this.height = height;

				if (width > height) 
				{
					gridCalc.ColumnDefinitions[2].Width = new GridLength(200);
				}
				else 
				{
					gridCalc.ColumnDefinitions[2].Width = new GridLength(160);
				}
			}
		}
		private void ClearFields()
		{
			this.entryPP.Text = "0";
			this.entryCI.Text = "0";
			this.entrySP.Text = "0";
			pickerState.SelectedIndex = -1;
			pickerMS.SelectedIndex = -1;
			pickerIncome.SelectedIndex = -1;
		}

		private void PopulateStates()
		{
			pickerState.Items.Clear ();
			List<DataString> dataStates = DataStrings.GetStates();
				if (dataStates != null && dataStates.Count > 0) {
					foreach (DataString data in dataStates) {
						pickerState.Items.Add (data.Text);
					}
				}
		}

		private void PopulateMaritalStatus()
		{
			pickerMS.Items.Clear ();
			pickerMS.Items.Add ("Single");
			pickerMS.Items.Add ("Married");
		}

		private void PopulateIncome()
		{
			List<DataString> dataIncome = null;
			pickerIncome.Items.Clear ();

			if (pickerMS.SelectedIndex == 0) 
			{
				dataIncome = DataStrings.GetDataIncomeGroup ("Single");
			} 
			else if (pickerMS.SelectedIndex == 1) 
			{
				dataIncome = DataStrings.GetDataIncomeGroup("Married");
			}

			if (dataIncome != null && dataIncome.Count > 0) 
			{
				if (pickerMS.SelectedIndex == 0) 
				{
					foreach (DataString data in dataIncome) 
					{
						pickerIncome.Items.Add (data.Text);
					}
				}
				else if (pickerMS.SelectedIndex == 1) 
				{
					foreach (DataString data in dataIncome) 
					{
						pickerIncome.Items.Add (data.Text);
					}
				}
			}
		}

		public void EntryPP_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (e.NewTextValue == e.OldTextValue)
				return;

			double pp = 0.0;

			if (!Double.TryParse (e.NewTextValue, out pp))
				Double.TryParse (e.OldTextValue, out pp);

			entryPP.Text = pp.ToString ();

			UpdatePercentageComplete ();
		}

		public void EntryCI_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (e.NewTextValue == e.OldTextValue)
				return;

			double ci = 0.0;

			if (!Double.TryParse (e.NewTextValue, out ci))
				Double.TryParse (e.OldTextValue, out ci);

			entryCI.Text = ci.ToString ();

			UpdatePercentageComplete ();
		}

		public void EntrySP_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (e.NewTextValue == e.OldTextValue)
				return;

			double sp = 0.0;

			if (!Double.TryParse (e.NewTextValue, out sp))
				Double.TryParse (e.OldTextValue, out sp);

			entrySP.Text = sp.ToString ();

			UpdatePercentageComplete ();
		}

		public void pickerState_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateTaxRate ();
			UpdatePercentageComplete ();
		}

		public void pickerMS_SelectedIndexChanged(object sender, EventArgs e)
		{
			PopulateIncome ();
			UpdateTaxRate ();
			UpdatePercentageComplete ();
		}

		public void pickerIncome_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateTaxRate ();
			UpdatePercentageComplete ();
		}

		private void UpdateTaxRate()
		{
			percentageTax = CalculateTaxRate ();
			var s = new FormattedString ();
			s.Spans.Add (new Span{ Text = percentageTax.ToString () + "%", FontSize= 27, FontAttributes = FontAttributes.Bold });
			s.Spans.Add (new Span { Text = Environment.NewLine });
			s.Spans.Add (new Span{ Text = "Tax Rate", FontSize= 16 });
			lblPercTax.FormattedText = s;
		}

		private double CalculateTaxRate()
		{
			string stateTax = "0.0";
			string federalTax = "0.0";

			if (pickerState.SelectedIndex >= 0)
			{
				stateTax = DataStrings.GetStates()[pickerState.SelectedIndex].Value;
			}

			if (pickerMS.SelectedIndex == 0 && pickerIncome.SelectedIndex >= 0) 
			{
				federalTax = DataStrings.GetDataIncomeGroup("Single")[pickerIncome.SelectedIndex].Value;
			} 
			else if (pickerMS.SelectedIndex == 1 && pickerIncome.SelectedIndex >= 0) 
			{
				federalTax = DataStrings.GetDataIncomeGroup("Married")[pickerIncome.SelectedIndex].Value;
			}

			return Convert.ToDouble(stateTax) + Convert.ToDouble(federalTax);
		}

		private void UpdatePercentageComplete()
		{
			int count = 0;

			if (!string.IsNullOrEmpty (entryPP.Text))
				count++;

			if (!string.IsNullOrEmpty (entryCI.Text))
				count++;

			if (!string.IsNullOrEmpty (entrySP.Text))
				count++;

			if (pickerState.SelectedIndex >= 0)
				count++;

			if (pickerMS.SelectedIndex >= 0)
				count++;

			if (pickerIncome.SelectedIndex >= 0)
				count++;

			percentageComplete = count * 100 / 6;
			var s = new FormattedString ();
			s.Spans.Add (new Span{ Text = percentageComplete.ToString () + "%", FontSize= 34, FontAttributes = FontAttributes.Bold });
			s.Spans.Add (new Span { Text = Environment.NewLine });
			s.Spans.Add (new Span{ Text = "Complete", FontSize= 16 });
			lblPercComplete.FormattedText = s;

			if (percentageComplete != 100.0)
				btnCalculate.IsEnabled = false;
			else
				btnCalculate.IsEnabled = true;
		}

		private void CalculateSavings()
		{
			if (percentageComplete == 100.00)
			{
				double pp = Convert.ToDouble(entryPP.Text);
				double ci = Convert.ToDouble(entryCI.Text);
				double sp = Convert.ToDouble(entrySP.Text);

				if (sp < pp + ci)
				{
					savings = 0.00;
				}
				else
				{
					double gain = sp - (pp + ci);
					savings = gain * percentageTax / 100;
					savings = Convert.ToDouble(savings.ToString("#.##"));
				}
			}
			else
			{
				savings = 0.0;
			}
		}

		public void btnCalculate_Clicked(object sender, EventArgs e)
		{
			CalculateSavings ();
			var s = new FormattedString ();
			s.Spans.Add (new Span{ Text = "Your 1031 savings", FontSize= 14 });
			s.Spans.Add (new Span { Text = Environment.NewLine });
			s.Spans.Add (new Span{ Text = "are:", FontSize= 14 });
			s.Spans.Add (new Span { Text = Environment.NewLine });
			s.Spans.Add (new Span{ Text = "$" + savings.ToString (), FontSize= 24, FontAttributes = FontAttributes.Bold });
			lblPercComplete.FormattedText = s;
		}

		private Property GetProperty()
		{
			Property prop = new Property ();

			if (!string.IsNullOrEmpty (entryPP.Text))
				prop.PurchasePrice = Convert.ToDouble (entryPP.Text);

			if (!string.IsNullOrEmpty (entryCI.Text))
				prop.CapitalImprovements = Convert.ToDouble (entryCI.Text);

			if (!string.IsNullOrEmpty (entrySP.Text))
				prop.SalePrice = Convert.ToDouble (entrySP.Text);

			if (pickerState.SelectedIndex >= 0)
				prop.State = DataStrings.GetStates() [pickerState.SelectedIndex].Text;

			if (pickerMS.SelectedIndex >= 0)
				prop.MaritalStatus = pickerMS.SelectedIndex == 0 ? "Single" : "Married";

			if (pickerIncome.SelectedIndex >= 0) 
			{
				if (pickerMS.SelectedIndex == 0)
					prop.Income = DataStrings.GetDataIncomeGroup("Single") [pickerIncome.SelectedIndex].Text;
				else if (pickerMS.SelectedIndex == 1)
					prop.Income = DataStrings.GetDataIncomeGroup("Married") [pickerIncome.SelectedIndex].Text;
			}

			prop.TaxRate = percentageTax;

			CalculateSavings ();
			prop.Savings = savings;

			return prop;
		}

		async void btnSave_Clicked(object sender, EventArgs e)
		{
			if (percentageComplete != 100.0) 
			{
				ShowToast ("Please fill in all the fields before saving the calculation.", 2000);
			} 
			else 
			{
				var saveDialog = new SaveDialog (GetProperty ());
				saveDialog.Disappearing += (object sender1, EventArgs ea) => {
					if(!saveDialog.IsCancelled)
					{
						ShowToast("Property '" + saveDialog.Property.Name + "' saved successfully.", 2000);
						ClearFields ();
					}
				};
				await Navigation.PushModalAsync (saveDialog);
			}
		}

		private void ShowToast(string message, int toastTime)
		{
			toastCalcLabel.Text = message;
			toastCalcLabel.IsVisible = true;
			Device.StartTimer(TimeSpan.FromMilliseconds(toastTime), () => {
				toastCalcLabel.Text = "";
				toastCalcLabel.IsVisible = false;
				return false;
			});
		}
	}
}

