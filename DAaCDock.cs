#region "copyright"

/*
    Copyright Dale Ghent <daleg@elemental.org>
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/
*/

#endregion "copyright"

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
using System.Text;
using System.Threading.Tasks;

namespace DaleGhent.NINA.DeviceActionsCommands {

    [Export(typeof(IDockableVM))]
    public class DAaCDock : DockableVM {
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

            Title = "Device Actions and Commands";

            RunActionCommand = new AsyncCommand<bool>(() => DoRunActionCommand());
            RunSendCommand = new AsyncCommand<bool>(() => DoRunSendCommand());

            UpdateSupportedActions(DeviceTypeEnum.TELESCOPE);
            ActionDeviceStatus = GetDeviceStatus(DeviceTypeEnum.TELESCOPE);
        }

        public override bool IsTool => true;

        private DeviceTypeEnum actionDeviceType = DeviceTypeEnum.TELESCOPE;

        public DeviceTypeEnum[] DeviceTypes => Enum.GetValues(typeof(DeviceTypeEnum))
            .Cast<DeviceTypeEnum>()
            .ToArray();

        public bool GetDeviceStatus(DeviceTypeEnum deviceType) {
            switch (deviceType) {
                case DeviceTypeEnum.CAMERA:
                    return cameraMediator.GetInfo().Connected;

                case DeviceTypeEnum.DOME:
                    return domeMediator.GetInfo().Connected;

                case DeviceTypeEnum.FILTERWHEEL:
                    return filterWheelMediator.GetInfo().Connected;

                case DeviceTypeEnum.FLATDEVICE:
                    return flatDeviceMediator.GetInfo().Connected;

                case DeviceTypeEnum.FOCUSER:
                    return focuserMediator.GetInfo().Connected;

                case DeviceTypeEnum.GUIDER:
                    return guiderMediator.GetInfo().Connected;

                case DeviceTypeEnum.ROTATOR:
                    return rotatorMediator.GetInfo().Connected;

                case DeviceTypeEnum.SAFETYMONITOR:
                    return safetyMonitorMediator.GetInfo().Connected;

                case DeviceTypeEnum.SWITCH:
                    return switchMediator.GetInfo().Connected;

                case DeviceTypeEnum.TELESCOPE:
                    return telescopeMediator.GetInfo().Connected;

                case DeviceTypeEnum.WEATHERDATA:
                    return weatherDataMediator.GetInfo().Connected;

                default:
                    return false;
            }
        }

        #region Device Actions

        private bool actionDeviceStatus = false;

        public bool ActionDeviceStatus {
            get => actionDeviceStatus;
            private set {
                actionDeviceStatus = value;
                RaisePropertyChanged();
            }
        }

        public DeviceTypeEnum ActionDeviceType {
            get => actionDeviceType;
            set {
                actionDeviceType = value;
                ActionDeviceStatus = GetDeviceStatus(actionDeviceType);

                if (ActionDeviceStatus) {
                    SupportedActions = UpdateSupportedActions(actionDeviceType);

                    if (SupportedActions.Any()) {
                        ActionName = 0;
                    }
                } else {
                    SupportedActions = new List<string>();
                }

                RaisePropertyChanged();
            }
        }

        private IList<string> supportedActions = new List<string>();

        public IList<string> SupportedActions {
            get => supportedActions;
            set {
                supportedActions = value;
                RaisePropertyChanged();
            }
        }

        private int actionName = 0;

        public int ActionName {
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

        private IList<string> UpdateSupportedActions(DeviceTypeEnum deviceType) {
            switch (deviceType) {
                case DeviceTypeEnum.CAMERA:
                    return cameraMediator.GetInfo().SupportedActions;

                case DeviceTypeEnum.DOME:
                    return domeMediator.GetInfo().SupportedActions;

                case DeviceTypeEnum.FILTERWHEEL:
                    return filterWheelMediator.GetInfo().SupportedActions;

                case DeviceTypeEnum.FLATDEVICE:
                    return flatDeviceMediator.GetInfo().SupportedActions;

                case DeviceTypeEnum.FOCUSER:
                    return focuserMediator.GetInfo().SupportedActions;

                case DeviceTypeEnum.GUIDER:
                    return guiderMediator.GetInfo().SupportedActions;

                case DeviceTypeEnum.ROTATOR:
                    return rotatorMediator.GetInfo().SupportedActions;

                case DeviceTypeEnum.SAFETYMONITOR:
                    return safetyMonitorMediator.GetInfo().SupportedActions;

                case DeviceTypeEnum.SWITCH:
                    return switchMediator.GetInfo().SupportedActions;

                case DeviceTypeEnum.TELESCOPE:
                    return telescopeMediator.GetInfo().SupportedActions;

                case DeviceTypeEnum.WEATHERDATA:
                    return weatherDataMediator.GetInfo().SupportedActions;
            }

            return new List<string>();
        }

        public IAsyncCommand RunActionCommand { get; }

        private async Task<bool> DoRunActionCommand() {
            await Task.Run(() => {
                string output = string.Empty;

                try {
                    switch (actionDeviceType) {
                        case DeviceTypeEnum.CAMERA:
                            output = cameraMediator.Action(supportedActions[actionName], actionParameters);
                            break;

                        case DeviceTypeEnum.DOME:
                            output = domeMediator.Action(supportedActions[actionName], actionParameters);
                            break;

                        case DeviceTypeEnum.FILTERWHEEL:
                            output = filterWheelMediator.Action(supportedActions[actionName], actionParameters);
                            break;

                        case DeviceTypeEnum.FLATDEVICE:
                            output = flatDeviceMediator.Action(supportedActions[actionName], actionParameters);
                            break;

                        case DeviceTypeEnum.FOCUSER:
                            output = focuserMediator.Action(supportedActions[actionName], actionParameters);
                            break;

                        case DeviceTypeEnum.GUIDER:
                            output = guiderMediator.Action(supportedActions[actionName], actionParameters);
                            break;

                        case DeviceTypeEnum.ROTATOR:
                            output = rotatorMediator.Action(supportedActions[actionName], actionParameters);
                            break;

                        case DeviceTypeEnum.SAFETYMONITOR:
                            output = safetyMonitorMediator.Action(supportedActions[actionName], actionParameters);
                            break;

                        case DeviceTypeEnum.SWITCH:
                            output = switchMediator.Action(supportedActions[actionName], actionParameters);
                            break;

                        case DeviceTypeEnum.TELESCOPE:
                            output = telescopeMediator.Action(supportedActions[actionName], actionParameters);
                            break;

                        case DeviceTypeEnum.WEATHERDATA:
                            output = weatherDataMediator.Action(supportedActions[actionName], actionParameters);
                            break;

                        default:
                            return false;
                    }

                    ActionOutput = output;
                    Logger.Info($"{actionDeviceType} Action {supportedActions[actionName]}({actionParameters}) returned: {output}");
                } catch (Exception ex) {
                    Logger.Error($"{actionDeviceType} Action {supportedActions[actionName]}({actionParameters}) failed: {ex.Message}");
                    Notification.ShowExternalError(ex.Message, "ASCOM driver");
                    return false;
                }

                return true;
            });

            return true;
        }

        #endregion

        #region SendCommand

        private bool sendCommandDeviceStatus = false;

        public bool SendCommandDeviceStatus {
            get => sendCommandDeviceStatus;
            private set {
                sendCommandDeviceStatus = value;
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

        private DeviceTypeEnum sendCommandDeviceType = DeviceTypeEnum.TELESCOPE;

        public DeviceTypeEnum SendCommandDeviceType {
            get => sendCommandDeviceType;
            set {
                sendCommandDeviceType = value;
                SendCommandDeviceStatus = GetDeviceStatus(sendCommandDeviceType);
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

        public SendCommandTypeEnum[] SendCommandTypes => Enum.GetValues(typeof(SendCommandTypeEnum))
            .Cast<SendCommandTypeEnum>().ToArray();

        private string sendCommandOutput = string.Empty;

        public string SendCommandOutput {
            get => sendCommandOutput;
            private set {
                sendCommandOutput = value;
                RaisePropertyChanged();
            }
        }

        public IAsyncCommand RunSendCommand { get; }

        private async Task<bool> DoRunSendCommand() {
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

        #endregion

        public void Dispose() {
        }
    }
}
