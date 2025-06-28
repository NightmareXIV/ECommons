<section id="about">
<a href="#about" alt="About"><h1>About ECommons</h1></a>
  <p>ECommons is a multi-functional library designed to work within Dalamud Plugins. It features a variety of different systems and shortcuts which cuts out a lot of boiler plate code normally used to do standard plugin tasks.</p>
</section>

<section id="getting-started">
<a href="#getting-started" alt="Getting Started"><h2>Getting Started</h2></a>
Get ECommons from NuGet using a console command:

```
dotnet add package ECommons
```
Or simply find it via NuGet package manager GUI.
  
Then, initialize in the constructor of your plugin:

```
ECommonsMain.Init(pluginInterface, this);
```

where pluginInterface is a <b>DalamudPluginInterface</b>.

Don't forget to dispose it in your plugin's dispose method:
```
ECommonsMain.Dispose();
```

<section id="getting-started">
<a href="#getting-started" alt="Getting Started"><h2>v3 changes</h2></a>
To ensure consistent building experience, ECommons 3.0.0.0 and higher no longer reference Windows Forms in any way. Additionally, `RELEASEFORMS` and `DEBUGFORMS` versions were removed. If you have previously used `System.Windows.Forms.Keys` with ECommons, replace it with `ECommons.WindowsFormsReflector.Keys`. Internal copy/paste methods now use reflection to call Windows Forms.

<section id="using-modules">
<a href="#using-modules" alt="Using Modules"><h2>Using Modules</h3></a>
ECommons comes with various modules which needs to be initalised at plugin runtime. To do so, modify your initalising code as follows:

```
ECommonsMain.Init(pluginInterface, this, Modules.<Module>);
```

where \<Module> is one of the following:
- All (For all modules)
- SplatoonAPI
- DalamudReflector
- ObjectLife
- ObjectFunctions
</section>

---

> [!WARNING]
> As of [`2024-04-15`](https://github.com/NightmareXIV/ECommons/commit/b4be673) `TaskManager`'s namespace has changed.\
> Add `using ECommons.Automation.LegacyTaskManager;` as an immediate fix.

> [!WARNING]
> As of [`2024-04-14`](https://github.com/NightmareXIV/ECommons/commit/6f1fd30) Windows Forms and Windows Targeting are now disabled by default.\
> Manually set a build configuration with forms as a fix.
