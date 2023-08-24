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
    public  class Query
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
        public  Query()
        {
            //coneecion = ConfigurationManager.ConnectionStrings["hana"].ConnectionString; 
            hanaConnection = new HanaConnection(ConfigurationManager.ConnectionStrings["hana"].ConnectionString);

        }

        public  bool ValidationQ()
        {
            string dia = ConfigurationManager.AppSettings["dia"];
            string query = $"SELECT \"WizardName\" FROM OPWZ WHERE DAYS_BETWEEN(\"PmntDate\",NOW()) < {dia}";

            pathDest = ConfigurationManager.AppSettings["rutaDestino"]; 

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
                        if (!File.Exists(pathDest + hdr.GetString(0).ToString()))
                            Service1.wizardNames.Add(hdr.GetString(0).ToString());
                    }
                }

                hanaConnection.Close();

                if (Service1.wizardNames.Count > 0)
                    return true;            
                return false;
            }
            catch (Exception)
            {
                if (hanaConnection.State == System.Data.ConnectionState.Open)
                    hanaConnection.Close();
                return false;
            }      
        }

        public void Ejecuta(string wzn)
        {

            RetrnList(wzn);
        }

        public List<SBODataField> RetrnList(string wizzad)
        {
            List<string> datas = new List<string>();
            List<string> querys = new List<string>();
            //querys.Add("STR_PAGOSMASIVOSBCP_CV2_MacroBCP");
            //querys.Add("STR_PAGOSMASIVOSBCP_DAV2_MacroBCP");
            querys.Add("CALOPWS");
            List<SBODataField> lstSBODataField = new List<SBODataField>();
            SBODataField data = null;

            foreach (var q in querys)
            {
                var subIdFila = 0;
                hanaConnection.Open();

                string query = $"CALL {q} ('{wizzad}')"; 
                cmd = new HanaCommand(query, hanaConnection);
                hdr = cmd.ExecuteReader();

                while (hdr.Read())
                {
                    string linea = "";
                    for (int i = 0; i < hdr.FieldCount; i++)
                    {
                        linea += hdr[i].ToString();

                        //data = new SBODataField();
                        //data.IDFIla = Convert.ToInt32(hdr.GetName(i).Split('_')[0]);
                        //data.SubIDFIla = subIdFila;
                        //data.Value = hdr[i].ToString();
                        

                        //lstSBODataField.Add(data);

                    }
                    subIdFila++;    
                }
                



            }
            return lstSBODataField;

        }

        /*
        public static bool crearArchivo(StringBuilder data, string nombre, string ruta)
        {
            try
            {
                Encoding utf8WithoutBom = new UTF8Encoding(false);

                if (data is null || string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(ruta))
                    throw new ArgumentNullException(data is null ? nameof(data) : string.IsNullOrEmpty(nombre)
                          ? nameof(nombre) : nameof(ruta), "Null argument in create file method");
                if (string.IsNullOrEmpty(data.ToString())) throw new ArgumentException($"Empty data: {nombre}");
                ruta = Path.Combine(ruta, $"{nombre}.txt");
                File.WriteAllText(ruta, data.ToString(), utf8WithoutBom);
                return true;
            }
            catch { throw; }
        }

        public StringBuilder generarData(string wizzad)
        {
            StringBuilder stringBuilder = null;
            string fila = string.Empty;
            try
            {


                List<SBODataField> lstDatos = RetrnList(wizzad);


                var lstFilasCSV = lstDatos.GroupBy(x => new { x.IDFIla, x.SubIDFIla })
                    .Select(y => new { ID = y.Key.IDFIla, SubID = y.Key.SubIDFIla, Value = y.Select(s => s.Value).ToList() })
                    .OrderBy(o => o.ID);
                //var lstFilasCSVAux = /*lstFilasCSV.Where(w => w.ID != 9) : lstFilasCSV.Where(w => w.ID != 2 && w.ID != 3);
                //lstFilasCSVAux.ToList().ForEach(l =>
                //{
                //    stringBuilder.Append(string.Concat(string.Join(",", l.Value), Environment.NewLine));
                //});
                /*
                lstFilasCSV.ToList().ForEach(l =>
                {
                stringBuilder.Append(l.Value, Environment.NewLine));
                });

                stringBuilder.Append(lstFilasCSV);
            }
            catch { throw; }
            return stringBuilder;
        }*/

    }
}
