import logging

import bpy

from batchlabs_blender.constants import Constants

class MONITOR_JOBS_OT_Operator(bpy.types.Operator):
    """ Monitor Batch Jobs """
    bl_idname = Constants.OP_ID_MONITOR_JOBS
    bl_label = "MONITOR_JOBS_OT_Operator"

    def __init__(self):
        self.log = logging.getLogger(Constants.LOG_NAME)

    def execute(self, context):
        self.log.debug("MONITOR_JOBS_OT_Operator.execute")
        handler = context.scene.batch_session.request_handler
        handler.call_batch_labs("jobs")

        return {"FINISHED"}
