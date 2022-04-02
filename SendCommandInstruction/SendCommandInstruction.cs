#region "copyright"

/*
    Copyright Dale Ghent <daleg@elemental.org>
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/
*/

#endregion "copyright"

using Newtonsoft.Json;
using NINA.Core.Enum;
using NINA.Core.Locale;
using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.Equipment.Interfaces.Mediator;
using NINA.Sequencer.SequenceItem;
using NINA.Sequencer.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaleGhent.NINA.DeviceActionsCommands {

    [ExportMetadata("Name", "Send Command")]
    [ExportMetadata("Description", "Send a command to a driver's CommandBlind, CommandBool, or CommandString interface")]
    [ExportMetadata("Icon", "CommandPromptSVG")]
    [ExportMetadata("Category", "Lbl_SequenceCategory_Utility")]
    [Export(typeof(ISequenceItem))]
    [JsonObject(MemberSerialization.OptIn)]
    public class SendCommandInstruction : SequenceItem, IValidatable {
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
        public SendCommandInstruction(ICameraMediator cameraMediator, IDomeMediator domeMediator, IFilterWheelMediator filterWheelMediator, IFlatDeviceMediator flatDeviceMediator,
                            IFocuserMediator focuserMediator, IGuiderMediator guiderMediator, IRotatorMediator rotatorMediator, ISafetyMonitorMediator safetyMonitorMediator,
                            ISwitchMediator switchMediator, ITelescopeMediator telescopeMediator, IWeatherDataMediator weatherDataMediator) {
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
        }

        private SendCommandInstruction(SendCommandInstruction cloneMe) : this(cameraMediator: cloneMe.cameraMediator, domeMediator: cloneMe.domeMediator, filterWheelMediator: cloneMe.filterWheelMediator,
                                                          flatDeviceMediator: cloneMe.flatDeviceMediator, focuserMediator: cloneMe.focuserMediator, guiderMediator: cloneMe.guiderMediator,
                                                          rotatorMediator: cloneMe.rotatorMediator, safetyMonitorMediator: cloneMe.safetyMonitorMediator, switchMediator: cloneMe.switchMediator,
                                                          telescopeMediator: cloneMe.telescopeMediator, weatherDataMediator: cloneMe.weatherDataMediator) {
            CopyMetaData(cloneMe);
        }

        public override object Clone() {
            return new SendCommandInstruction(this) {
                SendCommandType = SendCommandType,
                DeviceType = DeviceType,
                Command = Command,
                Raw = Raw,
            };
        }

        private IList<string> issues = new List<string>();

        public IList<string> Issues {
            get => issues;
            set {
                issues = value;
                RaisePropertyChanged();
            }
        }

        private string command = string.Empty;

        [JsonProperty]
        public string Command {
            get => command;
            set {
                if (!value.Equals(command)) {
                    command = value;
                    RaisePropertyChanged();
                    Validate();
                }
            }
        }

        private bool raw = true;

        [JsonProperty]
        public bool Raw {
            get => raw;
            set {
                raw = value;
                RaisePropertyChanged();
            }
        }

        private DeviceTypeEnum deviceType = DeviceTypeEnum.TELESCOPE;

        [JsonProperty]
        public DeviceTypeEnum DeviceType {
            get => deviceType;
            set {
                deviceType = value;
                RaisePropertyChanged();
                Validate();
            }
        }

        private SendCommandTypeEnum sendCommandType = SendCommandTypeEnum.STRING;

        [JsonProperty]
        public SendCommandTypeEnum SendCommandType {
            get => sendCommandType;
            set {
                sendCommandType = value;
                RaisePropertyChanged();
            }
        }

        public DeviceTypeEnum[] DeviceTypes => Enum.GetValues(typeof(DeviceTypeEnum))
            .Cast<DeviceTypeEnum>()
            .ToArray();

        public SendCommandTypeEnum[] SendCommandTypes => Enum.GetValues(typeof(SendCommandTypeEnum))
            .Cast<SendCommandTypeEnum>().ToArray();

        public override Task Execute(IProgress<ApplicationStatus> progress, CancellationToken token) {
            try {
                object output = null;

                switch (deviceType) {
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
                    Logger.Info($"{deviceType} SendCommand {command} returned: {output}");
                }
            } catch (Exception ex) {
                Logger.Error($"{deviceType} SendCommand {command} failed: {ex.Message}");
                throw new SequenceEntityFailedException(ex.Message);
            }

            return Task.CompletedTask;
        }

        public bool Validate() {
            var i = new List<string>();

            if (string.IsNullOrEmpty(command) || string.IsNullOrWhiteSpace(command)) {
                i.Add("No command has been provided");
            }

            switch (deviceType) {
                case DeviceTypeEnum.CAMERA:
                    if (!cameraMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblCameraNotConnected"]);
                    }
                    break;

                case DeviceTypeEnum.DOME:
                    if (!domeMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblDomeNotConnected"]);
                    }
                    break;

                case DeviceTypeEnum.FILTERWHEEL:
                    if (!filterWheelMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblFilterWheelNotConnected"]);
                    }
                    break;

                case DeviceTypeEnum.FLATDEVICE:
                    if (!flatDeviceMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblFlatDeviceNotConnected"]);
                    }
                    break;

                case DeviceTypeEnum.FOCUSER:
                    if (!focuserMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblFocuserNotConnected"]);
                    }
                    break;

                case DeviceTypeEnum.GUIDER:
                    if (!guiderMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblGuiderNotConnected"]);
                    }
                    break;

                case DeviceTypeEnum.ROTATOR:
                    if (!rotatorMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblRotatorNotConnected"]);
                    }
                    break;

                case DeviceTypeEnum.SAFETYMONITOR:
                    if (!safetyMonitorMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblSafetyMonitorNotConnected"]);
                    }
                    break;

                case DeviceTypeEnum.SWITCH:
                    if (!switchMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblSwitchNotConnected"]);
                    }
                    break;

                case DeviceTypeEnum.TELESCOPE:
                    if (!telescopeMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblTelescopeNotConnected"]);
                    }
                    break;

                case DeviceTypeEnum.WEATHERDATA:
                    if (!weatherDataMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblWeatherNoSource"]);
                    }
                    break;
            }

            Issues = i;
            return i.Count == 0;
        }

        public override void AfterParentChanged() {
            Validate();
        }

        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(SendCommandInstruction)}, Device Type: {DeviceType}, SendCommand Type: {SendCommandType}, Command: {Command}, Raw: {Raw}";
        }
    }
}
