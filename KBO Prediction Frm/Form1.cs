using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace KBO_Prediction_Frm
{
    public partial class Form1 : Form
    {
        /* SQL 접속 정보 / 계정정보 삭제
         */
        String connectionString = "";
        
        /* 게임 데이터 구조체에 저장 → 불러와서 쉽게 사용할 수 있게 */
        public struct GameData
        {
            public string GameDate
            {
                get { return GameDate; }
                set { GameDate = value; }
            }

            //home_team 정보
            public string GameHomeTeam
            {
                get { return GameHomeTeam; }
                set { GameHomeTeam = value; }
            }

            public string GameHomeResult
            {
                get { return GameHomeResult; }
                set { GameHomeResult = value; }
            }

            public int GameHomeR
            {
                get { return GameHomeR; }
                set { GameHomeR = value; }
            }

            public int GameHomeB
            {
                get { return GameHomeB; }
                set { GameHomeB = value; }
            }

            public int GameHomeE
            {
                get { return GameHomeE; }
                set { GameHomeE = value; }
            }

            public int GameHomeH
            {
                get { return GameHomeH; }
                set { GameHomeH = value; }
            }

            public int GameHomeSeasonDraws
            {
                get { return GameHomeSeasonDraws; }
                set { GameHomeSeasonDraws = value; }
            }

            public double GameHomeSeasonHra
            {
                get { return GameHomeSeasonHra; }
                set { GameHomeSeasonHra = value; }
            }

            public double GameHomeSeasonWra
            {
                get { return GameHomeSeasonWra; }
                set { GameHomeSeasonWra = value; }
            }

            public int GameHomeSeasonWins
            {
                get { return GameHomeSeasonWins; }
                set { GameHomeSeasonWins = value; }
            }

            public int GameHomeSeasonLoses
            {
                get { return GameHomeSeasonLoses; }
                set { GameHomeSeasonLoses = value; }
            }

            public int GameHomeSeasonRank
            {
                get { return GameHomeSeasonRank; }
                set { GameHomeSeasonRank = value; }
            }

            //away_team 정보
            public string GameAwayTeam
            {
                get { return GameAwayTeam; }
                set { GameAwayTeam = value; }
            }

            public string GameAwayResult
            {
                get { return GameAwayResult; }
                set { GameAwayResult = value; }
            }

            public int GameAwayR
            {
                get { return GameAwayR; }
                set { GameAwayR = value; }
            }

            public int GameAwayB
            {
                get { return GameAwayB; }
                set { GameAwayB = value; }
            }

            public int GameAwayE
            {
                get { return GameAwayE; }
                set { GameAwayE = value; }
            }

            public int GameAwayH
            {
                get { return GameAwayH; }
                set { GameAwayH = value; }
            }

            public int GameAwaySeasonDraws
            {
                get { return GameAwaySeasonDraws; }
                set { GameAwaySeasonDraws = value; }
            }

            public double GameAwaySeasonHra
            {
                get { return GameAwaySeasonHra; }
                set { GameAwaySeasonHra = value; }
            }

            public double GameAwaySeasonWra
            {
                get { return GameAwaySeasonWra; }
                set { GameAwaySeasonWra = value; }
            }

            public int GameAwaySeasonWins
            {
                get { return GameAwaySeasonWins; }
                set { GameAwaySeasonWins = value; }
            }

            public int GameAwaySeasonLoses
            {
                get { return GameAwaySeasonLoses; }
                set { GameAwaySeasonLoses = value; }
            }

            public int GameAwaySeasonRank
            {
                get { return GameAwaySeasonRank; }
                set { GameAwaySeasonRank = value; }
            }
        }

        /* 
         * List<T>를 구조체로 사용
         * 단, List<T>로 구조체를 사용할 경우, 구조체에 직접 접근 해 값을 수정할 수 없음
         * 따라서 자체 함수를 만들어 swapping 방식으로 값을 넘기는 방식으로 구현 (void ModifyGameData(...))
         */

        GameData lvGameData = new GameData();
        List<GameData> ListGameData = new List<GameData>();

        /*
         * List 형의 구조체 내의 값을 변경해주는 함수이다.
         * Split 함수를 통해 value의 값을 쪼개서 값을 넣어준다. (구분은 ,)
         */

        int game_count = 0;

        void ModifyGameData(List<GameData> inList, int index, string value)
        {

        }

        public Form1()
        {
            /* 컴포넌트 초기화 */
            InitializeComponent();
        }

        /* 폼 로드 */
        private void Form1_Load(object sender, EventArgs e)
        {
            InitData();
            InitNewGame();

        }

        private void InitNewGame()
        {
            string[] lines = File.ReadAllLines(@"..\..\result.txt", Encoding.Default);
            string[] vec = null;
            string[] soon_data = new string[10];

            int temp = 0;

            foreach (string show in lines)
            {
                game_count++;

                vec = show.Split(',');
                foreach (string word in vec)
                {
                    soon_data[temp] = word;
                    temp++;
                }

                ListViewItem lvt = new ListViewItem();

                /*
                 * 날짜,홈팀,홈선발,어웨이팀,어웨이선발,예측팀
                 * */

                lvt.SubItems.Add(game_count.ToString());
                lvt.SubItems.Add(soon_data[0]);
                lvt.SubItems.Add(soon_data[1]);
                lvt.SubItems.Add("vs");
                lvt.SubItems.Add(soon_data[3]);

                lvt.SubItems.Add("경기 전");

                lvt.SubItems.Add(soon_data[5]);
                //lvt.SubItems.Add("경기 전");
                lvt.SubItems.Add("");

                lvTable.Items.Add(lvt);

                temp = 0;
            }
        }

        /* 데이터를 불러오는 함수, SQL에 접근함 */
        private void InitData()
        {
            //데이터베이스에 접속
            var conn = new MySqlConnection(connectionString);
            conn.Open();

            //게임 로그 데이터를 가져옴
            string sql = "SELECT * FROM game_log";

            //내용 초기화
            lvTable.Items.Clear();

            //ExecuteReader 이용해 연결 모드로 데이터 가져오기
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                game_count++;

                ListViewItem lvt = new ListViewItem();

                lvt.SubItems.Add(game_count.ToString());
                lvt.SubItems.Add(rdr["date"].ToString());
                lvt.SubItems.Add(rdr["home_team"].ToString());
                lvt.SubItems.Add("vs");
                lvt.SubItems.Add(rdr["away_team"].ToString());

                if(rdr["home_result"].ToString() == "WIN")
                    lvt.SubItems.Add(rdr["home_team"].ToString());
                else
                    lvt.SubItems.Add(rdr["away_team"].ToString());

                lvt.SubItems.Add(rdr["predict_win_team"].ToString());
                lvt.SubItems.Add(rdr["predict_memo"].ToString());

                lvTable.Items.Add(lvt);
            }

            lvTable.EnsureVisible(lvTable.Items.Count - 1);

            rdr.Close();
            conn.Close();
        }

        /* 예측 */
        private void button1_Click(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines(@"..\..\result.txt", Encoding.Default);
            string[] vec = null;
            string[] soon_data = new string[10];

            int temp = 0;

            ListView.SelectedListViewItemCollection items = lvTable.SelectedItems;
            ListViewItem lvItem = items[0];
            string date2 = lvItem.SubItems[2].Text;
            string home_team2 = lvItem.SubItems[3].Text;
            string away_team2 = lvItem.SubItems[5].Text;
            
            foreach (string show in lines)
            {
                game_count++;

                vec = show.Split(',');
                foreach (string word in vec)
                {
                    soon_data[temp] = word;
                    temp++;
                }

                lvItem.SubItems[5].Text = "ㅇㅇㅇ";

                temp = 0;
            }
        }

        /* 팀의 마크 및 정보 가져오기 */
        private void GetTeamData(string date, string ht, string at, string pr, int index)
        {
            string predict_win_team = "";
            string real_win_team = "";
            string temp1 = "", temp2 = "", temp3 = "";
            double home_war = 0, away_war = 0;
            
            //스코어 및 정보 초기화
            home_score.ForeColor = Color.Black;
            away_score.ForeColor = Color.Black;
            home_data.ForeColor = Color.Black;
            away_data.ForeColor = Color.Black;
            home_listView.Items.Clear();
            away_listView.Items.Clear();
            realwin_txt.Image = txt_default.Image;
            predwin_txt.Image = txt_default.Image;

            home_score.Text = "0";
            away_score.Text = "0";

            //팀 마크 및 정보부터 가져오기
            //텍스트는 백그라운드, 마크는 이미지임
            if (ht == "kia" || ht == "KIA")
            {
                home_logo.Image = img_ht.Image;
                home_txt.BackgroundImage = txt_ht.BackgroundImage;
            }
            else if (ht == "두산")
            {
                home_logo.Image = img_ds.Image;
                home_txt.BackgroundImage = txt_ds.BackgroundImage;
            }
            else if (ht == "한화")
            {
                home_logo.Image = img_hh.Image;
                home_txt.BackgroundImage = txt_hh.BackgroundImage;
            }
            else if (ht == "SK" || ht == "sk")
            {
                home_logo.Image = img_sk.Image;
                home_txt.BackgroundImage = txt_sk.BackgroundImage;
            }
            else if (ht == "LG" || ht == "lg")
            {
                home_logo.Image = img_lg.Image;
                home_txt.BackgroundImage = txt_lg.BackgroundImage;
            }
            else if (ht == "넥센")
            {
                home_logo.Image = img_nx.Image;
                home_txt.BackgroundImage = txt_nx.BackgroundImage;
            }
            else if (ht == "삼성")
            {
                home_logo.Image = img_ss.Image;
                home_txt.BackgroundImage = txt_ss.BackgroundImage;
            }
            else if (ht == "롯데")
            {
                home_logo.Image = img_lt.Image;
                home_txt.BackgroundImage = txt_lt.BackgroundImage;
            }
            else if (ht == "KT" || ht == "kt")
            {
                home_logo.Image = img_kt.Image;
                home_txt.BackgroundImage = txt_kt.BackgroundImage;
            }
            else if (ht == "KT" || ht == "kt")
            {
                home_logo.Image = img_kt.Image;
                home_txt.BackgroundImage = txt_kt.BackgroundImage;
            }
            else if (ht == "NC" || ht == "nc")
            {
                home_logo.Image = img_nc.Image;
                home_txt.BackgroundImage = txt_nc.BackgroundImage;
            }
            if (at == "kia" || at == "KIA")
            {
                away_logo.Image = img_ht.Image;
                away_txt.BackgroundImage = txt_ht.BackgroundImage;
            }
            else if (at == "두산")
            {
                away_logo.Image = img_ds.Image;
                away_txt.BackgroundImage = txt_ds.BackgroundImage;
            }
            else if (at == "한화")
            {
                away_logo.Image = img_hh.Image;
                away_txt.BackgroundImage = txt_hh.BackgroundImage;
            }
            else if (at == "SK" || at == "sk")
            {
                away_logo.Image = img_sk.Image;
                away_txt.BackgroundImage = txt_sk.BackgroundImage;
            }
            else if (at == "LG" || at == "lg")
            {
                away_logo.Image = img_lg.Image;
                away_txt.BackgroundImage = txt_lg.BackgroundImage;
            }
            else if (at == "넥센")
            {
                away_logo.Image = img_nx.Image;
                away_txt.BackgroundImage = txt_nx.BackgroundImage;
            }
            else if (at == "삼성")
            {
                away_logo.Image = img_ss.Image;
                away_txt.BackgroundImage = txt_ss.BackgroundImage;
            }
            else if (at == "롯데")
            {
                away_logo.Image = img_lt.Image;
                away_txt.BackgroundImage = txt_lt.BackgroundImage;
            }
            else if (at == "KT" || at == "kt")
            {
                away_logo.Image = img_kt.Image;
                away_txt.BackgroundImage = txt_kt.BackgroundImage;
            }
            else if (at == "KT" || at == "kt")
            {
                away_logo.Image = img_kt.Image;
                away_txt.BackgroundImage = txt_kt.BackgroundImage;
            }
            else if (at == "NC" || at == "nc")
            {
                away_logo.Image = img_nc.Image;
                away_txt.BackgroundImage = txt_nc.BackgroundImage;
            }

            //데이터베이스에 접속
            var conn = new MySqlConnection(connectionString);
            conn.Open();

            //일단 각 팀의 정보를 가져옴 (그 날짜 기준)
            string sql = "SELECT * FROM game_log WHERE date = '" + date + "' and home_team = '" + ht +"'";

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();

            //경기전이면 약간의 조정이 필요 (데이터 가져올 때)
            if (pr == "경기 전")
            {
                rdr.Close();

                //각 팀 선발 구해오기
                string[] lines = File.ReadAllLines(@"..\..\result.txt", Encoding.Default);
                string[] vec = null;
                string[] soon_data = new string[10];

                int temp = 0;

                foreach (string show in lines)
                {
                    game_count++;

                    vec = show.Split(',');
                    foreach (string word in vec)
                    {
                        soon_data[temp] = word;
                        temp++;
                    }

                    if (soon_data[1] == ht)
                    {
                        ListViewItem lvt_ht = new ListViewItem();
                        lvt_ht.SubItems.Add(soon_data[2].ToString());
                        lvt_ht.SubItems.Add("투수");
                        lvt_ht.SubItems.Add("0");
                        home_listView.Items.Add(lvt_ht);

                        ListViewItem lvt_at = new ListViewItem();
                        lvt_at.SubItems.Add(soon_data[4].ToString());
                        lvt_at.SubItems.Add("투수");
                        lvt_at.SubItems.Add("0");
                        away_listView.Items.Add(lvt_at);
                        break;
                    }

                    temp = 0;
                }

                //전적 및 승률 불러오기
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        for (int i = index - 2; i > 0; i--)
                        {
                            if (lvTable.SelectedItems.Count == 1)
                            {
                                ListViewItem lvItem = lvTable.Items[i];
                                string date2 = lvItem.SubItems[2].Text;
                                string home_team2 = lvItem.SubItems[3].Text;
                                string away_team2 = lvItem.SubItems[5].Text;
                                
                                if (home_team2 == ht)
                                {
                                    sql = "SELECT * FROM game_log WHERE date = '" + date2.ToString() + "' and home_team = '" + ht + "'";

                                    cmd = new MySqlCommand(sql, conn);
                                    rdr = cmd.ExecuteReader();

                                    while (rdr.Read())
                                    {
                                        home_record.Text = rdr["home_season_wins"].ToString() + "승 " + rdr["home_season_draws"].ToString() + "무 " + rdr["home_season_loses"].ToString() + "패 (" + rdr["home_season_rank"].ToString() + "위)";
                                    }
                                    
                                    rdr.Close();
                                    break;
                                }
                                else if(away_team2 == ht)
                                {
                                    sql = "SELECT * FROM game_log WHERE date = '" + date2.ToString() + "' and away_team = '" + ht + "'";

                                    cmd = new MySqlCommand(sql, conn);
                                    rdr = cmd.ExecuteReader();

                                    while (rdr.Read())
                                    {
                                        home_record.Text = rdr["away_season_wins"].ToString() + "승 " + rdr["away_season_draws"].ToString() + "무 " + rdr["away_season_loses"].ToString() + "패 (" + rdr["away_season_rank"].ToString() + "위)";
                                    }

                                    rdr.Close();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = index - 2; i > 0; i--)
                        {
                            if (lvTable.SelectedItems.Count == 1)
                            {
                                ListViewItem lvItem = lvTable.Items[i];
                                string date2 = lvItem.SubItems[2].Text;
                                string home_team2 = lvItem.SubItems[3].Text;
                                string away_team2 = lvItem.SubItems[5].Text;
                                
                                if (home_team2 == at)
                                {
                                    sql = "SELECT * FROM game_log WHERE date = '" + date2.ToString() + "' and home_team = '" + at + "'";

                                    cmd = new MySqlCommand(sql, conn);
                                    rdr = cmd.ExecuteReader();

                                    while (rdr.Read())
                                    {
                                        away_record.Text = rdr["home_season_wins"].ToString() + "승 " + rdr["home_season_draws"].ToString() + "무 " + rdr["home_season_loses"].ToString() + "패 (" + rdr["home_season_rank"].ToString() + "위)";
                                    }

                                    rdr.Close();
                                    break;
                                }
                                else if (away_team2 == at)
                                {
                                    sql = "SELECT * FROM game_log WHERE date = '" + date2.ToString() + "' and away_team = '" + at + "'";

                                    cmd = new MySqlCommand(sql, conn);
                                    rdr = cmd.ExecuteReader();

                                    while (rdr.Read())
                                    {
                                        away_record.Text = rdr["away_season_wins"].ToString() + "승 " + rdr["away_season_draws"].ToString() + "무 " + rdr["away_season_loses"].ToString() + "패 (" + rdr["away_season_rank"].ToString() + "위)";
                                    }

                                    rdr.Close();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //ExecuteReader 이용해 연결 모드로 데이터 가져오기
                while (rdr.Read())
                {
                    home_record.Text = rdr["home_season_wins"].ToString() + "승 " + rdr["home_season_draws"].ToString() + "무 " + rdr["home_season_loses"].ToString() + "패 (" + rdr["home_season_rank"].ToString() + "위)";
                    away_record.Text = rdr["away_season_wins"].ToString() + "승 " + rdr["away_season_draws"].ToString() + "무 " + rdr["away_season_loses"].ToString() + "패 (" + rdr["away_season_rank"].ToString() + "위)";

                    home_score.Text = rdr["home_r"].ToString();
                    away_score.Text = rdr["away_r"].ToString();

                    home_war = Convert.ToDouble(rdr["predict_win_per"]);
                    away_war = Convert.ToDouble(rdr["predict_lose_per"]);

                    if (rdr["home_result"].ToString() == "WIN")
                    {
                        real_win_team = rdr["home_team"].ToString();
                        home_score.ForeColor = Color.Red;
                        away_score.ForeColor = Color.Black;
                    }
                    else if (rdr["home_result"].ToString() == "LOSE")
                    {
                        real_win_team = rdr["away_team"].ToString();
                        home_score.ForeColor = Color.Black;
                        away_score.ForeColor = Color.Red;
                    }
                    else
                    {
                        real_win_team = "";
                        home_score.ForeColor = Color.Green;
                        away_score.ForeColor = Color.Green;
                    }
                }

                rdr.Close();
            }

            /* 각 팀의 선수 데이터를 가져와 비교
             * 
             * 수정이 필요하다면 이 부분을 수정하면 될 것으로 보임
             * 
             */

            //일단 각 팀의 정보를 가져옴 (그 날짜 기준)
            sql = "SELECT name FROM batter_log WHERE date = '" + date + "' and my_team = '" + ht + "'";
            cmd = new MySqlCommand(sql, conn);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                ListViewItem lvt = new ListViewItem();

                lvt.SubItems.Add(rdr["name"].ToString());
                lvt.SubItems.Add("타자");
                lvt.SubItems.Add("0");

                home_listView.Items.Add(lvt);
            }
            rdr.Close();

            sql = "SELECT name FROM pitcher_log WHERE date = '" + date + "' and my_team = '" + ht + "'";
            cmd = new MySqlCommand(sql, conn);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                ListViewItem lvt = new ListViewItem();

                lvt.SubItems.Add(rdr["name"].ToString());
                lvt.SubItems.Add("투수");
                lvt.SubItems.Add("0");

                home_listView.Items.Add(lvt);
            }
            rdr.Close();

            sql = "SELECT name FROM batter_log WHERE date = '" + date + "' and my_team = '" + at + "'";
            cmd = new MySqlCommand(sql, conn);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                ListViewItem lvt = new ListViewItem();

                lvt.SubItems.Add(rdr["name"].ToString());
                lvt.SubItems.Add("타자");
                lvt.SubItems.Add("0");

                away_listView.Items.Add(lvt);
            }
            rdr.Close();
            
            sql = "SELECT name FROM pitcher_log WHERE date = '" + date + "' and my_team = '" + at + "'";
            cmd = new MySqlCommand(sql, conn);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                ListViewItem lvt = new ListViewItem();

                lvt.SubItems.Add(rdr["name"].ToString());
                lvt.SubItems.Add("투수");
                lvt.SubItems.Add("0");

                away_listView.Items.Add(lvt);
            }
            rdr.Close();

            //홈 팀의 리스트뷰(선수)를 읽어 WAR 구하기
            //타자 WAR 가져오기 (선수스탯 테이블에서 war 가져옴)
            
            for (int i = 0; i < home_listView.Items.Count; i++)
            {
                temp1 = home_listView.Items[i].SubItems[1].Text;
                temp2 = home_listView.Items[i].SubItems[2].Text;

                if (temp2 == "타자")
                    temp3 = "batter_stat";
                else
                    temp3 = "pitcher_stat";
                
                sql = "SELECT name, war FROM " + temp3 + " WHERE year = '" + (Int32.Parse(date.Substring(0, 4))).ToString() + "' and name = '" + temp1 + "'";
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    home_listView.Items[i].SubItems[3].Text = rdr["war"].ToString();
                }
                rdr.Close();
            }

            Thread.Sleep(10);

            //어웨이팀 WAR 구하기
            for (int i = 0; i < away_listView.Items.Count; i++)
            {
                temp1 = away_listView.Items[i].SubItems[1].Text;
                temp2 = away_listView.Items[i].SubItems[2].Text;

                if (temp2 == "타자")
                    temp3 = "batter_stat";
                else
                    temp3 = "pitcher_stat";

                sql = "SELECT war FROM " + temp3 + " WHERE year = '" + (Int32.Parse(date.Substring(0, 4))).ToString() + "' and name = '" + temp1 + "'";
                
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    away_listView.Items[i].SubItems[3].Text = rdr["war"].ToString();
                }
                rdr.Close();
            }

            //Thread.Sleep(10);

            //홈 war과 어웨이 war을 비교해 예측 팀을 띄우기

            //경기전이면 약간의 조정이 필요 (데이터 가져올 때)
            if (pr == "경기 전")
            {
                string[] lines = File.ReadAllLines(@"..\..\result.txt", Encoding.Default);
                string[] vec = null;
                string[] soon_data = new string[10];

                int temp = 0;

                foreach (string show in lines)
                {
                    game_count++;

                    vec = show.Split(',');
                    foreach (string word in vec)
                    {
                        soon_data[temp] = word;
                        temp++;
                    }

                    if (soon_data[1] == ht)
                    {
                        home_war = Convert.ToDouble(soon_data[6]) * 100;
                        away_war = Convert.ToDouble(soon_data[8]) * 100;

                        realwin_txt.BackgroundImage = txt_ds.BackgroundImage;
                        break;
                    }

                    temp = 0;
                }

                home_data.Text = home_war.ToString() + "%";
                away_data.Text = away_war.ToString() + "%";
                realwin_txt.BackgroundImage = txt_null.BackgroundImage;
            }
            else
            {
                home_war = Convert.ToDouble(home_war) * 100;
                away_war = Convert.ToDouble(away_war) * 100;

                //보정
                if(Math.Abs(home_war - away_war) >= 40)
                {
                    double balance1 = home_war;
                    double balance2 = away_war;

                    if (home_war > away_war)
                    {
                        home_war -= (Math.Abs(balance1 - balance2) /3);
                        away_war += (Math.Abs(balance1 - balance2) /3);
                    }
                    else
                    {
                        home_war += (Math.Abs(balance1 - balance2) / 3);
                        away_war -= (Math.Abs(balance1 - balance2) / 3);
                    }
                }

                home_data.Text = home_war.ToString("#.##") + "%";
                away_data.Text = away_war.ToString("#.##") + "%";
            }

            //마크 띄어주기
            if (home_war > away_war)
            {
                predict_win_team = ht;
                home_data.ForeColor = Color.Red;
                away_data.ForeColor = Color.Black;

                //predict_db_upload(date, ht, at, ht, home_war, at, away_war, "");
            }
            else if(away_war > home_war)
            {
                predict_win_team = at;
                home_data.ForeColor = Color.Black;
                away_data.ForeColor = Color.Red;

                //predict_db_upload(date, ht, at, at, away_war, ht, home_war, "");
            }
            else
            {
                home_data.ForeColor = Color.Green;
                away_data.ForeColor = Color.Green;

                //predict_db_upload(date, ht, at, "", 0, "", 0, "무승부 - WAR 두 팀 모두 다 " + home_war);
            }
            
            conn.Close();

            /* 승리팀 마크 띄어주기 */
            if (real_win_team == "kia" || real_win_team == "KIA")
            {
                realwin_txt.BackgroundImage = txt_ht.BackgroundImage;
            }
            else if (real_win_team == "두산")
            {
                realwin_txt.BackgroundImage = txt_ds.BackgroundImage;
            }
            else if (real_win_team == "한화")
            {
                realwin_txt.BackgroundImage = txt_hh.BackgroundImage;
            }
            else if (real_win_team == "SK" || real_win_team == "sk")
            {
                realwin_txt.BackgroundImage = txt_sk.BackgroundImage;
            }
            else if (real_win_team == "LG" || real_win_team == "lg")
            {
                realwin_txt.BackgroundImage = txt_lg.BackgroundImage;
            }
            else if (real_win_team == "넥센")
            {
                realwin_txt.BackgroundImage = txt_nx.BackgroundImage;
            }
            else if (real_win_team == "삼성")
            {
                realwin_txt.BackgroundImage = txt_ss.BackgroundImage;
            }
            else if (real_win_team == "롯데")
            {
                realwin_txt.BackgroundImage = txt_lt.BackgroundImage;
            }
            else if (real_win_team == "KT" || real_win_team == "kt")
            {
                realwin_txt.BackgroundImage = txt_kt.BackgroundImage;
            }
            else if (real_win_team == "KT" || real_win_team == "kt")
            {
                realwin_txt.BackgroundImage = txt_kt.BackgroundImage;
            }
            else if (real_win_team == "NC" || real_win_team == "nc")
            {
                realwin_txt.BackgroundImage = txt_nc.BackgroundImage;
            }

            if (predict_win_team == "kia" || predict_win_team == "KIA")
            {
                predwin_txt.BackgroundImage = txt_ht.BackgroundImage;
            }
            else if (predict_win_team == "두산")
            {
                predwin_txt.BackgroundImage = txt_ds.BackgroundImage;
            }
            else if (predict_win_team == "한화")
            {
                predwin_txt.BackgroundImage = txt_hh.BackgroundImage;
            }
            else if (predict_win_team == "SK" || predict_win_team == "sk")
            {
                predwin_txt.BackgroundImage = txt_sk.BackgroundImage;
            }
            else if (predict_win_team == "LG" || predict_win_team == "lg")
            {
                predwin_txt.BackgroundImage = txt_lg.BackgroundImage;
            }
            else if (predict_win_team == "넥센")
            {
                predwin_txt.BackgroundImage = txt_nx.BackgroundImage;
            }
            else if (predict_win_team == "삼성")
            {
                predwin_txt.BackgroundImage = txt_ss.BackgroundImage;
            }
            else if (predict_win_team == "롯데")
            {
                predwin_txt.BackgroundImage = txt_lt.BackgroundImage;
            }
            else if (predict_win_team == "KT" || predict_win_team == "kt")
            {
                predwin_txt.BackgroundImage = txt_kt.BackgroundImage;
            }
            else if (predict_win_team == "KT" || predict_win_team == "kt")
            {
                predwin_txt.BackgroundImage = txt_kt.BackgroundImage;
            }
            else if (predict_win_team == "NC" || predict_win_team == "nc")
            {
                predwin_txt.BackgroundImage = txt_nc.BackgroundImage;
            }



            return;
        }

        /* 예측 결과 DB 재업로드 
           안씀 (개발 테스트용)
             */
        private void predict_db_upload(string date, string ht, string at, string winteam, double winscore, string loseteam, double losescore, string memo)
        {
            //데이터베이스에 접속
            var conn = new MySqlConnection(connectionString);
            conn.Open();

            //일단 각 팀의 정보를 가져옴 (그 날짜 기준)
            string sql = "UPDATE game_log SET predict_win_team = '" + winteam + "', predict_lose_team = '" + loseteam + "', predict_win_wra = '" + winscore + "', predict_lose_wra = '" + losescore + "', predict_memo = '" + memo + "' "
                        + "WHERE date = '" + date + "' and home_team = '" + ht + "' and away_team = '" + at + "'";
            
            textBox1.Text = sql;

            //ExecuteReader 이용해 연결 모드로 데이터 가져오기
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();

            conn.Close();

            //InitData();
            return;
        }

        /* 공통 이벤트 */
        private void lvTable_Common_event()
        {
            //해당 데이터의 값을 읽어옴 (그 날 선수들의 데이터를 읽기 위해)
            if (lvTable.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = lvTable.SelectedItems;
                ListViewItem lvItem = items[0];
                string date = lvItem.SubItems[2].Text;
                string home_team = lvItem.SubItems[3].Text;
                string away_team = lvItem.SubItems[5].Text;

                GetTeamData(date, home_team, away_team, lvItem.SubItems[6].Text, Convert.ToInt32(lvItem.SubItems[1].Text));
            }
        }

        /* lvTable을 클릭했을 때 SQL에 접속해 해당 데이터를 싸그리 가져옴 */
        private void lvTable_Click(object sender, EventArgs e)
        {
            lvTable_Common_event();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void lvTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvTable_Common_event();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Load_predict_python();
        }

        private void Load_predict_python()
        {
            Process cmd = new Process();

            string path = "cd C:/Users/lab401/Desktop/졸업작품/KBO Prediction Frm (인터페이스)/KBO Prediction Frm";

            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(path);
            cmd.StandardInput.WriteLine("start get_data.py");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();

            Form2 dlg = new Form2();
            dlg.ShowDialog();

            dlg.Close();
        }
    }
}
