using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace PA_PixelSense
{
    public class Behaviour
    {

        // State object
        private StateObject _state = new StateObject();
        private double _screenWidth, _screenHeight;
        private string _serverIP;

        /// <summary>
        /// Behaviour constructor
        /// Initiate TcpCLient
        /// </summary>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        /// <param name="serverIP"></param>
        public Behaviour(double screenWidth, double screenHeight, string serverIP)
        {
            _state.workSocket = new TcpClient();
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _serverIP = serverIP;
        }

        /// <summary>
        /// Update position of the plane in the 3D environment
        /// </summary>
        /// <param name="plane"></param>
        public void Update(Plane plane)
        {
            //Try to connect to the server if not connected
            if (!_state.workSocket.Connected && _serverIP != "")
            {
                try
                {
                    _state.workSocket.Connect(_serverIP, 5447);
                    _state.workSocket.GetStream().BeginRead(_state.buffer, 0, StateObject.BufferSize, new AsyncCallback(ReadCallback), _state);
                    Console.WriteLine("Connection success");
                }
                catch (SocketException)
                {
                    Console.WriteLine("Connection faillure");
                }
            }

            //Send the position of the plane to the server
            Write(plane.Name + ":{X:" + (int)((plane.Position.X / _screenWidth) * 1000) + " " +
                "Y:" + (int)(((plane.Position.Y - _screenHeight / 2) / _screenHeight) * 1000) + " " +
                "a:" + (int)(plane.Rotation) + "}");

            Console.WriteLine(plane.Name + ":{X:" + (int)((plane.Position.X / _screenWidth) * 1000) + " " +
                "Y:" + (int)(((plane.Position.Y - _screenHeight / 2) / _screenHeight) * 1000) + " " +
                "a:" + (int)(plane.Rotation) + "}");
        }

        /// <summary>
        /// Remove the plane of the 3D environment
        /// </summary>
        /// <param name="plane"></param>
        public void Remove(Plane plane)
        {
            Write(plane.Name + ":Remove");
            Console.WriteLine(plane.Name + ":Remove");
        }

        /// <summary>
        /// Send a message to the server
        /// </summary>
        /// <param name="message"></param>
        public void Write(string message)
        {
            if (!_state.workSocket.Connected)
                return;
            NetworkStream serverStream = _state.workSocket.GetStream();
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(message + "\n");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        /// <summary>
        /// Read data from the server
        /// </summary>
        /// <param name="ar"></param>
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
