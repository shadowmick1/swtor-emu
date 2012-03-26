using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProtocolWalker.Capture;

namespace ProtocolWalker.Dialogs
{
    /// <summary>
    /// Interaction logic for CaptureSettingsDialog.xaml
    /// </summary>
    public partial class CaptureSettingsDialog : Window
    {
        public CaptureSettingsDialog()
        {
            InitializeComponent();
            foreach (string device in CaptureService.GetAllDevices())
            {
                cb_interfaceList.Items.Add(device.ToString());
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (cb_interfaceList.SelectedIndex == -1)
            {
                MessageBox.Show("You must select an interface to listen on!");
                return;
            }
            if (textBox1.Text == "")
            {
                MessageBox.Show("You must choose one or more TCP ports to listen on for Proxy traffic!");
                return;
            }
            string portsStr = textBox1.Text;
            string[] portsStrs = portsStr.Split(',');
            int[] ports = new int[portsStrs.Length];
            for (int i = 0; i < portsStrs.Length; i++)
            {
                ports[i] = int.Parse(portsStrs[i]);
            }
            CaptureService.ProxyCapturePorts = ports;
            CaptureService.SelectDevice(cb_interfaceList.SelectedIndex);
            this.Close();
        }
    }
}
