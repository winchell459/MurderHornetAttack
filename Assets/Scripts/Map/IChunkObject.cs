using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChunkObject 
{
    void Activate();
    void Deactivate();

    //-------------- transient chunk object ---------------------- ITransientChunkObject ??
    GameObject GameObject();
    void SetMyChunk(MapChunk myChunk);
}
