# Device Actions and Commands

This plugin provides Advanced Sequencer instructions to access any [Actions, Blind, Boolean, and String commands](https://ascom-standards.org/platform65rchelp/html/Methods_T_ASCOM_DriverAccess_AscomDriver.htm) that might be implemented in an ASCOM device driver. Any such actions and commands are generally for specific, advanced purposes and are **absolutely not** intended for general or casual use. Please consult with your driver's vendor regarding the proper use and use cases for these entities. This plugin merely provides handy access to these special and highly particular driver facilities.

## Returned data

Methods such as `Action()`, `CommandBool()`, and `CommandString()` that return data will have that data logged in the NINA application log. `CommandBlind()` returns `void` so there will be nothing to log when it returns after being called.

## A note about Action()

Per the [ASCOM specification](https://ascom-standards.org/platform65rchelp/html/M_ASCOM_DriverAccess_AscomDriver_Action.htm), any available action must be listed in the driver's `SupportedActions` property. In practice this may not always be the case. Consult with your vendor if an expected action name is not listed. An action that is not listed is **not** a defect with this plugin.

## Getting help

Help for this plugin may be found in the **#plugin-discussions** channel on the NINA project [Discord chat server](https://discord.gg/nighttime-imaging) or by filing an issue report at this plugin's [Github repository](https://github.com/daleghent/nina-device-commands-actions/issues