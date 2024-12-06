
# WLS Authorization Helper

This Nuget package may be used to add User Authorization to a Darwin API

## Setup

### Environment Variables

The following Environment Variables are required in order to enable access to the authorization service.

|Name | Description|
|------------- | -----------|
|**DARWIN_USERS_API_INTERNAL** | The internal base url to the [wiley/wls-usersapi](https://github.com/wiley/wls-usersapi/).
|**DARWIN_USERS_API_TOKEN** | The internal API secret token|

### Install
- Add WLS.Authorization Nuget Package from WLS Artifactory

### Startup.cs
- Update Startup.cs usages
-- ```using WLS.Authorization;```
- Add the following call to ConfigureServices method
-- ```services.AddWLSAuthorization();```

## Usage
### Policies
This package includes built-in Policies, which may be used in Authorize attributes ```[Authorize(```*```"policyname"```*```)]```
|Policy Name | Description|
|------------- | -----------|
|**Facilitator**|User's Role is Catalyst Facilitator or LPI Facilitator|

### Permissions
The following table describes the CRUD permissions associated with various data elements.

| Data Element | Dependency Injection Interface | Method|
| ------------ | ----------------------------- | --------- |
| Access_Code  | IAccessCodePermissions        | Create(int accountID) |