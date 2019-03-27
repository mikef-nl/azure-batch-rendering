FROM azurebatchrendering/windows:latest

ARG INSTALLER_SAS

COPY install3dsmax2018.ps1 C:/temp/install3dsmax2018.ps1

RUN powershell C:/temp/install3dsmax2018.ps1 "'%INSTALLER_SAS%'"
