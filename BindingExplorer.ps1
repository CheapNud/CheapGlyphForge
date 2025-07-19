
Start-Transcript -Path "BindingAnalysis.txt"
# CheapGlyphForge Complete Binding Explorer - FIXED VERSION
Write-Host "=======================================" -ForegroundColor Magenta
Write-Host "  CHEAPGLYPHFORGE BINDING EXPLORER" -ForegroundColor Magenta
Write-Host "=======================================" -ForegroundColor Magenta
Write-Host ""

# File Structure Overview
Write-Host "=== GENERATED FILES OVERVIEW ===" -ForegroundColor Green
Write-Host ""

Write-Host "Interface SDK Files:" -ForegroundColor Yellow
Get-ChildItem ".\CheapGlyphForge.GlyphInterface.Binding\obj\Debug\net9.0-android\generated\src\" -Recurse -Name | ForEach-Object {
    $size = (Get-Item ".\CheapGlyphForge.GlyphInterface.Binding\obj\Debug\net9.0-android\generated\src\$_").Length
    Write-Host "  $_ ($size bytes)" -ForegroundColor White
}

Write-Host "`nMatrix SDK Files:" -ForegroundColor Yellow  
Get-ChildItem ".\CheapGlyphForge.GlyphMatrix.Binding\obj\Debug\net9.0-android\generated\src\" -Recurse -Name | ForEach-Object {
    $size = (Get-Item ".\CheapGlyphForge.GlyphMatrix.Binding\obj\Debug\net9.0-android\generated\src\$_").Length
    Write-Host "  $_ ($size bytes)" -ForegroundColor White
}

# Function to analyze class files
function Analyze-ClassFile {
    param(
        [string]$FilePath,
        [string]$ClassName
    )
    
    if (Test-Path $FilePath) {
        Write-Host "=== $ClassName ===" -ForegroundColor Cyan
        $content = Get-Content $FilePath -Raw
        
        # Look for constructors
        $constructors = $content | Select-String "public\s+unsafe\s+$ClassName\s*\(" 
        if ($constructors) {
            Write-Host "Constructors:" -ForegroundColor Magenta
            $constructors | ForEach-Object {
                Write-Host "  $($_.Line.Trim())" -ForegroundColor White
            }
        }
        
        # Look for public methods
        $methods = $content | Select-String "public\s+[\w\[\]<>]+\s+\w+\s*\(" | Select-Object -First 10
        if ($methods) {
            Write-Host "Public Methods (first 10):" -ForegroundColor Magenta
            $methods | ForEach-Object {
                Write-Host "  $($_.Line.Trim())" -ForegroundColor White
            }
        }
        
        # Look for constants
        $constants = $content | Select-String "public\s+const\s+"
        if ($constants) {
            Write-Host "Constants:" -ForegroundColor Magenta
            $constants | ForEach-Object {
                Write-Host "  $($_.Line.Trim())" -ForegroundColor Yellow
            }
        }
        
        # Look for static methods
        $staticMethods = $content | Select-String "public\s+static\s+[\w\[\]<>]+\s+\w+\s*\("
        if ($staticMethods) {
            Write-Host "Static Methods:" -ForegroundColor Magenta
            $staticMethods | ForEach-Object {
                Write-Host "  $($_.Line.Trim())" -ForegroundColor Cyan
            }
        }
        
        Write-Host ""
    } else {
        Write-Host "=== $ClassName === (FILE NOT FOUND)" -ForegroundColor Red
        Write-Host ""
    }
}

# Analyze key classes
Write-Host "`n=== KEY CLASSES ANALYSIS ===" -ForegroundColor Green
Write-Host ""

Write-Host "INTERFACE SDK CLASSES:" -ForegroundColor Yellow
Analyze-ClassFile ".\CheapGlyphForge.GlyphInterface.Binding\obj\Debug\net9.0-android\generated\src\Com.Nothing.Ketchum.GlyphManager.cs" "GlyphManager"
Analyze-ClassFile ".\CheapGlyphForge.GlyphInterface.Binding\obj\Debug\net9.0-android\generated\src\Com.Nothing.Ketchum.Common.cs" "Common"
Analyze-ClassFile ".\CheapGlyphForge.GlyphInterface.Binding\obj\Debug\net9.0-android\generated\src\Com.Nothing.Ketchum.Glyph.cs" "Glyph"
Analyze-ClassFile ".\CheapGlyphForge.GlyphInterface.Binding\obj\Debug\net9.0-android\generated\src\Com.Nothing.Ketchum.GlyphFrame.cs" "GlyphFrame"

Write-Host "MATRIX SDK CLASSES:" -ForegroundColor Yellow
Analyze-ClassFile ".\CheapGlyphForge.GlyphMatrix.Binding\obj\Debug\net9.0-android\generated\src\Com.Nothing.Ketchum.GlyphMatrixManager.cs" "GlyphMatrixManager"
Analyze-ClassFile ".\CheapGlyphForge.GlyphMatrix.Binding\obj\Debug\net9.0-android\generated\src\Com.Nothing.Ketchum.GlyphMatrixFrame.cs" "GlyphMatrixFrame"
Analyze-ClassFile ".\CheapGlyphForge.GlyphMatrix.Binding\obj\Debug\net9.0-android\generated\src\Com.Nothing.Ketchum.GlyphMatrixObject.cs" "GlyphMatrixObject"
Analyze-ClassFile ".\CheapGlyphForge.GlyphMatrix.Binding\obj\Debug\net9.0-android\generated\src\Com.Nothing.Ketchum.GlyphToy.cs" "GlyphToy"

# Search for device detection methods
Write-Host "=== DEVICE DETECTION SEARCH ===" -ForegroundColor Green
Write-Host ""

$deviceMethods = @("is20111", "is22111", "is23111", "is23113", "is24111")
$foundAny = $false

foreach ($method in $deviceMethods) {
    $allFiles = Get-ChildItem ".\CheapGlyphForge.GlyphInterface.Binding\obj\Debug\net9.0-android\generated\src\*.cs"
    foreach ($file in $allFiles) {
        $content = Get-Content $file.FullName -Raw
        if ($content -match $method) {
            $matches = $content | Select-String $method
            foreach ($match in $matches) {
                Write-Host "Found $method in $($file.Name)" -ForegroundColor Yellow
                Write-Host "  $($match.Line.Trim())" -ForegroundColor White
                $foundAny = $true
            }
        }
    }
}

if (-not $foundAny) {
    Write-Host "No device detection methods found with expected names." -ForegroundColor Red
}

# Search for device constants
Write-Host "`n=== DEVICE CONSTANTS SEARCH ===" -ForegroundColor Green
Write-Host ""

$deviceConstants = @("DEVICE_20111", "DEVICE_22111", "DEVICE_23111", "DEVICE_23113", "DEVICE_24111")
$foundConstants = $false

foreach ($constant in $deviceConstants) {
    $allFiles = Get-ChildItem ".\CheapGlyphForge.GlyphInterface.Binding\obj\Debug\net9.0-android\generated\src\*.cs"
    foreach ($file in $allFiles) {
        $content = Get-Content $file.FullName -Raw
        if ($content -match $constant) {
            $matches = $content | Select-String $constant
            foreach ($match in $matches) {
                Write-Host "Found $constant in $($file.Name)" -ForegroundColor Yellow
                Write-Host "  $($match.Line.Trim())" -ForegroundColor White
                $foundConstants = $true
            }
        }
    }
}

if (-not $foundConstants) {
    Write-Host "No device constants found with expected names." -ForegroundColor Red
}

Write-Host "`n=== EXPLORATION COMPLETE ===" -ForegroundColor Green
Stop-Transcript