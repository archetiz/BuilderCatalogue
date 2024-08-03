#!/bin/bash

source config.sh

apiSpecFile=$1
outputFilePrefix="out_"

sed -i -e "s/${usernamePlaceholder}/{username}/g" $apiSpecFile
sed -i -e "s/by-id-id/id/g" $apiSpecFile
sed -i -e "s/${setNamePlaceholder}/{name}/g" $apiSpecFile

jq -r '.paths["/api/user/by-username/{username}"] += { "parameters": [ { "name": "username", "in": "path", "required": true, "schema": { "type": "string" } } ] }
	 | .paths["/api/set/by-name/{name}"] += { "parameters": [ { "name": "name", "in": "path", "required": true, "schema": { "type": "string" } } ] }' \
	$apiSpecFile > "${outputFilePrefix}${apiSpecFile}"

echo "Cleanup done."
