using System.Diagnostics;

namespace cursova;

public partial class Form1 : Form
{
    static string? path;
    static bool stat, mod, work;
    static int i = 1;
    static string keyPress = "";
    public static List<Process> pr = new List<Process>();
    public static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
    public static int alarmCounter = 1;
    public Form1()
    {
        InitializeComponent();
    }

    private void statistics()
    {
        List<string> s = File.ReadLines(path!).ToList();
        List<string> s1 = File.ReadLines(path!).ToList();
        string sEnd = "";
        bool inRewriteBlock = false;
        foreach (string s2 in s)
        {
            if (s2 == "#Start")
            {
                inRewriteBlock = true;
            }
            else if (s2 == "#End")
            {
                inRewriteBlock = false; s1.Remove("#Start"); s1.Remove("#End");
            }
            else if (inRewriteBlock)
            {
                s1.Remove(s2);
            }
        }
        foreach (string s2 in s1)
        {
            sEnd += s2;
            sEnd += "\n";
        }
        File.WriteAllText(path!, sEnd);
        pr.Clear();
        pr = Process.GetProcesses().ToList();
        File.AppendAllText(path!, "#Start");
        File.AppendAllText(path!, $"\nТік: {i}\n\n");
        File.AppendAllText(path!, $"Назва\tЧас запуску\n");
        foreach (Process a in pr)
        {
            try
            {
                File.AppendAllText(path!, $"{a.ProcessName}\t{a.StartTime}\n");
            }
            catch
            {
                File.AppendAllText(path!, $"{a.ProcessName}\t{DateTime.Now}\n");
            }
        }
        File.AppendAllText(path!, "#End");
    }

    private void moderation()
    {
        for (int i = 0; i < listBox2.Items.Count; i++)
        {
            for (int j = 0; j < pr.Count; j++)
            {
                if (pr[j].ProcessName == listBox2.Items[i].ToString() || pr[j].MainWindowTitle == listBox2.Items[i].ToString())
                {
                    File.AppendAllText(path!, $"Було відкрито заборонену програму: \"{listBox2.Items[i].ToString()}\"\n");
                    Process[] h = Process.GetProcessesByName(pr[j].ProcessName);
                    foreach (Process a in h)
                    {
                        a.Kill();
                    }
                }

            }
        }
    }

    private void button1_Click(object sender, EventArgs e)
    {
        OpenFileDialog file = new OpenFileDialog();
        if (file.ShowDialog() == DialogResult.OK)
        {
            if (file.FileName.ToString().Contains(".txt"))
            {
                path = file.FileName;
                textBox1.Text = path;
            }
            else
            {
                MessageBox.Show("Файл не є текстовим", "Помилка");
            }
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        stat = checkBox1.Checked;
        mod = checkBox2.Checked;
        this.KeyPreview = true;
        myTimer.Interval = 1000;
        myTimer.Tick += new EventHandler(TimerEventProcessor!);
        this.KeyPreview = true;
    }

    public void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
    {
        if (stat)
        {
            statistics();
        }
        if (mod)
        {
            moderation();
        }
    }

    private void button2_Click(object sender, EventArgs e)
    {
        listBox1.Items.Add(textBox2.Text);
    }

    private void button4_Click(object sender, EventArgs e)
    {
        listBox2.Items.Add(textBox3.Text);
    }
    private void button3_Click(object sender, EventArgs e)
    {
        listBox1.Items.Remove(listBox1.SelectedItem);
    }

    private void button5_Click(object sender, EventArgs e)
    {
        listBox2.Items.Remove(listBox2.SelectedItem);
    }

    private void button6_Click(object sender, EventArgs e)
    {
        Process.Start("notepad.exe", path!);
    }

    private void button7_Click(object sender, EventArgs e)
    {
        this.Location = new Point(-1000, -1000);
        this.ShowInTaskbar = false;
        this.TopLevel = true;
        this.TopMost = true;
        if (path != null)
        {
            File.WriteAllText(path, string.Empty);
            myTimer.Start();
            if (stat || mod)
            {
                work = true;
            }
            else
            {
                if (stat || mod)
                {
                    work = true;
                }
                else
                {
                    this.Location = new Point(200, 200);
                    this.ShowInTaskbar = true;
                    work = false;
                    this.TopLevel = false;
                    this.TopMost = false;
                    MessageBox.Show("Жоден з режимів роботи не вибрано!", "Помилка");
                }
            }
        }
        else
        {
            this.Location = new Point(200, 200);
            this.ShowInTaskbar = true;
            work = false;
            this.TopLevel = false;
            this.TopMost = false;
            MessageBox.Show("Шлях до файлу не вибрано!", "Помилка");
        }
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
        stat = checkBox1.Checked;
        mod = checkBox2.Checked;
    }

    private void checkBox2_CheckedChanged(object sender, EventArgs e)
    {
        stat = checkBox1.Checked;
        mod = checkBox2.Checked;
    }

    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
        if (work)
        {
            if (stat)
            {
                File.AppendAllText(path!, $"\nБуло натиснуто клавішу \"{e.KeyData.ToString()}\"\n");
            }
            if (mod)
            {
                keyPress += e.KeyCode.ToString();
                for (int h = 0; h < listBox1.Items.Count; h++)
                {
                    string temp = listBox1.Items[h].ToString()!.ToUpper();
                    if (keyPress.Contains(temp))
                    {
                        File.AppendAllText(path!, $"Було набрано заборонене слово: \"{listBox1.Items[h].ToString()}\"");
                        keyPress.Replace(temp, string.Empty);
                    }
                }
            }
        }
    }
    private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (System.Text.Encoding.UTF8.GetByteCount(new char[] { e.KeyChar }) > 1)
        {
            e.Handled = true;
        }
    }
}