#!/bin/sh -x

echo 'MTOA_INSTALL_SAS='$1
MTOA_INSTALL_SAS=$1

mkdir -m777 /tmp/mtoa
echo '-------------------------------------------------------------------------------------'
echo '------------------------------=Download Arnold 2018=---------------------------------'
echo '-------------------------------------------------------------------------------------'

azcopy --source $MTOA_INSTALL_SAS --destination /tmp/mtoa/MtoA-2.1.0.3-linux-2018.run

echo '-------------------------------------------------------------------------------------'
echo '------------------------=Extract and Install Arnold 2018=----------------------------'
echo '-------------------------------------------------------------------------------------'
7za x /tmp/mtoa/MtoA-2.1.0.3-linux-2018.run -o/tmp/mtoa/
7za x /tmp/mtoa/MtoA-2.1.0.3-linux-2018 -o/tmp/mtoa/MtoA2018
7za x /tmp/mtoa/MtoA2018/package.zip -o/opt/solidangle/mtoa/2018/

cp /opt/solidangle/mtoa/2018/arnoldRenderer.xml /usr/autodesk/mayaIO2018/bin/rendererDesc/
chmod 755 -R /opt/solidangle

yum clean all
