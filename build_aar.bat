@echo off
REM Unity USB Serial IMU AAR 빌드 스크립트 (Windows)

echo ======================================
echo Unity USB Serial IMU AAR Builder
echo ======================================
echo.

REM 빌드 타입 설정
set BUILD_TYPE=%1
if "%BUILD_TYPE%"=="" set BUILD_TYPE=release

if not "%BUILD_TYPE%"=="release" if not "%BUILD_TYPE%"=="debug" (
    echo Usage: build_aar.bat [release^|debug]
    echo Default: release
    exit /b 1
)

echo Build Type: %BUILD_TYPE%
echo.

REM 클린 빌드
echo Cleaning previous build...
call gradlew.bat clean

if errorlevel 1 (
    echo Failed to clean!
    exit /b 1
)

REM AAR 빌드
echo.
echo Building AAR (%BUILD_TYPE%)...

if "%BUILD_TYPE%"=="release" (
    call gradlew.bat assembleRelease
) else (
    call gradlew.bat assembleDebug
)

if errorlevel 1 (
    echo Build failed!
    exit /b 1
)

REM 출력 디렉토리 생성
echo.
echo Preparing output directory...
if not exist output mkdir output

REM AAR 파일 복사
echo Copying AAR file...
copy usbserialimu\build\outputs\aar\usbserialimu-%BUILD_TYPE%.aar output\

if errorlevel 1 (
    echo Failed to copy AAR file!
    exit /b 1
)

REM 성공 메시지
echo.
echo ======================================
echo Build successful!
echo ======================================
echo.
echo AAR file location:
dir output\usbserialimu-%BUILD_TYPE%.aar
echo.
echo To use in Unity:
echo 1. Copy output\usbserialimu-%BUILD_TYPE%.aar to Assets\Plugins\Android\
echo 2. Add UsbSerialImu.cs to your Unity project
echo 3. Follow the README.md for usage instructions
echo.

pause
