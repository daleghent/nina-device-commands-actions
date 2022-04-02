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

    [ExportMetadata("Name", "Device Action")]
    [ExportMetadata("Description", "Execute an advertised action in a driver")]
    [ExportMetadata("Icon", "ActionArrowSVG")]
    [ExportMetadata("Category", "Lbl_SequenceCategory_Utility")]
    [Export(typeof(ISequenceItem))]
    [JsonObject(MemberSerialization.OptIn)]
    public class DeviceActionInstruction : SequenceItem, IValidatable {
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
        public DeviceActionInstruction(ICameraMediator cameraMediator, IDomeMediator domeMediator, IFilterWheelMediator filterWheelMediator, IFlatDeviceMediator flatDeviceMediator,
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

            SupportedActions = UpdateSupportedActions();
        }

        private DeviceActionInstruction(DeviceActionInstruction cloneMe) : this(cameraMediator: cloneMe.cameraMediator, domeMediator: cloneMe.domeMediator, filterWheelMediator: cloneMe.filterWheelMediator,
                                                          flatDeviceMediator: cloneMe.flatDeviceMediator, focuserMediator: cloneMe.focuserMediator, guiderMediator: cloneMe.guiderMediator,
                                                          rotatorMediator: cloneMe.rotatorMediator, safetyMonitorMediator: cloneMe.safetyMonitorMediator, switchMediator: cloneMe.switchMediator,
                                                          telescopeMediator: cloneMe.telescopeMediator, weatherDataMediator: cloneMe.weatherDataMediator) {
            CopyMetaData(cloneMe);
        }

        public override object Clone() {
            return new DeviceActionInstruction(this) {
                DeviceType = DeviceType,
                ActionName = ActionName,
                ActionParameters = ActionParameters,
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

        private int actionName = 0;

        [JsonProperty]
        public int ActionName {
            get => actionName;
            set {
                actionName = value;
                RaisePropertyChanged();
            }
        }

        private string actionParameters = string.Empty;

        [JsonProperty]
        public string ActionParameters {
            get => actionParameters;
            set {
                if (!value.Equals(actionParameters)) {
                    actionParameters = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DeviceTypeEnum deviceType = DeviceTypeEnum.TELESCOPE;

        [JsonProperty]
        public DeviceTypeEnum DeviceType {
            get => deviceType;
            set {
                deviceType = value;
                SupportedActions = UpdateSupportedActions();
                RaisePropertyChanged();
            }
        }

        public DeviceTypeEnum[] DeviceTypes => Enum.GetValues(typeof(DeviceTypeEnum))
            .Cast<DeviceTypeEnum>()
            .ToArray();

        private IList<string> supportedActions = new List<string>();

        public IList<string> SupportedActions {
            get => supportedActions;
            set {
                supportedActions = value;
                RaisePropertyChanged();
            }
        }

        public override Task Execute(IProgress<ApplicationStatus> progress, CancellationToken token) {
            try {
                string output = string.Empty;

                switch (deviceType) {
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
                        throw new SequenceEntityFailedException("Unknown device type");
                }

                Logger.Info($"{deviceType} Action {supportedActions[actionName]}({actionParameters}) returned: {output}");
            } catch (Exception ex) {
                Logger.Error($"{deviceType} Action {supportedActions[actionName]}({actionParameters}) failed: {ex.Message}");
                throw new SequenceEntityFailedException(ex.Message);
            }

            return Task.CompletedTask;
        }

        public bool Validate() {
            var i = new List<string>();

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

            if (supportedActions == null || supportedActions.Count == 0 || supportedActions[0].Equals("--")) {
                i.Add(Loc.Instance["LblNoActionsAvailable"]);
            }

            Issues = i;
            return i.Count == 0;
        }

        public override void AfterParentChanged() {
            Validate();
        }

        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(DeviceActionInstruction)}, Device Type: {deviceType}, Action: {supportedActions[actionName]}, ActionParameters: {actionParameters}";
        }

        private IList<string> UpdateSupportedActions() {
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
    }
}
