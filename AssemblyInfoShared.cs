using System.Reflection;

[assembly: AssemblyTitle("SoftwareBotany.Sunlight")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("Software Botany LLC.")]
[assembly: AssemblyProduct("SoftwareBotany.Sunlight")]
[assembly: AssemblyCopyright("Copyright © Software Botany LLC. 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if DEBUG
    #if UNSAFE
        #if POSITIONLIST
[assembly: AssemblyConfiguration("Debug-Unsafe-PositionList")]
        #else
[assembly: AssemblyConfiguration("Debug-Unsafe")]
        #endif
    #else
        #if POSITIONLIST
[assembly: AssemblyConfiguration("Debug-Safe-PositionList")]
        #else
[assembly: AssemblyConfiguration("Debug-Safe")]
        #endif
    #endif
#else
    #if UNSAFE
        #if POSITIONLIST
[assembly: AssemblyConfiguration("Release-Unsafe-PositionList")]
        #else
[assembly: AssemblyConfiguration("Release-Unsafe")]
        #endif
    #else
        #if POSITIONLIST
[assembly: AssemblyConfiguration("Release-Safe-PositionList")]
        #else
[assembly: AssemblyConfiguration("Release-Safe")]
        #endif
    #endif
#endif

[assembly: AssemblyVersion("0.9.0.*")]