#!/bin/bash
#
# NetDrone Engine
# Copyright © 2015-2016 Origin Studio Inc.
#

if [ -d ./bin ]; then
	echo "remove bin directory"
	rm -rf ./bin
fi

if [ -d ./obj ]; then
	echo "remove obj directory"
	rm -rf ./obj
fi

xbuild /p:Configuration=Debug ./studyserver.sln
xbuild /p:Configuration=Release ./studyserver.sln

