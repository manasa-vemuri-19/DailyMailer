<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Mailer.aspx.cs" Inherits="DailyMailer.Mailer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
      <style type="text/css">
         .button
        {
            border: 1px solid #196880;
             background-color:  aquamarine;
             padding: 1px 0px;
            cursor: pointer;
             cursor: hand;
             font-family: Tahoma;
             font-size: 8pt;
             margin-left: 10px;
             margin-top: 0px;
         }
          .buttonred
        {
            border: 1px solid #196880;
             background-color:  palevioletred;
             padding: 1px 0px;
            cursor: pointer;
             cursor: hand;
             font-family: Tahoma;
             font-size: 8pt;
             margin-left: 0px;
             margin-top: 0px;
         }
        .mGrid
        {
            width: 100%;
            background-color: #fff; /* margin: 5px 0 10px 0;*/
            border: solid 1px #525252;
            border-collapse: collapse;
            font-family: Tahoma;
            font-size: 8pt;
        }
        .button:hover
        {
            border-style: solid;
            background-color: #aadeee;
            border-color: #196880;
            color: Black;
            border-width: 1px;
            padding: 1px 0px;
            cursor: pointer;
            cursor: hand;
            font-family: Tahoma;
            font-size: 8pt;
        }
        .FormLabel
        {
            background-color: #f0f0ed;
            font-family: Verdana;
            color: #000000;
            font-size: 10px;
            font-weight: normal;
        }
        .FormControls
        {
            background-color: White;
            font-family: Verdana;
            color: #000000;
            font-size: 10px;
            font-weight: normal;
        }
        .TextBox
        {
            font-family: verdana;
            font-size: 8pt;
        }
    </style>
    <style type="text/css">
        
        body
        {
        	font-family:Calibri;
        }
        #tbldev td
        {
            border: 1px solid #2aadd5;
        }
        #tblProd td
        {
            border: 1px solid #b12c1a;
        }
        
        
        .FormLabel
        {
            background-color: #f0f0ed;
            font-family: Verdana;
            color: #000000;
            font-size: 10px;
            font-weight: normal;
        }
        .FormControls
        {
            background-color: White;
            font-family: Verdana;
            color: #000000;
            font-size: 10px;
            font-weight: normal;
        }
        .style4
        {
            width: 35%;
        }
          .style2
        {
            color: #FF0000;
            font-family: Tahoma;
            font-size: x-small;
        }
        .style8
        {
            height: 145px;
        }
        .auto-style1 {
            height: 28px;
        }
        .auto-style2 {
            font-family: Verdana;
            color: #000000;
            font-size: 10px;
            font-weight: normal;
            height: 28px;
            background-color: White;
        }
        #iframe1 {
            width: 100%;
            height: 500px;
         
        }
    </style>
    <script>

 function SendClose() {

            document.getElementById('img1').style.display = "none";
            alert("Sent Successfully");

        }

        function showGIF() {
            setTimeout(function () {
                document.getElementById('img1').style.display = "inline-block";
            }, 50);
        }
    </script>

    <script>
         function OpenHtmlPopUp(page) {

            var domain = window.location.origin;
            var url = domain + "/HtmlFile/" + page;

            if (domain == 'http://nebula')
                url = domain + "/dailymailer" + "/HtmlFile/" + page;
            window.open(url, "_blank", "toolbar=yes,scrollbars=yes,resizable=yes,top=50,left=50,width=1000,height=700");
        }
</script>

</head>
<body>
    <form id="form1" runat="server">
     
 <h1 style="padding-left: 30px;font-family: 'Segoe UI'">Daily Mailer</h1>
        <div id="divNotify" style="font-family: 'Segoe UI'; font-size:20px; border:1px solid crimson; width:800px; margin-left:15px">
        &nbsp;    Last Mailer Sent by <b style="color: green" id="bSentBy" runat="server">xxxxxxx</b> on <b style="color: crimson" id="bSentOn" runat="server">04-Jul-2022 03.42 PM</b>
        </div>
        <br /><br /> 
                   <div style="margin-left:50px;">
                          <asp:Button ID="Button2" runat="server" CssClass="buttonred" Text=" Verify " 
                                        Width="88px" Height="22px" OnClientClick="showGIF()" onclick="Button1_Click" />
                                   <asp:Button ID="btnmailer" runat="server" CssClass="button" Text=" Send Mailer " 
                                        Width="88px" Height="22px" OnClientClick="showGIF()" onclick="btnmailer_Click" />
                                    <img id="img1" alt="" src="Images/ajax-loader.gif" style="display: none" />
                   </div>

                         
     
                
            <br />
<div style="padding-left: 20px">
        <asp:Label ID="lblError" runat="server" ForeColor="Red" Font-Bold="false" Font-Size="Small"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" ForeColor="Green" Font-Bold="false" Font-Size="Small"></asp:Label><br />
       
    </div> 
     

        <div id="divHtml" style="align-self:auto" >





            <%--<iframe id="iframe1"  runat="server"></iframe>--%>
        </div>


    </form>
</body>
</html>
