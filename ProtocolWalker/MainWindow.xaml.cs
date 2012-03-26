/*
* Copyright (C) 2008-2012 Emulator Nexus <http://emulatornexus.com//>
*
* This program is free software; you can redistribute it and/or modify it
* under the terms of the GNU General Public License as published by the
* Free Software Foundation; either version 3 of the License, or (at your
* option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT
* ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
* FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
* more details.
*
* You should have received a copy of the GNU General Public License along
* with this program. If not, see <http://www.gnu.org/licenses/>.
*/

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProtocolWalker.Dialogs;
using ProtocolWalker.Capture;
using Commons;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;
using System.Xml;

namespace ProtocolWalker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly CaptureSettingsDialog captureSettingsDialog;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public MainWindow()
        {
            InitializeComponent();
            AllocConsole();
            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            captureSettingsDialog = new CaptureSettingsDialog();
            captureSettingsDialog.Closed += new EventHandler(captureSettingsDialog_Closed);
            captureSettingsDialog.ShowDialog();
            CaptureSessionList.Instance.OnAdd += new EventAwareListAddEvent<CaptureSession>(SessionList_OnAdd);
        }

        void SessionList_OnAdd(object sender, EventArgs<CaptureSession> e)
        {
            // listbox1_SelectionChanged
            //<Label Content="Raw View" FontSize="10" FontWeight="Bold" Height="28" HorizontalAlignment="Left" Margin="374,-3,0,0" Name="label2" VerticalAlignment="Top" />
            //<TextBox Height="214" HorizontalAlignment="Left" Margin="374,17,0,0" Name="textBox1" VerticalAlignment="Top" Width="849" TextWrapping="Wrap" VerticalScrollBarVisibility="Visible" AcceptsReturn="True" />

            this.Dispatcher.Invoke(new Action(() =>
                {
                    TabItem ti = new TabItem();
                    ti.Content = new Grid();
                    ti.Header = "Session " + e.Value.ID;

                    Label lbl_captured_packets = new Label()
                    {
                        Content = "Captured Packets",
                        Height = 28,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        Margin = new Thickness(1, -3, 0, 0),
                        Name = "lbl_captured_packets_" + e.Value.ID,
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        FontWeight = FontWeights.Bold,
                        FontSize = 10
                    };
                    TextBox tb_raw_view = new TextBox()
                    {
                        Height = 214,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        Margin = new Thickness(374, 17, 0, 0),
                        Name = "tb_raw_view",
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        Width = 849,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                        AcceptsReturn = true,
                        FontFamily = new FontFamily("Courier New")
                    };
                    Label lbl_raw_view = new Label()
                    {
                        Content = "Raw View",
                        FontSize = 10,
                        FontWeight = FontWeights.Bold,
                        Height = 28,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        Margin = new Thickness(374, -3, 0, 0),
                        Name = "lbl_raw_view",
                        VerticalAlignment = System.Windows.VerticalAlignment.Top
                    };
                    PacketList packet_list = new PacketList()
                    {
                        Height = 667,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        Margin = new Thickness(4, 17, 0, 0),
                        Name = "packet_list",
                        VerticalAlignment = System.Windows.VerticalAlignment.Top,
                        Width = 364
                    };

                    packet_list.Session = e.Value;
                    
                    (ti.Content as Grid).Children.Add(lbl_captured_packets);
                    (ti.Content as Grid).Children.Add(packet_list);
                    (ti.Content as Grid).Children.Add(tb_raw_view);
                    (ti.Content as Grid).Children.Add(lbl_raw_view);

                    e.Value._uiPacketList = packet_list;
                    e.Value._uiPacketRawView = tb_raw_view;

                    packet_list.SelectionChanged += new SelectionChangedEventHandler(listBox1_SelectionChanged);

                    tabControl1.Items.Add(ti);
                }));
        }

        void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PacketList plist = e.Source as PacketList;
            TORCapturedPacket packet = plist.Session.GetPacket(plist.SelectedIndex + 1);
            if (packet == null)
            {
                MessageBox.Show("This packet doesn't exist!");
                return;
            }
            plist.Session._uiPacketRawView.Text = Utility.HexDump(packet.Data);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CaptureService.StopCapture();
        }

        void captureSettingsDialog_Closed(object sender, EventArgs e)
        {
            CaptureService.mw = this;
            CaptureService.StartCapture();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Show capture settings window
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            // find out current session
            int currentSessionIndex = tabControl1.SelectedIndex;
            if (currentSessionIndex == -1)
                return;
            // find related session
            CaptureSession session = CaptureSessionList.Instance.ElementAt<CaptureSession>(currentSessionIndex);

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "XML File (*.xml)|*.xml";
            saveDialog.Title = "Save session file as ...";
            bool? result = saveDialog.ShowDialog();

            if (result == null || result == false)
            {
                Console.WriteLine("some shit happened with save dialog.");
                return;
            }

            string outputFileName = saveDialog.FileName;

            try
            {
                XmlDocument doc = XmlFactory.CreateSessionDocument(session);
                doc.Save(outputFileName);
                MessageBox.Show("Session " + session.ID + " saved at " + outputFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception occured when saving to " + outputFileName + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }
    }
}
