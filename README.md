# my-improved-version-of-sm64dse
This is an edited version of sm64dse 2.3.5 by Fiachra

The .exe file is in bin/release

<h3>Changelog:</h3><br/>
-invisible pole now has an indicator for its height
<br/>
-custom Model for the StarMarker
<br/>
-painting shows its tilt properly
<br/>
-exit objects show their whole trigger area, also tilted
<br/>
-path objects draw the node-connecting lines when highlighted
<br/>
-added buttons for copying and pasting the objects coordinates
<br/>

<h4>Changes v1.1:</h4><br/>
-new interface for editing object properties
<br/>
-paths now show up as closed if they are
<br/>
-double clicking an object in the objectlist snaps the camera to it (for example usefull to find the level entrances)
<br/>
-in the 3d model tab you can select single areas to show up (pretty usefull to see what is going to be visible when entering a door)
<br/>
-some BugFixes
<br/>
<h4>Changes v1.2:</h4><br/>
-views now have index numbers like entrances
<br/>
-nodes are now ordered by their path
<br/>
-adding new PathNodes is much easier
<br/>
-the parameters for the paths startindex and its length are removed as they aren't needed and would screw up everything
<br/>
-saving won't screw up your paths anymore (at least i hope so)
<br/>

<h4>Changes v1.3:</h4><br/>
-if Model 3D Tab is active and an area is selected, the editor will only show the models that will be rendered
<br/>
-updated and added some star / star marker models, so you know what object you are actually dealing with
<br/>
-filetype filtering for selecting a: BMD, KCL, BTP, BCA, NCG/ICG, NCL/ICL, ISC/NSC -file
<br/>
-replacing a texture in the Texture Editor should now be possible for every texture, but you have to make sure that the correct palette is selected, because the auto selected one is not always what you want
<br/>

<h4>Changes v1.4:</h4><br/>
-i finally got rid of the old property table interface
<br/>
-there is an extra window for editing raw parameters, it has some conversion features
<br/>
-i added object specific parameters for Paintings, Stars and Star Markers
<br/>
-i fixed the one missing line of the Pole Renderer (i doubt anybody recognized, but it looks more right now)
<br/>
-btw. figured out a 4th parameter for Painting, its kind of a mode

<h4>Changes v1.4.1:</h4><br/>
-i fixed some things regarding the new Raw Editor of 1.4
<br/>
-the exit area markers are a bit thinner now so selecting entrances will be easier in some cases
<br/>
-the bowser puzzle renderer wont crash anymore when setting the piece index too high
