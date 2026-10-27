#:property TargetFramework=Net11.0
#:property PublishAot=false
//#:property Features=runtime-async=on

using System;
using System.Diagnostics;

Console.WriteLine(new StackTrace(fNeedFileInfo: true));
