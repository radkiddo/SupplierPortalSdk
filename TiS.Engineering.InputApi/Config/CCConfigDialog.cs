using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TiS.Engineering.InputApi
{
    /// <summary>
    /// Display and or edit the current profile and allow changes to be saved to a new file (or overwrite existing).
    /// </summary>
#if INTERNAL
    internal class CCConfigDialog
#else
    public class CCConfigDialog
#endif
    {
        #region class variables
        protected Form configForm;
        protected Label profileNameLabel;
        protected ComboBox profileNameComboBox;
        protected Panel buttonsPanel;
        protected PropertyGrid settingsPropertyGrid;
        protected Button addButton;
        protected Button removeButton;
        protected Button saveButton;
        protected Button exitButton;
        protected CCConfiguration cnfg;
        protected bool updating; 
        #endregion

        #region "LoadProfileList" method
        /// <summary>
        /// Load profiles to the profiles combo-box.
        /// </summary>
        /// <param name="profileName">The name of the profile to load.</param>
        protected virtual void LoadProfileList(string profileName)
        {
            if (!updating) try
                {
                    updating = true;
                    int idx = -1;
                    if (cnfg != null)
                    {
                        profileNameComboBox.Items.Clear();
                        int cnt = 0;


                        foreach (CCConfiguration.CCConfigurationData cData in cnfg.Configurations)
                        {
                            cnt++;
                            //-- create profile name  if necessry--\\
                            if (string.IsNullOrEmpty(cData.Name))
                            {
                                string newname = string.IsNullOrEmpty(cData.Name) ? cnt.ToString() : cData.Name;
                                List<String> confNames = new List<string>(cnfg.ProfileNames);
                                int lp = 0;
                                while (confNames.Contains(newname))
                                {
                                    lp++;
                                    newname = (string.IsNullOrEmpty(cData.Name) ? cnt.ToString() : cData.Name) + "_" + lp.ToString();
                                }
                                cData.Name = newname;
                            }

                            profileNameComboBox.Items.Add(cData.Name);
                            if (idx < 0 && !String.IsNullOrEmpty(profileName) && !String.IsNullOrEmpty(cData.Name) && String.Compare(profileName, cData.Name, true) == 0)
                            {
                                idx = cnt - 1;
                            }
                        }

                        profileNameComboBox.SelectedIndex = idx;
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                finally
                {
                    updating = false;
                }
        }
        #endregion

        #region "saveButton_Click" event
        /// <summary>
        /// Save XMl profile to file event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void saveButton_Click(object sender, EventArgs e)
        {
            if (cnfg != null) try
                {
                    using (SaveFileDialog sve = new SaveFileDialog())
                    {
                        sve.Title = "Save profile data to XML...";
                        sve.FileName = cnfg != null && File.Exists(cnfg.XmlPath) ? cnfg.XmlPath : string.Empty;
                        sve.Filter = "XML File (*.xml)|*.xml|Any file (*.*)|*.*";
                        sve.DefaultExt = "xml";

                        if (sve.ShowDialog() == DialogResult.OK)
                        {
                            cnfg.ToXml(sve.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
        } 
        #endregion

        #region "addButton_Click" event
        /// <summary>
        /// Add a blank profile to the profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void addButton_Click(object sender, EventArgs e)
        {
            if (cnfg != null && !updating) try
                {
                    updating = true;
                    List<CCConfiguration.CCConfigurationData> cfgLst = new List<CCConfiguration.CCConfigurationData>();
                    if (cnfg.Configurations != null) cfgLst.AddRange(cnfg.Configurations);
                    cfgLst.Add(new CCConfiguration.CCConfigurationData());
                    cfgLst[cfgLst.Count - 1].Name = cfgLst.Count.ToString();
                    profileNameComboBox.Items.Add(cfgLst.Count.ToString());
                    cnfg.Configurations = cfgLst.ToArray();
                    updating = false;

                    profileNameComboBox.SelectedIndex = profileNameComboBox.Items.Count - 1;
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                finally
                {
                    updating = false;
                }
        }
        #endregion

        #region "removeButton_Click" event
        /// <summary>
        /// Remove selected profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void removeButton_Click(object sender, EventArgs e)
        {
            if (cnfg != null && profileNameComboBox.SelectedIndex >= 0) try
                {
                    List<CCConfiguration.CCConfigurationData> cfgLst = new List<CCConfiguration.CCConfigurationData>();
                    if (cnfg.Configurations != null) cfgLst.AddRange(cnfg.Configurations);
                    cfgLst.RemoveAt(profileNameComboBox.SelectedIndex);
                    cnfg.Configurations = cfgLst.ToArray();

                    LoadProfileList(null);
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
        } 
        #endregion

        #region "profileNameComboBox_SeclectedChanged" event
        /// <summary>
        /// Switch comfiguration profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void profileNameComboBox_SeclectedChanged(object sender, EventArgs e)
        {
            if (!updating && cnfg != null && profileNameComboBox.SelectedIndex >= 0) try
                {
                    updating = true;
                    settingsPropertyGrid.SelectedObject = cnfg.GetConfiguration(profileNameComboBox.Text);
                    settingsPropertyGrid.Refresh();
                }
                catch (Exception ex)
                {
                    ILog.LogError(ex);
                }
                finally
                {
                    updating = false;
                }
        } 
        #endregion

        #region "ShowDialog" function
        /// <summary>
        /// Show the profile dialog for the specified profile.
        /// </summary>
        /// <param name="config">the profile to display and edit.</param>
        /// <param name="profileName">The profile name to pre-select.</param>
        /// <param name="lockProfileName">Lock the profile name slection combo-box</param>
        /// <returns>DialogResult</returns>
        public DialogResult ShowDialog(CCConfiguration config, String profileName, bool lockProfileName)
        {
            DialogResult res = DialogResult.None;
            try
            {
                if (config != null)
                {
                    using (configForm = new Form())
                    {
                        configForm.Text = Path.GetFileNameWithoutExtension(Application.ExecutablePath) + " - Editing profile file";
                        cnfg = config;

                        Panel topPanel = new Panel();
                        topPanel.Dock = DockStyle.Top;

                        profileNameLabel = new Label();
                        profileNameLabel.Text = "Profile to edit";
                        profileNameLabel.TextAlign = ContentAlignment.MiddleLeft;

                        profileNameComboBox = new ComboBox();
                        profileNameComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                        LoadProfileList(profileName);

                        profileNameComboBox.Dock = DockStyle.Top;
                        profileNameComboBox.SelectedIndexChanged += profileNameComboBox_SeclectedChanged;
                        topPanel.Controls.Add(profileNameComboBox);
                        topPanel.Controls.Add(profileNameLabel);
                        topPanel.BackColor = Color.Red;
                        topPanel.AutoSize = true;
                        //topPanel.Height = profileNameLabel.Height + profileNameComboBox.Height;

                        buttonsPanel = new Panel();
                        buttonsPanel.BorderStyle = BorderStyle.Fixed3D;

                        settingsPropertyGrid = new PropertyGrid();
                        settingsPropertyGrid.HelpVisible = true;
                        settingsPropertyGrid.ToolbarVisible = true;
                        settingsPropertyGrid.Dock = DockStyle.Fill;

                        try
                        {
                            settingsPropertyGrid.SelectedObject = config.GetConfiguration(profileName);
                        }
                        catch { }

                        saveButton = new Button();
                        saveButton.Text = "&Save...";
                        saveButton.Click += saveButton_Click;
                        saveButton.Anchor = AnchorStyles.Bottom;
                        saveButton.Left = (buttonsPanel.ClientRectangle.Width - ((saveButton.Width + 1) * 4)) / 2;
                        buttonsPanel.Controls.Add(saveButton);

                        addButton = new Button();
                        addButton.Text = "&Add";
                        addButton.Click += addButton_Click;
                        addButton.Left = saveButton.Left + saveButton.Width + 1;
                        addButton.Anchor = AnchorStyles.Bottom;
                        buttonsPanel.Controls.Add(addButton);

                        removeButton = new Button();
                        removeButton.Text = "&Remove";
                        removeButton.Click += removeButton_Click;
                        removeButton.Left = addButton.Left + addButton.Width + 1;
                        removeButton.Anchor = AnchorStyles.Bottom;
                        buttonsPanel.Controls.Add(removeButton);

                        exitButton = new Button();
                        exitButton.Text = "&Exit";
                        exitButton.Left = removeButton.Left + removeButton.Width + 1;
                        exitButton.Anchor = AnchorStyles.Bottom;
                        exitButton.DialogResult = DialogResult.Cancel;
                        buttonsPanel.Controls.Add(exitButton);

                        configForm.Width = Screen.PrimaryScreen.WorkingArea.Width / 4;
                        configForm.Height = Screen.PrimaryScreen.WorkingArea.Height - 100;
                        configForm.StartPosition = FormStartPosition.CenterScreen;
                        configForm.TopMost = true;


                        //-- Add and position created controls --\\
                        configForm.Controls.Add(settingsPropertyGrid);
                        configForm.Controls.Add(topPanel);
                        profileNameLabel.Dock = DockStyle.Top;
                        configForm.Controls.Add(profileNameLabel);
                        buttonsPanel.Dock = DockStyle.Bottom;
                        configForm.Controls.Add(buttonsPanel);

                        configForm.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                        buttonsPanel.Height = (int)Math.Round((double)saveButton.Height * 1.5d);
                        saveButton.Top = (buttonsPanel.Height - saveButton.Height) / 2;
                        addButton.Top = saveButton.Top;
                        removeButton.Top = saveButton.Top;
                        exitButton.Top = saveButton.Top;

                        res = configForm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                ILog.LogError(ex);
            }
            return res;
        } 
        #endregion
    }
}
