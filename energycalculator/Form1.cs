using System;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace energycalculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("http://localhost:5000/") };

        private async void btnSendData_Click(object sender, EventArgs e)
        {
            // ������ ���������, �������� �������������
            var meterData = new
            {
                MeterId = textBox1.Text,
                Day = double.Parse(textBox2.Text),
                Night = double.Parse(textBox3.Text)
            };

            var meterDatabill = new
            {
                MeterId = textBox1.Text,
                CurrentDay = double.Parse(textBox2.Text),
                CurrentNight = double.Parse(textBox3.Text)
            };

            var json = JsonSerializer.Serialize(meterData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var json2 = JsonSerializer.Serialize(meterDatabill);
            var content2 = new StringContent(json2, Encoding.UTF8, "application/json");

            try
            {
                // 1. ���������� ��������� � MeterService
                var meterResponse = await _client.PostAsync("meters", content);
                var meterResult = await meterResponse.Content.ReadAsStringAsync();

                // 2. ����������� ������ ��������� � BillingService
                var billingResponse = await _client.PostAsync("billing", content2);
                var billingResult = await billingResponse.Content.ReadAsStringAsync();

                // 3. ��������� � ��
                var dbResponse = await _client.PostAsync("database", content);
                var dbResult = await dbResponse.Content.ReadAsStringAsync();

                // ������� ���������� ������������
                if (billingResult.Contains("Your current readings are lower than previous. Do you want to correct them?"))
                {
                    DialogResult dialogResult = MessageBox.Show("Sure", "Some Title", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        billingResponse = await _client.PostAsync("correct", content2);
                        billingResult = await billingResponse.Content.ReadAsStringAsync();
                        MessageBox.Show($"Meter Service Response:\n{meterResult}\n\n" +
                                    $"Billing Service Response:\n{billingResult}\n\n" +
                                    $"Database Response:\n{dbResult}",
                                    "���������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        
                    }
                }
                else
                {
                    MessageBox.Show($"Meter Service Response:\n{meterResult}\n\n" +
                                    $"Billing Service Response:\n{billingResult}\n\n" +
                                    $"Database Response:\n{dbResult}",
                                    "���������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // 4. �������� ������� ��������� �� DatabaseService
                var historyResponse = await _client.GetAsync("database");
                var historyResult = await historyResponse.Content.ReadAsStringAsync();

                var readhis = JsonSerializer.Deserialize<List<MeterRecord>>(historyResult);
                if (readhis != null)
                {
                    // ��������� ���� ��� ������
                    using (var writer = new StreamWriter("history.txt"))
                    {
                        writer.WriteLine("Meter History\n");

                        // ��������������� ����� ������
                        foreach (var reading in readhis)
                        {
                            writer.WriteLine($"Meter ID: {reading.meterId}");
                            writer.WriteLine($"Previous Day: {reading.day} kWh");
                            writer.WriteLine($"Previous Night: {reading.night} kWh");
                            writer.WriteLine($"Reading Date: {reading.timestamp:yyyy-MM-dd HH:mm:ss}");
                            writer.WriteLine("-----------------------------------");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No readings found in the response.");
                }
                MessageBox.Show($"������� ���������:\n{historyResult}", "�������", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
    }
    public class MeterRecord
    {
        public string meterId { get; set; }
        public double day { get; set; }
        public double night { get; set; }
        public DateTime timestamp { get; set; } = DateTime.Now;
    }
}