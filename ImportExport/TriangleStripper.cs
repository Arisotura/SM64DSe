/* Attempts to convert a list of separate triangles to triangle strips wherever possible.
 * 
 * Based on the SGI / "tomesh" algorithm with some modifications.
 * 
 * See comments for details.
 *  
 *   Triangle Strips   
 *    v2___v4____v6
 *    /|\  |\    /\     
 * v0( | \ | \  /  \    
 *    \|__\|__\/____\   
 *    v1   v3  v5   v7
 * 
 * The vertices are normally arranged anti-clockwise, except that: in triangle-strips each second polygon uses clockwise arranged vertices.
 * 
 * Edge AB: v0 - v1
 * Edge BC: v1 - v2
 * Edge CA: v2 - v0
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM64DSe.ImportExport
{
    public class TriangleStripper
    {
        List<VertexLinked> m_Vertices;
        List<TriangleLinked> m_Triangles;

        List<TriangleLinked> m_TrianglesToProcess;

        public TriangleStripper(ModelBase.FaceListDef faceList)
        {
            if (faceList.m_Type != ModelBase.PolyListType.Triangles)
            {
                bool tris = true;
                for (int i = 0; i < faceList.m_Faces.Count; i++)
                {
                    tris = (faceList.m_Faces[i].m_NumVertices == 3);
                    if (!tris)
                        throw new ArgumentException("The provided FaceListDef must be triangulated.");
                }
                faceList.m_Type = ModelBase.PolyListType.Triangles;
            }

            m_Vertices = new List<VertexLinked>();
            m_Triangles = new List<TriangleLinked>();
            m_TrianglesToProcess = new List<TriangleLinked>();

            for (int i = 0; i < faceList.m_Faces.Count; i++)
            {
                if (IsDegenerateFace(faceList.m_Faces[i]))
                {
                    faceList.m_Faces.RemoveAt(i);
                }
            }

            for (int i = 0; i < faceList.m_Faces.Count; i++)
            {
                m_Triangles.Add(new TriangleLinked(faceList.m_Faces[i]));
            }

            for (int i = 0; i < m_Triangles.Count; i++)
            {
                ModelBase.FaceDef triangle = m_Triangles[i].m_Triangle;

                for (int j = 0; j < triangle.m_NumVertices; j++)
                {
                    VertexLinked vertex = new VertexLinked(triangle.m_Vertices[j]);

                    int index = m_Vertices.IndexOf(vertex);
                    if (index == -1)
                    {
                        m_Vertices.Add(vertex);
                        index = m_Vertices.Count - 1;
                    }

                    m_Vertices[index].m_LinkedTriangles.Add(i);
                }
            }
        }

        public List<ModelBase.FaceListDef> Stripify(bool keepVertexOrderDuringStripping = false)
        {
            for (int i = 0; i < m_Triangles.Count; i++)
            {
                ModelBase.FaceDef triangle = m_Triangles[i].m_Triangle;

                List<int> vertexALinked = GetVertex(triangle.m_Vertices[0]).m_LinkedTriangles;
                List<int> vertexBLinked = GetVertex(triangle.m_Vertices[1]).m_LinkedTriangles;
                List<int> vertexCLinked = GetVertex(triangle.m_Vertices[2]).m_LinkedTriangles;

                // Shares edge AB if both triangles reference vertices A and B
                var edgeAB = vertexALinked.Intersect(vertexBLinked).Except(new int[] { i });
                var edgeBC = vertexBLinked.Intersect(vertexCLinked).Except(new int[] { i });
                var edgeCA = vertexCLinked.Intersect(vertexALinked).Except(new int[] { i });

                if (edgeAB.Count() > 0)
                    m_Triangles[i].m_LinkedTriangles[(int)TriangleEdge.Edge_AB].AddRange(edgeAB);
                if (edgeBC.Count() > 0)
                    m_Triangles[i].m_LinkedTriangles[(int)TriangleEdge.Edge_BC].AddRange(edgeBC);
                if (edgeCA.Count() > 0)
                    m_Triangles[i].m_LinkedTriangles[(int)TriangleEdge.Edge_CA].AddRange(edgeCA);
            }

            m_TrianglesToProcess.AddRange(m_Triangles);

            // Sort by number of neighbours
            if (!keepVertexOrderDuringStripping)
            {
                m_TrianglesToProcess.Sort((a, b) => GetNumLinked(a).CompareTo(GetNumLinked(b)));
            }

            List<ModelBase.FaceListDef> tStrips = new List<ModelBase.FaceListDef>();

            // For storing triangles that have no neighbours or don't end up in a strip
            ModelBase.FaceListDef separateTriangles = new ModelBase.FaceListDef(ModelBase.PolyListType.Triangles);

            TriangleLinked currentTriangle = null;

            while (m_TrianglesToProcess.Count > 0)
            {
                while (true)
                {
                    if (m_TrianglesToProcess.Count <= 0)
                    {
                        currentTriangle = null;
                        break;
                    }

                    currentTriangle = m_TrianglesToProcess[0];
                    m_TrianglesToProcess.RemoveAt(0);
                    if (currentTriangle.m_Marked)
                        continue;
                    else
                        break;
                }
                if (currentTriangle == null) break;

                List<TriangleLinked>[] linked = GetLinked(currentTriangle);
                int numLinked = GetNumLinked(currentTriangle);

                currentTriangle.m_Marked = true;

                if (numLinked == 0)
                {
                    separateTriangles.m_Faces.Add(currentTriangle.m_Triangle);
                    continue;
                }

                // For each face build a strip off each of its edges and keep the longest one, discarding the 
                // rest. Not part of SGI.
                Tuple<ModelBase.FaceListDef, List<int>> tStripAB = 
                    GetStripAndIndicesForStartingEvenEdge(currentTriangle, TriangleEdge.Edge_AB);
                Tuple<ModelBase.FaceListDef, List<int>> tStripBC =
                    GetStripAndIndicesForStartingEvenEdge(currentTriangle, TriangleEdge.Edge_BC);
                Tuple<ModelBase.FaceListDef, List<int>> tStripCA =
                    GetStripAndIndicesForStartingEvenEdge(currentTriangle, TriangleEdge.Edge_CA);

                List<Tuple<ModelBase.FaceListDef, List<int>>> candidates = new List<Tuple<ModelBase.FaceListDef, List<int>>>() 
                    { tStripAB, tStripBC, tStripCA };

                List<int> stripLengths = new List<int>();
                foreach (Tuple<ModelBase.FaceListDef, List<int>> strip in candidates)
                    stripLengths.Add(strip.Item1.m_Faces.Count);
                int longestStripIndex = stripLengths.IndexOf(stripLengths.Max());

                Tuple<ModelBase.FaceListDef, List<int>> longestStrip = candidates[longestStripIndex];

                if (longestStrip.Item1.m_Faces.Count == 0)
                {
                    separateTriangles.m_Faces.Add(currentTriangle.m_Triangle);
                    continue;
                }

                foreach (int tri in longestStrip.Item2)
                    m_Triangles[tri].m_Marked = true;

                tStrips.Add(longestStrip.Item1);
            }

            if (separateTriangles.m_Faces.Count > 0) tStrips.Add(separateTriangles);

            return tStrips;
        }

        Tuple<ModelBase.FaceListDef, List<int>> GetStripAndIndicesForStartingEvenEdge(
            TriangleLinked start, TriangleEdge startForwardEdge)
        {
            List<int> stripIndices = new List<int>();
            List<TriangleRotation> stripRotations = new List<TriangleRotation>();

            List<TriangleLinked> linked = GetLinked(start)[(int)startForwardEdge];
            TriangleLinked bestNeighbour = DetermineBestNextNeighbour(start, linked, startForwardEdge);

            if (bestNeighbour == null) 
                return new Tuple<ModelBase.FaceListDef, List<int>>(new ModelBase.FaceListDef(), new List<int>());

            TriangleRotation startRotation = (TriangleRotation)((int)(startForwardEdge - TriangleEdge.Edge_BC + 3) % 3);

            TriangleLinked t = start;
            TriangleEdge currentEdge = startForwardEdge;
            TriangleRotation currentRotation = startRotation;
            bool even = true;
            int index_t = -1;
            while (t != null && !stripIndices.Contains((index_t = m_Triangles.IndexOf(t))))
            {
                stripIndices.Add(index_t);
                stripRotations.Add(currentRotation);

                linked = GetLinked(t)[(int)currentEdge];
                bestNeighbour = DetermineBestNextNeighbour(t, linked, currentEdge);

                t = bestNeighbour;

                even = !even;

                if (t != null)
                {
                    // Determine rotation and the edge to be used to get the next face

                    ModelBase.FaceDef triangleC_CW = new ModelBase.FaceDef(3);
                    if (even)
                    {
                        triangleC_CW.m_Vertices[0] = t.m_Triangle.m_Vertices[0];
                        triangleC_CW.m_Vertices[1] = t.m_Triangle.m_Vertices[1];
                        triangleC_CW.m_Vertices[2] = t.m_Triangle.m_Vertices[2];
                    }
                    else
                    {
                        triangleC_CW.m_Vertices[0] = t.m_Triangle.m_Vertices[2];
                        triangleC_CW.m_Vertices[1] = t.m_Triangle.m_Vertices[1];
                        triangleC_CW.m_Vertices[2] = t.m_Triangle.m_Vertices[0];
                    }

                    // The edge of the vertices which match the preceding triangle's
                    TriangleEdge linkBackEdge = TriangleEdge.Edge_AB;
                    // The vertices which match the preceding triangle's
                    ModelBase.VertexDef[] currentMatchedEdge = new ModelBase.VertexDef[2];
                    TriangleLinked previous = m_Triangles[stripIndices[stripIndices.Count - 1]];
                    currentMatchedEdge[0] = previous.m_Triangle.m_Vertices[(int)(currentEdge + 0) % 3];
                    currentMatchedEdge[1] = previous.m_Triangle.m_Vertices[(int)(currentEdge + 1) % 3];
                    // Find the edge in the current triangle which if odd has been made CW which matches 
                    // that from the preceding triangle. This will be set as the current triangle's first, 
                    // or 'AB' edge and the next edge (next two vertices) will be used to match the next 
                    // triangle.
                    for (int i = 0; i < 3; i++)
                    {
                        ModelBase.VertexDef[] edge = new ModelBase.VertexDef[2];
                        edge[0] = triangleC_CW.m_Vertices[(i + 0) % 3];
                        edge[1] = triangleC_CW.m_Vertices[(i + 1) % 3];
                        if (edge.Except(currentMatchedEdge).Count() == 0)
                        {
                            linkBackEdge = (TriangleEdge)i;
                            break;
                        }
                    }

                    TriangleEdge nextEdgeNoC_CW = (TriangleEdge)((int)(linkBackEdge + 1) % 3);

                    TriangleEdge nextEdge = nextEdgeNoC_CW;
                    if (!even)
                    {
                        // If odd, nextEdgeNoC_CW points to the edge to be used if written CW, however 
                        // all triangles have been read in as CCW so need to get the corresponding edge 
                        // in CCW version.
                        ModelBase.VertexDef[] nextEdgeNoC_CW_Vertices = new ModelBase.VertexDef[2];
                        nextEdgeNoC_CW_Vertices[0] = triangleC_CW.m_Vertices[(int)(nextEdgeNoC_CW + 0) % 3];
                        nextEdgeNoC_CW_Vertices[1] = triangleC_CW.m_Vertices[(int)(nextEdgeNoC_CW + 1) % 3];
                        for (int i = 0; i < 3; i++)
                        {
                            ModelBase.VertexDef[] ccwEdge = new ModelBase.VertexDef[2];
                            ccwEdge[0] = t.m_Triangle.m_Vertices[(i + 0) % 3];
                            ccwEdge[1] = t.m_Triangle.m_Vertices[(i + 1) % 3];
                            if (nextEdgeNoC_CW_Vertices.Except(ccwEdge).Count() == 0)
                            {
                                nextEdge = (TriangleEdge)i;
                                break;
                            }
                        }
                    }

                    // Now we need to determine the required rotation of the current triangle so that for 
                    // even triangles the new vertex in at index 0 and for odd triangles it occurs at 
                    // index 2.
                    ModelBase.VertexDef uniqueVertex = t.m_Triangle.m_Vertices.Except(previous.m_Triangle.m_Vertices).ElementAt(0);
                    int uniqueVertexIndex = Array.IndexOf(t.m_Triangle.m_Vertices, uniqueVertex);
                    TriangleRotation requiredRotation =
                        (even) ? (TriangleRotation)((uniqueVertexIndex - 2 + 3) % 3) : 
                        (TriangleRotation)(uniqueVertexIndex);

                    currentRotation = requiredRotation;
                    currentEdge = nextEdge;

                    // To best understand how this works, debug and step-through how the following model is handled:
                    //
                    // An example:
                    // Faces as defined in model (all Counter-Clockwise (CCW)):
                    // f 1 2 3 
                    // f 4 1 3
                    // f 4 5 1
                    // Build strip from edge CA. 
                    // # 2 3 1 (LS)      <- Need to Left Shift vertices so that CA is the second edge (in a tri. strip it's 
                    //                      always the second edge that's shared - see diagram at top).
                    // #   3 1 4 (4 1 3) <- For odd faces the new vertex must be at index [0]
                    //                      No Rot required, link-back CW: AB, CW forward: AB + 1 = BC, 
                    //                      CCW forward: edge in CCW that contains vertices in (CW forward) = AB
                    //                      The next triangle is the one that shares the CCW edge AB (vertices 1 and 4)
                    // #     1 4 5 (RS)  <- Even face the new vertex needs to be in index [2] so need to Right Shift vertices
                    //                      Repeat steps as for above face but don't need to worry about converting between CCW and CW order 
                }

            }

            ModelBase.FaceListDef tStrip = new ModelBase.FaceListDef(ModelBase.PolyListType.TriangleStrip);

            for (int i = 0; i < stripIndices.Count; i++)
            {
                TriangleRotation requiredRotation = (TriangleRotation)stripRotations[i];

                ModelBase.FaceDef rotated = new ModelBase.FaceDef(3);
                rotated.m_Vertices[0] = m_Triangles[stripIndices[i]].m_Triangle.m_Vertices[((int)(0 + requiredRotation) % 3)];
                rotated.m_Vertices[1] = m_Triangles[stripIndices[i]].m_Triangle.m_Vertices[((int)(1 + requiredRotation) % 3)];
                rotated.m_Vertices[2] = m_Triangles[stripIndices[i]].m_Triangle.m_Vertices[((int)(2 + requiredRotation) % 3)];

                tStrip.m_Faces.Add(rotated);
            }

            return new Tuple<ModelBase.FaceListDef, List<int>>(tStrip, stripIndices);
        }

        private TriangleLinked DetermineBestNextNeighbour(TriangleLinked triangle, List<TriangleLinked> neighbours, TriangleEdge edge, 
            bool tieBreak = false)
        {
            if (neighbours.Count == 0) return null;

            if (neighbours.Count == 1) return neighbours[0];

            neighbours.Sort((a, b) => GetNumLinked(a).CompareTo(GetNumLinked(b)));

            TriangleLinked[] twoTop = new TriangleLinked[2];
            twoTop[0] = neighbours[0]; twoTop[1] = neighbours[1];

            int[] numLinkedOfNeighbours = new int[] { GetNumLinked(twoTop[0]), GetNumLinked(twoTop[1]) };

            if (numLinkedOfNeighbours[0] < numLinkedOfNeighbours[1])
            {
                return twoTop[0];
            }
            else if (numLinkedOfNeighbours[1] < numLinkedOfNeighbours[0])
            {
                return twoTop[1];
            }
            else if (!tieBreak)
            {
                List<TriangleLinked> firstLinked = GetLinked(twoTop[0])[(int)edge];
                List<TriangleLinked> secondLinked = GetLinked(twoTop[1])[(int)edge];

                TriangleLinked firstBest = DetermineBestNextNeighbour(twoTop[0], firstLinked, edge, true);
                TriangleLinked secondBest = DetermineBestNextNeighbour(twoTop[1], secondLinked, edge, true);

                if (firstBest == null) { return twoTop[1]; }
                else if (secondBest == null) { return twoTop[0]; }
                else if (GetNumLinked(firstBest) < GetNumLinked(secondBest)) { return twoTop[0]; }
                else { return twoTop[1]; }
            }
            else if (tieBreak)
            {
                return twoTop[0];
            }
            else
            {
                return null;
            }
        }

        private List<TriangleLinked>[] GetLinked(TriangleLinked triangle)
        {
            List<TriangleLinked>[] linked = new List<TriangleLinked>[3];

            for (int i = 0; i < 3; i++)
            {
                List<int> linkedIndices = triangle.m_LinkedTriangles[i];
                for (int j = 0; j < linkedIndices.Count; j++)
                {
                    // Make sure it doesn't count itself as a neighbour
                    if (m_Triangles[m_Triangles.IndexOf(triangle)].Equals(linkedIndices[j]))
                    {
                        linkedIndices.RemoveAt(j);
                        continue;
                    }
                    // Ignore marked triangles (already in a strip)
                    if (m_Triangles[linkedIndices[j]].m_Marked)
                    {
                        linkedIndices.RemoveAt(j);
                        continue;
                    }
                    // Ignore triangles which share all 3 vertices
                    if (m_Triangles[linkedIndices[j]].m_Triangle.m_Vertices.Except(triangle.m_Triangle.m_Vertices).Count() == 0)
                    {
                        linkedIndices.RemoveAt(j);
                        continue;
                    }
                }
                linked[i] = new List<TriangleLinked>();
                for (int j = 0; j < linkedIndices.Count; j++)
                {
                    linked[i].Add(m_Triangles[linkedIndices[j]]);
                }
            }

            return linked;
        }

        private int GetNumLinked(TriangleLinked triangle)
        {
            List<TriangleLinked>[] linked = GetLinked(triangle);
            int count = 0;
            for (int i = 0; i < 3; i++)
            {
                count += linked[i].Count;
            }
            return count;
        }

        // Return true if two of a triangle's vertices are the same
        private bool IsDegenerateFace(ModelBase.FaceDef triangle)
        {
            List<ModelBase.VertexDef> verts = triangle.m_Vertices.ToList();
            return (verts.Count() != verts.Distinct().Count());
        }

        private VertexLinked GetVertex(ModelBase.VertexDef vertexDef)
        {
            return m_Vertices[m_Vertices.IndexOf(new VertexLinked(vertexDef))];
        }

        class VertexLinked
        {
            // Holds a vertex and a list of the indices of the trianlges that reference it
            Tuple<ModelBase.VertexDef, List<int>> m_VertexLinkedTriangleIndices;

            public VertexLinked(ModelBase.VertexDef vertex)
            {
                m_VertexLinkedTriangleIndices = new Tuple<ModelBase.VertexDef,List<int>>(vertex, new List<int>());
            }

            public ModelBase.VertexDef m_Vertex
            {
                get
                {
                    return m_VertexLinkedTriangleIndices.Item1;
                }
            }

            public List<int> m_LinkedTriangles
            {
                get
                {
                    return m_VertexLinkedTriangleIndices.Item2;
                }
            }

            public override bool Equals(object obj)
            {
                var item = obj as VertexLinked;

                if (item == null) return false;

                return m_Vertex.Equals(item.m_Vertex);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 13;
                    hash = hash * 7 + m_Vertex.GetHashCode();
                    return hash;
                }
            }
        }

        class TriangleLinked
        {
            // Holds a triangle and the indices of its neighbours
            Tuple<ModelBase.FaceDef, List<int>[]> m_TriangleLinkedNeighbours;

            public bool m_Marked;

            public TriangleLinked(ModelBase.FaceDef triangle)
            {
                m_TriangleLinkedNeighbours = new Tuple<ModelBase.FaceDef, List<int>[]>(triangle, new List<int>[] { null, null, null });
                for (int i = 0; i < 3; i++)
                    m_LinkedTriangles[i] = new List<int>();
                m_Marked = false;
            }

            public ModelBase.FaceDef m_Triangle
            {
                get
                {
                    return m_TriangleLinkedNeighbours.Item1;
                }
            }

            public List<int>[] m_LinkedTriangles
            {
                get
                {
                    return m_TriangleLinkedNeighbours.Item2;
                }
            }

            public override bool Equals(object obj)
            {
                var item = obj as TriangleLinked;

                if (item == null) return false;

                return m_Triangle.Equals(item.m_Triangle);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 13;
                    hash = hash * 7 + m_Triangle.GetHashCode();
                    return hash;
                }
            }
        }

        enum TriangleEdge
        {
            Edge_AB = 0,
            Edge_BC = 1, 
            Edge_CA = 2
        };

        enum TriangleRotation
        {
            TRot_None = 0,
            TRot_LS = 1,
            TRot_LSLS = 2
        };
    }
}
