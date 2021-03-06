﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using SharedClasses.DB;

namespace PK.Forms
{
    partial class ExaminationDocsPrint : Form
    {
        class Entrant
        {
            public readonly string ApplIDs;
            public readonly string Name;
            public readonly string Code;
            public readonly string Auditory;
            public readonly ushort AudSeat;

            public Entrant(string applIDs, string name, string code, string auditory, ushort audSeat)
            {
                ApplIDs = applIDs;
                Name = name;
                Code = code;
                Auditory = auditory;
                AudSeat = audSeat;
            }
        }

        private readonly DB_Connector _DB_Connection;
        private readonly uint _ExaminationID;
        private readonly string _ExamName;
        private readonly string _ExamDate;
        private readonly List<Entrant> _EntrantsTable;
        private readonly Dictionary<string, ushort> _Audiences;
        private readonly List<Tuple<char, string>> _Distribution; //TODO Нужно? Можно использовать только _EntrantsTable?

        public ExaminationDocsPrint(DB_Connector connection, uint examinationID)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _ExaminationID = examinationID;

            object[] buf = _DB_Connection.Select(
                DB_Table.EXAMINATIONS,
                new string[] { "subject_id", "date" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id",Relation.EQUAL,_ExaminationID)
                })[0];

            _ExamName = new DB_Helper(_DB_Connection).GetDictionaryItemName(FIS_Dictionary.SUBJECTS, (uint)buf[0]);
            _ExamDate = ((DateTime)buf[1]).ToShortDateString();

            var entrantsIDs = _DB_Connection.Select(
                DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                new string[] { "entrant_id" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("examination_id",Relation.EQUAL,_ExaminationID)
                });

            var entrants = _DB_Connection.Select(
                DB_Table.ENTRANTS_VIEW,
                new string[] { "id", "last_name", "first_name", "middle_name" }
                ).Join(
                entrantsIDs,
                en => en[0],
                i => i[0],
                (s1, s2) => new { ID = (uint)s1[0], LastName = s1[1].ToString(), FirstName = s1[2].ToString(), MiddleName = s1[3].ToString() }
                ).Join(
                _DB_Connection.Select(
                    DB_Table.APPLICATIONS,
                    new string[] { "id", "entrant_id" },
                    new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Settings.CurrentCampaignID),
                        new Tuple<string, Relation, object>("status",Relation.NOT_EQUAL,"withdrawn")
                    }),
                k1 => k1.ID,
                k2 => k2[1],
                (s1, s2) => new
                {
                    s1.ID,
                    s1.LastName,
                    s1.FirstName,
                    s1.MiddleName,
                    ApplID = (uint)s2[0]
                }).GroupBy(
                k => k.ID,
                (k, g) => new
                {
                    ID = k,
                    g.First().LastName,
                    g.First().FirstName,
                    g.First().MiddleName,
                    ApplIDs = string.Join(", ", g.Select(s => s.ApplID.ToString()))
                });

            _Audiences = _DB_Connection.Select(DB_Table.EXAMINATIONS_AUDIENCES,
                new string[] { "number", "capacity", "priority" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("examination_id",Relation.EQUAL,_ExaminationID)
                }).OrderBy(s => s[2]).ToDictionary(k => k[0].ToString(), v => (ushort)v[1]);

            _Distribution = SharedClasses.Utility.DistributeAbiturients(
                 _Audiences,
                 entrants.Select(en => char.ToUpper(en.LastName[0])).GroupBy(en => en).ToDictionary(k => k.Key, v => (ushort)v.Count())
                 );

            Dictionary<char, string> nameCodes = new Dictionary<char, string>
            {
                { 'А',"Q" },{'Б',"W" },{'В',"E" },{'Г',"R" },{'Д',"T" },{'Е',"Y" },{'Ё',"U" },
                { 'Ж',"I" },{'З',"O" },{'И',"P" },{'Й',"A" },{'К',"S" },{'Л',"D" },{'М',"F" },
                { 'Н',"G" },{'О',"H" },{'П',"J" },{'Р',"K" },{'С',"L" },{'Т',"Z" },{'У',"X" },
                { 'Ф',"C" },{'Х',"V" },{'Ц',"B" },{'Ч',"N" },{'Ш',"M" },{'Щ',"GQ" },{'Э',"KI" },
                { 'Ю',"AC" },{'Я',"MK" }
            };

            Dictionary<string, Tuple<ushort, ushort>> fill = _Audiences.ToDictionary(k => k.Key, v => new Tuple<ushort, ushort>(0, v.Value));
            _EntrantsTable = new List<Entrant>(entrants.Count());
            ushort count = 1;
            foreach (var entr in entrants)
            {
                foreach (Tuple<char, string> aud in _Distribution.FindAll(c => c.Item1 == char.ToUpper(entr.LastName[0])))
                    if (fill[aud.Item2].Item1 < fill[aud.Item2].Item2)
                    {
                        fill[aud.Item2] = Tuple.Create((ushort)(fill[aud.Item2].Item1 + 1), fill[aud.Item2].Item2);

                        _EntrantsTable.Add(new Entrant(
                            entr.ApplIDs,
                            entr.LastName + " " + entr.FirstName + " " + entr.MiddleName,
                            nameCodes[char.ToUpper(entr.LastName[0])] + nameCodes[char.ToUpper(entr.FirstName[0])] + "." + _ExaminationID.ToString() + count.ToString(),
                            aud.Item2,
                            fill[aud.Item2].Item1
                            ));

                        break;
                    }

                count++;
            }
        }

        private void bAlphaCodes_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string doc = Classes.Settings.TempPath + "alphaCodes" + new Random().Next();
            Classes.DocumentCreator.Create(
                Classes.Settings.DocumentsTemplatesPath + "AlphaCodes.xml",
                doc,
                new string[] { _ExamName, _ExamDate },
                new IEnumerable<string[]>[] { _EntrantsTable.Select(s => new string[] { s.ApplIDs, s.Name, s.Code }).OrderBy(s => s[1]) }
                );
            System.Diagnostics.Process.Start(doc + ".docx");

            Cursor.Current = Cursors.Default;
        }

        private void bAlphaAuditories_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string doc = Classes.Settings.TempPath + "alphaAuditories" + new Random().Next();
            Classes.DocumentCreator.Create(
                Classes.Settings.DocumentsTemplatesPath + "AlphaAuditories.xml",
                doc,
                new string[] { _ExamName, _ExamDate },
                new IEnumerable<string[]>[] { _EntrantsTable.Select(s => new string[] { s.ApplIDs, s.Name, s.Auditory }).OrderBy(s => s[1]) }
                );
            System.Diagnostics.Process.Start(doc + ".docx");

            Cursor.Current = Cursors.Default;
        }

        private void bAbitAudDistib_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            List<string[]> distribTable = new List<string[]>(_Audiences.Count());
            foreach (var aud in _Audiences)
            {
                var letters = _Distribution.Where(d => d.Item2 == aud.Key).Select(s => s.Item1);
                distribTable.Add(new string[]
                {
                    aud.Key,
                    letters.Any()?letters.Aggregate("",(a,d)=> a+= d+", ",s=>s.Remove(s.Length- 2)):"-",
                    aud.Value.ToString(),
                    _EntrantsTable.Where(en=>letters.Contains(char.ToUpper(en.Name[0]))).Count().ToString()
                });
            }

            string doc = Classes.Settings.TempPath + "abitAudDistrib" + new Random().Next();
            Classes.DocumentCreator.Create(
               Classes.Settings.DocumentsTemplatesPath + "AbitAudDistrib.xml",
               doc,
               new string[]
               {
                   _ExamName,
                   _ExamDate,
                   _EntrantsTable.Count().ToString(),
                   _Audiences.Sum(a=>a.Value).ToString()
               },
               new List<string[]>[] { distribTable }
               );
            System.Diagnostics.Process.Start(doc + ".docx");

            Cursor.Current = Cursors.Default;
        }

        private void bExamCardsSheet_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string doc = Classes.Settings.TempPath + "examCardsSheet" + new Random().Next();
            Classes.DocumentCreator.Create(
                doc,
                _Distribution.Select(s => new Classes.DocumentCreator.DocumentParameters(
                    Classes.Settings.DocumentsTemplatesPath + "ExamCardsSheet.xml",
                    null,
                    null,
                    new string[] { s.Item2, s.Item1.ToString(), _ExamName, _ExamDate },
                    new IEnumerable<string[]>[]
                    {
                        _EntrantsTable.Where(en=>char.ToUpper(en.Name[0])== s.Item1&&en.Auditory==s.Item2).Select(a=>
                        {
                            string[] parts=a.Name.Split(' ');
                            parts[0]+=" ";
                            parts[1]+="\n";
                            return  new string[] {a.ApplIDs, string.Concat(parts), a.AudSeat.ToString() };
                        }).OrderBy(a=>a[1])
                    })),
                false
                );
            System.Diagnostics.Process.Start(doc + ".docx");

            Cursor.Current = Cursors.Default;
        }

        private void bAudLists_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string doc = Classes.Settings.TempPath + "audLists" + new Random().Next();
            Classes.DocumentCreator.Create(
                doc,
                _EntrantsTable.GroupBy(
                    k => k.Auditory,
                    (k, g) => new Classes.DocumentCreator.DocumentParameters(
                Classes.Settings.DocumentsTemplatesPath + "AudLists.xml",
                null,
                null,
                new string[] { k, _ExamName, _ExamDate },
                new IEnumerable<string[]>[] { g.Select(s => new string[] { s.ApplIDs, s.Name }).OrderBy(s => s[1]) }
                )));
            System.Diagnostics.Process.Start(doc + ".docx");

            Cursor.Current = Cursors.Default;
        }

        private void bExamFill_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string doc = Classes.Settings.TempPath + "examFill" + new Random().Next();
            Classes.DocumentCreator.Create(
                Classes.Settings.DocumentsTemplatesPath + "ExamFill.xml",
                doc,
                new string[] { _ExamName, _ExamDate },
                new IEnumerable<string[]>[] { _EntrantsTable.GroupBy(k => char.ToUpper(k.Name[0]), (k, g) => new string[] { k.ToString(), g.Count().ToString() }).OrderBy(s => s[0]) }
                );
            System.Diagnostics.Process.Start(doc + ".docx");

            Cursor.Current = Cursors.Default;
        }
    }
}
