param
(   
    [string]$SolutionDir,
    [string]$ConfigurationName,
    [string]$TargetDir
)

Copy-Item "C:\Program Files (x86)\Acumatica ERP\AcumaticaDemo60\Pages\SM\SM206500*" "$SolutionDir\Customization\Pages\SM\" -force
Copy-Item "C:\Program Files (x86)\Acumatica ERP\AcumaticaDemo60\Pages\SM\SM206510*" "$SolutionDir\Customization\Pages\SM\" -force
Copy-Item "C:\Program Files (x86)\Acumatica ERP\AcumaticaDemo60\Pages\SM\SM206530*" "$SolutionDir\Customization\Pages\SM\" -force
Copy-Item "C:\Program Files (x86)\Acumatica ERP\AcumaticaDemo60\Pages\SO\SO302010*" "$SolutionDir\Customization\Pages\SO\" -force
Copy-Item "$TargetDir\PX.Objects.WM.dll" "$SolutionDir\Customization\Bin\PX.Objects.WM.dll" -force

If ($ConfigurationName = "Release") {
    $customizationProject = $SolutionDir + "Release\PickPackShip.zip"

    If (Test-Path $customizationProject){
	    Remove-Item $customizationProject
    }

    Add-Type -A System.IO.Compression.FileSystem
    [IO.Compression.ZipFile]::CreateFromDirectory($SolutionDir + "\Customization", $customizationProject)
}