import logging

import bpy
from ..constants import Constants

class MonitorPoolsOperator(bpy.types.Operator):
    """ 

    """
    bl_idname = "object.monitor_pools_operator"
    bl_label = "Invokes a Script"

    @classmethod
    def poll(cls, context):
        return context.active_object is not None

    def __init__(self):
        self.log = logging.getLogger(Constants.LOG_NAME)

    def execute(self, context):
        self.log.debug("MonitorPoolsOperator.execute")
        handler = context.scene.batch_session.request_handler
        handler.call_batch_labs("pools")
        return {"FINISHED"}
