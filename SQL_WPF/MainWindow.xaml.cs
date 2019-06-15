using System;
using System.Windows;
using System.Windows.Controls;
using 의료IT공학과.데이터베이스;
using System.Data;
using Microsoft.Win32;

namespace SQL_WPF
{
    public partial class MainWindow : Window
    {
        public static LocalDB db;
        string dbName = "";

        //db설정
        public void setDB(string dbName)
        {
           db = new LocalDB("Provider=Microsoft.ACE.OLEDB.12.0; " +
                                      "Data Source=../../../DBFiles/" + dbName +";" +
                                      "Persist Security Info=False");
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        //실행 버튼
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dbName.ToString() == "")
            {
                MessageBox.Show("database파일을 선택해주세요");
            }
            else
            {
                InsertQuery();
                list_Recent.Items.Insert(0, txt_Query.Text);
                txt_Query.Clear();
            }

        } 

        //쿼리 명령어 입력
        private void InsertQuery()
        {
            db.Open();

            string query = txt_Query.Text;

            try
            {
                db.Query(query);
            }
            catch (Exception e)
            {
                MessageBox.Show(query + "\n\n" + e.Message, "SQL Error");
                dataGrid_table.ItemsSource = "";
                dataGrid_table.Items.Refresh();

            }

            InitializeTable();

            db.Close();

        }

        //최근 실행 목록 텍스트박스로 부르기
        private void List_Recent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txt_Query.Text = list_Recent.SelectedItem.ToString();

        }

        //테이블 그리기
        public void InitializeTable()
        {
            DataTable dt = new DataTable();
               
            if (db.HasRows)
            {
                for (int i = 0; i < db.FieldCount; i++)
                {
                    dt.Columns.Add(db.GetName(i),typeof(string));
                }

                while (db.Read())
                {
                    DataRow row = dt.NewRow();
                    object[] rowArray = new object[db.FieldCount];

                    for (int i = 0; i < db.FieldCount; i++)
                    {
                        rowArray[i] = db.GetData(i);
                        row = dt.NewRow();
                        row.ItemArray = rowArray;
                    }
                    dt.Rows.Add(row);
                }

            }

                dataGrid_table.ItemsSource = dt.DefaultView;

            //조건에 맞지 않으면 메세지창 표시
            if(dataGrid_table.Items.Count == 1)
            {
                MessageBox.Show("조건문에 일치하는 결과값이 존재하지 않습니다");
            }

            Count();
        }

        //data개수 확인함수
        private void Count()
        {
            var rowcount = (dataGrid_table.Items.Count-1).ToString();
            txtBlock_count.Text = rowcount;
        }

        //도움말 버튼
        private void Btn_help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("INSERT INTO [Table] VALUES (values1,values2..)\n" 
                + "DELETE FROM [Table] WHERE [Condition]\n"
                + "UPDATE [Table] SET [Condtion = 'value'] WHERE [key = num]\n"
                + "SELECT [Column] FROM [Table] [Condition]","명령어 도움말 입니다");
        }

        //database 열기 버튼
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            {
                open.CheckFileExists = true;

                if (open.ShowDialog().GetValueOrDefault())
                {
                    dbName = open.FileName;
                }

                setDB(printDBname(dbName));
            }
           
        }

        //txtblock에 선택된 db파일 출력
        private string printDBname(string str)
        {

            string[] dataName = str.Split('\\');
            txtblock_dbname.Text = dataName[dataName.Length - 1];
            txtBlock_dbcheck.Text = "DB 연결됨";
            return txtblock_dbname.Text;
        }

        private void Txt_Query_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Return)
            {
                if (dbName.ToString() == "")
                {
                    MessageBox.Show("database파일을 선택해주세요");
                }
                else
                {
                    InsertQuery();
                    list_Recent.Items.Insert(0, txt_Query.Text);
                    txt_Query.Clear();
                }
            }
        }
    }
}