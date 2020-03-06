import logging

import bpy

from batchlabs_blender.constants import Constants

class DOWNLOAD_RENDERS_OT_Operator(bpy.types.Operator):
    """ Manage Data """
    
    bl_idname = Constants.OP_ID_DOWNLOAD_RENDERS
    bl_label = "DOWNLOAD_RENDERS_OT_Operator"

    def __init__(self):
        self.log = logging.getLogger(Constants.LOG_NAME)

    def execute(self, context):
        self.log.debug("DOWNLOAD_RENDERS_OT_Operator.execute")
        handler = context.scene.batch_session.request_handler
        handler.call_batch_labs("data")

        return {"FINISHED"}
