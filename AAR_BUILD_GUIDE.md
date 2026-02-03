AAR Build Guide – usbSerialImu

이 프로젝트는 USB Serial IMU 처리를 위한 Android AAR 라이브러리를 빌드하기 위한 레포지토리입니다.

이 레포의 유일한 목적은 다음 파일을 생성하는 것입니다:

usbserialimu-release.aar


Unity 관련 코드는 이 레포에 포함되어 있지 않습니다.

📁 프로젝트 구조
UsbSerialImu/
├── build.gradle
├── settings.gradle
├── gradle.properties
│
└── usbserialimu/
    ├── build.gradle
    ├── proguard-rules.pro
    ├── consumer-rules.pro
    └── src/main/
        ├── AndroidManifest.xml
        ├── java/com/unity/usbserial/UsbSerialPlugin.java
        └── res/xml/device_filter.xml

✅ 요구 사항

Android Studio

JDK 11 이상

Android SDK API 33 이상

인터넷 연결 (Gradle 의존성)

🚀 프로젝트 열기

Android Studio에서:

File → Open → UsbSerialImu 폴더


Gradle Sync 완료 대기

SDK 오류 발생 시, 프로젝트 루트에 local.properties 생성:

sdk.dir=본인_Android_SDK_경로

🏗 AAR 빌드
Android Studio

Gradle 패널:

usbserialimu → Tasks → build → assembleRelease

명령줄

Windows

gradlew assembleRelease


macOS / Linux

./gradlew assembleRelease

📦 결과물 위치
usbserialimu/build/outputs/aar/usbserialimu-release.aar


이 파일이 이 레포의 최종 산출물입니다.

🧪 Debug 빌드 (선택)
assembleDebug


결과:

usbserialimu-debug.aar

🐛 문제 해결
SDK location not found

local.properties 생성 후 SDK 경로 입력

Dependency 다운로드 실패

인터넷 연결 확인

JitPack 접근 가능 여부 확인

클린 빌드
./gradlew clean

📌 주의 사항

이 레포에 Unity 코드 추가하지 마세요

Java 패키지명 변경 시 Unity 쪽도 반드시 수정해야 합니다

이 레포는 Android AAR 빌드 전용입니다

목표:

USB Serial IMU Android AAR 라이브러리 빌드