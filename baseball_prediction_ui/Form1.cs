using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace baseball_prediction_ui
{
    public partial class Form1 : Form
    {
        /* 파싱 해야 하는데 두 클래스 이용해 사용 */
        HttpWebRequest wReq;
        HttpWebResponse wRes;

        string ViewDate;
        string DateYY, DateMM, DateDD;
        int BattlelistClick;

        public Form1()
        {
            InitializeComponent();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //progressBar1.Value = 69;
            //progressBar1.Value = 31;


            /* MonthCalender MaxSelectionCount 지정 (멀티 클릭 불가능하게) */
            monthCalendar1.MaxSelectionCount = 1;
            label_battle_list_reset();
        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        private void btn_viewday_Click(object sender, EventArgs e)
        {
            if (ViewDate == null)
            {
                MessageBox.Show("날짜를 선택해주세요.");
                return;
            }

            /* split으로 나누어 각각의 변수에 저장 */
            DateYY = ViewDate.Split('-')[0];
            DateMM = ViewDate.Split('-')[1];
            DateDD = ViewDate.Split('-')[2];

            /* deligate로 쓰레드 하나 만들어서 읽어오게 하기
             * (왜냐면 읽어오는 동안 프로그램에 렉 발생하면 보기 짜증남)
             * 대신 직접 쓰레드에서 변수선언하면 개박살나니까 deligate로
             */

            if(get_battle_information(DateYY, DateMM, DateDD) == false)
            {
                MessageBox.Show("정보 읽기에 실패했습니다.");
                return;
            }

            MessageBox.Show("경기 목록에서 경기를 선택한 후, 경기예측 버튼을 누르면 경기가 예측됩니다.");
        }

        /* 경기 불러오는 함수 및 버튼 이벤트 (radiobutton) */

        private void get_data_battle(int line)
        {
            /* 버튼의 enabled를 설정한 후, 관련 값을 설정, 경기는 총 다섯경기 */

            if (line != 1)
            {
                radioButton1.Checked = false;
                bi_group_1.BackColor = Color.White;
            }

            if (line != 2)
            {
                radioButton2.Checked = false;
                bi_group_2.BackColor = Color.White;
            }

            if (line != 3)
            {
                radioButton3.Checked = false;
                bi_group_3.BackColor = Color.White;
            }

            if (line != 4)
            {
                radioButton4.Checked = false;
                bi_group_4.BackColor = Color.White;
            }

            if (line != 5)
            {
                radioButton5.Checked = false;
                bi_group_5.BackColor = Color.White;
            }

            switch (line)
            {
                case 1:
                    radioButton1.Checked = true;
                    bi_group_1.BackColor = Color.Yellow;
                    break;
                case 2:
                    radioButton2.Checked = true;
                    bi_group_2.BackColor = Color.Yellow;
                    break;
                case 3:
                    radioButton3.Checked = true;
                    bi_group_3.BackColor = Color.Yellow;
                    break;
                case 4:
                    radioButton4.Checked = true;
                    bi_group_4.BackColor = Color.Yellow;
                    break;
                case 5:
                    radioButton5.Checked = true;
                    bi_group_5.BackColor = Color.Yellow;
                    break;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
                if (radioButton1.Checked == true)
                {
                    get_data_battle(1);
                }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
                if (radioButton2.Checked == true)
                {
                    get_data_battle(2);
                }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
                if (radioButton3.Checked == true)
                {
                    get_data_battle(3);
                }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
                if (radioButton4.Checked == true)
                {
                    get_data_battle(4);
                }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
                if (radioButton5.Checked == true)
                {
                    get_data_battle(5);
                }
        }

        private bool get_battle_information(string yy, string mm, string dd)
        {
            try
            {
                /* 해당 년도, 월의 링크를 우선 들어간 후 처리 */
                //string URL = "https://sports.news.naver.com/kbaseball/schedule/index.nhn?date=&month=" + mm + "&year=" + yy + "&teamCode=";
                string URL = "http://eng.koreabaseball.com/Schedule/DailySchedule.aspx";
                MessageBox.Show(URL);

                WebRequest request = WebRequest.Create(URL);

                WebResponse response = request.GetResponse();
                StreamReader stream = new StreamReader(response.GetResponseStream());

                string firstStr = stream.ReadToEnd();

                debug_txtBox.Text = firstStr;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            ViewDate = monthCalendar1.SelectionStart.ToShortDateString();
            lbl_date.Text = ViewDate + " 를 보려면";
        }

        /* 라벨 초기화 함수 */            
        private void label_battle_list_reset()
        {
            lb_ha_1.Text = "없음";
            lb_hb_1.Text = "0위 0승 0무 0패";
            lb_hc_1.Text = "0연승";

            lb_ha_2.Text = "없음";
            lb_hb_2.Text = "0위 0승 0무 0패";
            lb_hc_2.Text = "0연승";

            lb_ha_3.Text = "없음";
            lb_hb_3.Text = "0위 0승 0무 0패";
            lb_hc_3.Text = "0연승";

            lb_ha_4.Text = "없음";
            lb_hb_4.Text = "0위 0승 0무 0패";
            lb_hc_4.Text = "0연승";

            lb_ha_5.Text = "없음";
            lb_hb_5.Text = "0위 0승 0무 0패";
            lb_hc_5.Text = "0연승";

            lb_aa_1.Text = "없음";
            lb_ab_1.Text = "0위 0승 0무 0패";
            lb_ac_1.Text = "0연승";

            lb_aa_2.Text = "없음";
            lb_ab_2.Text = "0위 0승 0무 0패";
            lb_ac_2.Text = "0연승";

            lb_aa_3.Text = "없음";
            lb_ab_3.Text = "0위 0승 0무 0패";
            lb_ac_3.Text = "0연승";

            lb_aa_4.Text = "없음";
            lb_ab_4.Text = "0위 0승 0무 0패";
            lb_ac_4.Text = "0연승";

            lb_aa_5.Text = "없음";
            lb_ab_5.Text = "0위 0승 0무 0패";
            lb_ac_5.Text = "0연승";
        }

    }
}
