# AI_Steeringbehavour_Boids_Chase
 A tiny game that came as a result of an AI course, based on the concept of boids and various steering behaviours

 This small game project is more for showing my capability to use an AI algorithm to make a simple steering-behaviour simulation, moreso than actually being a game. As such the goal of this small game is simply to try to "swat" as many boids/"flies" as possible before the timer for a round runs out.

The game is divided into four main components, the "fly swatter" which carries the simple responsibility of enabling the player to play the game by "swatting" boids/"flies".
![Fly Swatter](/images/fly_swatter.png)

The camera-mover which as can be guessed by the name allows the player to move around the screen with the camera.
![Camera Mover](/images/camera_mover.png)

The "round" which handles restarting and timing for a given game round.
![Round](/images/round.png)

Last but not least perhaps the most important component of them all, the "steering-controller" which handles controlling all individual boids/"flies". The implementation of the steering-behaviours is in this case based on an abstraction and inheritance implementation.
![Steering Controller](/images/steering_controller.png)

The following are visual examples of different behaviours:

Aligning behaviour example.
![Align behaviour](/images/align_behaviour.png)

Avoidance behaviour example.
![Avoidance behaviour](/images/avoidance_behaviour.png)

Cohesion behaviour example.
![Cohesion behaviour](/images/cohesion_behaviour.png)

Flee behaviour example.
![Flee behaviour](/images/flee_behaviour.png)

Separation behaviour example.
![Separation behaviour](/images/separation_behaviour.png)

Wander behaviour example.
![Wander behaviour](/images/wander_behaviour.png)