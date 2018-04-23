using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class UsageStatsAPI
{
    private static string URL = "https://lambda-starship-user-stats.appspot.com";

    private static string REPL_COMMAND_URL = URL + "/repl-command";
    private static string EDITOR_CONTENT_URL = URL + "/editor-content";
    private static string ERROR_URL = URL + "/error";

    public class REPLCommand
    {
        public string uid;
        public long timestamp;
        public string command;
    }

    public class EditorContent
    {
        public string uid;
        public long timestamp;
        public string content;
    }

    public class ErrorInstance
    {
        public string uid;
        public long timestamp;
        public string description;
    }

    private static long Timestamp()
    {
        var epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
        return (long) ((System.DateTime.UtcNow - epochStart).TotalSeconds * 1000);
    }

    public static IEnumerator SendREPLCommand(string command)
    {
        REPLCommand replCommand = new REPLCommand();
        replCommand.uid = UUIDState.Get();
        replCommand.timestamp = Timestamp();
        replCommand.command = command;

        string json = JsonUtility.ToJson(replCommand);

        var header = new Dictionary<string, string>();
        header.Add("Content-Type", "application/json");

        var data = System.Text.Encoding.UTF8.GetBytes(json);

        WWW www = new WWW(REPL_COMMAND_URL, data, header);
        yield return www;
    }

    public static IEnumerator SendEditorContent(string content)
    {
        EditorContent editorContent = new EditorContent();
        editorContent.uid = UUIDState.Get();
        editorContent.timestamp = Timestamp();
        editorContent.content = content;

        string json = JsonUtility.ToJson(editorContent);

        var header = new Dictionary<string, string>();
        header.Add("Content-Type", "application/json");

        var data = System.Text.Encoding.UTF8.GetBytes(json);

        WWW www = new WWW(EDITOR_CONTENT_URL, data, header);
        yield return www;
    }

    public static IEnumerator SendErrorInstance(string description)
    {
        ErrorInstance error = new ErrorInstance();
        error.uid = UUIDState.Get();
        error.timestamp = Timestamp();
        error.description = description;

        string json = JsonUtility.ToJson(error);

        var header = new Dictionary<string, string>();
        header.Add("Content-Type", "application/json");

        var data = System.Text.Encoding.UTF8.GetBytes(json);

        WWW www = new WWW(ERROR_URL, data, header);
        yield return www;
    }
}
