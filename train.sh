#!/bin/sh
mlagents-learn Assets/ML-Agents/config/trainer_config.yaml --env=dist/training.app --no-graphics --num-envs=8 --run-id=$1 --train