using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Microsoft.VisualBasic.FileIO;

namespace Tool
{
    public partial class Type : Form
    {
        
        string[] TypeData;
        int id = 0;
        public Type()
        {
            InitializeComponent();
        }

        public void SaveCSV(String fp)
        {
            // CSVファイルオープン
            StreamWriter sw =
                new StreamWriter(fp, false,
                System.Text.Encoding.GetEncoding("SHIFT-JIS"));
            string WriteData = "";
            int RowCount = dataGridView1.Rows.Count -1 ;
            for (int i = 0; i <RowCount; i++)
            {
                
                WriteData += this.dataGridView1.Rows[i].Cells[0].Value.ToString();
                WriteData += ",";
                WriteData += this.dataGridView1.Rows[i].Cells[1].Value.ToString();
                WriteData += ",";
            }
            //書き込み
            sw.Write(WriteData);
            // CSVファイルクローズ
            sw.Close();
        }
        private void OpenCSV(string fp)
        {
            //// ロードする前に、データセットをクリアする。
            //this.dataSet1.Clear();
            //// データソースに XML のデータを読み込む。
            //FileStream myFileStream = new FileStream(Form1.Instance().TypePass, System.IO.FileMode.Open);
            //XmlTextReader myXmlReader = new XmlTextReader(myFileStream);
            //// XML ファイルから読み込む
            //this.dataSet1.ReadXml(myXmlReader);
            //myXmlReader.Close();

            try
            {
                
                TextFieldParser parser = new TextFieldParser(Form1.Instance().TypePass, System.Text.Encoding.GetEncoding("Shift_JIS"));
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    TypeData = parser.ReadFields();//1行読み込み
                }
                //this.dataGridView1.Rows.Clear();//すべて削除

                for (int _cnt = 0; _cnt < TypeData.Length / 2; _cnt++)
                {
                    this.dataGridView1.Rows.Add();
                }
                for (int id = 0, cnt = 0; cnt < TypeData.Length / 2; id += 2, cnt++)
                {

                    
                    this.dataGridView1.Rows[cnt].Cells[0].Value = TypeData[id];
                    this.dataGridView1.Rows[cnt].Cells[1].Value = TypeData[id + 1];
                    
                }
            }
            catch
            {
                MessageBox.Show("Type情報の初期化に失敗しました");
            }
        }


        private void Type_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            try
            {
                ////this.dataGridView1;//Form1に保存

                //// データソースに XML のデータを書き込む。
                //FileStream myFileStream = new FileStream(Form1.Instance().TypePass, System.IO.FileMode.Create);
                //XmlTextWriter myXmlWriter = new XmlTextWriter(myFileStream, System.Text.Encoding.UTF8);
                //// インデントをつけて書き出すように指定する。
                //myXmlWriter.Formatting = Formatting.Indented;
                //// XML ファイルに書き出す
                //this.dataSet1.WriteXml(myXmlWriter);
                //myXmlWriter.Close();

                this.SaveCSV(Form1.Instance().TypePass);
            }
            catch
            {
                MessageBox.Show("Type情報の保存に失敗しました");
            }
           

            //TypeデータをFrom1の配列に登録
            //for(int i = 0;i < this.dataGridView1.Rows.Count;i++ )
            //{
            //    Form1.Instance().TypeCount[i][0] = this.dataGridView1.Rows[i].Cells[0].Value;//TypeNumber
            //    Form1.Instance().TypeCount[i][1] = this.dataGridView1.Rows[i].Cells[1].Value;//TypeName
            //}
        }

        private void Type_Load(object sender, EventArgs e)
        {
            OpenCSV(Form1.Instance().TypePass);
            try
            {
               
            }
            catch
            {
                MessageBox.Show("Type情報の読み込みに失敗しました");
            }
        }

      

        
    }
}
