using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Text;
using System.Collections;
using System.Data;
using System.Diagnostics;

namespace Bot_Application1.Lib
{
    public class Dialog
    {
        public int dlgId;
        public string dlgNm;
        public string dlgMent;
        public string dlgLang;
    }

    public class Card
    {
        public int cardId;
        public int dlgId;
        public string cardType;
        public string cardText;
        public string cardTitle;
        public string cardSubTitle;
    }

    public class Button
    {
        public int btnId;
        public int dlgId;
        public int cardId;
        public string btnType;
        public string btnTitle;
        public string btnContext;
    }

    public class Image
    {
        public int imgId;
        public int dlgId;
        public int cardId;
        public string imgUrl;
    }

    public class Media
    {
        public int mediaId;
        public int dlgId;
        public int cardId;
        public string mediaUrl;
    }

    public class Luis
    {
        public int dlgId;
        //public string intent;
        //public string entities;
    }

    public class DbConnect
    {
        string connStr = "Data Source=faxtimedb.database.windows.net;Initial Catalog=taihoML;User ID=faxtime;Password=test2016!;";
        StringBuilder sb = new StringBuilder();

        public void ConnectDb()
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(connStr);
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

        public List<Luis> SelectLuis(string intent, String entities)
        {
            SqlDataReader rdr = null;
            List<Luis> luis = new List<Luis>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT DLG_ID, INTENT, ENTITIES FROM TBL_DLG_RELATION_LUIS WHERE INTENT = '" + intent + "' AND ENTITIES = '" + entities + "'" ;

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    //string intent = rdr["INTENT"] as string;
                    //string entities = rdr["ENTITIES"] as string;


                    Luis luis1 = new Luis();
                    luis1.dlgId = dlgId;
                    //luis1.intent = intent;
                    //luis1.entities = entities;


                    luis.Add(luis1);
                }
            }

            return luis;
        }

        public List<Dialog> SelectDialog(int dlgID)
        {
            SqlDataReader rdr = null;
            List<Dialog> dialog = new List<Dialog>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT DLG_ID, DLG_NM, DLG_MENT, DLG_LANG FROM TBL_DLG WHERE DLG_ID = '" + dlgID  + "'";

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string dlgNm = rdr["DLG_NM"] as string;
                    string dlgMent = rdr["DLG_MENT"] as string;
                    string dlgLang = rdr["DLG_LANG"] as string;


                    Debug.WriteLine("DIALOG :: dlgId : " + dlgId);
                    Debug.WriteLine("DIALOG :: dlgNm : " + dlgNm);
                    Debug.WriteLine("DIALOG :: dlgMent : " + dlgMent);
                    Debug.WriteLine("DIALOG :: dlgLang : " + dlgLang);


                    Dialog dlg = new Dialog();
                    dlg.dlgId = dlgId;
                    dlg.dlgNm = dlgNm;
                    dlg.dlgMent = dlgMent;
                    dlg.dlgLang = dlgLang;


                    dialog.Add(dlg);
                }
            }

            return dialog;
        }

        public List<Card> SelectDialogCard(int dlgID)
        {
            SqlDataReader rdr = null;
            List<Card> dialogCard = new List<Card>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT CARD_ID, DLG_ID, CARD_TYPE, CARD_TITLE, CARD_SUBTITLE, CARD_TEXT FROM TBL_DLG_CARD WHERE DLG_ID = " + dlgID ;

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int cardId = Convert.ToInt32(rdr["CARD_ID"]);
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string cardType = rdr["CARD_TYPE"] as string;
                    string cardText = rdr["CARD_TEXT"] as string;
                    string cardTitle = rdr["CARD_TITLE"] as string;
                    string cardSubTitle = rdr["CARD_SUBTITLE"] as string;

                    Debug.WriteLine("Card :: cardId : " + cardId);
                    Debug.WriteLine("Card :: dlgId : " + dlgId);
                    Debug.WriteLine("Card :: cardType : " + cardType);
                    Debug.WriteLine("Card :: cardText : " + cardText);
                    Debug.WriteLine("Card :: cardTitle : " + cardTitle);
                    Debug.WriteLine("Card :: cardSubTitle : " + cardSubTitle);


                    Card dlgCard = new Card();
                    dlgCard.cardId = cardId;
                    dlgCard.dlgId = dlgId;
                    dlgCard.cardType = cardType;
                    dlgCard.cardText = cardText;
                    dlgCard.cardTitle = cardTitle;
                    dlgCard.cardSubTitle = cardSubTitle;


                    dialogCard.Add(dlgCard);
                }
            }

            return dialogCard;
        }

        public List<Button> SelectBtn(int dlgID, int cardID)
        {
            SqlDataReader rdr = null;
            List<Button> button = new List<Button>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT DLG_ID, CARD_ID, BTN_ID, BTN_TYPE, BTN_TITLE, BTN_CONTEXT FROM TBL_DLG_BTN WHERE DLG_ID = " + dlgID + " AND CARD_ID = "+ cardID;

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int btnId = Convert.ToInt32(rdr["BTN_ID"]);
                    int cardId = Convert.ToInt32(rdr["CARD_ID"]);
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string btnType = rdr["BTN_TYPE"] as string;
                    string btnTitle = rdr["BTN_TITLE"] as string;
                    string btnContext = rdr["BTN_CONTEXT"] as string;

                    Debug.WriteLine("Button :: btnId : " + btnId);
                    Debug.WriteLine("Button :: dlgId : " + dlgId);
                    Debug.WriteLine("Button :: cardId : " + cardId);
                    Debug.WriteLine("Button :: btnType : " + btnType);
                    Debug.WriteLine("Button :: btnTitle : " + btnTitle);
                    Debug.WriteLine("Button :: btnContext : " + btnContext);

                    Button btn = new Button();
                    btn.btnId = btnId;
                    btn.cardId = cardId;
                    btn.dlgId = dlgId;
                    btn.btnType = btnType;
                    btn.btnTitle = btnTitle;
                    btn.btnContext = btnContext;


                    button.Add(btn);
                }
            }

            return button;
        }

        public List<Image> SelectImage(int dlgID, int cardID)
        {
            SqlDataReader rdr = null;
            List<Image> image = new List<Image>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT IMG_ID, DLG_ID, CARD_ID, IMG_URL FROM TBL_DLG_IMG WHERE DLG_ID = " + dlgID + " AND CARD_ID = " + cardID;

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int imgId = Convert.ToInt32(rdr["IMG_ID"]);
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    int cardId = Convert.ToInt32(rdr["CARD_ID"]);
                    string imgUrl = rdr["IMG_URL"] as string;

                    Debug.WriteLine("Image :: imgId : " + imgId);
                    Debug.WriteLine("Image :: dlgId : " + dlgId);
                    Debug.WriteLine("Image :: cardId : " + cardId);
                    Debug.WriteLine("Image :: imgUrl : " + imgUrl);



                    Image img = new Image();
                    img.dlgId = dlgId;
                    img.imgId = imgId;
                    img.cardId = cardId;
                    img.imgUrl = imgUrl;


                    image.Add(img);
                }
            }

            return image;
        }

        public List<Media> SelectMedia(int dlgID, int cardID)
        {
            SqlDataReader rdr = null;
            List<Media> media = new List<Media>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT MEDIA_ID, DLG_ID, CARD_ID, MEDIA_URL FROM TBL_DLG_MEDIA WHERE DLG_ID = " + dlgID + " AND CARD_ID = " + cardID;

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int mediaId = Convert.ToInt32(rdr["MEDIA_ID"]);
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    int cardId = Convert.ToInt32(rdr["CARD_ID"]);
                    string mediaUrl = rdr["MEDIA_URL"] as string;

                    Debug.WriteLine("Media :: mediaId : " + mediaId);
                    Debug.WriteLine("Media :: dlgId : " + dlgId);
                    Debug.WriteLine("Media :: cardId : " + cardId);
                    Debug.WriteLine("Media :: mediaUrl : " + mediaUrl);

                    Media med = new Media();
                    med.dlgId = dlgId;
                    med.cardId = cardId;
                    med.mediaId = mediaId;
                    med.mediaUrl = mediaUrl;


                    media.Add(med);
                }
            }

            return media;
        }

        //public List<Car> SelectDb(string intent)
        //{
        //    SqlDataReader rdr = null;
        //    List<Car> card = new List<Car>();

        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.Connection = conn;
        //        cmd.CommandText = "select * from tbl_card_test where INTENT = '"+ intent + "'";

        //        rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        //        while (rdr.Read())
        //        {
        //            string sid = rdr["SID"].ToString();
        //            string language = rdr["LANGUAGE"] as string;
        //            string cardType = rdr["CARD_TYPE"] as string;
        //            string cardMent = rdr["CARD_MENT"] as string;
        //            string cardTitle = rdr["CARD_TITLE"] as string;
        //            string cardSubTitle = rdr["CARD_SUBTITLE"] as string;
        //            string cardText = rdr["CARD_TEXT"] as string;
        //            string cardMedia = rdr["CARD_MEDIA"] as string;
        //            string cardImage = rdr["CARD_IMAGE"] as string;
        //            string cardButton = rdr["CARD_BUTTON"] as string;
        //            string cardButtonContent = rdr["CARD_BUTTON_CONTENT"] as string;
        //            string erase = rdr["ERASE"] as string;

        //            Car car = new Car();
        //            car.sid = sid;
        //            car.language = language;
        //            car.cardType = cardType;
        //            car.cardMent = cardMent;
        //            car.cardTitle = cardTitle;
        //            car.cardSubTitle = cardSubTitle;
        //            car.cardText = cardText;
        //            car.cardMedia = cardImage;
        //            car.cardImage = cardImage;
        //            car.cardButton = cardButton;
        //            car.cardButtonContent = cardButtonContent;
        //            car.erase = erase;

        //            card.Add(car);
        //        }
        //    }

        //    return card;
        //}

    }
}