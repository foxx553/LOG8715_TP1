using UnityEngine;
public struct UnoptimisedStruct1                // 112 bytes
{
    public UnoptimizedStruct2 mainFriend;       // 48 bytes
    public float[] distancesFromObjectives;     // 8-byte
    public UnoptimizedStruct2[] otherFriends;   // 8-byte
    public double range;                        // 8 bytes 
    public Vector3 position;                    // 12 bytes
    public float size;                          // 4 bytes
    public float velocity;                      // 4 bytes
    public int baseHP;                          // 4 bytes
    public int currentHp;                       // 4 bytes
    public int nbAllies;                        // 4 bytes
    public byte colorAlpha;                     // 1 byte
    public bool canJump;                        // 1 byte
    public bool isVisible;                      // 1 byte
    public bool isStanding;                     // 1 byte

    public UnoptimisedStruct1(float velocity, bool canJump, int baseHP, int nbAllies, Vector3 position, int currentHp, float[] distancesFromObjectives, byte colorAlpha, double range, UnoptimizedStruct2 mainFriend, bool isVisible, UnoptimizedStruct2[] otherFriends, bool isStanding, float size)
    {
        this.mainFriend = mainFriend;
        this.distancesFromObjectives = distancesFromObjectives;
        this.otherFriends = otherFriends;
        this.range = range;
        this.position = position;
        this.size = size;
        this.velocity = velocity;
        this.baseHP = baseHP;
        this.currentHp = currentHp;
        this.nbAllies = nbAllies;
        this.colorAlpha = colorAlpha;
        this.canJump = canJump;
        this.isVisible = isVisible;
        this.isStanding = isStanding;
    }
}

public enum FriendState : byte
{
    isFolowing,
    isSearching,
    isPatrolling,
    isGuarding,
    isJumping,
}

public struct UnoptimizedStruct2        // 48 bytes
{
    public double maxSight;             // 8 bytes
    public Vector3 position;            // 12 bytes
    public float height;                // 4 bytes
    public float velocity;              // 4 bytes
    public float maxVelocity;           // 4 bytes
    public float acceleration;          // 4 bytes
    public int currentObjective;        // 4 bytes
    public FriendState currentState;    // 1 byte
    public bool isAlive;                // 1 byte
    public bool canJump;                // 1 byte

    public UnoptimizedStruct2(bool isAlive, float height, FriendState currentState, float velocity, int currentObjective, double maxSight, bool canJump, float acceleration, Vector3 position, float maxVelocity)
    {
        this.maxSight = maxSight;
        this.position = position;
        this.height = height;
        this.velocity = velocity;
        this.maxVelocity = maxVelocity;
        this.acceleration = acceleration;
        this.currentObjective = currentObjective;
        this.currentState = currentState;
        this.isAlive = isAlive;
        this.canJump = canJump;
    }
}
