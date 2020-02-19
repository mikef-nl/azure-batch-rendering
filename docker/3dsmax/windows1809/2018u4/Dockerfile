ARG PARENT_TAG
ARG ACR_SERVER_REPO

FROM $ACR_SERVER_REPO/3dsmax:$PARENT_TAG

ARG INSTALLER_SAS

COPY install3dsmax2018.ps1 C:/temp/install3dsmax2018.ps1

RUN powershell -ExecutionPolicy ByPass -noexit C:/temp/install3dsmax2018.ps1 "'%INSTALLER_SAS%'"
