using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
namespace UsbSerialImu
{
    /// <summary>
    /// IMU 데이터 구조체
    /// </summary>
    [System.Serializable]
    public struct IMUData
    {
        public Vector3 acceleration;  // 가속도 (m/s²)
        public Vector3 gyroscope;     // 자이로스코프 (rad/s)
        public Vector3 magnetometer;  // 지자기 (μT)
        public Quaternion orientation; // 방향 (쿼터니언)
        public float timestamp;       // 타임스탬프
    }

    /// <summary>
    /// USB Serial IMU Manager
    /// Unity에서 Android USB Serial 장치를 통해 IMU 데이터를 받아오는 클래스
    /// </summary>
    public class UsbSerialImuManager : MonoBehaviour
    {
        [Header("Connection Settings")]
        [SerializeField] private int baudRate = 115200;
        [SerializeField] private int dataBits = 8;
        [SerializeField] private int stopBits = 1;
        [SerializeField] private int parity = 0; // 0: None, 1: Odd, 2: Even

        [Header("Data Settings")]
        [SerializeField] private bool autoConnect = true;
        [SerializeField] private float reconnectInterval = 3f;

        // 이벤트
        public event Action<IMUData> OnIMUDataReceived;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnError;

        // 상태
        private bool isConnected = false;
        private AndroidJavaObject usbSerialPlugin;
        private IMUData currentData;
        private float reconnectTimer = 0f;

        // Getter
        public bool IsConnected => isConnected;
        public IMUData CurrentData => currentData;

        private void Awake()
        {
            InitializePlugin();
        }

        private void Start()
        {
            if (autoConnect)
            {
                Connect();
            }
        }

        private void Update()
        {
            if (!isConnected && autoConnect)
            {
                reconnectTimer += Time.deltaTime;
                if (reconnectTimer >= reconnectInterval)
                {
                    reconnectTimer = 0f;
                    Connect();
                }
            }

            if (isConnected)
            {
                ReadData();
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Disconnect();
            }
            else if (autoConnect)
            {
                Connect();
            }
        }

        /// <summary>
        /// Android 플러그인 초기화
        /// </summary>
        private void InitializePlugin()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                usbSerialPlugin = new AndroidJavaObject("com.unity.usbserial.UsbSerialPlugin");
                Debug.Log("USB Serial Plugin initialized");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize USB Serial Plugin: {e.Message}");
                OnError?.Invoke($"Plugin initialization failed: {e.Message}");
            }
#else
            Debug.LogWarning("USB Serial is only available on Android devices");
#endif
        }

        /// <summary>
        /// USB Serial 장치에 연결
        /// </summary>
        public void Connect()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (usbSerialPlugin == null)
            {
                InitializePlugin();
                if (usbSerialPlugin == null) return;
            }

            try
            {
                bool result = usbSerialPlugin.Call<bool>("connect", baudRate, dataBits, stopBits, parity);
                
                if (result)
                {
                    isConnected = true;
                    Debug.Log("USB Serial IMU connected successfully");
                    OnConnected?.Invoke();
                }
                else
                {
                    isConnected = false;
                    Debug.LogWarning("Failed to connect to USB Serial IMU");
                    OnError?.Invoke("Connection failed");
                }
            }
            catch (Exception e)
            {
                isConnected = false;
                Debug.LogError($"Connection error: {e.Message}");
                OnError?.Invoke($"Connection error: {e.Message}");
            }
#else
            Debug.LogWarning("USB Serial connection is only available on Android devices");
#endif
        }

        /// <summary>
        /// USB Serial 연결 해제
        /// </summary>
        public void Disconnect()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (usbSerialPlugin != null && isConnected)
            {
                try
                {
                    usbSerialPlugin.Call("disconnect");
                    isConnected = false;
                    Debug.Log("USB Serial IMU disconnected");
                    OnDisconnected?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Disconnection error: {e.Message}");
                }
            }
#endif
        }

        /// <summary>
        /// IMU 데이터 읽기
        /// </summary>
        private void ReadData()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (usbSerialPlugin == null || !isConnected) return;

            try
            {
                string data = usbSerialPlugin.Call<string>("readData");
                
                if (!string.IsNullOrEmpty(data))
                {
                    ParseIMUData(data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Read data error: {e.Message}");
                isConnected = false;
                OnDisconnected?.Invoke();
            }
#endif
        }

        /// <summary>
        /// IMU 데이터 파싱
        /// 예상 포맷: "ACC:x,y,z;GYRO:x,y,z;MAG:x,y,z;TIME:timestamp"
        /// </summary>
        private void ParseIMUData(string raw)
        {
            try
            {
                // 예: "W: 0.12 X: 0.23 Y: -0.96 Z: 0.06"
                float w = 0, x = 0, y = 0, z = 0;

                string[] tokens = raw.Split(' ');

                for (int i = 0; i < tokens.Length - 1; i++)
                {
                    if (tokens[i] == "W:")
                        float.TryParse(tokens[i + 1], out w);

                    if (tokens[i] == "X:")
                        float.TryParse(tokens[i + 1], out x);

                    if (tokens[i] == "Y:")
                        float.TryParse(tokens[i + 1], out y);

                    if (tokens[i] == "Z:")
                        float.TryParse(tokens[i + 1], out z);
                }

                IMUData data = new IMUData();
                data.orientation = new Quaternion(x, y, z, w);
                data.timestamp = Time.time;

                currentData = data;
                OnIMUDataReceived?.Invoke(data);

                Debug.Log($"Parsed Quaternion => {data.orientation}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Parse error: {e.Message} Raw: {raw}");
            }
        }



        /// <summary>
        /// Vector3 파싱 (x,y,z 형식)
        /// </summary>
        private Vector3 ParseVector3(string value)
        {
            string[] components = value.Split(',');
            if (components.Length != 3)
            {
                Debug.LogWarning($"Invalid Vector3 format: {value}");
                return Vector3.zero;
            }

            float x = 0, y = 0, z = 0;
            float.TryParse(components[0], out x);
            float.TryParse(components[1], out y);
            float.TryParse(components[2], out z);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Quaternion 파싱 (w,x,y,z 형식)
        /// </summary>
        private Quaternion ParseQuaternion(string value)
        {
            string[] components = value.Split(',');
            if (components.Length != 4)
            {
                Debug.LogWarning($"Invalid Quaternion format: {value}");
                return Quaternion.identity;
            }

            float w = 0, x = 0, y = 0, z = 0;
            float.TryParse(components[0], out w);
            float.TryParse(components[1], out x);
            float.TryParse(components[2], out y);
            float.TryParse(components[3], out z);

            return new Quaternion(x, y, z, w);
        }

        /// <summary>
        /// 명령 전송 (캘리브레이션 등)
        /// </summary>
        public void SendCommand(string command)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (usbSerialPlugin != null && isConnected)
            {
                try
                {
                    usbSerialPlugin.Call("sendData", command);
                    Debug.Log($"Command sent: {command}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Send command error: {e.Message}");
                }
            }
#endif
        }

        /// <summary>
        /// 연결된 USB 장치 목록 가져오기
        /// </summary>
        public List<string> GetConnectedDevices()
        {
            List<string> devices = new List<string>();

#if UNITY_ANDROID && !UNITY_EDITOR
            if (usbSerialPlugin != null)
            {
                try
                {
                    AndroidJavaObject deviceArray = usbSerialPlugin.Call<AndroidJavaObject>("getDeviceList");
                    if (deviceArray != null)
                    {
                        int length = deviceArray.Call<int>("length");
                        for (int i = 0; i < length; i++)
                        {
                            string device = deviceArray.Call<string>("get", i);
                            devices.Add(device);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Get devices error: {e.Message}");
                }
            }
#endif
            return devices;
        }
    }
}