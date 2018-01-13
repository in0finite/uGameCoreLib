
## uGameCoreLib

Library for [uGameCore](https://github.com/in0finite/uGameCore).

### Compiling

Open monodevelop project.

Add references to unity's libraries:
- UnityEngine.dll
- UnityEngine.UI.dll
- UnityEngine.Networking.dll

Build it.

Because library uses Unity's UNET networking, it has to be post-processed by UNET weaver in order for the networking to work.

We do this by invoking a method from weaver's library, as it can be seen [here](https://gist.github.com/in0finite/666ec537c8d5e4268605f140d55abb4f).

After the library is weaved, copy it to your unity project, at the same location as the original library (Assets/uGameCore/Plugins/uGameCore.dll).

Note that UnityEngine.Networking.dll must be in the same folder as the library, or otherwise NetworkBehaviour scripts from the library will not be recognized (that is unity's limitation, not ours).

And you are good to go.
