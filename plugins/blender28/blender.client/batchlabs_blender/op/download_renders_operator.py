import logging

import bpy
from ..constants import Constants

class DownloadRendersOperator(bpy.types.Operator):
    """Open Batch Explorer to monitor your files."""
    bl_idname = "object.download_renders_operator"
    bl_label = "DownloadRendersOperator"

    def __init__(self):
        self.log = logging.getLogger(Constants.LOG_NAME)

    def execute(self, context):
        self.log.debug("DownloadRendersOperator.execute")
        handler = context.scene.batch_session.request_handler
        handler.call_batch_labs("data")

        return {"FINISHED"}
