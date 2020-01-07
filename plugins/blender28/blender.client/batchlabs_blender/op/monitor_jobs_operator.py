import logging

import bpy

from ..constants import Constants

class MonitorJobsOperator(bpy.types.Operator):
    """Open Batch Explorer to monitor your jobs."""
    bl_idname = "object.monitor_jobs_operator"
    bl_label = "MonitorJobsOperator"

    def __init__(self):
        self.log = logging.getLogger(Constants.LOG_NAME)

    def execute(self, context):
        self.log.debug("MonitorJobsOperator.execute")
        handler = context.scene.batch_session.request_handler
        handler.call_batch_labs("jobs")

        return {"FINISHED"}
