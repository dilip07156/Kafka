using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace KafkaConsumerService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            try
            {
                //ShowParameters();
                AddConfigurationFileDetails();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + (e.InnerException != null ? e.InnerException.Message : ""), "Error while installing", MessageBoxButtons.OK);
                base.Rollback(savedState);
            }
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        public void ShowParameters()
        {
            StringBuilder sb = new StringBuilder();
            StringDictionary myStringDictionary = this.Context.Parameters;
            if (this.Context.Parameters.Count > 0)
            {
                foreach (string myString in this.Context.Parameters.Keys)
                {
                    sb.AppendFormat("String={0} Value= {1}\n", myString,
                    this.Context.Parameters[myString]);
                }
            }
            MessageBox.Show(sb.ToString());
        }

        private void AddConfigurationFileDetails()
        {
            try
            {
                //IPAddress SERVICEURLIP;
                double dTIMERINTERVAL = 0;
                double dPOLLINTERVAL = 0;
                string SERVICEURLPORT = string.Empty;
                string POLLINTERVAL = string.Empty;
                string TIMERINTERVAL = string.Empty;
                string sSERVICEURLIP = string.Empty;

                string SQLSERVER = string.Empty;
                string SQLUSERNAME = string.Empty;
                string SQLPASSWORD = string.Empty;

                try
                {
                    SERVICEURLPORT = Context.Parameters["SERVICEURLPORT"];
                    POLLINTERVAL = Context.Parameters["POLLINTERVAL"];
                    TIMERINTERVAL = Context.Parameters["TIMERINTERVAL"];
                    sSERVICEURLIP = Context.Parameters["SERVICEURLIP"];
                    SQLSERVER = Context.Parameters["SQLSERVER"];
                    SQLUSERNAME = Context.Parameters["SQLUSERNAME"];
                    SQLPASSWORD = Context.Parameters["SQLPASSWORD"];


                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex.InnerException);
                }

                if (string.IsNullOrWhiteSpace(sSERVICEURLIP))
                {
                    throw new ApplicationException("Invalid Consumer Service IP Address / FQDN");
                }

                if (!string.IsNullOrWhiteSpace(SERVICEURLPORT))
                {
                    try
                    {
                        int port = Convert.ToInt32(SERVICEURLPORT);
                        if (!(port > 1 && port < 65535))
                        {
                            throw new ApplicationException();
                        }
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException("Invalid Port", e.InnerException);
                    }
                }

                if (string.IsNullOrWhiteSpace(TIMERINTERVAL))
                {
                    throw new ApplicationException("Please specify the Timer Interval in Milliseconds");
                }
                else
                {
                    if (!double.TryParse(TIMERINTERVAL, out dTIMERINTERVAL))
                    {
                        throw new ApplicationException("Invalid Timer Interval.");
                    }
                    else
                    {
                        if (!(dTIMERINTERVAL >= 10000 && dTIMERINTERVAL <= 900000))
                        {
                            throw new ApplicationException("Invalid Timer Interval.");
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(POLLINTERVAL))
                {
                    throw new ApplicationException("Please specify the Poll Interval in Milliseconds");
                }
                else
                {
                    
                    if (!double.TryParse(POLLINTERVAL, out dPOLLINTERVAL))
                    {
                        throw new ApplicationException("Invalid Poll Interval.");
                    }
                    else
                    {
                        if (!(dPOLLINTERVAL >= 1000 && dPOLLINTERVAL <= 60000))
                        {
                            
                            throw new ApplicationException("Invalid Poll Interval.");
                        }
                    }
                }


                //Construct Service URL.
                string SERVICEURL;

                if (string.IsNullOrWhiteSpace(SERVICEURLPORT))
                    SERVICEURL = "http://" + sSERVICEURLIP + "/Consumer.svc";
                else
                    SERVICEURL = "http://" + sSERVICEURLIP + ":" + SERVICEURLPORT + "/Consumer.svc";

                //Check Service Url Exists or Not.
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(SERVICEURL.Trim());
                    request.Timeout = 10000;
                    request.KeepAlive = false;
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new ApplicationException("Unable to communicate to Consumer Service using specified IP Address/FQDN and Port");
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex.InnerException);
                }


                //Check Service Url Exists or Not.
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(SERVICEURL.Trim());
                    request.Timeout = 10000;
                    request.KeepAlive = false;
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new ApplicationException("Unable to communicate to Consumer Service using specified IP Address/FQDN and Port");
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message, ex.InnerException);
                }

                //Construct SQL Connection String.
                string ConnectionString = string.Empty;

                if (!string.IsNullOrWhiteSpace(SQLSERVER) && !string.IsNullOrWhiteSpace(SQLUSERNAME) && !string.IsNullOrWhiteSpace(SQLPASSWORD))
                    ConnectionString = "metadata=res://*/ConsumerModel.csdl|res://*/ConsumerModel.ssdl|res://*/ConsumerModel.msl;provider=System.Data.SqlClient;provider connection string='data source=" + SQLSERVER + ";initial catalog=TLGX_MAPPING;user id=" + SQLUSERNAME + ";password=" + SQLPASSWORD + ";multipleactiveresultsets=True;connect timeout=180;application name=EntityFramework'";
                else
                    ConnectionString = "metadata=res://*/ConsumerModel.csdl|res://*/ConsumerModel.ssdl|res://*/ConsumerModel.msl;provider=System.Data.SqlClient;provider connection string='Data Source=DEV-DB-MMSSQL.TRAVELOGIXX.NET,21443;Initial Catalog=TLGX_MAPPING;User Id=sqladmin;Password=I4mth354P455w0rd!*;MultipleActiveResultSets=True;connect timeout=180;application name=EntityFramework'";


                // Get the path to the executable file that is being installed on the target computer  
                string assemblypath = Context.Parameters["assemblypath"];
                string appConfigPath = assemblypath + ".config";

                //string appConfigPath = Path.Combine(new DirectoryInfo(Context.Parameters["assemblypath"].ToString()).Parent.FullName, "[project name].exe");

                // Write the path to the app.config file  
                XmlDocument doc = new XmlDocument();
                doc.Load(appConfigPath);

                XmlNode configuration = null;
                foreach (XmlNode node in doc.ChildNodes)
                    if (node.Name == "configuration")
                        configuration = node;

                if (configuration != null)
                {
                    //MessageBox.Show("configuration != null");  
                    // Get the ‘appSettings’ node  
                    XmlNode settingNode = null;

                    XmlNode connectionStringsNode = null;
                    foreach (XmlNode node in configuration.ChildNodes)
                    {
                        if (node.Name == "appSettings")
                            settingNode = node;

                        if (node.Name == "connectionStrings")
                            connectionStringsNode = node;
                    }

                    if (settingNode != null)
                    {
                        //MessageBox.Show("settingNode != null");  
                        //Reassign values in the config file  
                        foreach (XmlNode node in settingNode.ChildNodes)
                        {
                            //MessageBox.Show("node.Value = " + node.Value);  
                            if (node.Attributes == null)
                                continue;
                            XmlAttribute attribute = node.Attributes["value"];
                            //MessageBox.Show("attribute != null ");  
                            //MessageBox.Show("node.Attributes['value'] = " + node.Attributes["value"].Value);  



                            if (node.Attributes["key"] != null)
                            {
                                //MessageBox.Show("node.Attributes['key'] != null ");  
                                //MessageBox.Show("node.Attributes['key'] = " + node.Attributes["key"].Value);  
                                switch (node.Attributes["key"].Value)
                                {
                                    case "MDMSVCUrl":
                                        attribute.Value = SERVICEURL;
                                        break;
                                    case "TimerInterval":
                                        attribute.Value = TIMERINTERVAL;
                                        break;
                                    case "PollInterval":
                                        attribute.Value = POLLINTERVAL;
                                        break;
                                }
                            }
                        }
                    }


                    if (connectionStringsNode != null)
                    {
                        foreach (XmlNode node in connectionStringsNode.ChildNodes)
                        {
                            if (node.Attributes == null)
                                continue;


                            XmlAttributeCollection attributes = node.Attributes;

                            //MessageBox.Show("node.Attributes['name'] != null ");
                            //MessageBox.Show("node.Attributes['name'] = " + node.Attributes["name"].Value);
                            //MessageBox.Show("node.Attributes['connectionString'] != null ");
                            //MessageBox.Show("node.Attributes['connectionString'] = " + node.Attributes["connectionString"].Value);
                            //MessageBox.Show("ConnectionString = " + ConnectionString);

                            node.Attributes["connectionString"].Value = ConnectionString;
                        }

                    }
                    doc.Save(appConfigPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private void KafkaConsumerServiceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void KafkaConsumerServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
