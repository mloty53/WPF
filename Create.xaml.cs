using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab_2
{
	public partial class Create : Window
	{
		public string FileName { get; private set; }
		public bool Directory { get; private set; }
		public bool aReadOnly { get; private set; }
		public bool aArchive { get; private set; }
		public bool aHidden { get; private set; }
		public bool aSystem { get; private set; }

		public Create()
		{
			InitializeComponent();
		}

		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			var txt = textBox.Text;
			if (Regex.Match(txt, @"^[A-Za-z0-9_~\-]{1,8}\.(txt|php|html)$").Success == false)
			{
				MessageBox.Show("Nieprawidłowa nazwa pliku!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			FileName = txt;
			Directory = rbDirectory.IsChecked.Value;
			aReadOnly = cbReadOnly.IsChecked.Value;
			aArchive = cbArchive.IsChecked.Value;
			aHidden = cbHidden.IsChecked.Value;
			aSystem = cbSystem.IsChecked.Value;

			this.Close();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
