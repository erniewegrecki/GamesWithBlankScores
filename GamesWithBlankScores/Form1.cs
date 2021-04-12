using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace GamesWithBlankScores
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class GameResult
        {
            public int LeagueYear { get; set; }
            public int DivisionNo { get; set; }
            public DateTime GameDate { get; set; }
            public int VisitorNo { get; set; }
            public char VisitorResult { get; set; }
            public int HomeNo { get; set; }
            public char HomeResult { get; set; }
        }

        string connString = ConfigurationManager.ConnectionStrings["thisConnection"].ToString();

        private void Form1_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                List<GameResult> gameResult = new List<GameResult>();

                using (SqlCommand cmd = new SqlCommand())
                {
                    int thisYear = DateTime.Today.Year;

                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Select * From GameSchedule Where LeagueYear = " + thisYear;

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {

                        while (rdr.Read())
                        {
                            gameResult.Add(new GameResult()
                            {
                                DivisionNo = rdr.GetInt32(rdr.GetOrdinal("DivisionNo")),
                                GameDate = rdr.GetDateTime(rdr.GetOrdinal("GameDate")),
                                HomeNo = rdr.GetInt32(rdr.GetOrdinal("HomeNo")),
                                HomeResult = ' ',
                                LeagueYear = DateTime.Now.Year,
                                VisitorNo = rdr.GetInt32(rdr.GetOrdinal("VisitorNo")),
                                VisitorResult = ' '
                            });
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    int ctr = 0;

                    foreach (GameResult game in gameResult)
                    {
                        ctr += 1;

                        cmd.CommandText = "Insert into GameResults Values(@LeagueYear, @DivisionNo, @GameDate, @VisitorNo, @VisitorResult, " +
                            "@HomeNo, @HomeResult)";

                        if (ctr == 1)
                        {
                            cmd.Parameters.AddWithValue("@LeagueYear", DateTime.Now.Year);
                            cmd.Parameters.AddWithValue("@DivisionNo", game.DivisionNo);
                            cmd.Parameters.AddWithValue("@GameDate", game.GameDate);
                            cmd.Parameters.AddWithValue("@VisitorNo", game.VisitorNo);
                            cmd.Parameters.AddWithValue("@VisitorResult", game.VisitorResult);
                            cmd.Parameters.AddWithValue("@HomeNo", game.HomeNo);
                            cmd.Parameters.AddWithValue("@HomeResult", game.HomeResult);
                        }
                        else
                        {
                            cmd.Parameters["@LeagueYear"].Value = DateTime.Now.Year;
                            cmd.Parameters["@DivisionNo"].Value = game.DivisionNo;
                            cmd.Parameters["@GameDate"].Value = game.GameDate;
                            cmd.Parameters["@VisitorNo"].Value = game.VisitorNo;
                            cmd.Parameters["@VisitorResult"].Value = game.VisitorResult;
                            cmd.Parameters["@HomeNo"].Value = game.HomeNo;
                            cmd.Parameters["@HomeResult"].Value = game.HomeResult;
                        }

                       
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }

                }
            }
        }
    }
}
