$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "..\src\LinkMover.App\LinkMover.App.csproj"
$root = Join-Path $PSScriptRoot "..\artifacts"
$targets = @("win-x64", "linux-x64", "osx-x64", "osx-arm64")
foreach ($rid in $targets) {
  $out = Join-Path $root $rid
  dotnet publish $project -c Release -r $rid --self-contained true -p:PublishSingleFile=true -o $out
  if ($LASTEXITCODE -ne 0) { throw "Publish failed for $rid" }
}
Get-ChildItem $root -File -Recurse | Get-FileHash -Algorithm SHA256 | ForEach-Object { "{0} *{1}" -f $_.Hash, $_.Path.Substring($root.Length + 1) } | Set-Content (Join-Path $root "SHA256SUMS.txt")
