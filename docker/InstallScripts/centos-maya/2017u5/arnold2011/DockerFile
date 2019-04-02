FROM azurebatchrendering/centos_maya2017update5:latest

ARG INSTALLER_SAS

COPY installArnold.sh /tmp/installArnold.sh
COPY mtoa.mod /usr/autodesk/mayaIO2017/modules/mtoa.mod

RUN sh /tmp/installArnold.sh $INSTALLER_SAS