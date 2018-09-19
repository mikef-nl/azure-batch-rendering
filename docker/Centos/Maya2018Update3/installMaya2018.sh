#!/bin/sh -x

echo 'MAYA_INSTALL_SAS='$1
MAYA_INSTALL_SAS=$1

mkdir -m777 /tmp/maya2018 -p
mkdir -m777 /tmp/maya/Maya2018 -p

echo '-------------------------------------------------------------------------------------'
echo '------------------------------=Download Maya 2018=-----------------------------------'
echo '-------------------------------------------------------------------------------------'
azcopy --source $MAYA_INSTALL_SAS --destination /tmp/maya/Autodesk_MayaIO_2018_3_Update_Linux_64bit.run

echo '-------------------------------------------------------------------------------------'
echo '------------------------------=Extract and Install Maya 2018=-----------------------------------'
echo '-------------------------------------------------------------------------------------'
7za e /tmp/maya/Autodesk_MayaIO_2018_3_Update_Linux_64bit.run -o/tmp/maya/
7za e /tmp/maya/Autodesk_MayaIO_2018_3_Update_Linux_64bit -o/tmp/maya/Maya2018/
chmod +x /tmp/maya/Maya2018/setup.sh
/tmp/maya/Maya2018/setup.sh 127.0.0.1

cp /usr/bin/render2018.sh /usr/bin/render.sh

chmod +x /usr/bin/render2018.sh
chmod +x /usr/bin/render.sh

rm -rf /tmp/maya2018
rm -rf /tmp/maya

yum clean all
