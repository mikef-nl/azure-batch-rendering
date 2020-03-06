import importlib
import os
import logging

import bpy

from bpy.app.handlers import persistent

from batchlabs_blender.preferences import UserPreferences
from batchlabs_blender.shared import BatchSettings
from batchlabs_blender.constants import Constants

from batchlabs_blender.menu import BATCH_LABS_MT_Submit_Menu
from batchlabs_blender.menu import BATCH_LABS_MT_BlenderMenu

from batchlabs_blender.op.download_renders_operator import DOWNLOAD_RENDERS_OT_Operator
from batchlabs_blender.op.monitor_jobs_operator import MONITOR_JOBS_OT_Operator
from batchlabs_blender.op.monitor_pools_operator import MONITOR_POOLS_OT_Operator
from batchlabs_blender.op.submit_job_operator import SUBMIT_JOB_OT_Operator

bl_info = {
    "name": "BatchLabs Blender Plugin",
    "author": "Microsoft Corporation <bigcompute@microsoft.com>",
    "version": (0, 2, 0),
    "blender": (2, 80, 0),
    "location": "Render Menu",
    "description": "Render your Blender scene externally with Azure Batch and BatchLabs.",
    "category": "Render"
}

_APP_DIR = os.path.dirname(__file__)

@persistent
def start_session(self):
    """
    Initializes the Batch session and registers all the property
    classes to the Blender context.
    This is handled in an event to allow it to run under the full
    Blender context rather than the limited loading scope.

    Once the session has started (or reported an error), this function
    is removed from the event handlers.
    """
    log = logging.getLogger(Constants.LOG_NAME)
    try:
        session = BatchSettings()

        def get_session(self):
            return session

        bpy.types.Scene.batch_session = property(get_session)
        log.info(property(get_session))

    except Exception as error:
        print("BatchLabs plugin failed to load.")
        print("Error: {0}".format(error))
        bpy.types.Scene.batch_error = error

    finally:
        bpy.app.handlers.depsgraph_update_post.remove(start_session)


def menu_func(self, context):
    """
    Add the BatchLabs menu options to the 'Render' menu in the main toolbar
    """
    self.layout.menu("BATCH_LABS_MT_BlenderMenu")

classes = ( UserPreferences, DOWNLOAD_RENDERS_OT_Operator, MONITOR_POOLS_OT_Operator, MONITOR_JOBS_OT_Operator, SUBMIT_JOB_OT_Operator, BATCH_LABS_MT_Submit_Menu, BATCH_LABS_MT_BlenderMenu )
batch_lab_classes_register, batch_lab_classes_unregister = bpy.utils.register_classes_factory(classes)

def register():
    """
    Register module and applicable classes.
    Here we also register the User Preferences for the Addon, so it can
    be configured in the Blender User Preferences window.
    """
    bpy.app.handlers.depsgraph_update_post.append(start_session)
    batch_lab_classes_register()
    bpy.types.TOPBAR_MT_render.append(menu_func)


def unregister():
    """
    Unregister the addon if deselected from the User Preferences window.
    """
    batch_lab_classes_unregister()
    bpy.types.TOPBAR_MT_render.remove(menu_func)

if __name__ == "__main__":
    register()
