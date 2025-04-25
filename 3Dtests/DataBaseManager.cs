using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls;
using Tic_Tac_Toe.Enumerators;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Tic_Tac_Toe
{
    class DataBaseManager: DrawableGameComponent
    {
        public bool online = true;//used to check if the database is online, if not, it will not try to connect to it
        private MySqlConnection connection;
        private string server;
        private string database;
        private string user;
        private string Connpassword;
        private string port;
        private string connectionString;
        private Game1 game;
        private TextBox[] textboxes;
        private Texture2D[] textures;
        private SpriteFont spriteFont;
        private List<MenuButton> _buttons;
        private MouseState dmouseState;
        SpriteBatch spriteBatch;
        public int UserID;
        public string Username, Password;//public to be got from main class for use in game
        private bool Working, initialised = false;//does not have an initialise class, this prevents errors
        public string[,] LeaderboardArray;//holds the leaderboard data
        Player player;


        public DataBaseManager(Game game, Texture2D[] Textures, SpriteFont Font, Player player): base(game)
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            server = "bbcftwomvem7ro4v46rq-mysql.services.clever-cloud.com";
            database = "bbcftwomvem7ro4v46rq";
            user = "uyks3ipislmzpecl";
            Connpassword = "qnlOzhYVtsbDtrR8N2gZ";
            port = "3306";
            Working = false;
            connectionString = string.Format("server={0};port={1};user id={2}; password={3}; database={4};", server, port, user, Connpassword, database);
            spriteFont = Font;
            LeaderboardArray = new string[25, 3];
            this.game = (Game1)game;
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                //potential for debugging string here
                System.Windows.Forms.MessageBox.Show("Database has failed", "sorgy");
                online = false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            if (!SignedIn())
            {
                textures = Textures;
                this.player = player;
                InitialiseSignIn();
            }
            else
            {
                this.game.UserID = UserID;
                player.UserID = UserID;
            }
        }


        public void IncreaseScore(Player player, int ScoreDuringRound)//called by other classes after game is won
        {
            if (online == false) { return; } //if the database is offline, do not try to connect to it
            if (player.UserID != 0)
            {
                string temp = ReadData("Score,Wins", $"UserID={player.UserID}");
                string[] temparray = temp.Split('£');
                int currentscore = int.Parse(temparray[1]);
                int currentwins = int.Parse(temparray[2]);
                updatedatabase(currentscore + ScoreDuringRound, currentwins + 1, player.UserID);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Only Player 1's score can be updated as player 2 is not logged in!", "Sorgy!");
            }
        }



        private bool SignedIn() //reads the file IF logged in previous
        {
            if (online == false) { return true; } //if the database is offline, do not try to connect to it
            try
            {
                BinaryReader reader = new BinaryReader(File.OpenRead("UserID.bin"));
                UserID = reader.ReadInt32();
                string username = ReadData("Username", $"UserID={UserID}");
                foreach (string str in username.Split('£'))
                {
                    if (!str.Contains("£") && !string.IsNullOrEmpty(str))
                    {
                        Username = str;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        private void InitialiseSignIn() //set up for sign in menu
        {
            if (online == false) { return; } //if the database is offline, do not try to connect to it
            game.Gamestate = GameStates.SignInMenu;
            _buttons = new List<MenuButton>();
            textboxes = new TextBox[2];
            _buttons.Add(textboxes[0] = new TextBox(new Vector2(675, 245), textures[0], game, 20, false, true, spriteFont, "Username: ", 10, 370, 1));
            _buttons.Add(textboxes[1] = new TextBox(new Vector2(675, 360), textures[0], game, 20, false, false, spriteFont, "Password: ", 10, 370, 2));
            _buttons.Add(new MenuButton(textures[1], new Vector2(640, 550), 3));
            _buttons.Add(new MenuButton(textures[2], new Vector2(320, 550), 4));
            initialised = true;
        }

        public void InsertData(int UserID, string Username, string Password, int score, int wins)//used by other classes, intends to be versatile
        {
            if (online == false) { return; } //if the database is offline, do not try to connect to it
            try
            {
                connection = new MySqlConnection(connectionString);
                string Query = "insert into TicTacToe(UserID,Username,Password,Score,Wins) values('" + UserID.ToString() + "','" + Username + "','" + Password + "','" + score.ToString() + "','" + wins.ToString() + "');"; //Command used
                MySqlCommand MyCommand2 = new MySqlCommand(Query, connection);
                MySqlDataReader MyReader2;
                connection.Open();
                MyReader2 = MyCommand2.ExecuteReader();//save data by executing query
                while (MyReader2.Read())
                {
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                //potential for debugging strings here
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null) { connection.Close(); }
            }
        }

        public string ReadData(string SelectedRows, string Where)//Used by other classes, intends to be versatile
        {
            if (online == false) { return ""; } //if the database is offline, do not try to connect to it
            string Data = "";
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                if (SelectedRows == "*") { SelectedRows = "UserID,Username,Password,Score,Wins"; }
                string query = $"SELECT {SelectedRows} FROM `TicTacToe` WHERE {Where};";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                string[] rowsselected = SelectedRows.Split(',');

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    foreach (string row in rowsselected)
                    {
                            Data += "£" + reader[row].ToString();
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Not Matching Data! Do you have an account?");
                }

            }
            catch (Exception ex)
            {
                //potential for debugging string here
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return Data;
        }

        

        private void updatedatabase(int Score, int Wins, int UserID)//updates the database with data, used by other methods
        {
            if (online == false) { return; } //if the database is offline, do not try to connect to it
            // "UPDATE Inventory SET Inventorynumber='"+ num +"',Inventory_Name='"+name+"', Quantity ='"+ quant+"',Location ='"+ location+"' Category ='"+ category+"' WHERE Inventorynumber ='"+ numquery +"';";
            string query = "UPDATE TicTacToe SET Score=@Score,Wins =@Wins WHERE UserID=@ID";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@Score", Score);
            cmd.Parameters.AddWithValue("@Wins", Wins);
            cmd.Parameters.AddWithValue("@ID", UserID);
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            connection.Close();

        }

        public override void Update(GameTime gameTime)//only run when in SignInMenu game state
        {
            if (online == false) { return; } //if the database is offline, do not try to connect to it
            if (initialised) {
                if (game.Gamestate == GameStates.SignInMenu)
                {
                    foreach (MenuButton button in _buttons)
                    {
                        button.Colour = Color.White;
                    }
                    var mouseState = Mouse.GetState(game.Window);
                    Rectangle MouseCollider = new Rectangle(mouseState.X, mouseState.Y, 10, 10);
                    MenuButton? ButtonHovering = UserInputManagment.HoveringOverButton(MouseCollider, _buttons);
                    if (ButtonHovering == null)
                    {
                        if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && dmouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                        {
                            foreach (var textBox in textboxes)
                            {
                                textBox.IsChecked = false;
                            }
                        }
                    }
                    else if (ButtonHovering == textboxes[0])//USRNAME TEXT BOX
                    {
                        if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && dmouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                        {
                            textboxes[0].IsChecked = true;
                            textboxes[1].IsChecked = false;
                        }
                    }
                    else if (ButtonHovering == textboxes[1])//PASSWORD TEXT BOX
                    {
                        if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && dmouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                        {
                            textboxes[1].IsChecked = true;
                            textboxes[0].IsChecked = false;
                        }
                    }
                    else if (ButtonHovering == _buttons[2]) { if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && dmouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released) { SignUp(); } }
                    else if (ButtonHovering == _buttons[3]) { if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && dmouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released) { Login(); } }
                    dmouseState = mouseState;
                }
                base.Update(gameTime);
            }
        } 

        private void Login()//uses other methods to login using given name and password, called by SignUp, always called if an account is used
        {
            if (online == false) { return; } //if the database is offline, do not try to connect to it
            if (!Working) {
                Working = true;
                string useridstr = ReadData("UserID", $"Username='{textboxes[0].heldText}' AND Password='{textboxes[1].heldText}'");
                if (useridstr != "")
                {
                    string[] UserIDSplit = useridstr.Split('£');
                    foreach (string split in UserIDSplit)
                    {
                        if (!split.Contains("£"))
                        {
                            useridstr = split;
                        }
                    }
                    
                    int UserID = int.Parse(useridstr);
                    BinaryWriter bwriter = new BinaryWriter(File.OpenWrite("UserID.bin"));
                    bwriter.Write(UserID);
                    bwriter.Close();
                    game.Gamestate = GameStates.Menu;
                    Username = textboxes[0].heldText;
                    Password = textboxes[1].heldText;
                    game.UserID = UserID;
                    player.UserID = UserID;
                }
                else
                {
                    Working = false;
                }
            }
        }

        private void SignUp()//creates a new account which is put on the database and then logs in
        {
            if (online == false) { return; } //if the database is offline, do not try to connect to it
            if (!Working)
            {
                if (!string.IsNullOrEmpty(textboxes[0].heldText))
                {
                    Working = true;
                    string useridstr = ReadData("MAX(UserID)", "UserID>0");
                    int temp = 1;
                    if (useridstr != "")
                    {
                        string[] UserIDSplit = useridstr.Split('£');
                        foreach (string split in UserIDSplit)
                        {
                            if (!split.Contains("£") && !string.IsNullOrEmpty(split))
                            {
                                if (temp < int.Parse(split))
                                {
                                    temp = int.Parse(split);
                                }
                            }
                        }
                        InsertData(temp + 1, textboxes[0].heldText, textboxes[1].heldText, 0, 0);
                    }
                    Working = false;
                    Login();
                }
                else
                {
                    Working = false;
                }
            }
        }


        public void RefreshLeaderboard()//refreshes the leaderboard on the menuscreen
        {
            if (online == false) { return; } //if the database is offline, do not try to connect to it
            string Data = "";
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                string query = $"SELECT Username, Score, Wins FROM `TicTacToe` ORDER BY Score DESC LIMIT 25;";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) //0.6 scale
                {
                    Data += reader["Username"].ToString() + "|";
                    Data += reader["Score"].ToString() + "|";
                    Data += reader["Wins"].ToString();
                    Data += "%%%";
                }
                string[] temp = Data.Split("%%%");
                for (int i = 0; i < temp.Length; i++)
                {
                    string[] LesserTemp = temp[i].Split('|');
                    for (int j = 0; j < LesserTemp.Length; j++)
                    {
                        LeaderboardArray[i, j] = LesserTemp[j];
                    }
                }
            }
            catch (Exception ex)
            {
                //potential for debugging string here
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }



        public override void Draw(GameTime gameTime)//only runs during SignInMenu
        {
            if (online == false) { return; } //if the database is offline, do not try to connect to it
            if (initialised)
            {
                if (game.Gamestate == GameStates.SignInMenu)
                {
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    spriteBatch.Begin();
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    foreach (MenuButton button in _buttons)
                    {
                        button.Draw(spriteBatch);
                    }
                    spriteBatch.End();
                    base.Draw(gameTime);
                }
            }
        }

    }
}
