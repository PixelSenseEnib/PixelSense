using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// State object for reading client data asynchronously
public class StateObject {
	// Client  socket.
	public Socket workSocket = null;
	// Size of receive buffer.
	public const int BufferSize = 1024;
	// Receive buffer.
	public byte[] buffer = new byte[BufferSize];
	// Received data string.
	public StringBuilder sb = new StringBuilder();  
}

public class socketServer : MonoBehaviour {
	public static int port = 5447;
	public int maxConnections = 32;
	public static LinkedList<StateObject> connections = new LinkedList<StateObject>();
	public static LinkedList<string> messages = new LinkedList<string>();

	// Thread signal.
	public static ManualResetEvent allDone = new ManualResetEvent(false);
	
	public static void StartListening() {
		// Data buffer for incoming data.
		byte[] bytes = new Byte[1024];
		
		// Establish the local endpoint for the socket.
		// The DNS name of the computer
		// running the listener is "host.contoso.com".
		IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
		IPAddress ipAddress = ipHostInfo.AddressList[0];
		IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
		// Create a TCP/IP socket.
		Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

		// Bind the socket to the local endpoint and listen for incoming connections.
		try {
			listener.Bind(localEndPoint);
			listener.Listen(100);
			while (true) {
				// Set the event to nonsignaled state.
				allDone.Reset();
				
				// Start an asynchronous socket to listen for connections.
				Debug.Log("Waiting for a connection...");
				listener.BeginAccept( 
				                     new AsyncCallback(AcceptCallback),
				                     listener );
				
				// Wait until a connection is made before continuing.
				allDone.WaitOne();
			}
			
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}
	
	public static void AcceptCallback(IAsyncResult ar) {
		// Signal the main thread to continue.
		allDone.Set();
		
		// Get the socket that handles the client request.
		Socket listener = (Socket) ar.AsyncState;
		Socket handler = listener.EndAccept(ar);

		Debug.Log("Connection openned");
		// Say Hello
		Send(handler, "Hello!\r\n");

		// Create the state object.
		StateObject state = new StateObject();
		state.workSocket = handler;
		connections.AddLast(state);
		handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
	}
	
	public static void ReadCallback(IAsyncResult ar) {
		String content = String.Empty;
		
		// Retrieve the state object and the handler socket
		// from the asynchronous state object.
		StateObject state = (StateObject) ar.AsyncState;
		Socket handler = state.workSocket;
		
		// Read data from the client socket. 
		int bytesRead = handler.EndReceive(ar);
		
		if (bytesRead > 0) {
			// There  might be more data, so store the data received so far.
			state.sb.Append(Encoding.ASCII.GetString(
				state.buffer,0,bytesRead));
			
			// Check for end-of-file tag. If it is not there, read 
			// more data.
			content = state.sb.ToString();
			if (content.IndexOf("\n") > -1) {
				// All the data has been read from the 
				// client. Display it on the console.
				//Debug.Log("Read " + content.Length + " bytes from socket. \n Data : " + content );
				lock(messages)
				{
					string[] tmps = content.Split('\n');
					foreach( string tmp in tmps)
						messages.AddLast(tmp);
				}
				state.sb.Remove(0, state.sb.Length); // Clean buffer
				handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
			} else {
				// Not all data received. Get more.
				handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
			}
		}
	}
	
	private static void Send(Socket handler, String data) {
		// Convert the string data to byte data using ASCII encoding.
		byte[] byteData = Encoding.ASCII.GetBytes(data);
		
		// Begin sending the data to the remote device.
		handler.BeginSend(byteData, 0, byteData.Length, 0,
		                  new AsyncCallback(SendCallback), handler);
	}
	
	private static void SendCallback(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket handler = (Socket) ar.AsyncState;
			
			// Complete sending the data to the remote device.
			int bytesSent = handler.EndSend(ar);
			//Debug.Log("Sent {0} bytes to client.", bytesSent);
			
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}
	private static void Close(Socket handler) {
		handler.Shutdown(SocketShutdown.Both);
		handler.Close();
	}

	public GameObject rafale;
	private Hashtable myPlaneHashtable = new Hashtable();
	// Use this for initialization
	void Start () {
		Thread t = new Thread(new ThreadStart(StartListening));
		t.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		lock(messages)
		{
			while( messages.Count > 0 )
			{
				string mess = messages.First.Value;
				messages.RemoveFirst();

				if( mess.IndexOf(":") > 0 )
				{
					try
					{
						string plane = mess.Remove(mess.IndexOf(":"));
						Debug.Log(mess);
						string param = mess.Substring(mess.IndexOf(":") + 2);
						param = param.Remove(param.Length - 1 );

						string[] p = param.Split(' ');

						float x = float.Parse(p[0].Split(':')[1]);
						float y = float.Parse(p[1].Split(':')[1]);
						float a = float.Parse(p[2].Split(':')[1]);

						if( ! myPlaneHashtable.Contains(plane) )
						{
							GameObject myPlane = (GameObject)Instantiate(rafale, new Vector3(0, 0, 0), Quaternion.identity);
							myPlane.transform.localScale = new Vector3(100, 100, 100);
							myPlane.transform.localEulerAngles = new Vector3(90, 10, 0);
							myPlaneHashtable[plane] = myPlane;
						}
						GameObject tmp = (GameObject)(myPlaneHashtable[plane]);
						tmp.transform.position = new Vector3(-y*0.172f, 0, -x*0.299f + 16);
						tmp.transform.localEulerAngles = new Vector3(90, a, 0);
					}
					catch( IndexOutOfRangeException ex )
					{
						continue;
					}
					catch( FormatException ex )
					{
						continue;
					}
					catch( ArgumentOutOfRangeException ex )
					{
						continue;
					}
				}
			}
		}
		//GameObject player = GameObject.Find("rafale1");
		//player.transform.Translate(0, 1, 0);
	}

	void Stop () {
		foreach(StateObject st in connections)
		{
			Close(st.workSocket);
		}
	}

	void OnGUI () {
		/*if (Network.peerType == NetworkPeerType.Disconnected) {			
			GUILayout.Label("Game server Offline");
			if (GUILayout.Button("Start Game Server")) {               

			}
		} else {			
			if (Network.peerType == NetworkPeerType.Connecting) {
				GUILayout.Label("Server Starting");
			} else {
				GUILayout.Label("Game Server Online");         
				GUILayout.Label("Server Ip: " + Network.player.ipAddress + " Port: " + Network.player.port);
				GUILayout.Label("Clients: " + Network.connections.Length + "/" + maxConnections);
				
				// Get information of connected clients
				foreach(NetworkPlayer client in Network.connections) {
					GUILayout.Label("Client " + client);   
				}
			}
			if (GUILayout.Button ("Stop Server")){             

			}
		}*/
	}
}
