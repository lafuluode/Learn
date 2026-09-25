param(
    [string]$ProjectRoot = "C:\My game\Learn",
    [string]$DestinationRoot = "C:\Users\25061\Desktop\背包"
)

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

$projectPath = [System.IO.Path]::GetFullPath($ProjectRoot)
$destinationPath = [System.IO.Path]::GetFullPath($DestinationRoot)
$packageName = "Unity工程师笔试_背包系统"
$packageFolder = Join-Path $destinationPath $packageName
$sourceZip = Join-Path $packageFolder "源码审查包.zip"
$playerZip = Join-Path $packageFolder "Windows运行包.zip"
$finalZip = Join-Path $destinationPath "${packageName}_交付包.zip"
$buildFolder = Join-Path $projectPath "Deliverables\Unity工程师笔试_背包系统\WindowsBuild"

if (-not (Test-Path -LiteralPath (Join-Path $buildFolder "Learn.exe"))) {
    throw "没有找到 Windows 构建产物：$buildFolder"
}

New-Item -ItemType Directory -Path $destinationPath -Force | Out-Null

if (Test-Path -LiteralPath $packageFolder) {
    Remove-Item -LiteralPath $packageFolder -Recurse -Force
}

New-Item -ItemType Directory -Path $packageFolder -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $packageFolder "算法题") -Force | Out-Null

Copy-Item -LiteralPath (Join-Path $projectPath "README.md") -Destination (Join-Path $packageFolder "README.md")
Copy-Item -LiteralPath (Join-Path $projectPath "Interview\AlgorithmSolutions.cs") -Destination (Join-Path $packageFolder "算法题\AlgorithmSolutions.cs")
Copy-Item -LiteralPath (Join-Path $projectPath "Interview\README.md") -Destination (Join-Path $packageFolder "算法题\README.md")

$runtimeScreenshot = Join-Path $projectPath "inventory_runtime.png"
if (Test-Path -LiteralPath $runtimeScreenshot) {
    New-Item -ItemType Directory -Path (Join-Path $packageFolder "运行截图") -Force | Out-Null
    Copy-Item -LiteralPath $runtimeScreenshot -Destination (Join-Path $packageFolder "运行截图\背包界面.png")
}

function New-FilteredZip {
    param(
        [Parameter(Mandatory = $true)][string]$ZipPath,
        [Parameter(Mandatory = $true)][array]$Inputs,
        [scriptblock]$Exclude
    )

    if (Test-Path -LiteralPath $ZipPath) {
        Remove-Item -LiteralPath $ZipPath -Force
    }

    $archive = [System.IO.Compression.ZipFile]::Open($ZipPath, [System.IO.Compression.ZipArchiveMode]::Create)
    try {
        foreach ($inputItem in $Inputs) {
            $source = [System.IO.Path]::GetFullPath($inputItem.Source)
            $prefix = [string]$inputItem.Prefix

            if (Test-Path -LiteralPath $source -PathType Leaf) {
                $relativeName = if ($prefix) { "$prefix/$([System.IO.Path]::GetFileName($source))" } else { [System.IO.Path]::GetFileName($source) }
                if (-not $Exclude -or -not (& $Exclude $source $relativeName)) {
                    [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $source, $relativeName, [System.IO.Compression.CompressionLevel]::Optimal) | Out-Null
                }
                continue
            }

            Get-ChildItem -LiteralPath $source -Recurse -File | ForEach-Object {
                $relative = $_.FullName.Substring($source.Length).TrimStart('\', '/') -replace '\\', '/'
                $entryName = if ($prefix) { "$prefix/$relative" } else { $relative }
                if (-not $Exclude -or -not (& $Exclude $_.FullName $entryName)) {
                    [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $_.FullName, $entryName, [System.IO.Compression.CompressionLevel]::Optimal) | Out-Null
                }
            }
        }
    }
    finally {
        $archive.Dispose()
    }
}

$assetFolders = @(
    "Scripts",
    "Scenes",
    "ConfigTable",
    "Tests",
    "Art\Inventory",
    "Art\Font",
    "AddressableAssetsData",
    "AddressableAssets",
    "GameAssets",
    "Settings",
    "TextMesh Pro",
    "Plugins",
    "XLua",
    "HotUpdate"
)

$sourceInputs = @(
    @{ Source = (Join-Path $projectPath "Packages"); Prefix = "Packages" },
    @{ Source = (Join-Path $projectPath "ProjectSettings"); Prefix = "ProjectSettings" },
    @{ Source = (Join-Path $projectPath "Interview"); Prefix = "Interview" },
    @{ Source = (Join-Path $projectPath "README.md"); Prefix = "" }
)

foreach ($assetFolder in $assetFolders) {
    $assetPath = Join-Path (Join-Path $projectPath "Assets") $assetFolder
    if (Test-Path -LiteralPath $assetPath) {
        $assetPrefix = "Assets/" + ($assetFolder -replace '\\', '/')
        $sourceInputs += @{ Source = $assetPath; Prefix = $assetPrefix }
    }
}

foreach ($optionalFile in @(".gitignore", ".vsconfig")) {
    $optionalPath = Join-Path $projectPath $optionalFile
    if (Test-Path -LiteralPath $optionalPath) {
        $sourceInputs += @{ Source = $optionalPath; Prefix = "" }
    }
}

New-FilteredZip -ZipPath $sourceZip -Inputs $sourceInputs -Exclude {
    param($fullPath, $entryName)
    return $entryName -match '(^|/)(\.vs|Library|Temp|Logs|obj|UserSettings|Deliverables|HybridCLRData)(/|$)' -or
           $entryName -match '\.(csproj|sln|suo|user)$'
}

New-FilteredZip -ZipPath $playerZip -Inputs @(
    @{ Source = $buildFolder; Prefix = "Windows运行包" }
) -Exclude {
    param($fullPath, $entryName)
    return $entryName -match 'Learn_BurstDebugInformation_DoNotShip'
}

if (Test-Path -LiteralPath $finalZip) {
    Remove-Item -LiteralPath $finalZip -Force
}

[System.IO.Compression.ZipFile]::CreateFromDirectory(
    $packageFolder,
    $finalZip,
    [System.IO.Compression.CompressionLevel]::Optimal,
    $true
)

$result = [ordered]@{
    PackageFolder = $packageFolder
    FinalZip = $finalZip
    FinalZipMB = [math]::Round((Get-Item -LiteralPath $finalZip).Length / 1MB, 2)
    SourceReviewZipMB = [math]::Round((Get-Item -LiteralPath $sourceZip).Length / 1MB, 2)
    PlayerZipMB = [math]::Round((Get-Item -LiteralPath $playerZip).Length / 1MB, 2)
}

$result | ConvertTo-Json
