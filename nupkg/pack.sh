#!/bin/bash

source='https://api.nuget.org/v3/index.json'
api_key=$1

if [ -z $api_key ]; then
    echo "Error: 请输入有效的 api key"
    exit 1
fi

#dotnet pack ../BeniceSoft.Abp.sln -o .
#
## shellcheck disable=SC2045
#for file in `grep BeniceSoft.Abp.Sample . -r -L`
##for file in `grep BeniceSoft.OAuth.DingTalk . -r -l`
#do
#  if [[ $file != '.' && $file != '..' && "${file##*.}"x = "nupkg"x ]]
#  then
#    echo "push nuget package ${file}"
#    
#    dotnet nuget push $file -s $source --api-key $api_key
#    echo "package ${file} pushed!"
#
#    rm -rf $file
#    echo "package ${file} deleted!"
#  fi
#done
#
## shellcheck disable=SC2045
#for file in `grep BeniceSoft.Abp.Sample . -r -l`
#do
#  if [[ $file != '.' && $file != '..' && "${file##*.}"x = "nupkg"x ]]
#  then
#    rm -rf $file
#    echo "package ${file} deleted!"
#  fi
#done
#
#
