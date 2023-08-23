using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Configuration;

namespace STR_GP_CONVERT_DOCUMENT
{
    public partial class Service1 : ServiceBase
    {
        private System.Threading.Timer timer = null;
        private static bool procesoTerminado;

        public Service1()
        {
            InitializeComponent();
            procesoTerminado = true;
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        public void Ejecutar()
        {

            if (procesoTerminado)
            {
                procesoTerminado = false;


                string origen = ConfigurationSettings.AppSettings["rutaOrigen"].ToString();
                string destino = ConfigurationSettings.AppSettings["rutaDestino"].ToString();

                foreach (var archivo in Directory.GetFiles(origen, "*.csv", SearchOption.AllDirectories))
                {
                    Application excelApp = new Application();
                    Workbook workbook = excelApp.Workbooks.Open(archivo);
                    Worksheet worksheet = workbook.Worksheets[1];

                    string archivoNombre = Path.GetFileNameWithoutExtension(archivo) + ".txt";
                    string archivoDestino = Path.Combine(destino, archivoNombre);

                    using (StreamWriter writer = new StreamWriter(archivoDestino))
                    {
                        Range usedRange = worksheet.UsedRange;

                        foreach (Range row in usedRange.Rows)
                        {
                            string line = "";
                            foreach (Range cell in row.Cells)
                            {
                                line += cell.Value; // Puedes cambiar el separador si es necesario
                            }
                            line = line.TrimEnd(';'); // Quitamos el último ;
                            line = line.Replace(";", " "); // Reemplazamos ; por espacio
                            writer.WriteLine(line);
                        }
                    }

                    workbook.Close(false);
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                }

                procesoTerminado = true;
            }
        }
    }
}
