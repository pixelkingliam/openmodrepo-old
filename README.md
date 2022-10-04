# openmodrepo

HTTP Server providing a easy to setup repository for game modifications

#### Features

* Multiple 'Games'
* Account making
* No Client-side JavaScript

#### Known bugs/issues
* /games page looks like ass and is broken for whatever reason
## Usage

The compiled binary (`server.exe`) can be ran

on Linux with `mono server.exe`

and on Windows by opening the `server.exe` file

---

*This requires Mono (Linux/MacOS/Windows) or .NET Framework 4.7.2 (Windows) to be installed*

*The compiled binary requires Newtonsoft.Json.dll present next to it*

## Compiling

This project is made with Mono (.NET Framework 4.7.2) and should be compiled with it

running `msbuild` will result in a executable (`server.exe`)

#### References

* [Newtonsoft.Json.Net](https://github.com/JamesNK/Newtonsoft.Json)

### Contributing
You are welcome to suggest new changes and do PRs
