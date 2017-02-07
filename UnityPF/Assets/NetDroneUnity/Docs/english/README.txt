#
# 1. Informations in the package components
#
Client
- Fonts (for examples of this client)
- Materials (Origin Studio Logo)
- Resources - binary ('bytes' format data file)
           - text ('csv' format data file)
- Scenes (for examples of this client)
- Scripts (Linked source code for 'Scenes')
          - DB ('csv', 'bytes', each file formats loader)
          - Net ('Packet' delegate processing for each protocol)
                - data (Data structures by NDP)
                - send ('Packet' transfer processing)
          - State ('State' processing for client)
          - UI (Message processing)
          - Unit (Player units processing and manager)
          - Util (NDCF processing)
          - World ('Channel/Room' processing)
- SDK
          - netdroneunity
                    - db ('csv', 'bytes' format data loader)
                    - net (NDP client based on 'Poll')
                    - util ('Singleton' design pattern and Server 'Time manager')
- Textures (Origin Studio Logo)

Docs (related documents)

Server (Examples of the NetDrone Engine for 'unityserver')
- hyperserver (PHP based Server)
- studyserver (C# based Server)
- unityserver (C++ based Server)

Tools
- DataConverter ('csv' to 'bytes')
- CryptTester ('text' or 'packet')
- DummyClient ('tcp' or 'rudp')
- ReuseMemory (allocation test)

#
# 2. GameFramework Guidelines
#
GameFramework is the subset of 'partial class' for applying 'global class/method' in C# as C++.

In addition, it includes 'Tick' class that keeps and processes server time, DB manager that applies 'csv' and 'bytes' format, 'Singleton' design pattern.

The GameFramework in 'UnityEngine namespace' does not need any extra declaration if it add 'using UnityEngine'. 

How to use 'GameFramework' is as follows

* How to replace formal class which uses 'MonoBehaviour'
Refer to 'client/Defines.cs', 'GameFramework' inherits 'MonoBehaviour'.
When you write script for Unity Editor, inherit 'GameFramework' instead 'MonoBehaviour'.

public class Example : GameFramework {
...
}

Now, all classes and methods in 'partial class GameFramework' can be approached comprehensively without namespace access.

* How to write general class in global access structures
The 'partial class GameFramework' includes all 'NetDroneUnity' sources and examples of client.
Please follow the examples below if you want to write new class and global methods.

Example of general global class:
namespace UnityEngine {
	public partial class GameFramework {
		public class CExample {
			public CExample() {}
			~CExample() {}

			...
		}
	}
}


Example of 'Singleton' global class:
namespace UnityEngine {
	public partial class GameFramework {
		public static CExample g_kExample { get { return CExample.GetInstance(); } }

		public class CExample : CSingleton<CExample> {
			public CExample() {}
			~CExample() {}

			public void Test() {}
			...
		}
	}
}

Now, it will be called without namespace access in anywhere that is included in 'partical class GameFramework' or inherited like 'public class Example : GameFramework' when you enter g_kExample.Test().

* The reason for defining explicit 'SAFE_DELETE'
Many developers consider the compulsory call of GC.Collect().
Because the method has compulsory collecting/removal function for all objects and it leads to performance degradation.

Although using 'Marshal.AllocHGlobal', 'Marshal.FreeHGlobal' in 'netdroneunity/net/Command.cs' methods is the one of solutions for the issue, you should apply this for only particular situation. Instead, please see the two guidelines below.

- Emphasis on reuse for minimising GC.Collect()method when you design class structures.
- You have to define Clear, Close functions call and null about deprecated objects. 

GameFramework uses 'SAFE_DELETE', 'SAFE_DELETE_ARRAY', and 'SAFE_DELETE_RELEASE' functions similar to the 'Defined Macro' of C++ for removal of object and defining null clearly.

Please refer to 'netdroneunity/Defines.cs' and 'Scripts/Defines.cs' for details.

#
# 3. Explanation of Hierarchy structures
#
The examples of the NetDrone Engine for Unity creates GameObject and indicates hierarchy structures and detailed information.

Indicated details are written below.

Networks (update on Packet and List information in the Text components)
         - Master
                  - Queue (Native:Count)
                  - Connector (Main:TCP)
                  - Connector (Sub:TCP)
         - Slave
                  - Queue (Native:Count)
                  - Connector (Main:UDP)
                  - Listener (Main:UDP)

Channels
         - User List
                     - user name (Key)
                     ...
         - Room List
                     - room name (Room Id)
                     ...

Main State (State Type)

Main Room (Room Id)
          - Other Player (Key)
          ...

Main Player (Key)

The way of multiple access tests is written below.

* Press 'Build settings' > 'PC, Mac & Linux Standalone' > 'Build', and then sign in different ID after running several test clients.
* Sign in after running ExampleMenu in the Unity Editor.
* If Room is created in the same channels, the 'Room Id' will be broadcasted to the in-channel users.
* Join all clients with input the 'Room Id'.

Now, Hierarchy structures as above are indicated on the Hierarchy tab.
Please test included functions in the several examples and see how they are indicated on the Hierarchy tab.

#
# 4. How to print log and massage
#
NetDrone engine for Unity indicates standard output by defining 'TRACE', 'OUTPUT', 'PRINT', 'ERROR' functions.

TRACE: 'DISABLE_UNITY' is defined, it is connected to 'Console.WriteLine(FILE, LINE, FUNCTION)' function. Otherwise, it is connectd to 'UnityEngine.Debug.Log' function.
OUTPUT: 'DISABLE_UNITY' is defined, it is connected to 'Console.WriteLine(FILE, LINE, FUNCTION)' function. Otherwise, it is connectd to 'UnityEngine.Debug.Log' function.
PRINT: 'DISABLE_UNITY' is defined, it is connected to 'Console.WriteLine' function. Otherwise, it is connectd to 'UnityEngine.Debug.Log' function.
ERROR: 'DISABLE_UNITY' is defined, it is connected to 'System.Diagnostics.Debug.WriteLine' function. Otherwise, it is connectd to 'UnityEngine.Debug.LogError' function.
 
If you need to use 'TRACE' command, create 'gmcs.rsp/smcs.rsp (-define:DEBUG)' file under Assets folder.
'TRACE' code will be removed if you clear 'DEBUG' declaration.

#
# 5. How to use an extension of the NetDrone Engine
#
NetDrone Engine for Unity applies to all projects based on C# as well as Unity Editor.
Add the declaration of 'DISABLE_UNITY' if you use it in other place, not Unity Editor.
 
Next, it is a place where you can add declaration in MonoDevelop.

Project Options: 'Build' > 'Compiler' > 'Define Symbols'

Please get more information at wiki.vogie.net and discuss at www.vogie.net