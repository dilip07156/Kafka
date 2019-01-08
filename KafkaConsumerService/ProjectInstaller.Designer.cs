namespace KafkaConsumerService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.KafkaConsumerServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.KafkaConsumerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // KafkaConsumerServiceProcessInstaller
            // 
            this.KafkaConsumerServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.KafkaConsumerServiceProcessInstaller.Password = null;
            this.KafkaConsumerServiceProcessInstaller.Username = null;
            // 
            // KafkaConsumerServiceInstaller
            // 
            this.KafkaConsumerServiceInstaller.Description = "This service reads data from Kafka queues from the upstream MDM system periodical" +
    "ly and update it into Nakshatra sql db";
            this.KafkaConsumerServiceInstaller.DisplayName = "Kafka Consumer Service";
            this.KafkaConsumerServiceInstaller.ServiceName = "KafkaConsumerService";
            this.KafkaConsumerServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.KafkaConsumerServiceProcessInstaller,
            this.KafkaConsumerServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller KafkaConsumerServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller KafkaConsumerServiceInstaller;
    }
}