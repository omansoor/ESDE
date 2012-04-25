using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Notification;
using System.Text;
using System.IO;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Scheduler;
using Microsoft.Expression.Interactivity.Media;
using System.Windows.Interactivity;


namespace EmbeddedSystemsProject
{
    public partial class MainPage : PhoneApplicationPage
    {
        private string HostName;
        private int PortNumber;
        
        private bool msgRecieve = true;
        private string UriHolder;


        PeriodicTask periodicTask;
        
        string periodicTaskName = "PeriodicAgent";
        public bool agentsAreEnabled = true;


        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }


        private void StartPeriodicAgent()
        {
            // Variable for tracking enabled status of background agents for this app.
            agentsAreEnabled = true;

            // Obtain a reference to the period task, if one exists
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            periodicTask = new PeriodicTask(periodicTaskName);

            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            periodicTask.Description = "This demonstrates a periodic task.";

            // Place the call to Add in a try block in case the user has disabled agents.
            try
            {
                ScheduledActionService.Add(periodicTask);
                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.

                ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(10));
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                    agentsAreEnabled = false;
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.

                }
            }
            catch (SchedulerServiceException)
            {
                // No user action required.

            }
        }
        // Constructor
        public MainPage()
        {

            InitializeComponent();
            
            HostName = String.Empty;
            PortNumber = 0;

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            StartPeriodicAgent();

        }


        
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            

            string temp = StorageDSK.read();
             
            if (temp != null && temp.Length > 0)
            {
                txtHostName.Text = temp.Substring(0, temp.IndexOf(":"));
                txtPortNumber.Text = temp.Substring(temp.IndexOf(":")+1);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Clear the log 
            Logg.ClearLog();
            // Make sure we can perform this action with valid data
            if (HostName != null && PortNumber > 0)
            {
                // Instantiate the SocketClient
                CWPWrap client = new CWPWrap();

                // Attempt to connect to the echo server
                //Log(String.Format("Connecting to server '{0}' over port {1} (echo) ...", txtRemoteHost.Text, ECHO_PORT), true);
                string result = client.GetConnection("cwp.opimobi.com", 20000, 5000);
                Logg.Log(result, false);

                // Attempt to send our message to be echoed to the echo server
                //Log(String.Format("Sending '{0}' to server ...", txtInput.Text), true);

                //result = client.Send(txtInput.Text);
                result = client.Send("test send");
                MessageBox.Show(result);
                Logg.Log(result, false);

                // Receive a response from the echo server
                Logg.Log("Requesting Receive ...", true);
                result = client.Receive();
                Logg.Log(result, false);

                // Close the socket connection explicitly
                client.Close();
              
 
            }

        }



        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

        }




        /// <summary>
        /// Handle the btnEcho_Click event by sending text to the echo server and outputting the response
        /// </summary>
        //private void btnEcho_Click(object sender, RoutedEventArgs e)


        #region UI Validation
        /// <summary>
        /// Validates the txtInput TextBox
        /// </summary>
        /// <returns>True if the txtInput TextBox contains valid data, False otherwise</returns>
        private bool ValidatePort()
        {
            
            // txtInput must contain some text
            if (String.IsNullOrWhiteSpace(txtPortNumber.Text))
            {
                MessageBox.Show("Please enter Port number");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the txtRemoteHost TextBox
        /// </summary>
        /// <returns>True if the txtRemoteHost contains valid data, False otherwise</returns>
        private bool ValidateRemoteHost()
        {
            // The txtRemoteHost must contain some text
            if (String.IsNullOrWhiteSpace(txtHostName.Text))
            {
                MessageBox.Show("Please enter a host name");
                return false;
            }

            return true;
        }
        #endregion



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateRemoteHost() && ValidatePort())
            {
                HostName = txtHostName.Text;
                PortNumber = int.Parse(txtPortNumber.Text);
                StorageDSK.store(txtHostName.Text + ":" + PortNumber);
                MessageBox.Show("Settings Saved");
               // NavigationService.Navigate(new Uri("/MainPage.xaml?page=1", UriKind.Relative));
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtPortNumber.Text = ""; txtHostName.Text = "";
            
        }
        private System.Windows.Interactivity.EventTrigger trigger;
        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
         
                trigger = new System.Windows.Interactivity.EventTrigger();
                trigger.EventName = "Click";
                PlaySoundAction beeper = new PlaySoundAction();
                beeper.Source = new Uri(@"C:\Users\Administrator\Documents\Expression\Blend 4\Projects\EmbeddedSystemsProject\EmbeddedSystemsProject\censor-beep-1.mp3", UriKind.Absolute);
                beeper.Volume = 1.0;
                trigger.Actions.Add(beeper);
                trigger.Attach(button1);
        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
              button1.Triggers.Clear();
        }

    }
}