
import json

def load_json_template(file_path: str, print_result: bool = False):
    """
    Loads and parses a JSON file on disk.
    
    :param `str` file_path: 
        Path to the json template file
    :param `bool` print_result:
        Print the contents of the file to standard out.
    """
    try:
        print("loading json file: '{0}'".format(file_path))
        with open(file_path) as stream:
            data = json.load(stream)

        if print_result == True:
            print("{}\n".format(file_path), data)

        return data

    except Exception as ex:
        print("failed to load json template: {0}, with error: {1}".format(file_path, str(ex)))
        raise
