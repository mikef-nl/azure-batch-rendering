
from azext.batch import BatchExtensionsClient
from azure.batch import BatchServiceClient
from azure.batch.batch_auth import SharedKeyCredentials
from azure.common.credentials import ServicePrincipalCredentials
from azure.storage.blob import BlockBlobService

from models.job import Job
from models.settings import TemplateSettings

class JobConfiguration(object):
    """
    Store everything needed to submit a job with an NCJ job template
    and parameters file.
    """
    def __init__(self, batch_client: BatchServiceClient, storage_client: BlockBlobService,
                 extensions_client: BatchExtensionsClient, job_template: object, job_parameters: object,
                 pool_name: str, storage_acc_url: str, template_settings: TemplateSettings):
        '''
        :param `azure.batch.BatchServiceClient` batch_client:
            A client for issuing REST requests to the Azure Batch service.
        :param `azure.storage.blob.BlockBlobService` storage_client:
            A client for issuing REST requests to the Azure blob storage service.
        :param `azext.batch.BatchExtensionsClient` extensions_client:
            A client for issuing REST requests to the Azure Batch service.
        :param `object` job_template:
            The JSON from the job.template.json file.
        :param `object` job_parameters:
            The JSON from the job.parameters.json file.
        :param `str` pool_name:
            The name of the pool to run jobs on.
        :param `str` storage_acc_url:
            Base URL of the storage account.
        :param `models.settings.TemplateSettings` template_settings:
            Self contained template settings class. Holds paths to the JSON tempaltes 
            and the parameter mapper python module.
        '''
        self.batch_client = batch_client
        self.storage_client = storage_client
        self.extensions_client = extensions_client
        self.job_template = job_template
        self.job_parameters = job_parameters
        self.pool_name = pool_name
        self.storage_acc_url = storage_acc_url
        self.template_settings = template_settings
