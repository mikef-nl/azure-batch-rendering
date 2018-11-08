#!/bin/sh -x

echo 'INSTALLER_SAS='$1
INSTALLER_SAS=$1

mkdir -m777 /tmp/mtoa
echo '-------------------------------------------------------------------------------------'
echo '------------------------------=Download Arnold=---------------------------------'
echo '-------------------------------------------------------------------------------------'

azcopy --source $INSTALLER_SAS --destination /tmp/mtoa/MtoA-linux-2018.run

echo '-------------------------------------------------------------------------------------'
echo '------------------------=Extract and Install Arnold=----------------------------'
echo '-------------------------------------------------------------------------------------'
7za x /tmp/mtoa/MtoA-linux-2018.run -o/tmp/mtoa/
7za x /tmp/mtoa/MtoA-linux-2018 -o/tmp/mtoa/MtoA2018
7za x /tmp/mtoa/MtoA2018/package.zip -o/opt/solidangle/mtoa/2018/

cp /opt/solidangle/mtoa/2018/arnoldRenderer.xml /usr/autodesk/mayaIO2018/bin/rendererDesc/
chmod 755 -R /opt/solidangle

yum clean all
