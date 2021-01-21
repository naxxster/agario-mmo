using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIModule
{
    // 설정 파일에서 읽어오는 구조로 변경할 
    private static string API_URL = "https://0sos1lsc4c.execute-api.ap-northeast-2.amazonaws.com/prod/";
    public static string GAMELIFT_MATCHREQUEST { get { return API_URL + "matchrequest"; } }
    public static string GAMELIFT_MATCHSTATUS {  get { return API_URL + "matchstatus";  } }


    //APIModule -> APIModel 로 이동할 MatchmakingRequest
   
    public class MatchmakingRequest
    {
        // Class for MatchRequest API parameters
        public string playerId;
        public string worldId;

        public MatchmakingRequest(string playerId, string worldId)
        {
            this.playerId = playerId;
            this.worldId = worldId;
        }
    }

    public struct MatchmakingResponse
    {
        public string ticketId;

        public MatchmakingResponse(string ticketId)
        {
            this.ticketId = ticketId;
        }
    }

    public struct MatchstatusRequest
    {
        public string ticketId;

        public MatchstatusRequest(string ticketId)
        {
            this.ticketId = ticketId;
        }
    }

    public struct MatchstatusResponse
    {
        public string address;
        public int port;
        public string playerSessionId;

        public MatchstatusResponse(string address, int port, string playerSessionId)
        {
            this.address = address;
            this.port = port;
            this.playerSessionId = playerSessionId;
        }
    }

}
