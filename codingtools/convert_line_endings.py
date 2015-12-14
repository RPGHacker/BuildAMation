#!/usr/bin/python

import os
import sys


def convert_line_endings(file):
    if '\r\n' in open(file, 'rb').read():
        print '%s contains DOS line endings. Converting' % file
        with open(file, 'rb') as infile:
            text = infile.read()
            text = text.replace('\r\n', '\n')
        with open(file, 'wb') as outfile:
            outfile.write(text)


def process_path(path, extension_list):
    if os.path.isfile(path):
        convert_line_endings(path)
    else:
        for root, dirs, files in os.walk(path):
            for file in files:
                file_ext = os.path.splitext(file)[1]
                if file_ext in extension_list:
                    full_path = os.path.join(root, file)
                    convert_line_endings(full_path)

if __name__ == "__main__":
    if len(sys.argv) > 1:
        extensions = sys.argv[2:]
        if not extensions:
            extensions = ['.cs']
        process_path(sys.argv[1], extensions)
    else:
        process_path('.', ['.cs'])
        process_path('tests', ['.h', '.c', '.cpp', '.m', '.mm', '.py', '.sh', '.bat'])
