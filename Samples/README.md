这是针对 NovaFramework 提供的演示案例；  

使用方法：  
1. 在应用加载模块增加==Game.Sample==程序集加载流程；
2. 在==Main==目录下的==GameSample==文件中修改当前运行的案例类型；
3. 需要启动==GameImport==导入模块，并在==GameConfig==文件中开启演示案例模块的跳转标识；
4. 运行程序，将自动转入对应案例类型的==SampleGate==并进入该案例的演示流程；

目前已有的演示案例包括：  
- Symbol Parser
- Inversion Of Control
- Object Lifecycle
- Dispatch Call
- Dependency Inject

