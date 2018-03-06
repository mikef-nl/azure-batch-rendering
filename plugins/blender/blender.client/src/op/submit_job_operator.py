import logging
import os

import bpy

from src.constants import Constants
from src.shared import BatchSettings

class SubmitJobOperator(bpy.types.Operator):
    bl_idname = Constants.OP_ID_SUBMIT_JOB
    bl_label = "SubmitJobOperator"
    job_type = bpy.props.StringProperty()
    
    _log = None
    _preferences = None

    def __init__(self):
        self._log = logging.getLogger(Constants.LOG_NAME)
        self._preferences = BatchSettings.get_user_preferences()

    def execute(self, context):
        # todo: check for and throw error if no job_type set
        self._log.debug("SubmitJobOperator.execute: " + self.job_type)

        handler = context.scene.batch_session.request_handler
        launch_url = str.format("market/blender/actions/{}/{}", self.job_type, "submit")
        arguments = {Constants.KEY_USE_AUTO_POOL: self._preferences.pool_type}

        if bpy.data.filepath:
            arguments[Constants.KEY_SCENE_FILE] = os.path.basename(bpy.data.filepath)

        self._log.debug("SubmitJobOperator - passing args: " + str(arguments))
        handler.call_batch_labs(launch_url, arguments)

        return {"FINISHED"}
