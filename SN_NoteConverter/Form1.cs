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
            using (sn_netEntities sn = new sn_netEntities())
            {
                switch (this.tbl_name)
                {
                    case TableName.NOTE_ISTAB:
                        BindingList<istab> l1 = new BindingList<istab>(sn.istab.Where(i => i.tabtyp == "06" || i.tabtyp == "07").ToList());
                        this.dgv1.DataSource = l1;
                        break;
                    case TableName.NOTE:
                        BindingList<SupportNote_Min> l2 = new BindingList<SupportNote_Min>(sn.support_note.Select(s => new SupportNote_Min { id = s.id, date = s.date, username = s.users_name }).ToList());
                        this.dgv1.DataSource = l2;
                        break;
                    case TableName.NOTE_COMMENT:
                        BindingList<support_note_comment> l3 = new BindingList<support_note_comment>(sn.support_note_comment.ToList());
                        this.dgv1.DataSource = l3;
                        break;
                    case TableName.NOTE_CALENDAR:
                        BindingList<note_calendar> l4 = new BindingList<note_calendar>(sn.note_calendar.ToList());
                        this.dgv1.DataSource = l4;
                        break;
                    case TableName.EVENT_CALENDAR:
                        BindingList<event_calendar> l5 = new BindingList<event_calendar>(sn.event_calendar.ToList());
                        this.dgv1.DataSource = l5;
                        break;
                    case TableName.TRAINING_CALENDAR:
                        BindingList<training_calendar> l6 = new BindingList<training_calendar>(sn.training_calendar.ToList());
                        this.dgv1.DataSource = l6;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class SupportNote_Min
    {
        public int id { get; set; }
        public DateTime? date { get; set; }
        public string username { get; set; }
        //public support_note support_note { get; set; }
    }
}
