using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEditor;
using Option;
using static Option.Option;

[InitializeOnLoad]
public class DiscordWebHook
{   
    [System.Serializable]
    struct WebHookData {
        public string url;
        public string name;
    }

    static Option<string> _webhookURL = None<string>();
    static Option<string> _username = None<string>();
    static void QuitMsg()
    {
        SendHook($":red_circle: {_username.UnwrapOr("Unkown user")} Logged off Unity");
        EditorPrefs.SetBool("FirstInitDone", false);
    }

    static void OpenMsg()
    {
        SendHook($":green_circle: {_username.UnwrapOr("Unkown user")} Logged onto Unity");
    }
 
    static void Init()
    {
       string root = Application.dataPath;
       string path = $"{root}/../Webhook.json";

       if (!File.Exists(path)) return;

       string fileData = File.ReadAllText(path);
       WebHookData data = JsonUtility.FromJson<WebHookData>(fileData);

       _webhookURL = Some<string>(data.url); 
       if (data.name.Length > 0)
            _username = Some<string>(data.name);
    }

    static DiscordWebHook()
    {
        Init();
        EditorApplication.quitting += QuitMsg;

        if (!EditorPrefs.GetBool("FirstInitDone", false))
        {
            Debug.Log("Sending webhook...");
            OpenMsg();
         
            EditorPrefs.SetBool("FirstInitDone", true);
        }
           
        
    }

    static void SendHook(string msg)
    {
        if (_webhookURL.IsNone()) return;

        Post(_webhookURL.Unwrap(), new System.Collections.Specialized.NameValueCollection(){
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
