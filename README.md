# Packet Helper

Packet Helper is a helper mod designed to easily create client and server packets as well as attaching them to your mod's custom methods.

 An example of implementhing this library can be found at https://github.com/iiVeil/ShareEm

## Initializing

Packet Helper uses a class called `Session` to store data for your packets

    Session session = new Session("YOUR.MOD.GUID")
 Session uses your mod GUID to create a unique packet offset to ensure that you don't overwrite packets on the game or other mods.
 
 
## Creating a packet
Creating packets with Packet Helper is insanely easy first we will be creating a client packet.

It is important to remember that you must only add your packet once. Be sure to patch a method that gets called once. 



    public static Session session = new Session("YOUR.MOD.GUID");
    public void MyClientPacketMethod(Packet packet) 
    {
	    // do some stuff with the packet
	}
	
	// This is my go-to method to create client packets
	[HarmonyPatch(typeof(LocalClient), "InitializeClientData")]
    [HarmonyPostfix]
    static void CreateNewClientPackets()
    {
        session.CreateNewClientPacket("A Unique Packet Name", MyPacketMethod);
    }
	
The packet name will be used later to identify which packet to send. The unique packet name does not have to correspond to the method name in any way.

Next, we will create a server packet.

    public static Session session = new Session("YOUR.MOD.GUID");
    public void MyServerPacketMethod(int fromClient, Packet packet) 
    {
	    // do some stuff with the packet
	}
	
	// This is my go-to method to create server packets
	[HarmonyPatch(typeof(Server), "InitializeServerPackets")]
    [HarmonyPostfix]
    static void CreateNewServerPackets()
    {
        session.CreateNewServerPacket("A Unique Packet Name", MyPacketMethod);
    }
It is important to note that server packet methods differ from client packet methods. They include a `fromClient` parameter that indicates the `client.id` that the packet came from.

When creating a packet it's vital to understand that the method you attach to a packet will be handling how that packet is interacted with when received.

## Sending packets
Sending a packet is slightly more complicated as it uses the class `Data` shown below

    public class Data
    {
        public int[] ints = null;
        public string[] strings = null;
        public bool[] bools = null;
        public float[] floats = null;
        public short[] shorts = null;
        public long[] longs = null;
        public Vector3[] vector3s = null;
        public Quaternion[] quaternions = null;
    }
`Data` is an important class as it allows us to send any amount of data through the packet.
  
Here we will be sending a packet to the server.

    public static Session session = new Session("YOUR.MOD.GUID");
    public void MyServerPacketMethod(int fromClient, Packet packet) 
    {
	    itemId = packet.ReadInt();
	    Debug.Log($"{fromClient}, {itemId}");
	}
	
	[HarmonyPatch(typeof(Server), "InitializeServerPackets")]
    [HarmonyPostfix]
    static void CreateNewServerPackets()
    {
        session.CreateNewServerPacket("MyServerPacket", MyServerPacketMethod);
    }
    
    [HarmonyPatch(typeof (ClientSend), "PickupItem")]
    [HarmonyPrefix]
    static void ClientSendSharePacket(ClientSend __instance, int itemID) {
      Item item = ItemManager.Instance.list[itemID].GetComponent < Item > ();
      if (item.powerup) {
	    // Create new data object 
        Data data = new Data();
        
        // Add the powerup id to the data
        data.ints = new int[] { item.powerup.id };
        
        // Send packet to the server and call subsequent method MyServerPacketMethod()
        session.SendPacketToServer("MyServerPacket", data);
      }
    }  

  Next is sending a packet to the client.

Remember, when sending a packet to the client, `fromClient` is not accessible. If you need that value in the method that handles the packet, put it in the data class.

    public static Session session = new Session("YOUR.MOD.GUID");
    public void MyClientPacketMethod(Packet packet) 
    {
	    fromClient = packet.ReadInt();
	    itemId = packet.ReadInt();
	    Debug.Log($"{fromClient}, {itemId}");
	}
	
	public void MyServerPacketMethod(int fromClient, Packet packet) 
    {
	    itemId = packet.ReadInt();
	    Debug.Log($"{fromClient}, {itemId}");
	    Data data = new Data();
	    data.ints = new int[] { fromClient, itemId };
	    // Send packet to all connected clients, the host is Client(0)
	    session.SendPacketToAllClients("MyClientPacket", data);
	}
	
	[HarmonyPatch(typeof(Server), "InitializeServerPackets")]
    [HarmonyPostfix]
    static void CreateNewServerPackets()
    {
        session.CreateNewServerPacket("MyServerPacket", MyServerPacketMethod);
    }
    
    [HarmonyPatch(typeof(LocalClient), "InitializeClientData")]
    [HarmonyPostfix]
    static void CreateNewClientPackets()
    {
        session.CreateNewClientPacket("MyClientPacket", MyClientPacketMethod);
    }
    
    [HarmonyPatch(typeof (ClientSend), "PickupItem")]
    [HarmonyPrefix]
    static void ClientSendSharePacket(ClientSend __instance, int itemID) {
      Item item = ItemManager.Instance.list[itemID].GetComponent < Item > ();
      if (item.powerup) {
	    // Create new data object 
        Data data = new Data();
        
        // Add the powerups id to the data
        data.ints = new int[] { item.powerup.id };
        
        // Send packet to the server and call subsequent method MyServerPacketMethod()
        session.SendPacketToServer("MyServerPacket", data);
      }
    }
  If you don't want every client to get the packet use `SendPacketToAllClientsExcept` instead.

`SendPacketToAllClientsExcept(int[] except, Packet packet)` where except is an array of client ids that you don't want receiving the packet.
  

##  Reading from packets
  Packets are written in the same order as the properties are set in the `Data` class. Subsequently, this requires you to read from the packet in the same order.

    // Assuming this is the data object sent
    Data data = new Data();
    data.strings = new string[] { "Username" };
    data.ints = new int[] { 1, 5, 6 };

    // You must read all integers first no matter what order you set the properties
    Debug.log( packet.ReadInt() ); // 1
    Debug.log( packet.ReadInt() ); // 5
    Debug.log( packet.ReadInt() ); // 6
    
    // Before reading the strings
    Debug.log( packet.ReadString() ); // Username
