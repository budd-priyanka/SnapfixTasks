using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace SnapfixTaskThree
{
    public partial class snapfixTaskThree : Form
    {
        public snapfixTaskThree()
        {
            InitializeComponent();
        }

        // Declaring constants.
        string URL_PREFIX = "https://live.documents.snapfix.io/sw_analyst/";
        string NAME_TEXT = "Name: ";
        string ADDRESS_TEXT = "Address: ";
        string DEVICES_TEXT = "Devices: ";
        string DEVICE_LIST_TEXT = "No device information available.";
        string MODEL_TEXT = "Model: ";
        string MANUF_TEXT = ", Manufacturer: ";
        string YEAR_TEXT = ", Year: ";
        string TABSPACE = ".  \t";
        string NEWLINE = "\n";
        string NAME_KEY = "name";
        string ADDRESS_KEY = "address";
        string DEVICES_KEY = "devices";
        string MANUF_KEY = "manuf";
        string MODEL_KEY = "model";
        string YEAR_KEY = "year";

        /* 
         * This method makes the request to the server which returns a JSON.
         * The JSON is parsed and the output is displayed in the details panel.
         * An error message is shown in case of request failure.
         */
        private bool MakeRequest(string idText)
        {
            string displayDetails = "";
            try
            {
                string urlPostfix = "user_" + idText + ".json";
                WebRequest request = WebRequest.Create(URL_PREFIX + urlPostfix);
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    string serverResponse = reader.ReadToEnd();
                    JObject jsonResponse = JObject.Parse(serverResponse);

                    //Creating details to be displayed
                    displayDetails += NAME_TEXT + jsonResponse[NAME_KEY] + NEWLINE;
                    displayDetails += ADDRESS_TEXT + jsonResponse[ADDRESS_KEY] + NEWLINE;
                    displayDetails += DEVICES_TEXT + NEWLINE;
                    if (jsonResponse[DEVICES_KEY].Count() > 0)
                    {
                        DEVICE_LIST_TEXT = "";
                        for(int i = 0; i < jsonResponse[DEVICES_KEY].Count(); i++)
                        {
                            DEVICE_LIST_TEXT += (i + 1).ToString()
                                + TABSPACE
                                + MODEL_TEXT
                                + jsonResponse[DEVICES_KEY][i][MODEL_KEY]
                                + MANUF_TEXT
                                + jsonResponse[DEVICES_KEY][i][MANUF_KEY]
                                + YEAR_TEXT
                                + jsonResponse[DEVICES_KEY][i][YEAR_KEY]
                                + NEWLINE;
                        }
                        displayDetails += DEVICE_LIST_TEXT;
                    }
                    else
                    {
                        displayDetails += DEVICE_LIST_TEXT;
                    }
                    dataLabel.Text = displayDetails;
                }
                response.Close();
                return true;
            }
            catch
            {
                MessageBox.Show("Failed to fetch details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Actions performed on button clicks 
        private void clickBehaviour(string idText)
        {
            if (idText.Length > 0)
            {
                if (MakeRequest(idText))
                {
                    detailsPanel.Visible = true;
                    refreshButton.Enabled = true;
                }
                else
                {
                    detailsPanel.Visible = false;
                }
            }
        }

        private void ClickMeButton_Click(object sender, EventArgs e)
        {
            clickBehaviour(idTextBox.Text);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            clickBehaviour(idTextBox.Text);
        }

        // Disable the buttons for blank ID field.
        private void IdTextBox_TextChanged(object sender, EventArgs e)
        {
            bool idHasValue = (idTextBox.Text.Length > 0);
            clickMeButton.Enabled = idHasValue;
            refreshButton.Enabled = idHasValue;
        }
    }
}
