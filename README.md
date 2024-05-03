<section id="about">
<a href="#about" alt="About"><h1>About ECommons</h1></a>
  <p>ECommons is a multi-functional library designed to work within Dalamud Plugins. It features a variety of different systems and shortcuts which cuts out a lot of boiler plate code normally used to do standard plugin tasks.</p>
</section>

# WARNING
## Starting from 2.2.0.2, if you have previously used TaskManager it's namespace has changed, simply add `using ECommons.Automation.LegacyTaskManager;` and you're all good.

# WARNING
## Starting from v2.2.0.1, Windows Forms and Windows Targeting are now disabled by default. This affects only couple functions, please set a build configuration with forms manually if you need so.

<section id="getting-started">
<a href="#getting-started" alt="Getting Started"><h2>Getting Started</h2></a>
Add ECommons as a submodule to your project:

```
git submodule add https://github.com/NightmareXIV/ECommons.git
```
Add it to your plugin's CSProj file:

```  
<ItemGroup>
    <ProjectReference Include="..\ECommons\ECommons\ECommons.csproj" />
</ItemGroup>
```

Then, in the entry point of your plugin:

```
ECommonsMain.Init(pluginInterface, this);
```

where pluginInterface is a <b>DalamudPluginInterface</b>.

Don't forget to dispose it in your plugin's dispose method:
```
ECommonsMain.Dispose();
```

Using certain functions like clipboard or keypresses will require you to enable Windows Forms module. Add the following section into your plugin's `.csproj` file:
```
<PropertyGroup>
  <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>
```
Additionally, to be able to build on Linux (if you're using Github Actions or want to send your plugin to official Dalamud repo), you will have to enable windows targeting. Add the following section into your plugin's `.csproj` file:
```
<PropertyGroup>
  <EnableWindowsTargeting>true</EnableWindowsTargeting>
</PropertyGroup>
```
</section>

<section id="using-modules">
<a href="#using-modules" alt="Using Modules"><h2>Using Modules</h3></a>
ECommons comes with various modules which needs to be initalised at plugin runtime. To do so, modify your initalising code as follows:

```
ECommonsMain.Init(pluginInterface, this, Modules.<Module>);
```

where \<Module> is one of the following:
- All (For all modules)
- Localization
- SplatoonAPI
- DalamudReflector
- ObjectLife
- ObjectFunctions
</section>
