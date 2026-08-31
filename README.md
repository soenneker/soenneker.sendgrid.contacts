[![](https://img.shields.io/nuget/v/soenneker.sendgrid.contacts.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.sendgrid.contacts/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.sendgrid.contacts/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.sendgrid.contacts/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.sendgrid.contacts.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.sendgrid.contacts/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.sendgrid.contacts/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.sendgrid.contacts/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.SendGrid.Contacts

Adds or updates SendGrid Marketing contacts, searches by email, retrieves by ID, and deletes selected contacts or the entire contact database.

## Installation

```bash
dotnet add package Soenneker.SendGrid.Contacts
```

## Configuration

```json
{
  "SendGrid": {
    "ApiKey": "SG.xxxxxxxxx"
  }
}
```

The API key needs access to Marketing contacts.

## Usage

```csharp
using Soenneker.SendGrid.Contacts.Abstract;
using Soenneker.SendGrid.Contacts.Registrars;

services.AddSendGridContactsUtilAsSingleton();

public sealed class ContactLookup
{
    private readonly ISendGridContactsUtil _contacts;

    public ContactLookup(ISendGridContactsUtil contacts)
    {
        _contacts = contacts;
    }

    public async Task Find(
        string email,
        CancellationToken cancellationToken)
    {
        var result = await _contacts.Search(
            email,
            cancellationToken: cancellationToken);
    }
}
```

`AddOrUpdate` submits SendGrid's asynchronous import job and returns its job response. `AddAndWait` submits the job, waits briefly, then retries contact lookup for the first contact in the request.

`Delete(ids)` removes the specified contacts. `DeleteAll()` requests deletion of the entire SendGrid Marketing contact database and cannot be scoped to a list; use it only when that full deletion is intended.
