
class Job(object):
    """
    Self contained single job definition created from the job definition JSON in
    the manifest file.
    """
    def __init__(self, 
        index, job_id, scene_file, container: str = None, folder: str = None):
        """
        :param `int` index:
            Index of the job in the manifest file. Used for setting the job priority.
        :param `str` job_id:
            Unique identifier the job.
        :param `str` scene_file:
            The path to the scene file in the storage container.
        :param `str` container:
            Name of the container in which the job assets are storred.
        :param `str` folder:
            Path to the local folder on disk that contains the assets for the job.
        """
        self.priority = index
        self.job_id = job_id
        self.scene_file = scene_file
        self.container = container
        self.folder = folder

        self.frame_start = 0
        self.frame_end = 0

    def get_container_name_without_prefix(self): 
        return self.container.replace("fgrp-", "")

    def get_start_frame(self): 
        return self.frame_start

    def set_start_frame(self, frame_start: int): 
        self.frame_start = frame_start

    def get_end_frame(self): 
        return self.frame_end

    def set_end_frame(self, frame_end: int): 
        self.frame_end = frame_end

    def get_has_container(self): 
        self.container is not None
    
    def get_has_folder(self): 
        self.folder is not None

    has_folder = property(get_has_folder)
    has_container = property(get_has_container)
    container_name_without_prefix = property(get_container_name_without_prefix)
    start_frame = property(get_start_frame, set_start_frame)
    end_frame = property(get_end_frame, set_end_frame)
