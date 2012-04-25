using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace EmbeddedSystemsProject
{
    public static class Logg
    {
        #region Logging
        /// <summary>
        /// Log text to the txtOutput TextBox
        /// </summary>
        /// <param name="message">The message to write to the txtOutput TextBox</param>
        /// <param name="isOutgoing">True if the message is an outgoing (client to server) message, False otherwise</param>
        /// <remarks>We differentiate between a message from the client and server 
        /// by prepending each line  with ">>" and "<<" respectively.</remarks>
        public static void Log(string message, bool isOutgoing)
        {
            string direction = (isOutgoing) ? ">> " : "<< ";
            //txtOutput.Text += Environment.NewLine + direction + message;
        }

        /// <summary>
        /// Clears the txtOutput TextBox
        /// </summary>
        public static void ClearLog()
        {
            //txtOutput.Text = String.Empty;
        }
        #endregion
    }
}
