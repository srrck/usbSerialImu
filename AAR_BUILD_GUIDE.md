# Unity Android USB Serial IMU - AAR 빌드 가이드

## 📦 Android Studio AAR 라이브러리 프로젝트

Unity에서 사용할 수 있는 AAR(Android Archive) 파일을 빌드하는 가이드입니다.

## 📁 프로젝트 구조

```
UsbSerialImu/
├── build.gradle                          # 프로젝트 레벨 빌드 설정
├── settings.gradle                       # 프로젝트 설정
├── gradle.properties                     # Gradle 속성
│
└── usbserialimu/                        # 라이브러리 모듈
    ├── build.gradle                      # 모듈 레벨 빌드 설정
    ├── proguard-rules.pro               # ProGuard 규칙
    ├── consumer-rules.pro               # Consumer ProGuard 규칙
    │
    └── src/
        └── main/
            ├── AndroidManifest.xml       # 매니페스트
            ├── java/
            │   └── com/
            │       └── unity/
            │           └── usbserial/
            │               └── UsbSerialPlugin.java  # 메인 플러그인
            │
            └── res/
                └── xml/
                    └── device_filter.xml  # USB 장치 필터
```

## 🚀 빌드 방법

### 방법 1: Android Studio 사용

1. **Android Studio 열기**
   ```
   File > Open > UsbSerialImu 폴더 선택
   ```

2. **Gradle Sync**
   ```
   프로젝트를 열면 자동으로 Gradle Sync 시작
   또는 File > Sync Project with Gradle Files
   ```

3. **AAR 빌드**
   ```
   우측 Gradle 탭 클릭
   UsbSerialImu > usbserialimu > Tasks > build > assembleRelease 더블클릭
   ```

4. **AAR 파일 위치**
   ```
   usbserialimu/build/outputs/aar/usbserialimu-release.aar
   ```

### 방법 2: 명령줄 사용

**Windows:**
```bash
cd UsbSerialImu
gradlew assembleRelease
```

**macOS/Linux:**
```bash
cd UsbSerialImu
./gradlew assembleRelease
```

**결과물:**
```
usbserialimu/build/outputs/aar/usbserialimu-release.aar
```

### 방법 3: Debug 버전 빌드

```bash
# Android Studio
Gradle > usbserialimu > Tasks > build > assembleDebug

# 명령줄
./gradlew assembleDebug
```

## 📋 빌드 전 체크리스트

### 1. JDK 설정
- [ ] JDK 11 이상 설치
- [ ] Android Studio의 JDK 경로 설정 확인
  - `File > Project Structure > SDK Location`

### 2. Android SDK 설정
- [ ] Android SDK 설치 (API Level 33)
- [ ] Android SDK Build-Tools 설치
- [ ] SDK Manager에서 필요한 컴포넌트 설치

### 3. Gradle 설정
- [ ] gradle-wrapper.properties 확인
- [ ] 인터넷 연결 (의존성 다운로드)

### 4. 의존성 확인
- [ ] usb-serial-for-android 라이브러리 다운로드 가능한지 확인
- [ ] JitPack 저장소 접근 가능한지 확인

## 🔧 Unity 프로젝트에 통합

### 1. AAR 파일 추가

생성된 AAR 파일을 Unity 프로젝트에 추가:

```
UnityProject/
└── Assets/
    └── Plugins/
        └── Android/
            ├── usbserialimu-release.aar  ← 여기에 복사
            └── ... (기타 파일들)
```

### 2. Unity에서 AAR 설정

1. Unity Editor에서 AAR 파일 선택
2. Inspector 창에서:
   - **Select platforms for plugin**: Android만 체크
   - **Android Settings**:
     - CPU: Any CPU
     - Load on startup: 체크

### 3. Unity C# 스크립트에서 사용

```csharp
// UsbSerialImu.cs에서 이미 구현됨
AndroidJavaObject usbSerialPlugin = new AndroidJavaObject("com.unity.usbserial.UsbSerialPlugin");
```

## 📊 빌드 출력물

### Release 빌드
- **파일**: `usbserialimu-release.aar`
- **크기**: 약 50-100 KB (의존성 제외)
- **최적화**: ProGuard 적용 가능
- **용도**: 배포용

### Debug 빌드
- **파일**: `usbserialimu-debug.aar`
- **크기**: Release보다 큼
- **최적화**: 없음
- **용도**: 디버깅용

## 🛠️ 고급 설정

### ProGuard 활성화

`usbserialimu/build.gradle`에서:

```gradle
buildTypes {
    release {
        minifyEnabled true  // 변경
        proguardFiles getDefaultProguardFile('proguard-android-optimize.txt'), 'proguard-rules.pro'
    }
}
```

### 버전 관리

`usbserialimu/build.gradle`에 추가:

```gradle
android {
    defaultConfig {
        versionCode 1
        versionName "1.0.0"
    }
}
```

### Maven 배포 설정

`usbserialimu/build.gradle`에 추가:

```gradle
apply plugin: 'maven-publish'

publishing {
    publications {
        release(MavenPublication) {
            groupId = 'com.unity'
            artifactId = 'usbserialimu'
            version = '1.0.0'

            afterEvaluate {
                from components.release
            }
        }
    }
}
```

## 🐛 문제 해결

### "SDK location not found" 오류

**해결방법:**
1. 프로젝트 루트에 `local.properties` 생성
2. 다음 내용 추가:
   ```properties
   sdk.dir=/Users/YourName/Library/Android/sdk
   # 또는 Windows: C\:\\Users\\YourName\\AppData\\Local\\Android\\Sdk
   ```

### Gradle Sync 실패

**해결방법:**
1. 인터넷 연결 확인
2. Gradle 캐시 삭제:
   ```bash
   ./gradlew clean
   rm -rf .gradle
   ```
3. Android Studio 재시작

### "Unable to resolve dependency" 오류

**해결방법:**
1. `build.gradle`에서 JitPack 저장소 확인:
   ```gradle
   maven { url 'https://jitpack.io' }
   ```
2. 인터넷 프록시 설정 확인
3. 의존성 버전 확인

### AAR 크기가 너무 큼

**해결방법:**
1. ProGuard 활성화
2. 불필요한 리소스 제거
3. `build.gradle`에서:
   ```gradle
   buildTypes {
       release {
           shrinkResources true
           minifyEnabled true
       }
   }
   ```

## 📝 빌드 스크립트

자동화를 위한 빌드 스크립트:

**build_aar.sh (macOS/Linux):**
```bash
#!/bin/bash
echo "Building AAR..."
./gradlew clean assembleRelease

echo "Copying AAR to output..."
mkdir -p output
cp usbserialimu/build/outputs/aar/*.aar output/

echo "Done! AAR file is in output/ directory"
ls -lh output/
```

**build_aar.bat (Windows):**
```batch
@echo off
echo Building AAR...
gradlew.bat clean assembleRelease

echo Copying AAR to output...
if not exist output mkdir output
copy usbserialimu\build\outputs\aar\*.aar output\

echo Done! AAR file is in output\ directory
dir output\
```

## 🔍 AAR 내용 확인

AAR 파일은 ZIP 파일이므로 압축 해제하여 내용 확인 가능:

```bash
unzip usbserialimu-release.aar -d aar_contents
```

**AAR 내부 구조:**
```
aar_contents/
├── AndroidManifest.xml
├── classes.jar           # 컴파일된 Java 클래스
├── R.txt                 # 리소스 ID
├── res/                  # 리소스 파일
│   └── xml/
│       └── device_filter.xml
└── libs/                 # 추가 라이브러리 (있는 경우)
```

## 📚 참고 자료

- [Android Library 공식 문서](https://developer.android.com/studio/projects/android-library)
- [AAR 포맷 설명](https://developer.android.com/studio/projects/android-library#aar-contents)
- [Gradle 빌드 가이드](https://developer.android.com/studio/build)
- [ProGuard 설정](https://developer.android.com/studio/build/shrink-code)

## 💡 팁

1. **빌드 속도 향상**
   ```properties
   # gradle.properties에 추가
   org.gradle.parallel=true
   org.gradle.daemon=true
   org.gradle.caching=true
   ```

2. **클린 빌드**
   ```bash
   ./gradlew clean build
   ```

3. **의존성 확인**
   ```bash
   ./gradlew dependencies
   ```

4. **빌드 캐시 삭제**
   ```bash
   ./gradlew cleanBuildCache
   ```

---

**프로젝트**: Unity USB Serial IMU Library
**빌드 타겟**: AAR (Android Archive)
**최소 SDK**: API 21 (Android 5.0)
**타겟 SDK**: API 33 (Android 13)
