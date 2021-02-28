# Tool-path-planning-on-5-axis-machining-center-using-DRL

## Objective
This project attempts to use DRL algorithm PPO integrated with ICM to finish tool path plnning task on 5-axis machining center automatically.

## Implementation
* To build up the simulation scenario, models of machine tool and a collection of material as well as corresponding products are first built.
* The models are imported as assets into Unity3D editor so that the simulation scenario is ready.
* The training process is implemented using [ML-Agents Toolkit](https://github.com/Unity-Technologies/ml-agents) (version 0.15.1) provided by Unity technologies.
* The trained strategies are finally tested and evaluated.

## Usage
* After configuration of ML-Agents Toolkit, all the contents in project need to be copied to the asset directory of the Unity3D project
* Before training or test, some configurations need to be modified in the inspector tab to control the simulation details.

## Simulation scenario
<div align=center><img src="https://github.com/Maximilian92/T02-Tool-path-planning-on-5-axis-machining-center-using-DRL/blob/master/image/Simulation%20scenario%20in%20Unity3D%20editor.png"></div>

## Demonstration
<div align=center><img src="https://github.com/Maximilian92/T02-Tool-path-planning-on-5-axis-machining-center-using-DRL/blob/master/image/DEMO.gif"></div>