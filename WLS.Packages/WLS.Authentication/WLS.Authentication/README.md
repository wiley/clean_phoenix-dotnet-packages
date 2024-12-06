# WLS Authentication Helper

This Nuget package may be used to add JWT Authentication to a Darwin API

## Setup

### Environment Variables

The following Environment Variables are required in order to enable JWT validation

|Name | Description|
|------------- | -----------|
|**DARWIN_JWT_SECRET_KEY** | The Jwt Secret Key used to sign the JWT Token|
|**DARWIN_JWT_ISSUER** | The JWT Token Issuer ("iss") used to validate the token|
|**DARWIN_JWT_AUDIENCE** | The JWT Token Audience ("aud") used to validate the token|

### Install
- Add WLS.Authentication Nuget Package from WLS Artifactory

### Startup.cs
- Update Startup.cs usages
-- ```using WLS.Authentication;```
- Add the following call to ConfigureServices method
-- ```services.AddWLSAuthentication();```
- Ensure the following call is included in the Configure method
-- ```services.UseAuthentication();```
