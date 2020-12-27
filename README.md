![CaesarSuite Banner](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/caesarsuite-banner.png)

# Caesar Suite

![Counter](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/gh_shield_0.svg)

Library and applications to work with Dаіmlеr diagnostics CBF files.

- Caesar : Library to operate on CBF files
- Trafo: Transforms CBF files into JSON
- Diogenes: ECU-Coding utility

![Header Image](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/diogenes-2.png)

## Warning

This project is far from production ready. Please be careful if you are planning to use this with your vehicle.

## Getting Started

Builds are available on the [Releases page](https://github.com/jglim/CaesarSuite/releases/) for the adventurous. Ensure that [.NET Framework 4.6 or newer](https://dotnet.microsoft.com/download) is installed. Optionally install .NET 5 too, if you intend to use [UnlockECU](https://github.com/jglim/UnlockECU).

## License

MIT

Icons from [http://www.famfamfam.com/lab/icons/silk/](http://www.famfamfam.com/lab/icons/silk/)

---

# Caesar

Caesar is an experimental parser for CBF files that is reverse engineered from the official, proprietary `c32s.dll` library. 

Names of classes, functions, properties will change often as they are matched with their disassembly's names. Please be mindful of this when referencing the Caesar library.

# Trafo

Trafo converts a CBF file into a simplified JSON representation and is backed by Caesar. At this point, the JSON should contain enough information to work with variant-coding strings. Usage is straightforward: drop a CBF file onto `Trafo.exe` and a JSON file will be created in the same folder as the input file.

# Diogenes

Diogenes is intended to be a FOSS replacement for Vediamo's variant-coding capabilities over J2534 interfaces. 

At this point, Diogenes can: 

- Load CBF files and display ECUs and their variants
- Parse a variant's VC domain
- Parse, visually modify and reinterpret a variant-coding string
- Generate authentication seed-keys from standalone DLLS (borrowed from my [SecurityAccessQuery](https://github.com/jglim/SecurityAccessQuery))

(New:)
- Open a UDS connection over J2534 (✅)
- Enumerate connected ECUs and identify online devices with compatible CBF files (✅ : UDS)
- Complete a seed-key challenge with a target ECU (✅ : Paired with [UnlockECU](https://github.com/jglim/UnlockECU))
- Write a new variant-coding string on a target ECU (✅* : Experimental, reuses fingerprints, still needs more testing)

Diogenes will be "complete" when it can:

- ❌: Test on an ECU that is installed in a vehicle. So far, tests have been made on a real ECU, but only on the bench.

*If you are looking for an alternative with a broader feature set, check out [OpenVehicleDiag](https://github.com/rnd-ash/OpenVehicleDiag) instead*

# Demo

## Establishing a connection

![Connection](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/demo-connect.gif)

## Reading ECU data

![Read Data](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/demo-data.gif)

## Variant coding

![VC](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/demo-vc.gif)

_More information on variant coding in [this discussion](https://github.com/jglim/CaesarSuite/discussions/7)_

## CFF flash export

_Dump a CFF flash file's raw memory contents (assumes unencrypted, uncompressed data). Files are labeled with its intended memory address (IDA: File -> Load file -> Additional binary file)_

![CFF](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/demo-cff.gif)

## (New!) CFF Splicer

_Modify a CFF flash file's memory segment data (assumes unencrypted, uncompressed data). New segment data can be of different file sizes, and the destination ECU memory address can be changed._

![CFF2](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/demo-cff-splice.gif)

---

# Contributing

## Issues

The primary roadblock for Caesar/Diogenes is the inability to parse Diagnostic Service code blobs, which are compiled binary scripts that automate tasks such as session switching, variant identification, ECU unlocking, and (apparently) variant coding. While Diogenes is usable on UDS, it still falls short for protocols like KW2C3PE because of this issue.

### Workaround

 - Session switching and variant identification on UDS devices is generally consistent and standardized.
 - ECU unlocking can be deferred to [UnlockECU](https://github.com/jglim/UnlockECU). 
 - Variant coding typically contains 4 bytes for the signature of the last system that modified it. Diogenes will automatically clone this value. 
 	- By default, the 4 bytes contain seemingly random data (`00 40 33 10`) on a "fresh" ECU, and become `00 00 00 01` when written with Vediamo.
 	- Some devices also include the SCN as part of the variant coding, which appears as an additional 128-bit/16-byte field. 
 	- Diogenes will detect "unfilled" values, and prompt for the next course of action.

### My lack of understanding

There are concepts which I do not understand that may impede the development of this project:

 - The SCN seems to be a specific 16-byte string, where the first 10 characters are the part number (as printed on the device), and an unknown 6 characters. Are these 6 characters unique to the device?
 - The 16-character string is typically queried via `DT_STO_ID_Calibration_Identification`. There is also a function for `DT_STO_ID_Calibration_Verification_Number` which returns an unknown 4 bytes. Are these values matched?
 - Is there a way to query the ECU if the current SCN is valid and accepted? This is crucial to check if the variant-coding was successful.


# Contributors

These individuals have helped to improve Diogenes in some form (e.g. code, testing, traces/logs, knowledge sharing, assets).

- @N0cynym
- @Feezex
- @rnd-ash

Thank you for your contributions.