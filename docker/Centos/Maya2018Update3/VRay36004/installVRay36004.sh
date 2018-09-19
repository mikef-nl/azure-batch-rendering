#!/bin/sh -x

echo 'VRAY_INSTALL_SAS='$1
VRAY_INSTALL_SAS=$1

echo '-------------------------------------------------------------------------------------'
echo '--------------------------=Download Vray 2018=------------------------------'
echo '-------------------------------------------------------------------------------------'
echo 'download vray4batch_36004_maya2018_linux_x64'
azcopy --source $VRAY_INSTALL_SAS --destination /tmp/vray/vray4batch_36004_maya2018_linux_x64 

chmod +x /tmp/vray/vray4batch_36004_maya2018_linux_x64
/tmp/vray/vray4batch_36004_maya2018_linux_x64 -gui=0 -configFile='/tmp/vray/vray_config2018.xml' -ignoreErrors=1

yum clean all
