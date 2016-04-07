﻿using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using PSXDH.BLL;
using PSXDH.Model;

namespace PSXDownloadHelper
{
    public partial class UrlReplace : UserControl
    {
        public delegate void ClickEventHandler(object sender, EventArgs e, UrlReplace obj);

        private readonly ResourceManager _rm = new ResourceManager(typeof(UrlReplace));

        public UrlReplace()
        {
            InitializeComponent();
        }

        public string PsnUrl
        {
            get { return tb_psn.Text; }
            set
            {
                tb_psn.Text = value;
                tb_local.Text = value;
                //lb_filename.Text = UrlOperate.GetUrlFileName(value);
                tb_filename.Text = UrlOperate.GetUrlFileName(value);
                if (AppConfig.Instance().BlockUpdates)
                {
                    Regex regex = new Regex(@"-A\d{4}-V\d{4}\.json");
                    Match match = regex.Match(value);
                    if (match.Success)
                    {
                        tb_filename.BackColor = Color.PaleGreen;

                        LocalPath = System.Environment.CurrentDirectory + @"\Json\IP9100-CUSA00001_00-PLAYROOM00000000-A0102-V0100.json.patched";
                        var t = new Task(() =>
                        {
                            try
                            {

                                var webFile = GetWebContent(value);
                                var localFile = GetFileData(@"Json\template.json");

                                string pattern = "packageDigest\"\\:\"([0-9A-Z]){64}";

                                Regex rgx = new Regex(pattern);
                                Match replacement = rgx.Match(webFile);

                                string result = rgx.Replace(localFile, replacement.Value);

                                //save patched json for future use
                                var sw = new StreamWriter(@"Json\" + tb_filename.Text + ".patched", false);
                                sw.Write(result);
                                sw.Close();

                                //save original json for debugging
                                sw = new StreamWriter(@"Json\" + tb_filename.Text, false);
                                sw.Write(webFile);
                                sw.Close();

                            }
                            catch
                            {

                            }
                        });
                        t.Start();


                    }
                }
            }
        }

        private static string GetFileData(string filename)
        {
            try
            {

                using (StreamReader sr = new StreamReader(filename))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return "";
            }

        }

        private static string GetWebContent(string url)
        {
            var wc = new WebClient { Credentials = CredentialCache.DefaultCredentials };
            var enc = Encoding.GetEncoding("UTF-8");
            var pageData = wc.DownloadData(url);
            var strValue = enc.GetString(pageData);
            return strValue;
        }

        public string LocalPath
        {
            get { return tb_local.Text; }
            set
            {

                if ("Select the local file or drag-and-drop here to enabled the local acceleration" == value)
                {
                    if (tb_local.Text == "")
                    {
                        tb_local.Text = value;
                    }
                }
                else
                {
                    tb_local.Text = value;
                }
            }
        }

        public DateTime LogTime
        {
            set { lb_time.Text = _rm.GetString("lb_time.Text") + @"[" + value.ToString("HH:mm:ss") + @"]"; }
        }

        public string MarkTxt
        {
            get { return tb_mark.Text; }
            set { tb_mark.Text = value; }
        }

        public bool DelBtnVisible
        {
            get { return btn_del.Visible; }
            set { btn_del.Visible = value; }
        }

        public string LixianUrl
        {
            get { return tb_lx.Text; }
            set { tb_lx.Text = value; }
        }

        public bool IsLixian
        {
            get { return !tb_lx.ReadOnly; }
            set
            {
                tb_lx.ReadOnly = !value;
                btn_replace.Enabled = !value;
                btn_enablelx.Text = value ? _rm.GetString("disenable") : _rm.GetString("enable");
            }
        }

        public bool IsCdn
        {
            get { return pic_isCdn.Visible; }
            set { pic_isCdn.Visible = value; }
        }

        public string Host
        {
            get { return (string)btn_ping.Tag; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    btn_ping.Visible = false;
                    tb_Ping.Visible = false;
                }
                btn_ping.Tag = value;
            }
        }

        public event ClickEventHandler ClickReplaceEvent;
        public event ClickEventHandler NameTextChanged;
        public event ClickEventHandler DelRecord;
        public event ClickEventHandler EnableLixian;
        public event ClickEventHandler DragDropFileto;
        public event ClickEventHandler PingClick;

        /// <summary>
        ///     替换文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_replace_Click(object sender, EventArgs e)
        {
            if (ClickReplaceEvent != null)
                ClickReplaceEvent(sender, e, this);
        }

        /// <summary>
        ///     拖动添加替换文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UrlReplace_DragDrop(object sender, DragEventArgs e)
        {
            if (DragDropFileto != null)
                DragDropFileto(sender, e, this);
        }

        /// <summary>
        ///     转换为urlinfo对象
        /// </summary>
        /// <returns></returns>
        public UrlInfo ToUrlInfo()
        {
            var ui = new UrlInfo
            {
                PsnUrl = PsnUrl,
                ReplacePath = LocalPath,
                MarkTxt = MarkTxt,
                LixianUrl = tb_lx.Text,
                IsLixian = !tb_lx.ReadOnly
            };
            return ui;
        }

        private void tb_psn_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(tb_psn.Text);
            ShowCopySuccess((MouseEventArgs)e);
        }

        private void ShowCopySuccess(MouseEventArgs e)
        {
            var p = new Point(e.X + 70, e.Y);
            lb_copy.Location = p;
            lb_copy.Visible = true;
            timer_main.Enabled = true;
        }

        private void timer_main_Tick(object sender, EventArgs e)
        {
            if (lb_copy.Visible)
                lb_copy.Visible = false;

            timer_main.Enabled = false;
        }

        private void tb_mark_TextChanged(object sender, EventArgs e)
        {
            if (NameTextChanged != null)
                NameTextChanged(sender, e, this);
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            if (DelRecord != null)
            {
                DelRecord(sender, e, this);
                Visible = false;
            }
        }

        /// <summary>
        ///     启用离线加速
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_enablelx_Click(object sender, EventArgs e)
        {
            if (btn_enablelx.Text == _rm.GetString("enable"))
            {
                btn_replace.Enabled = false;
                tb_lx.ReadOnly = false;
                btn_enablelx.Text = _rm.GetString("disenable");
                IsLixian = true;
                DataHistoryOperate.AddLog(ToUrlInfo());
            }
            else
            {
                btn_replace.Enabled = true;
                tb_lx.ReadOnly = true;
                btn_enablelx.Text = _rm.GetString("enable");
                IsLixian = false;
                DataHistoryOperate.AddLog(ToUrlInfo());
            }
        }

        private void tb_lx_TextChanged(object sender, EventArgs e)
        {
            if (EnableLixian != null)
                EnableLixian(sender, e, this);
        }

        public void SetLixian(bool set)
        {
            btn_enablelx.Enabled = set;
        }

        private void UrlReplace_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
        }

        private void btn_ping_Click(object sender, EventArgs e)
        {
            if (PingClick != null)
                PingClick(sender, e, this);
        }

        public void ShowPing(string host)
        {
            var ping = new Ping();
            ping.PingCompleted += ping_PingCompleted;
            ping.SendAsync(host, host);
        }

        void ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            tb_Ping.Text = (string)e.UserState;
            var reply = e.Reply;
            if (reply != null && reply.Status == IPStatus.Success)
            {
                tb_Ping.Text += "(" + reply.RoundtripTime + "ms)";
                if (reply.RoundtripTime <= 100)
                    tb_Ping.ForeColor = Color.Green;
                else if (reply.RoundtripTime <= 300)
                    tb_Ping.ForeColor = Color.FromArgb(192, 192, 0);
                else if (reply.RoundtripTime > 300)
                    tb_Ping.ForeColor = Color.Red;
                return;
            }
            tb_Ping.Text += _rm.GetString("pingfailed");
        }

        private void tb_psn_MouseEnter(object sender, EventArgs e)
        {
        }

        private void tb_psn_MouseLeave(object sender, EventArgs e)
        {

        }
    }
}