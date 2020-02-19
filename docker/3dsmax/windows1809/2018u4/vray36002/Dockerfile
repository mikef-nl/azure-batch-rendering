ARG PARENT_TAG
ARG ACR_SERVER_REPO

FROM $ACR_SERVER_REPO/3dsmax:$PARENT_TAG

ARG INSTALLER_SAS

COPY installVRay.ps1 C:/temp/installVRay.ps1
COPY VRayFor3dsMax2018config.xml C:/temp/vray/VRayFor3dsMax2018config.xml

RUN powershell -ExecutionPolicy ByPass -noexit C:/temp/installVray.ps1 "'%INSTALLER_SAS%'"
