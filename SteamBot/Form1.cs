using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SteamKit2;


namespace SteamBot
{
    public partial class Form1 : Form
    {
        static String version = "0.6";
        static String username = "KovetsBot";
        static String password = "mZ7ezeYUF5DWc2LddT3UWGZe";
        static SteamClient steamclient;
        static CallbackManager manager;
        static SteamUser steamuser;
        static SteamFriends steamfriends;
        static bool isRunning = false;

        public Form1()
        {
            InitializeComponent(); 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void Form1_Shown(Object sender, EventArgs e)
        {
        }

        
        
        void SteamLogin()
        {
            steamclient = new SteamClient();
            manager = new CallbackManager(steamclient);
            steamuser = steamclient.GetHandler<SteamUser>();
            steamfriends = steamclient.GetHandler<SteamFriends>();

            new Callback<SteamClient.ConnectedCallback>(OnConnected, manager);
            new Callback<SteamUser.LoggedOnCallback>(OnLoggedOn, manager);
            new Callback<SteamUser.AccountInfoCallback>(OnAccountInfo, manager);
            new Callback<SteamFriends.FriendMsgCallback>(OnChatMsg, manager);
            new Callback<SteamFriends.FriendsListCallback>(OnFriendList, manager);

            isRunning = true;
            steamclient.Connect();
            while (isRunning)
            {
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
            Console.ReadKey();
        }
        void OnConnected(SteamClient.ConnectedCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                isRunning = false;
                return;
            }

            if (listBox1.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {

                    listBox1.Items.Add("Connected to steam network \nLogging in " + username);

                }));
            }
            steamuser.LogOn(new SteamUser.LogOnDetails { Username = username, Password = password, });

        }
        void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.AccountLogonDenied)
            {
                if (listBox1.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        listBox1.Items.Add("Account is steam guard protected Failed to LogOn");
                    }));
                }
                return;
            }
            if (callback.Result != EResult.OK)
            {
                if (listBox1.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        listBox1.Items.Add("Unable to log in to Steam: " + callback.Result);
                    }));
                }
                return;
            }
            if (listBox1.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    
                    listBox1.Items.Add("Succesfully logged on to steamUser " + username);

                }));
            }
        }
        void OnAccountInfo(SteamUser.AccountInfoCallback callback)
        {
            steamfriends.SetPersonaState(EPersonaState.Online);
        }
        void OnChatMsg(SteamFriends.FriendMsgCallback callback)
        {
            String sender = steamfriends.GetFriendPersonaName(callback.Sender);
            String steamID = callback.Sender.AccountID.ToString();
            String steamID64 = callback.Sender.ToString();
            String accountType = callback.Sender.AccountType.ToString();
            String FriendPlayed = steamfriends.GetFriendGamePlayedName(callback.Sender.AccountID);
            String status = SetStatus(steamID64);

            if (callback.EntryType == EChatEntryType.ChatMsg)
            {

                this.Invoke(new MethodInvoker(delegate ()
                {
                    listBox1.Items.Add("");
                    listBox1.Items.Add("Chat log:");
                    listBox1.Items.Add(steamfriends.GetFriendPersonaName(callback.Sender) + " Has sent you '" + callback.Message + "'");
                }));
            }



            if (callback.Message.Equals("!help") || callback.Message.Equals("!Help") || callback.Message.Equals("!HELP"))
            {
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "List of commends:");
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "!help - get the list of commends");
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "!8ball - plays the classic 8ball game");
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "!roll - rolls a dice and gives you the result");
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "!info - gives you info about the bot");
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "!coinflip - flips a coin and gives you the result");
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "!github - gives you the creators github page");
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "tell me about myself - tells you the info the bot collected about you");
            }
            if (callback.Message.Equals("Hi") || callback.Message.Equals("hi") || callback.Message.Equals("Hello") || callback.Message.Equals("hello"))
            {
                Random rnd = new Random();
                int place = rnd.Next(1, 3);
                String[] answers = { "Hello", "Hi", "hello " + sender };
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, answers[place]);
            }
            if (callback.Message.Equals("tell me about myself") || callback.Message.Equals("Tell me about myself"))
            {
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "name: " + sender);
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "steamID: " + steamID);
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "steamID64: " + steamID64);
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "account type: " + accountType);

                if (FriendPlayed == null)
                {
                    steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "Game played: none");
                }
                else { steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "Game played: " + FriendPlayed); }

                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "bot status: " + status);
            }
            if (callback.Message.Equals("whats my status") || callback.Message.Equals("Whats my status") || callback.Message.Equals("what's my status") || callback.Message.Equals("What's my status"))
            {
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "your status is " + status);
            }
            if (callback.Message.Equals("!Roll") || callback.Message.Equals("!roll"))
            {
                Random rnd = new Random();
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "The dice have been rolled and the answer is " + rnd.Next(1, 6));
            }
            if (callback.Message.Contains("!8ball") || callback.Message.Contains("!8Ball"))
            {
                Random rnd = new Random();
                string[] answers = { "yes", "no", "maybe", "why would you ask that", "Ask later", "lol idk" };

                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, answers[rnd.Next(6) + 1]);
            }
            if (callback.Message.Equals("!info") || callback.Message.Equals("!Info"))
            {
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "Bot version - " + version);
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "Bot creator - shay alon vash");
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "steam profile - http://steamcommunity.com/id/kovetsbot");
            }
            if (callback.Message.Equals("!github") || callback.Message.Equals("!Github"))
            {
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "");
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "Github link");
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "https://github.com/ShayVash");
            }
            if (callback.Message.Equals("!coinflip") || callback.Message.Equals("!Coinflip"))
            {
                Random rnd = new Random();
                int FlipResult = rnd.Next(2) + 1;
                if (FlipResult == 1)
                    steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "The coin has been flipped and the result is Heads");
                if (FlipResult == 2)
                    steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "The coin has been flipped and the result is Tails");
            }
            if (callback.Message.Equals("nice to meet you") || callback.Message.Equals("Nice to meet you"))
            {
                steamfriends.SendChatMessage(callback.Sender, EChatEntryType.ChatMsg, "Nice to meet you too");
            }
        }
        static void OnFriendList(SteamFriends.FriendsListCallback callback)
        {
            Thread.Sleep(2500);

            foreach (var friend in callback.FriendList)
            {
                String sender = steamfriends.GetFriendPersonaName(friend.SteamID);
                if (friend.Relationship == EFriendRelationship.RequestRecipient)
                {
                    steamfriends.AddFriend(friend.SteamID);
                    Thread.Sleep(500);
                    Console.WriteLine(sender + " has been added to your friend list");
                    steamfriends.SendChatMessage(friend.SteamID, EChatEntryType.ChatMsg, "KovetsTech steam bot is not your friend type '!help' to get the list of commends");
                }
            }
        }
        public static string SetStatus(String steamID64)
        {
            string result = "user";
            String ownerID = "STEAM_0:1:81524689";

            if (steamID64.Equals(ownerID)) return "Owner";


            return result;
        }


        private void bot_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            contextMenuStrip1.Show(ptLowerLeft);
        }
        private void login(object sender, EventArgs e)
        {
            InputBoxItem[] items = new InputBoxItem[] {
            new InputBoxItem("Username"),
            new InputBoxItem("Password", true)
            };

            InputBox input = InputBox.Show("Login", items, InputBoxButtons.OKCancel);
            if (input.Result == InputBoxResult.OK)
            {
                username = input.Items["Username"];
                password = input.Items["Password"];
            }

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                SteamLogin();
            }).Start();
        }


        private void steam_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            contextMenuStrip2.Show(ptLowerLeft);
        }
        private void sendMessage(object sender, EventArgs e)
        {
            InputBoxItem[] items = new InputBoxItem[] {
            new InputBoxItem("User ID"),
            new InputBoxItem("Message")
            };

            InputBox input = InputBox.Show("Login", items, InputBoxButtons.OKCancel);
            if (input.Result == InputBoxResult.OK)
            {
                
            }
        }
    }
}

