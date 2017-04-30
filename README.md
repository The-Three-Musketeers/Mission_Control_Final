# Lewisburg Children’s Museum Rocket Launch Game

Team Name: The Three Musketeers

Team Members: Michael Hammer, Kenny Rader, and Keyi Zhang

Class Name: CSCI 475/476

Client: Professor Erin Jablonski

Instructor: Professor Brian King

----
## Abstract
As technology evolves rapidly around us, it is important to keep children informed about science and technology so that they can be prepared for the life they are growing into. This can be challenging due to the complex nature of technology and what young children can comprehend. The Lewisburg Children’s Museum is doing their part to resolve this dilemma by educating children about science and technology through their new space exhibit. Interactive learning is one of the best solutions to this task, combining the values of entertainment and education in one unique experience.

Our team has been dedicated to designing a game that allows children to launch a rocket, teaching them concepts such as gravity, trajectory, and fuel use in a fun environment. To implement the game we used a variety of tools such as the Unity Game Engine and a custom-built analog controller. The game supports an easy to use interface and realistic graphics, as well as a physical control panel with interactive components and lights. Every aspect of the game has been engineered with the purpose of keeping the children engaged so that the can better learn these fundamentals of rocket launches.

## Install
This project depends on several frameworks and platforms. Arduino IDE is optional if you just want to play the game without hardware.
+ Unity 5 (required). See [how to install Unit 5](https://docs.unity3d.com/Manual/InstallingUnity.html).
+ Git (required). See [how to install git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git).
+ Arduino IDE (optional). See [how to install Arduino IDE](https://www.arduino.cc/en/Guide/HomePage).

After all installations, you can clone the entire repository:
```
git clone --recursive git@github.com:The-Three-Musketeers/Mission_Control_Alpha.git
```
Using ```--recursive``` option will also clone the Arduino library source code as a submodule. If you just want to run the game on a computer, you can just clone the repository using the following command:
```
git clone git@github.com:The-Three-Musketeers/Mission_Control_Alpha.git
```

If you are using git with a GUI, such as Github Windows client, you can clone this repository directly there.

The cloning process will take a while. Once it is finished, you can open the ```TitleScreen.unity``` located in the ```Assets/Scenes``` folder. Unity will process the entire project, which may also take some time. 

### How to connect the hardware to the computer
If you want to use the hardware to play the game, you need to make sure the port is set in Unity. Unfortunately the ```.NET``` used in Unity does not support port query. As a result, you need to manually input the port number in the source code. Don't worry! It is easy to do!
1. Plug the hardware in and open the Arduino IDE.
2. In the Arduino IDE, go to ```Tools->Port```. Depends on your Arduino used in the hardware, it may appear in a different name, but that doesn't matter. Write down the port name. For windows, it should be something like ```COM3``` or ```COM4```.
3. Open ```Assets/Scripts/KeyListener.cs``` with any text editor you prefer, and then locate line 19, where it shows ```public static Serial serial = Serial.Connect("COM4");``` Change ```COM4``` to the port name you just write down.
4. Save the file and exit the text editor. 

Your game is ready to go!

## Build
We have provided built binary for this game in the [release page](https://github.com/The-Three-Musketeers/Mission_Control_Alpha/releases). If you want to build it on a different platform, please follow the Unity 5 build [tutorial](https://unity3d.com/learn/tutorials/projects/space-shooter-tutorial/building-game) or refer to this [documentation](https://docs.unity3d.com/Manual/BuildSettings.html).

## File Structure
Most of the file names are self-explanatory. The folder structure is managed by Unity 5, such as the ```Assets``` and ```ProjectSettings```. To make it easy to clone, we have also included a submodule link to the ```ArduinoNet``` library. 
