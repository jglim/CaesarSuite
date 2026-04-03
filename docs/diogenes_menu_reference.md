# Diogenes Menu Reference

This note documents the current top-bar menus in Diogenes and the purpose of each user-facing option.

The native unlock workflow introduced in this build is inspired by the provider-and-definition architecture used by the public `UnlockECU` project:

- https://github.com/jglim/UnlockECU

Caesar now uses its own in-application unlock menu and provider layer instead of launching the original `UnlockECU` UI.

## File

- `Load CBF Files`
  Loads a `.cbf` container into the current session.
- `Unload Existing Files`
  Clears all loaded containers and rebuilds the tree.
- `Load Compressed JSON`
  Loads a serialized Caesar container from a `.ccb` file.
- `Export Container as Compressed JSON`
  Saves the selected loaded container as a compressed Caesar JSON bundle.
- `Load JSON`
  Loads a serialized Caesar container from a plain `.json` export.
- `Export Container as JSON`
  Saves the selected loaded container as plain JSON.
- `About..`
  Shows the Diogenes and Caesar version dialog.
- `Exit`
  Closes the application.

## Connection

- `J2534 Interfaces`
  Lists detected J2534 devices and selects the active interface.
- `Disconnect`
  Leaves live communication mode and returns the app to simulation mode.
- `Scan CAN Bus...`
  Runs the CAN scanner against the currently selected J2534 device.
  This item is inserted dynamically and is only enabled when a device is open.

## Tools

- `Set Security Level (DLL)`
  Loads a security DLL and uses its exported logic to generate a key for a selected level.
- `Unlock ECU...`
  Opens the native unlock workflow backed by Caesar's internal provider and definition database.
- `Fix Client Access Permissions`
  Applies client-access corrections to a selected CBF container.
- `CFF: Flash Splicer`
  Opens the flash splicer utility for working with CFF content.
- `CFF: Export Flash Segments`
  Extracts memory segments from a selected CFF file.
- `UDS Hex Editor`
  Opens the raw UDS editor for manual read/write experimentation.
- `Diagnostic Trouble Codes (DTC)`
  Opens the DTC viewer for the currently connected and identified variant.
- `View ECU Metadata`
  Shows the current ECU metadata modal for the live connection.
- `Show Trace`
  Opens the live trace window.
- `Copy Console`
  Copies the main log console text to the clipboard.
- `Clear Console`
  Clears the main log console.
- `Identify ECU`
  Hidden by default. Reads and prints the VIN when enabled.
- `Inspect DSC (PAL/CBF)...`
  Parses standalone DSC PAL files or scans CBFs for embedded DSC blobs and disassembly output.
- `Generic Debug Button`
  Debug-only/internal inspection entry point. Not intended for normal end users.
- `List Variant IDs`
  Prints variant qualifiers and their matching IDs for the selected container.
- `Download Blocks`
  Opens the block download tool for manual block-transfer work.
- `Fix CBF Checksum`
  Recomputes and writes the trailing checksum on a selected CBF file.
- `Translate CBF Strings`
  Bulk-translates loaded CBF strings to English and then offers a JSON export.

## Preferences

- `Fingerprint Mode > Use Last Fingerprint`
  Reuses the last observed fingerprint value during supported workflows.
- `Fingerprint Mode > Custom Value`
  Disables clone mode and uses a manually configured fingerprint value.
- `SCN Mode > Use Last SCN`
  Reuses the last observed SCN value.
- `SCN Mode > Write Zeros (Vediamo)`
  Writes zeroed SCN data in the style commonly used by Vediamo-based workflows.
- `Allow Write Variant Coding`
  Enables write-capable variant coding actions. Leave this disabled unless a write operation is intentional.

## Unlock ECU Window

`Tools > Unlock ECU...` opens the native unlock page. Its controls are:

- `Filter`
  Narrows the definition list by ECU name or origin.
- Definition grid
  Shows the matching unlock definitions and their `ECU`, `Origin`, `Level`, `Seed`, `Payload`, and `Provider`.
- `Seed`
  Hex input for the seed returned by the ECU.
- `Paste`
  Pastes seed bytes from the clipboard.
- `Request Seed`
  Sends the seed request for the selected definition on the live connection.
- `Compute`
  Generates the unlock payload from the current seed without sending it.
- `Send Unlock`
  Builds and sends the full unlock request to the ECU.
- `Payload`
  Shows the generated provider payload only.
- `Request`
  Shows the full diagnostic request that will be sent for unlock.
- `Copy` beside `Payload`
  Copies the payload bytes.
- `Copy` beside `Request`
  Copies the full unlock request bytes.
- `Response`
  Shows the last ECU response from seed or unlock traffic.
- `Definition`
  Displays the details and parameters of the selected unlock definition.
- `Close`
  Closes the unlock window.

## Tree Shortcuts And Labels

- Diagnostic action nodes now show `[CAL x, SAL y]` to surface client-access and security-access requirements directly in the tree.
- `Ctrl+T` on a selected tree node translates the best available human-readable CBF text into English for that node.

## Notes For Testers

- `Unlock ECU...` is now the preferred path for in-app seed/key work.
- The older single-ECU seed/key helper was removed because its responsibilities are now covered by the shared unlock architecture.
- Some menu items remain diagnostic or reverse-engineering tools; they are documented here so outside testers can tell which entries are production-facing and which are investigative.
