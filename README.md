# FakeXrmEasy: Modern Unit Testing for Dynamics 365

A truly open-source testing framework for Dynamics 365 / Power Platform that makes unit testing plugins, workflows, and custom code simple and fast.

[![NuGet](https://img.shields.io/nuget/v/FakeXrmEasy.Community.svg)](https://www.nuget.org/packages/FakeXrmEasy.Community)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 🎉 Version 1.0.2 - Modern Dataverse Features!

**🔥 NEW in v1.0.2** (January 2026): Major enhancements for modern Dataverse testing!

### 🚀 Simplified Plugin Testing
- ✅ **Auto-Populate Entity Images** - No more manual pre/post image setup boilerplate!
- ✅ **Automatic Relationship Discovery** - Initialize metadata once, relationships auto-register!
- ✅ **Filtering Attributes Validation** - Pipeline simulation now matches real Dataverse behavior!

### 💪 Modern Bulk Operations
All optimized bulk operations now supported:
- ✅ **CreateMultiple** - Transactional bulk creates (no 1000 record limit!)
- ✅ **UpdateMultiple** - Transactional bulk updates
- ✅ **DeleteMultiple** - Transactional bulk deletes
- ✅ **UpsertMultiple** - Bulk upsert with create/update detection

### 🔧 Query Engine Fixes
Major improvements from upstream issues:
- ✅ **FetchXML Multiple Filters** - Multiple filter nodes now correctly combined with AND
- ✅ **Left Outer Joins** - Proper GroupJoin pattern for aggregate queries
- ✅ **Between Dates** - End dates include full day (23:59:59.999)
- ✅ **Date Operators** - ThisMonth, LastMonth, ThisWeek, LastWeek all working with timezone support
- ✅ **EntityReference.Name** - Automatically populated from PrimaryNameAttribute on retrieve

See [UPSTREAM_STATUS.md](UPSTREAM_STATUS.md) for full tracking of upstream issues!

### Previous Features (v1.0.1)
- ✅ **CalculateRollupFieldRequest support** - test rollup field calculations
- ✅ **SDK-style project format** - no more NuGet headaches!
- ✅ **IPluginExecutionContext4 support** - full Azure AD integration
- ✅ **Simplified project structure** - easier to maintain and contribute
- ✅ **Better tooling support** - works great with VS 2019/2022

See [MODERNIZATION.md](MODERNIZATION.md) and [SDK_STYLE_MIGRATION.md](SDK_STYLE_MIGRATION.md) for migration details.

## What is FakeXrmEasy?

FakeXrmEasy is a comprehensive mocking framework for Dynamics 365 that enables:

- **Unit Testing Plugins**: Test your plugin logic without deploying to a real environment
- **Workflow Testing**: Validate custom workflow activities with in-memory execution
- **Fast Test Execution**: Run hundreds of tests in seconds with in-memory context
- **No Server Required**: Test offline without connecting to Dynamics 365
- **Early and Late Bound Support**: Works with generated entities or dynamic Entity objects
- **Modern Project Format**: SDK-style projects with PackageReference (no more packages.config!)

## Getting Started

### Installation

```bash
Install-Package FakeXrmEasy.Community
```

Or via .NET CLI:
```bash
dotnet add package FakeXrmEasy.Community
```

### Quick Example

```csharp
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;
using Xunit;

public class AccountPluginTests
{
    [Fact]
    public void When_Account_Created_Should_Set_AccountNumber()
    {
        // Arrange
        var context = new XrmFakedContext();
        var target = new Entity("account")
        {
            ["name"] = "Contoso"
        };

        // Act
        context.ExecutePluginWithTarget<AccountNumberPlugin>(target);

        // Assert
        Assert.True(target.Contains("accountnumber"));
        Assert.NotNull(target["accountnumber"]);
    }
}
```

### Testing with Early-Bound Entities

```csharp
var context = new XrmFakedContext();
context.ProxyTypesAssembly = Assembly.GetExecutingAssembly();

var account = new Account
{
    Id = Guid.NewGuid(),
    Name = "Test Account"
};

context.Initialize(new List<Entity> { account });
var service = context.GetOrganizationService();

// Test your code with the service
```

### Testing Workflow Activities

```csharp
var context = new XrmFakedContext();
var inputs = new Dictionary<string, object>
{
    { "Target", new EntityReference("account", Guid.NewGuid()) },
    { "InputText", "Hello" }
};

var outputs = context.ExecuteCodeActivity<MyCustomActivity>(inputs);
Assert.Equal("Hello World", outputs["OutputText"]);
```

### 🔥 NEW: Simplified Plugin Testing (v1.0.2)

#### Auto-Populate Entity Images

**Before (manual boilerplate):**
```csharp
var preImage = service.Retrieve("account", accountId, new ColumnSet(true));
pluginContext.PreEntityImages.Add("PreImage", preImage);
var postImage = service.Retrieve("account", accountId, new ColumnSet(true));
pluginContext.PostEntityImages.Add("PostImage", postImage);
context.ExecutePluginWith<MyPlugin>(pluginContext);
```

**Now (automatic!):**
```csharp
// Entity images auto-populated from context!
context.ExecutePluginWithTarget<MyPlugin>(target,
    messageName: "Update",
    stage: 40,
    preImageColumns: new ColumnSet(true),
    postImageColumns: new ColumnSet(true));
```

#### Automatic Relationship Discovery

**Before:**
```csharp
// Had to manually register every relationship
context.AddRelationship("new_account_contact", new XrmFakedRelationship {
    IntersectEntity = "new_account_contact",
    Entity1LogicalName = "account",
    Entity1Attribute = "accountid",
    Entity2LogicalName = "contact",
    Entity2Attribute = "contactid",
    RelationshipType = XrmFakedRelationship.enmFakeRelationshipType.ManyToMany
});
```

**Now (automatic!):**
```csharp
// Relationships auto-registered from metadata!
context.InitializeMetadata(entityMetadata);
// All N:N, 1:N, and N:1 relationships are now available
```

### 🔥 NEW: Modern Bulk Operations (v1.0.2)

```csharp
// CreateMultiple - Transactional bulk create
var accounts = new EntityCollection();
accounts.Entities.Add(new Entity("account") { ["name"] = "Account 1" });
accounts.Entities.Add(new Entity("account") { ["name"] = "Account 2" });

var request = new CreateMultipleRequest { Targets = accounts };
var response = (CreateMultipleResponse)service.Execute(request);
// response.Ids contains all created IDs

// UpdateMultiple - Transactional bulk update
var updates = new EntityCollection();
updates.Entities.Add(new Entity("account") { Id = id1, ["revenue"] = 100000 });
updates.Entities.Add(new Entity("account") { Id = id2, ["revenue"] = 200000 });

service.Execute(new UpdateMultipleRequest { Targets = updates });

// UpsertMultiple - Bulk create or update with detection
var upserts = new EntityCollection();
upserts.Entities.Add(new Entity("account") { Id = existingId, ["name"] = "Updated" });
upserts.Entities.Add(new Entity("account") { ["name"] = "New Account" });

var upsertResponse = (UpsertMultipleResponse)service.Execute(
    new UpsertMultipleRequest { Targets = upserts });

foreach (var result in upsertResponse.Results)
{
    Console.WriteLine($"ID: {result.Id}, Created: {result.RecordCreated}");
}
```

## Features

### Core Capabilities

- ✅ **CRUD Operations**: Create, Read, Update, Delete with full relationship support
- ✅ **Query Support**: QueryExpression, FetchXML, and LINQ queries
- ✅ **Plugin Execution**: Full plugin pipeline simulation with pre/post images
- ✅ **Workflow Activities**: Test custom workflow activities
- ✅ **Metadata Support**: Automatic metadata inference from early-bound types
- ✅ **Security Testing**: Test security roles and access rights
- ✅ **ExecuteMultiple**: Batch operation support
- ✅ **Associate/Disassociate**: N:N relationship testing

### Supported Messages

FakeXrmEasy supports 50+ standard CRM messages including:

- Create, Update, Delete, Retrieve, RetrieveMultiple
- **NEW**: CreateMultiple, UpdateMultiple, DeleteMultiple, UpsertMultiple (v1.0.2)
- Associate, Disassociate
- Assign, GrantAccess, RevokeAccess, ModifyAccess
- SetState, SetStateDynamicEntity
- ExecuteMultiple, ExecuteTransaction
- WhoAmI, RetrieveVersion
- CalculateRollupField (v1.0.1)
- And many more...

## Building from Source

### Prerequisites

- .NET Framework 4.6.2 or higher
- Visual Studio 2017 or later (or MSBuild tools)

### Build Commands

```bash
# Restore packages and build
build.bat

# Or individual commands
build.bat clean      # Clean artifacts
build.bat restore    # Restore NuGet packages
build.bat build      # Build solution
build.bat test       # Run tests
build.bat pack       # Create NuGet package
```

## Project Structure

```
FakeXrmEasy/
├── FakeXrmEasy/              # Main library project
├── FakeXrmEasy.Shared/       # Shared implementation code
├── FakeXrmEasy.Tests/        # Test project
├── FakeXrmEasy.Tests.Shared/ # Shared test code
└── build.bat                 # Build script
```

## Documentation

For more detailed documentation, examples, and advanced scenarios, see:

- **Troubleshooting**: [Common issues and solutions](TROUBLESHOOTING.md)
- **IPluginExecutionContext4**: [New interface support](IPluginExecutionContext4_EXAMPLE.md)
- **Examples**: Check the [FakeXrmEasy.Tests](FakeXrmEasy.Tests/) project for comprehensive examples
- **Developer Guide**: See [CLAUDE.md](CLAUDE.md) for architecture and development guidelines

## Target Platform

**Dynamics 365 v9.x and later** (Power Platform / Common Data Service)

This community edition focuses exclusively on modern Dynamics 365 Online. For older CRM versions, please use the legacy branches.

## Contributing

We welcome contributions! This is a truly open-source project maintained by the community.

### How to Contribute

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Write tests for your changes
4. Commit your changes (`git commit -m 'Add amazing feature'`)
5. Push to the branch (`git push origin feature/amazing-feature`)
6. Open a Pull Request

### Contribution Guidelines

- **Include Tests**: All new features and bug fixes must include unit tests
- **Follow Conventions**: Match the existing code style
- **Document Changes**: Update README and docs as needed
- **One Feature Per PR**: Keep pull requests focused

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Support

If you or your organization uses FakeXrmEasy.Community and finds it valuable, consider contributing to the project by:

- Submitting bug fixes and new features
- Improving documentation
- Sharing your testing patterns with the community
- Reporting issues and providing feedback

## Community

- **NuGet Package**: [FakeXrmEasy.Community on NuGet](https://www.nuget.org/packages/FakeXrmEasy.Community)
- **Documentation**: See [CLAUDE.md](CLAUDE.md) for development guidelines and architecture details

## Acknowledgments

This project builds on the excellent foundation established by the original FakeXrmEasy v1.x. We're committed to keeping it truly open-source and community-driven.

Special thanks to all contributors who have helped make this project better!

---

**Made with ❤️ by the Dynamics 365 community**
