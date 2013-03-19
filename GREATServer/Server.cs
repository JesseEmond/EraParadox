//
//  Server.cs
//
//  Author:
//       William Turner <willtur.will@gmail.com>
//
//  Copyright (c) 2013 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Lidgren.Network;
using GREATLib;
using System.Collections.Generic;
using System.Diagnostics;

namespace GREATServer
{
	public class Server
	{
		Dictionary<NetConnection, Player> connections = new Dictionary<NetConnection, Player>();
		List<Player> players = new List<Player>();

		NetServer server;

		public Server()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("GREAT");
			config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
			config.Port = 14242;
			config.EnableUPnP = true;

#if DEBUG
			// LAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGGGGGGGG (MonoDevelop is saying bullshit, it works)
			config.SimulatedLoss = 0.01f;
			config.SimulatedMinimumLatency = 0.05f;
			config.SimulatedRandomLatency = 0.05f;
#endif

			this.server = new NetServer(config);
		}

		public void Start()
		{
			server.Start();
			server.UPnP.ForwardPort(server.Port, "GREAT Server");
		}

		public void Stop()
		{
			server.Shutdown("I'M DYING D:");
		}

		public void Update()
		{
			NetIncomingMessage msg;
			while ((msg = server.ReadMessage()) != null) {
				switch (msg.MessageType) {
					case NetIncomingMessageType.DiscoveryRequest:
						Console.WriteLine("Discover request from {0}", msg.SenderEndPoint);
						server.SendDiscoveryResponse(null, msg.SenderEndPoint);
						break;
					case NetIncomingMessageType.StatusChanged:
						NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
						if (status == NetConnectionStatus.Connected) {
							Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
							NetOutgoingMessage sup = server.CreateMessage("Sup?");
							server.SendMessage(sup, msg.SenderConnection, NetDeliveryMethod.ReliableUnordered);


							//TODO: send the player's id differently?
							int id = NextPlayerId();
							TellClientHisId(msg.SenderConnection, id);


							//TODO: have a "Game" class that handles a single game instead?
							// Note: this is temporary, to test different Network architectures
							Random r = new Random();
							Player p = 
								new Player() { 
									Id = id,
									Position = new Vec2(50f+(float)r.NextDouble() * 700f, 50f+(float)r.NextDouble() * 500f) 
								};
							players.Add(p);
							connections.Add(msg.SenderConnection, p);
							Console.WriteLine("New player joined! (spawned at " + p.Position.ToString() + ")");
						}

						else if (status == NetConnectionStatus.Disconnecting) {
							Console.WriteLine("Player left.");
							players.Remove(connections[msg.SenderConnection]);
							connections.Remove(msg.SenderConnection);
						}
						break;
					case NetIncomingMessageType.VerboseDebugMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.ErrorMessage:
						Console.WriteLine(msg.ReadString());
						break;
					case NetIncomingMessageType.Data:
						ReadData(msg);
						break;
					default:
						Console.WriteLine("Unhandled type: " + msg.MessageType);
						break;
				}
				server.Recycle(msg);
			}

			SyncPlayers();
		}

		/// <summary>
		/// Tells the client his player's identifier.
		/// </summary>
		/// <param name="conn">The client's connection.</param>
		/// <param name="id">The player's identifier.</param>
		private void TellClientHisId(NetConnection conn, int id)
		{
			NetOutgoingMessage msg = server.CreateMessage();
			msg.Write((int)ServerMessage.GivePlayerId);
			msg.Write(id);
			server.SendMessage(msg, conn, NetDeliveryMethod.ReliableUnordered);
		}

		/// <summary>
		/// Finds the next player identifier that a new player should have.
		/// </summary>
		/// <returns>The next player's identifier.</returns>
		private int NextPlayerId()
		{
			int id = Player.InvalidId;
			players.ForEach(p => id = Math.Max(id, p.Id));
			return id + 1;
		}

		/// <summary>
		/// Reads the data from an incomming message.
		/// </summary>
		/// <param name="msg">The message</para>
		private void ReadData(NetIncomingMessage msg)
		{
			try
			{
				ClientMessage type = (ClientMessage)msg.ReadInt32();

				switch (type)
				{
					case ClientMessage.MoveLeft:
						Debug.Assert(connections.ContainsKey(msg.SenderConnection));
						Physics.Move(connections[msg.SenderConnection], Direction.Left);
						break;

					case ClientMessage.MoveRight:
						Debug.Assert(connections.ContainsKey(msg.SenderConnection));
						Physics.Move(connections[msg.SenderConnection], Direction.Right);
						break;
						
					default:
						throw new NotImplementedException("Client message type not implemented.");
				}

				// Acknowledge the command, used for client-side prediction
				int commandId = msg.ReadInt32();
				AcknowledgeCommand(msg.SenderConnection, commandId);
			}
			catch (Exception)
			{
				Console.WriteLine("Data not properly formatted: " + msg.ToString());
			}
		}

		/// <summary>
		/// Acknowledges a client's command.
		/// </summary>
		/// <param name="client">Client.</param>
		/// <param name="commandId">Command identifier (NOT the command itself, but its id).</param>
		private void AcknowledgeCommand(NetConnection client, int commandId)
		{
			NetOutgoingMessage msg = server.CreateMessage();
			msg.Write((int)ServerMessage.AcknowledgeCommand);
			msg.Write(commandId);
			server.SendMessage(msg, client, NetDeliveryMethod.ReliableOrdered);
		}

		/// <summary>
		/// Syncs the states of the players.
		/// </summary>
		private void SyncPlayers()
		{
			//TODO: send data at bigger intervals (not every frame)
			NetOutgoingMessage msg = server.CreateMessage();
			int msgCode = (int)ServerMessage.PositionSync;
			msg.Write(msgCode);

			//TODO: cleaner way to sync the data?
			foreach (Player p in players) {
				msg.Write(p.Id);
				msg.WriteAllProperties(p.Position);
			}

			server.SendToAll(msg, NetDeliveryMethod.ReliableUnordered);
		}
	}
}

