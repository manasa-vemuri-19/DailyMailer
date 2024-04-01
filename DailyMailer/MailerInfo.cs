using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class MailerInfoEntity
{
    public DateTime? SentOn { get; set; }
    public string SentOnFormatted { get; set; }
    public string SentBy { get; set; }
}

public class MailerInfoConstants
{

    public const string ItravelMailer = "iTravel Mailer";
    public const string DemandLossMailer = "Demand Loss Mailer";
    public const string Be4CastMailer = "BE 4Cast Mailer";
    public const string DailyMailer = "Daily Mailer";
    public const string UtilizationMailer = "Utilization Mailer";
    public const string EmployeeMailer = "Employee Mailer";

}

public class MailerInfo
{

    static string connectionString = ConfigurationManager.AppSettings["DemandCaptureConnectionString"].ToString();

    public static void UpdateMailerStatus(string mailerName)
    {
        string sentBy = GetUserID();

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Update MailerSentStatus Set  LastSentOn=getdate(), SentBy=@SentBy where MailerName=@MailerName", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@MailerName", mailerName);
            cmd.Parameters.AddWithValue("@SentBy", sentBy);
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }

    private static string GetUserID()
    {
        string[] machineUsers = HttpContext.Current.User.Identity.Name.Split('\\');
        if (machineUsers.Length == 2)
            return machineUsers[1];
        return "";
    }


    public static MailerInfoEntity GetMailerSentStatus(string mailerName)
    {

        MailerInfoEntity entity = new MailerInfoEntity();
        SqlDataAdapter da = new SqlDataAdapter();
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("SELECT LastSentOn , SentBy FROM MailerSentStatus where  MailerName=@MailerName ", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@MailerName", mailerName);
            da.SelectCommand = cmd;
            DataTable dt = new DataTable();
            da.Fill(dt);
            entity.SentBy = dt.Rows[0]["SentBy"] + "";
            object lastSentOn = dt.Rows[0]["LastSentOn"];
            if (lastSentOn != DBNull.Value)
            {
                entity.SentOnFormatted = Convert.ToDateTime(dt.Rows[0]["LastSentOn"]).ToString("dd-MMM-yyyy hh:mm:ss tt") + "";
                entity.SentOn = Convert.ToDateTime(dt.Rows[0]["LastSentOn"]);
            }
        }
        return entity;
    }


}