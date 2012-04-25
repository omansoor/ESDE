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
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace EmbeddedSystemsProject
{
    public class CWPWrap
    {
        private static Socket CWPSocket;
        private static ManualResetEvent resetE;

        public string GetConnection(string serverURL, int port, int timeOut)
        {
            resetE = new ManualResetEvent(false);
           

            string result = String.Empty;
            DnsEndPoint server = new DnsEndPoint(serverURL, port);
            CWPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs asyncEventArgs = new SocketAsyncEventArgs();
            asyncEventArgs.RemoteEndPoint = server;

            asyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
            {
                result = e.SocketError.ToString(); 
                resetE.Set();  
            });

            resetE.Reset();
            CWPSocket.ConnectAsync(asyncEventArgs);
            resetE.WaitOne(5000);
            return result;

        }
        public string Send(String data)
        {
            string response = "Operation timed out";
            if (CWPSocket != null)
            {
                SocketAsyncEventArgs socketEventArgs = new SocketAsyncEventArgs();
                socketEventArgs.RemoteEndPoint = CWPSocket.RemoteEndPoint;
                socketEventArgs.UserToken = null;

                socketEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
                {
                    response = e.SocketError.ToString();

                    // Unblock the UI thread
                    resetE.Set();
                });
                byte[] payload = Encoding.UTF8.GetBytes(data);
                socketEventArgs.SetBuffer(payload, 0, payload.Length);

                // Sets the state of the event to nonsignaled, causing threads to block
                resetE.Reset();

                // Make an asynchronous Send request over the socket
                CWPSocket.SendAsync(socketEventArgs);

                // Block the UI thread for a maximum of 5000 milliseconds.
                // If no response comes back within this time then proceed
                resetE.WaitOne(5000);
            }
            else
                return "Socket not initialized";
            return response;
        }

        public string Receive()
        {
            string response = "Operation Timeout";

            // We are receiving over an established socket connection
            if (CWPSocket != null)
            {
                // Create SocketAsyncEventArgs context object
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = CWPSocket.RemoteEndPoint;

                // Setup the buffer to receive the data
                socketEventArg.SetBuffer(new Byte[2048], 0, 2048);

                // Inline event handler for the Completed event.
                // Note: This even handler was implemented inline in order to make this method self-contained.
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
                {
                    if (e.SocketError == SocketError.Success)
                    {
                        // Retrieve the data from the buffer
                        response = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                        response = response.Trim('\0');
                    }
                    else
                    {
                        response = e.SocketError.ToString();
                    }

                    resetE.Set();
                });

                // Sets the state of the event to nonsignaled, causing threads to block
                resetE.Reset();

                // Make an asynchronous Receive request over the socket
                CWPSocket.ReceiveAsync(socketEventArg);

                // Block the UI thread for a maximum of 5000 milliseconds.
                // If no response comes back within this time then proceed
                resetE.WaitOne(5000);
            }
            else
            {
                response = "Socket is not initialized";
            }

            return response;
        }

        /// <summary>
        /// Closes the Socket connection and releases all associated resources
        /// </summary>
        public void Close()
        {
            if (CWPSocket != null)
            {
                CWPSocket.Close();
            }
        }

    }
}
