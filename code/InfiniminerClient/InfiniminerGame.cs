using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Lidgren.Network;
using Lidgren.Network.Xna;

namespace Infiniminer
{
    public class InfiniminerGame : StateMasher.StateMachine
    {
        private const string config_filename = "marvulous_mod.client.config.txt";

        private void configure()
        {
            bool fullscreen = false;
            int width = 1024;
            int height = 768;

            DatafileLoader dataFile = new DatafileLoader(InfiniminerGame.configFilename());
            try
            {
                configHelper.ushortTernaryConfig(ref _connectionPort, "networkport", dataFile, InfiniminerGame.connectionPort(), (ushort)(InfiniminerGame.connectionPort() + 100));
            }
            catch (Exception) { }
            try
            {
                configHelper.stringTernaryConfig(ref _publicServerList, "public", dataFile);
            }
            catch (Exception) { }


            try
            {
                configHelper.stringTernaryConfig(ref playerHandle, "handle", dataFile);
            }
            catch (Exception) { }
            /*
            try
            {
                configHelper.stringTernaryConfig(ref _teamNameA, "team_a", dataFile);
            }
            catch (Exception) { }
            try
            {
                configHelper.colorTernaryConfig(ref _teamColorA, "color_a", dataFile);
            }
            catch (Exception) { }

            try
            {
                configHelper.stringTernaryConfig(ref _teamNameB, "team_b", dataFile);
            }
            catch (Exception) { }
            try
            {
                configHelper.colorTernaryConfig(ref _teamColorB, "color_b", dataFile);
            }
            catch (Exception) { }*/

            try
            {
                configHelper.intTernaryConfig(ref width, "width", dataFile, minScreenWidth, maxScreenWidth);
                graphicsDeviceManager.PreferredBackBufferWidth = width;
            }
            catch (Exception) { }
            try
            {
                configHelper.intTernaryConfig(ref height, "height", dataFile, minScreenHeight, maxScreenHeight);
                graphicsDeviceManager.PreferredBackBufferHeight = height;
            }
            catch (Exception) { }
            try
            {
                configHelper.boolTernaryConfig(ref fullscreen, "fullscreen", dataFile);
                graphicsDeviceManager.IsFullScreen = fullscreen;
            }
            catch (Exception) { }
            try
            {
                configHelper.boolTernaryConfig(ref RenderPretty, "pretty", dataFile);
            }
            catch (Exception) { }

            try
            {
                configHelper.boolTernaryConfig(ref DrawFrameRate, "showfps", dataFile);
            }
            catch (Exception) { }
            try
            {
                configHelper.boolTernaryConfig(ref InvertMouseYAxis, "yinvert", dataFile);
            }
            catch (Exception) { }

            try
            {
                configHelper.floatTernaryConfig(ref volumeLevel, "volume", dataFile, 0, 1);
            }
            catch (Exception) { }
            try
            {
                configHelper.boolTernaryConfig(ref noSong, "nosong", dataFile);
            }
            catch (Exception) { }
            if (volumeLevel == 0.0) // no point in checking noSound if volumeLevel is already set to zero
            {
                NoSound = true;
            }
            else
            {
                try
                {
                    configHelper.boolTernaryConfig(ref NoSound, "nosound", dataFile);
                }
                catch (Exception) { }
            }

            // stuff previlously defined in MainGameState.cs
            try
            {
                configHelper.floatTernaryConfig(ref MOVESPEED, "move", dataFile, minMOVESPEED, maxMOVESPEED);
            }
            catch (Exception) { }
            try
            {
                configHelper.floatTernaryConfig(ref GRAVITY, "gravity", dataFile, minGRAVITY, maxGRAVITY);
            }
            catch (Exception) { }
            try
            {
                configHelper.floatTernaryConfig(ref JUMPVELOCITY, "jump", dataFile, minJUMPVELOCITY, maxJUMPVELOCITY);
            }
            catch (Exception) { }
            try
            {
                configHelper.floatTernaryConfig(ref CLIMBVELOCITY, "climb", dataFile, minCLIMBVELOCITY, maxCLIMBVELOCITY);
            }
            catch (Exception) { }
            try
            {
                configHelper.floatTernaryConfig(ref DIEVELOCITY, "gosplat", dataFile, minDIEVELOCITY, maxDIEVELOCITY);
            }
            catch (Exception) { }
            try
            {
                configHelper.stringTernaryConfig(ref SPLATMSG, "msg_splat", dataFile);
            }
            catch (Exception) { }
            try
            {
                configHelper.stringTernaryConfig(ref LAVAMSG, "msg_lava", dataFile);
            }
            catch (Exception) { }
            try
            {
                configHelper.stringTernaryConfig(ref SHOCKMSG, "msg_shock", dataFile);
            }
            catch (Exception) { }
            try
            {
                configHelper.stringTernaryConfig(ref THEEARTHISFLAT, "msg_misadventure", dataFile);
            }
            catch (Exception) { }
            try
            {
                configHelper.stringTernaryConfig(ref DONTBEATNTDICK, "msg_explosion", dataFile);
            }
            catch (Exception) { }
        }

        private static ushort _connectionPort = 5565;
        public static ushort connectionPort()
        {
            return _connectionPort;
        }

        private static string _publicServerList = "http://apps.keithholman.net/plain";
        public const string gameName = "INFINIMINER";
        public static string publicServerList()
        {
            return _publicServerList;
        }

        private const string song_filename = "song_title";
        private static bool noSong = false;

        private static string _teamNameA  = "RED";
        private static Color _teamColorA = new Color(222, 24, 24);
        private static Color _teamBloodA = Color.Red;

        private static string _teamNameB = "BLUE";
        private static Color _teamColorB = new Color(80, 150, 255);
        private static Color _teamBloodB = Color.Blue;

        public static string teamNameA()
        {
            return _teamNameA;
        }
        public static Color teamColorA()
        {
            return _teamColorA;
        }
        public static Color teamBloodA()
        {
            return _teamBloodA;
        }

        public static string teamNameB()
        {
            return _teamNameB;
        }
        public static Color teamColorB()
        {
            return _teamColorB;
        }
        public static Color teamBloodB()
        {
            return _teamBloodB;
        }

        public static float MOVESPEED = 3.5f;
            private const float minMOVESPEED = 1.0f; // any slower and things are going to be too slow to be useful
            private const float maxMOVESPEED = 50.0f; // any slower and things are going to be too slow to be useful

        public static float GRAVITY = -8.0f;
            private const float minGRAVITY = -20.0f;
            private const float maxGRAVITY = -0.5f;

        public static float JUMPVELOCITY = 4.0f;
            private const float minJUMPVELOCITY = 0.5f;
            private const float maxJUMPVELOCITY = 20f;

        public static float CLIMBVELOCITY = 2.5f;
            private const float minCLIMBVELOCITY = 0.5f;
            private const float maxCLIMBVELOCITY = 20.0f;

        public static float DIEVELOCITY = 15.0f;
            private const float minDIEVELOCITY = 0.5f;
            private const float maxDIEVELOCITY = 50.0f;

        public static string SPLATMSG = "WAS KILLED BY GRAVITY!";
        public static string LAVAMSG  = "WAS INCINERATED BY LAVA!";
        public static string SHOCKMSG = "WAS ELECTROCUTED!";
        public static string THEEARTHISFLAT = "WAS KILLED BY MISADVENTURE!";
        public static string DONTBEATNTDICK = "WAS KILLED IN AN EXPLOSION!";

        double timeSinceLastUpdate = 0;
        string playerHandle = "I NEED TO RENAME MYSELF";
        float volumeLevel = 1.0f;
        NetBuffer msgBuffer = null;
        Song songTitle = null;

        private static int minScreenWidth  = 320;
        private static int maxScreenWidth  = 1440;
        private static int minScreenHeight = 240;
        private static int maxScreenHeight = 1080;

        public bool RenderPretty = true;
        public bool DrawFrameRate = false;
        public bool InvertMouseYAxis = false;
        public bool NoSound = false;

        public const string INFINIMINER_VERSION = "v1.5";
        public const int GROUND_LEVEL = 8;

        public InfiniminerGame(string[] args)
        {
        }
        public static string configFilename()
        {
            return config_filename;
        }

        public static string Sanitize(string input)
        {
            string output = "";
            for (int i = 0; i < input.Length; i++)
            {
                char c = (char)input[i];
                if (c >= 32 && c <= 126)
                    output += c;
            }
            return output;
        }

        public void JoinGame(IPEndPoint serverEndPoint)
        {
            // Clear out the map load progress indicator.
            //propertyBag.mapLoadProgress = new bool[propertyBag.mapSize, propertyBag.mapSize];
            //for (int i = 0; i < propertyBag.mapSize; i++)
            //    for (int j = 0; j < propertyBag.mapSize; j++)
            //        propertyBag.mapLoadProgress[i,j] = false;

            // Create our connect message.
            NetBuffer connectBuffer = propertyBag.netClient.CreateBuffer();
            connectBuffer.Write(propertyBag.playerHandle);
            connectBuffer.Write(INFINIMINER_VERSION);

            // Connect to the server.
            propertyBag.netClient.Connect(serverEndPoint, connectBuffer.ToArray());
        }

        public List<ServerInformation> EnumerateServers(float discoveryTime)
        {
            List<ServerInformation> serverList = new List<ServerInformation>();
            
            // Discover local servers.
            propertyBag.netClient.DiscoverLocalServers(InfiniminerGame.connectionPort());
            NetBuffer msgBuffer = propertyBag.netClient.CreateBuffer();
            NetMessageType msgType;
            float timeTaken = 0;
            while (timeTaken < discoveryTime)
            {
                while (propertyBag.netClient.ReadMessage(msgBuffer, out msgType))
                {
                    if (msgType == NetMessageType.ServerDiscovered)
                    {
                        bool serverFound = false;
                        ServerInformation serverInfo = new ServerInformation(msgBuffer);
                        foreach (ServerInformation si in serverList)
                            if (si.Equals(serverInfo))
                                serverFound = true;
                        if (!serverFound)
                            serverList.Add(serverInfo);
                    }
                }

                timeTaken += 0.1f;
                Thread.Sleep(100);
            }

            // Discover remote servers.
            try
            {
                string publicList = HttpRequest.Get(publicServerList(), null);
                foreach (string s in publicList.Split("\r\n".ToCharArray()))
                {
                    string[] args = s.Split(";".ToCharArray());
                    if (args.Length == 6)
                    {
                        IPAddress serverIp;
                        if (IPAddress.TryParse(args[1], out serverIp) && args[2] == gameName)
                        {
                            ServerInformation serverInfo = new ServerInformation(serverIp, args[0], args[5], args[3], args[4]);
                            serverList.Add(serverInfo);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return serverList;
        }

        public void UpdateNetwork(GameTime gameTime)
        {
            // Update the server with our status.
            timeSinceLastUpdate += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeSinceLastUpdate > 0.05)
            {
                timeSinceLastUpdate = 0;
                if (CurrentStateType == "Infiniminer.States.MainGameState")
                    propertyBag.SendPlayerUpdate();
            }

            // Recieve messages from the server.
            NetMessageType msgType;
            while (propertyBag.netClient.ReadMessage(msgBuffer, out msgType))
            {
                switch (msgType)
                {
                    case NetMessageType.StatusChanged:
                        {
                            if (propertyBag.netClient.Status == NetConnectionStatus.Disconnected)
                            {
                                ChangeState("Infiniminer.States.ServerBrowserState");
                            }
                            else if (propertyBag.netClient.Status == NetConnectionStatus.Connected)
                            {
                                if (propertyBag.MapSize == 0)
                                {
                                    //Get the map size from the server message
                                    propertyBag.MapSize = propertyBag.netClient.ServerConnection.RemoteHailData[0];
                                    // Clear out the map load progress indicator.
                                    propertyBag.mapLoadProgress = new bool[propertyBag.MapSize, propertyBag.MapSize];
                                }
                            }
                        }
                        break;

                    case NetMessageType.ConnectionRejected:
                        {
                            string[] reason = msgBuffer.ReadString().Split(";".ToCharArray());
                            if (reason.Length < 2 || reason[0] == "VER")
                                MessageBox.Show("Error: client/server version incompability!\r\nServer: " + msgBuffer.ReadString() + "\r\nClient: " + INFINIMINER_VERSION);
                            else
                                MessageBox.Show("Error: you are banned from this server!");
                            ChangeState("Infiniminer.States.ServerBrowserState");
                        }
                        break;

                    case NetMessageType.Data:
                        {
                            InfiniminerMessage dataType = (InfiniminerMessage)msgBuffer.ReadByte();
                            switch (dataType)
                            {
                                case InfiniminerMessage.BlockBulkTransfer:
                                    {
                                        //Decompress the sent data into its origonal size in decompressed stream
                                        var compressed = msgBuffer.ReadBytes(msgBuffer.LengthBytes-msgBuffer.Position/8);
                                        var compressedstream = new System.IO.MemoryStream(compressed);
                                        var decompresser = new System.IO.Compression.GZipStream(compressedstream, System.IO.Compression.CompressionMode.Decompress);

                                        byte x = (byte)decompresser.ReadByte();
                                        byte y = (byte)decompresser.ReadByte();
                                        propertyBag.mapLoadProgress[x,y] = true;
                                        for (byte dy = 0; dy < propertyBag.MapSize; dy++)
                                            for (byte z = 0; z < propertyBag.MapSize; z++)
                                            {
                                                BlockType blockType = (BlockType)decompresser.ReadByte();
                                                if (blockType != BlockType.None)
                                                    propertyBag.blockEngine.downloadList[x, y+dy, z] = blockType;
                                            }
                                        bool downloadComplete = true;
                                        for (x = 0; x < propertyBag.MapSize; x++)
                                            for (y = 0; y < propertyBag.MapSize; y += propertyBag.MapSize)
                                                if (propertyBag.mapLoadProgress[x,y] == false)
                                                {
                                                    downloadComplete = false;
                                                    break;
                                                }
                                        if (downloadComplete)
                                        {
                                            ChangeState("Infiniminer.States.TeamSelectionState");
                                            if (!NoSound)
                                                MediaPlayer.Stop();
                                            propertyBag.blockEngine.DownloadComplete();
                                        }
                                    }
                                    break;


                                case InfiniminerMessage.SetBeacon:
                                    {
                                        Vector3 position = msgBuffer.ReadVector3();
                                        string text = msgBuffer.ReadString();
                                        PlayerTeam team = (PlayerTeam)msgBuffer.ReadByte();

                                        if (text == "")
                                        {
                                            if (propertyBag.beaconList.ContainsKey(position))
                                                propertyBag.beaconList.Remove(position);
                                        }
                                        else
                                        {
                                            Beacon newBeacon = new Beacon();
                                            newBeacon.ID = text;
                                            newBeacon.Team = team;
                                            propertyBag.beaconList.Add(position, newBeacon);
                                        }
                                    }
                                    break;

                                case InfiniminerMessage.TriggerConstructionGunAnimation:
                                    {
                                        propertyBag.constructionGunAnimation = msgBuffer.ReadFloat();
                                        if (propertyBag.constructionGunAnimation <= -0.1)
                                            propertyBag.PlaySound(InfiniminerSound.RadarSwitch);
                                    }
                                    break;

                                case InfiniminerMessage.ResourceUpdate:
                                    {
                                        // ore, cash, weight, max ore, max weight, team ore, team A cash, team B cash, all uint
                                        propertyBag.playerOre = msgBuffer.ReadUInt32();
                                        propertyBag.playerCash = msgBuffer.ReadUInt32();
                                        propertyBag.playerWeight = msgBuffer.ReadUInt32();
                                        propertyBag.playerOreMax = msgBuffer.ReadUInt32();
                                        propertyBag.playerWeightMax = msgBuffer.ReadUInt32();
                                        propertyBag.teamOre = msgBuffer.ReadUInt32();
                                        propertyBag.teamACash = msgBuffer.ReadUInt32();
                                        propertyBag.teamBCash = msgBuffer.ReadUInt32();
                                    }
                                    break;

                                case InfiniminerMessage.BlockSet:
                                    {
                                        // x, y, z, type, all bytes
                                        byte x = msgBuffer.ReadByte();
                                        byte y = msgBuffer.ReadByte();
                                        byte z = msgBuffer.ReadByte();
                                        BlockType blockType = (BlockType)msgBuffer.ReadByte();
                                        if (blockType == BlockType.None)
                                        {
                                            if (propertyBag.blockEngine.BlockAtPoint(new Vector3(x, y, z)) != BlockType.None)
                                                propertyBag.blockEngine.RemoveBlock(x, y, z);
                                        }
                                        else
                                        {
                                            if (propertyBag.blockEngine.BlockAtPoint(new Vector3(x, y, z)) != BlockType.None)
                                                propertyBag.blockEngine.RemoveBlock(x, y, z);
                                            propertyBag.blockEngine.AddBlock(x, y, z, blockType);
                                            CheckForStandingInLava();                                          
                                        }
                                    }
                                    break;

                                case InfiniminerMessage.TriggerExplosion:
                                    {
                                        Vector3 blockPos = msgBuffer.ReadVector3();
                                        
                                        // Play the explosion sound.
                                        propertyBag.PlaySound(InfiniminerSound.Explosion, blockPos);

                                        // Create some particles.
                                        propertyBag.particleEngine.CreateExplosionDebris(blockPos);

                                        // Figure out what the effect is.
                                        float distFromExplosive = (blockPos + 0.5f * Vector3.One - propertyBag.playerPosition).Length();
                                        if (distFromExplosive < 3)
                                            propertyBag.KillPlayer(InfiniminerGame.DONTBEATNTDICK);
                                        else if (distFromExplosive < 8)
                                        {
                                            // If we're not in explosion mode, turn it on with the minimum ammount of shakiness.
                                            if (propertyBag.screenEffect != ScreenEffect.Explosion)
                                            {
                                                propertyBag.screenEffect = ScreenEffect.Explosion;
                                                propertyBag.screenEffectCounter = 2;
                                            }
                                            // If this bomb would result in a bigger shake, use its value.
                                            propertyBag.screenEffectCounter = Math.Min(propertyBag.screenEffectCounter, (distFromExplosive - 2) / 5);
                                        }
                                    }
                                    break;

                                case InfiniminerMessage.PlayerSetTeam:
                                    {
                                        uint playerId = msgBuffer.ReadUInt32();
                                        if (propertyBag.playerList.ContainsKey(playerId))
                                        {
                                            Player player = propertyBag.playerList[playerId];
                                            player.Team = (PlayerTeam)msgBuffer.ReadByte();
                                        }
                                    }
                                    break;

                                case InfiniminerMessage.PlayerJoined:
                                    {
                                        uint playerId = msgBuffer.ReadUInt32();
                                        string playerName = msgBuffer.ReadString();
                                        bool thisIsMe = msgBuffer.ReadBoolean();
                                        bool playerAlive = msgBuffer.ReadBoolean();
                                        propertyBag.playerList[playerId] = new Player(null, (Game)this);
                                        propertyBag.playerList[playerId].Handle = playerName;
                                        propertyBag.playerList[playerId].ID = playerId;
                                        propertyBag.playerList[playerId].Alive = playerAlive;
                                        if (thisIsMe)
                                            propertyBag.playerMyId = playerId;
                                    }
                                    break;

                                case InfiniminerMessage.PlayerLeft:
                                    {
                                        uint playerId = msgBuffer.ReadUInt32();
                                        if (propertyBag.playerList.ContainsKey(playerId))
                                            propertyBag.playerList.Remove(playerId);
                                    }
                                    break;

                                case InfiniminerMessage.PlayerDead:
                                    {
                                        uint playerId = msgBuffer.ReadUInt32();
                                        if (propertyBag.playerList.ContainsKey(playerId))
                                        {
                                            Player player = propertyBag.playerList[playerId];
                                            player.Alive = false;
                                            propertyBag.particleEngine.CreateBloodSplatter(player.Position, player.Team == PlayerTeam.A ? InfiniminerGame.teamBloodA() : InfiniminerGame.teamBloodB());
                                            if (playerId != propertyBag.playerMyId)
                                                propertyBag.PlaySound(InfiniminerSound.Death, player.Position);
                                        }
                                    }
                                    break;

                                case InfiniminerMessage.PlayerAlive:
                                    {
                                        uint playerId = msgBuffer.ReadUInt32();
                                        if (propertyBag.playerList.ContainsKey(playerId))
                                        {
                                            Player player = propertyBag.playerList[playerId];
                                            player.Alive = true;
                                        }
                                    }
                                    break;

                                case InfiniminerMessage.PlayerUpdate:
                                    {
                                        uint playerId = msgBuffer.ReadUInt32();
                                        if (propertyBag.playerList.ContainsKey(playerId))
                                        {
                                            Player player = propertyBag.playerList[playerId];
                                            player.UpdatePosition(msgBuffer.ReadVector3(), gameTime.TotalGameTime.TotalSeconds);
                                            player.Heading = msgBuffer.ReadVector3();
                                            player.Tool = (PlayerTools)msgBuffer.ReadByte();
                                            player.UsingTool = msgBuffer.ReadBoolean();
                                            player.Score = (uint)(msgBuffer.ReadUInt16() * 100);
                                        }
                                    }
                                    break;

                                case InfiniminerMessage.GameOver:
                                    {
                                        propertyBag.teamWinners = (PlayerTeam)msgBuffer.ReadByte();
                                    }
                                    break;

                                case InfiniminerMessage.ChatMessage:
                                    {
                                        ChatMessageType chatType = (ChatMessageType)msgBuffer.ReadByte();
                                        string chatString = msgBuffer.ReadString();
                                        ChatMessage chatMsg = new ChatMessage(chatString, chatType, 10);
                                        propertyBag.chatBuffer.Insert(0, chatMsg);
                                        propertyBag.PlaySound(InfiniminerSound.ClickLow);
                                    }
                                    break;

                                case InfiniminerMessage.PlayerPing:
                                    {
                                        uint playerId = (uint)msgBuffer.ReadInt32();
                                        if (propertyBag.playerList.ContainsKey(playerId))
                                        {
                                            if (propertyBag.playerList[playerId].Team == propertyBag.playerTeam)
                                            {
                                                propertyBag.playerList[playerId].Ping = 1;
                                                propertyBag.PlaySound(InfiniminerSound.Ping);
                                            }
                                        }
                                    }
                                    break;

                                case InfiniminerMessage.PlaySound:
                                    {
                                        InfiniminerSound sound = (InfiniminerSound)msgBuffer.ReadByte();
                                        bool hasPosition = msgBuffer.ReadBoolean();
                                        if (hasPosition)
                                        {
                                            Vector3 soundPosition = msgBuffer.ReadVector3();
                                            propertyBag.PlaySound(sound, soundPosition);
                                        }
                                        else
                                            propertyBag.PlaySound(sound);
                                    }
                                    break;
                                case InfiniminerMessage.TeamConfig:
                                    {
                                        PlayerTeam team = (PlayerTeam)msgBuffer.ReadByte();
                                        string name = msgBuffer.ReadString();
                                        Color teamColor = configHelper.string2Color(msgBuffer.ReadString());
                                        Color bloodColor = configHelper.string2Color(msgBuffer.ReadString());
                                        switch (team)
                                        {
                                            case PlayerTeam.A:
                                                _teamNameA = name;
                                                _teamColorA = teamColor;
                                                _teamBloodA = bloodColor;
                                            break;
                                            case PlayerTeam.B:
                                            _teamNameB = name;
                                                _teamColorB = teamColor;
                                                _teamBloodB = bloodColor;
                                            break;
                                        }
                                    }
                                 break;
                                case InfiniminerMessage.compatibleClient:
                                 {
                                     
                                     ProcessStartInfo url = new ProcessStartInfo(msgBuffer.ReadString());
                                     Process process = new Process();
                                     process.StartInfo = url;
#if !DEBUG
                                     try
                                     {
#endif
                                         process.Start();
#if !DEBUG
                                     }
                                     catch (Exception e)
                                     {
                                         MessageBox.Show("Could not load url for compatible client!\n" + e.ToString(),"Error",MessageBoxButtons.OK);
                                     }
#endif
                                 }
                                 break;
                            }
                        }
                        break;
                }
            }

            // Make sure our network thread actually gets to run.
            Thread.Sleep(1);
        }

        private void CheckForStandingInLava()
        {
            // Copied from TryToMoveTo; responsible for checking if lava has flowed over us.

            Vector3 movePosition = propertyBag.playerPosition;
            Vector3 midBodyPoint = movePosition + new Vector3(0, -0.7f, 0);
            Vector3 lowerBodyPoint = movePosition + new Vector3(0, -1.4f, 0);
            BlockType lowerBlock = propertyBag.blockEngine.BlockAtPoint(lowerBodyPoint);
            BlockType midBlock = propertyBag.blockEngine.BlockAtPoint(midBodyPoint);
            BlockType upperBlock = propertyBag.blockEngine.BlockAtPoint(movePosition);
            if (upperBlock == BlockType.Lava || lowerBlock == BlockType.Lava || midBlock == BlockType.Lava)
            {
                propertyBag.KillPlayer("WAS INCINERATED BY LAVA!");
            }
        }

        protected override void Initialize()
        {
            graphicsDeviceManager.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

            configure();

            graphicsDeviceManager.ApplyChanges();
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            propertyBag.netClient.Shutdown("Client exiting.");
            
            base.OnExiting(sender, args);
        }

        public void ResetPropertyBag()
        {
            if (propertyBag != null)
                propertyBag.netClient.Shutdown("");

            propertyBag = new Infiniminer.PropertyBag(this);
            propertyBag.playerHandle = playerHandle;
            propertyBag.volumeLevel = volumeLevel;
            msgBuffer = propertyBag.netClient.CreateBuffer();
        }

        protected override void LoadContent()
        {
            // Initialize the property bag.
            ResetPropertyBag();

            // Set the initial state to team selection
            ChangeState("Infiniminer.States.TitleState");

            // Play the title music.
            if (!NoSound)
            {
                songTitle = Content.Load<Song>(song_filename);
                if (!noSong)
                {
                    MediaPlayer.Play(songTitle);
                }
                MediaPlayer.Volume = propertyBag.volumeLevel;
            }
        }
    }
}