# ArrowsDemo
This project was created as a way to get familiar with high level programming and game development. So main goals were not to create a fun game but to lay out some programming and game dev problems to solve, solve them and learn while going through some kind of project. The concept and implementation of the game is really boring, the reason for such choice is it didn't start as a project for purely learning, but due to circumstances became one.

<p align="center">
  <img src="https://github.com/KEALHOVIK/ArrowsDemo/blob/master/Previews/Gameplay.gif" width=28% />
</p>

# Main Features:
- Completely math driven progression;
- Game economy with upgrades and skins;
- Changes to settings are applied on the fly;
- Player simulation and data analysis for game design tools;
- Tools for balancing game design (relations between rewards\difficulty and upgrade prices) with GUI controls and responsive result graphs;
    >It allows to quickly change the progression dynamics of the game and view what effects those changes would introduce (how long an average playthrough would take, would player experience a lot of runs without being able to buy upgrades etc.).</sub>
- Neat input system implementation (only used touch lately and with controller connected unity will give errors, but everything is laid out for controller use… just not fully implemented);    
    >By inheriting interfaces created by unity input system like "Controls.ITouchMovementActions" or "Controls.IMovementActions", creating "Controls()" and hooking a callback to implementation of such interface you cold add input to any object in a really clean way.
- Basic "mod" support (user could easily add custom skins to the shop).    
    >Models with .glb extensions can be put in "/Runtime Injest/Crossbows" or "/Runtime Injest/Projectiles" folder. On boot (during runtime) models would be imported, added to skin databases, shop icons would be generated. Note: model itself should be inside the folder with desired skin name, ingest data could be added to change default price.

# GUI for adjusting game balance:
<p align="center">
  <img src="https://github.com/KEALHOVIK/ArrowsDemo/blob/master/Previews/Balance-Controls.png" width=60% /> <img src="https://github.com/KEALHOVIK/ArrowsDemo/blob/master/Previews/Average-Player-Results.png" width=34.3% />
</p>

# Settings menu and shop demonstration:
<p align="center">
  <img src="https://github.com/KEALHOVIK/ArrowsDemo/blob/master/Previews/Settings-Menu.gif" width=30% /> &emsp; &emsp; &emsp; <img src="https://github.com/KEALHOVIK/ArrowsDemo/blob/master/Previews/Upgrades-And-Shop.gif" width=30% />
</p>

# Additional features and work:
- Music, audio and vibration;
- Dependency injection is used throughout;
- Procedural settings menu. Using implemented class data registry any new configurable option would be added to settings menu with correct changer (slider, toggle etc.);
- Settings and user data serialization;
- Game would use it's root directory to store saves and setting by default. But if it isn't allowed on platform or folder is write protected it gonna use appropriate OS specific folder;
- Registry for class data implemented allowing objects to register and receive field updates of that class. This is used for on the fly settings changes and user data storage and modification (save game\upgrades\skins). It has potential problems and isn't that neat, but for what it used for in project it allows for somewhat loosely coupled implementations of data consumers and updaters (menus, game events\states) and localizes option\data update code to it's consumers;
- Smooth point transfer at any reward\target damage. Half life calculations used to achieve accuracy, same execution time and constant perceived value change. Works great for any range of number be it animated transfer of 20 points or 20e+30 points;
- Extensive optimization of level generation and run simulation (dirty math gives speedup of 8000000 times):    
    >It was needed as without it balancing tool would take literally months to simulate for data. To get good data a lot of simulations with randomized virtual players needed, 350 playthroughs (at around 100 simulations) was used as a target to get to reasonable time. In final version simulation of about 800-1500 playthroughs turned out to give satisfactory results. With initial generation+simulation times of around 1.5-2 seconds that would be impossible;
    
    >Level generation is computationally expensive because a sequence of random math operation (including multiplication and division) gives huge discrepancies between generations (for average result of 10000 you could get a lot of generations of around 2000 and at the same time similar count of 5000000). With first implementations even median result of 10s of thousands generations would have error of +- one digit between them;
    
    >Speedup of a few times was achieved from getting rid of a couple of really inefficient call to standard functions and libraries. For example getting rid of "Enum.IsDefined" made generation two times faster;
    
    >Main speedup of approximately x350000 done by optimizing generation code. It was achieved by: implementing caches in factory classes (while still maintaining randomness), precomputing which operation in pair is preferable for a player (only until certain score precomputation needed, after which it can be decided by type of operations in pair), optimal usage of structs and classes, finally optimizing algorithms (minor speedup of around x2 or so);
    
    >Using logarithms to simplify math anywhere it's possible. This also eliminated the need for unlimited decimal numbers library making code run noticeably faster;
    
    >Multithreading optimization. Initially utilizing 8 core CPU only gave around x2 speedup, but x4.5 was achieved with optimizations. There was a lot of changes to "make profiler results make sense", meaning making seemingly less resource intensive function appear below more intensive functions. It was strange as in single thread those changes made that function actually run faster, but other functions took more time resulting in no difference. But at the same time those changes brought multithreading benefits up.
	

# Notes:
- Nuget packages:
  >Stored in "Assets/Plugins" folder;
  
  >Run "nuget restore -NoCache" to install packages;
  
  >Meta files in git are setup to use correct versions, when cloning repository unity would reset those settings so discard changes to "Assets/Plugins" folder to keep them.
- Blender models:
  >Blender models are used, so blender (2.93 for 2021.3.17f1) need to be installed (run "reimport all" if installed after project been opened) to run\build project successfully.
