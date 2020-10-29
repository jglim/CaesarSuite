# Caesar Suite

![Counter](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/gh_shield_0.svg)

Library and applications to work with Dаіmlеr diagnostics CBF files.

- Caesar : Library to operate on CBF files
- Trafo: Transforms CBF files into JSON
- Diogenes: ECU-Coding utility

    ![Header Image](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/diogenes-1.png)

## Warning

This project is far from production ready. Most of this is untested, and Diogenes will not connect with a vehicle at this time.

Please be careful if you are planning to use this with your vehicle.

## Getting Started

Builds are available on the [Releases page](https://github.com/jglim/CaesarSuite/releases/) for the adventurous.

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

Diogenes will be "complete" when it can:

- Open a UDS connection over J2534
- Enumerate connected ECUs and identify online devices with compatible CBF files
- Complete a seed-key challenge with a target ECU
- Write a new variant-coding string on a target ECU

*If you are looking for an alternative with a broader feature set, check out [OpenVehicleDiag](https://github.com/rnd-ash/OpenVehicleDiag) instead*

---

# Contributing

## Tests

I am unable to test the accuracy of the variant-coding computation. Help in this aspect is very welcome:

- Connect to your vehicle with Vediamo/Monaco and read out the variant-coding string from a target ECU
- In Diogenes, load the same CBF file for the target ECU, and check if the variant-coding output differs from Vediamo/Monaco
- Open an issue here with the results (even for successful 100% matches), screenshots will be very helpful too. Please indicate the ECU name and CBF filename in the issue.
- Thanks in advance :^)

## Reverse engineering c32s.dll

The Caesar library implementation originates from my interpretation of the  `c32s.dll` library that was provided by [@rnd-ash](https://github.com/rnd-ash), and may be a good place to start if you are looking to understand about how CBF files are parsed.