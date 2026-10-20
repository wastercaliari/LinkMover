$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"
$localDotnet = Join-Path $PSScriptRoot "..\.dotnet\dotnet.exe"
$dotnet = if (Test-Path $localDotnet) { $localDotnet } else { "dotnet" }
$project = Join-Path $PSScriptRoot "..\src\LinkMover.App\LinkMover.App.csproj"
$root = Join-Path $PSScriptRoot "..\artifacts"
$release = Join-Path $PSScriptRoot "..\releases"
$targets = @("win-x64", "linux-x64", "osx-x64", "osx-arm64")
New-Item -ItemType Directory -Force $root, $release | Out-Null
foreach ($rid in $targets) {
  $out = Join-Path $root $rid
  & $dotnet publish $project -c Release -r $rid --self-contained true -p:PublishSingleFile=true -o $out
  if ($LASTEXITCODE -ne 0) { throw "Publish failed for $rid" }
  $stage = Join-Path ([IO.Path]::GetTempPath()) ("LinkMover-release-" + $rid)
  if (Test-Path $stage) { Remove-Item $stage -Recurse -Force }
  New-Item -ItemType Directory -Force $stage | Out-Null
  Get-ChildItem $out -File | Where-Object { $_.Extension -ne ".pdb" } | Copy-Item -Destination $stage
  if ($rid -eq "win-x64" -and (Test-Path (Join-Path $stage "LinkMover.App.exe"))) {
    Rename-Item (Join-Path $stage "LinkMover.App.exe") "LinkMover.exe"
  }
  $zip = Join-Path $release ("LinkMover-" + $rid + ".zip")
  if (Test-Path $zip) { Remove-Item $zip -Force }
  Compress-Archive -Path (Join-Path $stage "*") -DestinationPath $zip -CompressionLevel Optimal
  Remove-Item $stage -Recurse -Force
}
Get-ChildItem $release -File | Where-Object { $_.Name -ne "SHA256SUMS.txt" } | Get-FileHash -Algorithm SHA256 | ForEach-Object {
  "{0} *{1}" -f $_.Hash, $_.Path.Substring($release.Length + 1)
} | Set-Content (Join-Path $release "SHA256SUMS.txt")
Write-Host "Builds and release archives generated in $release"
