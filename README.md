# SisalCase
This project is made for sisal


The game is using single scene to avoid loading times. First you need to provide a name for Player. 

Getting sunset time is working based on location. If can find any location in 3 seconds (this time can be adjusted on Scene-Managers-SunsetAPI - max wait for location) 
and get sunset based on last location. Else using default ones (also this can change  on Scene-Managers-SunsetAPI). If it has any
problem to get sunset, it uses a mockup data (from 15.05.2023 Istanbul)

Gameplay is like a classic pool game. For each round you will get +10 points for the ball that you touch and touched ball will disabled. If the white ball would not enter the pocket,
the score will counted.
After each round the touched balls will find some place else

When white ball entered a pocket, you can replace white ball by dragging the mouse on the table. It will place the white ball wherever you release the mouse.
