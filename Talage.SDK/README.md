# Talage.SDK

Talage API SDK for .NET.

## Install

```powershell
dotnet add package Talage.SDK --version 1.0.0
```

## Configuration (external)

Provide settings from your application's configuration (for example `appsettings.json`, environment variables, user-secrets, Key Vault, etc).

```json
{
  "Talage": {
    "BaseUrl": "https://example.talage.com",
    "ApiKey": "your-api-key",
    "Secret": "your-api-secret"
  }
}
```

## DI registration

```csharp
using Talage.SDK.DependencyInjection;

builder.Services.AddTalageSdk(builder.Configuration, settings =>
{
    builder.Configuration.GetSection("Talage").Bind(settings);
});
```

### Optional database-backed storage

If your application provides `ConnectionStrings:AppLog`, the SDK will use it for EF Core-backed storage (token/log related). If not provided, the SDK falls back to no-op/in-memory implementations.

## Usage

Resolve `Talage.SDK.Interfaces.ITalageClient` from DI and call its async APIs.

