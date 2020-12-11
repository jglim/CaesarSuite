# Caesar Suite

![Counter](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/gh_shield_0.svg)

Library and applications to work with Dаіmlеr diagnostics CBF files.

- Caesar : Library to operate on CBF files
- Trafo: Transforms CBF files into JSON
- Diogenes: ECU-Coding utility

    ![Header Image](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/diogenes-1.png)

## Warning

This project is far from production ready. Please be careful if you are planning to use this with your vehicle.

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

- Open a UDS connection over J2534 (✅)
- Enumerate connected ECUs and identify online devices with compatible CBF files (✅ : UDS)
- Complete a seed-key challenge with a target ECU (✅ : Paired with [UnlockECU](https://github.com/jglim/UnlockECU))
- Write a new variant-coding string on a target ECU (❌ : Typically about 4 unaccounted bytes, requires DiagServiceCode which is a significant undertaking)

*If you are looking for an alternative with a broader feature set, check out [OpenVehicleDiag](https://github.com/rnd-ash/OpenVehicleDiag) instead*

# Demo

## Establishing a connection

![Connection](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/demo-connect.gif)

## Reading ECU data

![Read Data](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/demo-data.gif)

## (Simulated) Variant coding

![VC](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/demo-vc.gif)

## (New!) CFF flash export

_Dump a CFF flash file's raw memory contents (assumes unencrypted, uncompressed data). Files are labeled with its intended memory address (IDA: File -> Load file -> Additional binary file)_

![CFF](https://raw.githubusercontent.com/jglim/CaesarSuite/main/docs/resources/demo-cff.gif)



---

# Contributing

## Issues

The primary roadblock for Caesar/Diogenes is the inability to parse Diagnostic Service code blobs, which are compiled binary scripts that automate tasks such as session switching, variant identification, ECU unlocking, and (apparently) variant coding. 

### Workaround attempt

 - Session switching and variant identification on UDS devices is generally consistent and standardized.
 - ECU unlocking can be deferred to [UnlockECU](https://github.com/jglim/UnlockECU). 
 - Variant coding is still a mystery as I typically observe about 4 unaccounted bytes that are appended onto the variant-string (Checksum? Tester-signature? I have no idea). 
 	- Furthermore, some devices also include the SCN as part of the variant coding, which appears as an additional 128-bit/16-byte field. 
 	- Guessing is too risky, hence Diogenes will not intentionally write a variant-code to an ECU at this time.

### Logs

A follow up on the mystery 4-byte suffix: these examples show CRD3 and MED40, where `X1X2X3X4` indicate the unknown bytes

```
MED40:
WriteService: WVC_Implizite_Variantenkodierung_Write :
2E10010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000 (424 bits)
2E1001  <- uds : WriteDataByIdentifier @ 1001
      363600E5184141578351893F501444200021000190010401001008000002  <- vc
                                                                  000102030405060708090A0B0C0D0E0F  <- SCN
                                                                                                  X1X2X3X4 <- ???
SID_RQ @ 0, size: 8
RecordDataIdentifier @ 8, size: 16
#0 @ 264, size: 128
#1 @ 392, size: 8
#2 @ 400, size: 8
#3 @ 408, size: 8
#4 @ 416, size: 8
#5 @ 24, size: 400

----------------

CRD3
WriteService: WVC_CRD3_explizit_restricted : 
2E100200000000000000000000000000000000000000000000000000000000000000000000 (296 bits)
2E1002   <- uds : WriteDataByIdentifier @ 1002
      000040080005006000100000AF88E0044251599803C049457900024A4E4E  <- vc
                                                                  X1X2X3X4  <- ???
SID_RQ @ 0, size: 8
RecordDataIdentifier @ 8, size: 16
Varcodestring @ 24, size: 272
#0 @ 264, size: 8
#1 @ 272, size: 8
#2 @ 280, size: 8
#3 @ 288, size: 8
```

In my experience, the 4 bytes contain seemingly random data (`00 40 33 10`) on a "fresh" ECU when a read command is issued. After variant-coding a single field, the value issued by the official client (V) changes to `00 00 00 01` and does not change even when I revert the changes or change other fields.


### My lack of understanding

There are concepts which I do not understand that may impede the development of this project:

 - The SCN seems to be a specific 16-byte string, where the first 10 characters are the part number (as printed on the device), and an unknown 6 characters. Are these 6 characters unique to the device?
 - The 16-character string is queried via `DT_STO_ID_Calibration_Identification`. There is also a function for `DT_STO_ID_Calibration_Verification_Number` which returns an unknown 4 bytes. Are these values matched?
 - Is there a way to query the ECU if the current SCN is valid and accepted? This is crucial to check if the variant-coding was successful.


## Tests

I am unable to test the accuracy of the variant-coding computation. Help in this aspect is very welcome:

- Connect to your vehicle with Vediamo/Monaco and read out the variant-coding string from a target ECU
- In Diogenes, load the same CBF file for the target ECU, and check if the variant-coding output differs from Vediamo/Monaco
- Open an issue here with the results (even for successful 100% matches), screenshots will be very helpful too. Please indicate the ECU name and CBF filename in the issue.
- Thanks in advance :^)

## Reverse engineering c32s.dll

The Caesar library implementation originates from my interpretation of the  `c32s.dll` library that was provided by [@rnd-ash](https://github.com/rnd-ash), and may be a good place to start if you are looking to understand about how CBF files are parsed. (See `_MIInterpreter` to figure out the DSC VM).