function global:exec {
    param (
        [Parameter(Mandatory=$true)]
        [ScriptBlock]$Script
    )

    Invoke-Command $Script

    if($LASTEXITCODE -ne 0) {
        throw "process '$Script' exited with code $LASTEXITCODE"
    }
}

function global:GenerateGallery {
    $galleryGenerator = Join-Path $PSScriptRoot "src/FluentDiagrams.BuildUtils/FluentDiagrams.BuildUtils.csproj"
    $galleryOutputDir = Join-Path $PSScriptRoot output/gallery/svg
    $regressionPathDir = Join-Path $PSScriptRoot src/RegressionTestData/svg
    exec { dotnet run --project $galleryGenerator -- --GalleryOutputDir $galleryOutputDir --RegressionPathDir $regressionPathDir }    
}

function global:Release {
    $outputDir = Join-Path $PSScriptRoot output
    $packageDestinationDir = Join-Path $outputDir packages

    $fluentDiagramsProject
    if ( -not ( test-path $outputDir ) ) {
        mkdir $outputDir
    }

    if( -not ( Test-Path $packageDestinationDir ) ) {
        mkdir $packageDestinationDir
    }

    Write-Host "Generating Gallery..."
    GenerateGallery

    Write-Host "Building FluentDiagrams..."
    $fluentDiagramsProject = Join-Path $PSScriptRoot src/FluentDiagrams/FluentDiagrams.csproj
    exec { dotnet build $fluentDiagramsProject -c Release -o $packageDestinationDir }

    Write-Host "Building FluentDiagrams.Svg..."
    $fluentDiagramsSvgProject = Join-Path $PSScriptRoot src/FluentDiagrams.Svg/FluentDiagrams.Svg.csproj
    exec { dotnet build $fluentDiagramsSvgProject -c Release -o $packageDestinationDir }
}
#######
Release