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

using OpenTK;

namespace SM64DSe
{
    public enum RenderMode
    {
        Opaque = 1,
        Translucent,
        Picking
    }

    public struct BoundingBox
    {
        public Vector3 m_Min, m_Max;

        /*public BoundingBox()
        {
            m_Min = m_Max = Vector3.Zero;
        }*/
    }
}
