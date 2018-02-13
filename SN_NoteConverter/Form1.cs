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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
            using (BackgroundWorker wrk = new BackgroundWorker())
            {
                using (sn_netEntities sn = new sn_netEntities())
                {
                    switch (this.tbl_name)
                    {
                        case TableName.NOTE_ISTAB:
                            this.istab = new BindingList<istab>(sn.istab.Where(i => i.tabtyp == "06" || i.tabtyp == "07").ToList());
                            this.dgv1.DataSource = this.istab;
                            break;
                        case TableName.NOTE:
                            this.support_note = new BindingList<SupportNote_Min>(sn.support_note.Select(s => new SupportNote_Min { id = s.id, support_note = s, date = s.date, username = s.users_name }).ToList());
                            this.dgv1.DataSource = this.support_note;
                            break;
                        case TableName.NOTE_COMMENT:
                            this.support_note_comment = new BindingList<support_note_comment>(sn.support_note_comment.ToList());
                            this.dgv1.DataSource = this.support_note_comment;
                            break;
                        case TableName.NOTE_CALENDAR:
                            this.note_calendar = new BindingList<note_calendar>(sn.note_calendar.ToList());
                            this.dgv1.DataSource = this.note_calendar;
                            break;
                        case TableName.EVENT_CALENDAR:
                            this.event_calendar = new BindingList<event_calendar>(sn.event_calendar.ToList());
                            this.dgv1.DataSource = this.event_calendar;
                            break;
                        case TableName.TRAINING_CALENDAR:
                            this.training_calendar = new BindingList<training_calendar>(sn.training_calendar.ToList());
                            this.dgv1.DataSource = this.training_calendar;
                            break;
                        default:
                            break;
                    }

                    ((Button)sender).Enabled = false;
                    this.btnStartImport.Enabled = true;
                }
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
                sn_notesEntities note = new sn_notesEntities();
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

                            note.note_istab2.Add(new note_istab2
                            {
                                id = this.istab[i].id,
                                abbreviate_en = this.istab[i].abbreviate_en,
                                abbreviate_th = this.istab[i].abbreviate_th,
                                chgby = null,
                                chgdat = null,
                                creby = string.Empty,
                                credat = DateTime.Now.Date,
                                tabtyp = this.istab[i].tabtyp,
                                typcod = this.istab[i].typcod,
                                typdes_en = this.istab[i].typdes_en,
                                typdes_th = this.istab[i].typdes_th,
                                use_pattern = false
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
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.istab.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void ImportNote()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                sn_notesEntities note = new sn_notesEntities();
                this.wrk.DoWork += delegate (object sender, DoWorkEventArgs e)
                {
                    for (int i = 0; i < this.support_note.Count; i++)
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
                            note.note2.Add(new note2
                            {
                                id = this.support_note[i].support_note.id,
                                contact = this.support_note[i].support_note.contact,
                                date = this.support_note[i].support_note.date,
                                duration = this.support_note[i].support_note.duration,
                                end_time = this.support_note[i].support_note.end_time,
                                file_path = this.support_note[i].support_note.file_path,
                                is_break = this.support_note[i].support_note.is_break,
                                problem = this.support_note[i].support_note.problem,
                                reason = this.support_note[i].support_note.reason,
                                rec_by = this.support_note[i].support_note.rec_by,
                                remark = this.support_note[i].support_note.remark,
                                sernum = this.support_note[i].support_note.sernum,
                                start_time = this.support_note[i].support_note.start_time,
                                users_id = user != null ? (int?)user.id : null,
                                users_name = this.support_note[i].support_note.users_name
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
                    this.lblProgress.Text = e.ProgressPercentage.ToString() + " / " + this.support_note.Count.ToString();
                };
                this.wrk.RunWorkerAsync();
            }
        }

        private void ImportNoteComment()
        {
            using (this.wrk = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true })
            {
                sn_notesEntities note = new sn_notesEntities();
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

                            note.note_comment2.Add(new note_comment2
                            {
                                id = this.support_note_comment[i].id,
                                date = this.support_note_comment[i].date,
                                description = this.support_note_comment[i].description,
                                file_path = this.support_note_comment[i].file_path,
                                note_id = this.support_note_comment[i].note_id,
                                rec_by = this.support_note_comment[i].rec_by,
                                type = this.support_note_comment[i].type
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
