using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DailyMailer
{
    public partial class Mailer : System.Web.UI.Page
    {
        static DataAccess objDL = new DataAccess();

        string head = @"<html lang=""en"">
            <head>
            <meta content='text/html; charset=utf-8' http-equiv='Content-Type'>
            
            <style type='text/css'>
   
            .courses-table{font-size: 12px;font-family:calibri; border-collapse: collapse;}
            .bodystyle{font-size: 14px;font-family:calibri;}
            .courses-table th{border: 1px solid black;}
            .Headings{border: 1px solid #708090;background-color: dimgray;color: white; padding:5px;font-size:13px;}
            .Heading_SOH{border: 1px solid #708090;background-color: dimgray;color: white; padding:5px;font-size:13px;white-space: nowrap;}

            .Headings1{border: 1px solid #708090;background-color: #f1f1f1;color: #708090; padding:3px;font-size:11px; font-weight:bold}
	
            .Headings_Util1{border: 1px solid #708090;background-color: burlywood;color: white; padding:5px;font-size:13px;}
            .Headings_Util2{border: 1px solid #708090;background-color: gray;color: white; padding:5px;font-size:13px;}
            .Headings_Util3{border: 1px solid #708090;background-color: lightsteelblue;color: white; padding:5px;font-size:13px;}

            .High{border: 1px solid  #a94442;background-color: #ebccd1;color: #a94442;}
 .redcolor{ color: red;}
	
            .Medium{border: 1px solid #8a6d3b;background-color: #faebcc;color: #8a6d3b;}
	
            .Low{border: 1px solid #3c763d;background-color: #d6e9c6;color: #3c763d;}
            .custom{border: 1px solid black;color: black; padding:3px;text-align: center;}
            .customOverall{border: 1px solid darkcyan;background-color: #ffe699;color: black; padding:3px;text-align: center;}
            .customBillable{border: 1px solid darkcyan;background-color: #92d050;color: black; padding:3px;text-align: center;}
            .customNonBillable{border: 1px solid darkcyan;background-color: #f7caac;color: black; padding:3px;text-align: center;}
            .subtitle {font-size:16px; color:black; font-weight:bold}
	
            table {
            display: table;
            border-collapse: collapse !important;
            border-spacing: 0px !important;
            -webkit-border-horizontal-spacing: 2px;
            -webkit-border-vertical-spacing: 2px;
            border-color:  #708090 !important;
            }

            .divchart {
            height: 18px;
            width: 55px;
            background-color: white;
            border: 1px solid lightgray;
             
            }

            .divinner {
            width: 6px;
            height: 17px;
            margin-left: 1px; 
            /*float:left;*/
            background-color: brown;
            
            }
            .imgWeekly
            {
            height:19px;
            width:58px;
            }


            </style>
            </head>
            <body class='bodystyle'>";

        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateMailerInfo();
        }
        protected void UpdateMailerInfo()
        {
            var info = MailerInfo.GetMailerSentStatus(MailerInfoConstants.DailyMailer);
            bSentBy.InnerText = info.SentBy;
            bSentOn.InnerText = info.SentOnFormatted == null ? "Info Not Available" : info.SentOnFormatted;
        }

        protected bool IsMailerRepeated()
        {

            var info = MailerInfo.GetMailerSentStatus(MailerInfoConstants.DailyMailer);
            bool returnValue = false;
            if (info.SentOn.HasValue)
            {
                if (info.SentOn.Value.ToString("dd-MM-yyyy") == DateTime.Now.ToString("dd-MM-yyyy"))
                {
                    UpdateMailerInfo();
                    ClientScript.RegisterStartupScript(this.GetType(), "Error", "alert('Mailer has been triggered today, if you wish to resend the mail, pls set the [LastSentOn] column value as null for the respected item in the table [MailerSentStatus]')", true);
                    returnValue = true;
                }
            }

            return returnValue;

        }

        protected void btnmailer_Click(object sender, EventArgs e)
        {
            if (IsMailerRepeated())
            {
                return;
            }


            // var dt_Users = objDL.GetDataFromQuery("select Parameter,ToList from Mailers_Daily_load_data ");

            DateTime date = DateTime.Now;

            string subject = "Daily Data Load Status - " + date.ToString("dd MMM yyyy");

            string[] tolist = ConfigurationManager.AppSettings["TO"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            string[] cclist = { };
            string[] bcclist = ConfigurationManager.AppSettings["BCC"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            string mainbody = GenerateHTML();

            foreach (string email in tolist)
            {
                string toName = "";
                // toName = "  Hi "+ row["Parameter"].ToString().Trim() + ", <br /><br />";
                toName = "  Hi " + email + ", <br /><br />";
                string localMainBody = mainbody;
                localMainBody = head + toName + localMainBody;
                //string toMailID   = row["ToList"].ToString().Trim();
                string[] toMailID = new string[] { email.ToString().Trim() };

                SMTPUtil.SendMail(subject, localMainBody, toMailID, cclist, bcclist);
            }

            MailerInfo.UpdateMailerStatus(MailerInfoConstants.DailyMailer); // Line
            UpdateMailerInfo();  // Line 


            ClientScript.RegisterStartupScript(this.GetType(), "SendClose", "SendClose();", true);
            lblSuccess.Visible = true;
            lblSuccess.Text = "Mailer Sent Successfully.";
        }



        static void FormatSOHTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in row.Table.Columns)
                {
                    if (col.ColumnName != "txtsu")
                    {
                        string value = (row[col] + "").Trim();
                        double d = 0;
                        if (double.TryParse(value, out d))
                        {
                            row[col] = Math.Round(d, 0);
                        }

                    }
                }
            }
        }




        static string GenerateHTML()
        {
            //double num;
            //double.TryParse("-100000.01", out num);
            //var str_num = string.Format(String.Format("{0:N}", num));




            var CurrDate = DateTime.Now;
            var PrevDate = CurrDate.AddDays(1);

            string title = "title";

           
            DataTable dt_RTBR_BEvsActual = objDL.ExecuteSP("SP_DailyMailerBody_Adj_RTBRVSBE");
            DataTable dt_MCCwithoutSDM = objDL.ExecuteSP("SP_DailyMailerBody_MCC_without_SDM_mappings");
            DataSet ds_Util = objDL.ExecuteSP_Ds("EASUtilizationMailer_7Days");
            DataTable dt_DelayedBilling1 = objDL.ExecuteSP("SP_DailyMailerBody_DelayedBilling_Category");
            DataTable dt_DelayedBilling2 = objDL.ExecuteSP("SP_DailyMailerBody_DelayedBilling_AgeingBucket");
            DataTable dt_AlconPBS = objDL.ExecuteSP("SP_DailyMailerBody_AlconPBS_New");
            DataTable dt_AlconPBS_Top5 = objDL.ExecuteSP("SP_DailyMailerBody_AlconPBSVariance");
            DataTable dt_RTBR = objDL.ExecuteSP("SP_DailyMailerBody_RTBR_enhancement");
            DataTable dt_RTBR_Top5 = objDL.ExecuteSP("SP_DailyMailerBody_RTBRVariance_enhancement");
            DataTable dt_ProjectStatus = objDL.ExecuteSP("SP_DailyMailerBody_ProjectStatus");
            DataTable dt_AlconConfirmation = objDL.ExecuteSP("SP_DailyMailerBody_Uninitiated_Alcon_Confirmation");

            DataTable dt_MissingMCC = objDL.ExecuteSP("SP_DailyMailerBody_MissingMCC");
            DataTable dt_MissingRoleMapping = objDL.ExecuteSP("SP_DailyMailerBody_MissingRoleMapping");
            DataTable dt_InvalidSDM = objDL.ExecuteSP("SP_DailyMailerBody_InvalidSDM");
            DataTable dt_nonEAS_BEusers = objDL.ExecuteSP("SP_DailyMailerBody_BEUserNotPartOfEASReport");
            DataTable dt_nonEAS_NebulaUsers = objDL.ExecuteSP("SP_DailyMailerBody_NebulaUserNotPartOfEASReport");

            DataTable dt_31MarDate = objDL.GetDataFromQuery("select max(ExRatedate) from BEExchangeRates_FA where month(ExRatedate)=3 and day(ExRatedate)=31");

            DataSet ds_SOB_BE = new DataSet();
            ds_SOB_BE = objDL.Execute_SP("SP_SOH_Current_Next_BE_Trend_V1");
            DataTable dt_SOB_BE_REV = ds_SOB_BE.Tables[0];
            DataTable dt_SOB_BE_VOL = ds_SOB_BE.Tables[1];

            dt_SOB_BE_REV.Columns.Remove("txtsu1");
            dt_SOB_BE_VOL.Columns.Remove("txtsu1");

            DataSet ds_SOB_BE_New_Biz = new DataSet();
            ds_SOB_BE_New_Biz = objDL.Execute_SP("SOH_BE_Snapshot_Report_New_Biz");
            DataTable dt_SOH_BE_New_Biz_REV = ds_SOB_BE_New_Biz.Tables[0];
            DataTable dt_SOH_BE_New_Biz_VOL = ds_SOB_BE_New_Biz.Tables[1];

            DataTable dt_Subcon_NonProd = objDL.ExecuteSP("SP_DailyMailer_Subcon_NonProd");

            //FormatSOHTable(dt_SOB_BE_REV);
            //FormatSOHTable(dt_SOB_BE_VOL);

            DataTable dt_SOHBEData_AsOfToday = objDL.ExecuteSP("SP_DailyMailerBody_SOH_BEData_AsOfToday_New");

            DateTime date_31Mar;
            DateTime.TryParse(dt_31MarDate.Rows[0][0].ToString().Trim(), out date_31Mar);

            string str_31Mar = date_31Mar.ToString("dd-MMM-yyyy");
            string date_Today = DateTime.Now.ToString("dd-MMM-yyyy");





            string h2 = "  Please find below the data load status as on  " + CurrDate.ToString("dddd, dd MMMM, yyyy") + "<br /><br />";
            string h3 = "  * value within bracket indicates difference between values of current load and previous load.<br />" ;
            string h4 = " <i class='redcolor'>  *** Replies sent to this mail-box are not monitored. *** </i><br /><br />";
            string html = "";


            string footer = "<br /><br />  Regards,<br />";
            footer += @"  Nebula Team
                            </body>
                            </html>";



            var table_SOH_BE_REV = SOH_BE_Trend(dt_SOB_BE_REV, "SOH BE Trend - Revenue as per " + str_31Mar + " exchange rates", str_31Mar);
            var table_SOH_BE_VOL = SOH_BE_Trend(dt_SOB_BE_VOL, "SOH BE Trend - Volume", str_31Mar);
            var table_SOHBEData_AsOfToday = GenerateTable_Type1(dt_SOHBEData_AsOfToday, "Current Qtr SOH BE as of today", str_31Mar);
            var table_SOHBE_New_Biz_Rev = GenerateTable_Type1_New_Biz(dt_SOH_BE_New_Biz_REV, "SOH New Biz Revenue", str_31Mar);
            var table_SOHBE_New_Biz_Vol = GenerateTable_Type1_New_Biz(dt_SOH_BE_New_Biz_VOL, "SOH New Biz Volume", str_31Mar);


            var table_Subcon_NonProd = GenerateTable_Type1_New_Biz(dt_Subcon_NonProd, "Subcon on Non-Prod", "");
            var table_MCCwithoutSDM = GenerateTable_Type2(dt_MCCwithoutSDM, "MCC without SDM mappings");

            var table_Util1 = GenerateTable_Type_Util(ds_Util.Tables[0], ds_Util.Tables[1], "Utilization % Snapshot", "* Note: In the project SL <> filter of Utilization report, “others” has been excluded in the above Snapshot. This includes Subcon.", "<tr> <th   >  </th>   <th   colspan = '7' > Overall - Utilization, Snapshot(%) </ th >  <th   colspan = '7' > Onsite - Utilization, Snapshot(%) </ th >               <th   colspan = '7' > Offshore - Utilization, Snapshot(%) </ th > </ tr > ");
            var table_Util2 = GenerateTable_Type_Util(ds_Util.Tables[0], ds_Util.Tables[2], "HC # Snapshot", "* Note: In the project SL <> filter of Utilization report, “others” has been excluded in the above Snapshot. This includes Subcon.", "<tr> <th   >  </th>   <th   colspan = '7' > Overall - Total Head Count, Snapshot (#)	 </ th >  <th   colspan = '7' > Onsite - Total Head Count, Snapshot (#)  </ th >    <th   colspan = '7' > Offshore - Total Head Count, Snapshot(#)	</ th > </ tr > ");
            var table_Util3 = GenerateTable_Type_Util(ds_Util.Tables[0], ds_Util.Tables[3], "Subcon HC # Snapshot", "* Note: In the project SL <> filter of Utilization report, “others” has been excluded in the above Snapshot.", "<tr> <th   >  </th>   <th   colspan = '7' > Subcon - Total HC, Snapshot (#) </ th >  <th   colspan = '7' > Subcon - Onsite HC, Snapshot (#)		 </ th > <th   colspan = '7' > Subcon - Offshore HC, Snapshot(#)	</ th > </ tr > ");


            var table_DelayedBilling1 = GenerateTable_Type1(dt_DelayedBilling1, "Delayed Billing(in KUSD)", "");
            var table_DelayedBilling2 = GenerateTable_Type1(dt_DelayedBilling2, "Delayed Billing(in KUSD)", "");
            var table_AlconPBS = GenerateTable_Type1(dt_AlconPBS, "Alcon PBS BE(in PersonMonths)", "");
            var table_AlconPBS_Top5 = GenerateTable_Type3(dt_AlconPBS_Top5, "Alcon PBS BE " + DateTime.Now.ToString("MMM") + " Variance as Alcon - BE, Top 5");
            var table_RTBR = GenerateTable_Type1(dt_RTBR, "RTBR(in KUSD) as per " + str_31Mar + " exchange rates", "");
            var table_RTBR_BE = GenerateTable_Type1(dt_RTBR_BEvsActual, "BE Vs RTBR (KUSD) as per " + str_31Mar + " exchange rates", str_31Mar);
            var table_RTBR_Top5 = GenerateTable_Type3_lat(dt_RTBR_Top5, "Current Quarter Adj. RTBR (in KUSD) as per " + str_31Mar + " exchange rates Top 5 ");
            var table_ProjectStatus = GenerateTable_Type2(dt_ProjectStatus, "Project Status as on " + date_Today);
            var table_AlconConfirmation = GenerateTable_Type2_lat(dt_AlconConfirmation, "Uninitiated Alcon Confirmation as on " + date_Today);
            var table_MissingMCC = GenerateTable_Type2(dt_MissingMCC, "Missing MCCs");
            var table_MissingRoleMapping = GenerateTable_Type2(dt_MissingRoleMapping, "Missing RoleMappings for all JL");
            var table_InvalidSDM = GenerateTable_Type2(dt_InvalidSDM, "Invalid SDM Mail IDs");
            var table_nonEAS_BEusers = GenerateTable_Type2(dt_nonEAS_BEusers, "Below users have been granted access to BE portal who are not part of EAS unit as of today");
            var table_nonEAS_NebulaUsers = GenerateTable_Type2(dt_nonEAS_NebulaUsers, "Below users have been granted access to Nebula portal who are not part of EAS unit as of today");



            html = h2 +
                   h3 +
                   h4 +
                   //table_MCCwithoutSDM +
                   //"<br /><br />" +
                      table_Util1 +
                       "<i>* ECAS - Excl Fluido and Simplus </i><br />" +
                      "<i>*  *Note: In the project SL<> filter of Utilization report, “others” has been excluded in the above Snapshot.This includes Subcon.</i> <br />" +
                   "<br />" +
                      table_Util2 +

                       "<i>* ECAS - Excl Fluido and Simplus </i><br />" +
                      "<i>*  *Note: In the project SL<> filter of Utilization report, “others” has been excluded in the above Snapshot.This includes Subcon.</i> <br />" +

                   "<br />" +
                      table_Util3 +

                       "<i>* ECAS - Excl Fluido and Simplus </i> <br />" +
                       "<i>*  Note: In the project SL <> filter of Utilization report, “others” has been excluded in the above Snapshot. </i> <br />" +

                   "<br />" +
                   table_Subcon_NonProd +

                       "<i>* ECAS - Excl Fluido and Simplus </i> <br />" +
                       "<i>*  Note: In the project SL <> filter of Utilization report, “others” has been excluded in the above Snapshot. </i> <br />" +

                   "<br />" +
                   table_DelayedBilling1 +
                   "<i>** Salesforce incl Fluido and Simplus </i><br/>" +
                   "<br />" +
                   table_DelayedBilling2 +
                   "<i>** Salesforce incl Fluido and Simplus </i><br/>" +
                   "<br /><br />" +
                   table_AlconPBS +

                    "<i> ***PBS means PBS Submitted </i><br/>" +

                    GetAlconPBSBESnapshot(str_31Mar) +
                     //"<i> Previous </ i >< br /> " +
                     //GetAlconPBSBESnapshot(str_31Mar) +

                    "<br /><br />" +
                     "<i>** Salesforce incl Fluido and Simplus </i><br/>" +
                    "<i>*** In the absence of a mechanism to segregate Fluido and Simplus BE in 4Cast360 system at the moment, only ECAS Standalone and Salesforce BE details are provided. We shall revisit this once there is a mechanism to segregate Fluido and Simplus. </i> <br /><br />" +

                   table_AlconPBS_Top5 +
                  "<i> *** PBS means PBS Submitted <br />  ^^^ ECAS - Excl Fluido and Simplus <br /> SOH BE (4Cast360)- New Biz are excluded</i> <br /><br />" +
                   "<br /><br />" +
                   table_RTBR +
                   "<i>* Adj. RTBR from PBS is considered for the Current Qtr and RTBR from PBS for Next Qtr. </i> <br />" +
                   "<i>" + GetAdjRTBRSnapshot(str_31Mar) + " </i> <br />" +
                    "<i>" + GetRTBRSnapshot(str_31Mar) + " </i> <br />" +
                    "<i> * *Salesforce incl Fluido and Simplus </i> <br />" +
                   
                    //"<i>^^^ ECAS - Incl Fluido and Simplus </i> <br /><br />" +
                    "<br /><br />" +
                   table_RTBR_BE +
                   "<i>* Adj. RTBR from PBS is considered for the Current Qtr and RTBR from PBS for Next Qtr. </i> <br />" +
                   "<i>" + GetAdjRTBRSnapshot(str_31Mar) + " </i> <br />" +
                    "<i>" + GetRTBRSnapshot(str_31Mar) + " </i> <br />" +
                    //"<i>^^^ ECAS - Incl Fluido and Simplus </i> <br />" +
                   "<i>" + GetAlconPBSBESnapshot(str_31Mar) + " <br />" +
                   "<i> * **In the absence of a mechanism to segregate Fluido and Simplus BE in 4Cast360 system at the moment, " +
                            "only ECAS Standalone and Salesforce BE details are provided.We shall revisit this once there is a " +
                            "mechanism to segregate Fluido and Simplus </ i > " +
                   "<br /><br />" +
                   table_RTBR_Top5 +
                   "<i>* Adj. RTBR from PBS is considered for the Current Qtr. </i> <br />" +
                    //"<i>^^^ ECAS - Incl Fluido and Simplus </i> <br /><br />" +
                    "<br /><br />" +
                    table_SOHBEData_AsOfToday +
                    "<i>** Salesforce incl Fluido and Simplus </i> <br/>" +
                    "<i>*** In the absence of a mechanism to segregate Fluido and Simplus BE in 4Cast360 system at the moment, only ECAS Standalone and Salesforce BE details are provided. We shall revisit this once there is a mechanism to segregate Fluido and Simplus. </i> <br /><br />" +
                    "<br /><br />" +
                    table_SOHBE_New_Biz_Rev +
                    "<i>** Salesforce incl Fluido and Simplus </i><br/>" +
                    "<i>*** In the absence of a mechanism to segregate Fluido and Simplus BE in 4Cast360 system at the moment, only ECAS Standalone and Salesforce BE details are provided. We shall revisit this once there is a mechanism to segregate Fluido and Simplus. </i> <br /><br />" +
                    "<br /><br />" +
                    table_SOHBE_New_Biz_Vol +
                    "<i>** Salesforce incl Fluido and Simplus </i><br/>" +
                    "<i>*** In the absence of a mechanism to segregate Fluido and Simplus BE in 4Cast360 system at the moment, only ECAS Standalone and Salesforce BE details are provided. We shall revisit this once there is a mechanism to segregate Fluido and Simplus. </i> <br /><br />" +
                   "<br /><br />" +
                   table_SOH_BE_REV +
                   "<i>** Salesforce incl Fluido and Simplus </i> <br/>" +
                   "<i>*** In the absence of a mechanism to segregate Fluido and Simplus BE in 4Cast360 system at the moment, only ECAS Standalone and Salesforce BE details are provided. We shall revisit this once there is a mechanism to segregate Fluido and Simplus. </i> <br /><br />" +
                   "<br /><br />" +
                   table_SOH_BE_VOL +
                   "<i>** Salesforce incl Fluido and Simplus </i> <br/>" +
                   "<i>*** In the absence of a mechanism to segregate Fluido and Simplus BE in 4Cast360 system at the moment, only ECAS Standalone and Salesforce BE details are provided. We shall revisit this once there is a mechanism to segregate Fluido and Simplus. </i> <br /><br />" +
                   "<br /><br />" +

                   table_ProjectStatus +
                   "<i>*ECAS - Excl Fluido and Simplus</i> <br/>" +
                   "<br /><br />" +
                   table_AlconConfirmation +
                   "<i>*ECAS - Excl Fluido and Simplus</i> <br/>" +
                   "<br /><br />" +
                    table_MCCwithoutSDM +
                   "<br /><br />" +
                   table_MissingMCC +
                      "<br /><br />" +
                   table_MissingRoleMapping +
                     "<br /><br />" +
                   table_InvalidSDM +
                    "<br /><br />" +
                   table_nonEAS_BEusers +
                    "<br /><br />" +
                   table_nonEAS_NebulaUsers +
                      "<br /><br />" +
                   footer;



            return html;




        }


        static string GetAlconPBSBESnapshot(string exrate)
        {

            DataTable dtBEDate = objDL.ExecuteSP("SP_DailyMailerBody_AlconPBS_BE_SnapshotDates");
            string returnValue = "";
            if (dtBEDate != null && dtBEDate.Rows.Count == 1)
            {
                string format = "SoH BE as per " + exrate + " Snapshots (4Cast360): Latest - {0} , Previous - {1}";
                string latest = Convert.ToDateTime(dtBEDate.Rows[0]["Latest"]).ToString("dd-MMM-yyyy");
                string prev = Convert.ToDateTime(dtBEDate.Rows[0]["Prev"]).ToString("dd-MMM-yyyy");
                returnValue = string.Format(format, latest, prev);

            }
            return returnValue;


        }

        static string GetRTBRSnapshot(string exrate)
        {

            DataTable dtBEDate = objDL.ExecuteSP("SP_DailyMailerBody_RTBR_SnapshotDates");
            string returnValue = "";
            if (dtBEDate != null && dtBEDate.Rows.Count == 1)
            {
                string format = "*** RTBR as per " + exrate + " snapshot between {0} and {1} are compared.";
                string latest = Convert.ToDateTime(dtBEDate.Rows[0]["Latest"]).ToString("dd-MMM-yyyy");
                string prev = Convert.ToDateTime(dtBEDate.Rows[0]["Prev"]).ToString("dd-MMM-yyyy");
                returnValue = string.Format(format, latest, prev);

            }
            return returnValue;


        }

        static string GetAdjRTBRSnapshot(string exrate)
        {

            DataTable dtBEDate = objDL.ExecuteSP("SP_DailyMailerBody_AdjRTBR_SnapshotDates");
            string returnValue = "";
            if (dtBEDate != null && dtBEDate.Rows.Count == 1)
            {
                if ((dtBEDate.Rows[0]["Latest"] + "").Trim() =="" || (dtBEDate.Rows[0]["Prev"] + "").Trim() == "")
                    returnValue = "";
                else
                {

                    string format = "** Adj. RTBR as per " + exrate + " snapshot between {0} and {1} are compared.";
                    string latest = Convert.ToDateTime(dtBEDate.Rows[0]["Latest"]).ToString("dd-MMM-yyyy");
                    string prev = Convert.ToDateTime(dtBEDate.Rows[0]["Prev"]).ToString("dd-MMM-yyyy");
                    returnValue = string.Format(format, latest, prev);
                }

            }
            return returnValue;


        }

        static string GenerateTable_Type_Util(DataTable dtdate, DataTable dt, string title, string bottomnote, string headerCols)
        {
            string table = ""; string body = ""; string foot = "";
            string tableTitle = @" <b class='subtitle'>" + title + "</b><br /><br />";
            string tableFormat = @" <table class='courses-table'>      <thead>     <tr >  {0} </tr>   </thead>  <tbody>";
            string headerFormat = @" <th align='center'  class='Headings'>{0}</th>";

            string headerFormat1 = @" <th align='center'  class='Headings_Util1'>{0}</th>";
            string headerFormat2 = @" <th align='center'  class='Headings_Util2'>{0}</th>";
            string headerFormat3 = @" <th align='center'  class='Headings_Util3'>{0}</th>";
            //string headerFormat1 = @" <th align='center'  style='width: 80px;' class='Headings'>{0}</th>";

            string html;

            if (dt.Rows.Count == 0)
            {
                html = " <b class='subtitle'>" + title + "</b> <br /> <span>No Data Exists</span>";
                return html;
            }





            headerCols += "<tr>";

            for (int i = 0; i < dtdate.Rows.Count; i++)
            {
                var colName = dtdate.Rows[i][0] + "";
                if (i == 0)
                    headerCols += string.Format(headerFormat, colName);
                if (i > 0 && i < 8)
                    headerCols += string.Format(headerFormat1, colName);
                if (i >= 8 && i <= 14)
                    headerCols += string.Format(headerFormat2, colName);
                if (i > 14)
                    headerCols += string.Format(headerFormat3, colName);
            }

            headerCols += "</tr>";



            table = string.Format(tableFormat, headerCols);

            foreach (DataRow row in dt.Rows)
            {
                body += "<tr>";

                foreach (DataColumn col in dt.Columns)
                {
                    if (title.Contains("%"))
                        body += @"<td class='custom'>" + FormatValue_Util_1(row[col].ToString()) + "</td>";
                    else
                        body += @"<td class='custom'>" + FormatValue_Util_2_3(row[col].ToString()) + "</td>";
                }
                body += "</tr>";
            }

            foot = @"  </tbody> </table>  <br />   ";
            //   foot += bottomnote + " <br /> ";

            html = tableTitle + table + body + foot;
            return html;
        }

        static string GenerateTable_Type1(DataTable dt, string title, string exchangeRate)
        {
            string header_top = "";
            string header = "";
            string table = "";
            string body = "";
            string foot = "";
            string tableTitle = @" <b class='subtitle'>" + title + "</b> <br /><br />";
            string tableFormat = @" <table class='courses-table'>      <thead>     <tr >  {0} </tr>   </thead>  <tbody>";
            string headerFormat = @" <th align='center'  class='Headings'>{0}</th>";
            //string headerFormat1 = @" <th align='center'  style='width: 80px;' class='Headings'>{0}</th>";

            string html;

            if (dt.Rows.Count == 0)
            {
                html = tableTitle + "<br /><span>No Data Exists</span>";
                return html;
            }


            string headerCols = "";

            if (title == "Current Qtr SOH BE as of today")
            {
                headerCols += "<tr>";
                headerCols += "<th class='Headings'></th><th colspan='4' align='center'  class='Headings'>Volume BE</th><th colspan='4' align='center'  class='Headings'>Revenue BE ( 31st Mar’ Fx)</th>";
                headerCols += "</tr><tr>";
            }

            if (title == "Alcon PBS BE(in PersonMonths)")
            {
                headerCols += "<tr>";
                headerCols += "<th class='Headings'></th><th class='Headings'></th>";
                var dt_Month = objDL.ExecuteSP("SP_DailyMailerBody_AlconPBS_GetMonths");
                foreach (DataRow row in dt_Month.Rows)
                {
                    headerCols += "<th colspan='6' align='center'  class='Headings'>" + row[0].ToString().Trim() + "</th>";
                }
                headerCols += "</tr>";
            }

            if (title == "BE Vs RTBR (KUSD) as per " + exchangeRate + " exchange rates")
            {
                headerCols += "<tr>";
                headerCols += "<th class='Headings'></th><th class='Headings'></th>";
                var dt_Quarters = objDL.ExecuteSP("SP_DailyMailerBody_BE_RTBR_GetQuarters");
                foreach (DataRow row in dt_Quarters.Rows)
                {
                    headerCols += "<th colspan='3' align='center'  class='Headings'>" + row[0].ToString().Trim() + "</th>";
                }
                headerCols += "</tr>";
            }

            headerCols += "<tr>";

            if (title.Contains("SOH BE as of today"))
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    var colName = dt.Columns[i].ColumnName.Replace("_1", "");
                    headerCols += string.Format(headerFormat, colName);


                }
            }
            else if (title == "BE Vs RTBR (KUSD) as per " + exchangeRate + " exchange rates")
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var colName = dt.Columns[i].ColumnName.Replace("1", "");
                    headerCols += string.Format(headerFormat, colName);
                }
            }

            else
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var colName = dt.Columns[i].ColumnName.Replace("_1", "").Replace("_2", "");
                    headerCols += string.Format(headerFormat, colName);
                }
            }


            headerCols += "</tr>";



            table = string.Format(tableFormat, headerCols);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                body += "<tr>";

                foreach (DataColumn col in dt.Columns)
                {

                    if (col.ColumnName.ToLower().Contains("loaded on") && (title != "Alcon PBS BE(in PersonMonths)") && (title != "Delayed Billing(in KUSD)"))
                    {
                        DateTime date_ = Convert.ToDateTime(row[col.ColumnName].ToString());
                        if (i % 5 == 0)
                        {
                            if (date_.Hour == 0 && date_.Minute == 0 && date_.Second == 0)
                                body += @"<td rowspan='" + 5 + "' class='custom'>" + String.Format("{0:d-MMM-yyyy}", date_) + "</td>";
                            else
                                body += @"<td rowspan='" + 5 + "' class='custom'>" + String.Format("{0:d-MMM-yyyy hh:mm tt}", date_) + "</td>";
                        }
                    }
                    else if (col.ColumnName.ToLower().Contains("loaded on") && (title == "Alcon PBS BE(in PersonMonths)"))
                    {
                        DateTime date_ = Convert.ToDateTime(row[col.ColumnName].ToString());
                        if (i % 7 == 0)
                        {
                            if (date_.Hour == 0 && date_.Minute == 0 && date_.Second == 0)
                                body += @"<td rowspan='" + 7 + "' class='custom'>" + String.Format("{0:d-MMM-yyyy}", date_) + "</td>";
                            else
                                body += @"<td rowspan='" + 7 + "' class='custom'>" + String.Format("{0:d-MMM-yyyy hh:mm tt}", date_) + "</td>";
                        }
                    }
                    else if (col.ColumnName.ToLower().Contains("loaded on") && (title == "Delayed Billing(in KUSD)"))
                    {
                        DateTime date_ = Convert.ToDateTime(row[col.ColumnName].ToString());
                        if (i % 7 == 0)
                        {
                            if (date_.Hour == 0 && date_.Minute == 0 && date_.Second == 0)
                                body += @"<td rowspan='" + 7 + "' class='custom'>" + String.Format("{0:d-MMM-yyyy}", date_) + "</td>";
                            else
                                body += @"<td rowspan='" + 7 + "' class='custom'>" + String.Format("{0:d-MMM-yyyy hh:mm tt}", date_) + "</td>";
                        }
                    }
                    else
                        body += @"<td  class='custom'>" + FormatValue(row[col].ToString()) + "</td>";
                }
                body += "</tr>";
            }

            foot = @"  </tbody> </table>  <br />   ";


            html = tableTitle + table + body + foot;

            return html;
        }

        static string GenerateTable_Type1_New_Biz(DataTable dt, string title, string exchangeRate)
        {
            _count4 = 0;
            string header_top = "";
            string header = "";
            string table = "";
            string body = "";
            string foot = "";
            string tableTitle = @" <b class='subtitle'>" + title + "</b> <br /><br />";
            string tableFormat = @" <table class='courses-table'>      <thead>     <tr >  {0} </tr>   </thead>  <tbody>";
            string headerFormat = @" <th align='center'  class='Headings'>{0}</th>";
            //string headerFormat1 = @" <th align='center'  style='width: 80px;' class='Headings'>{0}</th>";

            string html;

            if (dt.Rows.Count == 0)
            {
                html = tableTitle + "<br /><span>No Data Exists</span>";
                return html;
            }


            string headerCols = "";

            if (title == "Current Qtr SOH BE as of today")
            {
                headerCols += "<tr>";
                headerCols += "<th class='Headings'></th><th colspan='4' align='center'  class='Headings'>Volume BE</th><th colspan='4' align='center'  class='Headings'>Revenue BE ( 31st Mar’ Fx)</th>";
                headerCols += "</tr><tr>";
            }
            if (title == "SOH New Biz Revenue")
            {
                //headerCols += "<tr>";
                headerCols += "<th class='Headings'></th>";
                var dt_Quarters = objDL.ExecuteSP("SP_DailyMailerBody_SOH_BE_Trend_GetQuarters");
                foreach (DataRow row in dt_Quarters.Rows)
                {
                    headerCols += "<th colspan='3' align='center'  class='Headings'>" + row[0].ToString().Trim() + "</th>";
                }
                headerCols += "</tr><tr>";
            }
            if (title == "SOH New Biz Volume")
            {
                headerCols += "<tr>";
                headerCols += "<th class='Headings'></th>";
                var dt_Quarters = objDL.ExecuteSP("SP_DailyMailerBody_SOH_BE_Trend_GetQuarters");
                foreach (DataRow row in dt_Quarters.Rows)
                {
                    headerCols += "<th colspan='3' align='center'  class='Headings'>" + row[0].ToString().Trim() + "</th>";
                }
                headerCols += "</tr><tr>";
            }



            if (title == "Alcon PBS BE(in PersonMonths)")
            {
                headerCols += "<tr>";
                headerCols += "<th class='Headings'></th><th class='Headings'></th>";
                var dt_Month = objDL.ExecuteSP("SP_DailyMailerBody_AlconPBS_GetMonths");
                foreach (DataRow row in dt_Month.Rows)
                {
                    headerCols += "<th rowspan='7' align='center'  class='Headings'>" + row[0].ToString().Trim() + "</th>";
                }
                headerCols += "</tr>";
            }

            if (title == "BE Vs RTBR (KUSD) as per " + exchangeRate + " exchange rates")
            {
                headerCols += "<tr>";
                headerCols += "<th class='Headings'></th><th class='Headings'></th>";
                var dt_Quarters = objDL.ExecuteSP("SP_DailyMailerBody_BE_RTBR_GetQuarters");
                foreach (DataRow row in dt_Quarters.Rows)
                {
                    headerCols += "<th colspan='3' align='center'  class='Headings'>" + row[0].ToString().Trim() + "</th>";
                }
                headerCols += "</tr>";
            }

            headerCols += "<tr>";

            if (title.Contains("SOH BE as of today") )
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    var colName = dt.Columns[i].ColumnName.Replace("_1", "");
                    headerCols += string.Format(headerFormat, colName);


                }
            }
            else if (title == "BE Vs RTBR (KUSD) as per " + exchangeRate + " exchange rates")
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var colName = dt.Columns[i].ColumnName.Replace("1", "");
                    headerCols += string.Format(headerFormat, colName);
                }
            }
            else if (title == "SOH New Biz Volume" || title == "Subcon on Non-Prod")
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var colName = dt.Columns[i].ColumnName.Replace("1", "");
                    headerCols += string.Format(headerFormat, colName);
                }
            }
            else if (title == "SOH New Biz Revenue")
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var colName = dt.Columns[i].ColumnName.Replace("1", "");
                    headerCols += string.Format(headerFormat, colName);
                }
            }

            else
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var colName = dt.Columns[i].ColumnName.Replace("_1", "").Replace("_2", "");
                    headerCols += string.Format(headerFormat, colName);
                }
            }


            headerCols += "</tr>";



            table = string.Format(tableFormat, headerCols);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                body += "<tr>";

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.ToLower().Contains("loaded on"))
                    {
                        DateTime date_ = Convert.ToDateTime(row[col.ColumnName].ToString());
                        if (i % 5 == 0)
                        {
                            if (date_.Hour == 0 && date_.Minute == 0 && date_.Second == 0)
                                body += @"<td rowspan='" + 5 + "' class='custom'>" + String.Format("{0:d-MMM-yyyy}", date_) + "</td>";
                            else
                                body += @"<td rowspan='" + 5 + "' class='custom'>" + String.Format("{0:d-MMM-yyyy hh:mm tt}", date_) + "</td>";
                        }
                    }
                    else
                        body += @"<td class='custom'>" + FormatValue_New_Biz(row[col].ToString()) + "</td>";
                }
                body += "</tr>";
            }

            foot = @"  </tbody> </table>  <br />   ";


            html = tableTitle + table + body + foot;

            return html;
        }

        static string SOH_BE_Trend(DataTable dt, string title, string exchangerate)
        {
            string header_top = "";
            string header = "";
            string table = "";
            string body = "";
            string foot = "";
            string tableTitle = @" <b class='subtitle'>" + title + "</b> <br /><br />";
            string tableFormat = @" <table class='courses-table'>      <thead>     <tr >  {0} </tr>   </thead>  <tbody>";
            string headerFormat = @" <th align='center'  class='Heading_SOH'>{0}</th>";
            //string headerFormat1 = @" <th align='center'  style='width: 80px;' class='Headings'>{0}</th>";

            string html;

            if (dt.Rows.Count == 0)
            {
                html = tableTitle + "<br /><span>No Data Exists</span>";
                return html;
            }
            DataTable dt_2 = new DataTable();

            string headerCols = "";


            if (title == "SOH BE Trend - Revenue as per " + exchangerate + " exchange rates" || title == "SOH BE Trend - Volume")
            {

                headerCols += "<tr>";
                headerCols += "<th class='Headings'></th>";
                int count_Col = dt.Columns.Count - 1;
                int ColSpan = count_Col / 2;
                ColSpan = ColSpan - 1;

                var dt_Quarters = objDL.ExecuteSP("SP_DailyMailerBody_SOH_BE_Trend_GetQuarters");
                foreach (DataRow row in dt_Quarters.Rows)
                {
                    headerCols += "<th colspan='" + ColSpan.ToString() + "' align='center'  class='Headings'>" + row[0].ToString().Trim() + "</th>";
                }

                headerCols += "</tr>";
            }

            if (title == "SOH BE Trend - Revenue as per " + exchangerate + " exchange rates" || title == "SOH BE Trend - Volume")
            {

                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    var colName = dt.Columns[i].ColumnName;
                    //headerCols += string.Format(headerFormat, colName);
                    dt_2.Columns.Add(colName, typeof(string));

                }

                foreach (DataRow row in dt.Rows)
                {
                    dt_2.ImportRow(row);
                }

                dt_2.AcceptChanges();
            }

            if (title == "SOH BE Trend - Revenue as per " + exchangerate + " exchange rates" || title == "SOH BE Trend - Volume")
            {
                for (int i = 0; i < dt_2.Rows.Count; i++)
                {
                    var row = dt_2.Rows[i];


                    for (int k = 0; k < dt.Columns.Count - 1; k++)
                    {

                        int l_count = dt.Columns.Count;
                        if (k != 0 && k != l_count)
                        {
                            //int Two = Convert.ToInt32(dt.Rows[i][k + 1].ToString());
                            //int One = Convert.ToInt32(dt.Rows[i][k].ToString());

                            //string col_val = Convert.ToString(One - Two);
                            //string _Vl = FormatValue(One.ToString());
                            //_Vl = _Vl + " (" + col_val + ")";
                            //dt_2.Rows[i][k] = _Vl;

                            decimal Two = Convert.ToDecimal(dt.Rows[i][k + 1].ToString());
                            decimal One = Convert.ToDecimal(dt.Rows[i][k].ToString());

                            Decimal value = Decimal.Subtract(One, Two);
                            Double roundvalue = Convert.ToDouble(One);
                            Double roundvalue2 = Convert.ToDouble(value);
                            
                            string _Vl = "";
                            _Vl = FormatValue( Math.Round(roundvalue).ToString()) + " (" + FormatValue(Math.Round(roundvalue2).ToString()) + ")";
                            dt_2.Rows[i][k] = _Vl;
                        }
                        else
                        {
                            dt_2.Rows[i][k] = dt.Rows[i][k].ToString();
                            //body += @"<td class='custom'>" + FormatValue(dt_2.Rows[i][k].ToString()) + "</td>";
                        }

                    }

                }

                int count_excl = dt_2.Columns.Count - 1;
                int excl_first = count_excl / 2;


                dt_2.Columns.RemoveAt(excl_first);
                dt_2.AcceptChanges();
                dt_2.Columns.RemoveAt(excl_first + excl_first - 1);
                dt_2.AcceptChanges();

                //headerCols += "<tr>";
                for (int i = 0; i < dt_2.Columns.Count; i++)
                {

                    var colName = dt_2.Columns[i].ColumnName.Replace(" 12:00AM1", "").Replace(" 12:00AM", "");
                    if (colName == "txtsu")
                    {
                        colName = "SL";
                        headerCols += string.Format(headerFormat, colName);
                    }
                    else
                    {
                        string Col_Header = Convert.ToDateTime(colName).ToString("dd-MMM-yyyy");
                        headerCols += string.Format(headerFormat, Col_Header);
                    }

                }

                headerCols += "</tr>";
                table = string.Format(tableFormat, headerCols);

                for (int i = 0; i < dt_2.Rows.Count; i++)
                {
                    var row = dt_2.Rows[i];
                    body += "<tr>";

                    for (int k = 0; k < dt_2.Columns.Count; k++)
                    {

                        body += @"<td class='custom'>" + dt_2.Rows[i][k].ToString() + "</td>";

                    }

                    body += "</tr>";
                }

            }

            foot = @"  </tbody> </table>  <br />   ";


            html = tableTitle + table + body + foot;

            return html;
        }

        static string GenerateTable_Type2(DataTable dt, string title)
        {
            string table = ""; string body = ""; string foot = "";
            string tableTitle = @" <b class='subtitle'>" + title + "</b><br /><br />";
            string tableFormat = @" <table class='courses-table'>      <thead>     <tr >  {0} </tr>   </thead>  <tbody>";
            string headerFormat = @" <th align='center'  class='Headings'>{0}</th>";
            //string headerFormat1 = @" <th align='center'  style='width: 80px;' class='Headings'>{0}</th>";

            string html;

            if (dt.Rows.Count == 0)
            {
                html = " <b class='subtitle'>" + title + "</b> <br /> <span>No Data Exists</span>";
                return html;
            }

            string headerCols = "";
            //headerCols = "<tr><th></th><th></th><th class ='Headings'  colspan='6'>Weekly</th><th></th> <th class ='Headings'  colspan='3'>Monthly</th><th></th><th class ='Headings'  colspan='2'>Quarterly</th> </tr>";

            if (title.Contains("Uninitiated Alcon Confirmation"))
            {
                headerCols += "<tr>";
                headerCols += "<th class='Headings'></th><th colspan='4' align='center'  class='Headings'>FP</th><th colspan='2' align='center'  class='Headings'>TnM</th>";
                headerCols += "</tr><tr>";
                headerCols += "<th class='Headings'></th><th colspan='2' align='center'  class='Headings'>Past Duedate</th><th colspan='2' align='center'  class='Headings'>Future Duedate</th><th colspan='2'  align='center'  class='Headings'>Past Duedate</th>";
                headerCols += "</tr>";
            }

            headerCols += "<tr>";

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var colName = dt.Columns[i].ColumnName.Replace("_1", "").Replace("_2", "");
                headerCols += string.Format(headerFormat, colName);
            }

            headerCols += "</tr>";



            table = string.Format(tableFormat, headerCols);

            foreach (DataRow row in dt.Rows)
            {
                body += "<tr>";

                foreach (DataColumn col in dt.Columns)
                {
                    body += @"<td class='custom'>" + FormatValue(row[col].ToString()) + "</td>";
                }
                body += "</tr>";
            }

            foot = @"  </tbody> </table>  <br />   ";

            html = tableTitle + table + body + foot;
            return html;
        }

        static string GenerateTable_Type2_lat(DataTable dt, string title)
        {
            string table = ""; string body = ""; string foot = "";
            string tableTitle = @" <b class='subtitle'>" + title + "</b><br /><br />";
            string tableFormat = @" <table class='courses-table'>      <thead>     <tr >  {0} </tr>   </thead>  <tbody>";
            string headerFormat = @" <th align='center'  class='Headings'>{0}</th>";
            //string headerFormat1 = @" <th align='center'  style='width: 80px;' class='Headings'>{0}</th>";

            string html;

            if (dt.Rows.Count == 0)
            {
                html = " <b class='subtitle'>" + title + "</b> <br /> <span>No Data Exists</span>";
                return html;
            }

            string headerCols = "";
            //headerCols = "<tr><th></th><th></th><th class ='Headings'  colspan='6'>Weekly</th><th></th> <th class ='Headings'  colspan='3'>Monthly</th><th></th><th class ='Headings'  colspan='2'>Quarterly</th> </tr>";

            if (title.Contains("Uninitiated Alcon Confirmation"))
            {
                headerCols += "<tr>";
                headerCols += "<th class='Headings'></th><th colspan='4' align='center'  class='Headings'>FP</th><th colspan='2' align='center'  class='Headings'>TnM</th>";
                headerCols += "</tr><tr>";
                headerCols += "<th class='Headings'></th><th colspan='2' align='center'  class='Headings'>Past Duedate</th><th colspan='2' align='center'  class='Headings'>Future Duedate</th><th colspan='2'  align='center'  class='Headings'>Past Duedate</th>";
                headerCols += "</tr>";
            }

            headerCols += "<tr>";

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var colName = dt.Columns[i].ColumnName.Replace("_1", "").Replace("_2", "");
                headerCols += string.Format(headerFormat, colName);
            }

            headerCols += "</tr>";



            table = string.Format(tableFormat, headerCols);

            foreach (DataRow row in dt.Rows)
            {
                body += "<tr>";

                foreach (DataColumn col in dt.Columns)
                {
                    body += @"<td class='custom'>" + FormatValue_thousand(row[col].ToString()) + "</td>";
                }
                body += "</tr>";
            }

            foot = @"  </tbody> </table>  <br />   ";

            html = tableTitle + table + body + foot;
            return html;
        }

        static string GenerateTable_Type3(DataTable dt, string title)
        {
            string table = ""; string body = ""; string foot = "";
            string tableTitle = @" <b class='subtitle'>" + title + "</b> <br /><br />";
            string tableFormat = @" <table class='courses-table'>      <thead>     <tr >  {0} </tr>   </thead>  <tbody>";
            string headerFormat = @" <th align='center'  class='Headings'>{0}</th>";
            //string headerFormat1 = @" <th align='center'  style='width: 80px;' class='Headings'>{0}</th>";

            string html;

            if (dt.Rows.Count == 0)
            {
                html = tableTitle + "<br /><span>No Data Exists</span>";
                return html;
            }

            string headerCols = "";
            //headerCols = "<tr><th></th><th></th><th class ='Headings'  colspan='6'>Weekly</th><th></th> <th class ='Headings'  colspan='3'>Monthly</th><th></th><th class ='Headings'  colspan='2'>Quarterly</th> </tr>";

            for (int i = 0; i < dt.Columns.Count; i++)
                headerCols += string.Format(headerFormat, dt.Columns[i].ColumnName);

            table = string.Format(tableFormat, headerCols);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                body += "<tr>";

                foreach (DataColumn col in dt.Columns)
                {

                    if (col.ColumnName.ToLower() == "sub unit")
                    {
                        if (i % 10 == 0)
                            body += @"<td rowspan='" + 10 + "' class='custom'>" + row[col.ColumnName] + "</td>";

                    }
                    else if (col.ColumnName.ToLower().Trim() == "top 5 variance")
                    {
                        if (i % 5 == 0)
                            body += @"<td rowspan='" + 5 + "' class='custom'>" + row[col.ColumnName] + "</td>";

                    }
                    else
                        body += @"<td class='custom'>" + FormatValue(row[col].ToString()) + "</td>";
                }
                body += "</tr>";
            }

            foot = @"  </tbody> </table>  <br />   ";

            html = tableTitle + table + body + foot;

            return html;
        }

        static string GenerateTable_Type3_lat(DataTable dt, string title)
        {
            string table = ""; string body = ""; string foot = "";
            string tableTitle = @" <b class='subtitle'>" + title + "</b> <br /><br />";
            string tableFormat = @" <table class='courses-table'>      <thead>     <tr >  {0} </tr>   </thead>  <tbody>";
            string headerFormat = @" <th align='center'  class='Headings'>{0}</th>";
            //string headerFormat1 = @" <th align='center'  style='width: 80px;' class='Headings'>{0}</th>";

            string html;

            if (dt.Rows.Count == 0)
            {
                html = tableTitle + "<br /><span>No Data Exists</span>";
                return html;
            }

            string headerCols = "";
            //headerCols = "<tr><th></th><th></th><th class ='Headings'  colspan='6'>Weekly</th><th></th> <th class ='Headings'  colspan='3'>Monthly</th><th></th><th class ='Headings'  colspan='2'>Quarterly</th> </tr>";

            for (int i = 0; i < dt.Columns.Count; i++)
                headerCols += string.Format(headerFormat, dt.Columns[i].ColumnName);

            table = string.Format(tableFormat, headerCols);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                body += "<tr>";

                foreach (DataColumn col in dt.Columns)
                {

                    if (col.ColumnName.ToLower() == "sub unit")
                    {
                        if (i % 10 == 0)
                            body += @"<td rowspan='" + 10 + "' class='custom'>" + row[col.ColumnName] + "</td>";

                    }
                    else if (col.ColumnName.ToLower().Trim() == "top 5 variance")
                    {
                        if (i % 5 == 0)
                            body += @"<td rowspan='" + 5 + "' class='custom'>" + row[col.ColumnName] + "</td>";

                    }
                    else
                        body += @"<td class='custom'>" + FormatValue_thousand(row[col].ToString()) + "</td>";
                }
                body += "</tr>";
            }

            foot = @"  </tbody> </table>  <br />   ";

            html = tableTitle + table + body + foot;

            return html;
        }

        static string FormatValue_Util_1(string str)
        {

            if (str.Trim().Contains('(') && str.Trim().Contains('-'))
            {
                var value = str.Split('(');
                var left = value[0];
                var right = value[1].Replace(')', ' ').Trim();


                string html = left + "(<span class='redcolor'>" + right + "</span>" + ")";
                return html;
            }
            else
                return str;

        }

        static string FormatValue_Util_2_3(string str)
        {
            //int test;
            //if (int.TryParse(str, out test))
            //    return str;


            if (str.Trim().Contains('('))
            {
                var value = str.Split('(');
                var left = value[0];
                int num2;
                int.TryParse(left, out num2);
                var str_left = string.Format(String.Format("{0:N0}", num2));
                var right = value[1].Replace(')', ' ').Trim();


                string html = "";
                if (str.Trim().Contains('-'))
                    html = str_left + "(<span class='redcolor'>" + right + "</span>" + ")";
                else
                    html = str_left + "(" + right + ")";

                return html;
            }
            else
            {
                int num2;
                if (int.TryParse(str, out num2))
                    return string.Format(String.Format("{0:N0}", num2));
                else return str;
            }

        }



        static string FormatValue(string str)
        {



            string str_final = str;

            if (str.ToString().Contains('~'))
            {
                var lst_num = str.ToString().Trim().Split('~');
                int num1;
                int num2;
                int.TryParse(lst_num[0], out num1);
                int.TryParse(lst_num[1], out num2);

                var str_num1 = string.Format(String.Format("{0:N0}", num1));
                var str_num2 = string.Format(String.Format("{0:N0}", num2));

                if (num1 < 0)
                    str_num1 = "<span class='redcolor'>" + str_num1 + "</span>";
                if (num2 < 0)
                    str_num2 = "<span class='redcolor'>" + str_num2 + "</span>";

                if (str_num1 == "0" && str_num2 == "0")
                    str_final = "0";
                else
                    str_final = str_num1 + "(" + str_num2 + ")";


            }
            else
            {

                int num_;

                if (str.Trim().Contains('.'))
                {
                    double num;
                    if (double.TryParse(str.ToString(), out num))
                    {
                        //str_final = string.Format(String.Format("{0:G29}", decimal.Parse(num.ToString())));
                        //str_final = num.ToString("#,0.##");
                        str_final = String.Format("{0:0.0}", num);

                        if (num < 0)
                            str_final = "<span class='redcolor'>" + str_final + "</span>";
                    }
                }
                else if (int.TryParse(str, out num_))
                {
                    str_final = string.Format(String.Format("{0:N0}", num_));
                    if (str_final.Contains('-'))
                        str_final = "<span class='redcolor'>" + str_final + "</span>";
                }
                else
                    str_final = str;
            }
            return str_final;
        }

        static string FormatValue_thousand(string str)
        {



            string str_final = str;

            if (str.ToString().Contains('~'))
            {
                var lst_num = str.ToString().Trim().Split('~');
                int num1;
                int num2;
                int.TryParse(lst_num[0], out num1);
                int.TryParse(lst_num[1], out num2);

                var str_num1 = string.Format(String.Format("{0:N0}", num1));
                var str_num2 = string.Format(String.Format("{0:N0}", num2));

                if (num1 < 0)
                    str_num1 = "<span class='redcolor'>" + str_num1 + "</span>";
                if (num2 < 0)
                    str_num2 = "<span class='redcolor'>" + str_num2 + "</span>";

                if (str_num1 == "0" && str_num2 == "0")
                    str_final = "0";
                else
                    str_final = str_num1 + "(" + str_num2 + ")";


            }
            else
            {

                int num_;

                if (str.Trim().Contains('.'))
                {
                    double num;
                    if (double.TryParse(str.ToString(), out num))
                    {
                        //str_final = string.Format(String.Format("{0:G29}", decimal.Parse(num.ToString())));
                        str_final = num.ToString("#,0.##");
                        //str_final = String.Format("{0:0.00}", num);

                        if (num < 0)
                            str_final = "<span class='redcolor'>" + str_final + "</span>";
                    }
                }
                else if (int.TryParse(str, out num_))
                {
                    str_final = string.Format(String.Format("{0:N0}", num_));
                    if (str_final.Contains('-'))
                        str_final = "<span class='redcolor'>" + str_final + "</span>";
                }
                else
                    str_final = str;
            }
            return str_final;
        }

        static int _count4 = 0;
        static string FormatValue_New_Biz(string str)
        {

            string str_final = str;
            _count4++;
            if (str.ToString().Contains('~'))
            {
                var lst_num = str.ToString().Trim().Split('~');
                decimal num1;
                decimal num2;
                decimal.TryParse(lst_num[0], out num1);
                decimal.TryParse(lst_num[1], out num2);

                var str_num1 = num1.ToString();// string.Format(String.Format("{0:N0}", num1));
                var str_num2 = num2.ToString();// string.Format(String.Format("{0:N0}", num2));

                if (num1 < 0)
                    str_num1 = "<span class='redcolor'>" + str_num1 + "</span>";
                if (num2 < 0)
                    str_num2 = "<span class='redcolor'>" + str_num2 + "</span>";

                //if (str_num1 == "0" && str_num2 == "0")
                //    str_final = "0";
                //else
                //{
                //if(_count4 ==4 || _count4==8 || _count4== 12 ||_count4 == 16 || _count4 == 20)
                //    str_final = str_num1 + "(" + str_num2 + ")";
                //else
                        str_final = FormatValue(str_num1) + "(" + str_num2 + ")";
                //}

            }
            else
            {

                int num_;

                if (str.Trim().Contains('.'))
                {
                    double num;
                    if (double.TryParse(str.ToString(), out num))
                    {
                        //str_final = string.Format(String.Format("{0:G29}", decimal.Parse(num.ToString())));
                        // str_final = num.ToString("#,0.##");
                        str_final = String.Format("{0:0.00}", num);

                        if (num < 0)
                            str_final = "<span class='redcolor'>" + str_final + "</span>";
                    }
                }
                else if (int.TryParse(str, out num_))
                {
                    str_final = string.Format(String.Format("{0:N0}", num_));
                    if (str_final.Contains('-'))
                        str_final = "<span class='redcolor'>" + str_final + "</span>";
                }
                else
                    str_final = str;
            }
            return str_final;
        }

        //static string FormatValue_SOH(string str)
        //{

        //    string str_final = str;

        //            double num;
        //            if (double.TryParse(str.ToString(), out num))
        //            {
        //                //str_final = string.Format(String.Format("{0:G29}", decimal.Parse(num.ToString())));
        //                str_final = num.ToString("#,0.##");

        //                if (num < 0)
        //                    str_final = "<span class='redcolor'>" + str_final + "</span>";
        //            }



        //    return str_final;
        //}

        protected void Button1_Click(object sender, EventArgs e)
        {
            string html = GenerateHTML();
            string mainbody = head + html;
            string folder = Server.MapPath("HtmlFile");
            string fileName = "dailymailer" + DateTime.Now.ToString("ddmmmyyyyHHMMss") + ".html";
            string path = System.IO.Path.Combine(folder, fileName);
            System.IO.File.WriteAllText(path, mainbody);
            // iframe1.Attributes.Add("src", System.IO.Path.Combine("HtmlFile", fileName));

            string str = "<script>OpenHtmlPopUp('" + fileName + "')</script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Script", str, false);

        }

    }
}