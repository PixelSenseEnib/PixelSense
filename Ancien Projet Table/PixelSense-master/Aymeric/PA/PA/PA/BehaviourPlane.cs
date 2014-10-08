using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.Net.Sockets;

using Microsoft.Surface.Core;
using Enib.SurfaceLib;

namespace enib
{
    namespace pa
    {
        public class BehaviourPlane: Behaviour
        {
            // State object
            private StateObject _state = new StateObject();
            private int _screenWidth, _screenHeight;
            private bool _init = true;
            private string _serverIP;

            public BehaviourPlane(int screenWidth, int screenHeight, string serverIP)
            {
                _state.workSocket = new TcpClient();
                _screenWidth = screenWidth;
                _screenHeight = screenHeight;
                _serverIP = serverIP;
            }

            public override void Update(LinkedList<Sprite> objects, LinkedList<Sprite> selection, LinkedList<MyTouchPoint> touchPoints)
            {
                if (!_state.workSocket.Connected && _serverIP != "")
                {
                    try{
                        _state.workSocket.Connect(_serverIP, 5447);
                        _state.workSocket.GetStream().BeginRead(_state.buffer, 0, StateObject.BufferSize, new AsyncCallback(ReadCallback), _state);
                        _init = true;
                        Console.WriteLine("Connection success");
                    }
                    catch(SocketException)
                    {
                        Console.WriteLine("Connection faillure");
                    }
                }

                if (touchPoints.Count == 1)
                {
                    foreach (Sprite obj in selection)
                    {
                        if (obj is Plane)
                        {
                            Plane tmp = (Plane)obj;
                            tmp.Target = new Vector2(touchPoints.First.Value.Touch.X, touchPoints.First.Value.Touch.Y);
                        }
                    }
                }
                foreach (Sprite obj in objects)
                {
                    if (obj is Plane)
                    {
                        Plane tmp = (Plane)obj;
                        if (tmp.Speed > 0.07 || _init)
                        {
                            Write(tmp.Name + ":{X:" + (int)((tmp.Position.X / _screenWidth) * 1000) + " " +
                                "Y:" + (int)(((tmp.Position.Y - _screenHeight / 2) / _screenHeight) * 1000) + " " +
                                "a:" + (int)(tmp.Rotation * 180 / Math.PI) + "}");
                        }
                    }
                }
                _init = false;
            }

            public void Write(string message)
            {
                if (!_state.workSocket.Connected)
                    return;
                NetworkStream serverStream = _state.workSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(message + "\n");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }

            public static void ReadCallback(IAsyncResult ar)
            {
                String content = String.Empty;

                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                TcpClient handler = state.workSocket;

                // Read data from the client socket. 
                int bytesRead = handler.GetStream().EndRead(ar);

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read 
                    // more data.
                    content = state.sb.ToString();
                    if (content.IndexOf("\n") > -1)
                    {
                        // All the data has been read from the 
                        // client. Display it on the console.
                        Console.WriteLine("Read " + content.Length + " bytes from socket. \n Data : " + content);
                        state.sb.Remove(0, state.sb.Length); // Clean buffer
                        state.workSocket.GetStream().BeginRead(state.buffer, 0, StateObject.BufferSize, new AsyncCallback(ReadCallback), state);
                    }
                    else
                    {
                        // Not all data received. Get more.
                        state.workSocket.GetStream().BeginRead(state.buffer, 0, StateObject.BufferSize, new AsyncCallback(ReadCallback), state);
                    }
                }
            }
            
            public class StateObject
            {
                // Client  socket.
                public TcpClient workSocket = null;
                // Size of receive buffer.
                public const int BufferSize = 1024;
                // Receive buffer.
                public byte[] buffer = new byte[BufferSize];
                // Received data string.
                public StringBuilder sb = new StringBuilder();
            }
        }
    }
}
