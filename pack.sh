#!/bin/bash

releaseDir="PlanningPoker/bin/Release"

files=("Changelog.txt", "log4net.dll", "Newtonsoft.Json.dll", "PlanningPoker.NET.exe", "PlanningPoker.NET.exe.config", "RestSharp.dll")

for i in ${files[@]}; do
    i=$releaseDir.$i
    echo $i
done

echo ${files[@]} 

#gzip << echo ${files[@]} 


exit 0
