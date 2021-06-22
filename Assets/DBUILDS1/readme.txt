
1. CREATION OF DESTRUCTIBLE BUILDINGS

you can create buildings by snapping prefabs (building elements) together. you can find the prefabs in the _prefabs folder.

interior: 
- you find interior wall prefabs in the prefabs/interior folder.
- the floors have space for stairs (included) or elevator.

 
2. TAGS AND LAYERS 

for destruction scripts to work, 'dbuild' and 'chunks' tags and 'dbuild' and 'chunks' layers are needed

dbuild :: used on kinematic colliders
chunks :: used on fractured chunks

- these are already set-up in the prefabs.


3. HOW IT WORKS

the buildings are made up of kinematic objects, which on collision are switched to prefactured rigidbody objects, and explosion force is added to them.
