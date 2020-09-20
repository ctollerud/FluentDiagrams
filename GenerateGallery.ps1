$ErrorActionPreference = "Stop"

function exec {
    Param( [ScriptBlock]$routine )
    Invoke-Command $routine
    if( $LASTEXITCODE -ne 0 ) {
        throw "Invocation failed with exit code $LASTEXITCODE"
    }
}

$galleryGenerator = Join-Path $PSScriptRoot "src/FluentDiagrams.BuildUtils/FluentDiagrams.BuildUtils.csproj"
$galleryOutputDir = Join-Path $PSScriptRoot output/gallery/svg
$regressionPathDir = Join-Path $PSScriptRoot src/RegressionTestData/svg
exec { dotnet run --project $galleryGenerator -- --GalleryOutputDir $galleryOutputDir --RegressionPathDir $regressionPathDir }