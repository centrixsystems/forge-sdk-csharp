use dagger_sdk::{Container, Directory, Query};

/// Base .NET SDK container with NuGet cache.
pub fn dotnet_builder(client: &Query, source: Directory) -> Container {
    let nuget_cache = client.cache_volume("forge-sdk-csharp-nuget");

    client
        .container()
        .from("mcr.microsoft.com/dotnet/sdk:8.0")
        .with_mounted_directory("/build", source)
        .with_workdir("/build")
        .with_mounted_cache("/root/.nuget/packages", nuget_cache)
}
