ARG PARENT_TAG
ARG ACR_SERVER

FROM $ACR_SERVER/public/azure-batch/rendering/maya:$PARENT_TAG

ARG INSTALLER_SAS

COPY installMaya2017.sh /tmp/installMaya2017.sh
COPY maya2017.sh /usr/bin/maya2017.sh

RUN sh /tmp/installMaya2017.sh $INSTALLER_SAS 