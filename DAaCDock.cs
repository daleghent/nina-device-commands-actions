#region "copyright"

/*
    Copyright Dale Ghent <daleg@elemental.org>
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/
*/

#endregion "copyright"

using CommunityToolkit.Mvvm.Input;
using NINA.Core.Enum;
using NINA.Core.Utility;
using NINA.Core.Utility.Notification;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Equipment.Interfaces.ViewModel;
using NINA.Profile.Interfaces;
using NINA.WPF.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace DaleGhent.NINA.DeviceActionsCommands {

    [Export(typeof(IDockableVM))]
    public partial class DAaCDock : DockableVM, IDisposable {
        private readonly ICameraMediator cameraMediator;
        private readonly IDomeMediator domeMediator;
        private readonly IFilterWheelMediator filterWheelMediator;
        private readonly IFlatDeviceMediator flatDeviceMediator;
        private readonly IFocuserMediator focuserMediator;
        private readonly IGuiderMediator guiderMediator;
        private readonly IRotatorMediator rotatorMediator;
        private readonly ISafetyMonitorMediator safetyMonitorMediator;
        private readonly ISwitchMediator switchMediator;
        private readonly ITelescopeMediator telescopeMediator;
        private readonly IWeatherDataMediator weatherDataMediator;

        private const DeviceTypeEnum defaultDeviceType = DeviceTypeEnum.TELESCOPE;

        [ImportingConstructor]
        public DAaCDock(IProfileService profileService, ICameraMediator cameraMediator, IDomeMediator domeMediator, IFilterWheelMediator filterWheelMediator, IFlatDeviceMediator flatDeviceMediator,
                        IFocuserMediator focuserMediator, IGuiderMediator guiderMediator, IRotatorMediator rotatorMediator, ISafetyMonitorMediator safetyMonitorMediator,
                        ISwitchMediator switchMediator, ITelescopeMediator telescopeMediator, IWeatherDataMediator weatherDataMediator) : base(profileService) {
            this.cameraMediator = cameraMediator;
            this.domeMediator = domeMediator;
            this.filterWheelMediator = filterWheelMediator;
            this.flatDeviceMediator = flatDeviceMediator;
            this.focuserMediator = focuserMediator;
            this.guiderMediator = guiderMediator;
            this.rotatorMediator = rotatorMediator;
            this.safetyMonitorMediator = safetyMonitorMediator;
            this.switchMediator = switchMediator;
            this.telescopeMediator = telescopeMediator;
            this.weatherDataMediator = weatherDataMediator;

            cameraMediator.Connected += OnDeviceConnected;
            cameraMediator.Disconnected += OnDeviceDisconnected;
            domeMediator.Connected += OnDeviceConnected;
            domeMediator.Disconnected += OnDeviceDisconnected;
            filterWheelMediator.Connected += OnDeviceConnected;
            filterWheelMediator.Disconnected += OnDeviceDisconnected;
            flatDeviceMediator.Connected += OnDeviceConnected;
            flatDeviceMediator.Disconnected += OnDeviceDisconnected;
            focuserMediator.Connected += OnDeviceConnected;
            focuserMediator.Disconnected += OnDeviceDisconnected;
            guiderMediator.Connected += OnDeviceConnected;
            guiderMediator.Disconnected += OnDeviceDisconnected;
            rotatorMediator.Connected += OnDeviceConnected;
            rotatorMediator.Disconnected += OnDeviceDisconnected;
            safetyMonitorMediator.Connected += OnDeviceConnected;
            safetyMonitorMediator.Disconnected += OnDeviceDisconnected;
            switchMediator.Connected += OnDeviceConnected;
            switchMediator.Disconnected += OnDeviceDisconnected;
            telescopeMediator.Connected += OnDeviceConnected;
            telescopeMediator.Disconnected += OnDeviceDisconnected;
            weatherDataMediator.Connected += OnDeviceConnected;
            weatherDataMediator.Disconnected += OnDeviceDisconnected;

            Title = "Device Actions and Commands";
        }

        public override bool IsTool => true;

        private DeviceTypeEnum actionDeviceType = defaultDeviceType;

        public static DeviceTypeEnum[] DeviceTypes => Enum.GetValues(typeof(DeviceTypeEnum))
            .Cast<DeviceTypeEnum>()
            .ToArray();

        public bool GetDeviceStatus(DeviceTypeEnum deviceType) {
            return deviceType switch {
                DeviceTypeEnum.CAMERA => cameraMediator.GetInfo().Connected,
                DeviceTypeEnum.DOME => domeMediator.GetInfo().Connected,
                DeviceTypeEnum.FILTERWHEEL => filterWheelMediator.GetInfo().Connected,
                DeviceTypeEnum.FLATDEVICE => flatDeviceMediator.GetInfo().Connected,
                DeviceTypeEnum.FOCUSER => focuserMediator.GetInfo().Connected,
                DeviceTypeEnum.GUIDER => guiderMediator.GetInfo().Connected,
                DeviceTypeEnum.ROTATOR => rotatorMediator.GetInfo().Connected,
                DeviceTypeEnum.SAFETYMONITOR => safetyMonitorMediator.GetInfo().Connected,
                DeviceTypeEnum.SWITCH => switchMediator.GetInfo().Connected,
                DeviceTypeEnum.TELESCOPE => telescopeMediator.GetInfo().Connected,
                DeviceTypeEnum.WEATHERDATA => weatherDataMediator.GetInfo().Connected,
                _ => false,
            };
        }

        #region Device Actions

        private bool actionDeviceConnected = false;

        public bool ActionDeviceConnected {
            get => actionDeviceConnected;
            private set {
                actionDeviceConnected = value;
                RaisePropertyChanged();
            }
        }

        public DeviceTypeEnum ActionDeviceType {
            get => actionDeviceType;
            set {
                actionDeviceType = value;

                ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
                SupportedActions = GetSupportedActions();

                if (string.IsNullOrEmpty(ActionName) && SupportedActions.Any()) {
                    ActionName = SupportedActions.First();
                }

                RaiseAllPropertiesChanged();
            }
        }

        private AsyncObservableCollection<string> supportedActions = [];

        public AsyncObservableCollection<string> SupportedActions {
            get => supportedActions;
            set {
                supportedActions = value;
                RaisePropertyChanged();
            }
        }

        private string actionName = string.Empty;

        public string ActionName {
            get => actionName;
            set {
                actionName = value;
                RaisePropertyChanged();
            }
        }

        private string actionParameters = string.Empty;

        public string ActionParameters {
            get => actionParameters;
            set {
                if (!value.Equals(actionParameters)) {
                    actionParameters = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string actionOutput = string.Empty;

        public string ActionOutput {
            get => actionOutput;
            private set {
                actionOutput = value;
                RaisePropertyChanged();
            }
        }

        [RelayCommand]
        private async Task<bool> DoRunAction() {
            await Task.Run(() => {
                string output = string.Empty;

                try {
                    switch (actionDeviceType) {
                        case DeviceTypeEnum.CAMERA:
                            output = cameraMediator.Action(actionName, actionParameters);
                            break;

                        case DeviceTypeEnum.DOME:
                            output = domeMediator.Action(actionName, actionParameters);
                            break;

                        case DeviceTypeEnum.FILTERWHEEL:
                            output = filterWheelMediator.Action(actionName, actionParameters);
                            break;

                        case DeviceTypeEnum.FLATDEVICE:
                            output = flatDeviceMediator.Action(actionName, actionParameters);
                            break;

                        case DeviceTypeEnum.FOCUSER:
                            output = focuserMediator.Action(actionName, actionParameters);
                            break;

                        case DeviceTypeEnum.GUIDER:
                            output = guiderMediator.Action(actionName, actionParameters);
                            break;

                        case DeviceTypeEnum.ROTATOR:
                            output = rotatorMediator.Action(actionName, actionParameters);
                            break;

                        case DeviceTypeEnum.SAFETYMONITOR:
                            output = safetyMonitorMediator.Action(actionName, actionParameters);
                            break;

                        case DeviceTypeEnum.SWITCH:
                            output = switchMediator.Action(actionName, actionParameters);
                            break;

                        case DeviceTypeEnum.TELESCOPE:
                            output = telescopeMediator.Action(actionName, actionParameters);
                            break;

                        case DeviceTypeEnum.WEATHERDATA:
                            output = weatherDataMediator.Action(actionName, actionParameters);
                            break;

                        default:
                            return false;
                    }

                    ActionOutput = output;
                    Logger.Info($"{actionDeviceType} Action {actionName}({actionParameters}) returned: {output}");
                } catch (Exception ex) {
                    Logger.Error($"{actionDeviceType} Action {actionName}({actionParameters}) failed: {ex.Message}");
                    Notification.ShowExternalError(ex.Message, "ASCOM driver");
                    return false;
                }

                return true;
            });

            return true;
        }

        #endregion

        #region SendCommand

        private bool sendCommandDeviceConnected = false;

        public bool SendCommandDeviceConnected {
            get => sendCommandDeviceConnected;
            private set {
                sendCommandDeviceConnected = value;
                RaisePropertyChanged();
            }
        }

        private string command = string.Empty;

        public string Command {
            get => command;
            set {
                if (!value.Equals(command)) {
                    command = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool raw = true;

        public bool Raw {
            get => raw;
            set {
                raw = value;
                RaisePropertyChanged();
            }
        }

        private DeviceTypeEnum sendCommandDeviceType = defaultDeviceType;

        public DeviceTypeEnum SendCommandDeviceType {
            get => sendCommandDeviceType;
            set {
                sendCommandDeviceType = value;
                SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);

                RaisePropertyChanged();
            }
        }

        private SendCommandTypeEnum sendCommandType = SendCommandTypeEnum.STRING;

        public SendCommandTypeEnum SendCommandType {
            get => sendCommandType;
            set {
                sendCommandType = value;
                RaisePropertyChanged();
            }
        }

        public static SendCommandTypeEnum[] SendCommandTypes => Enum.GetValues(typeof(SendCommandTypeEnum))
            .Cast<SendCommandTypeEnum>().ToArray();

        private string sendCommandOutput = string.Empty;

        public string SendCommandOutput {
            get => sendCommandOutput;
            private set {
                sendCommandOutput = value;
                RaisePropertyChanged();
            }
        }

        [RelayCommand]
        private async Task<bool> DoRunSend() {
            if (string.IsNullOrEmpty(command)) { return false; }

            await Task.Run(() => {
                object output = null;

                try {
                    switch (sendCommandDeviceType) {
                        case DeviceTypeEnum.CAMERA:
                            switch (sendCommandType) {
                                case SendCommandTypeEnum.STRING:
                                    output = cameraMediator.SendCommandString(command, raw);
                                    break;

                                case SendCommandTypeEnum.BOOLEAN:
                                    output = cameraMediator.SendCommandBool(command, raw);
                                    break;

                                case SendCommandTypeEnum.BLIND:
                                    cameraMediator.SendCommandBlind(command, raw);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case DeviceTypeEnum.DOME:
                            switch (sendCommandType) {
                                case SendCommandTypeEnum.STRING:
                                    output = domeMediator.SendCommandString(command, raw);
                                    break;

                                case SendCommandTypeEnum.BOOLEAN:
                                    output = domeMediator.SendCommandBool(command, raw);
                                    break;

                                case SendCommandTypeEnum.BLIND:
                                    domeMediator.SendCommandBlind(command, raw);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case DeviceTypeEnum.FILTERWHEEL:
                            switch (sendCommandType) {
                                case SendCommandTypeEnum.STRING:
                                    output = filterWheelMediator.SendCommandString(command, raw);
                                    break;

                                case SendCommandTypeEnum.BOOLEAN:
                                    output = filterWheelMediator.SendCommandBool(command, raw);
                                    break;

                                case SendCommandTypeEnum.BLIND:
                                    filterWheelMediator.SendCommandBlind(command, raw);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case DeviceTypeEnum.FLATDEVICE:
                            switch (sendCommandType) {
                                case SendCommandTypeEnum.STRING:
                                    output = flatDeviceMediator.SendCommandString(command, raw);
                                    break;

                                case SendCommandTypeEnum.BOOLEAN:
                                    output = flatDeviceMediator.SendCommandBool(command, raw);
                                    break;

                                case SendCommandTypeEnum.BLIND:
                                    flatDeviceMediator.SendCommandBlind(command, raw);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case DeviceTypeEnum.FOCUSER:
                            switch (sendCommandType) {
                                case SendCommandTypeEnum.STRING:
                                    output = focuserMediator.SendCommandString(command, raw);
                                    break;

                                case SendCommandTypeEnum.BOOLEAN:
                                    output = focuserMediator.SendCommandBool(command, raw);
                                    break;

                                case SendCommandTypeEnum.BLIND:
                                    focuserMediator.SendCommandBlind(command, raw);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case DeviceTypeEnum.GUIDER:
                            switch (sendCommandType) {
                                case SendCommandTypeEnum.STRING:
                                    output = guiderMediator.SendCommandString(command, raw);
                                    break;

                                case SendCommandTypeEnum.BOOLEAN:
                                    output = guiderMediator.SendCommandBool(command, raw);
                                    break;

                                case SendCommandTypeEnum.BLIND:
                                    guiderMediator.SendCommandBlind(command, raw);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case DeviceTypeEnum.ROTATOR:
                            switch (sendCommandType) {
                                case SendCommandTypeEnum.STRING:
                                    output = rotatorMediator.SendCommandString(command, raw);
                                    break;

                                case SendCommandTypeEnum.BOOLEAN:
                                    output = rotatorMediator.SendCommandBool(command, raw);
                                    break;

                                case SendCommandTypeEnum.BLIND:
                                    rotatorMediator.SendCommandBlind(command, raw);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case DeviceTypeEnum.SAFETYMONITOR:
                            switch (sendCommandType) {
                                case SendCommandTypeEnum.STRING:
                                    output = safetyMonitorMediator.SendCommandString(command, raw);
                                    break;

                                case SendCommandTypeEnum.BOOLEAN:
                                    output = safetyMonitorMediator.SendCommandBool(command, raw);
                                    break;

                                case SendCommandTypeEnum.BLIND:
                                    safetyMonitorMediator.SendCommandBlind(command, raw);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case DeviceTypeEnum.SWITCH:
                            switch (sendCommandType) {
                                case SendCommandTypeEnum.STRING:
                                    output = switchMediator.SendCommandString(command, raw);
                                    break;

                                case SendCommandTypeEnum.BOOLEAN:
                                    output = switchMediator.SendCommandBool(command, raw);
                                    break;

                                case SendCommandTypeEnum.BLIND:
                                    switchMediator.SendCommandBlind(command, raw);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case DeviceTypeEnum.TELESCOPE:
                            switch (sendCommandType) {
                                case SendCommandTypeEnum.STRING:
                                    output = telescopeMediator.SendCommandString(command, raw);
                                    break;

                                case SendCommandTypeEnum.BOOLEAN:
                                    output = telescopeMediator.SendCommandBool(command, raw);
                                    break;

                                case SendCommandTypeEnum.BLIND:
                                    telescopeMediator.SendCommandBlind(command, raw);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case DeviceTypeEnum.WEATHERDATA:
                            switch (sendCommandType) {
                                case SendCommandTypeEnum.STRING:
                                    output = weatherDataMediator.SendCommandString(command, raw);
                                    break;

                                case SendCommandTypeEnum.BOOLEAN:
                                    output = weatherDataMediator.SendCommandBool(command, raw);
                                    break;

                                case SendCommandTypeEnum.BLIND:
                                    weatherDataMediator.SendCommandBlind(command, raw);
                                    break;

                                default:
                                    break;
                            }
                            break;
                    }

                    if (sendCommandType != SendCommandTypeEnum.BLIND) {
                        Logger.Info($"{sendCommandDeviceType} SendCommand {command} returned: {output}");
                        SendCommandOutput = output.ToString();
                    }
                } catch (Exception ex) {
                    Logger.Error($"{sendCommandDeviceType} SendCommand {command} failed: {ex.Message}");
                    Notification.ShowExternalError(ex.Message, "ASCOM driver");
                    return false;
                }

                return true;
            });

            return true;
        }

        private AsyncObservableCollection<string> GetSupportedActions() {
            IList<string> actions = [];

            switch (actionDeviceType) {
                case DeviceTypeEnum.CAMERA:
                    if (cameraMediator.GetInfo().Connected) {
                        actions = cameraMediator.GetInfo().SupportedActions;
                    }
                    break;

                case DeviceTypeEnum.DOME:
                    if (domeMediator.GetInfo().Connected) {
                        actions = domeMediator.GetInfo().SupportedActions;
                    }
                    break;

                case DeviceTypeEnum.FILTERWHEEL:
                    if (filterWheelMediator.GetInfo().Connected) {
                        actions = filterWheelMediator.GetInfo().SupportedActions;
                    }
                    break;

                case DeviceTypeEnum.FLATDEVICE:
                    if (flatDeviceMediator.GetInfo().Connected) {
                        actions = flatDeviceMediator.GetInfo().SupportedActions;
                    }
                    break;

                case DeviceTypeEnum.FOCUSER:
                    if (focuserMediator.GetInfo().Connected) {
                        actions = focuserMediator.GetInfo().SupportedActions;
                    }
                    break;

                case DeviceTypeEnum.GUIDER:
                    if (guiderMediator.GetInfo().Connected) {
                        actions = guiderMediator.GetInfo().SupportedActions;
                    }
                    break;

                case DeviceTypeEnum.ROTATOR:
                    if (rotatorMediator.GetInfo().Connected) {
                        actions = rotatorMediator.GetInfo().SupportedActions;
                    }
                    break;

                case DeviceTypeEnum.SAFETYMONITOR:
                    if (safetyMonitorMediator.GetInfo().Connected) {
                        actions = safetyMonitorMediator.GetInfo().SupportedActions;
                    }
                    break;

                case DeviceTypeEnum.SWITCH:
                    if (switchMediator.GetInfo().Connected) {
                        actions = switchMediator.GetInfo().SupportedActions;
                    }
                    break;

                case DeviceTypeEnum.TELESCOPE:
                    if (telescopeMediator.GetInfo().Connected) {
                        actions = telescopeMediator.GetInfo().SupportedActions;
                    }
                    break;

                case DeviceTypeEnum.WEATHERDATA:
                    if (weatherDataMediator.GetInfo().Connected) {
                        actions = weatherDataMediator.GetInfo().SupportedActions;
                    }
                    break;
            }

            // If the device is not connected or no actions are available, use the actions loaded from the JSON template
            if (actions == null || actions.Count == 0) {
                Logger.Debug($"No {actionDeviceType} actions available");
                return supportedActions;
            }

            var actionsList = new AsyncObservableCollection<string>();
            foreach (var action in actions) {
                actionsList.Add(action);
            }

            return actionsList;
        }


        private Task OnDeviceConnected(object sender, EventArgs e) {
            switch (sender) {
                case ICameraVM:
                    if (actionDeviceType == DeviceTypeEnum.CAMERA) {
                        SupportedActions = GetSupportedActions();
                        ActionName = SupportedActions.FirstOrDefault();
                        ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
                    }
                    if (sendCommandDeviceType == DeviceTypeEnum.CAMERA) {
                        SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);
                    }
                    break;

                case IDomeVM:
                    if (actionDeviceType == DeviceTypeEnum.DOME) {
                        SupportedActions = GetSupportedActions();
                        ActionName = SupportedActions.FirstOrDefault();
                        ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
                    }
                    if (sendCommandDeviceType == DeviceTypeEnum.DOME) {
                        SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);
                    }
                    break;

                case IFilterWheelVM:
                    if (actionDeviceType == DeviceTypeEnum.FILTERWHEEL) {
                        SupportedActions = GetSupportedActions();
                        ActionName = SupportedActions.FirstOrDefault();
                        ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
                    }
                    if (sendCommandDeviceType == DeviceTypeEnum.FILTERWHEEL) {
                        SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);
                    }
                    break;

                case IFlatDeviceVM:
                    if (actionDeviceType == DeviceTypeEnum.FLATDEVICE) {
                        SupportedActions = GetSupportedActions();
                        ActionName = SupportedActions.FirstOrDefault();
                        ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
                    }
                    if (sendCommandDeviceType == DeviceTypeEnum.FLATDEVICE) {
                        SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);
                    }
                    break;
                case IFocuserVM:
                    if (actionDeviceType == DeviceTypeEnum.FOCUSER) {
                        SupportedActions = GetSupportedActions();
                        ActionName = SupportedActions.FirstOrDefault();
                        ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
                    }
                    if (sendCommandDeviceType == DeviceTypeEnum.FOCUSER) {
                        SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);
                    }
                    break;

                case IGuiderVM:
                    if (actionDeviceType == DeviceTypeEnum.GUIDER) {
                        SupportedActions = GetSupportedActions();
                        ActionName = SupportedActions.FirstOrDefault();
                        ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
                    }
                    if (sendCommandDeviceType == DeviceTypeEnum.GUIDER) {
                        SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);
                    }
                    break;

                case IRotatorVM:
                    if (actionDeviceType == DeviceTypeEnum.ROTATOR) {
                        SupportedActions = GetSupportedActions();
                        ActionName = SupportedActions.FirstOrDefault();
                        ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
                    }
                    if (sendCommandDeviceType == DeviceTypeEnum.ROTATOR) {
                        SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);
                    }
                    break;

                case ISafetyMonitorVM:
                    if (actionDeviceType == DeviceTypeEnum.SAFETYMONITOR) {
                        SupportedActions = GetSupportedActions();
                        ActionName = SupportedActions.FirstOrDefault();
                        ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
                    }
                    if (sendCommandDeviceType == DeviceTypeEnum.SAFETYMONITOR) {
                        SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);
                    }
                    break;

                case ISwitchVM:
                    if (actionDeviceType == DeviceTypeEnum.SWITCH) {
                        SupportedActions = GetSupportedActions();
                        ActionName = SupportedActions.FirstOrDefault();
                        ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
                    }
                    if (sendCommandDeviceType == DeviceTypeEnum.SWITCH) {
                        SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);
                    }
                    break;

                case ITelescopeVM:
                    if (actionDeviceType == DeviceTypeEnum.TELESCOPE) {
                        SupportedActions = GetSupportedActions();
                        ActionName = SupportedActions.FirstOrDefault();
                        ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
                    }
                    if (sendCommandDeviceType == DeviceTypeEnum.TELESCOPE) {
                        SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);
                    }
                    break;

                case IWeatherDataVM:
                    if (actionDeviceType == DeviceTypeEnum.WEATHERDATA) {
                        SupportedActions = GetSupportedActions();
                        ActionName = SupportedActions.FirstOrDefault();
                        ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
                    }
                    if (sendCommandDeviceType == DeviceTypeEnum.WEATHERDATA) {
                        SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);
                    }
                    break;
            }

            RaiseAllPropertiesChanged();

            return Task.CompletedTask;
        }

        private Task OnDeviceDisconnected(object sender, EventArgs e) {
            SupportedActions = [];
            ActionDeviceConnected = GetDeviceStatus(actionDeviceType);
            SendCommandDeviceConnected = GetDeviceStatus(sendCommandDeviceType);
            RaiseAllPropertiesChanged();

            return Task.CompletedTask;
        }

        #endregion

        public void Dispose() {
            cameraMediator.Connected -= OnDeviceConnected;
            cameraMediator.Disconnected -= OnDeviceDisconnected;
            domeMediator.Connected -= OnDeviceConnected;
            domeMediator.Disconnected -= OnDeviceDisconnected;
            filterWheelMediator.Connected -= OnDeviceConnected;
            filterWheelMediator.Disconnected -= OnDeviceDisconnected;
            flatDeviceMediator.Connected -= OnDeviceConnected;
            flatDeviceMediator.Disconnected -= OnDeviceDisconnected;
            focuserMediator.Connected -= OnDeviceConnected;
            focuserMediator.Disconnected -= OnDeviceDisconnected;
            guiderMediator.Connected -= OnDeviceConnected;
            guiderMediator.Disconnected -= OnDeviceDisconnected;
            rotatorMediator.Connected -= OnDeviceConnected;
            rotatorMediator.Disconnected -= OnDeviceDisconnected;
            safetyMonitorMediator.Connected -= OnDeviceConnected;
            safetyMonitorMediator.Disconnected -= OnDeviceDisconnected;
            switchMediator.Connected -= OnDeviceConnected;
            switchMediator.Disconnected -= OnDeviceDisconnected;
            telescopeMediator.Connected -= OnDeviceConnected;
            telescopeMediator.Disconnected -= OnDeviceDisconnected;
            weatherDataMediator.Connected -= OnDeviceConnected;
            weatherDataMediator.Disconnected -= OnDeviceDisconnected;

            GC.SuppressFinalize(this);
        }
    }
}
