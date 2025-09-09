# Device Actions and Commands

## 2.1.0.0 - 2025-09-08
* Implemented dynamic refresh and activation of actions upon device status change (connect or disconnect).
* The list of available actions in the dockable Imaging screen window will now update automatically when the selected device changes state.
* The **Device Action** instruction for sequences will save the last selected action and preserve it as part of the sequence or template it is present in. A "Refresh Actions" button has been added to it so that the list of available actions can be refreshed if the user desires to change the device type or if a new version of the underlying device driver adds (or removes) an action.

## 2.0.0.0 - 2022-11-12
* Updated plugin to Microsoft .NET 7 for compatibility with NINA 3.0. The version of Device Actions and Commands that is compatible with NINA 2.x will remain under the 1.x versioning scheme, and Device Actions and Commands 2.x and later is relvant only to NINA 3.x and later.

## 1.0.0.0 - 2022-04-02
* Initial Release
* Added **Device Action** instruction
* Added **Send Command** instruction
* Added tabbed Imaging screen **Device Actions and Commands** anchorable window
