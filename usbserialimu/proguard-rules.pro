# Add project specific ProGuard rules here.
# You can control the set of applied configuration files using the
# proguardFiles setting in build.gradle.

# Keep Unity classes
-keep class com.unity3d.player.** { *; }

# Keep our plugin classes
-keep class com.unity.usbserial.** { *; }

# Keep USB Serial library classes
-keep class com.hoho.android.usbserial.** { *; }

# Keep methods called from Unity
-keepclassmembers class com.unity.usbserial.UsbSerialPlugin {
    public *;
}
