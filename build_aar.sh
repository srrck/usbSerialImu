#!/bin/bash

# Unity USB Serial IMU AAR 빌드 스크립트

echo "======================================"
echo "Unity USB Serial IMU AAR Builder"
echo "======================================"
echo ""

# 빌드 타입 선택
BUILD_TYPE=${1:-release}

if [ "$BUILD_TYPE" != "release" ] && [ "$BUILD_TYPE" != "debug" ]; then
    echo "Usage: ./build_aar.sh [release|debug]"
    echo "Default: release"
    exit 1
fi

echo "Build Type: $BUILD_TYPE"
echo ""

# 클린 빌드
echo "Cleaning previous build..."
./gradlew clean

if [ $? -ne 0 ]; then
    echo "❌ Clean failed!"
    exit 1
fi

# AAR 빌드
echo ""
echo "Building AAR ($BUILD_TYPE)..."

if [ "$BUILD_TYPE" == "release" ]; then
    ./gradlew assembleRelease
else
    ./gradlew assembleDebug
fi

if [ $? -ne 0 ]; then
    echo "❌ Build failed!"
    exit 1
fi

# 출력 디렉토리 생성
echo ""
echo "Preparing output directory..."
mkdir -p output

# AAR 파일 복사
echo "Copying AAR file..."
cp usbserialimu/build/outputs/aar/usbserialimu-${BUILD_TYPE}.aar output/

if [ $? -ne 0 ]; then
    echo "❌ Failed to copy AAR file!"
    exit 1
fi

# 성공 메시지
echo ""
echo "======================================"
echo "✅ Build successful!"
echo "======================================"
echo ""
echo "AAR file location:"
ls -lh output/usbserialimu-${BUILD_TYPE}.aar
echo ""
echo "File size:"
du -h output/usbserialimu-${BUILD_TYPE}.aar
echo ""
echo "To use in Unity:"
echo "1. Copy output/usbserialimu-${BUILD_TYPE}.aar to Assets/Plugins/Android/"
echo "2. Add UsbSerialImu.cs to your Unity project"
echo "3. Follow the README.md for usage instructions"
echo ""
