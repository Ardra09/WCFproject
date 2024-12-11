using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class Service : IService
{
	
    public string Processpayment(string userId, decimal amountToPay)
    {
        string result;

        try
        {
            using (SqlConnection con = new SqlConnection(@"server=DESKTOP-DID7AQD\SQLEXPRESS;database=ProjectDB;Integrated Security=true"))
            {
                con.Open();

                // Get the user's current balance
                decimal balance = GetBalance(con, userId);

                if (balance >= amountToPay)
                {
                    decimal newBalance = balance - amountToPay;

                    // Update the balance
                    bool isUpdated = UpdateBalance(con, userId, newBalance);

                    result = isUpdated ? "Payment successful, balance updated." : "Error updating balance.";
                }
                else
                {
                    result = "Insufficient balance.";
                }

                con.Close(); 
            }
        }
        catch (Exception ex)
        {
            result = "An error occurred during payment processing. Please try again later.";
            
        }

        return result;
    }

    private decimal GetBalance(SqlConnection con, string userId)
    {
        string query = "SELECT Bal_Amnt FROM Acnt_tab WHERE User_Id = @userId";

        try
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.NVarChar) { Value = userId });
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving balance: " + ex.Message, ex);
        }
    }
    private bool UpdateBalance(SqlConnection con, string userId, decimal newBalance)
    {
        string query = "UPDATE Acnt_tab SET Bal_Amnt = @newBalance WHERE User_Id = @userId";

        try
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add(new SqlParameter("@newBalance", SqlDbType.Decimal) { Value = newBalance });
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.NVarChar) { Value = userId });
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0; // If rows are affected, it means the update was successful
            }
        }
        catch (SqlException ex)
        {
            throw new Exception("An error occurred while updating the balance.", ex); 
        }
    }
public string GetData(int value)
	{
		return string.Format("You entered: {0}", value);
	}

	public CompositeType GetDataUsingDataContract(CompositeType composite)
	{
		if (composite == null)
		{
			throw new ArgumentNullException("composite");
		}
		if (composite.BoolValue)
		{
			composite.StringValue += "Suffix";
		}
		return composite;
	}
}
