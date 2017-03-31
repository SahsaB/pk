﻿using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace PK.Forms
{
    partial class Examinations : Form
    {
        private readonly Dictionary<char, string> _NameCodes = new Dictionary<char, string>
        {
            {'А',"Q" },{'Б',"W" },{'В',"E" },{'Г',"R" },{'Д',"T" },{'Е',"Y" },{'Ё',"U" },
            {'Ж',"I" },{'З',"O" },{'И',"P" },{'Й',"A" },{'К',"S" },{'Л',"D" },{'М',"F" },
            {'Н',"G" },{'О',"H" },{'П',"J" },{'Р',"K" },{'С',"L" },{'Т',"Z" },{'У',"X" },
            {'Ф',"C" },{'Х',"V" },{'Ц',"B" },{'Ч',"N" },{'Ш',"M" },{'Щ',"GQ" },{'Э',"KI" },
            {'Ю',"AC" },{'Я',"MK" }
        };

        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;

        public Examinations(Classes.DB_Connector connection)
        {
            #region Components
            InitializeComponent();

            dataGridView_Date.ValueType = typeof(DateTime);
            dataGridView_RegStartDate.ValueType = typeof(DateTime);
            dataGridView_RegEndDate.ValueType = typeof(DateTime);
            #endregion

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

            UpdateTable();
        }

        private void dataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool hasMarks = ExaminationHasMarks((uint)dataGridView[0, e.RowIndex].Value);
            toolStrip_Edit.Enabled = !hasMarks;
            toolStrip_Marks.Enabled = hasMarks;
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            _DB_Connection.Delete(DB_Table.EXAMINATIONS, new Dictionary<string, object> { { "id", e.Row.Cells[0].Value } });
        }

        private void toolStrip_Add_Click(object sender, EventArgs e)
        {
            ExaminationEdit form = new ExaminationEdit(_DB_Connection, null);
            form.ShowDialog();
            UpdateTable();
        }

        private void toolStrip_Edit_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 0)
            {
                ExaminationEdit form = new ExaminationEdit(_DB_Connection, (uint)dataGridView.SelectedRows[0].Cells[0].Value);
                form.ShowDialog();
                UpdateTable();
            }
            else
                MessageBox.Show("Выберите экзамен.");
        }

        private void toolStrip_Distribute_Click(object sender, EventArgs e)
        {
            var applications = _DB_Connection.Select(DB_Table.APPLICATIONS)
                .Where(a => (DateTime)a[3] >= (DateTime)dataGridView.SelectedRows[0].Cells[3].Value &&
                (DateTime)a[3] < (DateTime)dataGridView.SelectedRows[0].Cells[4].Value
                );

            var entrants = _DB_Connection.Select(DB_Table.ENTRANTS).Join(
                applications,
                en => en[0],
                a => a[2],
                (s1, s2) => new
                {
                    ID = s1[0],
                    LastName = s1[1].ToString(),
                    FirstName = s1[2].ToString(),
                    MiddleName = s1[3].ToString()
                }
              ).Distinct();

            List<string[]> entrantsTable = new List<string[]>(entrants.Count());
            ushort count = 1;
            foreach (var entr in entrants)
            {
                _DB_Connection.InsertOnDuplicateUpdate(
                    DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                    new Dictionary<string, object> { { "entrant_id", entr.ID }, { "examination_id", dataGridView.SelectedRows[0].Cells[0].Value } }
                    );

                entrantsTable.Add(new string[]
                {
                    count.ToString(),
                    entr.ID.ToString(),
                    entr.LastName+" "+entr.FirstName+" "+entr.MiddleName,
                    _NameCodes[entr.LastName[0]]+_NameCodes[entr.FirstName[0]]+"."+dataGridView.SelectedRows[0].Cells[0].Value+count.ToString()
                });
                count++;
            }

            Classes.DocumentCreator.Create(
                "D:\\Dmitry\\Documents\\GitHub\\pk\\System\\DocumentTemplates\\AlphaCodes.xml",
                "AlphaCodes",
                new string[]
                {
                    dataGridView.SelectedRows[0].Cells[1].Value.ToString(),
                    ((DateTime)dataGridView.SelectedRows[0].Cells[2].Value).ToShortDateString()
                },
                new List<string[]>[] { entrantsTable }
                );

            List<object[]> audiences = _DB_Connection.Select(DB_Table.EXAMINATIONS_AUDIENCES,
                new string[] { "number", "capacity" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("examination_id",Relation.EQUAL,dataGridView.SelectedRows[0].Cells[0].Value)
                });

            List<Tuple<char, string>> distribution = Classes.Utility.DistributeAbiturients(
                audiences.ToDictionary(k => k[0].ToString(), v => (ushort)v[1]),
                entrants.Select(en => en.LastName[0]).GroupBy(en => en).ToDictionary(k => k.Key, v => (ushort)v.Count())
                );

            List<string[]> distibTable = new List<string[]>(audiences.Count);
            foreach (object[] aud in audiences)
            {
                var letters = distribution.Where(d => d.Item2 == aud[0].ToString()).Select(s => s.Item1);
                distibTable.Add(new string[]
                {
                    aud[0].ToString(),
                    letters.Any()?letters.Aggregate("",(a,d)=> a+= d+", ",s=>s.Remove(s.Length- 2)):"-",
                    aud[1].ToString(),
                    entrants.Where(en=>letters.Contains(en.LastName[0])).Count().ToString()
                });
            }

            Classes.DocumentCreator.Create(
               "D:\\Dmitry\\Documents\\GitHub\\pk\\System\\DocumentTemplates\\AbitAudDistrib.xml",
               "AbitAudDistrib",
               new string[]
               {
                   dataGridView.SelectedRows[0].Cells[1].Value.ToString(),
                   ((DateTime)dataGridView.SelectedRows[0].Cells[2].Value).ToShortDateString(),
                   entrants.Count().ToString(),
                   audiences.Sum(a=>(ushort)a[1]).ToString()
               },
               new List<string[]>[] { distibTable }
               );
        }

        private void toolStrip_Marks_Click(object sender, EventArgs e)
        {
            ExaminationMarks form = new ExaminationMarks(_DB_Connection, (uint)dataGridView.SelectedRows[0].Cells[0].Value);
            form.ShowDialog();
        }

        private void UpdateTable()
        {
            Dictionary<uint, string> subjects = _DB_Helper.GetDictionaryItems(1);

            dataGridView.Rows.Clear();
            foreach (object[] row in _DB_Connection.Select(DB_Table.EXAMINATIONS))
                dataGridView.Rows.Add(
                    row[0],
                    subjects[(uint)row[2]],
                    row[3],
                    row[4],
                    row[5]
                    );
        }

        private bool ExaminationHasMarks(uint id)
        {
            return _DB_Connection.Select(
                     DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                     new string[] { "entrant_id" },
                     new List<Tuple<string, Relation, object>>
                     {
                        new Tuple<string, Relation, object> ("examination_id",Relation.EQUAL, id)
                     }).Count != 0;
        }
    }
}