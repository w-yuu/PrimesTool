using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using Microsoft.VisualBasic.FileIO;
namespace Tool
{


    public partial class Form1 : Form
    {
        //public string[,] TypeCount = new string[100,5];//100個まで

        // public DataSet typeDataSet = new DataSet();
        private static readonly Form1 _uniqueHoge = new Form1();
        public static Form1 Instance()
        {
            return _uniqueHoge;
        }

        public bool EditValueOK = true;
        public string TypePass = "Type.csv";
        public string[] TypeData;

        public string CommandPass = "Command.csv";
        public string[] CommandData;
        static public int OBJECT_ID = 20;//ゲーム側と合わせる。SetObjectの識別番号のID

        private const Int32 WM_COPYDATA = 0x4A;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
                if (cds.cbData > 0)
                {
                    //無限ループ
                    //---------------------------------------------------------------------
                    //  byte[] data = new byte[cds.cbData];
                    // Marshal.Copy(cds.lpData, data, 0, cds.cbData);
                    string hoge = cds.lpData;
                    // MessageBox.Show(hoge);
                    if (cds.dwData == (IntPtr)0)//デバッグデータポート
                    {   //とくにきめたない
                        textBox2.Text += hoge;//デバッグ画面に追加
                        textBox2.Text += Environment.NewLine;
                        this.label6.Text = "デバッグ情報を受信しました";
                        this.getDebug.Enabled = true;
                    }

                    if (cds.dwData == (IntPtr)1)//要素の追加(編集ではない)
                    {
                        EditValueOK = false;
                        //  textBox2.Text += hoge;//デバッグ画面に追加
                        //textBox2.Text += Environment.NewLine;//改行コードの挿入
                        int idx;
                        string[] _sprit = hoge.Split(' ');//オブジェクトの分解

                        for (int i = 0; i < _sprit.Length; i++)
                        {
                            string[] _item = _sprit[i].Split(',');//各要素に分解
                            //try
                            //{
                                this.dataGridView1.Rows.Add();
                                idx = this.dataGridView1.Rows.Count - 1;
                                for (int j = 0; j < _item.Length; j++)
                                {


                                    if (j != 5)//Visible
                                    {
                                        if (j != 1)//Type
                                        {
                                            //0,2,3,4番の要素を追加
                                            this.dataGridView1.Rows[idx].Cells[j].Value = _item[j];
                                        }
                                        else//int型なのでStringで名前を表示
                                        {

                                            for (int id = 0; id < TypeData.Length; id += 2)
                                            {
                                                if (_item[j] == TypeData[id])
                                                {
                                                    this.dataGridView1.Rows[idx].Cells[j].Value = TypeData[id+1];
                                                }
                                            }
                                            
                                           // -----------------------------------------------------------
                                            //if (_item[j] == '0'.ToString())
                                            //{
                                            //    this.dataGridView1.Rows[idx].Cells[j].Value = "Player";
                                            //}
                                            
                                            //if(_item[j] == '0'.ToString())
                                            //{
                                            //    this.dataGridView1.Rows[idx].Cells[j].Value = "Player";
                                            //}
                                            //else if (_item[j] == '1'.ToString())
                                            //{
                                            //    this.dataGridView1.Rows[idx].Cells[j].Value = "Enemy";
                                            //}
                                            //------------------------------------------------------------
                                            //bool OK = false;
                                            //for (int id = 0; i <= TypeData.Length; id+=2)
                                            //{
                                            //    if (_item[j] == TypeData[id])
                                            //    {
                                            //        OK = true;
                                            //        this.dataGridView1.Rows[idx].Cells[j].Value = TypeData[id + 1];
                                            //    }
                                            //}
                                            //if (OK == false)
                                            //{
                                            //    this.dataGridView1.Rows[idx].Cells[j].Value = "None";
                                            //}
                                        }

                                    }
                                    else//5番目はboolなので強制的に変換
                                    {
                                        if (_item[j] == '0'.ToString())
                                        {
                                            this.dataGridView1.Rows[idx].Cells[j].Value = false;
                                        }
                                        else
                                        {
                                            this.dataGridView1.Rows[idx].Cells[j].Value = true;
                                        }
                                    }
                                }
                            //}
                            //catch
                            //{
                            //    MessageBox.Show("要素追加のコマンドが一致しませんでした");
                            //}
                        }
                        EditValueOK = true;
                    }

                    //--------------------------------------------------------------------
                    if (cds.dwData == (IntPtr)2)//指定されたIDをセレクト状態にする
                    {
                        this.dataGridView1.ClearSelection();

                        for (int i = 1; i < dataGridView1.Rows.Count; i++)
                        {
                            if (this.dataGridView1.Rows[i].Cells[0].Value.ToString() == cds.lpData)
                            {
                                this.dataGridView1.Rows[i].Selected = true;
                            }
                        }
                    }
                }
                m.Result = (IntPtr)1;
            }
            base.WndProc(ref m);
        }


        [System.Runtime.InteropServices.DllImport(
      "user32.dll")]
        private static extern uint SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);





        struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public UInt32 cbData;
            public string lpData;
        }

        IntPtr hwnd;//ハンドル
        string msg;//送信コマンド
        public string ClassName;
        public string WindowName;
        public bool CheckBoxEdit = false;
        public Form1()
        {

            //System.Threading.Mutex objMutex = new System.Threading.Mutex(false, "gametool");

            ClassName = "Generic Game SDK Window";
            WindowName = "次　元　戦　争";

            InitializeComponent();

            LoadhWnd();//hWndを取得

            //lpClassNameの確認
            //   MessageBox.Show(System.Diagnostics.Process.GetCurrentProcess().ProcessName);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CommandData.Length; i++)
            {
                if (CommandData[i] == comboBox1.Text)
                {
                    if (System.Text.RegularExpressions.Regex.Match(CommandData[i-1], "(_OBJ)").Success == true)
                    {
                        Command(CommandData[i - 1], OBJECT_ID + 0);//オブジェクト命令と判断して送信
                    }
                    else
                    {
                        Command(CommandData[i - 1], 0);//
                    }
                    return;
                }
              
            }
            MessageBox.Show("コマンドを送信できませんでした");
          
        }
        private void LoadhWnd()
        {
            hwnd = FindWindow(ClassName, WindowName);//ターゲットを取得
            //this.label2.Text ="PID:"+ hwnd.ToString();
            if (hwnd.ToString() == "0")
            {
                this.label2.Text = "待機中";
                this.label4.Text = "";
                this.button5.Text = "編集開始";
                this.label5.Text = "";
                this.comboBox1.Enabled = false;
                dataGridView1.Rows.Clear();
                this.checkBox3.Checked = true;
            }
            else
            {
                this.label2.Text = "通信中:" + hwnd.ToString();
               
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.AcceptButton = this.button1;




            //Type情報の取得（ファイル読み込み）
            this.type情報の再読み込みToolStripMenuItem_Click(null, null);

        }



        public void Command(string Data, int dwData)
        {
            msg = Data;
            COPYDATASTRUCT data;
            data.dwData = new IntPtr(dwData);
            //data.lpData = Marshal.StringToHGlobalUni(msg);
            data.lpData = msg;
            data.cbData = (uint)(msg.Length + 1) * 2;


            SendMessage(hwnd, WM_COPYDATA, IntPtr.Zero, ref data);

        }

      
        private void button5_Click(object sender, EventArgs e)
        {//SetObjectで管理しない
            if (this.button5.Text == "編集開始")
            {
                if (checkBox2.Checked == true)
                {
                    Command("1", 100);
                }
                else
                {
                    Command("0", 100);
                }

                Command("okinput", 0);
                
                this.button5.Text = "編集終了";
                this.label5.Text ="コントロール中";

                comboBox1.Enabled = true;
            }
            else
            {
                comboBox1.Enabled = false;
                Command("noinput", 0);
               // Command("1", 100);//カメラの設定をオブジェクトにする
                this.button5.Text = "編集開始";
                this.label5.Text = "";
            }

        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)

                if( (EditValueOK == true) && (checkBox1.Checked == false)) //自動でなく取得してない状態
                {
                //=================================================
                try
                {
                    if (this.dataGridView1[0,e.RowIndex].Value.ToString() == "")
                    {
                        //MessageBox.Show("行が選択されていません");
                        return;
                    }

                    //データを文字列結合
                    //visibleはなんとなく渡していない

                    string TypeValue = "";

                    for (int id = 1; id < TypeData.Length; id += 2)
                    {
                        if (this.dataGridView1[1, e.RowIndex].Value.ToString() == TypeData[id])
                        {
                            TypeValue = TypeData[id - 1];
                        }
                    }


                    string Data = "";
                    Data = this.dataGridView1[0, e.RowIndex].Value.ToString() + " "; //ID
                    Data += TypeValue + " ";//タイプ
                    Data += this.dataGridView1[2, e.RowIndex].Value.ToString() + " ";//X
                    Data += this.dataGridView1[3, e.RowIndex].Value.ToString() + " ";//Y
                    Data += this.dataGridView1[4, e.RowIndex].Value.ToString() + " ";//Z
                    Data += this.dataGridView1[6, e.RowIndex].Value.ToString() + " ";//RotX
                    Data += this.dataGridView1[7, e.RowIndex].Value.ToString() + " ";//RotY
                    Data += this.dataGridView1[8, e.RowIndex].Value.ToString() + " ";//Group
                    Data += this.dataGridView1[9, e.RowIndex].Value.ToString() + " ";//Scale
                    Data += this.dataGridView1[10, e.RowIndex].Value.ToString();      //Flag


                    if (int.Parse(TypeValue) >= OBJECT_ID)
                    {
                        Command(Data, OBJECT_ID + 6);
                    }
                    else
                    {
                        Command(Data, 6);
                    }
                }//6
                catch
                {
                    MessageBox.Show("ゲームとの通信が確立していません");
                }
                }
                //=================================================




                //ゲーム側のDraw処理を飛ばすかどうか
                if (e.ColumnIndex == 5)
                {
                    if (CheckBoxEdit == false)//描画のON/OFF
                    {
                        return;
                    }
                    string TypeValue = "";

                    for (int id = 1; id < TypeData.Length; id += 2)
                    {
                        if (dataGridView1[1,e.RowIndex].Value.ToString() == TypeData[id])
                        {
                            TypeValue = TypeData[id - 1];
                            break;
                        }
                    }
                    if (int.Parse(TypeValue) >= OBJECT_ID)
                    {
                        Command(dataGridView1[0, e.RowIndex].Value.ToString(), OBJECT_ID + 1);//オブジェクト側
                    }
                    else
                    {

                        Command(dataGridView1[0, e.RowIndex].Value.ToString(), 1);//キャラ側
                    }
                }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dataGridView1.CurrentCellAddress.X == 5 && this.dataGridView1.IsCurrentCellDirty)
            {
                //コミット
                this.dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            LoadhWnd();
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            this.CheckBoxEdit = true;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.CheckBoxEdit = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            EditValueOK = false;
            dataGridView1.Rows.Clear();
            EditValueOK = true;
            Command("list", 0);
            Command("list_OBJ", OBJECT_ID + 0);

        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Command(textBox3.Text, 2);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Command(textBox3.Text, 4);//データの出力
        }

       

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (this.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString() == "") return;
                string TypeValue = "";

                for (int id = 1; id < TypeData.Length; id += 2)
                {
                    if (dataGridView1[1, e.RowIndex].Value.ToString() == TypeData[id])
                    {
                        TypeValue = TypeData[id - 1];
                        break;
                    }
                }
                if (int.Parse(TypeValue) >= OBJECT_ID)
                {
                    Command(this.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(), OBJECT_ID + 5);
                }
                else
                {
                    Command(this.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(), 5);
                }
            }
            catch
            {
                MessageBox.Show("ゲームとの通信が確立していません");
            }
        }

       

        private void button14_Click(object sender, EventArgs e)
        {
            Command("end", 0);
            this.label4.Text = "編集を終了しました";
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == true)
            {
                this.AutoCheck.Enabled = true;
            }
            else { this.AutoCheck.Enabled = false; }
        }

        private void AutoCheck_Tick(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            Command("list", 0);
            Command("list_OBJ", OBJECT_ID + 0);
        }

        private void 設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Type hoge = new Type();
            hoge.ShowDialog();
            this.type情報の再読み込みToolStripMenuItem_Click(null,null);
        }

        private void type情報の再読み込みToolStripMenuItem_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();//表示されているコマンドをいったん消去
            try
            {
                TextFieldParser parser = new TextFieldParser(TypePass, System.Text.Encoding.GetEncoding("Shift_JIS"));
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    TypeData = parser.ReadFields();//1行読み込み
                }

               
            }
            catch
            {
                MessageBox.Show("Type設定の初期化に失敗しました");
            }
            //------------------------------
            try
            {
                TextFieldParser parser = new TextFieldParser(CommandPass, System.Text.Encoding.GetEncoding("Shift_JIS"));
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    CommandData = parser.ReadFields();//1行読み込み
                }

                //CommandをcomboBoxに追加
                for (int i = 1; i < CommandData.Length; i += 2)
                {
                    comboBox1.Items.Add(CommandData[i]);
                }
            }
            catch
            {
                MessageBox.Show("Command設定の初期化に失敗しました");
            }



            
        }

        private void getDebug_Tick(object sender, EventArgs e)
        {
            this.label6.Text = "";
            this.getDebug.Enabled = false;
        }

        private void コマンド管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command hoge = new Command();
            hoge.ShowDialog();
            this.type情報の再読み込みToolStripMenuItem_Click(null, null);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
           
        }

        private void 自動反映ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //自動反映を切り替える
            if (checkBox1.Checked == true)
            {
                checkBox1.Checked = false;
            }
            else
            {
                checkBox1.Checked = true;
            }
        }

        private void 選択している行の値を変更ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dataGridView1.SelectedCells[0].Value.ToString() == "")
                {
                    MessageBox.Show("行が選択されていません");
                    return;
                }

                //データを文字列結合
                //visibleはなんとなく渡していない

                string TypeValue = "";

                for (int id = 1; id < TypeData.Length; id += 2)
                {
                    if (this.dataGridView1.SelectedCells[1].Value.ToString() == TypeData[id])
                    {
                        TypeValue = TypeData[id - 1];
                    }
                }


                string Data = "";
                Data = this.dataGridView1.SelectedCells[0].Value.ToString() + " "; //ID
                Data += TypeValue + " ";//タイプ
                Data += this.dataGridView1.SelectedCells[2].Value.ToString() + " ";//X
                Data += this.dataGridView1.SelectedCells[3].Value.ToString() + " ";//Y
                Data += this.dataGridView1.SelectedCells[4].Value.ToString() + " ";//Z
                Data += this.dataGridView1.SelectedCells[6].Value.ToString() + " ";//RotX
                Data += this.dataGridView1.SelectedCells[7].Value.ToString() + " ";//RotY
                Data += this.dataGridView1.SelectedCells[8].Value.ToString() + " ";//Group
                Data += this.dataGridView1.SelectedCells[9].Value.ToString() + " ";//Scale
                Data += this.dataGridView1.SelectedCells[10].Value.ToString();      //Flag


                if (int.Parse(TypeValue) >= OBJECT_ID)
                {
                    Command(Data,  OBJECT_ID + 6);
                }
                else
                {
                    Command(Data, 6);
                }
            }//6
            catch
            {
                MessageBox.Show("ゲームとの通信が確立していません");
            }

        }

        private void 編集の実行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button5_Click(null, null);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Command(textBox1.Text, OBJECT_ID + 9);
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Command(textBox1.Text,OBJECT_ID + 4);//データの出力
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Command(textBox1.Text,OBJECT_ID + 2);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                Command("1", 100);
            }
            else
            {
                Command("0", 100);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                Command("1", 200);//有効
            }
            else
            {
                Command("0", 200);//無効
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Command(textBox3.Text, 9);
        }









    }
}
