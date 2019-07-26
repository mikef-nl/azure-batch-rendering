#!/bin/bash
export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/usr/autodesk/mayaIO2019/plug-ins/xgen/lib
/usr/autodesk/mayaIO2019/bin/Render "$@"
exit $?