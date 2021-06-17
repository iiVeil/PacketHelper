using HarmonyLib;
using BepInEx;
using UnityEngine;
using TMPro;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Steamworks;
using Steamworks.Data;

namespace NetworkHelper
{

    /*
     * Data class to be used when sending packets to the server
     * 
     * Data data = new NetworkHelper.Data;
     * data.ints = {1, 2, 3, 4};
     * ...
     * 
     * A seemingly shitty way to work around the fact i cant pass mixed arrays as arguments to functions.
     * This could use some work by an experienced modder.
     * 
     */
    public class Data
    {
        public int[] ints = null;
        public string[] strings = null;
        public float[] floats = null;
        public bool[] bools = null;
        public short[] shorts = null;
        public long[] longs = null;
        public Vector3[] vector3s = null;
        public Quaternion[] quaternions = null;
    }



    public class Session
    {
        public int packetOffset = 0;
        public int totalNewPackets = 0;
        public Dictionary<string, int> newPackets = new Dictionary<string, int>() { };

        /*
         * Session("Mod.GUID")
         * 
         * Uses input from GUID to create a packetOffset to ensure packet numbers dont collide
         * 
         */
        public Session(string GUID)
        {
            foreach (Char c in GUID.ToCharArray()) {
                if ((int)c % 32 % 5 == 0) { this.packetOffset *= (int)c % 32; } else { this.packetOffset += (int)c % 32; }
            }
            Debug.Log($"Initialized Packet Offset: {this.packetOffset}");
        }

        /*
         * Session.CreateNewServerPacket("identifying name", MyNameSpace.MethodName)
         * 
         * Intializes a new packet on the server with a helpful indentifying name to send later.
         * 
         * method represents the method to run when the server recieves this packet.
         * 
         */
        public void CreateNewServerPacket(string name, Action<int, Packet> method)
        {
            if (this.newPackets.ContainsKey(name)) { Debug.LogError("A packet identifier with this name already exists."); return; }
            int packetID = this.packetOffset + this.totalNewPackets;
            Server.PacketHandlers.Add(packetID, new Server.PacketHandler(method));
            this.newPackets.Add(name, packetID);
            this.totalNewPackets++;
        }

        /*
         * Session.CreateNewClientPacket("identifying name", MyNameSpace.MethodName)
         * 
         * Initializes a new packet on the server with a helpful identifying name to send later.
         * 
         * method represents the method to run when the client recieves this packet.
         * 
         */

        public void CreateNewClientPacket(string name, Action<Packet> method)
        {
            if (this.newPackets.ContainsKey(name)) { Debug.LogError("A packet identifier with this name already exists."); return; }
            int packetID = this.packetOffset + this.totalNewPackets;
            LocalClient.packetHandlers.Add(packetID, new LocalClient.PacketHandler(method));
            this.newPackets.Add(name, packetID);
            this.totalNewPackets++;
        }

        /*
         * Data data = new Data;
         * data.ints = new int[] {1, 2, 3}
         * ...
         * SendPacketToServer("identifying name", data)
         * 
         * It is important to note that packets are written in this order: 
         *          ints > strings > bools > floats > shorts > longs > vector3s > quaternions
         *
         * Due to how the packet is written to; this REQUIRES you to read from the packet IN THE SAME ORDER YOU WROTE TO IT.
         * 
         */
        public void SendPacketToServer(string name, Data data)
        {
            if (!this.newPackets.ContainsKey(name)) { Debug.LogError($" No valid custom packet named {name}.");return; }

            using (Packet packet = new Packet(this.newPackets[name]))
            {
                if (data.ints != null)
                {
                    foreach (int i in data.ints)
                    {
                        packet.Write(i);
                    }
                }
                if (data.strings != null)
                {
                    foreach (string i in data.strings)
                    {
                        packet.Write(i);
                    }
                }
                if (data.bools != null)
                {
                    foreach (bool i in data.bools)
                    {
                        packet.Write(i);
                    }
                }
                if (data.floats != null)
                {
                    foreach (int i in data.floats)
                    {
                        packet.Write(i);
                    }
                }
                if (data.shorts != null)
                {
                    foreach (int i in data.ints)
                    {
                        packet.Write(i);
                    }
                }
                if (data.longs != null)
                {
                    foreach (long i in data.longs)
                    {
                        packet.Write(i);
                    }
                }
                if (data.vector3s != null)
                {
                    foreach (Vector3 i in data.vector3s)
                    {
                        packet.Write(i);
                    }
                }
                if (data.quaternions != null)
                {
                    foreach (Quaternion i in data.quaternions)
                    {
                        packet.Write(i);
                    }
                }
                SendTCPDataToServer(packet);
            }
        }
        
        /*
         * Data data = new Data;
         * data.ints = new int[] {1, 2, 3}
         * ...
         * SendPacketToAllClients("identifying name", data)
         * 
         * It is important to note that packets are written in this order: 
         *          ints > strings > bools > floats > shorts > longs > vector3s > quaternions
         *
         * Due to how the packet is written to; this REQUIRES you to read from the packet IN THE SAME ORDER YOU WROTE TO IT.
         * 
         */
        public void SendPacketToAllClients(string name, Data data)
        {
            if (!this.newPackets.ContainsKey(name)) { Debug.LogError($" No valid custom packet named {name}."); return; }

            using (Packet packet = new Packet(this.newPackets[name]))
            {
                if (data.ints != null)
                {
                    foreach (int i in data.ints)
                    {
                        packet.Write(i);
                    }
                }
                if (data.strings != null)
                {
                    foreach (string i in data.strings)
                    {
                        packet.Write(i);
                    }
                }
                if (data.bools != null)
                {
                    foreach (bool i in data.bools)
                    {
                        packet.Write(i);
                    }
                }
                if (data.floats != null)
                {
                    foreach (int i in data.floats)
                    {
                        packet.Write(i);
                    }
                }
                if (data.shorts != null)
                {
                    foreach (int i in data.ints)
                    {
                        packet.Write(i);
                    }
                }
                if (data.longs != null)
                {
                    foreach (long i in data.longs)
                    {
                        packet.Write(i);
                    }
                }
                if (data.vector3s != null)
                {
                    foreach (Vector3 i in data.vector3s)
                    {
                        packet.Write(i);
                    }
                }
                if (data.quaternions != null)
                {
                    foreach (Quaternion i in data.quaternions)
                    {
                        packet.Write(i);
                    }
                }
                SendTCPDataToAll(packet);
            }
        }

        /*
         * Data data = new Data;
         * data.ints = new int[] {1, 2, 3}
         * ...
         * SendPacketToAllClientsExcept( { 0, 2 } , "identifying name", data)
         * 
         * In this case, the packet would be sent to every user except the Host(0) and the 2nd user that connected(2)
         * 
         * It is important to note that packets are written in this order: 
         *          ints > strings > bools > floats > shorts > longs > vector3s > quaternions
         *
         * Due to how the packet is written to; this REQUIRES you to read from the packet IN THE SAME ORDER YOU WROTE TO IT.
         * 
         */
        public void SendPacketToAllClientsExcept(int[] except, string name, Data data)
        {
            if (!this.newPackets.ContainsKey(name)) { Debug.LogError($" No valid custom packet named {name}."); return; }

            using (Packet packet = new Packet(this.newPackets[name]))
            {

                if (data.ints != null)
                {
                    foreach (int i in data.ints)
                    {
                        packet.Write(i);
                    }
                }
                if (data.strings != null)
                {
                    foreach (string i in data.strings)
                    {
                        packet.Write(i);
                    }
                }
                if (data.bools != null)
                {
                    foreach (bool i in data.bools)
                    {
                        packet.Write(i);
                    }
                }
                if (data.floats != null)
                {
                    foreach (int i in data.floats)
                    {
                        packet.Write(i);
                    }
                }
                if (data.shorts != null)
                {
                    foreach (int i in data.ints)
                    {
                        packet.Write(i);
                    }
                }
                if (data.longs != null)
                {
                    foreach (long i in data.longs)
                    {
                        packet.Write(i);
                    }
                }
                if (data.vector3s != null)
                {
                    foreach (Vector3 i in data.vector3s)
                    {
                        packet.Write(i);
                    }
                }
                if (data.quaternions != null)
                {
                    foreach (Quaternion i in data.quaternions)
                    {
                        packet.Write(i);
                    }
                }
                SendTCPDataToAllExcept(except, packet);
            }
        }
        
        
        
        
        /*
         * Private methods to send data 
         */
        private static void SendTCPDataToServer(Packet packet)
        {
            ClientSend.bytesSent += packet.Length();
            ClientSend.packetsSent++;
            packet.WriteLength();
            if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
            {
                LocalClient.instance.tcp.SendData(packet);
                return;
            }
            SteamPacketManager.SendPacket(LocalClient.instance.serverHost.Value, packet, P2PSend.Reliable, SteamPacketManager.NetworkChannel.ToServer);
        }
        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
            {
                for (int i = 1; i < Server.MaxPlayers; i++)
                {
                    Server.clients[i].tcp.SendData(packet);
                }
                return;
            }
            foreach (Client client in Server.clients.Values)
            {
                if (((client != null) ? client.player : null) != null)
                {
                    SteamPacketManager.SendPacket(client.player.steamId.Value, packet, ServerSend.TCPvariant, SteamPacketManager.NetworkChannel.ToClient);
                }
            }
        }
        private static void SendTCPDataToAllExcept(int[] except, Packet packet)
        {
            packet.WriteLength();
            if (NetworkController.Instance.networkType == NetworkController.NetworkType.Classic)
            {
                for (int i = 1; i < Server.MaxPlayers; i++)
                {
                    Server.clients[i].tcp.SendData(packet);
                }
                return;
            }
            foreach (Client client in Server.clients.Values)
            {
                if (((client != null) ? client.player : null) != null)
                {
                    if (!except.Contains(client.id))
                    {
                        SteamPacketManager.SendPacket(client.player.steamId.Value, packet, ServerSend.TCPvariant, SteamPacketManager.NetworkChannel.ToClient);
                    }
                }
            }
        }


    }
}


