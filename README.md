AppCore .NET event model
-------------------

[![Build Status](https://dev.azure.com/AppCoreNet/EventModel/_apis/build/status/AppCoreNet.EventModel%20CI?branchName=dev)](https://dev.azure.com/AppCoreNet/EventModel/_build/latest?definitionId=5&branchName=dev)
![Azure DevOps tests (compact)](https://img.shields.io/azure-devops/tests/AppCoreNet/EventModel/5?compact_message)
![Azure DevOps coverage (branch)](https://img.shields.io/azure-devops/coverage/AppCoreNet/EventModel/5/dev)
![Nuget](https://img.shields.io/nuget/v/AppCore.EventModel.Abstractions)

This repository includes projects containing abstractions and implementations of the event framework.

All artifacts are licensed under the [MIT license](LICENSE). You are free to use them in open-source or commercial projects as long
as you keep the copyright notice intact when redistributing or otherwise reusing our artifacts.

## Packages

Latest development packages can be found on [MyGet](https://www.myget.org/gallery/appcorenet).

Package                                            | Description
---------------------------------------------------|-----------------------------------------------------------------------------
`AppCore.EventModel`                               | Provides event framework default implementations.
`AppCore.EventModel.Abstractions`                  | Provides the public API of the event framework.
`AppCore.EventModel.NewtonsoftJson`                | JSON serialization support for events.
`AppCore.EventModel.Logging`                       | Adds logging of events.
`AppCore.EventModel.EntityFrameworkCore.SqlServer` | Adds support for queing events using SQL Server via EF Core.
`AppCore.EventModel.EntityFrameworkCore.MySql`     | Adds support for queing events using MySql via EF Core.
`AppCore.EventModel.EntityFrameworkCore.PostgreSql`| Adds support for queing events using PostgreSql via EF Core.
`AppCore.EventModel.Store`                         | Event store default implementation.
`AppCore.EventModel.Store.Abstractions`            | Provides the public API for the event store.

## Contributing

Contributions, whether you file an issue, fix some bug or implement a new feature, are highly appreciated. The whole user community
will benefit from them.

Please refer to the [Contribution guide](CONTRIBUTING.md).
