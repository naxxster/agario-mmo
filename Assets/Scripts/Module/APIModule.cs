using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIModule
{
    //public static string API_URL = "https://0sos1lsc4c.execute-api.ap-northeast-2.amazonaws.com/prod/";

    public class MatchRequestParam
    {
        // Class for MatchRequest API parameters
        public string worldId;

        public MatchRequestParam(string worldId)
        {
            this.worldId = worldId;
        }
    }

}
