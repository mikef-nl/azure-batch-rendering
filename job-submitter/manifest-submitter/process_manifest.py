import argparse
import copy
import importlib
import os

from azure.batch.models import BatchErrorException
from cloud import batchapi
from cloud.authentication import AuthenticationProvider
from models import AuthSettings
from models import Job
from models import JobConfiguration
from models import TemplateSettings
from typing import Tuple
from utils import exception_utils
from utils import file_utils


# todo: add logging to file.
# todo: ask matthchr how to get the code out of one of these: BatchErrorException
def main():
    print("\n### running python job submitter ###")

    # get command line arguments
    args = _parse_runner_arguments()

    # load the auth, settings, jobs and pool from the JSON manifest file
    auth_settings, template_settings, jobs, pool = _load_job_manifest(args)
    auth_provider = AuthenticationProvider(auth_settings)

    # load job and parameters templates
    job_template = file_utils.load_json_template(
        template_settings.job_template_file_path)
    job_parameters = file_utils.load_json_template(
        template_settings.job_parameter_file_path)
    job_config = JobConfiguration(
        batch_client=auth_provider.create_batch_client(),
        storage_client=auth_provider.create_storage_client(),
        extensions_client=auth_provider.create_batch_extensions_client(),
        job_template=job_template,
        job_parameters=job_parameters,
        pool_name=pool,
        storage_acc_url=auth_settings.storage_acc_url,
        template_settings=template_settings)

    if args.operation.lower() == "test":
        print("\n### operating in 'TEST' mode, no jobs will be submitted ###")
        for job in jobs:
            _test_job(job, job_config)

    elif args.operation.lower() == "run":
        print("\n### operating in 'RUN' mode ###")
        _run_manifest(jobs, job_config)

    else:
        print(
            "unrecognised run operation: '{0}', exiting".format(args.operation))

    print("exiting application normally")


def _parse_runner_arguments():
    """
    Parse the command line arguments passed to the script.
    """
    print("parsing command line arguments")
    parser = argparse.ArgumentParser(
        description="Submit a collection of jobs to Azure Batch")
    parser.add_argument("--manifest", type=str, help="Manifest file location.")
    parser.add_argument("--operation", nargs="?", type=str, default="run",
                        help="What operation to run? Can choose 'run' or 'test'.")
    parser.add_argument("--pool", nargs="?", type=str,
                        help="Pool name to run the jobs on. At this stage, it must already exist.")

    args = parser.parse_args()
    print("args.manifest: '{}'".format(args.manifest))
    print("args.operation: '{}'".format(args.operation))
    print("args.pool: '{}'".format(args.pool))

    if args.manifest == None:
        raise Exception(
            "manifest file was not supplied on the command line. application cannot continue.")

    return args


def _load_job_manifest(
        args: object
) -> Tuple[AuthSettings, TemplateSettings, list, str]:
    """
    Load the manifest file and parse the JSON to get the job definitions.
    Creates a Job object for each definition in the manifest.
    """
    print("using manifest file: '{}'".format(args.manifest))
    manifest = file_utils.load_json_template(args.manifest)

    # TODO: add some checks and tests for this method

    pool = ""
    print("checking if pool is defined in the manifest or command line")

    if args.pool != None:
        pool = args.pool
        print("using pool: '{}' from command line".format(pool))

    elif manifest.get("pool") != None and manifest["pool"].strip() != "":
        pool = manifest["pool"]
        print("using pool: '{}' from manifest".format(pool))

    elif args.operation.lower() == "run":
        raise Exception(
            "No pool was defined in either the manifest or the command line. application cannot continue.")

    print("loading authentication settings from manifest")
    auth = AuthSettings(
        manifest["auth"]["batchAccName"],
        manifest["auth"]["batchAccUrl"],
        manifest["auth"]["batchAccSub"],
        manifest["auth"]["storageAccName"],
        manifest["auth"]["storageAccUrl"],
        manifest["auth"]["storageAccSharedKey"],
        manifest["auth"]["servicePrincipalClientId"],
        manifest["auth"]["servicePrincipalSecret"],
        manifest["auth"]["servicePrincipalTenant"])

    print("loading template settings from manifest")
    settings = TemplateSettings(
        manifest["settings"]["jobTemplateFilePath"],
        manifest["settings"]["jobParametersFilePath"],
        manifest["settings"]["parameterMapperPath"],
        manifest["settings"]["mapperModuleName"])

    jobs = []
    counter = 1
    print("loading job definitions from manifest")

    for jobDef in manifest["jobs"]:
        job = Job(counter, jobDef["jobId"], jobDef["sceneFile"])

        if jobDef.get("container") != None:
            job.container = jobDef["container"]

        if jobDef.get("folder") != None:
            job.folder = jobDef["folder"]

        if jobDef.get("frameStart") != None:
            job.start_frame = int(jobDef["frameStart"])

        if jobDef.get("frameEnd") != None:
            job.end_frame = int(jobDef["frameEnd"])

        print("adding job: {0}".format(job.job_id))
        jobs.append(job)
        counter += 1

    return auth, settings, jobs, pool


def _test_job(job: Job, job_config: JobConfiguration):
    """
    Test mode only.
    - checks there is no job with the specified job ID
    - checks storge container and scnene file exists for each job
    """
    print("testing job: {} - scene file: {}, container: {}, folder: {}".format(
        job.job_id, job.scene_file, job.container, job.folder))

    try:
        job_exists = batchapi.does_job_exist(job_config.batch_client,
                                             job.job_id)
        if job_exists == True:
            print(
                "WARN :: job with id '{}' already exists. Job definition cannot be submitted.".format(
                    job.job_id))
        else:
            print("PASS :: job does not already exist")

        scene_exists = batchapi.scene_file_exists(job_config.storage_client,
                                                  job.container, job.scene_file)
        if scene_exists == True:
            print("PASS :: container and scene file exists")
        else:
            print(
                "WARN :: container '{}' and or scene file: {} does not exist. job definition cannot be submitted.".format(
                    job.container, job.scene_file))

    except Exception:
        # error has already been logged
        print(
            "ERROR :: Failed to request job: '{}' or scene file: '{}/{}'".format(
                job.job_id, job.container, job.scene_file))


def _run_manifest(jobs: [Job], job_config: JobConfiguration):
    """
    End-to-end run mode. 
    - foreach entry in the manifest, submit a job using the pre-defined template
    """
    try:
        abs_path = os.path.abspath(
            job_config.template_settings.parameter_mapper_file_path)
        print("loading parameter mapper module: ", abs_path)

        # dynamically load the custom module for mapping job parameters
        spec = importlib.util.spec_from_file_location(
            job_config.template_settings.mapper_module_name, abs_path)
        module = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(module)

        # create an instance of the mapper and execute the map_parameters method
        mapper = module.ParameterMapper()
    except Exception as ex:
        print("failed to dymanically load the parameter mapper: ", ex)
        raise

    print("about to submit '{}' jobs to '{}'".format(len(jobs),
                                                     job_config.batch_client.config.base_url))
    for job in jobs:
        try:
            _submit_job(job, job_config, mapper)
        except:
            print("caught exception while processing job: ", job.job_id)


def _submit_job(job: Job, job_config: JobConfiguration, mapper: object):
    """
    Submits a job to the Batch service via the Batch Extensions client.
    """
    try:
        job_exists = batchapi.does_job_exist(job_config.batch_client,
                                             job.job_id)
        if job_exists == True:
            print("ALERT :: job already exists with id: {}, skipping.".format(
                job.job_id))
            return

        pool_name = job_config.pool_name
        print("configuring job with identifier: '{}'".format(job.job_id))

        # call out to the dynamic module to map the parameters
        mapped_params = mapper.map_parameters(
            job_config.storage_acc_url,
            job_config.storage_client,
            copy.copy(job_config.job_parameters),
            job.job_id,
            job.container,
            job.scene_file,
            job.priority,
            job.start_frame,
            job.end_frame,
            pool_name
        )
        print("mapped parameters: ", mapped_params)

        # get batch extensions client to map the parameters to the job template
        job_json = job_config.extensions_client.job.expand_template(
            job_config.job_template, mapped_params)
        jobparameters = job_config.extensions_client.job.jobparameter_from_json(
            job_json)

        # submit the job
        print("submitting job to batch extensions client: ", job.job_id)
        job_config.extensions_client.job.add(jobparameters)

    except BatchErrorException as bex:
        print("caught batch exception while submitting job: ", job.job_id)
        exception_utils.print_batch_exception(bex)

    except Exception as ex:
        print(
            "caught generic exception while submitting job: '{}'. with error:".format(
                job.job_id), ex)


if __name__ == '__main__':
    main()
