using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InsectGroup : Insect, IChunkObject
{
    MapChunk myChunk;


    protected abstract bool CheckDespawn();
    public abstract void Respawn();
    public abstract void Despawn();
    

    protected void HandleInsectGroupDeath()
    {
        //myChunk.RemoveTransientChunkObject(this);
        Map.StaticMap.HandleChunkObjectDeath(this);
    }

    GameObject IChunkObject.GameObject()
    {
        return gameObject;
    }

    
    void IChunkObject.SetMyChunk(MapChunk myChunk)
    {
        this.myChunk = myChunk;
    }

    void IChunkObject.Activate()
    {
        Respawn();
    }
    void IChunkObject.Deactivate()
    {
        if(CheckDespawn())
            Despawn();
    }
}
