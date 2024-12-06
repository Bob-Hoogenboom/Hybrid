//Tutorial: https://www.youtube.com/watch?v=L4WfHT_58Dg 
//Github Library Unity3D X arduino: https://github.com/jrowberg/i2cdevlib

// I2Cdev and MPU6050 must be installed as libraries, or else the .cpp/.h files
// for both classes must be in the include path of your project
#include "I2Cdev.h"

#include "MPU6050_6Axis_MotionApps20.h"
//#include "MPU6050.h" // not necessary if using MotionApps include file

// Arduino Wire library is required if I2Cdev I2CDEV_ARDUINO_WIRE implementation
// is used in I2Cdev.h
#if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
    #include "Wire.h"
#endif

// class default I2C address is 0x68
// specific I2C addresses may be passed as a parameter here
// AD0 low = 0x68 (default for SparkFun breakout and InvenSense evaluation board)
// AD0 high = 0x69
MPU6050 mpu;

#define OUTPUT_READABLE_QUATERNION

#define INTERRUPT_PIN 2  // use pin 2 on Arduino Uno & most boards
#define LED_PIN 13       // (Arduino is 13, Teensy is 11, Teensy++ is 6)
#define CALIBRATE_BUTTON 4     // Define the button pin
#define ACTION_BUTTON 6     // Define the button pin

bool blinkState = false;

// MPU control/status vars
bool dmpReady = false;  // set true if DMP init was successful
uint8_t mpuIntStatus;   // holds actual interrupt status byte from MPU
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint16_t packetSize;    // expected DMP packet size (default is 42 bytes)
uint16_t fifoCount;     // count of all bytes currently in FIFO
uint8_t fifoBuffer[64]; // FIFO storage buffer

// orientation/motion vars
Quaternion q;           // [w, x, y, z]         quaternion container

// Interrupt detection routine
volatile bool mpuInterrupt = false;
void dmpDataReady() {
    mpuInterrupt = true;
}

void setup() {
    // Setup I2C
    #if I2CDEV_IMPLEMENTATION == I2CDEV_ARDUINO_WIRE
        Wire.begin();
        Wire.setClock(400000); // 400kHz I2C clock
    #endif

    // Initialize serial communication
    Serial.begin(115200);
    while (!Serial); // wait for Leonardo enumeration

    // Initialize MPU
    Serial.println(F("Initializing I2C devices..."));
    mpu.initialize();
    pinMode(INTERRUPT_PIN, INPUT);

    // Verify connection
    Serial.println(mpu.testConnection() ? F("MPU6050 connection successful") : F("MPU6050 connection failed"));

    // Initialize DMP
    Serial.println(F("Initializing DMP..."));
    devStatus = mpu.dmpInitialize();

    // Set gyro offsets
    mpu.setXGyroOffset(220);
    mpu.setYGyroOffset(76);
    mpu.setZGyroOffset(-85);
    mpu.setZAccelOffset(1788);

    // Check if DMP initialization was successful
    if (devStatus == 0) {
        mpu.CalibrateAccel(6);
        mpu.CalibrateGyro(6);
        mpu.PrintActiveOffsets();
        mpu.setDMPEnabled(true);

        // Enable interrupt detection
        attachInterrupt(digitalPinToInterrupt(INTERRUPT_PIN), dmpDataReady, RISING);
        mpuIntStatus = mpu.getIntStatus();

        // Set flag
        dmpReady = true;
        packetSize = mpu.dmpGetFIFOPacketSize();
    } else {
        Serial.print(F("DMP Initialization failed (code "));
        Serial.print(devStatus);
        Serial.println(F(")"));
    }

    // Configure LED and button pin
    pinMode(LED_PIN, OUTPUT);
    pinMode(CALIBRATE_BUTTON, INPUT_PULLUP); // Enable internal pull-up resistor
    pinMode(ACTION_BUTTON, INPUT_PULLUP); // Enable internal pull-up resistor
}

void loop() {
    if (!dmpReady) return;

    // Check if FIFO has data and handle overflow
    fifoCount = mpu.getFIFOCount();
    if (fifoCount >= 1024) { // Overflow
        mpu.resetFIFO();
        Serial.println(F("FIFO overflow!"));
    }

    // If interrupt flag is set, read the packet
    if (mpuInterrupt || fifoCount >= packetSize) {
        mpuInterrupt = false; // Clear interrupt flag

        // Get data from FIFO
        if (mpu.dmpGetCurrentFIFOPacket(fifoBuffer)) {
            mpu.dmpGetQuaternion(&q, fifoBuffer);
            int calButtonState = digitalRead(CALIBRATE_BUTTON);
            int actionButtonState = digitalRead(ACTION_BUTTON);

            // Print quaternion and button state
            Serial.print(q.w);
            Serial.print(",");
            Serial.print(q.x);
            Serial.print(",");
            Serial.print(q.y);
            Serial.print(",");
            Serial.print(q.z);
            Serial.print(",");
            Serial.print(calButtonState); 
            Serial.print(",");
            Serial.println(actionButtonState);

            // Blink LED
            blinkState = !blinkState;
            digitalWrite(LED_PIN, blinkState);

            // Delay for Unity frame rate
            delay(33);
        }
    }
}
