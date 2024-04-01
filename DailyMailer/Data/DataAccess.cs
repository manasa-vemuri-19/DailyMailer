using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Data.SqlClient;
using BEData ;


public class DataAccess
{
    private string strConn;
    private SqlConnection objConn;
    //private SqlDataAdapter objDataAdapter;
    //private SqlCommand sqlCmd;
    Logger logger = new Logger();

    string fileName = "DataLayer.DataAccess";
    public DataAccess()
    {

        try
        {
           
            // strConn = "Data Source=apac-ops;Initial Catalog=DemandCaptureDev;Persist Security Info=True;User ID=WBUser;Password=cmed@123";
            strConn = System.Configuration.ConfigurationManager.AppSettings["DemandCaptureConnectionString"];
        }
        catch (Exception ex)
        {


            throw ex;
        }
    }
    public void GetConnection()
    {
        try
        {
            objConn = new SqlConnection(strConn);
            objConn.Open();
        }
        catch (Exception ex)
        {


            throw ex;
        }
    }
    public void CloseConnection()
    {
        try
        {
            if (objConn.State == ConnectionState.Open)
                objConn.Close();
        }
        catch (Exception ex)
        {


            throw ex;
        }

    }


    public DataSet ExecuteSP_Ds(string SP)
    {
        SqlCommand sqlCmd = new SqlCommand();
        sqlCmd.CommandText = SP;
        sqlCmd.CommandType = CommandType.StoredProcedure;
        GetConnection();
        sqlCmd.Connection = objConn;
        sqlCmd.CommandTimeout = int.MaxValue;
        SqlDataAdapter sqlAdapter;
        DataSet ds = new DataSet();

        try
        {

            // sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlAdapter = new SqlDataAdapter(sqlCmd);
            sqlAdapter.Fill(ds);
            sqlCmd.Dispose();
        }
        catch (Exception ex)
        {
            logger.LogErrorToServer(Logger.LoggerType.Error, fileName, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            throw ex;
        }
        finally
        {
            if (objConn != null)
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
        }
        return ds;
    }
    public void ExecuteSP(string strSPName, ref DataSet ds, SqlCommand sqlCmd)
    {
        //SqlCommand sqlCmd;
        SqlDataAdapter sqlAdapter;
        DataTable tbl = new DataTable();

        try
        {
            sqlCmd.Connection = objConn;
            sqlCmd.CommandTimeout = 500;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = strSPName;
            //foreach (SqlParameter objParam in objParamColl)
            //{
            //    sqlCmd.Parameters.Add(objParam);
            //}

            sqlAdapter = new SqlDataAdapter(sqlCmd);
            sqlAdapter.Fill(ds);
            sqlCmd.Dispose();
        }
        catch (Exception ex)
        {


            logger.LogErrorToServer(Logger.LoggerType.Error, fileName, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            throw ex;
        }
        finally
        {
            if (objConn != null)
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
        }
    }

    //Specifically for BE Trends Report
    public void ExecuteSPTrends(string strSPName, ref DataSet ds, SqlCommand sqlCmd)
    {
        //SqlCommand sqlCmd;
        SqlDataAdapter sqlAdapter;
        DataTable tbl = new DataTable();

        try
        {
            sqlCmd.Connection = objConn;
            sqlCmd.CommandTimeout = 100;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = strSPName;
            //foreach (SqlParameter objParam in objParamColl)
            //{
            //    sqlCmd.Parameters.Add(objParam);
            //}

            sqlAdapter = new SqlDataAdapter(sqlCmd);
            sqlAdapter.Fill(ds);
            sqlCmd.Dispose();
        }
        catch (Exception ex)
        {


            logger.LogErrorToServer(Logger.LoggerType.Error, fileName, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            throw ex;
        }
        finally
        {
            if (objConn != null)
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
        }
    }

    public void ExecuteSP(string strSPName, SqlCommand sqlCmd)
    {
        //SqlCommand sqlCmd;
        try
        {
            sqlCmd.Connection = objConn;
            sqlCmd.CommandTimeout = int.MaxValue;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = strSPName;
            //foreach (SqlParameter objParam in objParamColl)
            //{
            //    sqlCmd.Parameters.Add(objParam);
            //}
            sqlCmd.ExecuteNonQuery();
            sqlCmd.Dispose();
        }
        catch (Exception ex)
        {


            logger.LogErrorToServer(Logger.LoggerType.Error, fileName, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            throw ex;
        }
        finally
        {
            if (objConn != null)
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
        }
    }

    public void ExecuteSP(string strSPName, ref DataSet ds)
    {
        SqlCommand sqlCmd = new SqlCommand();
        SqlDataAdapter sqlAdapter;
        DataTable tbl = new DataTable();

        try
        {
            sqlCmd.Connection = objConn;
            sqlCmd.CommandTimeout = 30;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = strSPName;
            sqlAdapter = new SqlDataAdapter(sqlCmd);
            sqlAdapter.Fill(ds);
            sqlCmd.Dispose();
        }
        catch (Exception ex)
        {


            logger.LogErrorToServer(Logger.LoggerType.Error, fileName, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            throw ex;
        }
        finally
        {
            if (objConn != null)
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
        }
    }
    //VISA function added:
    public bool Execute_SP(SqlCommand sqlCmd)
    {
        bool returnValue = false;
        try
        {
            GetConnection();
            sqlCmd.Connection = objConn;
            sqlCmd.CommandTimeout = 30;
            sqlCmd.CommandType = CommandType.StoredProcedure;

            //foreach (SqlParameter objParam in objParamColl)
            //{
            //    sqlCmd.Parameters.Add(objParam);
            //}
            sqlCmd.ExecuteNonQuery();
            sqlCmd.Dispose();
            returnValue = true;
        }
        catch (Exception ex)
        {
            returnValue = false;
            logger.LogErrorToServer(Logger.LoggerType.Error, fileName, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex.Message, ex.StackTrace);

        }
        finally
        {
            if (objConn != null)
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
        }
        return returnValue;
    }


    public DataSet Execute_SP(string strSPName)
    {
        DataSet ds = new DataSet();
        SqlCommand sqlCmd = new SqlCommand();
        SqlDataAdapter sqlAdapter;
        DataTable tbl = new DataTable();

        try
        {
            GetConnection();
            sqlCmd.Connection = objConn;
            sqlCmd.CommandTimeout = int.MaxValue;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = strSPName;
            sqlAdapter = new SqlDataAdapter(sqlCmd);
            sqlAdapter.Fill(ds);
            sqlCmd.Dispose();
            return ds;
        }
        catch (Exception ex)
        {


            logger.LogErrorToServer(Logger.LoggerType.Error, fileName, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            throw ex;
        }
        finally
        {
            if (objConn != null)
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
        }
    }

    public DataTable ExecuteSP(SqlCommand sqlCmd)
    {
        //SqlCommand sqlCmd;
        SqlDataAdapter sqlAdapter;
        DataTable tbl = new DataTable();

        try
        {
            GetConnection();
            sqlCmd.Connection = objConn;
            sqlCmd.CommandTimeout = 30;
            // sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlAdapter = new SqlDataAdapter(sqlCmd);
            sqlAdapter.Fill(tbl);
            sqlCmd.Dispose();
        }
        catch (Exception ex)
        {
            logger.LogErrorToServer(Logger.LoggerType.Error, fileName, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            throw ex;
        }
        finally
        {
            if (objConn != null)
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
        }
        return tbl;
    }

    public DataTable ExecuteSP(string SP)
    {
        SqlCommand sqlCmd = new SqlCommand();
        sqlCmd.CommandText = SP;
        sqlCmd.CommandType = CommandType.StoredProcedure;
        GetConnection();
        sqlCmd.Connection = objConn;
        sqlCmd.CommandTimeout = int.MaxValue;
        SqlDataAdapter sqlAdapter;
        DataTable dt = new DataTable();

        try
        {
            
            // sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlAdapter = new SqlDataAdapter(sqlCmd);
            sqlAdapter.Fill(dt);
            sqlCmd.Dispose();
        }
        catch (Exception ex)
        {
            logger.LogErrorToServer(Logger.LoggerType.Error, fileName, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            throw ex;
        }
        finally
        {
            if (objConn != null)
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
        }
        return dt;
    }


    public DataTable GetDataFromQuery(string SP)
    {
        SqlCommand sqlCmd = new SqlCommand();
        sqlCmd.CommandText = SP;
        sqlCmd.CommandType = CommandType.Text;
        GetConnection();
        sqlCmd.Connection = objConn;
        sqlCmd.CommandTimeout = int.MaxValue;
        SqlDataAdapter sqlAdapter;
        DataTable dt = new DataTable();

        try
        {
           
            // sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlAdapter = new SqlDataAdapter(sqlCmd);
            sqlAdapter.Fill(dt);
            sqlCmd.Dispose();
        }
        catch (Exception ex)
        {
            logger.LogErrorToServer(Logger.LoggerType.Error, fileName, System.Reflection.MethodInfo.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            throw ex;
        }
        finally
        {
            if (objConn != null)
                if (objConn.State == ConnectionState.Open)
                    objConn.Close();
        }
        return dt;
    }
    




}

