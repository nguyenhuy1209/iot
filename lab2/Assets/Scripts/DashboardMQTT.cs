using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dashboard
{
    public enum Device { LED, PUMP }

    public class StatusData
    {
        public string temperature { get; set; }
        public string humidity { get; set; }
    }

    public class ControlData
    {
        public string device { get; set; }
        public string status { get; set; }

    }

    public class DashboardMQTT : M2MqttUnityClient
    {
        public DashboardUI dashboard_manager;
        private string student_id = "1852405";
        private List<string> topics = new List<string>();

        [SerializeField]
        private StatusData status_data;

	    public void ConnectServer()
        {
            this.brokerAddress = dashboard_manager.broker_url.text;
            this.brokerPort = 1883;
            this.mqttUserName = dashboard_manager.username.text;
            this.mqttPassword = dashboard_manager.password.text;

            this.Connect();
        }

        public void CloseConnection() {
            Disconnect();
            dashboard_manager.SwitchLayer();
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            dashboard_manager.SwitchLayer();
        }

        protected override void SubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    Debug.Log(string.Format("Subscribed topic: {0}", topic));
                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                    Debug.Log(string.Format("Unscribed topic: {0}", topic));
                }
            }

        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        protected override void Start()
        {
            topics.Add(string.Format("/bkiot/{0}/status", student_id));
            topics.Add(string.Format("/bkiot/{0}/led", student_id));
            topics.Add(string.Format("/bkiot/{0}/pump", student_id));

            base.Start();
        }
        
        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);

            Debug.Log("Received: " + msg);
            if (topic == topics[0])
                ProcessMessageStatus(msg);
        }

        private void ProcessMessageStatus(string msg)
        {
            status_data = JsonConvert.DeserializeObject<StatusData>(msg);
            status_data.temperature = Math.Round(float.Parse(status_data.temperature), 2).ToString();
            status_data.humidity = Math.Round(float.Parse(status_data.humidity), 2).ToString();
            dashboard_manager.UpdateStatusData(status_data);
        }

        public void PublishLed()
        {
            ControlData data = dashboard_manager.GetLedControlData();
            string msg_control = JsonConvert.SerializeObject(data);
            client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg_control), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

        public void PublishPump()
        {
            ControlData data = dashboard_manager.GetPumpControlData();
            string msg_control = JsonConvert.SerializeObject(data);
            client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_control), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }
    }
}
