#!/bin/sh
mlagents-learn config/trainer_config.yaml --env=dist/trainingLinux.x86_64 --no-graphics --num-envs=2 --run-id=$1 --train