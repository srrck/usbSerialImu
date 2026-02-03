# Unity USB Serial IMU - Android Library (AAR)

Unity에서 사용할 수 있는 Android USB Serial IMU 라이브러리 AAR 빌드 프로젝트입니다.

## 📋 목차

- [개요](#개요)
- [빠른 시작](#빠른-시작)
- [프로젝트 구조](#프로젝트-구조)
- [빌드 방법](#빌드-방법)
- [Unity 통합](#unity-통합)
- [문제 해결](#문제-해결)

## 🎯 개요

이 프로젝트는 Unity Android 앱에서 USB Serial 통신을 통해 IMU 센서 데이터를 받아오기 위한 Android Native 플러그인 라이브러리입니다.

### 주요 기능

- ✅ USB Serial 통신 (FTDI, CP210x, CH340 등)
- ✅ IMU 센서 데이터 수신
- ✅ 자동 USB 권한 관리
- ✅ 다양한 USB 장치 지원
- ✅ Unity 친화적인 AAR 패키지

## 🚀 빠른 시작

### 필수 요구사항

- **Android Studio**: Arctic Fox (2020.3.1) 이상
- **JDK**: 11 이상
- **Android SDK**: API Level 33
- **Gradle**: 8.0 이상 (자동 설치)

### 1. 프로젝트 열기

```bash
# Android Studio
File > Open > UsbSerialImu 폴더 선택
```

### 2. SDK 경로 설정

`local.properties` 파일 생성:

```properties
sdk.dir=/Users/YourName/Library/Android/sdk
```

또는 `local.properties.template` 복사 후 수정

### 3. AAR 빌드

**방법 A: Android Studio 사용**
```
Gradle 탭 > usbserialimu > Tasks > build > assembleRelease
```

**방법 B: 명령줄 사용**
```bash
# macOS/Linux
./build_aar.sh release

# Windows
build_aar.bat release
```

### 4. 결과 확인

빌드된 AAR 파일 위치:
```
output/usbserialimu-release.aar
```

## 📁 프로젝트 구조

```
UsbSerialImu/
├── build.gradle                                    # 프로젝트 빌드 설정
├── settings.gradle                                 # 프로젝트 설정
├── gradle.properties                               # Gradle 속성
├── build_aar.sh                                    # 빌드 스크립트 (Unix)
├── build_aar.bat                                   # 빌드 스크립트 (Windows)
├── AAR_BUILD_GUIDE.md                             # 상세 빌드 가이드
│
├── gradle/
│   └── wrapper/
│       └── gradle-wrapper.properties              # Gradle Wrapper 설정
│
└── usbserialimu/                                  # 라이브러리 모듈
    ├── build.gradle                                # 모듈 빌드 설정
    ├── proguard-rules.pro                         # ProGuard 규칙
    ├── consumer-rules.pro                         # Consumer ProGuard
    │
    └── src/
        └── main/
            ├── AndroidManifest.xml                 # 매니페스트
            │
            ├── java/
            │   └── com/unity/usbserial/
            │       └── UsbSerialPlugin.java       # USB Serial 플러그인
            │
            └── res/
                └── xml/
                    └── device_filter.xml          # USB 장치 필터
```

## 🛠️ 빌드 방법

### Android Studio에서 빌드

1. **프로젝트 열기**
   - `File > Open`
   - `UsbSerialImu` 폴더 선택

2. **Gradle Sync**
   - 자동으로 시작되거나
   - `File > Sync Project with Gradle Files`

3. **빌드 실행**
   - 우측 `Gradle` 탭 클릭
   - `usbserialimu > Tasks > build > assembleRelease` 더블클릭

4. **결과 확인**
   - `usbserialimu/build/outputs/aar/usbserialimu-release.aar`

### 명령줄에서 빌드

**macOS/Linux:**
```bash
# Release 빌드
./build_aar.sh release

# Debug 빌드
./build_aar.sh debug

# 또는 직접 Gradle 사용
./gradlew assembleRelease
```

**Windows:**
```batch
REM Release 빌드
build_aar.bat release

REM Debug 빌드
build_aar.bat debug

REM 또는 직접 Gradle 사용
gradlew.bat assembleRelease
```

### 빌드 옵션

**Clean 빌드:**
```bash
./gradlew clean assembleRelease
```

**Debug 빌드:**
```bash
./gradlew assembleDebug
```

**모든 빌드 타입:**
```bash
./gradlew assemble
```

## 🎮 Unity 통합

### 1. AAR 파일 추가

빌드된 AAR 파일을 Unity 프로젝트에 복사:

```
UnityProject/
└── Assets/
    └── Plugins/
        └── Android/
            ├── usbserialimu-release.aar  ← 여기에 복사
            └── ...
```

### 2. Unity에서 AAR 설정

1. Unity Editor에서 AAR 파일 선택
2. Inspector 설정:
   - ✅ **Select platforms for plugin**: Android만 선택
   - **Android Settings**:
     - CPU: `Any CPU`
     - ✅ Load on startup

### 3. Unity C# 스크립트 추가

`Assets/Scripts/` 폴더에 다음 파일들 추가:
- `UsbSerialImu.cs` (메인 라이브러리)
- `ImuExample.cs` (사용 예제)

### 4. 씬 설정

1. 빈 GameObject 생성 (이름: "ImuManager")
2. `UsbSerialImuManager` 컴포넌트 추가
3. Inspector에서 설정 조정

### 5. 빌드 및 테스트

Unity에서 Android 빌드 후 실제 기기에서 테스트

## 🔧 고급 설정

### ProGuard 최적화

`usbserialimu/build.gradle` 수정:

```gradle
buildTypes {
    release {
        minifyEnabled true
        shrinkResources true
        proguardFiles getDefaultProguardFile('proguard-android-optimize.txt'), 
                      'proguard-rules.pro'
    }
}
```

### 버전 관리

`usbserialimu/build.gradle`에 버전 추가:

```gradle
android {
    defaultConfig {
        versionCode 1
        versionName "1.0.0"
    }
}
```

### 의존성 업데이트

USB Serial 라이브러리 버전 변경:

```gradle
dependencies {
    implementation 'com.github.mik3y:usb-serial-for-android:3.6.0'  // 버전 업데이트
}
```

## 🐛 문제 해결

### SDK 경로 오류

**증상:**
```
SDK location not found. Define location with an ANDROID_SDK_ROOT environment variable
or by setting the sdk.dir path in your project's local properties file
```

**해결:**
1. 프로젝트 루트에 `local.properties` 생성
2. SDK 경로 추가:
   ```properties
   sdk.dir=/path/to/android/sdk
   ```

### Gradle Sync 실패

**해결 방법:**
```bash
# 캐시 삭제
./gradlew clean
rm -rf .gradle

# Android Studio 재시작
```

### 의존성 다운로드 실패

**해결 방법:**
1. 인터넷 연결 확인
2. `build.gradle`에서 JitPack 저장소 확인:
   ```gradle
   maven { url 'https://jitpack.io' }
   ```
3. Gradle 캐시 삭제 후 재시도

### AAR 파일이 생성되지 않음

**확인 사항:**
- [ ] Gradle 빌드가 성공적으로 완료됐는가?
- [ ] `usbserialimu/build/outputs/aar/` 폴더 확인
- [ ] 빌드 로그에서 에러 확인

### Unity에서 AAR이 인식되지 않음

**해결 방법:**
1. AAR 파일이 `Assets/Plugins/Android/`에 있는지 확인
2. Unity Editor 재시작
3. AAR 파일 Inspector 설정 확인
4. `Assets > Reimport All`

## 📚 참고 문서

- [AAR_BUILD_GUIDE.md](AAR_BUILD_GUIDE.md) - 상세 빌드 가이드
- [Android Library 공식 문서](https://developer.android.com/studio/projects/android-library)
- [Unity Android Plugin](https://docs.unity3d.com/Manual/PluginsForAndroid.html)
- [USB Serial Library](https://github.com/mik3y/usb-serial-for-android)

## 📝 버전 정보

- **프로젝트**: Unity USB Serial IMU Library
- **버전**: 1.0.0
- **타겟 SDK**: Android 13 (API 33)
- **최소 SDK**: Android 5.0 (API 21)
- **Gradle**: 8.0
- **빌드 출력**: AAR (Android Archive)

## 📄 라이선스

MIT License

## 🤝 기여

이슈와 풀 리퀘스트를 환영합니다!

---

**제작**: Unity USB Serial IMU Library Team
**마지막 업데이트**: 2024
