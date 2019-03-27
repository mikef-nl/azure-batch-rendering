$INSTALLER_SAS = $args[0].trim("'")

echo $INSTALLER_SAS

New-Item 'C:\temp\Max2018ToArnold' -type directory

echo '-------------------------------------------------------------------------------------'
echo '------------------------=Download and install Arnold for 3DS Max 2018=--------------------------'
echo '-------------------------------------------------------------------------------------'
echo $INSTALLER_SAS

AzCopy.exe /source:$INSTALLER_SAS /dest:'C:\temp\MAXtoA-1.2.926.2018.exe'

7z e 'C:\temp\MAXtoA-1.2.926.2018.exe' -o'C:\temp\Max2018ToArnold\'
echo 'extract complete'
Start-Process msiexec.exe -ArgumentList @("/I", "C:\temp\Max2018ToArnold\ArnoldPlug.msi", "/passive", "/qn") -Wait
cd C:\temp

#cd ..
#Remove-Item .\temp -Force -Recurse
#cd 'C:\Program Files'
#Remove-Item .\Autodesk -Force -Recurse
#echo 'finished creating the windows image'