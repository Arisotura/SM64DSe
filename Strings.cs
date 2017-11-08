/*
    Copyright 2012 Kuribo64

    This file is part of SM64DSe.

    SM64DSe is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.

    SM64DSe is distributed in the hope that it will be useful, but WITHOUT ANY 
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along 
    with SM64DSe. If not, see http://www.gnu.org/licenses/.
*/

namespace SM64DSe
{
    public static class Strings
    {
        public static string[] LevelNames = 
        {
            "General purpose test map",
	        "Castle grounds",
	        "Castle - 1st floor",
	        "Castle backyards",
	        "Castle - basement",
	        "Castle - 2nd floor",
	        "Bob-Omb Battlefield",
	        "Whomp's Fortress",
	        "Jolly Roger Bay",
	        "Jolly Roger Bay - ship",
	        "Cool, Cool Mountain",
	        "Cool, Cool Mountain - slider",
	        "Big Boo's Haunt",
	        "Hazy Maze Cave",
	        "Lethal Lava Land",
	        "Lethal Lava Land - volcano",
	        "Shifting Sand Land",
	        "Shifting Sand Land - pyramid",
	        "Dire, Dire Docks",
	        "Snowman's Land",
	        "Snowman's Land - Igloo",
	        "Wet-Dry World",
	        "Tall Tall Mountain",
	        "Tall Tall Mountain - slider",
	        "Tiny-Huge Island - huge",
	        "Tiny-Huge Island - tiny",
	        "Tiny-Huge Island - mountain",
	        "Tick Tock Clock",
	        "Rainbow Ride",
	        "The Princess' Secret Slide",
	        "The Secret Aquarium",
	        "[?] Switch",
	        "The Secret Under the Moat",
	        "Behind the Waterfall",
	        "Over the Rainbows",
	        "Bowser in the Dark World - map",
	        "Bowser in the Dark World - fight",
	        "Bowser in the Fire Sea - map",
	        "Bowser in the Fire Sea - fight",
	        "Bowser in the Sky - map",
	        "Bowser in the Sky - fight",
	        "Tox Box test map",
	        "The Secret of Battle Fort",
	        "Sunshine Isles",
	        "Goomboss Battle - map",
	        "Goomboss Battle - fight",
	        "Big Boo Battle - map",
	        "Big Boo Battle - fight",
	        "Chief Chilly Challenge - map",
	        "Chief Chilly Challenge - fight",
	        "Play Room",
	        "Castle grounds (multiplayer)"
        };

        public static string[] DoorTypes = 
        {
            "Virtual door", 
            "Standard door", 
            "Door with star (0)", 
            "Door with star (1) (WF)",                              
            "Door with star (3) (JRB)", 
            "Door with star (8) (Goomboss)", 
            "Locked door (castle basement)", 
            "Locked door (castle 2nd floor)",                              
            "Standard door", 
            "Bowser door (BitDW)", 
            "Bowser door (BitFS)", 
            "Bowser door (castle 2nd floor)",                 
            "Bowser door (BitS)", 
            "Door with star (1) (playroom)", 
            "Door with star (3) (CCM)", 
            "Old wooden door",                    
            "Rusted metal door", 
            "HMC door", 
            "BBH door", 
            "Character change (Mario)",
            "Character change (Luigi)", 
            "Character change (Wario)", 
            "Locked door (castle entrance)", 
            "Silent room door"
        };

        public static readonly string MODEL_FORMATS_FILTER = "All Supported Models|*.dae;*.imd;*.obj|" +
                "COLLADA DAE|*.dae|NITRO Intermediate Model Data|*.imd|Wavefront OBJ|*.obj";
        public static readonly string MODEL_ANIMATION_FORMATS_FILTER = "All Supported Animation Formats|*.dae;*.ica|" +
                "COLLADA DAE|*.dae|NITRO Intermediate Character Animation|*.ica";

        public static readonly string MODEL_EXPORT_FORMATS_FILTER = 
            "COLLADA DAE (.dae)|*.dae|Wavefront OBJ (.obj)|*.obj";

        public static readonly string IMAGE_EXPORT_PNG_FILTER = "PNG Image (.png)|*.png";

        public static readonly string FILTER_XML = "XML Document (.xml)|*.xml";

        public static string[] WHITESPACE = new string[] { " ", "\n", "\r\n", "\t" };

        public static bool IsBlank(string value)
        {
            return (value == null || value.Trim().Length < 1);
        }

        public static bool IsNotBlank(string value)
        {
            return !IsBlank(value);
        }
    }
}
