﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PK.Forms
{
    public partial class Main : Form
    {
        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Connector _DB_UpdateConnection;
        private readonly Classes.DB_Helper _DB_Helper;
        private readonly string _UserLogin;
        private readonly string _UserRole;

        private readonly Dictionary<string, string> _Statuses = new Dictionary<string, string> { { "new", "Новое" }, { "adm_budget", "Зачислен на бюджет" }, { "adm_paid", "Зачислен на платное" },
            { "adm_both", "Зачислен на бюджет и платное" }, { "withdrawn", "Забрал документы" } };
        private uint _SelectedAppID;

        public Main(string userRole, string usersLogin)
        {
            InitializeComponent();

            _DB_Connection = new Classes.DB_Connector(Properties.Settings.Default.pk_db_CS, userRole,
                new Classes.DB_Connector(Properties.Settings.Default.pk_db_CS, "initial", "1234").Select(
                DB_Table.ROLES_PASSWORDS,
                new string[] { "password" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("role", Relation.EQUAL, userRole) }
                )[0][0].ToString());

            _DB_UpdateConnection = new Classes.DB_Connector(Properties.Settings.Default.pk_db_CS, userRole,
                new Classes.DB_Connector(Properties.Settings.Default.pk_db_CS, "initial", "1234").Select(
                DB_Table.ROLES_PASSWORDS,
                new string[] { "password" },
                new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("role", Relation.EQUAL, userRole) }
                )[0][0].ToString());

            _DB_Helper = new Classes.DB_Helper(_DB_Connection);
            _UserLogin = usersLogin;
            _UserRole = userRole;
            SetUserRole();

            dgvApplications.Sort(dgvApplications_LastName, System.ComponentModel.ListSortDirection.Ascending);
            System.IO.Directory.CreateDirectory(Classes.Utility.TempPath);            
            dtpRegDate.Value = dtpRegDate.MinDate;
            SetCurrentCampaign();
            rbNew.Checked = true;

            lFilter.BackColor = toolStrip.BackColor;
            rbAdm.BackColor = toolStrip.BackColor;
            rbNew.BackColor = toolStrip.BackColor;
            rbWithdraw.BackColor = toolStrip.BackColor;
        }

        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _DB_Connection.Dispose();

                    if (components != null)
                        components.Dispose();
                }

                try
                {
                    System.IO.Directory.Delete(Classes.Utility.TempPath, true);
                }
                catch (System.IO.IOException) { }

                base.Dispose(disposing);
            }
        }

        ~Main()
        {
            Dispose(false);
        }
        #endregion

        private void menuStrip_Campaign_Campaigns_Click(object sender, EventArgs e)
        {
            Campaigns form = new Campaigns(_DB_Connection);
            form.ShowDialog();
            SetCurrentCampaign();
        }

        private void CreateApplication_Click(object sender, EventArgs e)
        {
            if (Classes.Settings.CurrentCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                Form form;
                if (_DB_Helper.IsMasterCampaign(Classes.Settings.CurrentCampaignID))
                    form = new ApplicationMagEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, null);
                else
                    form = new ApplicationEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, null);

                form.ShowDialog();
                UpdateApplicationsTable();
                FilterAppsTable();
            }
        }

        private void menuStrip_TargetOrganizations_Click(object sender, EventArgs e)
        {
            TargetOrganizations form = new TargetOrganizations(_DB_Connection);
            form.ShowDialog();
        }

        private void menuStrip_Dictionaries_Click(object sender, EventArgs e)
        {
            Dictionaries form = new Dictionaries(_DB_Connection);
            form.Show();
        }

        private void menuStrip_DirDictionary_Click(object sender, EventArgs e)
        {
            DirectionsDictionary form = new DirectionsDictionary(_DB_Connection);
            form.Show();
        }

        private void menuStrip_OlympDictionary_Click(object sender, EventArgs e)
        {
            OlympicsDictionary form = new OlympicsDictionary(_DB_Connection);
            form.Show();
        }

        private void menuStrip_Faculties_Click(object sender, EventArgs e)
        {
            Faculties form = new Faculties(_DB_Connection);
            form.ShowDialog();
        }

        private void menuStrip_Directions_Click(object sender, EventArgs e)
        {
            DirectionsProfiles form = new DirectionsProfiles(_DB_Connection);
            form.ShowDialog();
        }

        private void menuStrip_Campaign_Exams_Click(object sender, EventArgs e)
        {
            if (_DB_Helper.IsMasterCampaign(Classes.Settings.CurrentCampaignID))
            {
                MasterExaminations form = new MasterExaminations(_DB_Connection);
                form.ShowDialog();
            }
            else
            {
                Examinations form = new Examinations(_DB_Connection);
                form.ShowDialog();
            }
        }

        private void menuStrip_InstitutionAchievements_Click(object sender, EventArgs e)
        {
            if (Classes.Settings.CurrentCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                InstitutionAchievementsEdit form = new InstitutionAchievementsEdit(_DB_Connection);
                form.ShowDialog();
            }
        }

        private void menuStrip_Campaign_Orders_Click(object sender, EventArgs e)
        {
            Orders form = new Orders(_DB_Connection);
            form.ShowDialog();
            UpdateApplicationsTable();
            FilterAppsTable();
        }

        private void menuStrip_Campaign_Statistics_Click(object sender, EventArgs e)
        {
            Statistics form = new Statistics(_DB_Connection);
            form.Show();
        }

        private void menuStrip_Constants_Click(object sender, EventArgs e)
        {
            if (Classes.Settings.CurrentCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                Constants form = new Constants(_DB_Connection, Classes.Settings.CurrentCampaignID);
                form.ShowDialog();
            }
        }

        private void menuStrip_KLADR_Update_Click(object sender, EventArgs e)
        {
            KLADR_Update form = new KLADR_Update(_DB_Connection.User, _DB_Connection.Password);
            form.ShowDialog();
        }


        private void toolStrip_Users_Click(object sender, EventArgs e)
        {
            Users form = new Users(_DB_Connection);
            form.ShowDialog();
        }

        private void toolStrip_FIS_Export_Click(object sender, EventArgs e)
        {
            FIS_Export form = new FIS_Export(_DB_Connection);
            form.ShowDialog();
        }

        private void toolStrip_RegJournal_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Classes.OutDocuments.RegistrationJournal(_DB_Connection));
        }
        
        private void menuStrip_DirsPlaces_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Classes.OutDocuments.DirectionsPlaces(_DB_Connection));
        }

        private void menuStrip_ProfilesPlaces_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Classes.OutDocuments.ProfilesPlaces(_DB_Connection));
        }

        private void menuStrip_CheckEgeMarks_Click(object sender, EventArgs e)
        {
            EGE_Check form = new EGE_Check(_DB_Connection);
            form.ShowDialog();
        }

        private void dgvApplications_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            _SelectedAppID = (uint)dgvApplications.CurrentRow.Cells[dgvApplications_ID.Index].Value;

            if (Classes.Settings.CurrentCampaignID == 0)
                MessageBox.Show("Не выбрана текущая кампания. Перейдите в Главное меню -> Приемная кампания -> Приемные кампании.");
            else
            {
                if (!(bool)_DB_Connection.Select(DB_Table.APPLICATIONS, new string[] { "master_appl" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)dgvApplications.CurrentRow.Cells[0].Value)
                })[0][0])
                {
                    ApplicationEdit form = new ApplicationEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, (uint)dgvApplications.CurrentRow.Cells[0].Value);
                    form.ShowDialog();
                }
                else
                {
                    ApplicationMagEdit form = new ApplicationMagEdit(_DB_Connection, Classes.Settings.CurrentCampaignID, _UserLogin, (uint)dgvApplications.CurrentRow.Cells[0].Value);
                    form.ShowDialog();
                }
                UpdateApplicationsTable();
                FilterAppsTable();
            }
        }

        private void dgvApplications_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvApplications.SelectedRows.Count > 0)
                _SelectedAppID = (uint)dgvApplications.SelectedRows[0].Cells[dgvApplications_ID.Index].Value;
        }


        private void tbField_Leave(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                (sender as TextBox).Text = (sender as TextBox).Tag.ToString();
                (sender as TextBox).ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void tbField_Enter(object sender, EventArgs e)
        {
            if ((sender as TextBox).ForeColor == System.Drawing.Color.Gray)
            {
                (sender as TextBox).Text = "";
                (sender as TextBox).ForeColor = System.Drawing.Color.Black;
            }
        }

        private void tbField_TextChanged(object sender, EventArgs e)
        {            
                FilterAppsTable();
        }

        private void cbDateSearch_CheckedChanged(object sender, EventArgs e)
        {
            dtpRegDate.Enabled = cbDateSearch.Checked;
            if (!dtpRegDate.Enabled)
                dtpRegDate.Value = dtpRegDate.MinDate;
            else
                dtpRegDate.Value = DateTime.Now;
        }

        private void rbFilter_CheckedChanged(object sender, EventArgs e)
        {
            FilterAppsTable();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            backgroundWorker.RunWorkerAsync();
            timer.Start();
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            e.Result = GetAppsTableRows();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            CompleteUpdateAppsTable(e.Result as List<DataGridViewRow>);
            FilterAppsTable();
        }


        private List<DataGridViewRow> GetAppsTableRows()
        {
            List<DataGridViewRow> appRows = new List<DataGridViewRow>();
            List<object[]> apps = new List<object[]>();
            if (_UserRole == "registrator")
            apps = _DB_UpdateConnection.Select(DB_Table.APPLICATIONS, new string[] { "id", "entrant_id", "registration_time", "registrator_login", "edit_time", "status", "withdraw_date" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Settings.CurrentCampaignID),
                    new Tuple<string, Relation, object>("registrator_login", Relation.EQUAL, _UserLogin),
                    new Tuple<string, Relation, object>("registration_time", Relation.GREATER_EQUAL, DateTime.Now.Date)
                });
            else if (_UserRole == "inspector")
                apps = _DB_UpdateConnection.Select(DB_Table.APPLICATIONS, new string[] { "id", "entrant_id", "registration_time", "registrator_login", "edit_time", "status", "withdraw_date" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Settings.CurrentCampaignID),
                    new Tuple<string, Relation, object>("registration_time", Relation.GREATER_EQUAL, DateTime.Now.Date)
                });
            else
                apps = _DB_UpdateConnection.Select(DB_Table.APPLICATIONS, new string[] { "id", "entrant_id", "registration_time", "registrator_login", "edit_time", "status", "withdraw_date" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id", Relation.EQUAL, Classes.Settings.CurrentCampaignID)
                });
            var directions = _DB_UpdateConnection.Select(DB_Table.DIRECTIONS, new string[] { "direction_id", "faculty_short_name", "short_name" });
            var profiles = _DB_UpdateConnection.Select(DB_Table.PROFILES, new string[] { "direction_id", "faculty_short_name", "short_name" });

                foreach (var application in apps)
                {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvApplications);
                object[] names = _DB_UpdateConnection.Select(DB_Table.ENTRANTS_VIEW, new string[] { "last_name", "first_name", "middle_name" },
                        new List<Tuple<string, Relation, object>>
                        {
                            new Tuple<string, Relation, object>("id", Relation.EQUAL, (uint)application[1])
                        })[0];
                    var appDocuments = _DB_UpdateConnection.Select(DB_Table._APPLICATIONS_HAS_DOCUMENTS, new string[] { "documents_id" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, (uint)application[0])
                    });
                {
                    row.SetValues(application[0], names[0], names[1], names[2], null, null, null, application[2] as DateTime?, application[4] as DateTime?, application[6], null, null,
                                application[3], _Statuses[application[5].ToString()]);
                }

                    foreach (var document in _DB_UpdateConnection.Select(DB_Table.DOCUMENTS, new string[] { "id", "type", "original_recieved_date" }).Join(
                        appDocuments,
                        docs => docs[0],
                        appdocs => appdocs[0],
                        (s1, s2) => new
                        {
                            Type = s1[1].ToString(),
                            OriginalRecievedDate = s1[2] as DateTime?
                        }
                        ).Select(s => new { Value = new Tuple<string, DateTime?>(s.Type, s.OriginalRecievedDate) }).ToList())
                        if ((document.Value.Item1 == "school_certificate" || document.Value.Item1 == "high_edu_diploma" || document.Value.Item1 == "academic_diploma"
                        || document.Value.Item1 == "middle_edu_diploma") && (document.Value.Item2 != null))
                            row.Cells[dgvApplications_Original.Index].Value = true;


                    var appDirections = _DB_UpdateConnection.Select(DB_Table.APPLICATIONS_ENTRANCES, new string[] { "direction_id", "faculty_short_name", "profile_short_name" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("application_id", Relation.EQUAL, application[0])
                    });
                    if (!dgvApplications_Programs.Visible)
                    {
                        string[] entrance = appDirections.Join(
                            directions,
                            entrances => new Tuple<uint, string>((uint)entrances[0], entrances[1].ToString()),
                            dirs => new Tuple<uint, string>((uint)dirs[0], dirs[1].ToString()),
                            (s1, s2) => (s1[2]!=null && s1[2].ToString() != "")? s1[2].ToString() + "*": s2[2].ToString()).ToArray();
                        foreach (object shortName in entrance)
                            if (row.Cells[dgvApplications_Entrances.Index].Value == null)
                            row.Cells[dgvApplications_Entrances.Index].Value = shortName;
                            else
                            row.Cells[dgvApplications_Entrances.Index].Value = row.Cells[dgvApplications_Entrances.Index].Value.ToString() + ", " + shortName;
                    }
                    else
                    {
                        foreach (object[] appDir in appDirections)
                            if (row.Cells[dgvApplications_Programs.Index].Value == null)
                            row.Cells[dgvApplications_Programs.Index].Value = appDir[2];
                            else
                            row.Cells[dgvApplications_Programs.Index].Value = row.Cells[dgvApplications_Programs.Index].Value + ", " + appDir[2].ToString();
                    }
                    var appOrdersData = _DB_UpdateConnection.Select(DB_Table.ORDERS_HAS_APPLICATIONS, new string[] { "orders_number" }, new List<Tuple<string, Relation, object>>
                    {
                        new Tuple<string, Relation, object>("applications_id", Relation.EQUAL, application[0])
                    });
                    var appOrders = _DB_UpdateConnection.Select(DB_Table.ORDERS, new string[] { "number", "type", "date" }).Join(
                        appOrdersData,
                        orders => orders[0],
                        data => data[0],
                        (s1, s2) => new { Number = s1[0].ToString(), Type = s1[1].ToString(), Date = (DateTime)s1[2] }
                        );
                    string admNumber = null;
                    string exNumber = null;
                    DateTime admDate = DateTime.MinValue;
                    DateTime exDate = DateTime.MinValue;
                    foreach (var order in appOrders)
                        if (order.Type == "admission" && order.Date > admDate.Date)
                        {
                            admNumber = order.Number;
                            admDate = order.Date;
                        }
                        else if (order.Type == "exception" && order.Date > exDate.Date)
                        {
                            exNumber = order.Number;
                            exDate = order.Date;
                        }

                    if (admNumber != null)
                    row.Cells[dgvApplications_EnrollmentDate.Index].Value = admDate.ToShortDateString() + " " + admNumber;
                    if (exNumber != null)
                    row.Cells[dgvApplications_DeductionDate.Index].Value = exDate.ToShortDateString() + " " + exNumber;

                appRows.Add(row);
                }
            return appRows;
        }

        private void UpdateApplicationsTable()
        {
            CompleteUpdateAppsTable(GetAppsTableRows());
        }

        private void CompleteUpdateAppsTable(List<DataGridViewRow> rows)
        {
            DataGridViewColumn sortColumn = dgvApplications.SortedColumn;
            int displayedRow = dgvApplications.FirstDisplayedScrollingRowIndex;
            dgvApplications.Rows.Clear();
            dgvApplications.Rows.AddRange(rows.ToArray());
            foreach (DataGridViewRow row in dgvApplications.Rows)
                if ((uint)row.Cells[dgvApplications_ID.Index].Value == _SelectedAppID)
                    row.Selected = true;
            if (sortColumn != null)
                dgvApplications.Sort(sortColumn, System.ComponentModel.ListSortDirection.Ascending);
            if (displayedRow >= 0 && dgvApplications.Rows.Count >= displayedRow)
                dgvApplications.FirstDisplayedScrollingRowIndex = displayedRow;
        }

        private void FilterAppsTable()
        {
            ChangeColumnsVisible();
            int visibleCount = 0;
            foreach (DataGridViewRow row in dgvApplications.Rows)
            {
                if (rbNew.Checked && (row.Cells[dgvApplications_Status.Index].Value.ToString() != _Statuses["new"]))
                        row.Visible = false;
                    else if (rbWithdraw.Checked && (row.Cells[dgvApplications_Status.Index].Value.ToString() != _Statuses["withdrawn"]))
                        row.Visible = false;
                    else if (rbAdm.Checked && (row.Cells[dgvApplications_Status.Index].Value.ToString() != _Statuses["adm_budget"]
                        && row.Cells[dgvApplications_Status.Index].Value.ToString() != _Statuses["adm_paid"]
                        && row.Cells[dgvApplications_Status.Index].Value.ToString() != _Statuses["adm_both"]))
                        row.Visible = false;
                    else row.Visible = true;
                if (row.Visible)
                {
                    bool matches = true;
                    if ((tbRegNumber.Text != "") && (tbRegNumber.Text != tbRegNumber.Tag.ToString()) && !row.Cells[dgvApplications_ID.Index].Value.ToString().ToLower().StartsWith(tbRegNumber.Text.ToLower()))
                        matches = false;
                    else if ((tbLastName.Text != "") && (tbLastName.Text != tbLastName.Tag.ToString()) && !row.Cells[dgvApplications_LastName.Index].Value.ToString().ToLower().StartsWith(tbLastName.Text.ToLower()))
                        matches = false;
                    else if ((tbFirstName.Text != "") && (tbFirstName.Text != tbFirstName.Tag.ToString()) && !row.Cells[dgvApplications_FirstName.Index].Value.ToString().ToLower().StartsWith(tbFirstName.Text.ToLower()))
                        matches = false;
                    else if ((tbMiddleName.Text != "") && (tbMiddleName.Text != tbMiddleName.Tag.ToString()) && !row.Cells[dgvApplications_MiddleName.Index].Value.ToString().ToLower().StartsWith(tbMiddleName.Text.ToLower()))
                        matches = false;
                    else if ((dtpRegDate.Value != dtpRegDate.MinDate) && ((row.Cells[dgvApplications_RegDate.Index].Value as DateTime?).Value.Date != dtpRegDate.Value.Date))
                        matches = false;
                    row.Visible = matches;
                    if (matches)
                        visibleCount++;
                }
            }
            lbDispalyedCount.Text = lbDispalyedCount.Tag.ToString() + visibleCount;
        }

        private void SetCurrentCampaign()
        {
            if (Classes.Settings.CurrentCampaignID != 0)
            {
                toolStrip_CurrCampaign.Text = _DB_Connection.Select(DB_Table.CAMPAIGNS, new string[] { "name" }, new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL, Classes.Settings.CurrentCampaignID)
                })[0][0].ToString();

                bool master = _DB_Helper.IsMasterCampaign(Classes.Settings.CurrentCampaignID);
                dgvApplications_Original.Visible = !master;
                dgvApplications_Entrances.Visible = !master;
                dgvApplications_Programs.Visible = master;

                UpdateApplicationsTable();
                FilterAppsTable();
                if (_UserRole != "registrator")
                    timer.Start();
            }
        }

        private void SetUserRole()
        {
            List<string> roles = new List<string>();
            if (_UserRole == "registrator")
                menuStrip.Enabled = false;

            if (_UserRole == "registrator")
                roles.Add("registrator");
            else if (_UserRole == "inspector")
                roles.AddRange(new string[] { "registrator", "inspector" });
            else if (_UserRole == "administrator")
                roles.AddRange(new string[] { "registrator", "inspector", "administrator" });

            foreach (ToolStripMenuItem menuStrip in MainMenuStrip.Items)
            {
                foreach (ToolStripItem submenuItem in menuStrip.DropDownItems)
                    if (submenuItem.Tag != null && !roles.Contains(submenuItem.Tag.ToString()))
                        submenuItem.Enabled = false;

                bool enabled = false;
                foreach (ToolStripItem submenuItem in menuStrip.DropDownItems)
                    if (submenuItem.Enabled)
                            enabled = true;

                if (!enabled)
                    menuStrip.Enabled = false;
            }
        }

        private void ChangeColumnsVisible()
        {
            dgvApplications_PickUpDate.Visible = rbWithdraw.Checked;
            dgvApplications_EnrollmentDate.Visible = rbNew.Checked || rbAdm.Checked;
            dgvApplications_DeductionDate.Visible = rbNew.Checked;
            dgvApplications_Status.Visible = rbAdm.Checked;
        }
    }
}
