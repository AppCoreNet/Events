AppCore .NET Events
-------------------

[![Build Status](https://dev.azure.com/AppCoreNet/Events/_apis/build/status/AppCoreNet.Events%20CI?branchName=dev)](https://dev.azure.com/AppCoreNet/Events/_build/latest?definitionId=5&branchName=dev)
![Azure DevOps tests (compact)](https://img.shields.io/azure-devops/tests/AppCoreNet/Events/5?compact_message)
![Azure DevOps coverage (branch)](https://img.shields.io/azure-devops/coverage/AppCoreNet/Events/5/dev)
![Nuget](https://img.shields.io/nuget/v/AppCore.Events.Abstractions)

This repository includes projects containing abstractions and implementations of the event framework.

All artifacts are licensed under the [MIT license](LICENSE). You are free to use them in open-source or commercial projects as long
as you keep the copyright notice intact when redistributing or otherwise reusing our artifacts.

## Packages

Latest development packages can be found on [MyGet](https://www.myget.org/gallery/appcorenet).

Package                                    | Description
-------------------------------------------|-----------------------------------------------------------------------------
`AppCore.Events`                           | Provides event framework default implementations.
`AppCore.Events.Abstractions`              | Provides the public API of the event framework.
`AppCore.Events.Formatters.NewtonsoftJson` | JSON serialization support for events.
`AppCore.Events.Logging`                   | Adds logging of events.
`AppCore.Events.EntityFrameworkCore3.SqlServer` | Adds support for queing events using SQL Server via EF Core.
`AppCore.Events.Storage`                   | Event store default implementation.
`AppCore.Events.Storage.Abstractions`      | Provides the public API for the event store.

## Contributing

Contributions, whether you file an issue, fix some bug or implement a new feature, are highly appreciated. The whole user community
will benefit from them.

Please refer to the [Contribution guide](CONTRIBUTING.md).
