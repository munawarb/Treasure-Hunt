#### What's New
On this page you'll find a list of all updates made to Treasure Hunt since it's open-source push on November 11, 2018. The Master branch incorporates all of the changes noted here; therefore, to get the latest version, either pull the latest Master branch or download its zip file.

##### 4.44 11/18/2018
- The game no longer relies on hard-coded frame ticks and now operates on a time elapsed basis with respect to frames. In other words, the game time might look something like `fw = t` where `f` represents the number of frames in the current session and `w` represents the width of each frame in milliseconds. Previously, Treasure Hunt was only accounting for `f` and assumed `w=100`, so the time elapsed formula would look like `100f = t`. This change allows for future adjustments to the frame rate if needed ([#5](../../issues/5))
- The game no longer uses the `Short` type and all `Short` integers have been promoted to `Integer`. This was necessary to prevent arithmetic overflow exceptions especially on account of trackers now using the frame width ([#10](../../issues/10))
- The game loop that was started in the load event handler has been replaced with a timer to control the game loop. This is less resource-intensive ([#11](../../issues/11))
- Treasure Hunt no longer uses `KeyDown` and takes advantage of `DirectInput` through the use of `BPCSharedComponent.dll`, a library I wrote for Three-D Velocity. Keys are now processed in the game loop, making key processing smoother ([#9](../../issues/9))
- The single footstep sound has been replaced with better footstep sounds ([#7](../../issues/7))
- Key presses are handled better now and weapons can be rapid-fired. A lot of the key-handling code was cleaned up ([#6](../../issues/6))
- The game loop is now much cleaner and no longer runs every tenth of a second. The slow frame rate would force the player to slow down so the game could update the state ([#4](../../issues/4))
- When the player holds down an arrow key, the character will move steadily in that direction as opposed to taking only one step ([#3](../../issues/3))

##### 4.42 11/13/2018
- Treasure Hunt now has an auto updater, and the window layout has been fixed ([#2](../../issues/2))
- Exceptions should be logged properly now ([#1](../../issues/1))