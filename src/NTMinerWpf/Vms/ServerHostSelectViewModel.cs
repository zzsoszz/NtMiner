﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ServerHostSelectViewModel : ViewModelBase {
        private string _selectedResult;
        public readonly Action<string> OnOk;
        public ICommand HideView { get; set; }

        public ServerHostSelectViewModel(string selected, Action<string> onOk) {
            _selectedResult = selected;
            OnOk = onOk;
        }

        public List<string> ServerHosts {
            get {
                var list = NTMinerRegistry.GetControlCenterHosts().ToList();
                if (!list.Contains("127.0.0.1") && !list.Contains("localhost")) {
                    list.Add("127.0.0.1");
                }
                return list;
            }
            set {
                NTMinerRegistry.SetControlCenterHosts(value.ToArray());
                OnPropertyChanged(nameof(ServerHosts));
            }
        }

        public string SelectedResult {
            get => _selectedResult;
            set {
                _selectedResult = value;
                OnPropertyChanged(nameof(SelectedResult));
            }
        }
    }
}
