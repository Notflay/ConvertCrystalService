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
using Sap.Data.Hana;

namespace STR_GP_CONVERT_DOCUMENT
{
    public partial class Service1 : ServiceBase
    {
        private System.Threading.Timer timer = null;
        private static bool procesoTerminado;
        public static List<string> wizardNames = new List<string>();
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

                Query query = new Query();

                if (query.ValidationQ())
                {
                    foreach (var wzn in wizardNames)
                    {
                        query.Ejecuta(wzn);


                    }
                }
                procesoTerminado = true;
            }
        }
    }
}
