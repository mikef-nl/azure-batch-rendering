ARG PARENT_TAG
ARG ACR_SERVER_REPO

FROM $ACR_SERVER_REPO/3dsmax:$PARENT_TAG

ARG INSTALLER_SAS

COPY installArnold.ps1 C:/temp/installArnold.ps1

RUN powershell -ExecutionPolicy ByPass -noexit C:/temp/installArnold.ps1 "'%INSTALLER_SAS%'"
