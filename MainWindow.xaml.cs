using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Lab_2
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void DeleteFile_Click(object sender, RoutedEventArgs e)
		{
			if (sender != null)
			{
				var tvi = ((ContextMenu) ((MenuItem) sender).Parent).PlacementTarget as TreeViewItem;
				var path = Path.Combine((string) tvi.Tag, (string) tvi.Header);
				var attr = File.GetAttributes(path);

				if (attr.HasFlag(FileAttributes.ReadOnly))
					File.SetAttributes(path, attr & ~FileAttributes.ReadOnly);

				File.Delete(path);
				(tvi.Parent as TreeViewItem).Items.Remove(tvi);
			}
		}

		private void OpenFile_Click(object sender, RoutedEventArgs e)
		{
			if(sender != null)
			{
				var tvi = ((ContextMenu) ((MenuItem) sender).Parent).PlacementTarget as TreeViewItem;

				using (var sr = new StreamReader(Path.Combine((string) tvi.Tag, (string) tvi.Header)))
				{
					textBlock.Text = sr.ReadToEnd();
				}
			}
		}

		private void CreateFile_Click(object sender, RoutedEventArgs e)
		{
			if(sender != null)
			{
				var tvi = ((ContextMenu) ((MenuItem) sender).Parent).PlacementTarget as TreeViewItem;
				var wnd = new Create();
				wnd.ShowDialog();

				if(wnd.FileName != null)
				{
					var path = Path.Combine((string) tvi.Tag, (string) tvi.Header);
					var newFilePath = Path.Combine(path, wnd.FileName);

					if (wnd.Directory)
						Directory.CreateDirectory(newFilePath);
					else
						File.Create(newFilePath);

					FileAttributes attr = 0;
					if (wnd.aArchive)
						attr |= FileAttributes.Archive;
					if (wnd.aReadOnly)
						attr |= FileAttributes.ReadOnly;
					if (wnd.aHidden)
						attr |= FileAttributes.Hidden;
					if (wnd.aSystem)
						attr |= FileAttributes.System;

					File.SetAttributes(newFilePath, attr);

					var item = new TreeViewItem()
					{
						Header = wnd.FileName,
						Tag = path
					};

					if (wnd.Directory)
						item.ContextMenu = CreatePathCtx();
					else
						item.ContextMenu = CreateFileCtx();

					tvi.Items.Add(item);
				}
			}
		}

		private void DeleteFolder_Click(object sender, RoutedEventArgs e)
		{
			if (sender != null)
			{
				var tvi = ((ContextMenu) ((MenuItem) sender).Parent).PlacementTarget as TreeViewItem;
				var path = Path.Combine((string) tvi.Tag, (string) tvi.Header);

				RemoveDirectory(new DirectoryInfo(path));
				Directory.Delete(path);

				if (tvi.Parent is TreeViewItem)
					(tvi.Parent as TreeViewItem).Items.Remove(tvi);
				else
					treeView.Items.Remove(tvi);
			}
		}

		void RemoveDirectory(DirectoryInfo dir)
		{
			foreach (var item in dir.EnumerateFileSystemInfos())
			{
				if (item is DirectoryInfo)
				{
					RemoveDirectory(item as DirectoryInfo);
				}
				else
				{
					var attr = item.Attributes;
					if (attr.HasFlag(FileAttributes.ReadOnly))
						item.Attributes = attr & ~FileAttributes.ReadOnly;
				}
				item.Delete();
			}
		}

		private void itemOpen_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new System.Windows.Forms.FolderBrowserDialog()
			{
				Description = "Select directory to open"
			};

			if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				var root = new TreeViewItem()
				{
					Header = Path.GetFileName(dlg.SelectedPath),
					Tag = Path.GetDirectoryName(dlg.SelectedPath),
					ContextMenu = CreatePathCtx()
				};

				DirectoryInfo dir = new DirectoryInfo(dlg.SelectedPath);
				RetrieveFiles(root, dir);

				this.treeView.Items.Add(root);
			}
		}

		private void itemExit_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		void RetrieveFiles(TreeViewItem root, DirectoryInfo dir)
		{
			foreach (var item in dir.EnumerateFileSystemInfos())
			{
				TreeViewItem tvi = new TreeViewItem()
				{
					Header = item.Name,
					Tag = Path.GetDirectoryName(item.FullName)
				};

				if (item is DirectoryInfo)
				{
					tvi.ContextMenu = CreatePathCtx();
					RetrieveFiles(tvi, item as DirectoryInfo);
				}
				else
				{
					tvi.ContextMenu = CreateFileCtx();
				}
				root.Items.Add(tvi);
			}
		}

		ContextMenu CreatePathCtx()
		{
			var create = new MenuItem()
			{
				Header = "Create"
			};
			create.Click += CreateFile_Click;
			
			var delete = new MenuItem()
			{
				Header = "Delete"
			};
			delete.Click += DeleteFolder_Click;

			var ctx = new ContextMenu();
			ctx.Items.Add(create);
			ctx.Items.Add(delete);
			return ctx;
		}

		ContextMenu CreateFileCtx()
		{
			var open = new MenuItem()
			{
				Header = "Open"
			};
			open.Click += OpenFile_Click;

			var delete = new MenuItem()
			{
				Header = "Delete"
			};
			delete.Click += DeleteFile_Click;

			var ctx = new ContextMenu();
			ctx.Items.Add(open);
			ctx.Items.Add(delete);
			return ctx;
		}

		private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if(e.NewValue is TreeViewItem)
			{
				string[] c = { "-", "r", "a", "s", "h" };
				var item = e.NewValue as TreeViewItem;
				var attr = File.GetAttributes(Path.Combine((string) item.Tag, (string) item.Header));
				rash.Text =
					c[1 * (attr.HasFlag(FileAttributes.ReadOnly) ? 1 : 0)] +
					c[2 * (attr.HasFlag(FileAttributes.Archive) ? 1 : 0)] +
					c[3 * (attr.HasFlag(FileAttributes.System) ? 1 : 0)] +
					c[4 * (attr.HasFlag(FileAttributes.Hidden) ? 1 : 0)];
			}
		}
	}
}
