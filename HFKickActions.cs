using UnityEngine;
using HoldfastSharedMethods;
using System.Collections.Generic;
using System;
using UnityEngine.UI;


public class HFKickActions : IHoldfastSharedMethods
{
    //Player Info
    public static Dictionary<int, ulong> PlayerDicSteamID = new Dictionary<int, ulong>();
    public static Dictionary<int, string> PlayerDicName = new Dictionary<int, string>();

    //Info for kick
    bool blnKickInProgress = false;
    int intKickerID;
    int intKickeeID;
    double dblPlayersInServer;
    double dblVotesNeededToKick;
    int intVotesForKick;
    int intVotesAgainstKick;
    float fltTimeVoteNow;
    float fltTimeVoteEnd;
    public static Dictionary<ulong, bool> PlayerVotedSteam = new Dictionary<ulong, bool>();


    public static InputField f1MenuInputField;




    public void OnRCLogin(int playerId, string inputPassword, bool isLoggedIn)
    {

    }

    public void OnIsServer(bool server)
    {
        //Get all the canvas items in the game
        var canvases = Resources.FindObjectsOfTypeAll<Canvas>();
        for (int i = 0; i < canvases.Length; i++)
        {
            Debug.LogWarning("Canvas Test");
            //Find the one that's called "Game Console Panel"
            if (string.Compare(canvases[i].name, "Game Console Panel", true) == 0)
            {
                Debug.LogWarning("CanvasFound");
                //Inside this, now we need to find the input field where the player types messages.
                f1MenuInputField = canvases[i].GetComponentInChildren<InputField>(true);
                Debug.LogWarning(f1MenuInputField);
                if (f1MenuInputField != null)
                {
                    Debug.Log("Found the Game Console Panel");
                }
                else
                {
                    Debug.Log("We did Not find Game Console Panel");
                }
                break;
            }
        }
        var rcCommand = "rc broadcast Vote Kick Initialised";
        Debug.LogWarning(rcCommand);
        f1MenuInputField.onEndEdit.Invoke(rcCommand);
    }


    public void OnSyncValueState(int value)
    {
        //Debug.LogFormat("OnSyncValueState {0}", value);
    }

    public void OnUpdateSyncedTime(double time)
    {
        //Debug.LogWarningFormat("OnUpdateSyncedTime {0}", time);
    }

    public void OnUpdateElapsedTime(float time)
    {
        //Debug.LogWarningFormat("OnUpdateElapsedTime {0}", time);
        fltTimeVoteNow = time;
        if (blnKickInProgress == true)
        {
            if (time >= fltTimeVoteEnd)
            {
                var rcCommand = "rc broadcast A vote to kick " + PlayerDicName[intKickeeID] + " has ran out of time";
                Debug.LogWarning(rcCommand);
                f1MenuInputField.onEndEdit.Invoke(rcCommand);

                blnKickInProgress = false;
                PlayerVotedSteam.Clear();
            }
        }
    }

    public void OnUpdateTimeRemaining(float time)
    {
        //Debug.LogWarningFormat("GetTimeRemaining {0}", time);
       
    }

    public void OnIsClient(bool client, ulong steamId)
    {
        //Debug.LogFormat("IsClient {0} {1}", client, steamId);

    }

    public void OnDamageableObjectDamaged(GameObject damageableObject, int damageableObjectId, int shipId, int oldHp, int newHp)
    {
        //Debug.LogFormat("OnDamageableObjectDamaged {0} {1} {2} {3} {4}", damageableObject.name, damageableObjectId, shipId, oldHp, newHp);
    }

    public void OnPlayerHurt(int playerId, byte oldHp, byte newHp, EntityHealthChangedReason reason)
    {
        //Debug.LogFormat("OnPlayerHurt {0} {1} {2} {3}", playerId, oldHp, newHp, reason);
    }

    public void OnPlayerKilledPlayer(int killerPlayerId, int victimPlayerId, EntityHealthChangedReason reason, string additionalDetails)
    {
        //Debug.LogFormat("OnPlayerKilledPlayer {0} {1} {2} {3}", killerPlayerId, victimPlayerId, reason, additionalDetails);
    }

    public void OnPlayerShoot(int playerId, bool dryShot)
    {
        //Debug.LogFormat("OnPlayerShoot {0} {1}", playerId, dryShot);
    }

    public void OnPlayerJoined(int playerId, ulong steamId, string playerName, string regimentTag, bool isBot)
    {
        
        //Adds all Join data to relevant lists
        PlayerDicSteamID.Add(playerId, steamId);
        //PlayerDicID.Add(playerId, playerId);
        PlayerDicName.Add(playerId, playerName);
    }

    public void OnPlayerLeft(int playerId)
    {
        //Debug.LogFormat("OnPlayerLeft {0}", playerId);
        PlayerDicSteamID.Remove(playerId);
        //PlayerDicID.Add(playerId, playerId);
        PlayerDicName.Remove(playerId);
    }

    public void OnPlayerSpawned(int playerId, int spawnSectionId, FactionCountry playerFaction, PlayerClass playerClass, int uniformId, GameObject playerObject)
    {
       
    }

    public void OnScorableAction(int playerId, int score, ScorableActionType reason)
    {
        //Debug.LogFormat("OnScorableAction {0} {1} {2}", playerId, score, reason.ToString());
    }

    public void OnTextMessage(int playerId, TextChatChannel channel, string text)
    {
        //Debug.LogFormat("OnTextMessage {0} {1} {2}", playerId, channel, text);

            //Checks Chat Message
            if (text.Contains("/votekick start"))
            {
                if (blnKickInProgress == false)
                {
                string strPlayerID = string.Empty;
                    
                for (int i = 0; i < text.Length; i++)
                {
                    if (Char.IsDigit(text[i]))
                        strPlayerID += text[i];
                }

                if (strPlayerID != string.Empty)
                    {
                        intKickerID = playerId;
                        intKickeeID = Int32.Parse(strPlayerID);
                        dblPlayersInServer = PlayerDicName.Count;
                        dblVotesNeededToKick = 0.6 * dblPlayersInServer;

                        var rcCommand = "rc broadcast A vote to kick " + PlayerDicName[intKickeeID] + " has begun, use '/votekick yes/no' to vote";
                        Debug.LogWarning(rcCommand);
                        f1MenuInputField.onEndEdit.Invoke(rcCommand);
                        blnKickInProgress = true;
                        fltTimeVoteEnd = fltTimeVoteNow + 60; 
                    }
                    else
                    {
                        var rcCommand = "rc serverAdmin privateMessage " + playerId + " " + "Please ensure you use the correct syntax, /votekick start PlayerID";
                        Debug.LogWarning(rcCommand);
                        f1MenuInputField.onEndEdit.Invoke(rcCommand);
                    }
                }
                else
                {
                    var rcCommand = "rc serverAdmin privateMessage " + playerId + " " + "A Vote Is Already In Progress!";
                    Debug.LogWarning(rcCommand);
                    f1MenuInputField.onEndEdit.Invoke(rcCommand);
                }
            }
            else if (text.Contains("/votekick yes"))
            {
                if (PlayerVotedSteam.ContainsKey(PlayerDicSteamID[playerId]))
                {
                    var rcCommand = "rc serverAdmin privateMessage " + playerId + " " + "You have already voted!";
                    Debug.LogWarning(rcCommand);
                    f1MenuInputField.onEndEdit.Invoke(rcCommand);

                }
                else
                {
                    PlayerVotedSteam.Add(PlayerDicSteamID[playerId], true);
                    intVotesForKick =+ 1;

                    if (intVotesForKick >= dblVotesNeededToKick)
                    {
                        var rcCommand = "rc broadcast A vote to kick " + PlayerDicName[intKickeeID] + " has succeeded";
                        Debug.LogWarning(rcCommand);
                        f1MenuInputField.onEndEdit.Invoke(rcCommand);

                        rcCommand = "rc serverAdmin kick " + intKickeeID; ;
                        Debug.LogWarning(rcCommand);
                        f1MenuInputField.onEndEdit.Invoke(rcCommand);

                        blnKickInProgress = false;
                        PlayerVotedSteam.Clear();

                    }
                }
                
                
            }
            else if (text.Contains("/votekick no"))
            {
                if (PlayerVotedSteam.ContainsKey(PlayerDicSteamID[playerId]))
                {
                    var rcCommand = "rc serverAdmin privateMessage " + playerId + " " + "You have already voted!";
                    Debug.LogWarning(rcCommand);
                    f1MenuInputField.onEndEdit.Invoke(rcCommand);

                }
                else
                {
                    PlayerVotedSteam.Add(PlayerDicSteamID[playerId], false);
                    intVotesAgainstKick = +1;

                    if (intVotesAgainstKick >= dblVotesNeededToKick)
                    {
                        var rcCommand = "rc broadcast A vote to kick " + PlayerDicName[intKickeeID] + " has failed";
                        Debug.LogWarning(rcCommand);
                        f1MenuInputField.onEndEdit.Invoke(rcCommand);


                        blnKickInProgress = false;
                        PlayerVotedSteam.Clear();

                    }
                }
            }
        }



      
    

    public void OnRoundDetails(int roundId, string serverName, string mapName, FactionCountry attackingFaction, FactionCountry defendingFaction, GameplayMode gameplayMode, GameType gameType)
    {
        //Debug.LogFormat("OnRoundDetails {0} {1} {2} {3} {4} {5} {6}", roundId, serverName, mapName, attackingFaction, defendingFaction,
            //gameplayMode, gameType);
    }

    public void OnPlayerBlock(int attackingPlayerId, int defendingPlayerId)
    {
        //Debug.LogFormat("OnPlayerBlock {0} {1}", attackingPlayerId, defendingPlayerId);
    }

    public void OnPlayerMeleeStartSecondaryAttack(int playerId)
    {
        //Debug.LogFormat("OnPlayerMeleeStartSecondaryAttack {0}", playerId);
    }

    public void OnPlayerWeaponSwitch(int playerId, string weapon)
    {
        //Debug.LogFormat("OnPlayerWeaponSwitch {0} {1}", playerId, weapon);
    }

    public void OnCapturePointCaptured(int capturePoint)
    {
        //Debug.LogFormat("OnCapturePointCaptured {0}", capturePoint);
    }

    public void OnCapturePointOwnerChanged(int capturePoint, FactionCountry factionCountry)
    {
        //Debug.LogFormat("OnCapturePointOwnerChanged {0} {1}", capturePoint, factionCountry.ToString());
    }

    public void OnCapturePointDataUpdated(int capturePoint, int defendingPlayerCount, int attackingPlayerCount)
    {
        //Debug.LogFormat("OnCapturePointDataUpdated {0} {1} {2}", capturePoint, defendingPlayerCount, attackingPlayerCount);
    }

    public void OnRoundEndFactionWinner(FactionCountry factionCountry, FactionRoundWinnerReason reason)
    {
        PlayerDicSteamID.Clear();
        PlayerDicName.Clear();
    }

    public void OnRoundEndPlayerWinner(int playerId)
    {
        //Debug.LogFormat("OnRoundEndPlayerWinner {0}", playerId);
    }

    public void OnPlayerStartCarry(int playerId, CarryableObjectType carryableObject)
    {
        //Debug.LogFormat("OnPlayerStartCarry {0} {1}", playerId, carryableObject);
    }

    public void OnPlayerEndCarry(int playerId)
    {
        //Debug.LogFormat("OnPlayerEndCarry {0}", playerId);
    }

    public void OnPlayerShout(int playerId, CharacterVoicePhrase voicePhrase)
    {
        //Debug.LogFormat("OnPlayerShout {0} {1}", playerId, voicePhrase);
    }

    public void OnInteractableObjectInteraction(int playerId, int interactableObjectId, GameObject interactableObject, InteractionActivationType interactionActivationType, int nextActivationStateTransitionIndex)
    {
        //Debug.LogFormat("OnInteractableObjectInteractedWith {0} {1} {2} {3} {4}", playerId, interactableObjectId, interactableObject.name, interactionActivationType, nextActivationStateTransitionIndex);
    }

    public void PassConfigVariables(string[] value)
    {
        //Debug.Log("PassConfigVariables : ");
        //for (int i = 0; i < value.Length; i++)
        //{
        //    Debug.Log(value[i]);
        //}
    }

    public void OnEmplacementPlaced(int itemId, GameObject objectBuilt, EmplacementType emplacementType)
    {
        //Debug.LogFormat("OnEmplacementPlaced {0} {1} {2}", itemId, objectBuilt.name, emplacementType);
    }

    public void OnEmplacementConstructed(int itemId)
    {
        //Debug.LogFormat("OnEmplacementConstructed {0}", itemId);
    }

    public void OnBuffStart(int playerId, BuffType buff)
    {
        //Debug.LogFormat("OnBuffStart {0} {1}", playerId, buff);
    }

    public void OnBuffStop(int playerId, BuffType buff)
    {
        //Debug.LogFormat("OnBuffStop {0} {1}", playerId, buff);
    }

    public void OnShotInfo(int playerId, int shotCount, Vector3[][] shotsPointsPositions, float[] trajectileDistances, float[] distanceFromFiringPositions, float[] horizontalDeviationAngles, float[] maxHorizontalDeviationAngles, float[] muzzleVelocities, float[] gravities, float[] damageHitBaseDamages, float[] damageRangeUnitValues, float[] damagePostTraitAndBuffValues, float[] totalDamages, Vector3[] hitPositions, Vector3[] hitDirections, int[] hitPlayerIds, int[] hitDamageableObjectIds, int[] hitShipIds, int[] hitVehicleIds)
    {
        //Debug.LogFormat("OnShotInfo Player {0}, Shot Count {1}", playerId, shotCount);
        //for (int i = 0; i < shotCount; i++)
        //{
        //    Debug.LogFormat("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15} ... ohh yeah and also positions...", trajectileDistances[i], distanceFromFiringPositions[i], horizontalDeviationAngles[i], maxHorizontalDeviationAngles[i], muzzleVelocities[i], gravities[i], damageHitBaseDamages[i], damageRangeUnitValues[i], damagePostTraitAndBuffValues[i], totalDamages[i], hitPositions[i], hitDirections[i], hitPlayerIds[i], hitDamageableObjectIds[i], hitShipIds[i], hitVehicleIds[i]);
        //}
    }

    public void OnVehicleSpawned(int vehicleId, FactionCountry vehicleFaction, PlayerClass vehicleClass, GameObject vehicleObject, int ownerPlayerId)
    {
        //Debug.LogFormat("OnVehicleSpawned {0} {1} {2} {3} {4}", vehicleId, vehicleFaction, vehicleClass, vehicleObject.name, ownerPlayerId);
    }

    public void OnVehicleHurt(int vehicleId, byte oldHp, byte newHp, EntityHealthChangedReason reason)
    {
        //Debug.LogFormat("OnVehicleHurt {0} {1} {2} {3}", vehicleId, oldHp, newHp, reason);
    }

    public void OnPlayerKilledVehicle(int killerPlayerId, int victimVehicleId, EntityHealthChangedReason reason, string details)
    {
        //Debug.LogFormat("OnPlayerKilledVehicle {0} {1} {2} {3}", killerPlayerId, victimVehicleId, reason, details);
    }

    public void OnShipSpawned(int shipId, GameObject shipObject, FactionCountry shipfaction, ShipType shipType, int shipNameId)
    {
        //Debug.LogFormat("OnShipSpawned {0} {1} {2} {3}, {4}", shipId, shipObject.name, shipfaction, shipType, shipNameId);
    }

    public void OnShipDamaged(int shipId, int oldHp, int newHp)
    {
        //Debug.LogFormat("OnShipDamaged {0} {1} {2}", shipId, oldHp, newHp);
    }

    public void OnAdminPlayerAction(int playerId, int adminId, ServerAdminAction action, string reason)
    {
        //Debug.LogFormat("OnShipDamaged {0} {1} {2} {3}", playerId, adminId, action.ToString(), reason);
    }

    public void OnConsoleCommand(string input, string output, bool success)
    {

    }

    public void OnRCCommand(int playerId, string input, string output, bool success)
    {


    }
}
