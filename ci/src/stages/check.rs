use dagger_sdk::{Directory, Query};
use eyre::WrapErr;

use crate::containers::dotnet_builder;

/// Build the project with warnings treated as errors.
pub async fn run(client: &Query, source: Directory) -> eyre::Result<String> {
    let output = dotnet_builder(client, source)
        .with_exec(vec!["dotnet", "build", "--warnaserror"])
        .with_exec(vec!["sh", "-c", "echo 'check: dotnet build passed'"])
        .stdout()
        .await
        .wrap_err("check failed")?;

    Ok(output)
}
