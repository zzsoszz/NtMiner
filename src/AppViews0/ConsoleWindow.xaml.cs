﻿using NTMiner.Bus;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace NTMiner.Views {
    internal class SafeNativeMethods {
        internal const int GWL_STYLE = -16;
        internal const int WS_VISIBLE = 0x10000000;
        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        internal static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    }

    public partial class ConsoleWindow : Window {
        public static readonly ConsoleWindow Instance = new ConsoleWindow();
        public Action OnSplashHided;
        private double _left;
        private double _top;
        IMessagePathId messagePathId = null;
        private ConsoleWindow() {
            this.Width = AppStatic.MainWindowWidth;
            this.Height = AppStatic.MainWindowHeight;
            InitializeComponent();
            this.LocationChanged += (s, e) => {
                if (this.Left != _left || this.Top != _top) {
                    if (this.Background != WpfUtil.BlackBrush) {
                        this.Background = WpfUtil.BlackBrush;
                    }
                    _left = this.Left;
                    _top = this.Top;
                    if (messagePathId == null) {
                        messagePathId = VirtualRoot.BuildEventPath<Per1SecondEvent>("检查控制台窗口位置是否不再变动，如果不再变动将背景置为透明", LogEnum.None,
                            action: message => {
                                if (_left == this.Left && _top == this.Top) {
                                    VirtualRoot.DeletePath(messagePathId);
                                    messagePathId = null;
                                    if (this.Background != WpfUtil.TransparentBrush) {
                                        this.Background = WpfUtil.TransparentBrush;
                                    }
                                }
                            });
                    }
                }
            };
        }

        private bool _isSplashed = false;
        public void HideSplash() {
            Splash.Visibility = Visibility.Collapsed;
            this.ShowInTaskbar = false;
            IntPtr parent = new WindowInteropHelper(this).Handle;
            IntPtr console = NTMinerConsole.Show();
            SafeNativeMethods.SetParent(console, parent);
            SafeNativeMethods.SetWindowLong(console, SafeNativeMethods.GWL_STYLE, SafeNativeMethods.WS_VISIBLE);
            _isSplashed = true;
            OnSplashHided?.Invoke();
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private int _marginLeft, _marginTop, _height, _width;
        public void ReSizeConsoleWindow(int marginLeft, int marginTop, int height) {
            if (!_isSplashed) {
                return;
            }
            const int paddingLeft = 4;
            const int paddingRight = 5;
            int width = (int)this.ActualWidth - paddingLeft - paddingRight - marginLeft;
            if (width < 0) {
                width = 0;
            }
            if (_marginLeft == marginLeft && _marginTop == marginTop && _height == height && _width == width) {
                return;
            }
            _marginLeft = marginLeft;
            _marginTop = marginTop;
            _height = height;
            _width = width;

            IntPtr console = NTMinerConsole.Show();
            SafeNativeMethods.MoveWindow(console, paddingLeft + marginLeft, marginTop, width, height, true);
        }
    }
}
