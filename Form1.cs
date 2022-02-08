using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace Set_List_Manager
{


    public partial class set_list_manager : Form
    {
        int status_main = 0;
        int status_setting = 0;

        public set_list_manager()
        {
            InitializeComponent();
        }

        static string set_list_pass;
        static string music_library_pass;

        public void add_set_list()
        {
            //1行追加
            show_set_list.Rows.Add(add_music.Text, add_singer.Text, add_url.Text, "再生前");
            //追加した行番号を取得
            int i = show_set_list.Rows.Count - 1;
            //追加した行にフォーカスを移す
            show_set_list[0, i].Selected = true;

            //フォーム消去
            add_music.Text = "";
            add_singer.Text = "";
            add_url.Text = "";
        }
        public void display_obs(string music, string singer)
        {
            string obs_txt = notation.Text;
            //表記通りにテキストを変換
            obs_txt = obs_txt.Replace("{music}", music);
            obs_txt = obs_txt.Replace("{singer}", singer);

            //テキスト出力
            StreamWriter set_list = new StreamWriter(set_list_pass, true, Encoding.GetEncoding("utf-8"));
            set_list.WriteLine(obs_txt);
            set_list.Close();
        }
        public void clear_obs()
        {
            StreamWriter obx_txt = new StreamWriter(set_list_pass, false, Encoding.GetEncoding("utf-8"));
            obx_txt.Write("");
            obx_txt.Close();
        }
        public void reload_music_library_list()
        {
            music_library_list.Items.Clear();
            for (int i = 0; i <= show_music_library.Rows.Count - 1; i++)
            {
                music_library_list.Items.Add(show_music_library.Rows[i].Cells[0].Value + ":" + show_music_library.Rows[i].Cells[2].Value);
            }
            if (show_music_library.Rows.Count != 0)
            {
                music_library_list.SelectedIndex = 0;
            }
        }
        public void get_music_library()
        {
            if (!File.Exists(music_library_pass))
            {
                return;
            }
            TextFieldParser music_library = new TextFieldParser(music_library_pass, Encoding.GetEncoding("utf-8"));
            music_library.TextFieldType = FieldType.Delimited;
            music_library.SetDelimiters(",");

            //DataGridViewの初期化
            show_music_library.Rows.Clear();

            while (!music_library.EndOfData)
            {
                show_music_library.Rows.Add(music_library.ReadFields());
            }
            music_library.Close();
            reload_music_library_list();
        }
        public void reload_set_list_pass()
        {
            if (display_set_list_pass.Text.Length == 0)
            {
                set_list_pass = System.Environment.CurrentDirectory + "\\set_list.txt";
            }
            else
            {
                set_list_pass = display_set_list_pass.Text;

            }
        }
        public void reload_music_library_pass()
        {
            if (display_music_library_pass.Text.Length == 0)
            {
                music_library_pass = System.Environment.CurrentDirectory + "\\music_library.csv";
            }
            else
            {
                music_library_pass = display_music_library_pass.Text;
            }
        }
        public void reload_music_library_file()
        {
            //ファイルに書き込み
            StreamWriter music_library = new StreamWriter(music_library_pass, false, Encoding.GetEncoding("utf-8"));
            for (int i = 0; i <= show_music_library.Rows.Count - 1; i++)
            {
                music_library.WriteLine("\"" + show_music_library.Rows[i].Cells[0].Value + "\",\"" + show_music_library.Rows[i].Cells[1].Value + "\",\"" + show_music_library.Rows[i].Cells[2].Value + "\",\"" + show_music_library.Rows[i].Cells[3].Value + "\",\"" + show_music_library.Rows[i].Cells[4].Value);
            }
            music_library.Close();
        }

        private void start()
        {
            for (int i = 0; i <= show_set_list.Rows.Count - 1; i++)
            {
                if (show_set_list.Rows[i].Cells[3].Value.ToString() == "再生前")
                {
                    song.Text = "停止";
                    status_main = 1;
                    show_set_list.Rows[i].Cells[3].Value = "再生中";
                    if (song_before.Checked)
                    {
                        display_obs(show_set_list.Rows[i].Cells[0].Value.ToString(), show_set_list.Rows[i].Cells[1].Value.ToString());
                    }
                    if (show_set_list.Rows[i].Cells[2].Value != null)
                    {
                        switch (browser_list.SelectedIndex)
                        {
                            case 1: //Microsoft Edge
                                System.Diagnostics.Process.Start("msedge.exe", show_set_list.Rows[i].Cells[2].Value.ToString() + " " + browser_argument.Text);
                                break;
                            case 2: //google Chrome 
                                System.Diagnostics.Process.Start("chrome.exe", "-url " + show_set_list.Rows[i].Cells[2].Value.ToString() + " " + browser_argument.Text);
                                break;
                        }
                        return;

                    }
                }
            }
            MessageBox.Show("全ての曲を歌いました。新しい曲を追加してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void stop()
        {
            song.Text = "開始";
            status_main = 0;
            for (int i = 0; i <= show_set_list.Rows.Count - 1; i++)
            {
                if (show_set_list.Rows[i].Cells[3].Value.ToString() == "再生中")
                {
                    show_set_list.Rows[i].Cells[3].Value = "再生終了";
                    if (song_after.Checked)
                    {
                        display_obs(show_set_list.Rows[i].Cells[0].Value.ToString(), show_set_list.Rows[i].Cells[1].Value.ToString());
                    }
                    break;
                }
            }
        }

        //startup
        private void set_list_manager_Load(object sender, EventArgs e)
        {
            if (song_before.Checked == false)
            {
                song_after.Checked = true;
            }

            reload_music_library_pass();
            get_music_library();

            reload_set_list_pass();
            if (System.IO.File.Exists(set_list_pass) && System.IO.File.ReadAllText(set_list_pass) != "")
            {
                clear_obs();
            }

            browser_list.SelectedIndex = int.Parse(Properties.Settings.Default.browser_select);

            string[] temp;

            if (Properties.Settings.Default.start_key.Length != 0)
            {
                temp = Properties.Settings.Default.start_key.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    start_list.Add(int.Parse(temp[i]));
                }
                reload_keyconfig(start_list, keyconfig_start);
            }


            if (Properties.Settings.Default.stop_key.Length != 0)
            {
                temp = Properties.Settings.Default.stop_key.Split(',');
                for (int i = 0; i < temp.Length; i++)
                {
                    stop_list.Add(int.Parse(temp[i]));
                }
                reload_keyconfig(stop_list, keyconfig_stop);
            }


            keyboardHook.KeyDownEvent += KeyboardHook_KeyDownEvent;
            keyboardHook.KeyUpEvent += KeyboardHook_KeyUpEvent;
            keyboardHook.Hook();
        }


        //main
        private void add_setlist_Click(object sender, EventArgs e)
        {
            add_set_list();
        }
        private void add_setlist_for_library_Click(object sender, EventArgs e)
        {
            if (show_music_library.Rows.Count == 0)
            {
                return;
            }

            int select_list = music_library_list.SelectedIndex;


            add_music.Text = show_music_library.Rows[select_list].Cells[0].Value.ToString();
            add_singer.Text = show_music_library.Rows[select_list].Cells[2].Value.ToString();
            add_url.Text = show_music_library.Rows[select_list].Cells[4].Value.ToString();

            add_set_list();

        }


        private void song_Click(object sender, EventArgs e)
        {

            switch (status_main)
            {
                case 0: //歌唱前
                    start();
                    break;
                case 1:
                    stop();
                    break;
            }
        }
        private void music_next_Click(object sender, EventArgs e)
        {
            //もしlistが空なら終了
            if (show_set_list.Rows.Count == 0)
            {
                return;
            }
            int select_line = show_set_list.SelectedCells[0].RowIndex;
            for (int i = 0; i <= show_set_list.Rows.Count; i++)
            {
                if (show_set_list.Rows[i].Cells[3].Value.ToString() == "再生前")
                {
                    show_set_list.Rows.Insert(i, show_set_list.Rows[select_line].Cells[0].Value, show_set_list.Rows[select_line].Cells[1].Value, show_set_list.Rows[select_line].Cells[2].Value, show_set_list.Rows[select_line].Cells[3].Value);
                    show_set_list.Rows[i].Selected = true;
                    break;
                }
            }
            show_set_list.Rows.RemoveAt(select_line + 1);

        }
        private void music_up_Click(object sender, EventArgs e)
        {
            //セットリストが0なら終了
            if (show_set_list.Rows.Count == 0)
            {
                return;
            }

            string temp_music, temp_singer, temp_url;

            int i = show_set_list.SelectedCells[0].RowIndex;
            //一番上なら無視する
            if (i == 0)
            {
                return;
            }
            //再生終了より前にはしない
            if (show_set_list.Rows[i - 1].Cells[3].Value.ToString() != "再生前")
            {
                return;
            }
            //入れ替え先を退避
            temp_music = show_set_list.Rows[i - 1].Cells[0].Value.ToString();
            temp_singer = show_set_list.Rows[i - 1].Cells[1].Value.ToString();
            temp_url = show_set_list.Rows[i - 1].Cells[2].Value.ToString();
            //一つ上へ
            show_set_list.Rows[i - 1].Cells[0].Value = show_set_list.Rows[i].Cells[0].Value;
            show_set_list.Rows[i - 1].Cells[1].Value = show_set_list.Rows[i].Cells[1].Value;
            show_set_list.Rows[i - 1].Cells[2].Value = show_set_list.Rows[i].Cells[2].Value;
            //退避したものを代入
            show_set_list.Rows[i].Cells[0].Value = temp_music;
            show_set_list.Rows[i].Cells[1].Value = temp_singer;
            show_set_list.Rows[i].Cells[2].Value = temp_url;
            //選択先変更
            show_set_list.Rows[i - 1].Selected = true;
        }
        private void music_down_Click(object sender, EventArgs e)
        {
            //セットリストが0なら終了
            if (show_set_list.Rows.Count == 0)
            {
                return;
            }

            string temp_music, temp_singer, temp_url;

            int i = show_set_list.SelectedCells[0].RowIndex;
            //一番下にいるなら無視する
            if (i == show_set_list.Rows.Count - 1)
            {
                return;
            }
            //再生終了したものは動かない
            if (show_set_list.Rows[i + 1].Cells[3].Value.ToString() != "再生前")
            {
                return;
            }
            //入れ替え先を退避
            temp_music = show_set_list.Rows[i + 1].Cells[0].Value.ToString();
            temp_singer = show_set_list.Rows[i + 1].Cells[1].Value.ToString();
            temp_url = show_set_list.Rows[i + 1].Cells[2].Value.ToString();
            //一つ下へ
            show_set_list.Rows[i + 1].Cells[0].Value = show_set_list.Rows[i].Cells[0].Value;
            show_set_list.Rows[i + 1].Cells[1].Value = show_set_list.Rows[i].Cells[1].Value;
            show_set_list.Rows[i + 1].Cells[2].Value = show_set_list.Rows[i].Cells[2].Value;
            //退避したものを代入
            show_set_list.Rows[i].Cells[0].Value = temp_music;
            show_set_list.Rows[i].Cells[1].Value = temp_singer;
            show_set_list.Rows[i].Cells[2].Value = temp_url;
            //選択先変更
            show_set_list.Rows[i + 1].Selected = true;
        }
        private void music_delete_Click(object sender, EventArgs e)
        {
            //セットリストが0なら終了
            if (show_set_list.Rows.Count == 0)
            {
                return;
            }
            show_set_list.Rows.RemoveAt(show_set_list.SelectedCells[0].RowIndex);
        }

        private void music_clear_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("ファイルにセットリストが記録されています。消去しますか?", "確認", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            switch (result)
            {
                case DialogResult.Yes:
                    show_set_list.Rows.Clear();
                    clear_obs();
                    break;
                case DialogResult.No:
                    show_set_list.Rows.Clear();
                    break;
                case DialogResult.Cancel:
                    break;

            }

        }



        //music_library
        private void set_music_library_pass_Click(object sender, EventArgs e)
        {
            OpenFileDialog music_list = new OpenFileDialog();
            music_list.FileName = "music_library.csv";
            music_list.Filter = "CSVファイル(*.csv)|*.csv|全てのファイル(*.*)|*.*";
            music_list.CheckFileExists = false;

            music_list.ShowDialog();
            display_music_library_pass.Text = music_list.FileName;

            music_list.Dispose();
        }
        private void display_music_library_pass_TextChanged(object sender, EventArgs e)
        {
            reload_music_library_pass();
            get_music_library();

        }
        private void reload_music_library_Click(object sender, EventArgs e)
        {
            get_music_library();
        }

        private void add_music_library_Click(object sender, EventArgs e)
        {
            //1行追加
            show_music_library.Rows.Add(add_library_music.Text, add_library_music_kana.Text, add_library_singer.Text, add_library_singer_kana.Text, add_library_url.Text);
            //追加した行番号を取得
            int i = show_music_library.Rows.Count - 1;
            //追加した行にフォーカスを移す
            show_music_library.Rows[i].Selected = true;

            //ファイル書き込み
            StreamWriter music_library = new StreamWriter(music_library_pass, true, Encoding.GetEncoding("utf-8"));
            music_library.WriteLine("\"" + add_library_music.Text + "\",\"" + add_library_music_kana.Text + "\",\"" + add_library_singer.Text + "\",\"" + add_library_singer_kana.Text + "\",\"" + add_library_url.Text + "\"");
            music_library.Close();

            //ファーム消去
            add_library_music.Text = "";
            add_library_music_kana.Text = "";
            add_library_singer.Text = "";
            add_library_singer_kana.Text = "";
            add_library_url.Text = "";

            reload_music_library_list();
        }

        private void library_delete_Click(object sender, EventArgs e)
        {
            if (show_music_library.Rows.Count == 0)
            {
                return;
            }
            show_music_library.Rows.RemoveAt(show_music_library.SelectedCells[0].RowIndex);

            reload_music_library_file();
            reload_music_library_list();
        }

        private void reload_file_Click(object sender, EventArgs e)
        {
            reload_music_library_file();
        }


        //setting
        private void set_set_list_pass_Click(object sender, EventArgs e)
        {
            OpenFileDialog set_list = new OpenFileDialog();
            set_list.FileName = "set_list.txt";
            set_list.Filter = "Textファイル(*.txt)|*.txt|全てのファイル(*.*)|*.*";
            set_list.CheckFileExists = false;

            set_list.ShowDialog();

            set_list_pass = set_list.FileName;
            display_set_list_pass.Text = set_list_pass;
            set_list.Dispose();
        }
        private void display_set_list_pass_TextChanged(object sender, EventArgs e)
        {
            reload_set_list_pass();
        }

        private void browser_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.browser_select = browser_list.SelectedIndex.ToString();


            if (browser_list.SelectedIndex == 0)
            {
                HIKISU.Visible = false;
                browser_argument.Visible = false;
            }
            else
            {
                HIKISU.Visible = true;
                browser_argument.Visible = true;
            }
            debug2.Text = browser_list.SelectedIndex.ToString();
        }

        private void set_keyconfig_start_Click(object sender, EventArgs e)
        {
            if (status_setting != 1)
            {
                set_keyconfig_start.Text = "保存";
                set_keyconfig_stop.Text = "設定";
                status_setting = 1;
                start_list.Clear();
                Properties.Settings.Default.start_key = null;
                keyconfig_start.Text = "";
            }
            else
            {
                set_keyconfig_start.Text = "設定";
                status_setting = 0;
                if (start_list.Count != 0)
                {
                    Properties.Settings.Default.start_key = start_list[0].ToString();
                    for (int i = 1; i < start_list.Count; i++)
                    {
                        Properties.Settings.Default.start_key += "," + start_list[i].ToString();
                    }
                }
            }
        }
        private void set_keyconfig_stop_Click(object sender, EventArgs e)
        {
            if (status_setting != 2)
            {

                set_keyconfig_stop.Text = "保存";
                set_keyconfig_start.Text = "設定";
                status_setting = 2;
                stop_list.Clear();
                Properties.Settings.Default.stop_key = null;
                keyconfig_stop.Text = "";
            }
            else
            {
                set_keyconfig_stop.Text = "設定";
                status_setting = 0;

                if (stop_list.Count != 0)
                {
                    Properties.Settings.Default.stop_key = stop_list[0].ToString();
                    for (int i = 1; i < stop_list.Count; i++)
                    {
                        Properties.Settings.Default.stop_key += "," + stop_list[i].ToString();
                    }
                }
            }
        }



        //Closing
        private void set_list_manager_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        
        //ShortcutKey
        KeyboardHook keyboardHook = new KeyboardHook();

        List<int> key_list = new List<int>();
        List<int> start_list = new List<int>();
        List<int> stop_list = new List<int>();

        int counter = 0;


        private void reload_keyconfig(List<int> list, Control label)
        {
            KeysConverter kc = new KeysConverter();


            Boolean temp = false;
            label.Text = "";
            if (list.Contains(160) || list.Contains(161))
            {
                label.Text = "Shift";
                temp = true;
            }

            if (list.Contains(162) || list.Contains(163))
            {
                if (temp)
                {
                    label.Text += " + Ctrl";
                }
                else
                {
                    label.Text = "Ctrl";
                    temp = true;
                }
            }


            if (list.Contains(163) || list.Contains(164))
            {
                if (temp)
                {
                    label.Text += " + Alt";
                }
                else
                {
                    label.Text = "Alt";
                    temp = true;
                }
            }


            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i] < 160 || 165 < list[i])
                {

                    if (temp)
                    {

                        label.Text += " + " + kc.ConvertToString(list[i]);
                    }
                    else
                    {

                        label.Text = kc.ConvertToString(list[i]);
                        temp = true;
                    }
                }
            }
        }

        private void KeyboardHook_KeyDownEvent(object sender, KeyEventArg e)
        {
            KeysConverter kc = new KeysConverter();

            if (!key_list.Contains(e.KeyCode))
            {
                key_list.Add(e.KeyCode);
            }

            switch (status_setting)
            {
                case 0:
                    Boolean flag = true;

                    switch (status_main)
                    {
                        case 0:
                            if(start_list.Count == 0)
                            {
                                break;
                            }
                            for (int i = 0; i < start_list.Count(); i++)
                            {
                                if (!key_list.Contains(start_list[i]))
                                {
                                    flag = false;
                                }
                            }

                            if (flag)
                            {
                                start();
                            }
                            break;
                        case 1:
                            if(stop_list.Count == 0)
                            {
                                break;
                            }
                            for (int i = 0; i < stop_list.Count(); i++)
                            {
                                if (!key_list.Contains(stop_list[i]))
                                {
                                    flag = false;
                                }
                            }

                            if (flag)
                            {
                                stop();
                            }
                            break;

                    }
                    break;
                case 1:
                    if (counter == 0)
                    {
                        start_list.Clear();
                    }
                    if (!start_list.Contains(e.KeyCode))
                    {
                        start_list.Add(e.KeyCode);
                        counter++;
                    }
                    reload_keyconfig(start_list, keyconfig_start);
                    break;
                case 2:
                    if (counter == 0)
                    {
                        stop_list.Clear();
                    }
                    if (!stop_list.Contains(e.KeyCode))
                    {
                        stop_list.Add(e.KeyCode);
                        counter++;
                    }
                    reload_keyconfig(stop_list, keyconfig_stop);
                    break;
            }

        }

        private void KeyboardHook_KeyUpEvent(object sender, KeyEventArg e)
        {
            KeysConverter kc = new KeysConverter();

            if (key_list.Contains(e.KeyCode))
            {
                key_list.Remove(e.KeyCode);
            }

            switch (status_setting)
            {
                case 0:
                    break;
                case 1:
                    if (start_list.Contains(e.KeyCode))
                    {
                        counter--;
                    }
                    reload_keyconfig(start_list, keyconfig_start);
                    break;
                case 2:
                    if (stop_list.Contains(e.KeyCode))
                    {
                        counter--;
                    }
                    reload_keyconfig(stop_list, keyconfig_stop);
                    break;
            }
        }
       
        private void set_list_manager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData.ToString() == debug2.Text)
            {
                debug3.Text = "OK";
            }
            else
            {
                debug3.Text = "NG";
            }
        }
    }


    class KeyboardHook
    {
        protected const int WH_KEYBOARD_LL = 0x000D;
        protected const int WM_KEYDOWN = 0x0100;
        protected const int WM_KEYUP = 0x0101;
        protected const int WM_SYSKEYDOWN = 0x0104;
        protected const int WM_SYSKEYUP = 0x0105;

        [StructLayout(LayoutKind.Sequential)]
        public class KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [Flags]
        public enum KBDLLHOOKSTRUCTFlags : uint
        {
            KEYEVENTF_EXTENDEDKEY = 0x0001,
            KEYEVENTF_KEYUP = 0x0002,
            KEYEVENTF_SCANCODE = 0x0008,
            KEYEVENTF_UNICODE = 0x0004,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private KeyboardProc proc;
        private IntPtr hookId = IntPtr.Zero;

        public void Hook()
        {
            if (hookId == IntPtr.Zero)
            {
                proc = HookProcedure;
                using (var curProcess = Process.GetCurrentProcess())
                {
                    using (ProcessModule curModule = curProcess.MainModule)
                    {
                        hookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                    }
                }
            }
        }

        public void UnHook()
        {
            UnhookWindowsHookEx(hookId);
            hookId = IntPtr.Zero;
        }

        public IntPtr HookProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                var kb = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                var vkCode = (int)kb.vkCode;
                OnKeyDownEvent(vkCode);
            }
            else if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP))
            {
                var kb = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                var vkCode = (int)kb.vkCode;
                OnKeyUpEvent(vkCode);
            }
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        public delegate void KeyEventHandler(object sender, KeyEventArg e);
        public event KeyEventHandler KeyDownEvent;
        public event KeyEventHandler KeyUpEvent;

        protected void OnKeyDownEvent(int keyCode)
        {
            KeyDownEvent?.Invoke(this, new KeyEventArg(keyCode));
        }
        protected void OnKeyUpEvent(int keyCode)
        {
            KeyUpEvent?.Invoke(this, new KeyEventArg(keyCode));
        }

    }
    
    public class KeyEventArg : EventArgs
    {
        public int KeyCode { get; }

        public KeyEventArg(int keyCode)
        {
            KeyCode = keyCode;
        }
    }
    


}
