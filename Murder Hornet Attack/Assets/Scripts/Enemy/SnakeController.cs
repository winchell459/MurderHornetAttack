using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public SnakeController Head, Tail;
    public Vector2 Target;
    public float TargetThreshold = 0.01f;
    public float Velocity = 5;
    public Vector2 Direction = new Vector2(-1,1);
    private Vector2[] Directions = { new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, 1), new Vector2(1,-1), new Vector2(0,1), new Vector2(-1,1)};

    // Start is called before the first frame update
    void Start()
    {
        Target = Utility.HoneycombGridToWorldPostion((Utility.WorldToHoneycomb(transform.position)));
        Debug.Log(Target);
    }

    // Update is called once per frame
    void Update()
    {

        if (Vector2.Distance(transform.position, Target) > Velocity * Time.deltaTime)
        {
            transform.position = Vector2.MoveTowards(transform.position, Target, Velocity * Time.deltaTime);
        }
        else
        {
            transform.position = Target;
            getRandomLoc();
            transform.right = -(Target - (Vector2)transform.position);
            
        }
        
    }

    private void getRandomLoc()
    {
        int randDist = Random.Range(3, 10);
        int ranDir = Random.Range(0, 2);
        if (ranDir == 0) Direction = GetNewDirection(Direction, 1);
        else Direction = GetNewDirection(Direction, -1);
        Target = Utility.HoneycombGridToWorldPostion( GetHoneycombDirection(Utility.WorldToHoneycomb(transform.position), Direction, randDist));
    }

    private Vector2 GetNewDirection(Vector2 current, int turns) //turns + for counter clockwise
    {
        Vector2 newDir = new Vector2(0, 0);
        int index = 0;
        for(index = 0; index < Directions.Length; index += 1)
        {
            if(current.x == Directions[index].x && current.y == Directions[index].y)
            {
                break;
            }
        }
        if (turns > 0) index = (index + 1) % Directions.Length;
        else
        {
            index -= 1;
            if (index < 0) index = Directions.Length - 1;
        }
        newDir = Directions[index];

        turns = turns - (int)Mathf.Sign(turns) *1;
        if (turns != 0) return GetNewDirection(newDir, turns);
        else return newDir;
    }

    public Vector2 GetHoneycombDirection(Vector2 start, Vector2 dir, int honeyDistance)
    {
        //start = Utility.WorldPointToHoneycombGrid(start);
        Vector2 end = start;
        end.x += dir.x * honeyDistance;
        if (dir.x == 0) end.y += dir.y * honeyDistance;
        else if(start.x % 2 == 0 && dir.y > 0 || start.x % 2 != 0 && dir.y < 0)
        {
            end.y += Mathf.Ceil(honeyDistance / 2);
        }
        else
        {
            end.y += Mathf.Ceil((honeyDistance - 1) / 2);
        }

        return end;
    }
}
