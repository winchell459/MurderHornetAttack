using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="LevelCompleteParameters", menuName = "LevelCompleteParameters")]
public class LevelCompleteParameters : ScriptableObject
{
    public LevelTask[] levelTasks;
}

[System.Serializable]
public class LevelTask
{
    [SerializeField] private string taskName = "Default Task Name";
    public TaskRequirements Requirements;
    public string CompletionPrompt;
    public string FailedPrompt;
    public bool Completed;


}

[System.Serializable]
public class TaskRequirements
{
    public bool PrincessFind;
    public bool PetalsCollect;
    public int EggCount;
    public bool AntMoundsTriggered;
    public bool QueenDefeat;
    public bool NestClear;
    public bool PitClear;
}