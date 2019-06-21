FROM azurebatchrendering/maya_2018u3:latest-centos

ARG INSTALLER_SAS

COPY installArnold.sh /tmp/installArnold.sh
COPY mtoa.mod /usr/autodesk/mayaIO2018/modules/mtoa.mod

RUN sh /tmp/installArnold.sh $INSTALLER_SAS