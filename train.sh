#!/bin/sh
mlagents-learn config/trainer_config.yaml --env=dist/training.app --no-graphics --num-envs=2 --run-id=$1 --train