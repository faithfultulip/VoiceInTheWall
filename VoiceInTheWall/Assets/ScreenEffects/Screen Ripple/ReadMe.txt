Thank you for choosing Screen Ripple !

This is a complete collection of ripple shader package.
There are four demos in package, please note you need click the play button to see the ripple effect.
1. Demo - Ripple Collection
   This is a collection of post process ripple shaders.
   There are 3 ripple functional components attached on camera:
   
   - Screen Click Ripple
     It contains three kinds of click ripple. You can switch ripple type in enum inspector "Ripple Type".
	 Click left mouse in game view windows, you will see the ripple spread from your click position.
	 There are also rich enough set of parameters in inspector, click the small arrow "Ripple 1" / "Ripple 2" / "Ripple 3" and expand corresponding tweakable parameters.
	 
	 Toggle "Enable Mask Objects" make ripple not affect gameobjects mark as "Ripple Mask Name" layer.
	 
   - Screen Flow Ripple
     Simulate water flow down on screen. Useful case for example, image your character come out from underwater and water cover his lunettes.
	 This is a texture free post process effect, not need any addtional texture.
	 
   - Screen Water Distortion
     When you enable this component, you will see water splash on your screen.
	 This effect require two textures represent "bump map" and "relief map". And it is really realistic.
   
   How to use them in your own project ?
    => Just attach the corresponding component to your main camera.
	=> Component "ScreenClickRipple.cs" used for screen click ripple.
    => Component "ScreenFlowRipple.cs"	used for screen flow ripple.
	=> Component "ScreenWaterDistortion.cs" used for screen water distortion.

2. Demo - 2D FullScreen Water
   2D water reflection fx based on full screen effect shader.
   How to used in your own project ?
    => Just attach component "ScreenReflection.cs" to your main camera.

3. Demo - 2D Lake
   This is a per-sprite reflection fx shader.
   You can use any shape of geometry to represent a region of water. And make a sprite reflected by this region of water.
   How to used in your own project ?
    => Create a quad represent the water surface, use shader "Screen Ripple/Lake 2D Ripple" and attach component "Lake2DWater.cs" to control it.
    => Make the sprites need to be reflected draw before water surface, this can be done by set appropriate sprite layer.
	=> Attach component "Lake2DSprite.cs" to sprites that need inverted reflection in water. Note set the reflected sprite layer. Let it draw before water surface.
   
4. Demo - Rain
   A on screen rain drop ripple shader.
   The rain is simulated based on a unity particle system with a specific shader work with particle system.
   How to used in your own project ?
    => Create a particle system splash quad on screen, attach material that use shader "Screen Ripple/Droplet".
	
5. Demo - Droplet
   High quality on screen water droplet shader. Relative heavy on mobile platform.
   How to used in your own project ?
    => Create a plane mesh with shader "Screen Ripple/Droplet2" cover screen, and attach script "CaptureScene.cs".

Demo scene demonstrate all features. Please reference them as usage example.

In version 1.8 upgrade, we solved the Y flip click ripple problem.
In version 1.9 upgrade, we support resolution independent perfect circle shape for ripple1 and ripple2.
In version 2.0 upgrade, a whole new big upgrade. Add many new features!
In version 2.1 upgrade, we add a high quality on screen water droplet demo.

If you like it, please give us a good review on asset store. We will keep moving !
Any question, suggestion or requesting, please contact qq_d_y@163.com.
Hope we can help more and more unity3d developers.