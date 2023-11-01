#!/bin/bash

while true; do
  if [ -z "$(docker-compose ps -q app)" ] ; then
    echo "App is not running. Docker compose stop..."
    docker-compose stop tunnel monitor
    exit 0
  fi
  sleep 10
done