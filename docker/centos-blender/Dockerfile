FROM centos:centos7

RUN yum update -y && \
  yum -y install \
    sudo \
    libunwind \
    libicu \
    ImageMagick \
    epel-release \
    p7zip \
    bzip2 \
    mesa-libGLU \
    libXi \
    wget \
    rsync \
    which
  
RUN sudo rpm -Uvh https://packages.microsoft.com/config/rhel/7/packages-microsoft-prod.rpm && \
    sudo sh -c 'echo -e "[packages-microsoft-com-prod]\nname=packages-microsoft-com-prod \nbaseurl=https://packages.microsoft.com/yumrepos/microsoft-rhel7.3-prod\nenabled=1\ngpgcheck=1\ngpgkey=https://packages.microsoft.com/keys/microsoft.asc" > /etc/yum.repos.d/dotnetdev.repo' && \
    sudo rpm --import https://packages.microsoft.com/keys/microsoft.asc && \
    sudo sh -c 'echo -e "[azure-cli]\nname=Azure CLI\nbaseurl=https://packages.microsoft.com/yumrepos/azure-cli\nenabled=1\ngpgcheck=0" > /etc/yum.repos.d/azure-cli.repo'
    
RUN sudo yum -y install \
    dotnet-sdk-2.2 \
    azure-cli
    
RUN sudo wget -O azcopy.tar.gz https://aka.ms/downloadazcopyprlinux && \
  tar -xf azcopy.tar.gz && \
  ./install.sh && \
  rm azcopy.tar.gz

RUN sudo yum clean all