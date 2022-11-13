using System.Reflection;
using System.Runtime.InteropServices;

// [MANDATORY] The following GUID is used as a unique identifier of the plugin
[assembly: Guid("1a1405f2-ca4e-49e0-9482-725b730b8d0c")]

// [MANDATORY] The assembly versioning
//Should be incremented for each new release build of a plugin
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]

// [MANDATORY] The name of your plugin
[assembly: AssemblyTitle("Device Actions and Commands")]
// [MANDATORY] A short description of your plugin
[assembly: AssemblyDescription("Sequence instructions for accessing any available ASCOM (and perhaps other) driver actions and raw commands")]

// The following attributes are not required for the plugin per se, but are required by the official manifest meta data

// Your name
[assembly: AssemblyCompany("Dale Ghent")]
// The product name that this plugin is part of
[assembly: AssemblyProduct("Device Actions and Commands")]
[assembly: AssemblyCopyright("Copyright © 2022 Dale Ghent")]

// The minimum Version of N.I.N.A. that this plugin is compatible with
[assembly: AssemblyMetadata("MinimumApplicationVersion", "3.0.0.1001")]

// The license your plugin code is using
[assembly: AssemblyMetadata("License", "MPL-2.0")]
// The url to the license
[assembly: AssemblyMetadata("LicenseURL", "https://www.mozilla.org/en-US/MPL/2.0/")]
// The repository where your pluggin is hosted
[assembly: AssemblyMetadata("Repository", "https://github.com/daleghent/nina-device-commands-actions")]

// The following attributes are optional for the official manifest meta data

//[Optional] Your plugin homepage - omit if not applicaple
//[assembly: AssemblyMetadata("Homepage", "https://daleghent.com/moon-angle")]

//[Optional] Common tags that quickly describe your plugin
[assembly: AssemblyMetadata("Tags", "ASCOM, action, commandblind, commandbool, commandstring")]

//[Optional] A link that will show a log of all changes in between your plugin's versions
[assembly: AssemblyMetadata("ChangelogURL", "https://github.com/daleghent/nina-device-commands-actions/blob/main/CHANGELOG.md")]

//[Optional] The url to a featured logo that will be displayed in the plugin list next to the name
//[assembly: AssemblyMetadata("FeaturedImageURL", "https://daleghent.github.io/nina-plugins/assets/images/daac-logo.png")]
//[Optional] A url to an example screenshot of your plugin in action
[assembly: AssemblyMetadata("ScreenshotURL", "https://daleghent.github.io/nina-plugins/assets/images/daac-screen1.png")]
//[Optional] An additional url to an example example screenshot of your plugin in action
[assembly: AssemblyMetadata("AltScreenshotURL", "")]
//[Optional] An in-depth description of your plugin
[assembly: AssemblyMetadata("LongDescription", @"# Device Actions and Commands

This plugin provides Advanced Sequencer instructions and an Imaging screen window to access any [Actions, Blind, Boolean, and String commands](https://ascom-standards.org/platform65rchelp/html/Methods_T_ASCOM_DriverAccess_AscomDriver.htm) that might be implemented in an ASCOM device driver. Any such actions and commands are generally for specific, advanced purposes and are **absolutely not** intended for general or casual use. Please consult with your driver's vendor regarding the proper use and use cases for these entities. This plugin merely provides handy access to these special and highly particular driver facilities.

## Returned data

Methods such as `Action()`, `CommandBool()`, and `CommandString()` that return data will have that data logged in the NINA application log. `CommandBlind()` returns `void` so there will be nothing to log when it returns after being called.

## A note about Action()

Per the [ASCOM specification](https://ascom-standards.org/platform65rchelp/html/M_ASCOM_DriverAccess_AscomDriver_Action.htm), any available action must be listed in the driver's `SupportedActions` property. In practice this may not always be the case. Consult with your vendor if an expected action name is not listed. An action that is not listed is **not** a defect with this plugin.

## Getting help

Help for this plugin may be found in the **#plugin-discussions** channel on the NINA project [Discord chat server](https://discord.gg/nighttime-imaging) or by filing an issue report at this plugin's [Github repository](https://github.com/daleghent/nina-device-commands-actions/issues)")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
// [Unused]
[assembly: AssemblyConfiguration("")]
// [Unused]
[assembly: AssemblyTrademark("")]
// [Unused]
[assembly: AssemblyCulture("")]
