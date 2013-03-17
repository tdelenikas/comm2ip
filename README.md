# About

The Comm2IP utility application is used to create an interface between a (normal or virtual) serial port and an IP endpoint.

# Information / Usage

## What is Comm2IP?

The Comm2IP interface is used to create an interface between a (normal or virtual) serial port and an IP endpoint. You can use it, for example, for mapping your COM1 serial port to 127.0.0.1:52000 IP endpoint. Once the link has been created, all traffic sent to the IP endpoint will be forwarded to the serial port and vice verca.

Comm2IP is written in C#/.NET Framework, with a tiny size and minimal dependencies, and targets Win32/64 environments. I've used .NET v4 to build it, however you can safely built it with .NET v2 as well (you may need to modify the ```log4net``` configuration or references for .NET v2).

It comes in two flavors:

 * The **Comm2IP** library, which you can reference and use in your applications.
 * The **Comm2IPService** standalone application, which can be used either from the command line or as a Windows Service.

## Comm2IP library

TODO

## Comm2IPService application

Use this to create a Serial-To-IP link without writing code. There are two modes of operation: command line and Windows Service. Run `Comm2IPService` from the command prompt for syntax help.

### Using Comm2IPService from the command prompt

Run:

```
C:>Comm2IPService -a 127.0.0.1 -p 12000 -c com1 -b 19200
```

to create a link between COM1 (baud rate @19200) and localhost, port 12000

### Using Comm2IPService as a Windows service

The Windows service supports up to 5 links (code configurable).

To alter the links, edit `Comm2IPService.exe.config` and check the **Bindings** sections. Each **Binding** accepts the same syntax that you use for the command line invocation.

Use:

```
C:>Comm2IPService --install
```

to install `Comm2IPService` as a Windows service.

Use:

```
C:>Comm2IPService --uninstall
```

to uninstall `Comm2IPService` Windows service.

# License

All SMSLib components are licensed under the terms of the Apache v2 license.

Copyright (c) 2002-2013, smslib.org

Licensed under the Apache License, Version 2.0 (the "License");
You may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
