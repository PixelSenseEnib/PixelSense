using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace PA_WPF
{
    public class Behaviour
    {

          // State object
            private StateObject _state = new StateObject();
            private double _screenWidth, _screenHeight;
            private bool _init = true;
            private string _serverIP;

            public Behaviour(double screenWidth, double screenHeight, string serverIP)
            {
                _state.workSocket = new TcpClient();
                _screenWidth = screenWidth;
                _screenHeight = screenHeight;
                _serverIP = serverIP;
            }

            public void Update( Plane plane )
            {
                if (!_state.workSocket.Connected && _serverIP != "")
                {
                    try
                    {
                        _state.workSocket.Connect(_serverIP, 5447);
                        _state.workSocket.GetStream().BeginRead(_state.buffer, 0, StateObject.BufferSize, new AsyncCallback(ReadCallback), _state);
                        _init = true;
                        Console.WriteLine("Connection success");
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("Connection faillure");
                    }
                }
                  
                        if (_init)
                        {
                            Write(plane.Name + ":{X:" + (int)((plane.Position.X / _screenWidth) * 1000) + " " +
                                "Y:" + (int)(((plane.Position.Y - _screenHeight / 2) / _screenHeight) * 1000) + " " +
                                "a:" + (int)(plane.Rotation) + "}");
                            Console.WriteLine(plane.Name + ":{X:" + (int)((plane.Position.X / _screenWidth) * 1000) + " " +
                                "Y:" + (int)(((plane.Position.Y - _screenHeight / 2) / _screenHeight) * 1000) + " " +
                                "a:" + (int)(plane.Rotation) + "}");
                        }

            }

            public void Remove(Plane plane)
            {
                if (_init)
                {
                    Write(plane.Name + ":Remove");
                    Console.WriteLine(plane.Name + ":Remove");
                }
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
