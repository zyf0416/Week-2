# Prototyping motion-based interaction with an Android Accelerometer in Unity

In the lecture, we saw a number of different approaches for designing motion-based interactions. We also saw how many of these could be implemented using the accelerometer sensors found in most modern mobile devices (and implemented a bit more elegantly and robustly by combining gyroscope and compass sensor data where available).

In this practical, we are going to learn how to get started with prototyping movement based interactions by using the accelerometer in your Android tablets. We will see how to implement some basic motion-based interactions using pose and simple relative movement on rails. We aren’t going to look at absolute movement yet – because this will come later in the module when we work with the HTC Vive’s motion controllers during the VR-related weeks.

All the files you need for this practical are in this repository. To get started:

1. Make a copy of this repository in your GitHub account by pressing the ```Use Template``` button
2. Clone your copy of the repository onto the machine you're working on
3. Load up the Unity project that's inside the repository

## Task 1: Getting started with Android development in Unity

In this practical, we are going to use Unity to create some simple Android apps that use the accelerometer for interaction. The Unity project you're working with has already been configured to build and run on Android. However, in case you ever need to convert a Unity project to run on Android in the future (e.g. for your assessment) these tutorials can help you:

- Enabling Android debugging: https://docs.unity3d.com/Manual/android-sdksetup.html
- Setting the Bundle Identifier: http://answers.unity3d.com/questions/162141/android-bundle-identifier-has-not-been-setup.html

## Task 2: Accessing and printing accelerometer data in Unity

Recall from the lecture that an accelerometer is a simple device that senses acceleration along an axis in g-force. Your Android tablets have a three-axis accelerometer, which senses acceleration along the x, y and z axes. Accessing the accelerometer in Unity is very simple. Last year we saw how Unity has an Input class that provides access to most common input devices – like keyboard, mice and game pads. This same class has a public variable called acceleration, which provides the current accelerometer readings for the x , y and z axes as a 3D Vector representing acceleration in g-force:

- https://docs.unity3d.com/ScriptReference/Input-acceleration.html

Your next task is to display the acceleration values from the accelerometer on the screen. You should do this by creating a UI with a text element in the top right corner of the screen and updating it every frame with the current accelerometer values using a script. If you can’t recall how to create a text UI from last year, see this tutorial for a reminder:

- https://unity3d.com/learn/tutorials/topics/user-interface-ui/ui-text

You may wish to include the following line in your startup method to prevent the device from rotating when completing this and the subsequent tasks:

```c#
Screen.orientation = ScreenOrientation.LandscapeLeft;
```

Run your script and look at the three values. You should notice two things:

1. The numbers change if you shake the tablet around (because its accelerating/decelerating)
2. If the tablet is held still, the three numbers won’t read ```[0, 0, 0]```. Rather, they will read values that make up a vector pointing away from gravity (e.g. ```[0, 1, 0]```).

## Task 3: Tilt control of a racing car using pose

One of the most common motion-based interactions we see in mobile games is tilt control. Tilt control let’s us do things like steer a car or roll a ball by reorienting a mobile device in our hand. We can implement tilt control by inferring information about the tablet’s pose from its accelerometer readings.

Recall from the lecture that when an accelerometer is held still, it will read a constant 1g acceleration-vector pointing away from the center of the earth. We can use the direction of this acceleration vector to work out whether the device is tilted in a particular direction. To see this phenomenon in action, complete the simple experiment with the scene you made in the last task:

1. Hold the device in your hand so that the screen is parallel with the floor/desk
2. Tilt the device to the left and right and watch how the numbers change
3. Tilt the device forwards and backwards and watch how the numbers change

You should notice that one of the axes varies between -1.0 and 1.0 when you tilt the device left and right and another varies between -1.0 and 1.0 when you tilt forward and backwards. Notice that this data is very similar to the data we got from the analogue control stick of the gamepad using Input.GetAxis in the MPIE module last year.

In this task, you are going to use this tilt information from the accelerometer to control a racing car. The racing car should be controlled in the following way when the player holds the tablet like a steering wheel:

1. Roll the device left and right to steer, with the extent of roll affecting the sharpness of turn
2. Tilt the device forward to accelerate and backwards to brake

I've provdied a Car prefab you can use for this task. You can find it in ```Prefabs``` in the ```Task 3 - Car``` folder of the project. Once you’ve added the car into a scene, locate the ```CarUserControl``` script component and edit the code in the ```FixedUpdate``` method so that it uses the accelerometer for steering and acceleration/braking rather than the Horizontal and Vertical axes from the gamepad.

## Task 4: Removing unwanted movement data from our car controller

You might notice that the car control is a little jittery and has errors when you move the device up/down or left/right in your hands. This is because the accelerometer data senses any movements that the device makes, as well as the 1g upwards acceleration caused by gravity that we are using to infer pose.

We can use a low-pass filter to remove acceleration readings caused by movements (i.e. high frequency data) and leave only the acceleration caused by gravity (i.e. low frequency data). The code below shows how to implement a simple low-pass filter in C#. This works by keeping a running average of a sensor reading over time (as the ‘gravity’ variable) and adding a proportion of that average to a proportion of the current sensor reading (the ‘x’ variable). Note that the proportion of current reading vs. averaged gravity that is used depends on the ‘sensitivity’ variable. If a larger proportion of the averaged gravity is used, the pose of the device will be less susceptible to noise from movements, but will also be less responsive due to the averaging of sensor values over time. 

```c#
// member variables declared at class level (i.e. put this bit at the top of your script)
float gravity = 0.0f;
float sensitivity = 0.1f;

// code to calculate latest gravity reading for axis in update function (put this in, e.g., the update function)
gravity = ((1.0f - sensitivity) * gravity) + (sensitivity * x);
```
To complete the task, use a low-pass filter to extract the gravity value for the two accelerometer axes that you are using to control the car (note: you’ll need to replicate the low pass filter code for each axis). Experiment with different sensitivity values to find a nice balance between smooth control and responsiveness for each.

## Task 5: An on rails approach for sword fighting

In the lecture, we saw how the ‘on rails’ approach could give the user an illusion of having greater control over motion-based interactions than is actually possible to track with an accelerometer (or other sensor). With this approach, simple movement events (like average acceleration on all axes exceeding a threshold) are used to trigger animations that correspond with more complex movements that the user is instructed to make (e.g. in a game tutorial). For example, the Wii sports games use this approach to good effect.

In this final task we are going to create a very simple ‘on rails’ interaction where a sharp upwards movement of the tablet causes a sword slashing animation to trigger on a character. To get started, open the scene within the ```Task 5 - On Rails``` folder in the Unity project. The Paladin game object in this scene has a script attached to them that contains a ```Swing``` method that can be used to trigger a sword swinging animation.

To implement the on rails interaction described, we need to implement the following two steps:

1. Check whether the tablet is currently moving upwards by looking for acceleration faster than a threshold on one of its axes
2. If it is, play the animation using the ‘Swing’ method

Implementing the interaction is a little bit more complicated than simply reading the raw acceleration values to see if the tablet is moving. Recall that our accelerometer readings include an acceleration of 1g in a direction away from the center of the earth due to gravity. Therefore, we may see acceleration on the axis we define as upwards due to gravity, even when the user hasn’t moved the tablet.

There is a simple solution to this problem. In the last task, we used a low-pass filter to extract the gravity value for an accelerometer axis. By subtracting this gravity value from our current sensor reading, we can extract sensor readings that relate to movement (this is called a high-pass filter). For example, the code below shows how the saved gravity value for the x-axis (the ‘gravity’ variable) can be subtracted from the current x-axis sensor reading (the ‘x’ variable) to give the current movement on the x-axis:

```c#
float movement = x - gravity;
```

Implement the on rails interaction described above using a high-pass filter like the one shown above. Two additional animations are included in the Paladin character (```Kick``` and ```Shield Slash```). To complete the task, write code to trigger these animations in response to different relative movements of the tablet.

## Optional Extensions

If you complete all of the above tasks before the end of the practical, or would like to continue to develop your skills in your free study time, then you should consider experimenting with some of the following tasks:

- Improve the control of your car racing game by lessening the reduction in sensitivity to the car’s steering that is caused as the tablet is tilted forward and back to brake or accelerate (this is a flaw in the basic implementation caused by the inability of the sensor to detect rotation around the axis of gravity).
- Consider using changes in rotation as well as movement to better discern between different gestures when controlling the animated attacks of the Paladin character.
