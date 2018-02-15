
## uGameCoreLib

Library for [uGameCore](https://github.com/in0finite/uGameCore).

### IDE setup

Open monodevelop solution.

Add references to unity's libraries:
- UnityEngine.dll
- UnityEngine.UI.dll
- UnityEngine.Networking.dll

Build it.

Because library uses Unity's UNET networking, it has to be post-processed by UNET weaver in order for the networking to work.

I created a program which can do this. Download it from it's repository [here](https://github.com/in0finite/UnityLibraryPostProcessor/raw/master/bin/Debug/UnityLibraryPostProcessor.exe). Place it in ${TargetDir} of the project.

Add "After Build" step to monodevelop project. Set working directory to ${TargetDir}. Enter the following command:

	mono UnityLibraryPostProcessor.exe "C:\Program Files\Unity\" uGameCore.dll

Make sure to set correct path to unity installation.

After the "After Build" step executes, library will be ready to use. Now you just need to copy this library to your unity project, at the same location as the original library (Assets/uGameCore/Plugins/uGameCore.dll) - this can also be done as after-build step.

And you are good to go.

### Notes

- Project uses the following preprocessor definition: SERVER. If you need any of your own, feel free to add them.

- You can also use other IDEs, like Visual Studio.

### Any questions ?
- post an issue or email me

