using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String strHostName = string.Empty;
            // Getting Ip address of local machine...
            // First get the host name of local machine.
            strHostName = Dns.GetHostName();

            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            foreach (IPAddress IP in addr)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    textBox1.Text = IP.ToString();
            }
        }
        void SCANTHR(IPAddress ipScan)
        {
            try
            {

                //IPAddress ipScan = IPAddress.Parse(IP);
              /*  Ping pingSender = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true;
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 1000;
                PingReply reply = pingSender.Send(ipScan, timeout);// (ipScan, timeout, buffer, options);
                string hostname = null;
                string macaddr = null;*/
                //if (reply.Status == IPStatus.Success)
                {
                    //   lock (thisLock)
                    {
                        // hostname = 
                        // macaddr = GetMacUsingARP(ipScan);
                        // UpdateText(ipScan.ToString(), hostname, macaddr);
                        int ID = ipScan.GetAddressBytes()[3];
                        if (ID<255)
                        try
                        {
                            ips[ID].MAC = GetMacUsingARP(ipScan);
                            ips[ID].IP = ipScan.ToString();
                            
                            ips[ID].HOST = GetHostName(ipScan);
                        }
                        catch
                        {
                          //  ips[ID].IP = "";
                         //   ips[ID].MAC = "";
                           // ips[ID].HOST = "";
                        }
                    }
                    //   i++;

                }
                ;



            }
            catch (Exception ex)
            {
              /*  int ID = ipScan.GetAddressBytes()[3];
                ips[ID].IP = ipScan.ToString();
                ips[ID].MAC = ex.Message;
                ips[ID].HOST = "BŁĄD";*/
            }
            // UpdateDONE((int)(ipScan.GetAddressBytes()[3]));

            lock (thisLock)
            {
                finished++;
            }

        }

        private Object thisLock = new Object();
        int finished;

        #region UPDATE_TEXT_FROM THREAD
        delegate void UpdateTextDelegate(string IP, string hostname, string macaddr);

        public void UpdateText(string IP, string hostname, string macaddr)
        {
            try
            {
                if (ListView1.InvokeRequired)
                    ListView1.Invoke(new UpdateTextDelegate(UpdateTextCallback),
                new object[] { IP, hostname, macaddr });
                else
                {
                    UpdateTextCallback(IP, hostname, macaddr); //call directly
                    Application.DoEvents();
                }
            }
            catch { }
        }

        private void UpdateTextCallback(string IP, string hostname, string macaddr)
        {
            try
            {
                ListViewItem itm = new ListViewItem();
                //            netip[i] = ipScan.ToString();
                itm.Text = IP;
                itm.SubItems.Add(hostname);
                itm.SubItems.Add(macaddr);
                ListView1.Items.Add(itm);


            }
            catch { }
        }
        #endregion


        #region Update_DONE_FROM THREAD
        delegate void UpdateDONEDelegate(int IP);

        public void UpdateDONE(int IP)
        {
            try
            {
                if (ListView1.InvokeRequired)
                    ListView1.Invoke(new UpdateDONEDelegate(UpdateDONECallback),
                new object[] { IP });
                else
                {
                    UpdateDONECallback(IP); //call directly
                    Application.DoEvents();
                }
            }
            catch { }
        }

        private void UpdateDONECallback(int IP)
        {
            try
            {

                finshed[IP] = true;

            }
            catch { }
        }
        #endregion


        public string GetHostName(IPAddress pAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(pAddress);
               
                if (entry.HostName != pAddress.ToString() || entry.Aliases==null)
                    return entry.HostName;
                else
                {
                    return entry.Aliases[0];
                }
            }
            catch (Exception)
            {
                return pAddress.ToString();
            }
        }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(
            int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);


        private static string GetMacUsingARP(IPAddress IPAddr)
        {
            int intAddress = BitConverter.ToInt32(IPAddr.GetAddressBytes(), 0);

            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;
            if (SendARP(intAddress, 0, macAddr, ref macAddrLen) != 0)
                throw new InvalidOperationException("SendARP failed.");

            string[] str = new string[(int)macAddrLen];
            for (int i = 0; i < macAddrLen; i++)
                str[i] = macAddr[i].ToString("x2");

            //  Console.WriteLine(string.Join(":", str));

            return string.Join(":", str);
        }


        Thread[] thrs = new Thread[255];
        bool[] finshed = new bool[255];
        int petla;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
               // button2_Click(null, null);
                petla = 0;
                //ListView1.Items.Clear();
                button1.Enabled = false;
                button2.Enabled = false;
                ListView1.Enabled = false;

                Application.DoEvents();

                IPAddress ip = IPAddress.Parse(textBox1.Text);

                int[] aa = new int[4];

                //Get first, second, … byte of address
                aa[0] = Convert.ToInt32(ip.GetAddressBytes()[0]);
                aa[1] = Convert.ToInt32(ip.GetAddressBytes()[1]);
                aa[2] = Convert.ToInt32(ip.GetAddressBytes()[2]);
                aa[3] = Convert.ToInt32(ip.GetAddressBytes()[3]);
                adr = aa[0].ToString() + "." + aa[1].ToString() + "." + aa[2].ToString() + ".";
                label2.Text = "Skanuję adresy :" + adr + "1 - " + adr + "254 ... ";
                Application.DoEvents();
                lock (thisLock)
                {
                    finished = 0;

                    for (int i = 1; i < 255; i++)
                    {
                       // ips[i] = new IPS();
                       /// ips[i].IP = "";
                       // ips[i].MAC = "";
                       // ips[i].HOST = "";
                       // thrs[i] = null;
                    }
                    SetThreads(1);

                }
                timer1.Start();
            }
            catch (Exception ex)
            {
                ShowErr(ex);
            }

        }

        string adr;
        int startIndex;
        int threads = 255;

        public class IPS
        {
            public string IP;
            public string HOST;
            public string MAC;
        }
        IPS[] ips = new IPS[255];
        private void SetThreads(int start)
        {
            startIndex = start;
            if (startIndex > 254) return;
            int end = start + threads; if (end > 255) end = 255;
            for (int i = start; i < end; i++)
            {
                if (ips[i] == null) ips[i] = new IPS();
                thrs[i] = new Thread(() => SCANTHR(IPAddress.Parse(adr + i.ToString())));
                finshed[i] = false;
                thrs[i].Start();
            }
        }
        private void ShowErr(Exception ex)
        {
            MessageBox.Show(ex.Message, "BŁĄD", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            // bool rv = true;
            //  int end = startIndex + threads; if (end > 255) end = 255;
            /*  for (int i = startIndex; i < end; i++) rv = rv & finshed[i];
              if (rv)
              {
                  if (end < 255)
                  {
                      SetThreads(startIndex + threads);
                  }
                  else
                  {
                      label2.Text += " Koniec";
                      button1.Enabled = true;
                  }
              }
              else
                  timer1.Start();*/
            lock (thisLock)
            {

                if (finished > 253)
                {
                    if (petla >=2)
                    {
                        label2.Text += " Koniec";
                        button1.Enabled = true;
                        button2.Enabled = true;
                        ListView1.Enabled = true;
                        FillAll();
                        return;
                    }
                    else
                    {
                        FillAll();
                        finished = 0;
                        petla++;
                        SetThreads(1);

                    }
                }
                else
                {
                    if (finished >= (startIndex + threads) - 1)
                    {
                        Fill();
                        SetThreads(startIndex + threads);
                    }
                }

            }

            timer1.Start();
        }
        private void Fill()
        {
         //   ListView1.Items.Clear();
            int end = startIndex + threads; if (end > 255) end = 255;
            for (int i = startIndex; i < end; i++)
            {
                if (!string.IsNullOrEmpty(ips[i].IP))
                {
                    ListViewItem itm = new ListViewItem();
                    //            netip[i] = ipScan.ToString();
                    itm.Text = ips[i].IP;
                    itm.SubItems.Add(ips[i].HOST);
                    itm.SubItems.Add(ips[i].MAC);
                    ListView1.Items.Add(itm);
                }
            }
        }
        private void FillAll()
        {
               ListView1.Items.Clear();
          
            for (int i = 1; i < 255; i++)
            {
                if (!string.IsNullOrEmpty(ips[i].IP))
                {
                    ListViewItem itm = new ListViewItem();
                    //            netip[i] = ipScan.ToString();
                    itm.Text = ips[i].IP;
                    itm.SubItems.Add(ips[i].HOST);
                    itm.SubItems.Add(ips[i].MAC);
                    ListView1.Items.Add(itm);
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ListView1.Items.Clear();
            for (int i = 1; i < 255; i++)
            {
                if (ips[i] == null) ips[i] = new IPS();
                 ips[i].IP = "";
                 ips[i].MAC = "";
                 ips[i].HOST = "";
                 thrs[i] = null;
            }
        }
        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }
        private void pingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string IP = ListView1.SelectedItems[0].Text;
            if (PingHost(IP))
            {
                MessageBox.Show("Ping Succesful");
            }
            else
            {
                MessageBox.Show("Ping Failed");
            }
        }
        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;
            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }
        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string IP = ListView1.SelectedItems[0].Text;
                System.Diagnostics.Process.Start("http://" + IP);
            }
            catch { }
        }
        private void copyIPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            try
            {
                string IP = ListView1.SelectedItems[0].Text;
                Clipboard.SetText(IP);
            }
            catch { }
        }
    }
}
