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
            NOTE_ISTAB,
            NOTE,
            NOTE_COMMENT,
            NOTE_CALENDAR,
            EVENT_CALENDAR,
            TRAINING_CALENDAR
        }

        private TableName tbl_name;
        private BindingList<istab> istab;
        private BindingList<SupportNote_Min> support_note;
        private BindingList<support_note_comment> support_note_comment;
        private BindingList<note_calendar> note_calendar;
        private BindingList<event_calendar> event_calendar;
        private BindingList<training_calendar> training_calendar;
        private BackgroundWorker wrk;
        private List<users> users;
        string conn_str_to_new_server = "Server=localhost;Database=sn_notes;Uid=root;Pwd=12345;Charset=utf8";
        MySqlConnection conn_to_new_server;
        MySqlCommand cmd;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.conn_to_new_server = new MySqlConnection(this.conn_str_to_new_server);
            Enum.GetValues(typeof(TableName)).Cast<TableName>().ToList().ForEach(i => this.comboBox1.Items.Add(new XDropdownListItem { Text = i.ToString(), Value = i }));
            using (sn_netEntities sn = new sn_netEntities())
            {
                this.users = sn.users.ToList();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.tbl_name = (TableName)((XDropdownListItem)((ComboBox)sender).SelectedItem).Value;
            this.btnLoadOldData.Enabled = true;
        }

        private void btnLoadOldData_Click(object sender, EventArgs e)
        {
            using (sn_netEntities sn = new sn_netEntities())
            {
                switch (this.tbl_name)
                {
                    case TableName.NOTE_ISTAB:
                        this.istab = new BindingList<istab>(sn.istab.Where(i => i.tabtyp == "06" || i.tabtyp == "07").OrderBy(i => i.id).ToList());
                        this.dgv1.DataSource = this.istab;
                        break;
                    case TableName.NOTE:
                        this.support_note = new BindingList<SupportNote_Min>(sn.support_note.OrderBy(s => s.id).Select(s => new SupportNote_Min { id = s.id, support_note = s, date = s.date, username = s.users_name }).ToList());
                        Console.WriteLine(" ===> Load complete");
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
                    default:
                        break;
                }

                ((Button)sender).Enabled = false;
                this.btnStartImport.Enabled = true;
            }
        }

        private void btnStartImport_Click(object sender, EventArgs e)
        {
            this.btnLoadOldData.Enabled = false;
            ((Button)sender).Enabled = false;
            this.btnStopImport.Enabled = true;

            switch (this.tbl_name)
            {
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
                default:
                    MessageBox.Show("Please select table to import");
                    break;
            }
        }

        private void ImportIstab()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                this.conn_to_new_server.Open();
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
                            this.cmd.CommandText = "Insert into note_istab2 (id, abbreviate_en, abbreviate_th, chgby, chgdat, creby, credat, tabtyp, typcod, typdes_en, typdes_th, use_pattern) values(@id, @abbr_en, @abbr_th, @chgby, @chgdat, @creby, @credat, @tabtyp, @typcod, @typdes_en, @typdes_th, @use_pattern)";
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
                this.conn_to_new_server.Open();
                //sn_notesEntities note = new sn_notesEntities();
                //var last_imported_note = note.note2.OrderByDescending(n => n.id).FirstOrDefault();
                MySqlCommand command = this.conn_to_new_server.CreateCommand();
                command.CommandText = "Select id from note2 Order By id DESC Limit 0, 1";
                MySqlDataReader reader = command.ExecuteReader();

                int ndx = 0;

                if (reader.HasRows)
                {
                    int last_id = -1;
                    while (reader.Read())
                    {
                        last_id = (int)reader["id"];
                    }

                    var ln = this.support_note.Where(n => n.id == last_id).FirstOrDefault();
                    if (ln != null)
                    {
                        ndx = this.support_note.IndexOf(ln) + 1;
                    }
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
                            this.cmd.CommandText = "Insert into note2 (id, users_id, date, users_name, sernum, contact, start_time, end_time, duration, problem, remark, is_break, reason, file_path, rec_by) values (@id, @users_id, @date, @users_name, @sernum, @contact, @start_time, @end_time, @duration, @problem, @remark, @is_break, @reason, @file_path, @rec_by)";
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
                    //note.Dispose();
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
                //sn_notesEntities note = new sn_notesEntities();
                this.conn_to_new_server.Open();
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
                            this.cmd.CommandText = "Insert into note_comment";
                            this.cmd.Parameters.AddWithValue("", this.support_note_comment[i].id);
                            //note.note_comment2.Add(new note_comment2
                            //{
                            //    id = this.support_note_comment[i].id,
                            //    date = this.support_note_comment[i].date,
                            //    description = this.support_note_comment[i].description,
                            //    file_path = this.support_note_comment[i].file_path,
                            //    note_id = this.support_note_comment[i].note_id,
                            //    rec_by = this.support_note_comment[i].rec_by,
                            //    type = this.support_note_comment[i].type
                            //});

                            //note.SaveChanges();

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
                sn_notesEntities note = new sn_notesEntities();
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

                            note.note_calendar2.Add(new note_calendar2
                            {
                                id = this.note_calendar[i].id,
                                date = this.note_calendar[i].date,
                                description = this.note_calendar[i].description,
                                group_maid = this.note_calendar[i].group_maid,
                                group_weekend = this.note_calendar[i].group_weekend,
                                max_leave = this.note_calendar[i].max_leave,
                                rec_by = this.note_calendar[i].rec_by,
                                type = this.note_calendar[i].type
                            });

                            note.SaveChanges();

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
                    note.Dispose();
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
                sn_notesEntities note = new sn_notesEntities();
                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    List<note_istab2> istab = note.note_istab2.ToList();

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

                            note_istab2 istab_event = istab.Where(ev => ev.tabtyp == this.event_calendar[i].event_type && ev.typcod == this.event_calendar[i].event_code).FirstOrDefault();
                            note.event_calendar2.Add(new event_calendar2
                            {
                                id = this.event_calendar[i].id,
                                customer = this.event_calendar[i].customer,
                                date = this.event_calendar[i].date,
                                event_code = this.event_calendar[i].event_code,
                                event_code_id = istab_event != null ? (int?)istab_event.id : null,
                                event_type = this.event_calendar[i].event_type,
                                fine = this.event_calendar[i].fine,
                                from_time = this.event_calendar[i].from_time.Substring(0,5),
                                med_cert = this.event_calendar[i].med_cert,
                                realname = this.event_calendar[i].realname,
                                rec_by = this.event_calendar[i].rec_by,
                                series = null,
                                status = this.event_calendar[i].status,
                                to_time = this.event_calendar[i].to_time.Substring(0,5),
                                users_name = this.event_calendar[i].users_name
                            });

                            note.SaveChanges();

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
                    note.Dispose();
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
                sn_notesEntities note = new sn_notesEntities();
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

                            note.training_calendar2.Add(new training_calendar2
                            {
                                id = this.training_calendar[i].id,
                                course_type = this.training_calendar[i].course_type,
                                date = this.training_calendar[i].date,
                                rec_by = this.training_calendar[i].rec_by,
                                remark = this.training_calendar[i].remark,
                                status = this.training_calendar[i].status,
                                term = this.training_calendar[i].term,
                                trainer = this.training_calendar[i].trainer
                            });

                            note.SaveChanges();

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
                    note.Dispose();
                };
                this.wrk.ProgressChanged += delegate (object sender, ProgressChangedEventArgs e)
                {
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.training_calendar.Count.ToString();
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
    }

    public class SupportNote_Min
    {
        public int id { get; set; }
        public DateTime? date { get; set; }
        public string username { get; set; }
        public support_note support_note { get; set; }
    }
}
