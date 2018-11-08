FROM azurebatchrendering/centos:latest

ARG INSTALLER_SAS

COPY installMaya2018.sh /tmp/installMaya2018.sh
COPY maya2018.sh /usr/bin/maya2018.sh

RUN sh /tmp/installMaya2018.sh $INSTALLER_SAS 