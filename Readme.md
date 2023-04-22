# Easy3rdPerson - Quick and free way to allow 3rd person in your VRChat Udon world for Desktop players.

## Overview

* This is a script written in C# using UdonSharp for VRChat, a social virtual reality platform. The script provides a way to toggle between first-person and third-person camera modes and enables various camera settings like zoom, center translate, and clip settings. It also allows the user to set up a player fade effect, where the visibility of the player model fades as the user moves further away from it.

* The script begins by defining several private variables and public settings. The public settings can be modified from the Inspector window in Unity Editor, allowing users to customize the behavior of the script. The private variables, on the other hand, are used internally by the script and should not be modified directly.

* The script contains several functions that are called at various points in its lifecycle. Start() initializes some of the variables and disables the script if the user is in VR mode. Update() is called once per frame and is responsible for updating the position and rotation of the camera based on the user's head tracking data. It also checks if the user has pressed a key to switch camera modes and updates the camera settings accordingly.

* The UpdateRenderTexture() function is used to update the render textures used by the cameras. This function is called once at the beginning of the script and every time the display size of the camera changes. The UpdateThirdRenderTexture(), UpdateFirstRenderTexture(), and UpdateThirdRenderTexturePlayer() functions are helper functions that create new render textures for the cameras and update the textures when necessary.

* The script also contains a number of private variables that are used to keep track of the camera state and various settings. These variables are updated when the user presses the appropriate keys or when the camera is moved.

* Overall, the script provides a way to quickly and easily switch between first-person and third-person camera modes, with a number of customizable settings for both modes. It is designed to work within the VRChat environment, but could potentially be adapted for use in other applications or games.

### Demo Video : 

[<img src="https://i.ibb.co/Zc9HB0t/3rdpersonthumbnail.png" width="20%">](https://drive.google.com/file/d/1CtsqWGevlS4Op6z4t0R2UMJ9lHwa2pPb/view?usp=sharing "Easy3rdPersonUdon Demo Video")


## Default Controls

* Use the Mouse Wheel Scroll to zoom towards your character in Third Person View (Front or Back).
* Use X to switch from a view to another (First -> Third Back -> Third Front -> First -> ...).
* Use Left Control and Mouse Wheel Scroll to translate vertically the center of third person camera rotation.

## Install

To install the script, drag and drop the prefab Easy3rdPerson.prefab in the scene after opening the unity package.

## Parameters

Do not touch the Internal parameters in the inspector!

* switchViewInput: The key input for switching between the 1st and 3rd person view.
* zoomSpeed: The speed of the zoom function.
* zoomMin: The minimum zoom value.
* zoomMax: The maximum zoom value.
* translateCenterInput: The key input for translating the center of the view.
* deltaCenterSpeed: The speed of the center translation.
* maxDeltaCenter: The maximum value for the center translation.
* minDeltaCenter: The minimum value for the center translation.
* clipLayers: The layers to clip with.
* enableDeltaClip: A boolean that enables the clipping feature.
* deltaClip: The value of the clip offset.
* enableGradientPlayer: A boolean that enables the player fade feature.
* minimumPlayerDisplayDistance: The minimum distance the player is displayed on the canvas.
* totalPlayerDisplayDistance: The total distance the player is displayed on the canvas.

## Known issues

* The prefab may reduce FPS since it works with RenderTextures.
* Sometimes, the menu may not show properly, just switch back to first person mode, change avatar, move a bit and it should be fine.

## Advanced Information for developers

### Private Variables:

* displayImageTransform: A reference to the RectTransform of the RawImage.
* displayImage: A reference to the RawImage for the 3rd person view.
* displayImageUI: A reference to the RawImage for the 1st person view.
* displayImagePlayer: A reference to the RawImage for the player view.
* thirdPersonCamera: A reference to the 3rd person camera.
* thirdPersonCameraPlayer: A reference to the player camera.
* firstPersonCamera: A reference to the 1st person camera.
* backSpot: A reference to the Transform for the back position.
* frontSpot: A reference to the Transform for the front position.
* loadingWarning: A reference to the loading warning object.
* firstPersonRenderTexture: The RenderTexture for the 1st person view.
* thirdPersonRenderTexture: The RenderTexture for the 3rd person view.
* thirdPersonRenderTexturePlayer: The RenderTexture for the player view.
* displaySize: The size of the display.
* currentState: The current state of the view (0 = 1st person, 1 = back, 2 = front).
* currentFrontScroll: The current value of the front scroll.
* currentBackScroll: The current value of the back scroll.
* currentDeltaCenter: The current value of the center translation.

### Private Methods:

* Start(): Initializes the script by checking if the user is in VR and updating the render texture.
* UpdateRenderTexture(): Updates the render texture for the 3rd person view and the player view.
* UpdateThirdRenderTexture(): Updates the render texture for the 3rd person view.
* UpdateFirstRenderTexture(): Updates the render texture for the 1st person view.
* UpdateThirdRenderTexturePlayer(): Updates the render texture for the player view.
* Update(): Updates the view and checks for user input.
* UpdateScroll() : Updates the current scroll view and gradient of the player.
* UpdateScrollBack(float scroll) : Helper method for Update Scroll for the back view.
* UpdateScrollFront(float scroll) : Helper method for Update Scroll for the front view.
* SwitchState() : Switch the view between first, back and front.
* Switch3rdPerson() : Helper method for SwitchState() for regrouping common behaviours between back and front.

Note: This advanced section assumes that the user has a basic understanding of Unity and scripting.
