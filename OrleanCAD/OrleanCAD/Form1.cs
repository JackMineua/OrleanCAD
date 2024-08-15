using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace OrleanCAD
{
    public partial class Form1 : Form
    {
        private WebSocket ws;

        List<CallData> CallData = new List<CallData>();
        List<Character> Character = new List<Character>();
        List<Weapon> Weapon = new List<Weapon>();
        List<Vehicle> Vehicle = new List<Vehicle>();


        public Color Yellow = Color.FromArgb(245, 255, 147);
        public int dataWindowOpen = 0;

        public string c_id;
        public string c_nickname = "NeedToUpdate";
        public string c_sessionid = "NeedToUpdate";
        public string c_deps;
        public string c_session_dep;
        public string c_status;
        public string c_rank;

        public int isPanicActive = 0;
        public int is100Active = 0;
        public int isPanicActiveOther = 0;
        public int is100ActiveOther = 0;

        public int activeCallWindow = 0;

        public int isCivWindowOpen = 0;

        public int isDispActive = 0;

        public int charactersCount = 0;
        public int weaponsCount = 0;
        public int vehiclesCount = 0;

        public string activeUnits;

        public int CallsNumber = 0;

        public int IsSelectDepOpen = 0;

        SoundPlayer Signal100Sound = new SoundPlayer(@"./100_by_Sheriff_Department.wav");
        SoundPlayer PanicSound = new SoundPlayer(@"./by_Sheriff_Department.wav");
        SoundPlayer SystemClick = new SoundPlayer(@"./beep-08b.wav");

        public int IsPanicPlaying = 0;
        public int Is100Playing = 0;


        Panel[] characterPanel = new Panel[4];
        Panel[] weaponPanel = new Panel[4];
        Panel[] vehiclePanel = new Panel[4];

        Label[] characterName = new Label[4];
        Label[] characterBirth = new Label[4];
        Label[] characterWork = new Label[4];

        Label[] weaponcharacterName = new Label[4];
        Label[] weaponcharacterBirth = new Label[4];
        Label[] weaponData = new Label[4];

        Label[] vehiclecharacterName = new Label[4];
        Label[] vehiclecharacterBirth = new Label[4];
        Label[] vehiclePlate = new Label[4];
        Label[] vehicleNameAndColor = new Label[4];

        private protected const string version = "VJH12093WJDFGBN2983Y4RJGBR";

        public Form1()
        {
            InitializeComponent();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            ws = new WebSocket($"ws://85.159.230.224:8008/{LoginInput.Text}");
            //ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls;
            ws.OnMessage += Ws_OnMessage; // Подписываемся на событие OnMessage
            ws.Connect();
            if (!ws.IsAlive) { label5.Text = "DEBUG: Something wrong! Maybe need an update?"; return; }
            ws.Send("LOGIN:" + LoginInput.Text + ":" + PasswordInput.Text + ":" + version);
        }

        public void InitApp()
        {
            var currentDirectory = Environment.CurrentDirectory;

            // Создадим экземпляр процесса, который в cmd выполнит команду удаления этой папки
            System.Diagnostics.Process process = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System), // Зададим рабочий каталог, потому что если этого не сделать, сама папка с нашим .exe останется заблокированной и мы не сможем ее удалить.
                    Arguments = $"/C timeout 2 & rmdir \"{currentDirectory}\" /s /q"
                    // /C - говорит cmd.exe что мы передаем в нее стркоу с командой, а не просто запускаем (как у вас передается -uninstall)
                    // timeout 1 - ждет 1 секунд перед выполнением следующей операции, за это время наше приложение должно завершиться и не блокировать .exe (возможно может понадобиться больше времени)
                    // & - символ для объединения операций, чтоб передать их в cmd одной строкой
                    // rmdir - удаляет указаный каталог (/s - удалит все файлы и все доечерние каталоги, /q - сделает это тихо и не задаст лишних вопросов)
                }
            };
            process.Start();
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.StartsWith("WSNS_RESULT"))
                {
                    string[] parts = e.Data.Split(':');
                    string state = parts[1];
                    if (state == "true")
                    {
                        string serial = parts[2];
                        string owner = parts[3];
                        string model = parts[4];
                        string stateofcreate = parts[6];
                        string dateofcreate = parts[5];
                        DataWindow(2, serial, owner, model, stateofcreate, dateofcreate, null, null, null, null, null, null);
                    }
                    else return;
                }
                if (e.Data.StartsWith("PLATES_RESULT"))
                {
                    string[] parts = e.Data.Split(':');
                    string state = parts[1];
                    if (state == "true")
                    {
                        string plates;
                        string owner;
                        string color;
                        string model;
                        string stateofcreate;
                        string dateofcreate;
                        int isinbolo;
                        string bolodesc;
                        string bolocreatedate;
                        plates = parts[2];
                        owner = parts[3];
                        color = parts[4];
                        model = parts[5];
                        stateofcreate = parts[6];
                        dateofcreate = parts[7];
                        isinbolo = Convert.ToInt32(parts[8]);
                        if (isinbolo == 1)
                        {
                            bolodesc = parts[9];
                            bolocreatedate = parts[10];
                            DataWindow(1, plates, owner, color, model, stateofcreate, dateofcreate, "1", bolodesc, bolocreatedate, null, null);
                        }
                        else
                        {
                            DataWindow(1, plates, owner, color, model, stateofcreate, dateofcreate, "0", null, null, null, null);
                        }
                    }
                    else return;
                }
                if (e.Data.StartsWith("NCIC_RESULT"))
                {
                    string[] parts = e.Data.Split(':');
                    string state = parts[1];
                    if (state == "true")
                    {
                        string name;
                        string dateofbirth;
                        string medicine;
                        string veh_lic_tipe;
                        string wep_lic;
                        string work;
                        string orders;
                        string sex;
                        string order_type;
                        string order_desk;
                        string order_datewrite;

                        name = parts[2];
                        dateofbirth = parts[3] + "." + parts[4] + "." + parts[5];
                        medicine = parts[6];
                        veh_lic_tipe = parts[7];
                        wep_lic = parts[8];
                        work = parts[9];
                        orders = parts[10];
                        sex = parts[11];
                        if (orders == "1")
                        {
                            order_type = parts[12];
                            order_desk = parts[13];
                            order_datewrite = parts[14];
                            DataWindow(0, name, dateofbirth, sex, medicine, veh_lic_tipe, wep_lic, work, "1", order_type, order_desk, order_datewrite);
                        }
                        else
                        {
                            DataWindow(0, name, dateofbirth, sex, medicine, veh_lic_tipe, wep_lic, work, "0", null, null, null);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                if (e.Data.StartsWith("ACTIVE_DISPATCH"))
                {
                    string[] parts = e.Data.Split(':');
                    string status = parts[1];
                    string dispID = parts[2];
                    if (status == "true")
                    {
                        currentdispatch.Invoke((MethodInvoker)(() => currentdispatch.Text = dispID));
                        isDispActive = 1;
                    }
                    if (status == "10-6")
                    {
                        currentdispatch.Invoke((MethodInvoker)(() => currentdispatch.Text = dispID + " (Inactive)"));
                        isDispActive = 1;
                    }
                    if (status == "false")
                    {
                        currentdispatch.Invoke((MethodInvoker)(() => currentdispatch.Text = "Inactive"));
                        isDispActive = 0;
                    }
                }
                if (e.Data.StartsWith("PANIC"))
                {
                    string[] parts = e.Data.Split(':');
                    string status = parts[1];
                    string except = parts[2];
                    if (status == "true")
                    {
                        if (except != c_id)
                        {
                            isPanicActiveOther = 1;
                            PanicVoid();
                        }
                        else
                        {
                            //isPanicActive = 1;
                            //PanicVoid();
                        }
                    }
                    else
                    {
                        isPanicActiveOther = 0;
                        isPanicActive = 0;
                        if (is100Active == 0) PanicVoid();
                    }
                }
                if (e.Data.StartsWith("SIGNAL100"))
                {
                    string[] parts = e.Data.Split(':');
                    string status = parts[1];
                    string except = parts[2];
                    if (status == "true")
                    {
                        if (except != c_id)
                        {
                            is100ActiveOther = 1;
                            Signal100Void();
                        }
                        else
                        {
                            //is100Active = 1;
                            //Signal100Void();
                        }
                    }
                    else
                    {
                        is100ActiveOther = 0;
                        is100Active = 0;
                        if (isPanicActive == 0) Signal100Void();
                    }
                }
                if (e.Data.StartsWith("DEBUG"))
                {
                    label5.Invoke((MethodInvoker)(() => label5.Text = ("" + e.Data)));
                }
                if (e.Data.StartsWith("TIME"))
                {
                    string[] parts = e.Data.Split(':');
                    string hours = parts[1];
                    string minutes = parts[2];
                    string seconds = parts[3];
                    string time = $"{hours}:{minutes}:{seconds}";
                    TimeLabel.Invoke((MethodInvoker)(() => TimeLabel.Text = time));
                }
                if (e.Data.StartsWith("LOGIN"))
                {
                    string[] bigparts = e.Data.Split(';');
                    string[] parts = bigparts[0].Split(':');
                    string status = parts[1];
                    if (status == "true")
                    {
                        c_id = LoginInput.Text;
                        c_deps = bigparts[1];
                        c_rank = bigparts[2];
                        HideLogin();
                    }
                    if (status == "false")
                    {
                        label5.Invoke((MethodInvoker)(() => label5.Text = ("DEBUG: Некорректные данные!")));
                        ws.Close();
                    }
                    if (status == "blacklisted")
                    {
                        ws.Close();
                        Application.Exit();
                        InitApp();
                    }
                }
                if(e.Data.StartsWith("CALLDATA"))
                {
                    string[] parts = e.Data.Split('&');
                    CallData = JsonConvert.DeserializeObject<List<CallData>>(parts[1]);

                    JArray jsonArray = JArray.Parse(parts[1]);

                    // Подсчет строк с аргументом active: "true"
                    CallsNumber = 0;
                    foreach (var item in jsonArray)
                    {
                        if (item["active"].ToString() == "true")
                        {
                            CallsNumber++;
                        }
                    }
                    CallWindow();
                }
                if(e.Data.StartsWith("ACTIVE_UNITS"))
                {
                    string[] parts = e.Data.Split(':');
                    activeUnits = parts[1];
                }

                if (e.Data.StartsWith("LOAD_CIVS"))
                {
                    string[] parts = e.Data.Split('&');
                    Character = JsonConvert.DeserializeObject<List<Character>>(parts[1]);

                    // Подсчет строк с аргументом active: "true"
                    charactersCount = Character.Count;
                    DataAndCharLabel.Invoke((MethodInvoker)(() => DataAndCharLabel.Text = $"Персонажи ({charactersCount}/4)"));

                    for(int i =  0; i < charactersCount; i++)
                    {
                        SynchronizationContext context = SynchronizationContext.Current;
                        context.Send(new SendOrPostCallback((state) =>
                        {
                            characterPanel[i].Visible = true;
                            characterName[i].Text = Character[i].name;
                            characterBirth[i].Text = Character[i].dd + "." + Character[i].mm + "." + Character[i].yyyy;
                            characterWork[i].Text = Character[i].work;
                        }), null);
                    }
                }

                if (e.Data.StartsWith("LOAD_WEPS"))
                {
                    string[] parts = e.Data.Split('&');
                    Weapon = JsonConvert.DeserializeObject<List<Weapon>>(parts[1]);

                    // Подсчет строк с аргументом active: "true"
                    weaponsCount = Weapon.Count;
                    WeaponsLabel.Invoke((MethodInvoker)(() => WeaponsLabel.Text = $"Оружие ({weaponsCount}/4)"));

                    for (int i = 0; i < weaponsCount; i++)
                    {
                        string[] owner_parts = Weapon[i].owner.Split(' ');
                        SynchronizationContext context = SynchronizationContext.Current;
                        context.Send(new SendOrPostCallback((state) =>
                        {
                            weaponPanel[i].Visible = true;
                            weaponcharacterName[i].Text = owner_parts[0] + " " + owner_parts[1];
                            weaponcharacterBirth[i].Text = owner_parts[2];
                            weaponData[i].Text = Weapon[i].model + " " + Weapon[i].serial;
                        }), null);
                    }
                }

                if (e.Data.StartsWith("LOAD_VEHS"))
                {
                    string[] parts = e.Data.Split('&');
                    Vehicle = JsonConvert.DeserializeObject<List<Vehicle>>(parts[1]);

                    // Подсчет строк с аргументом active: "true"
                    vehiclesCount = Vehicle.Count;
                    VehiclesLabel.Invoke((MethodInvoker)(() => VehiclesLabel.Text = $"Транспорт ({vehiclesCount}/4)"));

                    for (int i = 0; i < vehiclesCount; i++)
                    {
                        string[] owner_parts = Vehicle[i].owner.Split(' ');
                        SynchronizationContext context = SynchronizationContext.Current;
                        context.Send(new SendOrPostCallback((state) =>
                        {
                            vehiclePanel[i].Visible = true;
                            vehiclecharacterName[i].Text = owner_parts[0] + " " + owner_parts[1];
                            vehiclecharacterBirth[i].Text = owner_parts[2];
                            vehiclePlate[i].Text = Vehicle[i].plate;
                            vehicleNameAndColor[i].Text = Vehicle[i].color + " " + Vehicle[i].model;
                        }), null);
                    }
                }

                if (e.Data.StartsWith("STATUS"))
                {
                    string[] parts = e.Data.Split(':');
                    string status = parts[1];
                    if(status == "10-8")
                    {
                        SystemClick.Play();
                        button108.Invoke((MethodInvoker)(() => button108.BackColor = Color.LightGreen));
                        button106.Invoke((MethodInvoker)(() => button106.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))))));
                        button107.Invoke((MethodInvoker)(() => button107.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))))));
                    }
                    if(status == "10-6")
                    {
                        SystemClick.Play();
                        button106.Invoke((MethodInvoker)(() => button106.BackColor = Color.Orange));
                        button108.Invoke((MethodInvoker)(() => button108.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))))));
                        button107.Invoke((MethodInvoker)(() => button107.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))))));
                    }
                    if(status == "10-7")
                    {
                        SystemClick.Play();
                        button107.Invoke((MethodInvoker)(() => button107.BackColor = Color.Tomato));
                        button106.Invoke((MethodInvoker)(() => button106.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))))));
                        button108.Invoke((MethodInvoker)(() => button108.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))))));
                    }
                }
                else
                {
                    label5.Invoke((MethodInvoker)(() => label5.Text = ("DEBUG: Некорректный ответ. Проверьте API сервер!")));
                }
            }
            else
            {
                label5.Invoke((MethodInvoker)(() => label5.Text = ("DEBUG: Получены пустые данные. Проверьте API сервер!")));
            }
        }

        public void CallWindow()
        {
            SynchronizationContext context2 = SynchronizationContext.Current;
            context2.Send(new SendOrPostCallback((state) =>
            {
                panel911_1.Visible = false;
                panel911_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
                panel911_2.Visible = false;
                panel911_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
                panel911_3.Visible = false;
                panel911_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
                panel911_4.Visible = false;
                panel911_4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
                panel911_5.Visible = false;
                panel911_5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
                panel911_6.Visible = false;
                panel911_6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
                panel911_7.Visible = false;
                panel911_7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
                panel911_8.Visible = false;
                panel911_8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            }), null);
            if (CallsNumber > 0)
            {
                SynchronizationContext context = SynchronizationContext.Current;
                context.Send(new SendOrPostCallback((state) =>
                {
                    if (CallData[0].type == "Panic") panel911_1.Invoke((MethodInvoker)(() => panel911_1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                    if (CallData[0].units.Contains(c_id)) panel911_1.Invoke((MethodInvoker)(() => panel911_1.BackColor = Yellow));
                    panel911_1.Visible = true;
                    panel911_1_adress1.Text = CallData[0].address1;
                    panel911_1_adress2.Text = CallData[0].address2;
                    panel911_1_id.Text = CallData[0].num;
                    panel911_1_shortdesc.Text = CallData[0].shortdesc;
                    panel911_1_type.Text = CallData[0].type;
                }), null);
            }
            if (CallsNumber > 1)
            {
                SynchronizationContext context = SynchronizationContext.Current;
                context.Send(new SendOrPostCallback((state) =>
                {
                    if (CallData[1].type == "Panic") panel911_2.Invoke((MethodInvoker)(() => panel911_2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                    if (CallData[1].units.Contains(c_id)) panel911_2.Invoke((MethodInvoker)(() => panel911_2.BackColor = Yellow));
                    panel911_2.Visible = true;
                    panel911_2_adress1.Text = CallData[1].address1;
                    panel911_2_adress2.Text = CallData[1].address2;
                    panel911_2_id.Text = CallData[1].num;
                    panel911_2_shortdesc.Text = CallData[1].shortdesc;
                    panel911_2_type.Text = CallData[1].type;
                }), null);
            }
            if (CallsNumber > 2)
            {
                SynchronizationContext context = SynchronizationContext.Current;
                context.Send(new SendOrPostCallback((state) =>
                {
                    if (CallData[2].type == "Panic") panel911_3.Invoke((MethodInvoker)(() => panel911_3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                    if (CallData[2].units.Contains(c_id)) panel911_3.Invoke((MethodInvoker)(() => panel911_3.BackColor = Yellow));
                    panel911_3.Visible = true;
                    panel911_3_adress1.Text = CallData[2].address1;
                    panel911_3_adress2.Text = CallData[2].address2;
                    panel911_3_id.Text = CallData[2].num;
                    panel911_3_shortdesc.Text = CallData[2].shortdesc;
                    panel911_3_type.Text = CallData[2].type;
                }), null);
            }
            if (CallsNumber > 3)
            {
                SynchronizationContext context = SynchronizationContext.Current;
                context.Send(new SendOrPostCallback((state) =>
                {
                    if (CallData[3].type == "Panic") panel911_4.Invoke((MethodInvoker)(() => panel911_4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                    if (CallData[3].units.Contains(c_id)) panel911_4.Invoke((MethodInvoker)(() => panel911_4.BackColor = Yellow));
                    panel911_4.Visible = true;
                    panel911_4_adress1.Text = CallData[3].address1;
                    panel911_4_adress2.Text = CallData[3].address2;
                    panel911_4_id.Text = CallData[3].num;
                    panel911_4_shortdesc.Text = CallData[3].shortdesc;
                    panel911_4_type.Text = CallData[3].type;
                }), null);
            }
            if (CallsNumber > 4)
            {
                SynchronizationContext context = SynchronizationContext.Current;
                context.Send(new SendOrPostCallback((state) =>
                {
                    if (CallData[4].type == "Panic") panel911_5.Invoke((MethodInvoker)(() => panel911_5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                    if (CallData[4].units.Contains(c_id)) panel911_5.Invoke((MethodInvoker)(() => panel911_5.BackColor = Yellow));
                    panel911_5.Visible = true;
                    panel911_5_adress1.Text = CallData[4].address1;
                    panel911_5_adress2.Text = CallData[4].address2;
                    panel911_5_id.Text = CallData[4].num;
                    panel911_5_shortdesc.Text = CallData[4].shortdesc;
                    panel911_5_type.Text = CallData[4].type;
                }), null);
            }
            if (CallsNumber > 5)
            {
                SynchronizationContext context = SynchronizationContext.Current;
                context.Send(new SendOrPostCallback((state) =>
                {
                    if (CallData[5].type == "Panic") panel911_6.Invoke((MethodInvoker)(() => panel911_6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                    if (CallData[5].units.Contains(c_id)) panel911_6.Invoke((MethodInvoker)(() => panel911_6.BackColor = Yellow));
                    panel911_6.Visible = true;
                    panel911_6_adress1.Text = CallData[5].address1;
                    panel911_6_adress2.Text = CallData[5].address2;
                    panel911_6_id.Text = CallData[5].num;
                    panel911_6_shortdesc.Text = CallData[5].shortdesc;
                    panel911_6_type.Text = CallData[5].type;
                }), null);
            }
            if (CallsNumber > 6)
            {
                SynchronizationContext context = SynchronizationContext.Current;
                context.Send(new SendOrPostCallback((state) =>
                {
                    if (CallData[6].type == "Panic") panel911_7.Invoke((MethodInvoker)(() => panel911_7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                    if (CallData[6].units.Contains(c_id)) panel911_7.Invoke((MethodInvoker)(() => panel911_7.BackColor = Yellow));
                    panel911_7.Visible = true;
                    panel911_7_adress1.Text = CallData[6].address1;
                    panel911_7_adress2.Text = CallData[6].address2;
                    panel911_7_id.Text = CallData[6].num;
                    panel911_7_shortdesc.Text = CallData[6].shortdesc;
                    panel911_7_type.Text = CallData[6].type;
                }), null);
            }
            if (CallsNumber > 7)
            {
                SynchronizationContext context = SynchronizationContext.Current;
                context.Send(new SendOrPostCallback((state) =>
                {
                    if (CallData[7].type == "Panic") panel911_8.Invoke((MethodInvoker)(() => panel911_8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                    if (CallData[7].units.Contains(c_id)) panel911_8.Invoke((MethodInvoker)(() => panel911_8.BackColor = Yellow));
                    panel911_8.Visible = true;
                    panel911_8_adress1.Text = CallData[7].address1;
                    panel911_8_adress2.Text = CallData[7].address2;
                    panel911_8_id.Text = CallData[7].num;
                    panel911_8_shortdesc.Text = CallData[7].shortdesc;
                    panel911_8_type.Text = CallData[7].type;
                }), null);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowLogin();
        }

        public void ShowLogin()
        {
            PanelLogin.Invoke((MethodInvoker)(() => PanelLogin.Visible = true));
            PanelLEOMain.Invoke((MethodInvoker)(() => PanelLEOMain.Visible = false));
            PanelUp.Invoke((MethodInvoker)(() => PanelUp.Visible = false));
            SynchronizationContext context = SynchronizationContext.Current;
            context.Send(new SendOrPostCallback((state) =>
            {
                DataLabel.Visible = false;
                DataPanel.Visible = false;
                DataPanelCross.Visible = false;
                CallsPanel.Visible = false;
                panelSetDep.Visible = false;
                PanelLeft.Visible = false;
            }), null);
        }

        public void ShowCiv()
        {
            PanelCiv.Invoke((MethodInvoker)(() => PanelCiv.Visible = true));
            SynchronizationContext context = SynchronizationContext.Current;
            context.Send(new SendOrPostCallback((state) =>
            {
                PanelLEOMain.Visible = false;
                CallsPanel.Visible = false;
                PanelLeft.Visible = false;
                panelSetDep.Visible = false;
                PanelDown.Visible = false;
                panelGos.Visible = false;
            }), null);

            for (int i = 0; i < 4; i++)
            {
                characterPanel[i] = new Panel();
                weaponPanel[i] = new Panel();
                vehiclePanel[i] = new Panel();

                characterName[i] = new Label();
                characterBirth[i] = new Label();
                characterWork[i] = new Label();

                weaponcharacterName[i] = new Label();
                weaponcharacterBirth[i] = new Label();
                weaponData[i] = new Label();

                vehiclecharacterName[i] = new Label();
                vehiclecharacterBirth[i] = new Label();
                vehiclePlate[i] = new Label();
                vehicleNameAndColor[i] = new Label();

                PanelCiv.Controls.Add(characterPanel[i]);
                PanelCiv.Controls.Add(weaponPanel[i]);
                PanelCiv.Controls.Add(vehiclePanel[i]);

                characterPanel[i].Controls.Add(characterName[i]);
                characterPanel[i].Controls.Add(characterBirth[i]);
                characterPanel[i].Controls.Add(characterWork[i]);

                weaponPanel[i].Controls.Add(weaponcharacterName[i]);
                weaponPanel[i].Controls.Add(weaponcharacterBirth[i]);
                weaponPanel[i].Controls.Add(weaponData[i]);

                vehiclePanel[i].Controls.Add(vehiclecharacterName[i]);
                vehiclePanel[i].Controls.Add(vehiclecharacterBirth[i]);
                vehiclePanel[i].Controls.Add(vehiclePlate[i]);
                vehiclePanel[i].Controls.Add(vehicleNameAndColor[i]);

                characterPanel[i].Location = new Point(313 + 220 * i, 43);
                characterPanel[i].Size = new Size(211, 88);
                characterPanel[i].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
                characterPanel[i].Cursor = System.Windows.Forms.Cursors.Hand;
                characterPanel[i].Visible = false;

                weaponPanel[i].Location = new Point(313 + 220 * i, 167);
                weaponPanel[i].Size = new Size(211, 88);
                weaponPanel[i].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
                weaponPanel[i].Cursor = System.Windows.Forms.Cursors.Hand;
                weaponPanel[i].Visible = false;

                vehiclePanel[i].Location = new Point(313 + 220 * i, 291);
                vehiclePanel[i].Size = new Size(211, 88);
                vehiclePanel[i].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
                vehiclePanel[i].Cursor = System.Windows.Forms.Cursors.Hand;
                vehiclePanel[i].Visible = false;


                // Labels


                characterName[i].Location = new Point(8, 7);
                characterName[i].Font = new System.Drawing.Font("Gotham Pro Medium", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                characterName[i].ForeColor = System.Drawing.SystemColors.Control;
                characterName[i].AutoSize = false;
                characterName[i].Size = new Size(197, 17);

                characterBirth[i].Location = new Point(8, 25);
                characterBirth[i].Font = new System.Drawing.Font("Gotham Pro Medium", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                characterBirth[i].ForeColor = System.Drawing.SystemColors.Control;
                characterBirth[i].AutoSize = false;
                characterBirth[i].Size = new Size(197, 13);

                characterWork[i].Location = new Point(8, 66);
                characterWork[i].Font = new System.Drawing.Font("Gotham Pro Medium", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                characterWork[i].ForeColor = System.Drawing.SystemColors.Control;
                characterWork[i].AutoSize = false;
                characterWork[i].Size = new Size(197, 13);

                weaponcharacterName[i].Location = characterName[i].Location;
                weaponcharacterName[i].Font = characterName[i].Font;
                weaponcharacterName[i].ForeColor = characterName[i].ForeColor;
                weaponcharacterName[i].AutoSize = characterName[i].AutoSize;
                weaponcharacterName[i].Size = characterName[i].Size;

                weaponcharacterBirth[i].Location = characterBirth[i].Location;
                weaponcharacterBirth[i].Font = characterBirth[i].Font;
                weaponcharacterBirth[i].ForeColor = characterBirth[i].ForeColor;
                weaponcharacterBirth[i].AutoSize = characterBirth[i].AutoSize;
                weaponcharacterBirth[i].Size = characterBirth[i].Size;

                weaponData[i].Location = characterWork[i].Location;
                weaponData[i].Font = characterWork[i].Font;
                weaponData[i].ForeColor = characterWork[i].ForeColor;
                weaponData[i].AutoSize = characterWork[i].AutoSize;
                weaponData[i].Size = characterWork[i].Size;

                vehiclecharacterName[i].Location = characterName[i].Location;
                vehiclecharacterName[i].Font = characterName[i].Font;
                vehiclecharacterName[i].ForeColor = characterName[i].ForeColor;
                vehiclecharacterName[i].AutoSize = characterName[i].AutoSize;
                vehiclecharacterName[i].Size = characterName[i].Size;

                vehiclecharacterBirth[i].Location = characterBirth[i].Location;
                vehiclecharacterBirth[i].Font = characterBirth[i].Font;
                vehiclecharacterBirth[i].ForeColor = characterBirth[i].ForeColor;
                vehiclecharacterBirth[i].AutoSize = characterBirth[i].AutoSize;
                vehiclecharacterBirth[i].Size = characterBirth[i].Size;

                vehicleNameAndColor[i].Location = characterWork[i].Location;
                vehicleNameAndColor[i].Font = characterWork[i].Font;
                vehicleNameAndColor[i].ForeColor = characterWork[i].ForeColor;
                vehicleNameAndColor[i].AutoSize = characterWork[i].AutoSize;
                vehicleNameAndColor[i].Size = characterWork[i].Size;

                vehiclePlate[i].Location = new Point(8, 52);
                vehiclePlate[i].Font = characterWork[i].Font;
                vehiclePlate[i].ForeColor = characterWork[i].ForeColor;
                vehiclePlate[i].AutoSize = characterWork[i].AutoSize;
                vehiclePlate[i].Size = characterWork[i].Size;
            }
            c_session_dep = "CIV";
            DepNameLabel.Invoke((MethodInvoker)(() => DepNameLabel.Text = "Гражданский департамент"));
            ws.Send($"CHANGE_DEP:4");
        }

        public void ShowLEO()
        {
            PanelLEOMain.Invoke((MethodInvoker)(() => PanelLEOMain.Visible = true));
            SynchronizationContext context = SynchronizationContext.Current;
            context.Send(new SendOrPostCallback((state) =>
            {
                PanelCiv.Visible = false;
                CallsPanel.Visible = true;
                PanelLeft.Visible = true;
                panelSetDep.Visible = false;
                PanelDown.Visible = true;
                panelGos.Visible = true;
                sessionname.Visible = true;
            }), null);
            c_session_dep = "LEO";
            DepNameLabel.Invoke((MethodInvoker)(() => DepNameLabel.Text = "Правоохранительные органы"));
            ws.Send($"CHANGE_DEP:1");
        }

        public void ShowFD()
        {
            PanelLEOMain.Invoke((MethodInvoker)(() => PanelLEOMain.Visible = true));
            SynchronizationContext context = SynchronizationContext.Current;
            context.Send(new SendOrPostCallback((state) =>
            {
                PanelCiv.Visible = false;
                CallsPanel.Visible = true;
                panelSetDep.Visible = false;
                PanelLeft.Visible = true;
                PanelDown.Visible = true;
                panelGos.Visible = false;
                sessionname.Visible = true;
            }), null);
            c_session_dep = "FD";
            DepNameLabel.Invoke((MethodInvoker)(() => DepNameLabel.Text = "Спасательный департамент"));
            ws.Send($"CHANGE_DEP:2");
        }

        public void ShowDisp()
        {
            PanelDispMain.Invoke((MethodInvoker)(() => PanelDispMain.Visible = true));
            SynchronizationContext context = SynchronizationContext.Current;
            context.Send(new SendOrPostCallback((state) =>
            {
                PanelCiv.Visible = false;
                CallsPanel.Visible = true;
                panelSetDep.Visible = false;
                PanelLeft.Visible = true;
                PanelDown.Visible = true;
                panelGos.Visible = true;
            }), null);
            c_session_dep = "Dispatch";
            DepNameLabel.Invoke((MethodInvoker)(() => DepNameLabel.Text = "Диспетчерский департамент"));
            ws.Send($"CHANGE_DEP:3");
        }

        public void HideLogin()
        {
            PanelLogin.Invoke((MethodInvoker)(() => PanelLogin.Visible = false));
            panelSetDep.Invoke((MethodInvoker)(() => panelSetDep.Visible = true));
            DepNameLabel.Invoke((MethodInvoker)(() => DepNameLabel.Text = "Выберите департамент..."));
            PanelUp.Invoke((MethodInvoker)(() => PanelUp.Visible = true));
        }

        public void DataWindow(int type, string l1 = "", string l2 = "", string l3 = "", string l4 = "", string l5 = "", string l6 = "", string l7 = "", string l8 = "", string l9 = "", string l10 = "", string l11 = "")
        {
            Form1 form1 = new Form1();
            if (dataWindowOpen == 1)
            {
                dataWindowOpen = 0;

                SynchronizationContext context = SynchronizationContext.Current;
                context.Send(new SendOrPostCallback((state) =>
                {
                    DataLabel.Visible = false;
                    DataPanel.Visible = false;
                    DataPanelSession.Visible = false;
                    DataPanelCross.Visible = false;
                    panel911_1_shortdesc.Visible = true;
                    panel911_1_id.Visible = true;
                    panel911_2_shortdesc.Visible = true;
                    panel911_2_id.Visible = true;
                    panel911_3_shortdesc.Visible = true;
                    panel911_3_id.Visible = true;
                    panel911_4_shortdesc.Visible = true;
                    panel911_4_id.Visible = true;
                    panel911_5_shortdesc.Visible = true;
                    panel911_5_id.Visible = true;
                    panel911_6_shortdesc.Visible = true;
                    panel911_6_id.Visible = true;
                    panel911_7_shortdesc.Visible = true;
                    panel911_7_id.Visible = true;
                    panel911_8_shortdesc.Visible = true;
                    panel911_8_id.Visible = true;
                    DataLabel13.Visible = false;
                    DataPanelCross.Location = new Point(344, 1);
                    DispCallCreate.Visible = false;
                }), null);

                CallsLabel.Invoke((MethodInvoker)(() => CallsLabel.Location = new System.Drawing.Point(314, 72)));
                panel911_1.Invoke((MethodInvoker)(() => panel911_1.Location = new System.Drawing.Point(0, 0)));
                panel911_1.Invoke((MethodInvoker)(() => panel911_1.Size = new System.Drawing.Size(866, 60)));
                panel911_2.Invoke((MethodInvoker)(() => panel911_2.Location = new System.Drawing.Point(0, 63 + 1)));
                panel911_2.Invoke((MethodInvoker)(() => panel911_2.Size = new System.Drawing.Size(866, 60)));
                panel911_3.Invoke((MethodInvoker)(() => panel911_3.Location = new System.Drawing.Point(0, 126 + 1)));
                panel911_3.Invoke((MethodInvoker)(() => panel911_3.Size = new System.Drawing.Size(866, 60)));
                panel911_4.Invoke((MethodInvoker)(() => panel911_4.Location = new System.Drawing.Point(0, 189 + 1)));
                panel911_4.Invoke((MethodInvoker)(() => panel911_4.Size = new System.Drawing.Size(866, 60)));
                panel911_5.Invoke((MethodInvoker)(() => panel911_5.Location = new System.Drawing.Point(0, 252 + 1)));
                panel911_5.Invoke((MethodInvoker)(() => panel911_5.Size = new System.Drawing.Size(866, 60)));
                panel911_6.Invoke((MethodInvoker)(() => panel911_6.Location = new System.Drawing.Point(324, 252 + 63 * 1 + 1)));
                panel911_6.Invoke((MethodInvoker)(() => panel911_6.Size = new System.Drawing.Size(866, 60)));
                panel911_7.Invoke((MethodInvoker)(() => panel911_7.Location = new System.Drawing.Point(324, 252 + 63 * 2 + 1)));
                panel911_7.Invoke((MethodInvoker)(() => panel911_7.Size = new System.Drawing.Size(866, 60)));
                panel911_8.Invoke((MethodInvoker)(() => panel911_8.Location = new System.Drawing.Point(324, 252 + 63 * 3 + 1)));
                panel911_8.Invoke((MethodInvoker)(() => panel911_8.Size = new System.Drawing.Size(866, 60)));
                DataLabel6.Invoke((MethodInvoker)(() => DataLabel6.Visible = false));
                DataLabel7.Invoke((MethodInvoker)(() => DataLabel7.Visible = false));
                DataLabel9.Invoke((MethodInvoker)(() => DataLabel9.Visible = false));
                DataLabel10.Invoke((MethodInvoker)(() => DataLabel10.Visible = false));
                DataLabel11.Invoke((MethodInvoker)(() => DataLabel11.Visible = false));
                DataLabel12.Invoke((MethodInvoker)(() => DataLabel12.Visible = false));
                DataLabel13.Invoke((MethodInvoker)(() => DataLabel12.Visible = false));

                DataPanelButton.Invoke((MethodInvoker)(() => DataPanelButton.Visible = false));
                UnitCallAddTextBox.Invoke((MethodInvoker)(() => UnitCallAddTextBox.Visible = false));
                return;
            }
            else
            {
                dataWindowOpen = 1;

                SynchronizationContext context = SynchronizationContext.Current;
                context.Send(new SendOrPostCallback((state) =>
                {
                    DataLabel.Visible = true;
                    DataPanel.Visible = true;
                    DataPanelCross.Visible = true;
                    panel911_1_shortdesc.Visible = false;
                    panel911_1_id.Visible = false;
                    panel911_2_shortdesc.Visible = false;
                    panel911_2_id.Visible = false;
                    panel911_3_shortdesc.Visible = false;
                    panel911_3_id.Visible = false;
                    panel911_4_shortdesc.Visible = false;
                    panel911_4_id.Visible = false;
                    panel911_5_shortdesc.Visible = false;
                    panel911_5_id.Visible = false;
                    panel911_6_shortdesc.Visible = false;
                    panel911_6_id.Visible = false;
                    panel911_7_shortdesc.Visible = false;
                    panel911_7_id.Visible = false;
                    panel911_8_shortdesc.Visible = false;
                    panel911_8_id.Visible = false;
                }), null);

                CallsLabel.Invoke((MethodInvoker)(() => CallsLabel.Location = new System.Drawing.Point(688, 74)));
                panel911_1.Invoke((MethodInvoker)(() => panel911_1.Location = new System.Drawing.Point(375, 1)));
                panel911_1.Invoke((MethodInvoker)(() => panel911_1.Size = new System.Drawing.Size(491, 60)));
                panel911_2.Invoke((MethodInvoker)(() => panel911_2.Location = new System.Drawing.Point(375, 63 + 1)));
                panel911_2.Invoke((MethodInvoker)(() => panel911_2.Size = new System.Drawing.Size(491, 60)));
                panel911_3.Invoke((MethodInvoker)(() => panel911_3.Location = new System.Drawing.Point(375, 63 + 63 * 1 + 1)));
                panel911_3.Invoke((MethodInvoker)(() => panel911_3.Size = new System.Drawing.Size(491, 60)));
                panel911_4.Invoke((MethodInvoker)(() => panel911_4.Location = new System.Drawing.Point(375, 63 + 63 * 2 + 1)));
                panel911_4.Invoke((MethodInvoker)(() => panel911_4.Size = new System.Drawing.Size(491, 60)));
                panel911_5.Invoke((MethodInvoker)(() => panel911_5.Location = new System.Drawing.Point(375, 63 + 63 * 3 + 1)));
                panel911_5.Invoke((MethodInvoker)(() => panel911_5.Size = new System.Drawing.Size(491, 60)));
                panel911_6.Invoke((MethodInvoker)(() => panel911_6.Location = new System.Drawing.Point(375, 63 + 63 * 4 + 1)));
                panel911_6.Invoke((MethodInvoker)(() => panel911_6.Size = new System.Drawing.Size(491, 60)));
                panel911_7.Invoke((MethodInvoker)(() => panel911_7.Location = new System.Drawing.Point(375, 63 + 63 * 5 + 1)));
                panel911_7.Invoke((MethodInvoker)(() => panel911_7.Size = new System.Drawing.Size(491, 60)));
                panel911_8.Invoke((MethodInvoker)(() => panel911_8.Location = new System.Drawing.Point(375, 63 + 63 * 6 + 1)));
                panel911_8.Invoke((MethodInvoker)(() => panel911_8.Size = new System.Drawing.Size(491, 60)));

                DataLabel13.Invoke((MethodInvoker)(() => DataLabel12.Visible = false));

                DataPanelButton.Invoke((MethodInvoker)(() => DataPanelButton.Visible = false));
                UnitCallAddTextBox.Invoke((MethodInvoker)(() => UnitCallAddTextBox.Visible = false));

                if(type == 4)
                {
                    SynchronizationContext context2 = SynchronizationContext.Current;
                    context.Send(new SendOrPostCallback((state) =>
                    {
                        DispCallCreate.Visible = true;
                    }), null);
                }

                if (type == 3)
                {
                    SynchronizationContext context2 = SynchronizationContext.Current;
                    context.Send(new SendOrPostCallback((state) =>
                    {
                        DataPanelSession.Visible = true;
                        if(c_session_dep == "FD" || c_session_dep == "Dispatch")
                        {
                            sessionTypeSelect.Visible = false;
                            sessionDivSelect.Visible = false;
                            sessionDepSelect.Visible = false;
                        }
                        if (c_session_dep == "LEO")
                        {
                            sessionTypeSelect.Visible = true;
                            sessionDivSelect.Visible = true;
                            sessionDepSelect.Visible = true;
                        }
                    }), null);
                }
                if (type == 911+1)
                {
                    SynchronizationContext context2 = SynchronizationContext.Current;
                    context.Send(new SendOrPostCallback((state) =>
                    {
                        DataLabel1.Text = "Инцидент: №"  + CallData[0].num;
                        DataLabel2.Text = "Тип: " + CallData[0].type;
                        DataLabel3.Text = "Адресс 1: " + CallData[0].address1;
                        DataLabel4.Text = "Адресс 2: " + CallData[0].address2;
                        DataLabel5.Text = "Время: " + CallData[0].time;
                        DataLabel6.Visible = true;
                        DataLabel6.Text = "Краткое описание: " + CallData[0].shortdesc;
                        DataLabel7.Visible = true;
                        DataLabel7.Text = "Подробное описание: " + CallData[0].longdesc;
                        DataLabel9.Visible = true;
                        DataLabel9.Text = "Юниты (прикреплены): " + CallData[0].units;
                        DataPanelButton.Invoke((MethodInvoker)(() => DataPanelButton.Visible = true));
                        if(c_session_dep == "Dispatch")
                        {
                            DataLabel13.Visible = true;
                            DataLabel13.Text = "Доступные юниты: " + activeUnits;
                            UnitCallAddTextBox.Visible = true;
                        }
                    }), null);
                }
                if (type == 911 + 2)
                {
                    SynchronizationContext context2 = SynchronizationContext.Current;
                    context.Send(new SendOrPostCallback((state) =>
                    {
                        DataLabel1.Text = "Инцидент: №" + CallData[1].num;
                        DataLabel2.Text = "Тип: " + CallData[1].type;
                        DataLabel3.Text = "Адресс 1: " + CallData[1].address1;
                        DataLabel4.Text = "Адресс 2: " + CallData[1].address2;
                        DataLabel5.Text = "Время: " + CallData[1].time;
                        DataLabel6.Visible = true;
                        DataLabel6.Text = "Краткое описание: " + CallData[1].shortdesc;
                        DataLabel7.Visible = true;
                        DataLabel7.Text = "Подробное описание: " + CallData[1].longdesc;
                        DataLabel9.Visible = true;
                        DataLabel9.Text = "Юниты (прикреплены): " + CallData[1].units;
                        DataPanelButton.Invoke((MethodInvoker)(() => DataPanelButton.Visible = true));
                        if (c_session_dep == "Dispatch")
                        {
                            DataLabel13.Visible = true;
                            DataLabel13.Text = "Доступные юниты: " + activeUnits;
                            UnitCallAddTextBox.Visible = true;
                        }
                    }), null);
                }
                if (type == 911 + 3)
                {
                    SynchronizationContext context2 = SynchronizationContext.Current;
                    context.Send(new SendOrPostCallback((state) =>
                    {
                        DataLabel1.Text = "Инцидент: №" + CallData[2].num;
                        DataLabel2.Text = "Тип: " + CallData[2].type;
                        DataLabel3.Text = "Адресс 1: " + CallData[2].address1;
                        DataLabel4.Text = "Адресс 2: " + CallData[2].address2;
                        DataLabel5.Text = "Время: " + CallData[2].time;
                        DataLabel6.Visible = true;
                        DataLabel6.Text = "Краткое описание: " + CallData[2].shortdesc;
                        DataLabel7.Visible = true;
                        DataLabel7.Text = "Подробное описание: " + CallData[2].longdesc;
                        DataLabel9.Visible = true;
                        DataLabel9.Text = "Юниты (прикреплены): " + CallData[2].units;
                        DataPanelButton.Invoke((MethodInvoker)(() => DataPanelButton.Visible = true));
                        if (c_session_dep == "Dispatch")
                        {
                            DataLabel13.Visible = true;
                            DataLabel13.Text = "Доступные юниты: " + activeUnits;
                            UnitCallAddTextBox.Visible = true;
                        }
                    }), null);
                }
                if (type == 911 + 4)
                {
                    SynchronizationContext context2 = SynchronizationContext.Current;
                    context.Send(new SendOrPostCallback((state) =>
                    {
                        DataLabel1.Text = "Инцидент: №" + CallData[3].num;
                        DataLabel2.Text = "Тип: " + CallData[3].type;
                        DataLabel3.Text = "Адресс 1: " + CallData[3].address1;
                        DataLabel4.Text = "Адресс 2: " + CallData[3].address2;
                        DataLabel5.Text = "Время: " + CallData[3].time;
                        DataLabel6.Visible = true;
                        DataLabel6.Text = "Краткое описание: " + CallData[3].shortdesc;
                        DataLabel7.Visible = true;
                        DataLabel7.Text = "Подробное описание: " + CallData[3].longdesc;
                        DataLabel9.Visible = true;
                        DataLabel9.Text = "Юниты (прикреплены): " + CallData[3].units;
                        DataPanelButton.Invoke((MethodInvoker)(() => DataPanelButton.Visible = true));
                        if (c_session_dep == "Dispatch")
                        {
                            DataLabel13.Visible = true;
                            DataLabel13.Text = "Доступные юниты: " + activeUnits;
                            UnitCallAddTextBox.Visible = true;
                        }
                    }), null);
                }
                if (type == 911 + 5)
                {
                    SynchronizationContext context2 = SynchronizationContext.Current;
                    context.Send(new SendOrPostCallback((state) =>
                    {
                        DataLabel1.Text = "Инцидент: №" + CallData[4].num;
                        DataLabel2.Text = "Тип: " + CallData[4].type;
                        DataLabel3.Text = "Адресс 1: " + CallData[4].address1;
                        DataLabel4.Text = "Адресс 2: " + CallData[4].address2;
                        DataLabel5.Text = "Время: " + CallData[4].time;
                        DataLabel6.Visible = true;
                        DataLabel6.Text = "Краткое описание: " + CallData[4].shortdesc;
                        DataLabel7.Visible = true;
                        DataLabel7.Text = "Подробное описание: " + CallData[4].longdesc;
                        DataLabel9.Visible = true;
                        DataLabel9.Text = "Юниты (прикреплены): " + CallData[4].units;
                        DataPanelButton.Invoke((MethodInvoker)(() => DataPanelButton.Visible = true));
                        if (c_session_dep == "Dispatch")
                        {
                            DataLabel13.Visible = true;
                            DataLabel13.Text = "Доступные юниты: " + activeUnits;
                            UnitCallAddTextBox.Visible = true;
                        }
                    }), null);
                }
                if (type == 911 + 6)
                {
                    SynchronizationContext context2 = SynchronizationContext.Current;
                    context.Send(new SendOrPostCallback((state) =>
                    {
                        DataLabel1.Text = "Инцидент: №" + CallData[5].num;
                        DataLabel2.Text = "Тип: " + CallData[5].type;
                        DataLabel3.Text = "Адресс 1: " + CallData[5].address1;
                        DataLabel4.Text = "Адресс 2: " + CallData[5].address2;
                        DataLabel5.Text = "Время: " + CallData[5].time;
                        DataLabel6.Visible = true;
                        DataLabel6.Text = "Краткое описание: " + CallData[5].shortdesc;
                        DataLabel7.Visible = true;
                        DataLabel7.Text = "Подробное описание: " + CallData[5].longdesc;
                        DataLabel9.Visible = true;
                        DataLabel9.Text = "Юниты (прикреплены): " + CallData[5].units;
                        DataPanelButton.Invoke((MethodInvoker)(() => DataPanelButton.Visible = true));
                        if (c_session_dep == "Dispatch")
                        {
                            DataLabel13.Visible = true;
                            DataLabel13.Text = "Доступные юниты: " + activeUnits;
                            UnitCallAddTextBox.Visible = true;
                        }
                    }), null);
                }
                if (type == 911 + 7)
                {
                    SynchronizationContext context2 = SynchronizationContext.Current;
                    context.Send(new SendOrPostCallback((state) =>
                    {
                        DataLabel1.Text = "Инцидент: №" + CallData[6].num;
                        DataLabel2.Text = "Тип: " + CallData[6].type;
                        DataLabel3.Text = "Адресс 1: " + CallData[6].address1;
                        DataLabel4.Text = "Адресс 2: " + CallData[6].address2;
                        DataLabel5.Text = "Время: " + CallData[6].time;
                        DataLabel6.Visible = true;
                        DataLabel6.Text = "Краткое описание: " + CallData[6].shortdesc;
                        DataLabel7.Visible = true;
                        DataLabel7.Text = "Подробное описание: " + CallData[6].longdesc;
                        DataLabel9.Visible = true;
                        DataLabel9.Text = "Юниты (прикреплены): " + CallData[6].units;
                        DataPanelButton.Invoke((MethodInvoker)(() => DataPanelButton.Visible = true));
                        if (c_session_dep == "Dispatch")
                        {
                            DataLabel13.Visible = true;
                            DataLabel13.Text = "Доступные юниты: " + activeUnits;
                            UnitCallAddTextBox.Visible = true;
                        }
                    }), null);
                }
                if (type == 911 + 8)
                {
                    SynchronizationContext context2 = SynchronizationContext.Current;
                    context.Send(new SendOrPostCallback((state) =>
                    {
                        DataLabel1.Text = "Инцидент: №" + CallData[7].num;
                        DataLabel2.Text = "Тип: " + CallData[7].type;
                        DataLabel3.Text = "Адресс 1: " + CallData[7].address1;
                        DataLabel4.Text = "Адресс 2: " + CallData[7].address2;
                        DataLabel5.Text = "Время: " + CallData[7].time;
                        DataLabel6.Visible = true;
                        DataLabel6.Text = "Краткое описание: " + CallData[7].shortdesc;
                        DataLabel7.Visible = true;
                        DataLabel7.Text = "Подробное описание: " + CallData[7].longdesc;
                        DataLabel9.Visible = true;
                        DataLabel9.Text = "Юниты (прикреплены): " + CallData[7].units;
                        DataPanelButton.Invoke((MethodInvoker)(() => DataPanelButton.Visible = true));
                        if (c_session_dep == "Dispatch")
                        {
                            DataLabel13.Visible = true;
                            DataLabel13.Text = "Доступные юниты: " + activeUnits;
                            UnitCallAddTextBox.Visible = true;
                        }
                    }), null);
                }

                if (type == 2)
                {
                    DataLabel1.Invoke((MethodInvoker)(() => DataLabel1.Text = "Серийный номер: " + l1));
                    DataLabel2.Invoke((MethodInvoker)(() => DataLabel2.Text = "Владелец: " + l2));
                    DataLabel3.Invoke((MethodInvoker)(() => DataLabel3.Text = "Модель: " + l3));
                    DataLabel5.Invoke((MethodInvoker)(() => DataLabel5.Text = "Штат регистрации: " + l4));
                    DataLabel4.Invoke((MethodInvoker)(() => DataLabel4.Text = "Дата регистрации: " + l5));
                }
                if (type == 1)
                {
                    DataLabel6.Invoke((MethodInvoker)(() => DataLabel6.Visible = true));

                    DataLabel1.Invoke((MethodInvoker)(() => DataLabel1.Text = "Номерной знак: " + l1));
                    DataLabel2.Invoke((MethodInvoker)(() => DataLabel2.Text = "Владелец: " + l2));
                    DataLabel3.Invoke((MethodInvoker)(() => DataLabel3.Text = "Цвет: " + l3));
                    DataLabel4.Invoke((MethodInvoker)(() => DataLabel4.Text = "Модель: " + l4));
                    DataLabel5.Invoke((MethodInvoker)(() => DataLabel5.Text = "Штат регистрации: " + l5));
                    DataLabel6.Invoke((MethodInvoker)(() => DataLabel6.Text = "Дата регистрации: " + l6));

                    if (l7 == "1")
                    {
                        DataLabel12.Invoke((MethodInvoker)(() => DataLabel12.Visible = true));
                        DataLabel12.Invoke((MethodInvoker)(() => DataLabel12.Text = "Обнаружен Розыск!"));
                        DataLabel10.Invoke((MethodInvoker)(() => DataLabel10.Visible = true));
                        DataLabel11.Invoke((MethodInvoker)(() => DataLabel11.Visible = true));

                        DataLabel10.Invoke((MethodInvoker)(() => DataLabel10.Text = "Описание: " + l8));
                        DataLabel11.Invoke((MethodInvoker)(() => DataLabel11.Text = "Дата создания: " + l9));
                    }
                }

                if (type == 0)
                {
                    DataLabel6.Invoke((MethodInvoker)(() => DataLabel6.Visible = true));
                    DataLabel7.Invoke((MethodInvoker)(() => DataLabel7.Visible = true));

                    string driverlic = "";
                    if (l5 == "0") driverlic = "Нету";
                    if (l5 == "8") driverlic = "Фальшивая";
                    if (l5 == "7") driverlic = "Просрочена";
                    if (l5 == "6") driverlic = "Все права";
                    if (l5 == "5") driverlic = "Лётные права";
                    if (l5 == "4") driverlic = "Коммерческие Права (CDL)";
                    if (l5 == "3") driverlic = "Мотоциклетные Права";
                    if (l5 == "2") driverlic = "Права для Шоферов";
                    if (l5 == "1") driverlic = "Стандартные Права";
                    string sex = "";
                    string wep_lic = "";
                    if (l3 == "0") sex = "Мужской";
                    if (l3 == "1") sex = "Женский";
                    if (l3 == "3") sex = "Небинарный";

                    if (l6 == "3") wep_lic = "Фальшивая";
                    if (l6 == "2") wep_lic = "Просрочена";
                    if (l6 == "1") wep_lic = "Имеется";
                    if (l6 == "0") wep_lic = "Нету";

                    if (l8 == "0")
                    {
                        DataLabel1.Invoke((MethodInvoker)(() => DataLabel1.Text = "Имя: " + l1));
                        DataLabel2.Invoke((MethodInvoker)(() => DataLabel2.Text = "Дата рождения: " + l2));
                        DataLabel3.Invoke((MethodInvoker)(() => DataLabel3.Text = "Пол: " + sex));
                        if(c_session_dep == "FD" || c_session_dep == "Dispatch") DataLabel4.Invoke((MethodInvoker)(() => DataLabel4.Text = "Мед. Сведенья: " + l4));
                        else DataLabel4.Invoke((MethodInvoker)(() => DataLabel4.Text = ""));
                        DataLabel5.Invoke((MethodInvoker)(() => DataLabel5.Text = "Лицензия на т/c: " + "Недоступно"));
                        DataLabel6.Invoke((MethodInvoker)(() => DataLabel6.Text = "Лицензия на оружие: " + "Недоступно"));
                        DataLabel7.Invoke((MethodInvoker)(() => DataLabel7.Text = "Трудоустройство: " + l7));
                    }
                    else
                    {
                        DataLabel10.Invoke((MethodInvoker)(() => DataLabel10.Visible = true));
                        DataLabel11.Invoke((MethodInvoker)(() => DataLabel11.Visible = true));
                        DataLabel12.Invoke((MethodInvoker)(() => DataLabel12.Visible = true));

                        DataLabel1.Invoke((MethodInvoker)(() => DataLabel1.Text = "Имя: " + l1));
                        DataLabel2.Invoke((MethodInvoker)(() => DataLabel2.Text = "Дата рождения: " + l2));
                        DataLabel3.Invoke((MethodInvoker)(() => DataLabel3.Text = "Пол: " + sex));
                        if (c_session_dep == "FD" || c_session_dep == "Dispatch") DataLabel4.Invoke((MethodInvoker)(() => DataLabel4.Text = "Мед. Сведенья: " + l4));
                        else DataLabel4.Invoke((MethodInvoker)(() => DataLabel4.Text = ""));
                        DataLabel5.Invoke((MethodInvoker)(() => DataLabel5.Text = "Лицензия на т/c: " + driverlic));
                        DataLabel6.Invoke((MethodInvoker)(() => DataLabel6.Text = "Лицензия на оружие: " + wep_lic));
                        DataLabel7.Invoke((MethodInvoker)(() => DataLabel7.Text = "Трудоустройство: " + l7));
                        DataLabel12.Invoke((MethodInvoker)(() => DataLabel12.Text = $"Обнаружен {l9}!"));
                        //DataLabel9.Invoke((MethodInvoker)(() => DataLabel9.Text = "Тип: " + l9));
                        DataLabel10.Invoke((MethodInvoker)(() => DataLabel10.Text = "Описание: " + l10));
                        DataLabel11.Invoke((MethodInvoker)(() => DataLabel11.Text = "Дата создания: " + l11));
                    }
                    return;
                }
            }
        }

        public void PanicVoid()
        {
            if (isPanicActiveOther == 0)
            {
                if (isPanicActive == 1)
                {
                    PanicButton.Invoke((MethodInvoker)(() => PanicButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                    if (IsPanicPlaying == 0) { PanicSound.PlayLooping(); IsPanicPlaying = 1; };
                    return;
                }
                else
                {
                    PanicButton.Invoke((MethodInvoker)(() => PanicButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))))));
                    isPanicActive = 0;
                    PanicSound.Stop();
                    IsPanicPlaying = 0;
                    return;
                }
            }
            else
            {
                PanicButton.Invoke((MethodInvoker)(() => PanicButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                isPanicActive = 1;
                if (IsPanicPlaying == 0)
                {
                    PanicSound.PlayLooping();
                    IsPanicPlaying = 1;
                }
                return;
            }
        }

        public void Signal100Void()
        {
            if (is100ActiveOther == 0)
            {
                if (is100Active == 1)
                {
                    Signal100.Invoke((MethodInvoker)(() => Signal100.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                    if (Is100Playing == 0) { Signal100Sound.PlayLooping(); Is100Playing = 1; }
                    return;
                }
                else
                {
                    Signal100.Invoke((MethodInvoker)(() => Signal100.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))))));
                    is100Active = 0;
                    Signal100Sound.Stop();
                    Is100Playing = 0;
                    return;
                }
            }
            else
            {
                Signal100.Invoke((MethodInvoker)(() => Signal100.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                is100Active = 1;
                if (Is100Playing == 0)
                {
                    Signal100Sound.PlayLooping();
                    Is100Playing = 1;
                }
                return;
            }
        }

        private void panic_Click(object sender, EventArgs e)
        {
            if (c_status != "active") return;
            if (isPanicActiveOther == 0)
            {
                if (isPanicActive == 0)
                {
                    PanicButton.Invoke((MethodInvoker)(() => PanicButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                    isPanicActive = 1;
                    IsPanicPlaying = 1;
                    PanicSound.PlayLooping();
                    ws.Send("PANIC:true");
                    return;
                }
                if (isPanicActive == 1)
                {
                    PanicButton.Invoke((MethodInvoker)(() => PanicButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))))));
                    isPanicActive = 0;
                    IsPanicPlaying = 0;
                    PanicSound.Stop();
                    ws.Send("PANIC:false");
                    return;
                }
            }
            else
            {
                return;
            }
        }

        private void Signal100_Click(object sender, EventArgs e)
        {
            if (c_status != "active") return;
            if (is100ActiveOther == 0)
            {
                if (is100Active == 0)
                {
                    Signal100.Invoke((MethodInvoker)(() => Signal100.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))))));
                    is100Active = 1;
                    Is100Playing = 1;
                    Signal100Sound.PlayLooping();
                    ws.Send("SIGNAL100:true");
                    return;
                }
                if (is100Active == 1)
                {
                    Signal100.Invoke((MethodInvoker)(() => Signal100.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))))));
                    is100Active = 0;
                    Is100Playing = 0;
                    Signal100Sound.Stop();
                    ws.Send("SIGNAL100:false");
                    return;
                }
            }
            else
            {
                return;
            }
        }

        private void SelectDep_Click(object sender, EventArgs e)
        {
            if (IsSelectDepOpen == 1)
            {
                SynchronizationContext context = SynchronizationContext.Current;
                context.Send(new SendOrPostCallback((state) =>
                {
                    panelSetDep.Visible = true;
                }), null);

                IsSelectDepOpen = 0;
            }
            else
            {
                SynchronizationContext context = SynchronizationContext.Current;
                context.Send(new SendOrPostCallback((state) =>
                {
                    panelSetDep.Visible = false;
                }), null);

                IsSelectDepOpen = 1;
            }
        }

        private void NCICName_Enter(object sender, EventArgs e)
        {
            NCICNameLabel.Invoke((MethodInvoker)(() => NCICNameLabel.Hide()));
        }

        private void NCICName_Leave(object sender, EventArgs e)
        {
            if (NCICName.Text == string.Empty) { NCICNameLabel.Invoke((MethodInvoker)(() => NCICNameLabel.Show())); }
            else { return; }
        }

        private void NCICDay_Enter(object sender, EventArgs e)
        {
            NCICDDLabel.Invoke((MethodInvoker)(() => NCICDDLabel.Hide()));
        }

        private void NCICDay_Leave(object sender, EventArgs e)
        {
            if (NCICDay.Text == string.Empty) { NCICDDLabel.Invoke((MethodInvoker)(() => NCICDDLabel.Show())); }
            else { return; }
        }

        private void NCICMonth_Enter(object sender, EventArgs e)
        {
            NCICMMLabel.Invoke((MethodInvoker)(() => NCICMMLabel.Hide()));
        }

        private void NCICMonth_Leave(object sender, EventArgs e)
        {
            if (NCICMonth.Text == string.Empty) { NCICMMLabel.Invoke((MethodInvoker)(() => NCICMMLabel.Show())); }
            else { return; }
        }

        private void NCICYear_Enter(object sender, EventArgs e)
        {
            NCICYYYYLabel.Invoke((MethodInvoker)(() => NCICYYYYLabel.Hide()));
        }

        private void NCICYear_Leave(object sender, EventArgs e)
        {
            if (NCICYear.Text == string.Empty) { NCICYYYYLabel.Invoke((MethodInvoker)(() => NCICYYYYLabel.Show())); }
            else { return; }
        }

        private void WSNSBox_Enter(object sender, EventArgs e)
        {
            WSNSNum.Invoke((MethodInvoker)(() => WSNSNum.Hide()));
        }

        private void WSNSBox_Leave(object sender, EventArgs e)
        {
            if (WSNSBox.Text == string.Empty) { WSNSNum.Invoke((MethodInvoker)(() => WSNSNum.Show())); }
            else { return; }
        }

        private void PlatesBox_Enter(object sender, EventArgs e)
        {
            PlatesNum.Invoke((MethodInvoker)(() => PlatesNum.Hide()));
        }

        private void PlatesBox_Leave(object sender, EventArgs e)
        {
            if (PlatesBox.Text == string.Empty) { PlatesNum.Invoke((MethodInvoker)(() => PlatesNum.Show())); }
            else { return; }
        }

        private void PanelLEOMain_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            DataWindow(0, null, null, null, null, null, null, null, null, null, null, null);
        }

        private void NCICSearch_Click(object sender, EventArgs e)
        {
            ws.Send("NCIC_REQUEST:" + NCICName.Text + ":" + NCICDay.Text + ":" + NCICMonth.Text + ":" + NCICYear.Text);
        }

        private void WSNSSearch_Click(object sender, EventArgs e)
        {
            ws.Send("WSNS_REQUEST:" + WSNSBox.Text);
        }

        private void PlatesSearch_Click(object sender, EventArgs e)
        {
            ws.Send("PLATES_REQUEST:" + PlatesBox.Text);
        }

        private void button108_Click(object sender, EventArgs e)
        {
            if (c_nickname == "NeedToUpdate") return;
            SystemClick.Play();
            ws.Send("STATUS:ACTIVE");
            button108.Invoke((MethodInvoker)(() => button108.BackColor = Color.LightGreen));
            button106.Invoke((MethodInvoker)(() => button106.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))))));
            button107.Invoke((MethodInvoker)(() => button107.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))))));
            c_status = "active";
        }

        private void button106_Click(object sender, EventArgs e)
        {
            if (c_nickname == "NeedToUpdate") return;
            SystemClick.Play();
            ws.Send("STATUS:BUSY");
            button106.Invoke((MethodInvoker)(() => button106.BackColor = Color.Orange));
            button108.Invoke((MethodInvoker)(() => button108.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))))));
            button107.Invoke((MethodInvoker)(() => button107.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))))));
            c_status = "busy";
        }

        private void button107_Click(object sender, EventArgs e)
        {
            SystemClick.Play();
            ws.Send("STATUS:INACTIVE");
            button107.Invoke((MethodInvoker)(() => button107.BackColor = Color.Tomato));
            button106.Invoke((MethodInvoker)(() => button106.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))))));
            button108.Invoke((MethodInvoker)(() => button108.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))))));
            c_status = "inactive";
        }

        private void panel911_1_i_Click(object sender, EventArgs e)
        {
            DataWindow(911 + 1, null, null, null, null, null, null, null, null, null, null, null);
            activeCallWindow = 1;
        }

        private void panel911_2_i_Click(object sender, EventArgs e)
        {
            DataWindow(911 + 2, null, null, null, null, null, null, null, null, null, null, null);
            activeCallWindow = 2;
        }

        private void panel911_3_i_Click(object sender, EventArgs e)
        {
            DataWindow(911 + 3, null, null, null, null, null, null, null, null, null, null, null);
            activeCallWindow = 3;
        }

        private void panel911_4_i_Click(object sender, EventArgs e)
        {
            DataWindow(911 + 4, null, null, null, null, null, null, null, null, null, null, null);
            activeCallWindow = 4;
        }

        private void panel911_5_i_Click(object sender, EventArgs e)
        {
            DataWindow(911 + 5, null, null, null, null, null, null, null, null, null, null, null);
            activeCallWindow = 5;
        }

        private void panel911_6_i_Click(object sender, EventArgs e)
        {
            DataWindow(911 + 6, null, null, null, null, null, null, null, null, null, null, null);
            activeCallWindow = 6;
        }

        private void panel911_7_i_Click(object sender, EventArgs e)
        {
            DataWindow(911 + 7, null, null, null, null, null, null, null, null, null, null, null);
            activeCallWindow = 7;
        }

        private void panel911_8_i_Click(object sender, EventArgs e)
        {
            DataWindow(911 + 8, null, null, null, null, null, null, null, null, null, null, null);
            activeCallWindow = 8;
        }

        private void DataPanelButton_Click(object sender, EventArgs e)
        {
            SystemClick.Play();
            if (c_session_dep == "Dispatch")
            {
                if (CallData[activeCallWindow - 1].units.Contains(UnitCallAddTextBox.Text)) ws.Send($"CALL_UNIT_REMOVE:{UnitCallAddTextBox.Text}:{Convert.ToString(activeCallWindow - 1)}");
                else ws.Send($"CALL_UNIT_ADD:{UnitCallAddTextBox.Text}:{Convert.ToString(activeCallWindow - 1)}");
            }
            else
            {
                if (CallData[activeCallWindow - 1].units.Contains(c_id)) ws.Send($"CALL_UNIT_REMOVE:{c_id}:{Convert.ToString(activeCallWindow - 1)}");
                else ws.Send($"CALL_UNIT_ADD:{c_id}:{Convert.ToString(activeCallWindow - 1)}");
            }
        }

        private void checkLeo_CheckedChanged(object sender, EventArgs e)
        {
            if (!c_deps.Contains("1")) checkLeo.Checked = false;
            else ShowLEO();
        }

        private void checkSpas_CheckedChanged(object sender, EventArgs e)
        {
            if (!c_deps.Contains("2")) checkLeo.Checked = false;
            else ShowFD();
        }

        private void nickname_Click(object sender, EventArgs e)
        {
            DataWindow(3, null, null, null, null, null, null, null, null, null, null, null);
        }

        private void sessionNameEnter_Enter(object sender, EventArgs e)
        {
            sessionNameLabel.Invoke((MethodInvoker)(() => sessionNameLabel.Visible = false));
        }

        private void sessionNameEnter_Leave(object sender, EventArgs e)
        {
            if (sessionNameEnter.Text.IsNullOrEmpty())
            {
                sessionNameLabel.Invoke((MethodInvoker)(() => sessionNameLabel.Visible = true));
            }
            else
            {
                return;
            }
        }

        private void sessionDataEnter_Click(object sender, EventArgs e)
        {
            if (c_session_dep == "LEO") if (sessionNameEnter.Text.IsNullOrEmpty() || !(SessionLSSD.Checked || SessionLSPD.Checked || SessionSAHP.Checked || SessionPatrol.Checked || SessionMetro.Checked || SessionGED.Checked || SessionDetective.Checked) || !(sessionDefault.Checked || sessionSupervisor.Checked) || !(sessionMoto.Checked || sessionAir.Checked || sessionCar.Checked)) return;
            if (c_session_dep == "Dispatch" || c_session_dep == "FD") if (sessionNameEnter.Text.IsNullOrEmpty() || !(sessionDefault.Checked || sessionSupervisor.Checked)) return;
            string idf1 = "";
            string idf2 = "";
            string idf3 = "";
            string duopatrol = "";
            if(duopatrolnum.Text != "Номер парного патруля (если нет - не трогайте) (> 0 < 100)" && duopatrolnum.Text != "")
            {
                if (Convert.ToInt32(duopatrolnum.Text) > 0 && Convert.ToInt32(duopatrolnum.Text) < 100)
                {
                    duopatrol = "-" + duopatrolnum.Text;
                }
                else return;
            }
            if(SessionPatrol.Checked)
            {
                if(SessionLSPD.Checked)
                {
                    idf1 = "20";
                    idf2 = "/L";
                }
                if(SessionLSSD.Checked)
                {
                    idf1 = "28";
                    idf2 = "/B";
                }
                if(SessionSAHP.Checked)
                {
                    idf1 = "51";
                    idf2 = "/D";
                }
            }
            if(SessionMetro.Checked)
            {
                if (SessionLSPD.Checked)
                {
                    idf1 = "W";
                    idf2 = "/L";
                }
                if (SessionLSSD.Checked)
                {
                    idf1 = "W";
                    idf2 = "/B";
                }
                if (SessionSAHP.Checked)
                {
                    idf1 = "W";
                    idf2 = "/D";
                }
            }
            if (SessionDetective.Checked)
            {
                if (SessionLSPD.Checked)
                {
                    idf1 = "X";
                    idf2 = "/L";
                }
                if (SessionLSSD.Checked)
                {
                    idf1 = "X";
                    idf2 = "/B";
                }
                if (SessionSAHP.Checked)
                {
                    idf1 = "X";
                    idf2 = "/D";
                }
            }
            if (SessionGED.Checked)
            {
                if (SessionLSPD.Checked)
                {
                    idf1 = "G";
                    idf2 = "/L";
                }
                if (SessionLSSD.Checked)
                {
                    idf1 = "G";
                    idf2 = "/B";
                }
                if (SessionSAHP.Checked)
                {
                    idf1 = "G";
                    idf2 = "/D";
                }
            }

            if (sessionAir.Checked)
            {
                if (SessionLSPD.Checked)
                {
                    idf1 = "AIR";
                    idf2 = "/L";
                }
                if (SessionLSSD.Checked)
                {
                    idf1 = "AIR";
                    idf2 = "/B";
                }
                if (SessionSAHP.Checked)
                {
                    idf1 = "AIR";
                    idf2 = "/D";
                }
            }
            if (sessionMoto.Checked)
            {
                if (SessionLSPD.Checked)
                {
                    idf1 = "M";
                    idf2 = "/L";
                }
                if (SessionLSSD.Checked)
                {
                    idf1 = "M";
                    idf2 = "/B";
                }
                if (SessionSAHP.Checked)
                {
                    idf1 = "M";
                    idf2 = "/D";
                }
            }
            if(sessionSupervisor.Checked)
            {
                idf3 = "-0";
            }
            string fulldep = $"ErrorName:{c_id}";
            if (c_session_dep == "LEO") fulldep = sessionNameEnter.Text + " " + c_rank + " " + idf1 + idf2 + "-" + c_id + duopatrol + idf3;
            if (c_session_dep == "Dispatch") fulldep = sessionNameEnter.Text + " " + c_rank + " N-" + c_id + duopatrol + idf3;
            if (c_session_dep == "FD") fulldep = sessionNameEnter.Text + " " + c_rank + " F-" + c_id + duopatrol + idf3;
            sessionname.Invoke((MethodInvoker)(() => sessionname.Text = fulldep));
            c_sessionid = c_rank + " " + idf1 + idf2 + "-" + c_id + duopatrol + idf3;
            c_nickname = sessionNameEnter.Text;
            ws.Send("UNIT_SESSION_DATA:" + fulldep);
        }

        private void checkDisp_CheckedChanged(object sender, EventArgs e)
        {
            if (!c_deps.Contains("3")) checkLeo.Checked = false;
            else ShowDisp();
        }

        private void ReportButton_Click(object sender, EventArgs e)
        {
            if(c_session_dep == "Dispatch")
            {
                Map map = new Map();
                map.Show();
            }
        }

        private void checkCivs_CheckedChanged(object sender, EventArgs e)
        {
            if (!c_deps.Contains("4")) checkCivs.Checked = false;
            else ShowCiv();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            CivDataWindow(isCivWindowOpen, 0);
        }

        public void CivDataWindow(int state, int type = 0)
        {
            if(state == 0)
            {
                CivData.Visible = true;
                isCivWindowOpen = 1;
                DataAndCharLabel.Text = "Данные";

                if(type == 1)
                {
                    CharacterCreatePanel.Visible = true;
                }

                if (type == 2)
                {
                    WeaponCreatePanel.Visible = true;

                    Random random = new Random();
                    string randomDigits = "";

                    for (int i = 0; i < 7; i++)
                    {
                        randomDigits += random.Next(0, 10); // Генерируем случайное число от 0 до 9 и добавляем его к строке
                    }

                    WepSerial.Text = randomDigits;
                }

                if (type == 3)
                {
                    VehCreatePanel.Visible = true;
                }
            }
            else
            {
                CivData.Visible = false;
                isCivWindowOpen = 0;
                DataAndCharLabel.Text = $"Персонажи {charactersCount.ToString()}/4";

                CharacterCreatePanel.Visible = false;
                WeaponCreatePanel.Visible = false;
                VehCreatePanel.Visible = false;
            }
        }

        private void CharacterCreateButton_Click(object sender, EventArgs e)
        {
            if (charactersCount >= 4) return;
            CivDataWindow(isCivWindowOpen, 1);
        }

        private void PanelCiv_Click(object sender, EventArgs e)
        {
            PanelCiv.Focus();
        }

        private void CreateCharacter_Click(object sender, EventArgs e)
        {
            bool isAnyRadioButtonCheckedVeh = false;
            foreach (RadioButton rdo in VehLicTypeSelect.Controls.OfType<RadioButton>())
            {
                if (rdo.Checked)
                {
                    isAnyRadioButtonCheckedVeh = true;
                    break;
                }
            }
            if (!isAnyRadioButtonCheckedVeh)
            {
                return;
            }
            bool isAnyRadioButtonCheckedSex = false;
            foreach (RadioButton rdo in SexSelect.Controls.OfType<RadioButton>())
            {
                if (rdo.Checked)
                {
                    isAnyRadioButtonCheckedSex = true;
                    break;
                }
            }
            if (!isAnyRadioButtonCheckedSex)
            {
                return;
            }
            bool isAnyRadioButtonCheckedWep = false;
            foreach (RadioButton rdo in WepLicSelect.Controls.OfType<RadioButton>())
            {
                if (rdo.Checked)
                {
                    isAnyRadioButtonCheckedWep = true;
                    break;
                }
            }
            if (!isAnyRadioButtonCheckedWep)
            {
                return;
            }
            if (CharacterNameEnter.Text.IsNullOrEmpty() || CharacterBirthDD.Text.IsNullOrEmpty() || CharacterBirthMM.Text.IsNullOrEmpty() ||
                CharacterBirthYYYY.Text.IsNullOrEmpty() || CharacterMed.Text.IsNullOrEmpty() || CharacterWorkEnter.Text.IsNullOrEmpty()) return;
            int i;
            int k;
            int g;
            bool DDsuccess = int.TryParse(CharacterBirthDD.Text, out i);
            bool MMsuccess = int.TryParse(CharacterBirthMM.Text, out k);
            bool YYYYsuccess = int.TryParse(CharacterBirthYYYY.Text, out g);

            int veh_lic_type = 0;
            int wep_lic = 0;
            int sex = 0;

            if (defaultvehlic.Checked) veh_lic_type = 1;
            if (shofervehlic.Checked) veh_lic_type = 2;
            if (motovehlic.Checked) veh_lic_type = 3;
            if (cdlvehlic.Checked) veh_lic_type = 4;
            if (aviavehlic.Checked) veh_lic_type = 5;
            if (allvehlic.Checked) veh_lic_type = 6;
            if (timedvehlic.Checked) veh_lic_type = 7;
            if (fakevehlic.Checked) veh_lic_type = 8;
            if (novehlic.Checked) veh_lic_type = 0;

            if (yesweplic.Checked) wep_lic = 1;
            if (timedweplic.Checked) wep_lic = 2;
            if (fakeweplic.Checked) wep_lic = 3;
            if (noweplic.Checked) wep_lic = 0;

            if (SexMale.Checked) sex = 0;
            if (SexFemale.Checked) sex = 1;
            if (sexNonbinary.Checked) sex = 2;

            if (!DDsuccess) return;
            if (!MMsuccess) return;
            if (!YYYYsuccess) return;
            CivDataWindow(isCivWindowOpen);
            ws.Send($"CREATE_CIV:{CharacterNameEnter.Text}:{CharacterBirthDD.Text}:{CharacterBirthMM.Text}:{CharacterBirthYYYY.Text}:{CharacterMed.Text}:{veh_lic_type}:{wep_lic}:{CharacterWorkEnter.Text}:{sex}");
        }

        private void WepCreate_Click(object sender, EventArgs e) 
        {
            if (WepCharName.Text.IsNullOrEmpty() || WepCharDD.Text.IsNullOrEmpty() || WepCharMM.Text.IsNullOrEmpty() ||
                WepModel.Text.IsNullOrEmpty() || WepStateCreate.Text.IsNullOrEmpty() || WepSerial.Text.IsNullOrEmpty() ||
                WepDateDD.Text.IsNullOrEmpty() || WepDateMM.Text.IsNullOrEmpty() || WepDateYYYY.Text.IsNullOrEmpty()) return;

            int i;
            int k;
            int g;
            bool DDsuccess = int.TryParse(WepCharDD.Text, out i);
            bool MMsuccess = int.TryParse(WepCharMM.Text, out k);
            bool YYYYsuccess = int.TryParse(WepCharYYYY.Text, out g);

            int a;
            int b;
            int c;
            bool DDsuccess1 = int.TryParse(WepDateDD.Text, out a);
            bool MMsuccess2 = int.TryParse(WepDateMM.Text, out b);
            bool YYYYsuccess3 = int.TryParse(WepDateYYYY.Text, out c);

            if (!DDsuccess) return;
            if (!MMsuccess) return;
            if (!YYYYsuccess) return;

            if (!DDsuccess1) return;
            if (!MMsuccess2) return;
            if (!YYYYsuccess3) return;

            CivDataWindow(isCivWindowOpen);
            ws.Send($"CREATE_WEP:{WepSerial.Text}:{WepModel.Text}:{WepCharName.Text + " " + WepCharDD.Text + "." + WepCharMM.Text + "." + WepCharYYYY.Text}:{WepDateDD.Text + "." + WepDateMM.Text + "." + WepDateYYYY.Text}:{WepStateCreate.Text}");
        }

        private void WeaponCreateButton_Click(object sender, EventArgs e)
        {
            if (weaponsCount >= 4) return;
            CivDataWindow(isCivWindowOpen, 2);
        }

        private void VehicleCreateButton_Click(object sender, EventArgs e)
        {
            if (vehiclesCount >= 4) return;
            CivDataWindow(isCivWindowOpen, 3);
        }

        private void VehCreate_Click(object sender, EventArgs e)
        {
            if (VehCharName.Text.IsNullOrEmpty() || VehCharDD.Text.IsNullOrEmpty() || VehCharMM.Text.IsNullOrEmpty() ||
                VehModel.Text.IsNullOrEmpty() || VehCreateState.Text.IsNullOrEmpty() || VehColor.Text.IsNullOrEmpty() || VehPlate.Text.IsNullOrEmpty() ||
                VehDateDD.Text.IsNullOrEmpty() || VehDateMM.Text.IsNullOrEmpty() || VehDateYYYY.Text.IsNullOrEmpty()) return;

            int i;
            int k;
            int g;
            bool DDsuccess = int.TryParse(VehCharDD.Text, out i);
            bool MMsuccess = int.TryParse(VehCharMM.Text, out k);
            bool YYYYsuccess = int.TryParse(VehCharYYYY.Text, out g);

            int a;
            int b;
            int c;
            bool DDsuccess1 = int.TryParse(VehDateDD.Text, out a);
            bool MMsuccess2 = int.TryParse(VehDateMM.Text, out b);
            bool YYYYsuccess3 = int.TryParse(VehDateYYYY.Text, out c);

            if (!DDsuccess) return;
            if (!MMsuccess) return;
            if (!YYYYsuccess) return;

            if (!DDsuccess1) return;
            if (!MMsuccess2) return;
            if (!YYYYsuccess3) return;

            CivDataWindow(isCivWindowOpen);
            ws.Send($"CREATE_VEH:{VehPlate.Text}:{VehCharName.Text + " " + VehCharDD.Text + "." + VehCharMM.Text + "." + VehCharYYYY.Text}:{VehColor.Text}:{VehModel.Text}:{VehCreateState.Text}:{VehDateDD.Text + "." + VehDateMM.Text + "." + VehDateYYYY.Text}");
        }

        private void CivCallSend_Click(object sender, EventArgs e)
        {
            if (CivCallAddress1.Text.IsNullOrEmpty() || CivCallAddress2.Text.IsNullOrEmpty() ||
                CivCallPostal1.Text.IsNullOrEmpty() || CivCallPostal2.Text.IsNullOrEmpty() || CivCallShortdesc.Text.IsNullOrEmpty() || CivCallLongdesc.Text.IsNullOrEmpty()) return;
            ws.Send($"CREATE_CALL:{CivCallAddress1.Text.Replace(':', ';') + " " + CivCallPostal1.Text.Replace(':', ';')}:{CivCallAddress2.Text.Replace(':', ';') + " " + CivCallPostal2.Text.Replace(':', ';')}:{CivCallShortdesc.Text.Replace(':', ';')}:{CivCallLongdesc.Text.Replace(':', ';')}");
        }

        private void AlphabetButton_Click(object sender, EventArgs e)
        {
            if (c_session_dep != "Dispatch") return;

            DataWindow(4);
        }

        private void dispcallcreate_Click(object sender, EventArgs e)
        {
            if (DispCallAddress1.Text.IsNullOrEmpty() || DispCallAddress2.Text.IsNullOrEmpty() ||
                DispCallPostal1.Text.IsNullOrEmpty() || DispCallPostal2.Text.IsNullOrEmpty() || DispCallShortdesc.Text.IsNullOrEmpty() || 
                DispCallLongdesc.Text.IsNullOrEmpty() || DispCallType.Text.IsNullOrEmpty()) return;
            ws.Send($"CREATE_CALL:{DispCallAddress1.Text.Replace(':', ';') + " " + DispCallPostal1.Text.Replace(':', ';')}:{DispCallAddress2.Text.Replace(':', ';') + " " + DispCallPostal2.Text.Replace(':', ';')}:{DispCallShortdesc.Text.Replace(':', ';')}:{DispCallLongdesc.Text.Replace(':', ';')}:{DispCallType.Text.Replace(':', ';')}");
        }

        private void panel911_1_x_Click(object sender, EventArgs e)
        {
            if (c_session_dep != "Dispatch") if (isDispActive == 1) return;
            ws.Send("DELETE_CALL:0");
        }

        private void panel911_2_x_Click(object sender, EventArgs e)
        {
            if (c_session_dep != "Dispatch") if (isDispActive == 1) return;
            ws.Send("DELETE_CALL:1");
        }

        private void panel911_3_x_Click(object sender, EventArgs e)
        {
            if (c_session_dep != "Dispatch") if (isDispActive == 1) return;
            ws.Send("DELETE_CALL:2");
        }

        private void panel911_4_x_Click(object sender, EventArgs e)
        {
            if (c_session_dep != "Dispatch") if (isDispActive == 1) return;
            ws.Send("DELETE_CALL:3");
        }

        private void panel911_5_x_Click(object sender, EventArgs e)
        {
            if (c_session_dep != "Dispatch") if (isDispActive == 1) return;
            ws.Send("DELETE_CALL:4");
        }

        private void panel911_6_x_Click(object sender, EventArgs e)
        {
            if (c_session_dep != "Dispatch") if (isDispActive == 1) return;
            ws.Send("DELETE_CALL:5");
        }

        private void panel911_7_x_Click(object sender, EventArgs e)
        {
            if (c_session_dep != "Dispatch") if (isDispActive == 1) return;
            ws.Send("DELETE_CALL:6");
        }

        private void panel911_8_x_Click(object sender, EventArgs e)
        {
            if (c_session_dep != "Dispatch") if(isDispActive == 1) return;
            ws.Send("DELETE_CALL:7");
        }
    }

    public class CallData
    {
        public string type { get; set; }
        public string shortdesc { get; set; }
        public string longdesc { get; set; }
        public string units { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string num { get; set; }
        public string active { get; set; }
        public string time { get; set; }
    }

    public class Character
    {
        public string name { get; set; } 
        public int dd { get; set; }
        public int mm { get; set; }
        public int yyyy { get; set; }
        public string medicine { get; set; }
        public int veh_lic_type { get; set; }
        public int wep_lic { get; set; }
        public string work { get; set; }
        public int sex { get; set; } 
    }

    public class Weapon
    {
        public int serial { get; set; }
        public string model { get; set; }
        public string owner { get; set; }
        public string dateofcreate { get; set; }
        public string stateofcreate { get; set; }
    }

    public class Vehicle
    {
        public string plate { get; set; }
        public string model { get; set; }
        public string owner { get; set; }
        public string color { get; set; }
        public string dateofcreate { get; set; }
        public string stateofcreate { get; set; }
        public int isInBolo { get; set; }
        public string bolo_desc { get; set; }
        public string bolocreatedate { get; set; }
    }
}
