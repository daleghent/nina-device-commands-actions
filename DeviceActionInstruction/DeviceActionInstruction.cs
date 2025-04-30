#region "copyright"

/*
    Copyright Dale Ghent <daleg@elemental.org>
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/
*/

#endregion "copyright"

using CommunityToolkit.Mvvm.Input;
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
    public partial class DeviceActionInstruction : SequenceItem, IValidatable, IDisposable {
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
                SupportedActions = SupportedActions,
            };
        }

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

        private DeviceTypeEnum deviceType = DeviceTypeEnum.TELESCOPE;

        [JsonProperty]
        public DeviceTypeEnum DeviceType {
            get => deviceType;
            set {
                deviceType = value;
                RaiseAllPropertiesChanged();
                Validate();
            }
        }

        private IList<string> issues = [];

        public IList<string> Issues {
            get => issues;
            set {
                issues = value;
                RaisePropertyChanged();
            }
        }

        private string actionName = string.Empty;

        [JsonProperty]
        public string ActionName {
            get => actionName;
            set {
                if (string.IsNullOrEmpty(value)) {
                    return;
                }
                actionName = value;
                Validate();
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

        private AsyncObservableCollection<string> supportedActions = [];

        [JsonProperty]
        public AsyncObservableCollection<string> SupportedActions {
            get => supportedActions;
            set {
                supportedActions = value;
                RaisePropertyChanged();
            }
        }

        public static DeviceTypeEnum[] DeviceTypes => Enum.GetValues(typeof(DeviceTypeEnum))
            .Cast<DeviceTypeEnum>()
            .ToArray();


        private bool hasActions = false;

        public bool HasActions {
            get => hasActions;
            set {
                if (hasActions != value) {
                    hasActions = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool enableRefresh = false;

        public bool EnableRefresh {
            get => enableRefresh;
            set {
                if (enableRefresh != value) {
                    enableRefresh = value;
                    RaisePropertyChanged();
                }
            }
        }

        public override Task Execute(IProgress<ApplicationStatus> progress, CancellationToken token) {
            try {
                var output = string.Empty;

                output = deviceType switch {
                    DeviceTypeEnum.CAMERA => cameraMediator.Action(actionName, actionParameters),
                    DeviceTypeEnum.DOME => domeMediator.Action(actionName, actionParameters),
                    DeviceTypeEnum.FILTERWHEEL => filterWheelMediator.Action(actionName, actionParameters),
                    DeviceTypeEnum.FLATDEVICE => flatDeviceMediator.Action(actionName, actionParameters),
                    DeviceTypeEnum.FOCUSER => focuserMediator.Action(actionName, actionParameters),
                    DeviceTypeEnum.GUIDER => guiderMediator.Action(actionName, actionParameters),
                    DeviceTypeEnum.ROTATOR => rotatorMediator.Action(actionName, actionParameters),
                    DeviceTypeEnum.SAFETYMONITOR => safetyMonitorMediator.Action(actionName, actionParameters),
                    DeviceTypeEnum.SWITCH => switchMediator.Action(actionName, actionParameters),
                    DeviceTypeEnum.TELESCOPE => telescopeMediator.Action(actionName, actionParameters),
                    DeviceTypeEnum.WEATHERDATA => weatherDataMediator.Action(actionName, actionParameters),
                    _ => throw new SequenceEntityFailedException("Unknown device type for Action"),
                };
                Logger.Info($"{deviceType} Action {actionName}({actionParameters}) returned: {output}");
            } catch (Exception ex) {
                Logger.Error($"{deviceType} Action {actionName}({actionParameters}) failed: {ex.Message}");
                throw new SequenceEntityFailedException(ex.Message);
            }

            return Task.CompletedTask;
        }

        public bool Validate() {
            var i = new List<string>();

            IList<string> actionsList = [];
            var connected = false;

            switch (deviceType) {
                case DeviceTypeEnum.CAMERA:
                    if (!cameraMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblCameraNotConnected"]);
                        goto end;
                    } else {
                        connected = true;
                        actionsList = cameraMediator?.GetInfo().SupportedActions ?? [];
                    }
                    break;

                case DeviceTypeEnum.DOME:
                    if (!domeMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblDomeNotConnected"]);
                        goto end;
                    } else {
                        connected = true;
                        actionsList = domeMediator?.GetInfo().SupportedActions ?? [];
                    }
                    break;

                case DeviceTypeEnum.FILTERWHEEL:
                    if (!filterWheelMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblFilterWheelNotConnected"]);
                        goto end;
                    } else {
                        connected = true;
                        actionsList = filterWheelMediator?.GetInfo().SupportedActions ?? [];
                    }
                    break;

                case DeviceTypeEnum.FLATDEVICE:
                    if (!flatDeviceMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblFlatDeviceNotConnected"]);
                        goto end;
                    } else {
                        connected = true;
                        actionsList = flatDeviceMediator?.GetInfo().SupportedActions ?? [];
                    }
                    break;

                case DeviceTypeEnum.FOCUSER:
                    if (!focuserMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblFocuserNotConnected"]);
                        goto end;
                    } else {
                        connected = true;
                        actionsList = focuserMediator?.GetInfo().SupportedActions ?? [];
                    }
                    break;

                case DeviceTypeEnum.GUIDER:
                    if (!guiderMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblGuiderNotConnected"]);
                        goto end;
                    } else {
                        connected = true;
                        actionsList = guiderMediator?.GetInfo().SupportedActions ?? [];
                    }
                    break;

                case DeviceTypeEnum.ROTATOR:
                    if (!rotatorMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblRotatorNotConnected"]);
                        goto end;
                    } else {
                        connected = true;
                        actionsList = rotatorMediator?.GetInfo().SupportedActions ?? [];
                    }
                    break;

                case DeviceTypeEnum.SAFETYMONITOR:
                    if (!safetyMonitorMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblSafetyMonitorNotConnected"]);
                        goto end;
                    } else {
                        connected = true;
                        actionsList = safetyMonitorMediator?.GetInfo().SupportedActions ?? [];
                    }
                    break;

                case DeviceTypeEnum.SWITCH:
                    if (!switchMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblSwitchNotConnected"]);
                        goto end;
                    } else {
                        connected = true;
                        actionsList = switchMediator?.GetInfo().SupportedActions ?? [];
                    }
                    break;

                case DeviceTypeEnum.TELESCOPE:
                    if (!telescopeMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblTelescopeNotConnected"]);
                        goto end;
                    } else {
                        connected = true;
                        actionsList = telescopeMediator?.GetInfo().SupportedActions ?? [];
                    }
                    break;

                case DeviceTypeEnum.WEATHERDATA:
                    if (!weatherDataMediator.GetInfo().Connected) {
                        i.Add(Loc.Instance["LblWeatherNoSource"]);
                        goto end;
                    } else {
                        connected = true;
                        actionsList = weatherDataMediator?.GetInfo().SupportedActions ?? [];
                    }
                    break;
            }

            if (!actionsList.Contains(ActionName)) {
                i.Add($"{ActionName} is not an available driver action");
                goto end;
            }

            if (supportedActions == null || supportedActions.Count == 0) {
                i.Add(Loc.Instance["LblNoActionsAvailable"]);
                goto end;
            }

            if (supportedActions.Count > 0 && string.IsNullOrEmpty(actionName)) {
                i.Add("The action name to use is not selected");
            }

        end:
            Issues = i;
            var passed = i.Count == 0;

            HasActions = actionsList.Count > 0;
            EnableRefresh = connected;

            return passed;
        }

        public override string ToString() {
            return $"Category: {Category}, Item: {Name}, Device Type: {deviceType}, Action: {actionName}, ActionParameters: {actionParameters}";
        }

        private AsyncObservableCollection<string> GetSupportedActions() {
            IList<string> actions = [];

            switch (deviceType) {
                case DeviceTypeEnum.CAMERA:
                    if (cameraMediator.GetInfo().Connected) {
                        actions = cameraMediator.GetInfo().SupportedActions;
                    } else {
                        return supportedActions;
                    }
                    break;

                case DeviceTypeEnum.DOME:
                    if (domeMediator.GetInfo().Connected) {
                        actions = domeMediator.GetInfo().SupportedActions;
                    } else {
                        return supportedActions;
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
                    } else {
                        return supportedActions;
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
                    } else {
                        return supportedActions;
                    }
                    break;

                case DeviceTypeEnum.ROTATOR:
                    if (rotatorMediator.GetInfo().Connected) {
                        actions = rotatorMediator.GetInfo().SupportedActions;
                    } else {
                        return supportedActions;
                    }
                    break;

                case DeviceTypeEnum.SAFETYMONITOR:
                    if (safetyMonitorMediator.GetInfo().Connected) {
                        actions = safetyMonitorMediator.GetInfo().SupportedActions;
                    } else {
                        return supportedActions;
                    }
                    break;

                case DeviceTypeEnum.SWITCH:
                    if (switchMediator.GetInfo().Connected) {
                        actions = switchMediator.GetInfo().SupportedActions;
                    } else {
                        return supportedActions;
                    }
                    break;

                case DeviceTypeEnum.TELESCOPE:
                    if (telescopeMediator.GetInfo().Connected) {
                        actions = telescopeMediator.GetInfo().SupportedActions;
                    } else {
                        return supportedActions;
                    }
                    break;

                case DeviceTypeEnum.WEATHERDATA:
                    if (weatherDataMediator.GetInfo().Connected) {
                        actions = weatherDataMediator.GetInfo().SupportedActions;
                    } else {
                        return supportedActions;
                    }
                    break;
            }

            var actionsList = new AsyncObservableCollection<string>();
            foreach (var action in actions) {
                actionsList.Add(action);
            }

            HasActions = actionsList.Count > 0;

            return actionsList;
        }

        [RelayCommand]
        private async Task RefreshActions(object o) {
            SupportedActions = GetSupportedActions();

            if (!SupportedActions.Contains(ActionName) && SupportedActions.Any()) {
                ActionName = SupportedActions.First();
                ActionParameters = string.Empty;
            }

            await Task.CompletedTask;
        }

        private Task OnDeviceConnected(object sender, EventArgs e) {
            Validate();
            return Task.CompletedTask;
        }

        private Task OnDeviceDisconnected(object sender, EventArgs e) {
            Validate();
            return Task.CompletedTask;
        }
    }
}
