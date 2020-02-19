$INSTALLER_SAS = $args[0].trim("'")

echo $INSTALLER_SAS

echo '-------------------------------------------------------------------------------------'
echo '------------------=Download and install VRay for 3DS Max 2018=---------------------'
echo '-------------------------------------------------------------------------------------'
AzCopy.exe /source:$INSTALLER_SAS /dest:'c:\temp\vray\vray.exe'

C:\temp\vray\vray.exe -gui=0 -configFile='c:\temp\vray\VRayForMaya2018config.xml' -quiet=1 -auto -VARIABLE_CGLS_INSTALLED=1

Remove-Item 'C:\temp\vray' -Force -Recurse
echo 'Finished installing VRay for 3dsmax2018'
