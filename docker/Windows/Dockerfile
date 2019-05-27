FROM microsoft/windowsservercore

COPY installDependencies.ps1 C:/temp/installDependencies.ps1

SHELL ["cmd", "/S", "/C"] 
RUN powershell -noexit "& ""C:/temp/installDependencies.ps1"""