using System;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class WebSocket : WebSocketBehavior
{
    //protected override void OnOpen()
    //{
    //    //Debug.Log("OnOpen");
    //}

    //protected override void OnClose(CloseEventArgs e)
    //{
    //    //Debug.Log("OnClose");
    //}

    protected override void OnError(ErrorEventArgs e)
    {
        Server.Log("Blad WebSocket.", e.Message);
        base.OnError(e);
    }

    protected Headers GetHeaders()
    {
        return new Headers()
        {
            SocketId = ID,
            UserAgent = Headers["User-Agent"],
            Host = Headers["Host"]
        };
    }

    protected bool CheckPacket(ulong userId, bool game = true)
    {
        if (!Server.Pilots.ContainsKey(userId))
        {
            Server.Log("Naglowek sesji nie pasuje do gracza.", userId);
            return false;
        }

        if (game)
            return Server.Pilots[userId].Headers?.SocketId == ID;
        return Server.Pilots[userId].ChatHeaders?.SocketId == ID;
    }
}