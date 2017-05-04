using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Wtf.WinApp
{
    public static class Ext
    {
        public static IList<T> Include<T>(this IList<T> list, params T[] items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }

            return list;
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            filePicker.FileTypeFilter.Include("*");

            var file = await filePicker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedLogFile", file);

                TextBox1.Text = file.Path;
                Title.Text = file.Name;

                await ReadFileAsync(file.Path);
            }
            else
            {
                TextBox1.Text = "Operation canceled.";
            }
        }


        private async Task ReadFileAsync(string fileName)
        {
            await Task.Run(async () =>
            {
                Task.Yield();
                var bytes = ReadFile(fileName);

                await UpdateUI(() =>
                {
                    // Your UI update code goes here!
                    if (LogViewer.Blocks.Contains(Placeholder))
                    {
                        LogViewer.Blocks.Remove(Placeholder);
                    }
                    LogViewer.IsTextSelectionEnabled = true;

                    var p = new Paragraph();
                    p.Inlines.Add(new Run { Text = Encoding.UTF8.GetString(bytes) });
                    LogViewer.Blocks.Add(p);
                });
            });
        }

        private async Task UpdateUI(DispatchedHandler action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                action);
        }


        private byte[] ReadFile(string fileName)
        {
            var bytes = Array.Empty<byte>();
            try

            {
                using (var fsSource = new FileStream(fileName,
                    FileMode.Open, FileAccess.Read))
                {
                    // Read the source file into a byte array.
                    bytes = new byte[fsSource.Length];
                    var numBytesToRead = (int) fsSource.Length;
                    var numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        // Read may return anything from 0 to numBytesToRead.
                        var n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

                        // Break when the end of the file is reached.
                        if (n == 0)
                        {
                            break;
                        }

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }

                    return bytes;
                }
            }
            catch (FileNotFoundException ioEx)
            {
                Debug.WriteLine(ioEx.Message);
            }

            return bytes;
        }
    }
}