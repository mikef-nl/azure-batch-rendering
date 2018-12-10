
from azure.batch.models import BatchErrorException


def print_batch_exception(ex: BatchErrorException):
    """
    Prints the contents of the specified Batch exception.
    
    :param `azure.batch.models.BatchErrorException` ex:
        The exception to convert into something readable
    """
    if ex.error and ex.error.message and ex.error.message.value:
        print(ex.error.message.value)
        if ex.error.values:
            for mesg in ex.error.values:
                print('{}:\t{}'.format(mesg.key, mesg.value))
