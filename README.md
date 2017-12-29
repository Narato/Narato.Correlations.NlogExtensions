# Narato.Correlations.NLogExtensions
This library contains Nlog extensions for automatically enriching log messages with a correlation ID

Getting started
==========
### 1. Add dependency in your project's csproj file

```xml
<PackageReference Include="Narato.Correlations" Version="2.0.0" />
<PackageReference Include="Narato.Correlations.NlogExtensions" Version="2.0.1" />
```

### 2. Create a nlog.config file.
For basic information on what the nlog.config file should look like, [go here](https://github.com/NLog/NLog/wiki/Configuration-file).

Please note the `<add assembly="Narato.Correlations.NlogExtensions"/>` part. This will actually load the layout renderer.
Also note the ${correlation-id}. This will get filled in with the correlation ID.
```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      internalLogLevel="Warn"
      internalLogFile="c:\temp\internal-nlog.txt">

  <extensions>
    <add assembly="Narato.Correlations.NlogExtensions"/>
  </extensions>

  <!-- the targets to write to -->
  <targets>
     <!-- write logs to file -->
     <target xsi:type="File" name="allfile" fileName="c:\temp\nlog-all-${shortdate}.log"
                 layout="${longdate}|${event-properties:item=EventId.Id}|${logger}|${uppercase:${level}}|${message} ${exception}" />

     <target xsi:type="File" name="ownFile-web" fileName="c:\temp\nlog-own-${shortdate}.log"
             layout="${longdate}|${event-properties:item=EventId.Id}|${logger}|${uppercase:${level}}|  ${message} ${exception}|correlation ID: ${correlation-id}" />

     <!-- write to the void aka just remove -->
    <target xsi:type="Null" name="blackhole" />
  </targets>

  <!-- rules to map from logger name to target -->
  <rules>
    <!--All logs, including from Microsoft-->
    <logger name="*" minlevel="Trace" writeTo="allfile" />

    <!--Skip Microsoft logs and so log only own logs-->
    <logger name="Microsoft.*" minlevel="Trace" writeTo="blackhole" final="true" />
    <logger name="*" minlevel="Trace" writeTo="ownFile-web" />
  </rules>
</nlog>
```

### 3. Configure Startup.cs
This library doesn't need additional configuration, but make sure that the configuration for [Narato.Correlations](https://github.com/Narato/Narato.Correlations) is correct.

### 4. Update Program.cs to enable NLog
add ```using NLog.Web;```

```C#
public static void Main(string[] args)
{
    // NLog: setup the logger first to catch all errors
    var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
    try
    {
        logger.Debug("init main");
        BuildWebHost(args).Run();
    }
    catch (Exception e)
    {
        //NLog: catch setup errors
        logger.Error(e, "Stopped program because of exception");
        throw;
    }
}

public static IWebHost BuildWebHost(string[] args) =>
    WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .UseNLog() // NLog: setup NLog for Dependency injection
        .Build();

```

# Helping out

If you want to help out, please read [this wiki page](https://github.com/Narato/Narato.Correlations.NlogExtensions/wiki/Helping-out)