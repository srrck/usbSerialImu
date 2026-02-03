package com.unity.usbserial;

import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.hardware.usb.UsbDevice;
import android.hardware.usb.UsbDeviceConnection;
import android.hardware.usb.UsbManager;
import android.util.Log;

import com.hoho.android.usbserial.driver.UsbSerialDriver;
import com.hoho.android.usbserial.driver.UsbSerialPort;
import com.hoho.android.usbserial.driver.UsbSerialProber;
import com.hoho.android.usbserial.util.SerialInputOutputManager;

import com.unity3d.player.UnityPlayer;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.Executors;

/**
 * Unity용 USB Serial 플러그인
 * IMU 센서와 USB Serial 통신을 담당
 */
public class UsbSerialPlugin implements SerialInputOutputManager.Listener {
    
    private static final String TAG = "UsbSerialPlugin";
    private static final String ACTION_USB_PERMISSION = "com.unity.usbserial.USB_PERMISSION";
    
    private Context context;
    private UsbManager usbManager;
    private UsbSerialPort serialPort;
    private SerialInputOutputManager ioManager;
    private UsbDeviceConnection connection;
    
    private boolean connected = false;
    private StringBuilder dataBuffer = new StringBuilder();
    
    // Unity에서 호출할 게임 오브젝트 이름
    private static final String UNITY_CALLBACK_OBJECT = "UsbSerialImuManager";
    
    public UsbSerialPlugin() {
        this.context = UnityPlayer.currentActivity;
        this.usbManager = (UsbManager) context.getSystemService(Context.USB_SERVICE);
        
        // USB 권한 브로드캐스트 리시버 등록
        IntentFilter filter = new IntentFilter(ACTION_USB_PERMISSION);

        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.S) {
            context.registerReceiver(usbPermissionReceiver, filter, Context.RECEIVER_NOT_EXPORTED);
        } else {
            context.registerReceiver(usbPermissionReceiver, filter);
        }
        Log.d(TAG, "UsbSerialPlugin initialized");
    }
    
    /**
     * USB Serial 장치에 연결
     * @param baudRate 통신 속도 (예: 115200)
     * @param dataBits 데이터 비트 (예: 8)
     * @param stopBits 정지 비트 (예: 1)
     * @param parity 패리티 (0: None, 1: Odd, 2: Even)
     * @return 연결 성공 여부
     */
    public boolean connect(int baudRate, int dataBits, int stopBits, int parity) {
        try {
            // 연결된 USB 장치 찾기
            List<UsbSerialDriver> availableDrivers = UsbSerialProber.getDefaultProber().findAllDrivers(usbManager);
            
            if (availableDrivers.isEmpty()) {
                Log.e(TAG, "No USB devices found");
                return false;
            }
            
            // 첫 번째 장치 선택
            UsbSerialDriver driver = availableDrivers.get(0);
            UsbDevice device = driver.getDevice();
            
            // USB 권한 확인
            if (!usbManager.hasPermission(device)) {
                PendingIntent permissionIntent = PendingIntent.getBroadcast(
                    context, 
                    0, 
                    new Intent(ACTION_USB_PERMISSION), 
                    PendingIntent.FLAG_IMMUTABLE
                );
                usbManager.requestPermission(device, permissionIntent);
                Log.d(TAG, "Requesting USB permission");
                return false;
            }
            
            // USB 연결 열기
            connection = usbManager.openDevice(device);
            if (connection == null) {
                Log.e(TAG, "Failed to open USB device");
                return false;
            }
            
            // Serial Port 설정
            serialPort = driver.getPorts().get(0);
            serialPort.open(connection);
            serialPort.setParameters(
                baudRate, 
                dataBits, 
                stopBits, 
                parity
            );
            
            // IO Manager 시작
            ioManager = new SerialInputOutputManager(serialPort, this);
            ioManager.start();
            
            connected = true;
            Log.d(TAG, "USB Serial connected successfully");
            return true;
            
        } catch (Exception e) {
            Log.e(TAG, "Connection error: " + e.getMessage(), e);
            disconnect();
            return false;
        }
    }
    
    /**
     * USB Serial 연결 해제
     */
    public void disconnect() {
        connected = false;
        
        if (ioManager != null) {
            ioManager.stop();
            ioManager = null;
        }
        
        if (serialPort != null) {
            try {
                serialPort.close();
            } catch (IOException e) {
                Log.e(TAG, "Error closing serial port", e);
            }
            serialPort = null;
        }
        
        if (connection != null) {
            connection.close();
            connection = null;
        }
        
        Log.d(TAG, "USB Serial disconnected");
    }
    
    /**
     * 데이터 읽기
     * @return 수신된 데이터 문자열
     */
    public String readData() {
        synchronized (dataBuffer) {
            if (dataBuffer.length() > 0) {
                String data = dataBuffer.toString();
                dataBuffer.setLength(0);
                return data;
            }
        }
        return "";
    }
    
    /**
     * 데이터 전송
     * @param data 전송할 데이터
     */
    public void sendData(String data) {
        if (serialPort != null && connected) {
            try {
                byte[] bytes = data.getBytes();
                serialPort.write(bytes, 1000);
                Log.d(TAG, "Data sent: " + data);
            } catch (IOException e) {
                Log.e(TAG, "Error sending data", e);
            }
        }
    }
    
    /**
     * 연결된 USB 장치 목록 가져오기
     * @return 장치 이름 배열
     */
    public String[] getDeviceList() {
        List<UsbSerialDriver> availableDrivers = UsbSerialProber.getDefaultProber().findAllDrivers(usbManager);
        List<String> deviceNames = new ArrayList<>();
        
        for (UsbSerialDriver driver : availableDrivers) {
            UsbDevice device = driver.getDevice();
            String deviceInfo = String.format(
                "%s (VID: %04X, PID: %04X)",
                device.getDeviceName(),
                device.getVendorId(),
                device.getProductId()
            );
            deviceNames.add(deviceInfo);
        }
        
        return deviceNames.toArray(new String[0]);
    }
    
    /**
     * 연결 상태 확인
     * @return 연결 여부
     */
    public boolean isConnected() {
        return connected;
    }
    
    // SerialInputOutputManager.Listener 구현
    @Override
    public void onNewData(byte[] data) {
        try {
            String receivedData = new String(data, "UTF-8");
            synchronized (dataBuffer) {
                dataBuffer.append(receivedData);
            }
            Log.d(TAG, "Data received: " + receivedData);
        } catch (Exception e) {
            Log.e(TAG, "Error processing received data", e);
        }
    }
    
    @Override
    public void onRunError(Exception e) {
        Log.e(TAG, "Serial IO error", e);
        disconnect();
    }
    
    // USB 권한 브로드캐스트 리시버
    private final BroadcastReceiver usbPermissionReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            if (ACTION_USB_PERMISSION.equals(action)) {
                synchronized (this) {
                    UsbDevice device = intent.getParcelableExtra(UsbManager.EXTRA_DEVICE);
                    
                    if (intent.getBooleanExtra(UsbManager.EXTRA_PERMISSION_GRANTED, false)) {
                        if (device != null) {
                            Log.d(TAG, "USB permission granted");
                            // 권한이 부여되면 자동으로 연결 시도
                        }
                    } else {
                        Log.e(TAG, "USB permission denied");
                    }
                }
            }
        }
    };
    
    /**
     * 리소스 정리
     */
    public void cleanup() {
        disconnect();
        try {
            context.unregisterReceiver(usbPermissionReceiver);
        } catch (Exception e) {
            Log.e(TAG, "Error unregistering receiver", e);
        }
    }
}
