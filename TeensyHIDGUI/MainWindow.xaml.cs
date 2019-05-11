using Hardcodet.Wpf.TaskbarNotification;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;

using TeensyHIDWindows.Comm;

namespace TeensyHIDGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
			// UpdateDeviceList()
        }

		private static readonly ConcurrentDictionary<string, HIDConnection> TeensyConnections = new ConcurrentDictionary<string, HIDConnection>();



        private void UpdateDeviceList()
		{

		}

        #region Taskbar Icon

		private bool _hasShownMinimizedMessage;

        private void Window_StateChanged(object sender, EventArgs e)
		{
			switch (WindowState)
			{
				case WindowState.Minimized:
					OnMinimized();
                    break;
				case WindowState.Maximized:
				case WindowState.Normal:
					OnRestored();
                    break;
			}
		}

		private void OnMinimized()
		{
			TaskbarIcon.Visibility = Visibility.Visible;
			ShowInTaskbar = false;
			if (_hasShownMinimizedMessage) return;
			TaskbarIcon.ShowBalloonTip("Don't worry!", "I'm still here. Just click my icon if you want me back!", BalloonIcon.Info);
			_hasShownMinimizedMessage = true;
		}

		private void OnRestored()
		{
			TaskbarIcon.Visibility = Visibility.Collapsed;
			ShowInTaskbar = true;
			Activate();
			Focus();
        }

        private void TaskbarIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Normal;
			e.Handled = true;
		}

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			TaskbarIcon.Dispose();
        }
    }
}
