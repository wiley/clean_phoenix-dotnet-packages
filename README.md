# wls-net-packages
Dotnet nuget packages solution

All the nuget packages created by WLS is centralized into a single .NET solution, split into solution folders for being better organized.

They generate the nuget package file on build, we control the version based into semantic version (SemVer). 
ie: X.Y.Z, where X is the Major, Y is the Minor, and Z is the Patch, all of them being integers.

We use the WLS. to identify that the nugets / projects are related to WLS, so that we dont have conflicts with possible packages from other sources that can have a similar name.
