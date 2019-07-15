ARG PARENT_TAG
ARG ACR_SERVER

FROM $ACR_SERVER/public/azure-batch/rendering/maya:$PARENT_TAG

ARG INSTALLER_SAS

COPY installArnold.sh /tmp/installArnold.sh
COPY mtoa.mod /usr/autodesk/mayaIO2018/modules/mtoa.mod

RUN sh /tmp/installArnold.sh $INSTALLER_SAS