use dagger_sdk::{Directory, Query};
use eyre::WrapErr;

use crate::containers::dotnet_builder;

/// Run dotnet tests.
pub async fn run(client: &Query, source: Directory) -> eyre::Result<String> {
    let output = dotnet_builder(client, source)
        .with_exec(vec!["dotnet", "test"])
        .with_exec(vec!["sh", "-c", "echo 'test: all tests passed'"])
        .stdout()
        .await
        .wrap_err("test failed")?;

    Ok(output)
}
