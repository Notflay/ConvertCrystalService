using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STR_GP_CONVERT_DOCUMENT
{
    public class Query
    {
        HanaConnection hanaConnection;
        HanaCommand cmd;
        HanaCommand cmds;
        HanaCommand cmdss;
        HanaCommand cmdsss;
        HanaDataReader hdr;
        HanaDataReader hdrs;
        HanaDataReader hdrss;
        HanaDataReader hdrsss;
        public string pathDest;
        //string coneecion = null; 
        public Query()
        {
            //coneecion = ConfigurationManager.ConnectionStrings["hana"].ConnectionString; 
            hanaConnection = new HanaConnection(ConfigurationManager.ConnectionStrings["hana"].ConnectionString);

        }

        public bool ValidationQ()
        {
            string dia = ConfigurationManager.AppSettings["dia"];
            string query = $"SELECT \"WizardName\" FROM OPWZ WHERE DAYS_BETWEEN(\"PmntDate\",NOW()) < {dia}";

            pathDest = ConfigurationManager.AppSettings["rutaDestino"];
            if (!pathDest.EndsWith("\\")) 
                pathDest = pathDest + "\\";
            try
            {
                hanaConnection.Open();
                cmd = new HanaCommand(query, hanaConnection);
                hdr = cmd.ExecuteReader();
                if (hdr.HasRows)
                {
                    while (hdr.Read())
                    {
                        dynamic a = hdr.GetString(0);
                        string path = pathDest + hdr.GetString(0).ToString();
                        if (!File.Exists(path + ".txt"))
                            Service1.wizardNames.Add(hdr.GetString(0).ToString());
                    }
                }

                hanaConnection.Close();

                if (Service1.wizardNames.Count > 0)
                
                    return true;
                

                return false;
                WriteToFile("No hay pagos en" + dia);
            }
            catch (Exception e)
            {
                WriteToFile(e.Message);
                if (hanaConnection.State == System.Data.ConnectionState.Open)
                    hanaConnection.Close();
                return false;
            }
        }

        public void Ejecuta(string wizzad)
        {

            List<string> datas = new List<string>();

            List<string> querys = new List<string>();

            querys.Add("STR_PAGOSMASIVOSBCP_CV2_MacroBCP");
            querys.Add("STR_PAGOSMASIVOSBCP_DAV2_MACROBCPV1");

            foreach (var q in querys)
            {
                try
                {

                    hanaConnection.Open();


                    string query = $"CALL {q}('{wizzad}')";
                    cmd = new HanaCommand(query, hanaConnection);
                    hdr = cmd.ExecuteReader();

                    while (hdr.Read())
                    {
                        string proveedor = "";
                        string linea = "";
                        for (int i = 0; i < hdr.FieldCount; i++)
                        {
                            if (hdr.GetName(i).ToString().Equals("NumDocProvee"))
                                proveedor = hdr[i].ToString();

                            linea += hdr[i].ToString();
                        }
                        if (!string.IsNullOrEmpty(proveedor))
                        {
                            datas.Add(linea);

                            query = $"CALL STR_PAGOSMASIVOSBCP_DBV2_MacroBCP('{wizzad}','{proveedor.Trim()}')";


                            cmds = new HanaCommand(query, hanaConnection);
                            hdrs = cmds.ExecuteReader();

                            while (hdrs.Read())
                            {
                                linea = string.Empty;
                                for (int t = 0; t < hdrs.FieldCount; t++)
                                {
                                    linea += hdrs[t].ToString();
                                }
                                datas.Add(linea);

                            }
                            linea = "";
                        }
                        if (!string.IsNullOrEmpty(linea)) datas.Add(linea);
                    }
                    hanaConnection.Close();

                }
                catch (Exception e)
                {
                    hanaConnection.Close();
                    WriteToFile($"ERROR: Al crear pago {wizzad} " + e.Message);

                }

            }

            if (datas.Count > 0)
            {                
                File.WriteAllText(pathDest + wizzad + ".txt", string.Join("\n", datas));
                Service1.exitoso = true;    
            }
        }



        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\Service_FacturaCreate_Log_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(DateTime.Now.ToString() + " - " + Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(DateTime.Now.ToString() + " - " + Message);
                }
            }
        }

    }
}
