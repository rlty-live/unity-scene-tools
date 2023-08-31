using System;
using System.Collections.Generic;
using UnityEngine;

public static class AllPlayers
{
    public static IPlayer Me { get; internal set; }
    public static Dictionary<int, IPlayer> List;
    public static event Action<IPlayer> OnPlayerJoined, OnPlayerLeft;

    private static Dictionary<int, PlayerLocalData> _localDataCache = new Dictionary<int, PlayerLocalData>();

    private static Vector3 _lastKnownPosition;
    private static Quaternion _lastKnownRotation;
    private static bool _lastKnownPositionExists = false;
    public static void NotifyPlayerJoined(IPlayer player, bool me=false)
    {
        JLogBase.Log("Creating player: " + player.ClientId, typeof(AllPlayers));
        if (me)
        {
            Me = player;
            if (_lastKnownPositionExists)
            {
                Transform tr = (player as MonoBehaviour).transform;
                tr.position = _lastKnownPosition;
                tr.rotation= _lastKnownRotation;
                _lastKnownPositionExists = false;
            }
        }
            
        List[player.ClientId] = player;
#if !UNITY_SERVER
        if (_localDataCache.TryGetValue(player.ClientId, out PlayerLocalData data))
        {
            //restore player local data from cache
            player.IsLocalyMuted = data.isLocalyMuted;
            _localDataCache.Remove(player.ClientId);
        }
#endif
        OnPlayerJoined?.Invoke(player);
    }

    public static void NotifyPlayerLeft(IPlayer player)
    {
        JLogBase.Log("Destroying player: " + player.ClientId, typeof(AllPlayers));
        List.Remove(player.ClientId);
#if !UNITY_SERVER
        //store player local data in cache
        PlayerLocalData data = new PlayerLocalData();
        data.isLocalyMuted = player.IsLocalyMuted;
        _localDataCache[player.ClientId] = data;

        if (Me==player)
        {
            //store last known position
            Transform tr = (player as MonoBehaviour).transform;
            _lastKnownPosition = tr.position;
            _lastKnownRotation = tr.rotation;
            _lastKnownPositionExists = true;
        }
#endif
        if (Me == player)
            Me = null;
        OnPlayerLeft?.Invoke(player);
    }
}

/// <summary>
/// Use this class to store local data on players so they can be persistent even with the LOD system
/// </summary>
public class PlayerLocalData
{
    public bool isLocalyMuted;
}

public interface IPlayer
{
    Transform Transform { get; }

    int ClientId { get; }

    bool Sync_IsAdmin { get; set; }
    uint AgoraUserId { get; set; }
    
    bool Sync_IsVoiceBoosted {get;set;}
    
    bool Sync_IsServerMuted {get;set;}
    
    bool Sync_IsCrossServerMuted {get;set;}
    
    bool IsLocalyMuted { get; set; }

    string Username { get; set; }
    string SkinDesc { get; set; }

    /// <summary>
    /// -1=no mic
    /// 0= muted
    /// 1= silent
    /// 2 = talking
    /// </summary>
    event Action<int> OnTalkChanged;

    int IsTalking { get; set; }

    event Action<string> OnNameChanged;

    event Action<string> OnSkinRebuild;

    void Teleport(Vector3 position, Quaternion rotation);

    /// <summary>
    /// Modify speed of player (used by jumpers)
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    void SetAdditionalSpeed(Vector3 worldSpaceSpeedVector);

    /// <summary>
    /// Force vertical velocity (used by jumpers)
    /// </summary>
    /// <param name="verticalVelocity"></param>
    void SetVerticalVelocity(float verticalVelocity);
}