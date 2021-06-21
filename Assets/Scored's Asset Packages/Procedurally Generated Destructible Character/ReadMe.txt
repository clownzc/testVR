***************************************************
* The Procedualy Generated Destructible Character *
* By Duncan 'ScoredOne' Mellor                    *
* @ScoredOne / ScoredOne1994@gmail.com            *
***************************************************

-- Glossery --

1. Introduction
2. Basic Setup
3. Prefabs
4. Inspector Variables
5. How to Animate
6. Block Stats Guide
7. Shooting Scene
8. Custom Characters

-- Introduction --

Hello and thank you for downloading the Procedurally Generated Destructible Body, this readme is the guide on how to use it and also everything included in this package.
This is hopefully just the first version of many that I hope to make, progressing this concept further each time.
This product is to be used like an imported character, you gain the model, generated avatar, all ready for set up and animation to be put into your games.

-- Basic Setup --

The "BodyTemplate" is the base for the generation, it has numbers for it preloaded into it but feel free to change it. If you made a change and lost the original
the Use Defaults toggle will build the original structure.
Just drag it into a scene and click build but in its current state it will generate over 1000 blocks. Their is a low poly build as well which will generate around 600
(Other Templates are NOT protected by Use Defaults)
Its current size will be massive so to bring it down just change the base layers scale AFTER its is generated, I recommend 0.1 on xyz.
Because of the nature of its creation, the Avatar will NOT be saved when trying to save it meaning it will not work after, if you wish to save it press the Save Avatar File
button and it will save the avatar file to the folder "Saved Avatars". Be aware deleting the Avatar after it has been saved will delete the avatar from the character and
may be lost.

-- Prefabs --

(look at premade prefabs as example)
BodyCube requires nothing extra, this is the base block for all the cubes in the premade structure so be careful when adding to it as it will be spawned hundreds of times
Foot requires "FootCenter" and "ToeEnd" to be attached
Hand requires "HandCenter" to be attached
Head requires "HeadCenter" to be attached
Joint_Peice requires "ConnectionPoint" and "JointCenter" to be attached
Limb_Piece, LowerBody_Piece, UpperBody_Piece (IF NOT GENERATING) require "ConnectionPoint" to be attached

It should be ready after generation to have components attached to it to make it a real playable character; animation, controller ext.
The blocks of the character CAN be removed after generation to add to the customization, whether that be individual blocks or whole limbs just dont destroy the skeleton,
These objects are named accordingly including the word Avatar in the name.

-- Inspector Variables --
AvatarBodyBlock : used to give shape to the AvatarMesh
UseDefaults : True if you wish to use the preset values

Name : label of the gameobject
Prefab : The gameobject that the Limb/Torso will be based around
Generate : True if you want to spawn the destructible pattern over a solid prefab, False if you have a prefab that you wish to use instead of spawning a destructible shape
PieceType : The type of generation, 0 = Limb, 1 = Lower Body, 2 = Upper Body
FillCore : If after debth there's a gap in the center toggling this will fill it with a single block
Destructible : (appears when FillCore is true) if you want the fill core to be destructible as well
FillHealth : (appears when FillCore is true) FillCore's assigned health
DestroyTime : (appears when FillCore is true) After destroy is triggered how long until the Fill is despawned
DestroyOnSleep : (appears when FillCore is true) Overrites DestroyTime, instead of destroying after a time period it will destroy when the RidgidBody's Sleep is triggered
CustomFill : (appears when FillCore is true) Prefab used to create the FillCore instead of the default which is BuildPiece (Can be left blank)
BuildPiece : The block that will be limb/torso structure
BuildType : Shape of generation, currently only 0 as a square, later circle will be added
PrefabSize : Size of the BuildPiece, eg currently it's set to Unitys default cube and its size is 1 so adjust accordingly for comparison
Height : How tall is the generation
Debth : How many layers do you wish to generate inwards
BuildType : 0 = Square, 1 = Circle (editor will replace either with the other)
SquareX: X = Length, Y = Width of the square
CircleX : number of blocks in the circumference in the circle, circumference size increases with number of blocks
OffSet : (Circle) adjust the radius of the circumference

TotalGenBlocks : editor only and shows how many blocks were generated over the structure (does not include: fillcore, prefabs such as joints, head, ext)

*PLEASE DONT CHANGE THE SIZE ARRAYS*
If needed the size of the arrays should be:
Limbs : 8 , Torso : 2 , Joints : 8 , Hands : 2 , Feet : 2

-- How to Animate --

The character is set up just like any other model in unity, the main downside however is that the avatar can't be configured after creation, the reason why is currently
unknown but it's most likely due to the avater being created in unity instead of being imported. 
A basic animation controller and an Idle animation are already included to work as examples for animation. The character will animate with any Mecanism animation and also
animations can be created within the Animation window of unity. The Idle Animation file has included within it all the values that can be edited for animating, values that
will have no effect on animation have been left out.
If you wish to try other better made animations, please check out the RPG Character Animation Pack, I used the free version to test out the characters animation, they 
can’t be included with this package for obvious reasons.

-- Block Stats Guide--

Block Stats is a basic script iv made to attach to each of the prefabs to make them destructible, its basic but it works so feel free to expand it to how you wish.
GenerateDestructableScript iv made it so that is has stored references to all the blocks it has generated so things such as splash damage can be implemented if you wish.
Currently included within it are:
Destructible : Toggle if the piece will trigger destroy when Health = 0
Health : how much life the prefab has
DestroyTime : Delay between destroy being called and the prefab is actually destroyed
DestroyOnSleep : Cancels DestroyTime, when the prefabs RigidBody declares itself asleep (stopped moving) then destroy then prefab
Included as well is a change all script in the other folder called 'BlockStatsChangeAll' which allows you to change all the 'BlockStats' scripts to a different set of values.
To use it, attach it to the parent object of all the 'BlockStats' you wish to change (one of the block holders for example) and press the button 'GetScripts', this will create
a debug message showing how many scripts it has found. From their input the values you want to set them to and press the button 'Change All' to set them.

-- Shooting Scene --

W A S D to move, you will move in a 3d manner, forward will move you where the camera is facing.
left click to shoot, 1 click = 1 shot
The scripts used to make this scene were quickly put together myself but feel free to use them if you find them of any use to you, they are found under Scripts/Other

-- Custom Characters --

Inside the folder "CustomMade" is a lower detail character iv created using the generated body as a base and replaced with my own design, although simple it shows that sticking with
whats generated as its appearence is not required. After the size and shape of the avatar is determined all blocks for appearence can be removed and replaced.