#!/bin/sh
rm config/curricula/chess/.DS_Store; mlagents-learn config/trainer_config.yaml --curriculum=config/curricula/chess/ --run-id=$1 --train