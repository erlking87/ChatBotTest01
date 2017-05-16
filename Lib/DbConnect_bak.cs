using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Text;
using System.Collections;
using System.Data;

namespace Bot_Application1.Lib
{
    public class Car
    {
        public string sid;
        public string language;
        public string cardType;
        public string cardMent;
        public string cardTitle;
        public string cardSubTitle;
        public string cardText;
        public string cardMedia;
        public string cardImage;
        public string cardButton;
        public string cardButtonContent;
        public string erase;
    }

    public class DbConnect_bak
    {
        string strConn = "Data Source=faxtimedb.database.windows.net;Initial Catalog=taihoML;User ID=faxtime;Password=test2016!;";
        StringBuilder sb = new StringBuilder();

        public void ConnectBakDb()
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(strConn);
                conn.Open();
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }

        public List<Car> SelectDb(string intent)
        {
            SqlDataReader rdr = null;
            List<Car> card = new List<Car>();

            using (SqlConnection conn = new SqlConnection(strConn))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from tbl_card_test where INTENT = '"+ intent + "'";

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    string sid = rdr["SID"].ToString();
                    string language = rdr["LANGUAGE"] as string;
                    string cardType = rdr["CARD_TYPE"] as string;
                    string cardMent = rdr["CARD_MENT"] as string;
                    string cardTitle = rdr["CARD_TITLE"] as string;
                    string cardSubTitle = rdr["CARD_SUBTITLE"] as string;
                    string cardText = rdr["CARD_TEXT"] as string;
                    string cardMedia = rdr["CARD_MEDIA"] as string;
                    string cardImage = rdr["CARD_IMAGE"] as string;
                    string cardButton = rdr["CARD_BUTTON"] as string;
                    string cardButtonContent = rdr["CARD_BUTTON_CONTENT"] as string;
                    string erase = rdr["ERASE"] as string;

                    Car car = new Car();
                    car.sid = sid;
                    car.language = language;
                    car.cardType = cardType;
                    car.cardMent = cardMent;
                    car.cardTitle = cardTitle;
                    car.cardSubTitle = cardSubTitle;
                    car.cardText = cardText;
                    car.cardMedia = cardImage;
                    car.cardImage = cardImage;
                    car.cardButton = cardButton;
                    car.cardButtonContent = cardButtonContent;
                    car.erase = erase;

                    card.Add(car);
                }
            }

            return card;
        }

    }
}