using System.Collections.Generic;
using UnityEngine;
using Aws.GameLift.Server;

public class GameLiftServer
{

    public bool GameLiftStart(int listenPort)
    {
        Debug.Log("GameLift Start with Port:" + listenPort);
        var initSDKOutcome = GameLiftServerAPI.InitSDK();
        if (initSDKOutcome.Success)
        {
            ProcessParameters processParameters = new ProcessParameters(
                (gameSession) =>
                {
                    //OnStartGameSession Callback
                    GameLiftServerAPI.ActivateGameSession();
                },
                () =>
                {
                    //OnProcessTerminate Callback
                    GameLiftServerAPI.ProcessEnding();
                },
                () =>
                {
                    //OnHealthCheck Callback
                    return true;
                },
                listenPort,
                new LogParameters(new List<string>()
                {
                    "./local/game/logs/myserver.log"
                }
                ));
            var processReadyOutcome = GameLiftServerAPI.ProcessReady(processParameters);
            if (processReadyOutcome.Success)
            {
                Debug.Log("ProcessReady Success");
                return true;
            }
            else
            {
                Debug.Log("ProcessReady Failure : " + processReadyOutcome.Error.ToString());
                return false;
            }
        }
        else
        {
            Debug.Log("InitSDK failure : " + initSDKOutcome.Error.ToString());
            return false;
        }
    }

    public bool AcceptPlayer(string playerSessionId)
    {
        var acceptPlayerSessionOutcome = GameLiftServerAPI.AcceptPlayerSession(playerSessionId);
        if (acceptPlayerSessionOutcome.Success)
        {
            Debug.Log("Player Session Validated.");
            return true;
        }
        else
        {
            Debug.Log("Player Session Rejected. AcceptPlayerSession Result : " + acceptPlayerSessionOutcome.Error.ToString());
            return false;
        }
    }

    public void RemovePlayer(string playerSessionId)
    {
        var removePlayerSessionOutcome = GameLiftServerAPI.RemovePlayerSession(playerSessionId);
        if (removePlayerSessionOutcome.Success)
        {
            Debug.Log("Remove Player Session Success : " + playerSessionId);
        }
        else
        {
            Debug.Log("Remove Player Session Failed. Result : " + removePlayerSessionOutcome.Error.ToString());
        }
    }

    public void EndProcess()
    {
        var processEndingOutcome = GameLiftServerAPI.ProcessEnding();
        if (processEndingOutcome.Success)
        {
            Debug.Log("End GameLift Server Process");
        }
        else
        {
            Debug.Log("Process Ending Failed. Result : " + processEndingOutcome.Error.ToString());
        }
    }
}
