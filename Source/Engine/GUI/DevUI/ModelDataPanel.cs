using ImGuiNET;
using WinterEngine.Resource;

namespace WinterEngine.Gui.DevUI;

public class ModelDataPanel : ImGuiPanel
{
    Md3Model data;

    public ModelDataPanel(Md3Model mdata)
    {
        Title = "Model Data";
        Size = new Vector2(300, 300);
        Flags = ImGuiWindowFlags.NoSavedSettings;

        data = mdata;
    }

    public override void OnLayout()
    {
        if (ImGui.Begin("Model Info"))
        {
            ImGui.Text(data.Header.name);
            ImGui.Text($"{data.Header.version}");
            ImGui.Separator();
            ImGui.Text($"Frames: {data.Header.numFrames}");
            ImGui.Text($"Tags: {data.Header.numTags}");
            ImGui.Text($"Surfaces: {data.Header.numSurfaces}");
            ImGui.Text($"Skins: {data.Header.numSkins}");
            ImGui.Separator();
            // frames
            if (ImGui.CollapsingHeader("Frames"))
            {
                int count = 0;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                foreach (Md3Frame frame in data.Frames)
                {
                    if (ImGui.CollapsingHeader($"Frame {count} ({frame.name})##root_frames_frame_{count}"))
                    {
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Text($"Min Bounds: {frame.minBounds.X},{frame.minBounds.Y},{frame.minBounds.Z}");
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Text($"Max Bounds: {frame.maxBounds.X},{frame.maxBounds.Y},{frame.maxBounds.Z}");
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Text($"Local Origin: {frame.localOrigin.X},{frame.localOrigin.Y},{frame.localOrigin.Z}");
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Text($"Radius: {frame.radius}");
                    }
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                    count++;
                }
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 8);
            }

            if (ImGui.CollapsingHeader("Tags"))
            {
                int count = 0;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                foreach (Md3Tag tag in data.Tags)
                {
                    if (ImGui.CollapsingHeader($"Tag {count} ({tag.name})##root_tags_tag_{count}"))
                    {
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Text($"Origin: {tag.origin.X},{tag.origin.Y},{tag.origin.Z}");
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Text($"Axis 1: {tag.axis[0].X},{tag.axis[0].Y},{tag.axis[0].Z}");
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Text($"Axis 2: {tag.axis[1].X},{tag.axis[1].Y},{tag.axis[1].Z}");
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Text($"Axis 3: {tag.axis[2].X},{tag.axis[2].Y},{tag.axis[2].Z}");
                    }
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                    count++;
                }
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 8);
            }

            if (ImGui.CollapsingHeader("Surfaces"))
            {
                int count = 0;
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                foreach (Md3Surface surf in data.Surfaces)
                {
                    if (ImGui.CollapsingHeader($"Surface {count} ({surf.name})##root_surfs_surf_{count}"))
                    {
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Text($"Frames: {surf.numFrames}");
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Text($"Shaders: {surf.numShaders}");
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Text($"Verticies: {surf.numVerts}");
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Text($"Triangles: {surf.numTriangles}");
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                        ImGui.Separator();
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);

                        if (ImGui.CollapsingHeader($"Shaders##root_surf_{count}_shaders"))
                        {
                            int sCount = 0;
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
                            foreach (Md3Shader shader in surf.shaders)
                            {
                                if (ImGui.CollapsingHeader($"Shader {sCount}##root_surf_{count}_shaders_shader_{sCount}"))
                                {
                                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
                                    ImGui.Text(shader.name);
                                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
                                    ImGui.Text($"{shader.shaderIndex}");
                                }
                                sCount++;
                                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
                            }
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 16);
                        }
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);

                        if (ImGui.CollapsingHeader($"Triangles##root_surf_{count}_triangles"))
                        {
                            int sCount = 0;
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
                            foreach (Md3Triangle trig in surf.triangles)
                            {
                                ImGui.Text($"{sCount} | Indices: {trig.indexes[0]},{trig.indexes[1]},{trig.indexes[2]}");
                                sCount++;
                                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
                            }
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 8);
                        }
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);

                        if (ImGui.CollapsingHeader($"UVs##root_surf_{count}_uvs"))
                        {
                            int sCount = 0;
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
                            foreach (Vector2 uv in surf.texCoords)
                            {
                                ImGui.Text($"{sCount}: {uv.X},{uv.Y}");
                                sCount++;
                                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
                            }
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 8);
                        }
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);

                        if (ImGui.CollapsingHeader($"Vertices##root_surf_{count}_vertices"))
                        {
                            int sCount = 0;
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
                            for (int i = 0; i < (surf.numVerts * surf.numFrames); i += surf.numVerts)
                            {
                                if (ImGui.CollapsingHeader($"Frame {sCount}##root_surf_{count}_verts_vertframe_{sCount}"))
                                {
                                    int vCount = 0;
                                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
                                    for (int f = i; f < i + surf.numVerts; f++)
                                    {
                                        Md3Vertex vertex = surf.vertices[f];
                                        ImGui.Text($"{vCount} | Position: {vertex.x},{vertex.y},{vertex.z} Normal: {vertex.normal}");
                                        vCount++;
                                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
                                    }
                                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 16);
                                }
                                sCount++;
                                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16);
                            }
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 8);
                        }
                    }
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);
                    count++;
                }
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 8);
            }

            ImGui.End();
        }
    }
}
