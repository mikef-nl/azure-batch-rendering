FROM azurebatchrendering/maya:latest-centos

ARG INSTALLER_SAS

COPY installMaya2019.sh /tmp/installMaya2019.sh
COPY maya2019.sh /usr/bin/maya2019.sh

RUN sh /tmp/installMaya2019.sh $INSTALLER_SAS 