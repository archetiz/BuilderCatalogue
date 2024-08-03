#!/bin/bash

source config.sh

devproxy --config-file generate-openapi-spec-config.json --record &

sleep 5 # Give a few seconds for devproxy to start

call_api() {
	curl -ix "http://localhost:8000" "https://d16m5wbro86fg2.cloudfront.net/api${1}" --ssl-no-revoke
}

call_api "/users"
call_api "/user/by-username/${usernamePlaceholder}"
call_api "/user/by-id/${userIdPlaceholder}"
call_api "/sets"
call_api "/set/by-name/${setNamePlaceholder}"
call_api "/set/by-id/${setIdPlaceholder}"
call_api "/colours"

echo "Recording done. Press Ctrl + C to generate API spec."

wait
