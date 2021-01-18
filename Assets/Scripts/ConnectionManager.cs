using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager
{
    public string Host = "127.0.0.1";
    public string Port = "7777";

    //public ConnectionManager()
    //{
    //    Dictionary<string, string> dict = GetCommandLineArguments();
    //    if (dict.ContainsKey("host"))
    //    {
    //        this.Host = dict["host"];
    //    }
    //    if (dict.ContainsKey("port"))
    //    {
    //        this.Port = dict["port"];
    //    }
    //    Debug.Log("Initial Host - " + this.Host + ", Port Info - " + this.Port);
    //}

    public void UpdateConnectionManager(string host, string port)
    {
        this.Host = host;
        this.Port = port;
    }

    private Dictionary<string, string> GetCommandLineArguments()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string key = null;
        for (int i=1; i<args.Length; i++)
        {
            if (key != null)
            {
                dictionary.Add(key, args[i]);
                key = null;
            }
            else 
            {
                if(args[i].StartsWith("-"))
                {
                    key = args[i].Substring(1);
                }
            }
        }
        return dictionary;
    }
}
