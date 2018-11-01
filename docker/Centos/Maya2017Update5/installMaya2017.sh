#!/bin/sh -x

echo 'INSTALLER_SAS='$1
INSTALLER_SAS=$1

mkdir -m777 /tmp/maya2017 -p
mkdir -m777 /tmp/maya/Maya2017 -p

echo '-------------------------------------------------------------------------------------'
echo '------------------------------=Download Maya=-----------------------------------'
echo '-------------------------------------------------------------------------------------'
azcopy --source $INSTALLER_SAS --destination /tmp/maya/Autodesk_MayaIO_2017_Linux_64bit.run

echo '-------------------------------------------------------------------------------------'
echo '------------------------------=Extract and Install Maya=-----------------------------------'
echo '-------------------------------------------------------------------------------------'
7za e /tmp/maya/Autodesk_MayaIO_2017_Linux_64bit.run -o/tmp/maya/
7za e /tmp/maya/Autodesk_MayaIO_2017_Linux_64bit -o/tmp/maya/Maya2017/
chmod +x /tmp/maya/Maya2017/setup.sh
/tmp/maya/Maya2017/setup.sh 127.0.0.1

cp /usr/bin/maya2017.sh /usr/bin/render2017.sh
cp /usr/bin/maya2017.sh /usr/bin/render.sh

chmod +x /usr/bin/maya2017.sh
chmod +x /usr/bin/render2017.sh
chmod +x /usr/bin/render.sh

rm -rf /tmp/maya2017
rm -rf /tmp/maya

yum clean all
