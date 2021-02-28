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
## Demonstration
<div align=center><img src="git@github.com:Maximilian92/T01-Tool-path-planning-on-5-axis-machining-center-using-DRL/image/Simulation scenario in Unity3D editor"></div>
<div align=center><img src="git@github.com:Maximilian92/T01-Tool-path-planning-on-5-axis-machining-center-using-DRL/image/Demo"></div>