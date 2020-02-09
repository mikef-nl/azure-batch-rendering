ARG PARENT_TAG
ARG ACR_SERVER_REPO

FROM $ACR_SERVER_REPO/blender:$PARENT_TAG

RUN curl -SL "http://builder.blender.org/download/blender-2.80-030c7df19da9-linux-glibc224-x86_64.tar.bz2" -o blender.tar.bz2 && \
	tar -jxvf blender.tar.bz2 -C /usr/local/bin --strip-components=1 && \
	rm blender.tar.bz2

RUN apt-get update && \
  apt-get install python-pyasn1 walinuxagent -y && \
  apt-get clean
