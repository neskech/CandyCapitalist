using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class DiscordWebHook
{   
    const string WEB_HOOK_URL = "https://discord.com/api/webhooks/1071936469985402960/yfmNTypC5HIWT5_eurBQaZxkEfh7xISeEV-aGn7vU62CpxkElecHuZBTAcQ5W8AT16Wn";
    static Dictionary<string, string> machineNameToDiscord;
  
    static void QuitMsg()
    {
        string machineName = System.Environment.MachineName;
        machineName = machineNameToDiscord.ContainsKey(machineName) ? 
                      machineNameToDiscord[machineName] : machineName;
        SendHook($"{machineName}", $":red_circle: {machineName} Logged off Unity");
        EditorPrefs.SetBool("FirstInitDone", false);
    }

    static void OpenMsg()
    {
        string machineName = System.Environment.MachineName;
        machineName = machineNameToDiscord.ContainsKey(machineName) ? 
                      machineNameToDiscord[machineName] : machineName;
        SendHook($"{machineName}", $":green_circle: {machineName} Logged on Unity");
    }

    static void InitDictionary()
    {
        machineNameToDiscord = new Dictionary<string, string>(){
            {
                "ness", //1
                "Craig"
            },
            {
                "DESKTOP-GHJOATP", //2
                "Daniel"
            },
        };
    }

    static DiscordWebHook()
    {
        EditorApplication.quitting += QuitMsg;
        
        if (!EditorPrefs.GetBool("FirstInitDone", false))
        {
            Debug.Log("Sending webhook...");
            InitDictionary();
            OpenMsg();
         
            EditorPrefs.SetBool("FirstInitDone", true);
        }
           
        
    }

    static void SendHook(string username, string msg)
    {
        Post(WEB_HOOK_URL, new System.Collections.Specialized.NameValueCollection(){
            {
                "content",
                 msg
            }
        });
    }

    static byte[] Post(string url, NameValueCollection pairs)
    {
        using (WebClient client = new WebClient())
        {
            return client.UploadValues(url, pairs);
        }
    }
}
