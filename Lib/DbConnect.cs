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
        public string carTitle;
        public string carImage;
        public string carButton;
        public string carButtonContent;
        public string erase;
    }

    public class DbConnect
    {
        string strConn = "Data Source=faxtimedb.database.windows.net;Initial Catalog=taihoML;User ID=faxtime;Password=test2016!;";
        StringBuilder sb = new StringBuilder();

        public void ConnectDb()
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

        public List<Car> SelectDb()
        {
            SqlDataReader rdr = null;
            List<Car> card = new List<Car>();

            using (SqlConnection conn = new SqlConnection(strConn))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from tbl_card_test";

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    string sid = rdr["SID"].ToString();
                    string carTitle = rdr["CAR_TITLE"] as string;
                    string carImage = rdr["CAR_IMAGE"] as string;
                    string carButton = rdr["CAR_BUTTON"] as string;
                    string carButtonContent = rdr["CAR_BUTTON_CONTENT"] as string;
                    string erase = rdr["ERASE"] as string;

                    Car car = new Car();
                    car.sid = sid;
                    car.carTitle = carTitle;
                    car.carImage = carImage;
                    car.carButton = carButton;
                    car.carButtonContent = carButtonContent;
                    car.erase = erase;

                    card.Add(car);
                }
            }

            return card;
        }

    }
}