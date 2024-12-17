using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTT_SUB_Console
{
    class Program
    {
        public static MqttClient client;
        public static BusinessLayer bl;
        public static int srno = 0;
        public static DateTime dt = new DateTime();

        private static Timer _timer = null;
        static void Main(string[] args)
        {
            //int height = 5;

            //for (int i = 0; i < height; i++)
            //{
            //    // Print leading spaces
            //    for (int j = height - i - 1; j > 0; j--)
            //    {
            //        Console.Write(" ");
            //    }

            //    // Print stars
            //    for (int k = 0; k < (2 * i + 1); k++)
            //    {
            //        Console.Write("*");
            //    }

            //    // Move to the next line
            //    Console.WriteLine();
            //}
           // Console.ReadLine();
            #region ReadTextFile

            string directoryPath = @"E:\"; // Replace with your directory path

            // Get all .txt files in the directory
            string[] filePaths = Directory.GetFiles(directoryPath, "LOG.txt");
            bl = new BusinessLayer();
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            for (int i = 0; i < filePaths.Length; i++)
            {
                string jsonFilePath = filePaths[i];
                string content = File.ReadAllText(jsonFilePath);

                string[] lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                Parallel.ForEach(lines, item =>
                // foreach (var item in lines)
                {

                    RootObject Obj = json_serializer.Deserialize<RootObject>(item);
                    string[] str = Obj.Name.Split('_');

                    if (str[2] == "WeldLog")
                    {
                        try
                        {
                            bl.Insert_Weldlog(Obj.Name, Obj.WeldTimer, Obj.Message.WeldLog.partIdentString, Obj.Message.WeldLog.iActual2, Obj.Message.WeldLog.progNo, Obj.Message.WeldLog.iDemand2, Obj.Message.WeldLog.weldTimeActualValue, Obj.Message.WeldLog.resistanceActualValue, Obj.Message.WeldLog.CurrentCurve[0], Obj.Message.WeldLog.VoltageCurve[0], Obj.Message.WeldLog.ForceCurve[0], Obj.TimeStamp);

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    // }
                });

                //foreach (var item in lines)
                //{
                //    JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                //    RootObject Obj = json_serializer.Deserialize<RootObject>(item);
                //    string[] str = Obj.Name.Split('_');

                //    if (str[2] == "WeldLog")
                //    {
                //        try
                //        {
                //            bl.Insert_Weldlog(Obj.Name, Obj.WeldTimer, Obj.Message.WeldLog.partIdentString, Obj.Message.WeldLog.iActual2, Obj.Message.WeldLog.progNo, Obj.Message.WeldLog.iDemand2, Obj.Message.WeldLog.weldTimeActualValue, Obj.Message.WeldLog.resistanceActualValue, Obj.Message.WeldLog.CurrentCurve[0], Obj.Message.WeldLog.VoltageCurve[0], Obj.Message.WeldLog.ForceCurve[0], Obj.TimeStamp);

                //        }
                //        catch (Exception ex)
                //        {

                //        }
                //    }
                //}


                //using (StreamReader file = new StreamReader(jsonFilePath))
                //{
                //    string jsonString;
                //    while ((jsonString = file.ReadLine()) != null)
                //    {
                //        //dynamic jsonObject = JsonConvert.DeserializeObject(jsonString);

                //        JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                //        RootObject Obj = json_serializer.Deserialize<RootObject>(jsonString);
                //        string[] str = Obj.Name.Split('_');

                //        if (str[2] == "WeldLog")
                //        {
                //            try
                //            {
                //                bl.Insert_Weldlog(Obj.Name, Obj.WeldTimer, Obj.Message.WeldLog.partIdentString, Obj.Message.WeldLog.iActual2, Obj.Message.WeldLog.progNo, Obj.Message.WeldLog.iDemand2, Obj.Message.WeldLog.weldTimeActualValue, Obj.Message.WeldLog.resistanceActualValue, Obj.Message.WeldLog.CurrentCurve[0], Obj.Message.WeldLog.VoltageCurve[0], Obj.Message.WeldLog.ForceCurve[0], Obj.TimeStamp);

                //            }
                //            catch (Exception ex)
                //            {

                //            }
                //        }
                //        else
                //        {
                //            bl.Insert_ProgParameter(Obj.Name, Obj.WeldTimer, Obj.Message.parameterValuesTable.subIndex, Obj.Message.parameterValuesTable.SP_HOLD_MS, Obj.Message.parameterValuesTable.SP_SQZ_MS, Obj.Message.parameterValuesTable.SP_IMPULS_CNT, Obj.TimeStamp);

                //        }
                //    }
                //}

                Console.WriteLine("File Injest Successfully.");
                Console.ReadLine();

            }

            #endregion


            //Bypass();

            try
            {
                //  bl = new BusinessLayer();

                // Set up MQTT client with specific broker address and port
                //string brokerAddress = "127.0.0.1"; // Replace with the MQTT broker address
                string brokerAddress = ConfigurationManager.AppSettings["IP"].ToString();  // Replace with the MQTT broker address
                int brokerPort = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString());  // Replace with the MQTT broker port number
                client = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);

                // client.ConnectionClosed += Client_ConnectionClosed;


                // Set up callback for received messages
                client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

                // Connect to the MQTT broker
                string clientId = Guid.NewGuid().ToString();
                client.Connect(clientId);
                //  bl.Insert_ErrorLog("Main", "Connection Successfully", "Connection Successfully");


                // Subscribe to a topic
                string topic = ConfigurationManager.AppSettings["Topic1"].ToString(); // Replace with the desired MQTT topic
                string topic2 = ConfigurationManager.AppSettings["Topic2"].ToString();// Replace with the desired MQTT topic
                client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                client.Subscribe(new string[] { topic2 }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                // Keep the application running
                Console.WriteLine($"Subscribed to topic: {topic}");
                Console.WriteLine("Press Q to exit");
                _timer = new Timer(TimerCallback, null, 0, 60000 * 5);
                while (Console.ReadKey().Key != ConsoleKey.Q) { }

                // Disconnect from the MQTT broker
                client.Disconnect();
            }
            catch (Exception ex)
            {
                //  bl.Insert_ErrorLog("Main", ex.Message + " " + ex.InnerException.Message, ex.StackTrace);
            }
        }

        private static void Client_ConnectionClosed(object sender, EventArgs e)
        {
            if (!client.IsConnected)
            {
                // bl.Insert_ErrorLog("Client_ConnectionClosed", "MQTT server Disconnected", "MQTT server Disconnected");
            }
        }

        private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (client.IsConnected)
            {
                try
                {
                    // Handle received message
                    string message = Encoding.UTF8.GetString(e.Message);


                    #region Database
                    JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                    RootObject Obj = json_serializer.Deserialize<RootObject>(message);

                    if (e.Topic == ConfigurationManager.AppSettings["Topic1"].ToString())
                    {
                        if (Obj.Message.WeldLog != null)
                        {
                            try
                            {
                                // Console.WriteLine("Srno:" + (srno) + "" + message);
                                FileWriter(message);
                                //bl.Insert_Weldlog(Obj.Name, Obj.WeldTimer, Obj.Message.WeldLog.partIdentString, Obj.Message.WeldLog.iActual2, Obj.Message.WeldLog.progNo, Obj.Message.WeldLog.iDemand2, Obj.Message.WeldLog.weldTimeActualValue, Obj.Message.WeldLog.resistanceActualValue, Obj.Message.WeldLog.CurrentCurve[0], Obj.Message.WeldLog.VoltageCurve[0], Obj.Message.WeldLog.ForceCurve[0], Obj.TimeStamp);

                            }
                            catch (Exception ex)
                            {
                                FileWriter(ex.ToString());
                                // bl.Insert_ErrorLog("Client_MqttMsgPublishReceived("+ e.Topic + ")", ex.Message, ex.StackTrace);

                            }
                        }
                    }

                    //if (e.Topic == ConfigurationManager.AppSettings["Topic2"].ToString())
                    //{
                    //    if (Obj.Message.parameterValuesTable != null)
                    //    {
                    //        try
                    //        {
                    //            Console.WriteLine("Srno:" + (srno) + "" + message);
                    //            FileWriter(message);
                    //            //bl.Insert_ProgParameter(Obj.Name, Obj.WeldTimer, Obj.Message.parameterValuesTable.subIndex, Obj.Message.parameterValuesTable.SP_1_COOL_MS, Obj.Message.parameterValuesTable.SP_1_KA, Obj.Message.parameterValuesTable.SP_1_WELD_MS, Obj.Message.parameterValuesTable.SP_2_COOL_MS, Obj.Message.parameterValuesTable.SP_2_KA, Obj.Message.parameterValuesTable.SP_2_WELD_MS, Obj.Message.parameterValuesTable.SP_3_WELD_MS, Obj.Message.parameterValuesTable.SP_3_KA, Obj.Message.parameterValuesTable.SP_HOLD_MS, Obj.Message.parameterValuesTable.SP_SQZ_MS, Obj.Message.parameterValuesTable.SP_IMPULS_CNT, Obj.TimeStamp);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            //bl.Insert_ErrorLog("Client_MqttMsgPublishReceived(" + e.Topic + ")", ex.Message, ex.StackTrace);
                    //        }
                    //    }
                    //}
                    #endregion
                    srno++;
                }
                catch (Exception ex)
                {
                    // bl.Insert_ErrorLog("Client_MqttMsgPublishReceived", ex.Message, ex.StackTrace);

                }
            }


        }

        static void FileWriter(string msg)
        {
            //dt = DateTime.Now;
            //string ss = dt.Year + "" + dt.Month + "" + dt.Day + "_" + dt.TimeOfDay.Seconds;

            ////string currentDirectory = System.IO.Directory.GetCurrentDirectory().ToString();
            //string currentDirectory = ConfigurationManager.AppSettings["LogPath"].ToString();

            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(currentDirectory + @"\" + "LOG_" + ss + ".txt", true))
            //{
            //    file.WriteLine(msg);
            //}


            string currentDirectory = ConfigurationManager.AppSettings["LogPath"].ToString();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(currentDirectory + @"\" + "LOG.txt", true))
            {
                file.WriteLine(msg);
            }
        }


        private static void TimerCallback(Object o)
        {
            if (!client.IsConnected)
            {
                Main(null);
            }
        }




        #region Properties
        public class WeldLog
        {
            public string dateTime { get; set; }
            public double iActual1 { get; set; }
            public double iActual2 { get; set; }
            public double iActual3 { get; set; }
            public string partIdentString { get; set; }
            public double pha1 { get; set; }
            public double pha2 { get; set; }
            public double pha3 { get; set; }
            public int progNo { get; set; }
            public string spotName { get; set; }
            public string timerName { get; set; }
            public double iDemand1 { get; set; }
            public double iDemand2 { get; set; }
            public double iDemand3 { get; set; }
            public int resistanceActualValue { get; set; }
            public int weldTimeActualValue { get; set; }
            public List<int> CurrentCurve { get; set; }
            public List<int> VoltageCurve { get; set; }
            public List<int> ForceCurve { get; set; }
        }
        public class ParameterValuesTable
        {
            public int subIndex { get; set; }
            public double SP_1_COOL_MS { get; set; }
            public double SP_1_KA { get; set; }
            public double SP_1_WELD_MS { get; set; }
            public double SP_2_COOL_MS { get; set; }
            public double SP_2_KA { get; set; }
            public double SP_2_WELD_MS { get; set; }
            public double SP_HOLD_MS { get; set; }
            public double SP_SQZ_MS { get; set; }
            public double SP_IMPULS_CNT { get; set; }
            public double SP_3_WELD_MS { get; set; }
            public double SP_3_KA { get; set; }
        }
        public class Message
        {
            public WeldLog WeldLog { get; set; }
            public ParameterValuesTable parameterValuesTable { get; set; }
        }
        public class RootObject
        {
            public string Name { get; set; }
            public string WeldTimer { get; set; }
            public DateTime TimeStamp { get; set; }
            public string OutputFormat { get; set; }
            public Message Message { get; set; }
        }
        #endregion

    }


}
