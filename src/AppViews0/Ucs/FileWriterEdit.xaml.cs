﻿using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class FileWriterEdit : UserControl {
        public static void ShowWindow(FormType formType, FileWriterViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "文件书写器",
                FormType = formType,
                IsMaskTheParent = true,
                Width = 950,
                CloseVisible = System.Windows.Visibility.Visible,
                IconName = "Icon_FileWriter"
            }, ucFactory: (window) =>
            {
                FileWriterViewModel vm = new FileWriterViewModel(source);
                window.AddOnecePath<CloseWindowCommand>("处理关闭窗口命令", LogEnum.DevConsole, action: message => {
                    window.Close();
                }, pathId: vm.Id, location: typeof(FileWriterEdit));
                return new FileWriterEdit(vm);
            }, fixedSize: true);
        }

        private FileWriterViewModel Vm {
            get {
                return (FileWriterViewModel)this.DataContext;
            }
        }

        public FileWriterEdit(FileWriterViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
