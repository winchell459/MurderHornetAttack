using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InsectSolo : Insect, IChunkObject
{
    void IChunkObject.Activate()
    {
        gameObject.SetActive(true);
    }

    void IChunkObject.Deactivate()
    {
        gameObject.SetActive(false);
    }

    GameObject IChunkObject.GameObject()
    {
        return gameObject;
    }

    MapChunk myChunk;
    void IChunkObject.SetMyChunk(MapChunk myChunk)
    {
        this.myChunk = myChunk;
    }

    protected void HandleInsectSoloDeath()
    {
        //myChunk.RemoveTransientChunkObject(this);
        Map.StaticMap.HandleChunkObjectDeath(this);
    }
}
