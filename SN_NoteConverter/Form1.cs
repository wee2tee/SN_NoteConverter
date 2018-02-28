using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CC;
using SN_NoteConverter.Model;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
//using SN_NoteConverter.Model2;


namespace SN_NoteConverter
{
    public partial class Form1 : Form
    {
        private enum TableName
        {
            USERS,
            NOTE_ISTAB,
            NOTE,
            NOTE_COMMENT,
            NOTE_CALENDAR,
            EVENT_CALENDAR,
            TRAINING_CALENDAR,
            CLOUD_SRV,
            MA,
            MAC_ALLOWED,
            SERIAL_PASSWORD
        }

        private TableName tbl_name;
        private BindingList<users> old_users;
        private BindingList<istab> istab;
        private BindingList<SupportNote_Min> support_note;
        private BindingList<support_note_comment> support_note_comment;
        private BindingList<note_calendar> note_calendar;
        private BindingList<event_calendar> event_calendar;
        private BindingList<training_calendar> training_calendar;
        private BindingList<cloud_srv> cloud_srv;
        private BindingList<ma> ma;
        private BindingList<mac_allowed> mac_allowed;
        private BindingList<serial_password> serial_password;
        private BackgroundWorker wrk;
        private List<users> users;
        private List<serial_minimum> serials;
        private string new_db_server { get { return this.txtNewServerName.Text; } }
        private string new_db_name { get { return this.txtNewDbName.Text; } }
        private string new_uid { get { return this.txtNewUid.Text; } }
        private string new_pwd { get { return this.txtNewPassword.Text; } }
        private string conn_str_to_new_server;
        MySqlConnection conn_to_new_server;
        MySqlCommand cmd;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Enum.GetValues(typeof(TableName)).Cast<TableName>().ToList().ForEach(i => this.comboBox1.Items.Add(new XDropdownListItem { Text = i.ToString(), Value = i }));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.tbl_name = (TableName)((XDropdownListItem)((ComboBox)sender).SelectedItem).Value;
            this.btnLoadOldData.Enabled = true;
        }

        private void btnLoadOldData_Click(object sender, EventArgs e)
        {
            try
            {
                using (sn_netEntities sn = DBX.DataSet(this.txtOldSrv.Text, this.txtOldDB.Text, this.txtOldUid.Text, this.txtOldPwd.Text))
                {
                    this.users = sn.users.ToList();
                    //this.serials = sn.serial.OrderBy(s => s.sernum).Select(s => new serial_minimum { id = s.id, sernum = s.sernum }).ToList();
                    //Console.WriteLine(" => users.count : " + this.users.Count + "\n => serials.count : " + this.serials.Count);

                    switch (this.tbl_name)
                    {
                        case TableName.USERS:
                            this.old_users = new BindingList<users>(sn.users.ToList());
                            this.dgv1.DataSource = this.old_users;
                            break;
                        case TableName.NOTE_ISTAB:
                            this.istab = new BindingList<istab>(sn.istab.Where(i => i.tabtyp == "06" || i.tabtyp == "07").OrderBy(i => i.id).ToList());
                            this.dgv1.DataSource = this.istab;
                            break;
                        case TableName.NOTE:
                            this.support_note = new BindingList<SupportNote_Min>(sn.support_note.OrderBy(s => s.id).Select(s => new SupportNote_Min { id = s.id, support_note = s, date = s.date, username = s.users_name }).ToList());
                            Console.WriteLine(" ===> Load note complete");
                            this.dgv1.DataSource = this.support_note;
                            break;
                        case TableName.NOTE_COMMENT:
                            this.support_note_comment = new BindingList<support_note_comment>(sn.support_note_comment.OrderBy(s => s.id).ToList());
                            this.dgv1.DataSource = this.support_note_comment;
                            break;
                        case TableName.NOTE_CALENDAR:
                            this.note_calendar = new BindingList<note_calendar>(sn.note_calendar.OrderBy(n => n.id).ToList());
                            this.dgv1.DataSource = this.note_calendar;
                            break;
                        case TableName.EVENT_CALENDAR:
                            this.event_calendar = new BindingList<event_calendar>(sn.event_calendar.OrderBy(i => i.id).ToList());
                            this.dgv1.DataSource = this.event_calendar;
                            break;
                        case TableName.TRAINING_CALENDAR:
                            this.training_calendar = new BindingList<training_calendar>(sn.training_calendar.OrderBy(t => t.id).ToList());
                            this.dgv1.DataSource = this.training_calendar;
                            break;
                        case TableName.CLOUD_SRV:
                            this.cloud_srv = new BindingList<cloud_srv>(sn.cloud_srv.OrderBy(c => c.id).ToList());
                            this.dgv1.DataSource = this.cloud_srv;
                            break;
                        case TableName.MA:
                            this.ma = new BindingList<ma>(sn.ma.OrderBy(m => m.id).ToList());
                            this.dgv1.DataSource = this.ma;
                            break;
                        case TableName.MAC_ALLOWED:
                            this.mac_allowed = new BindingList<mac_allowed>(sn.mac_allowed.OrderBy(m => m.id).ToList());
                            this.dgv1.DataSource = this.mac_allowed;
                            break;
                        case TableName.SERIAL_PASSWORD:
                            this.serial_password = new BindingList<serial_password>(sn.serial_password.OrderBy(s => s.id).ToList());
                            this.dgv1.DataSource = this.serial_password;
                            break;
                        default:
                            break;
                    }

                ((Button)sender).Enabled = false;
                    this.btnStartImport.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStartImport_Click(object sender, EventArgs e)
        {
            try
            {
                this.conn_str_to_new_server = "Server=" + this.new_db_server + ";Database=" + this.new_db_name + ";Uid=" + this.new_uid + ";Pwd=" + this.new_pwd + ";Charset=utf8";
                this.conn_to_new_server = new MySqlConnection(this.conn_str_to_new_server);
                this.conn_to_new_server.Open();
                this.btnLoadOldData.Enabled = false;
                ((Button)sender).Enabled = false;
                this.btnStopImport.Enabled = true;

                switch (this.tbl_name)
                {
                    case TableName.USERS:
                        this.ImportUsers();
                        break;
                    case TableName.NOTE_ISTAB:
                        this.ImportIstab();
                        break;
                    case TableName.NOTE:
                        this.ImportNote();
                        break;
                    case TableName.NOTE_COMMENT:
                        this.ImportNoteComment();
                        break;
                    case TableName.NOTE_CALENDAR:
                        this.ImportNoteCalendar();
                        break;
                    case TableName.EVENT_CALENDAR:
                        this.ImportEventCalendar();
                        break;
                    case TableName.TRAINING_CALENDAR:
                        this.ImportTrainingCalendar();
                        break;
                    case TableName.CLOUD_SRV:
                        this.ImportCloud_Srv();
                        break;
                    case TableName.MA:
                        this.ImportMa();
                        break;
                    case TableName.MAC_ALLOWED:
                        this.ImportMac_Allowed();
                        break;
                    case TableName.SERIAL_PASSWORD:
                        this.ImportSerial_Password();
                        break;
                    default:
                        MessageBox.Show("Please select table to import");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void ImportUsers()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    for (int i = 0; i < this.users.Count; i++)
                    {
                        try
                        {
                            if (this.wrk.CancellationPending)
                            {
                                e.Cancel = true;
                                this.btnLoadOldData.Enabled = true;
                                this.btnStopImport.Enabled = false;
                                return;
                            }

                            this.cmd = this.conn_to_new_server.CreateCommand();
                            this.cmd.CommandText = "Insert into users (id, username, userpassword, name, email, level, usergroup_id, status, allowed_web_login, training_expert, max_absent, create_at, last_use, rec_by) values(@id, @username, @userpassword, @name, @email, @level, @usergroup_id, @status, @allowed_web_login, @training_expert, @max_absent, @create_at, @last_use, @rec_by)";
                            this.cmd.Parameters.AddWithValue("@id", this.users[i].id);
                            this.cmd.Parameters.AddWithValue("@username", this.users[i].username);
                            this.cmd.Parameters.AddWithValue("@userpassword", this.users[i].username.EncryptToBytesString());
                            this.cmd.Parameters.AddWithValue("@name", this.users[i].name);
                            this.cmd.Parameters.AddWithValue("@email", this.users[i].email);
                            this.cmd.Parameters.AddWithValue("@level", this.users[i].level);
                            this.cmd.Parameters.AddWithValue("@usergroup_id", null);
                            this.cmd.Parameters.AddWithValue("@status", this.users[i].status);
                            this.cmd.Parameters.AddWithValue("@allowed_web_login", this.users[i].allowed_web_login);
                            this.cmd.Parameters.AddWithValue("@training_expert", this.users[i].training_expert);
                            this.cmd.Parameters.AddWithValue("@max_absent", this.users[i].max_absent);
                            this.cmd.Parameters.AddWithValue("@create_at", this.users[i].create_at);
                            this.cmd.Parameters.AddWithValue("@last_use", this.users[i].last_use);
                            this.cmd.Parameters.AddWithValue("@rec_by", this.users[i].rec_by);
                            this.cmd.ExecuteNonQuery();

                            this.wrk.ReportProgress(i + 1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                };
                this.wrk.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                {
                    MessageBox.Show("Import completed.");
                    this.btnLoadOldData.Enabled = true;
                    this.btnStartImport.Enabled = false;
                    this.btnStopImport.Enabled = false;
                    this.conn_to_new_server.Close();
                };
                this.wrk.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
                {
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.users.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void ImportIstab()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    for (int i = 0; i < this.istab.Count; i++)
                    {
                        try
                        {
                            if (this.wrk.CancellationPending)
                            {
                                e.Cancel = true;
                                this.btnLoadOldData.Enabled = true;
                                this.btnStopImport.Enabled = false;
                                return;
                            }

                            this.cmd = this.conn_to_new_server.CreateCommand();
                            this.cmd.CommandText = "Insert into note_istab (id, abbreviate_en, abbreviate_th, chgby, chgdat, creby, credat, tabtyp, typcod, typdes_en, typdes_th, use_pattern) values(@id, @abbr_en, @abbr_th, @chgby, @chgdat, @creby, @credat, @tabtyp, @typcod, @typdes_en, @typdes_th, @use_pattern)";
                            this.cmd.Parameters.AddWithValue("@id", this.istab[i].id);
                            this.cmd.Parameters.AddWithValue("@abbr_en", this.istab[i].abbreviate_en);
                            this.cmd.Parameters.AddWithValue("@abbr_th", this.istab[i].abbreviate_th);
                            this.cmd.Parameters.AddWithValue("@chgby", null);
                            this.cmd.Parameters.AddWithValue("@chgdat", null);
                            this.cmd.Parameters.AddWithValue("@creby", string.Empty);
                            this.cmd.Parameters.AddWithValue("@credat", DateTime.Now.Date);
                            this.cmd.Parameters.AddWithValue("@tabtyp", this.istab[i].tabtyp);
                            this.cmd.Parameters.AddWithValue("@typcod", this.istab[i].typcod);
                            this.cmd.Parameters.AddWithValue("@typdes_en", this.istab[i].typdes_en);
                            this.cmd.Parameters.AddWithValue("@typdes_th", this.istab[i].typdes_th);
                            this.cmd.Parameters.AddWithValue("@use_pattern", false);
                            this.cmd.ExecuteNonQuery();

                            this.wrk.ReportProgress(i + 1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                };
                this.wrk.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                {
                    MessageBox.Show("Import completed.");
                    this.btnLoadOldData.Enabled = true;
                    this.btnStartImport.Enabled = false;
                    this.btnStopImport.Enabled = false;
                    this.conn_to_new_server.Close();
                };
                this.wrk.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
                {
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.istab.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void ImportNote()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                int ndx = 0;

                try
                {
                    MySqlCommand command = this.conn_to_new_server.CreateCommand();
                    command.CommandText = "Select id From note Order By id DESC Limit 0,1";
                    MySqlDataReader reader = command.ExecuteReader();
                    int id = -1;
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader["id"]);
                    }

                    if(id > -1)
                    {
                        var last_imported_note = this.support_note.Where(s => s.id == id).FirstOrDefault();
                        if(last_imported_note != null)
                        {
                            ndx = this.support_note.IndexOf(last_imported_note) + 1;
                        }
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    for (int i = ndx; i < this.support_note.Count; i++)
                    {
                        try
                        {
                            if (this.wrk.CancellationPending)
                            {
                                e.Cancel = true;
                                this.btnLoadOldData.Enabled = true;
                                this.btnStopImport.Enabled = false;
                                return;
                            }

                            var user = this.users.Where(u => u.username.Trim() == this.support_note[i].support_note.users_name.Trim()).FirstOrDefault();

                            this.cmd = this.conn_to_new_server.CreateCommand();
                            this.cmd.CommandText = "Insert into note (id, users_id, date, users_name, sernum, contact, start_time, end_time, duration, problem, remark, is_break, reason, file_path, rec_by) values (@id, @users_id, @date, @users_name, @sernum, @contact, @start_time, @end_time, @duration, @problem, @remark, @is_break, @reason, @file_path, @rec_by)";
                            this.cmd.Parameters.AddWithValue("@id", this.support_note[i].support_note.id);
                            this.cmd.Parameters.AddWithValue("@users_id", (user != null ? (int?)user.id : null));
                            this.cmd.Parameters.AddWithValue("@date", this.support_note[i].support_note.date);
                            this.cmd.Parameters.AddWithValue("@users_name", this.support_note[i].support_note.users_name);
                            this.cmd.Parameters.AddWithValue("@sernum", this.support_note[i].support_note.sernum);
                            this.cmd.Parameters.AddWithValue("@contact", this.support_note[i].support_note.contact);
                            this.cmd.Parameters.AddWithValue("@start_time", this.support_note[i].support_note.start_time);
                            this.cmd.Parameters.AddWithValue("@end_time", this.support_note[i].support_note.end_time);
                            this.cmd.Parameters.AddWithValue("@duration", this.support_note[i].support_note.duration);
                            this.cmd.Parameters.AddWithValue("@problem", this.support_note[i].support_note.problem);
                            this.cmd.Parameters.AddWithValue("@remark", this.support_note[i].support_note.remark);
                            this.cmd.Parameters.AddWithValue("@is_break", this.support_note[i].support_note.is_break);
                            this.cmd.Parameters.AddWithValue("@reason", this.support_note[i].support_note.reason);
                            this.cmd.Parameters.AddWithValue("@file_path", this.support_note[i].support_note.file_path);
                            this.cmd.Parameters.AddWithValue("@rec_by", this.support_note[i].support_note.rec_by);
                            this.cmd.ExecuteNonQuery();

                            this.wrk.ReportProgress(i + 1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                };
                this.wrk.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                {
                    MessageBox.Show("Import completed.");
                    this.btnLoadOldData.Enabled = true;
                    this.btnStartImport.Enabled = false;
                    this.btnStopImport.Enabled = false;
                    this.conn_to_new_server.Close();
                };
                this.wrk.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
                {
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.support_note.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void ImportNoteComment()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    for (int i = 0; i < this.support_note_comment.Count; i++)
                    {
                        try
                        {
                            if (this.wrk.CancellationPending)
                            {
                                e.Cancel = true;
                                this.btnLoadOldData.Enabled = true;
                                this.btnStopImport.Enabled = false;
                                return;
                            }

                            this.cmd = this.conn_to_new_server.CreateCommand();
                            this.cmd.CommandText = "Insert into note_comment (id, date, description, file_path, note_id, rec_by, type) values (@id, @date, @description, @file_path, @note_id, @rec_by, @type)";
                            this.cmd.Parameters.AddWithValue("@id", this.support_note_comment[i].id);
                            this.cmd.Parameters.AddWithValue("@date", this.support_note_comment[i].date);
                            this.cmd.Parameters.AddWithValue("@description", this.support_note_comment[i].description);
                            this.cmd.Parameters.AddWithValue("@file_path", this.support_note_comment[i].file_path);
                            this.cmd.Parameters.AddWithValue("@note_id", this.support_note_comment[i].note_id);
                            this.cmd.Parameters.AddWithValue("@rec_by", this.support_note_comment[i].rec_by);
                            this.cmd.Parameters.AddWithValue("@type", this.support_note_comment[i].type);
                            this.cmd.ExecuteNonQuery();

                            this.wrk.ReportProgress(i + 1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                };
                this.wrk.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                {
                    MessageBox.Show("Import completed.");
                    this.btnLoadOldData.Enabled = true;
                    this.btnStartImport.Enabled = false;
                    this.btnStopImport.Enabled = false;
                    //note.Dispose();
                    this.conn_to_new_server.Close();
                };
                this.wrk.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
                {
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.support_note_comment.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void ImportNoteCalendar()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                //this.conn_to_new_server.Open();
                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    for (int i = 0; i < this.note_calendar.Count; i++)
                    {
                        try
                        {
                            if (this.wrk.CancellationPending)
                            {
                                e.Cancel = true;
                                this.btnLoadOldData.Enabled = true;
                                this.btnStopImport.Enabled = false;
                                return;
                            }

                            this.cmd = this.conn_to_new_server.CreateCommand();
                            this.cmd.CommandText = "Insert into note_calendar (id, date, description, group_maid, group_weekend, max_leave, rec_by, type) values (@id, @date, @description, @group_maid, @group_weekend, @max_leave, @rec_by, @type)";
                            this.cmd.Parameters.AddWithValue("@id", this.note_calendar[i].id);
                            this.cmd.Parameters.AddWithValue("@date", this.note_calendar[i].date);
                            this.cmd.Parameters.AddWithValue("@description", this.note_calendar[i].description);
                            this.cmd.Parameters.AddWithValue("@group_maid", this.note_calendar[i].group_maid);
                            this.cmd.Parameters.AddWithValue("@group_weekend", this.note_calendar[i].group_weekend);
                            this.cmd.Parameters.AddWithValue("@max_leave", this.note_calendar[i].max_leave);
                            this.cmd.Parameters.AddWithValue("@rec_by", this.note_calendar[i].rec_by);
                            this.cmd.Parameters.AddWithValue("@type", this.note_calendar[i].type);
                            this.cmd.ExecuteNonQuery();

                            this.wrk.ReportProgress(i + 1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                };
                this.wrk.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                {
                    MessageBox.Show("Import completed.");
                    this.btnLoadOldData.Enabled = true;
                    this.btnStartImport.Enabled = false;
                    this.btnStopImport.Enabled = false;
                    this.conn_to_new_server.Close();
                };
                this.wrk.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
                {
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.note_calendar.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void ImportEventCalendar()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                //this.conn_to_new_server.Open();
                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    List<XNote_Istab> istabs = new List<XNote_Istab>();
                    MySqlCommand cmd_istab = this.conn_to_new_server.CreateCommand();
                    cmd_istab.CommandText = "Select * From note_istab";
                    MySqlDataReader reader = cmd_istab.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            istabs.Add(new XNote_Istab
                            {
                                id = (int)reader["id"],
                                abbreviate_en = (string)reader["abbreviate_en"],
                                abbreviate_th = (string)reader["abbreviate_th"],
                                chgby = reader.IsDBNull(reader.GetOrdinal("chgby")) ? null : (string)reader["chgby"],
                                chgdat = reader.IsDBNull(reader.GetOrdinal("chgdat")) ? null : (DateTime?)reader["chgdat"],
                                creby = (string)reader["creby"],
                                credat = (DateTime)reader["credat"],
                                tabtyp = (string)reader["tabtyp"],
                                typcod = (string)reader["typcod"],
                                typdes_en = (string)reader["typdes_en"],
                                typdes_th = (string)reader["typdes_th"],
                                use_pattern = (bool)reader["use_pattern"]
                            });
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(" ==> " + ex.Message);
                    }

                    for (int i = 0; i < this.event_calendar.Count; i++)
                    {
                        try
                        {
                            if (this.wrk.CancellationPending)
                            {
                                e.Cancel = true;
                                this.btnLoadOldData.Enabled = true;
                                this.btnStopImport.Enabled = false;
                                return;
                            }
                            var istab = istabs.Where(x => x.tabtyp == this.event_calendar[i].event_type && x.typcod == this.event_calendar[i].event_code).FirstOrDefault();

                            this.cmd = this.conn_to_new_server.CreateCommand();
                            this.cmd.CommandText = "Insert into event_calendar (id, customer, date, event_code, event_code_id, event_type, fine, from_time, med_cert, realname, rec_by, series, status, to_time, users_name) values (@id, @customer, @date, @event_code, @event_code_id, @event_type, @fine, @from_time, @med_cert, @realname, @rec_by, @series, @status, @to_time, @users_name)";
                            this.cmd.Parameters.AddWithValue("@id", this.event_calendar[i].id);
                            this.cmd.Parameters.AddWithValue("@customer", this.event_calendar[i].customer);
                            this.cmd.Parameters.AddWithValue("@date", this.event_calendar[i].date);
                            this.cmd.Parameters.AddWithValue("@event_code", this.event_calendar[i].event_code);
                            this.cmd.Parameters.AddWithValue("@event_code_id", istab != null ? (int?)istab.id : null);
                            this.cmd.Parameters.AddWithValue("@event_type", this.event_calendar[i].event_type);
                            this.cmd.Parameters.AddWithValue("@fine", this.event_calendar[i].fine);
                            this.cmd.Parameters.AddWithValue("@from_time", this.event_calendar[i].from_time.Substring(0, 5));
                            this.cmd.Parameters.AddWithValue("@med_cert", this.event_calendar[i].med_cert);
                            this.cmd.Parameters.AddWithValue("@realname", this.event_calendar[i].realname);
                            this.cmd.Parameters.AddWithValue("@rec_by", this.event_calendar[i].rec_by);
                            this.cmd.Parameters.AddWithValue("@series", null);
                            this.cmd.Parameters.AddWithValue("@status", this.event_calendar[i].status);
                            this.cmd.Parameters.AddWithValue("@to_time", this.event_calendar[i].to_time.Substring(0, 5));
                            this.cmd.Parameters.AddWithValue("@users_name", this.event_calendar[i].users_name);
                            this.cmd.ExecuteNonQuery();

                            this.wrk.ReportProgress(i + 1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                };
                this.wrk.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                {
                    MessageBox.Show("Import completed.");
                    this.btnLoadOldData.Enabled = true;
                    this.btnStartImport.Enabled = false;
                    this.btnStopImport.Enabled = false;
                    this.conn_to_new_server.Close();
                };
                this.wrk.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
                {
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.event_calendar.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void ImportTrainingCalendar()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    for (int i = 0; i < this.training_calendar.Count; i++)
                    {
                        try
                        {
                            if (this.wrk.CancellationPending)
                            {
                                e.Cancel = true;
                                this.btnLoadOldData.Enabled = true;
                                this.btnStopImport.Enabled = false;
                                return;
                            }
                            this.cmd = this.conn_to_new_server.CreateCommand();
                            this.cmd.CommandText = "Insert into training_calendar (id, course_type, date, rec_by, remark, status, term, trainer) values (@id, @course_type, @date, @rec_by, @remark, @status, @term, @trainer)";
                            this.cmd.Parameters.AddWithValue("@id", this.training_calendar[i].id);
                            this.cmd.Parameters.AddWithValue("@course_type", this.training_calendar[i].course_type);
                            this.cmd.Parameters.AddWithValue("@date", this.training_calendar[i].date);
                            this.cmd.Parameters.AddWithValue("@rec_by", this.training_calendar[i].rec_by);
                            this.cmd.Parameters.AddWithValue("@remark", this.training_calendar[i].remark);
                            this.cmd.Parameters.AddWithValue("@status", this.training_calendar[i].status);
                            this.cmd.Parameters.AddWithValue("@term", this.training_calendar[i].term);
                            this.cmd.Parameters.AddWithValue("@trainer", this.training_calendar[i].trainer);
                            this.cmd.ExecuteNonQuery();

                            this.wrk.ReportProgress(i + 1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                };
                this.wrk.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                {
                    MessageBox.Show("Import completed.");
                    this.btnLoadOldData.Enabled = true;
                    this.btnStartImport.Enabled = false;
                    this.btnStopImport.Enabled = false;
                    this.conn_to_new_server.Close();
                };
                this.wrk.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
                {
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.training_calendar.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void ImportCloud_Srv()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    this.serials = this.GetSerialMin();

                    for (int i = 0; i < this.cloud_srv.Count; i++)
                    {
                        try
                        {
                            if (this.wrk.CancellationPending)
                            {
                                e.Cancel = true;
                                this.btnLoadOldData.Enabled = true;
                                this.btnStopImport.Enabled = false;
                                return;
                            }

                            var ser = this.serials.Where(s => s.sernum.Trim() == this.cloud_srv[i].sernum.Trim()).FirstOrDefault();
                            var creby = this.users.Where(u => u.username.Trim() == this.cloud_srv[i].rec_by.Trim()).FirstOrDefault();
                            //Console.WriteLine(" => begin record cloud_srv : " + this.cloud_srv[i].sernum);
                            this.cmd = this.conn_to_new_server.CreateCommand();
                            this.cmd.CommandText = "Insert into cloud_srv (id, start_date, end_date, email, serial_id, creby_id, credat, chgby_id, chgdat, flag) values (@id, @start_date, @end_date, @email, @serial_id, @creby_id, @credat, @chgby_id, @chgdat, @flag)";
                            this.cmd.Parameters.AddWithValue("@id", this.cloud_srv[i].id);
                            this.cmd.Parameters.AddWithValue("@start_date", this.cloud_srv[i].start_date);
                            this.cmd.Parameters.AddWithValue("@end_date", this.cloud_srv[i].end_date);
                            this.cmd.Parameters.AddWithValue("@email", this.cloud_srv[i].email);
                            this.cmd.Parameters.AddWithValue("@serial_id", ser != null ? (int?)ser.id : null);
                            this.cmd.Parameters.AddWithValue("@creby_id", creby != null ? (int?)creby.id : null);
                            this.cmd.Parameters.AddWithValue("@credat", this.cloud_srv[i].rec_date);
                            this.cmd.Parameters.AddWithValue("@chgby_id", null);
                            this.cmd.Parameters.AddWithValue("@chgdat", null);
                            this.cmd.Parameters.AddWithValue("@flag", 0);
                            this.cmd.ExecuteNonQuery();

                            this.wrk.ReportProgress(i + 1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                };
                this.wrk.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                {
                    MessageBox.Show("Import completed.");
                    this.btnLoadOldData.Enabled = true;
                    this.btnStartImport.Enabled = false;
                    this.btnStopImport.Enabled = false;
                    this.conn_to_new_server.Close();
                };
                this.wrk.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
                {
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.cloud_srv.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void ImportMa()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    this.serials = this.GetSerialMin();

                    for (int i = 0; i < this.ma.Count; i++)
                    {
                        try
                        {
                            if (this.wrk.CancellationPending)
                            {
                                e.Cancel = true;
                                this.btnLoadOldData.Enabled = true;
                                this.btnStopImport.Enabled = false;
                                return;
                            }

                            var ser = this.serials.Where(s => s.sernum.Trim() == this.ma[i].sernum.Trim()).FirstOrDefault();
                            var creby = this.users.Where(u => u.username.Trim() == this.ma[i].rec_by.Trim()).FirstOrDefault();
                            
                            this.cmd = this.conn_to_new_server.CreateCommand();
                            this.cmd.CommandText = "Insert into ma (id, start_date, end_date, email, serial_id, creby_id, credat, chgby_id, chgdat, flag) values (@id, @start_date, @end_date, @email, @serial_id, @creby_id, @credat, @chgby_id, @chgdat, @flag)";
                            this.cmd.Parameters.AddWithValue("@id", this.ma[i].id);
                            this.cmd.Parameters.AddWithValue("@start_date", this.ma[i].start_date);
                            this.cmd.Parameters.AddWithValue("@end_date", this.ma[i].end_date);
                            this.cmd.Parameters.AddWithValue("@email", this.ma[i].email);
                            this.cmd.Parameters.AddWithValue("@serial_id", ser != null ? (int?)ser.id : null);
                            this.cmd.Parameters.AddWithValue("@creby_id", creby != null ? (int?)creby.id : null);
                            this.cmd.Parameters.AddWithValue("@credat", this.ma[i].rec_date);
                            this.cmd.Parameters.AddWithValue("@chgby_id", null);
                            this.cmd.Parameters.AddWithValue("@chgdat", null);
                            this.cmd.Parameters.AddWithValue("@flag", 0);
                            this.cmd.ExecuteNonQuery();

                            this.wrk.ReportProgress(i + 1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                };
                this.wrk.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                {
                    MessageBox.Show("Import completed.");
                    this.btnLoadOldData.Enabled = true;
                    this.btnStartImport.Enabled = false;
                    this.btnStopImport.Enabled = false;
                    this.conn_to_new_server.Close();
                };
                this.wrk.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
                {
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.ma.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void ImportMac_Allowed()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    this.serials = this.GetSerialMin();

                    for (int i = 0; i < this.ma.Count; i++)
                    {
                        try
                        {
                            if (this.wrk.CancellationPending)
                            {
                                e.Cancel = true;
                                this.btnLoadOldData.Enabled = true;
                                this.btnStopImport.Enabled = false;
                                return;
                            }

                            var ser = this.serials.Where(s => s.sernum.Trim() == this.ma[i].sernum.Trim()).FirstOrDefault();
                            var creby = this.users.Where(u => u.username.Trim() == this.ma[i].rec_by.Trim()).FirstOrDefault();
                            
                            this.cmd = this.conn_to_new_server.CreateCommand();
                            //this.cmd.CommandText = "Insert into ma (id, start_date, end_date, email, serial_id, creby_id, credat, chgby_id, chgdat, flag) values (@id, @start_date, @end_date, @email, @serial_id, @creby_id, @credat, @chgby_id, @chgdat, @flag)";
                            //this.cmd.Parameters.AddWithValue("@id", this.ma[i].id);
                            //this.cmd.Parameters.AddWithValue("@start_date", this.ma[i].start_date);
                            //this.cmd.Parameters.AddWithValue("@end_date", this.ma[i].end_date);
                            //this.cmd.Parameters.AddWithValue("@email", this.ma[i].email);
                            //this.cmd.Parameters.AddWithValue("@serial_id", ser != null ? (int?)ser.id : null);
                            //this.cmd.Parameters.AddWithValue("@creby_id", creby != null ? (int?)creby.id : null);
                            //this.cmd.Parameters.AddWithValue("@credat", this.ma[i].rec_date);
                            //this.cmd.Parameters.AddWithValue("@chgby_id", null);
                            //this.cmd.Parameters.AddWithValue("@chgdat", null);
                            //this.cmd.Parameters.AddWithValue("@flag", 0);
                            //this.cmd.ExecuteNonQuery();

                            this.wrk.ReportProgress(i + 1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                };
                this.wrk.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                {
                    MessageBox.Show("Import completed.");
                    this.btnLoadOldData.Enabled = true;
                    this.btnStartImport.Enabled = false;
                    this.btnStopImport.Enabled = false;
                    this.conn_to_new_server.Close();
                };
                this.wrk.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
                {
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.ma.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void ImportSerial_Password()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    this.serials = this.GetSerialMin();

                    for (int i = 0; i < this.ma.Count; i++)
                    {
                        try
                        {
                            if (this.wrk.CancellationPending)
                            {
                                e.Cancel = true;
                                this.btnLoadOldData.Enabled = true;
                                this.btnStopImport.Enabled = false;
                                return;
                            }

                            var ser = this.serials.Where(s => s.sernum.Trim() == this.ma[i].sernum.Trim()).FirstOrDefault();
                            var creby = this.users.Where(u => u.username.Trim() == this.ma[i].rec_by.Trim()).FirstOrDefault();
                            
                            this.cmd = this.conn_to_new_server.CreateCommand();
                            //this.cmd.CommandText = "Insert into ma (id, start_date, end_date, email, serial_id, creby_id, credat, chgby_id, chgdat, flag) values (@id, @start_date, @end_date, @email, @serial_id, @creby_id, @credat, @chgby_id, @chgdat, @flag)";
                            //this.cmd.Parameters.AddWithValue("@id", this.ma[i].id);
                            //this.cmd.Parameters.AddWithValue("@start_date", this.ma[i].start_date);
                            //this.cmd.Parameters.AddWithValue("@end_date", this.ma[i].end_date);
                            //this.cmd.Parameters.AddWithValue("@email", this.ma[i].email);
                            //this.cmd.Parameters.AddWithValue("@serial_id", ser != null ? (int?)ser.id : null);
                            //this.cmd.Parameters.AddWithValue("@creby_id", creby != null ? (int?)creby.id : null);
                            //this.cmd.Parameters.AddWithValue("@credat", this.ma[i].rec_date);
                            //this.cmd.Parameters.AddWithValue("@chgby_id", null);
                            //this.cmd.Parameters.AddWithValue("@chgdat", null);
                            //this.cmd.Parameters.AddWithValue("@flag", 0);
                            //this.cmd.ExecuteNonQuery();

                            this.wrk.ReportProgress(i + 1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                };
                this.wrk.RunWorkerCompleted += delegate (object sender, RunWorkerCompletedEventArgs e)
                {
                    MessageBox.Show("Import completed.");
                    this.btnLoadOldData.Enabled = true;
                    this.btnStartImport.Enabled = false;
                    this.btnStopImport.Enabled = false;
                    this.conn_to_new_server.Close();
                };
                this.wrk.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
                {
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.ma.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void btnStopImport_Click(object sender, EventArgs e)
        {
            if(this.wrk != null)
            {
                this.wrk.CancelAsync();
            }
        }

        private void chLock_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                this.txtNewDbName.Enabled = false;
                this.txtNewPassword.Enabled = false;
                this.txtNewServerName.Enabled = false;
                this.txtNewUid.Enabled = false;
            }
            else
            {
                this.txtNewDbName.Enabled = true;
                this.txtNewPassword.Enabled = true;
                this.txtNewServerName.Enabled = true;
                this.txtNewUid.Enabled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                this.txtOldPwd.Enabled = false;
                this.txtOldUid.Enabled = false;
                this.txtOldDB.Enabled = false;
                this.txtOldSrv.Enabled = false;
            }
            else
            {
                this.txtOldPwd.Enabled = true;
                this.txtOldUid.Enabled = true;
                this.txtOldDB.Enabled = true;
                this.txtOldSrv.Enabled = true;
            }
        }

        private List<serial_minimum> GetSerialMin()
        {
            List<serial_minimum> serials = new List<serial_minimum>();

            string conn_str = "Server=" + this.new_db_server + ";Database=" + this.new_db_name + ";Uid=" + this.new_uid + ";Pwd=" + this.new_pwd + ";Charset=utf8";
            using (MySqlConnection conn = new MySqlConnection(conn_str))
            {
                conn.Open();

                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "Select id, sernum From serial Order By sernum";
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var ser_min = new serial_minimum
                    {
                        id = Convert.ToInt32(rdr["id"]),
                        sernum = rdr["sernum"].ToString()
                    };
                    serials.Add(ser_min);
                }

                return serials;
            }
        }
    }

    public class SupportNote_Min
    {
        public int id { get; set; }
        public DateTime? date { get; set; }
        public string username { get; set; }
        public support_note support_note { get; set; }
    }

    public class XNote_Istab
    {
        public int id { get; set; }
        public string tabtyp { get; set; }
        public string typcod { get; set; }
        public string abbreviate_en { get; set; }
        public string abbreviate_th { get; set; }
        public string typdes_en { get; set; }
        public string typdes_th { get; set; }
        public bool use_pattern { get; set; }
        public string creby { get; set; }
        public DateTime credat { get; set; }
        public string chgby { get; set; }
        public DateTime? chgdat { get; set; }
    }

    public static class Helper
    {
        public static string EncryptToBytesString(this string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            string bytes_str = string.Empty;
            foreach (var b in bytes)
            {
                bytes_str += ((int)b).FillLeadingZero(4);
            }

            return bytes_str;
        }

        public static string DecryptFromBytesString(this string bytes_string)
        {
            IEnumerable<string> str = bytes_string.SplitInParts(4);

            List<byte> b = new List<byte>();
            foreach (var s in str)
            {
                b.Add((byte)Convert.ToInt32(s));
            }

            return Encoding.UTF8.GetString(b.ToArray());
        }

        public static string FillLeadingZero(this int source_string, int total_digit)
        {
            string result = string.Empty;

            for (int i = 0; i < total_digit - source_string.ToString().Length; i++)
            {
                result += "0";
            }

            return result + source_string.ToString();
        }

        public static IEnumerable<String> SplitInParts(this String s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", "partLength");

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }
    
    public class serial_minimum
    {
        public int id { get; set; }
        public string sernum { get; set; }
    }
}
