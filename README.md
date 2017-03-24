UWP Streaming Install Debugging App
===================================

This is the debugging app for UWP streaming install showcasing the debugging APIs. You can download and deploy the app to debug your own stream-able apps. Please refer to the [blog posts](https://blogs.msdn.microsoft.com/appinstaller/2017/03/15/uwp-streaming-app-installation/) for more information on streaming install.

Get Started
-----------

You can download this project and run it locally if you have the Windows 10 Creators Update and the Windows 10 Creators Update SDK. Before you can load the project in Visual Studio, please change the `TargetPlatformVersion` and `TargetPlatformMinVersion` in the solution's .csproj to match your installed SDK version.

The app's main usage of the streaming install debugging APIs is in the `MainPage.xaml.cs` file.