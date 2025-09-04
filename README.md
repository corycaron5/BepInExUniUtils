# BepInEx Universal Utilities

Universal Utilities for modding Unity games with BepInEx.

---

## Features

- Input framework for mapping actions to Unity keycodes, including modifiers
- Simple Json configuration system for when BepInEx configs aren't powerful enough

---

## Dependencies

- [BepInEx](https://github.com/BepInEx/BepInEx)
- [NewtonsoftJson](https://github.com/JamesNK/Newtonsoft.Json)
- Netstandard 2.x (Target framework) (InputTrigger uses the HashCode utility for generating it's hashcode, to support other frameworks, clone this repository and generate the hashcode another way)

## Installation

### End Users

Download the latest version from the [Releases](https://github.com/corycaron5/BepInExUniUtils/releases) page.
Unzip and copy the `BepInExUniUtils.dll` file into the BepInEx/plugins folder.

### Developers

Currently there is no Nuget package for this, just reference the assembly directly in your IDE of choice.

---

## [Check out the documentation](https://corycaron5.github.io/BepInExUniUtils/)
